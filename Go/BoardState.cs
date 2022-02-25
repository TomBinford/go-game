using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Go
{
    enum Player
    {
        None,
        White = Stone.White,
        Black = Stone.Black
    }

    class BoardStateNode
    {
        public BoardState State { get; }
        public BoardStateNode Previous { get; }

        public BoardStateNode(BoardState state)
        {
            State = state;
            Previous = state.Previous;
        }
    }

    struct BoardState
    {
        public BoardStateNode Previous { get; }
        private readonly Stone[,] state;
        public Stone this[int x, int y] => state[y, x]; //row, column
        public Stone this[Point intersection] => this[intersection.X, intersection.Y];

        public Player CurrentPlayer { get; }

        private BoardState(BoardStateNode previous, Stone[,] state, Player player)
        {
            Previous = previous;
            this.state = state;
            CurrentPlayer = player;
        }

        public BoardState(int numLines, Player firstPlayer)
            : this(null, new Stone[numLines, numLines], firstPlayer)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj is BoardState other)
            {
                if (CurrentPlayer != other.CurrentPlayer)
                {
                    return false;
                }
                return CurrentPlayer == other.CurrentPlayer && state.Equals(other.state);
            }
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(CurrentPlayer, state);

        public static bool operator ==(BoardState a, BoardState b) => a.Equals(b);
        public static bool operator !=(BoardState a, BoardState b) => !a.Equals(b);

        public bool Contains(Point intersection)
        {
            return 0 <= intersection.X && intersection.X < state.GetLength(1) && 0 <= intersection.Y && intersection.Y < state.GetLength(0);
        }

        public BoardState? MakePlay(Play play)
        {
            if (play is PassPlay)
            {
                Player nextPlayer = CurrentPlayer == Player.Black ? Player.White : Player.Black;
                return new BoardState(new BoardStateNode(this), state.DeepCopy(), nextPlayer);
            }
            else if (play is MovePlay move)
            {
                return MakeMove(move);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private BoardState? MakeMove(MovePlay move)
        {
            Point intersection = move.Intersection;
            if (!Contains(intersection) || this[intersection] != Stone.Empty)
            {
                return null;
            }

            Stone[,] newState = state.DeepCopy();
            newState[intersection.Y, intersection.X] = (Stone)CurrentPlayer;
            Player nextPlayer = CurrentPlayer == Player.Black ? Player.White : Player.Black;
            BoardState newBoardState = new BoardState(new BoardStateNode(this), newState, nextPlayer);

            //Remove stones of the opponent's color that do not have liberties (they are surrounded)
            foreach (StoneGroup group in newBoardState.StoneGroups((Stone)nextPlayer))
            {
                //Only have to check one of the intersections because
                //connected stones either all have liberties or all don't
                if (!newBoardState.HasLiberties(group.Intersections[0]))
                {
                    foreach (Point i in group.Intersections)
                    {
                        newState[i.Y, i.X] = Stone.Empty;
                    }
                }
            }

            //Self-capture (removing liberties from one's own pieces) is prohibited
            //This can only be checked after the opponent's stones have been removed.
            foreach (StoneGroup group in newBoardState.StoneGroups((Stone)CurrentPlayer))
            {
                if (!newBoardState.HasLiberties(group.Intersections[0]))
                {
                    return null;
                }
            }

            for (BoardStateNode node = newBoardState.Previous; node != null; node = node.Previous)
            {
                //repetition of a previous state is not allowed
                if (node.State.Equals(newState))
                {
                    return null;
                }
            }
            return newBoardState;
        }

        public List<StoneGroup> StoneGroups(params Stone[] wanted)
        {
            BoardState @this = this; //Make a copy because DFS can't capture this
            bool[,] visited = new bool[state.GetLength(0), state.GetLength(1)];

            void DFS(Point intersection, StoneGroup group)
            {
                if (!visited[intersection.Y, intersection.X] && @this[intersection] == group.Stone)
                {
                    visited[intersection.Y, intersection.X] = true;
                    group.Intersections.Add(intersection);
                    foreach (Point adj in @this.Adjacencies(intersection))
                    {
                        DFS(adj, group);
                    }
                }
            }

            List<StoneGroup> groups = new List<StoneGroup>();
            for (int row = 0; row < state.GetLength(0); row++)
            {
                for (int col = 0; col < state.GetLength(1); col++)
                {
                    Point intersection = new Point(col, row);
                    if (wanted.Contains(this[intersection]))
                    {
                        StoneGroup group = new StoneGroup(this[intersection], new List<Point>());
                        DFS(intersection, group);
                        if (group.Intersections.Any())
                        {
                            groups.Add(group);
                        }
                    }
                }
            }
            return groups;
        }

        //A stone has a liberty if it is:
        // -adjacent to an empty intersection
        // -or connected to a stone with a liberty
        public bool HasLiberties(Point intersection)
        {
            Stone stone = this[intersection];
            if (stone == Stone.Empty)
            {
                return true;
            }

            BoardState @this = this; //Make a copy because DFS can't capture this
            bool[,] visited = new bool[state.GetLength(0), state.GetLength(1)];
            bool DFS(Point intersection)
            {
                if (visited[intersection.Y, intersection.X])
                {
                    return false;
                }
                if (@this.Adjacencies(intersection).Any(i => @this[i] == Stone.Empty))
                {
                    return true;
                }
                visited[intersection.Y, intersection.X] = true;
                return @this.Adjacencies(intersection).Where(i => @this[i] == stone).Any(DFS);
            }

            return DFS(intersection);
        }

        private List<Point> Adjacencies(Point intersection)
        {
            return new List<Point>() {
            new Point(intersection.X - 1, intersection.Y),
            new Point(intersection.X + 1, intersection.Y),
            new Point(intersection.X, intersection.Y - 1),
            new Point(intersection.X, intersection.Y + 1)
            }.Where(Contains).ToList();
        }
    }
}
