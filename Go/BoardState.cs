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

            //TODO: handle captures

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

        public List<StoneGroup> StoneGroups(bool includeEmpty)
        {
            Stone[,] state = this.state; //Reference to state to let the inner function work
            bool[,] visited = new bool[state.GetLength(0), state.GetLength(1)];

            void DFS(int row, int col, StoneGroup group)
            {
                if (row < 0 || row >= state.GetLength(0) || col < 0 || col >= state.GetLength(1))
                {
                    return;
                }
                if (!visited[row, col])
                {
                    if (state[row, col] == group.Stone)
                    {
                        visited[row, col] = true;
                        group.Intersections.Add(new Point(col, row)); //row is y, col is x
                        DFS(row - 1, col, group);
                        DFS(row + 1, col, group);
                        DFS(row, col - 1, group);
                        DFS(row, col + 1, group);
                    }
                }
            }

            List<StoneGroup> groups = new List<StoneGroup>();
            for (int row = state.GetLowerBound(0); row < state.GetUpperBound(0); row++)
            {
                for (int col = state.GetLowerBound(1); col < state.GetUpperBound(1); col++)
                {
                    if (includeEmpty || state[row, col] != Stone.Empty)
                    {
                        StoneGroup group = new StoneGroup(state[row, col], new List<Point>());
                        DFS(row, col, group);
                        if (group.Intersections.Count > 0)
                        {
                            groups.Add(group);
                        }
                    }
                }
            }
            return groups;
        }
    }
}
