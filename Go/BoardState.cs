using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Go
{
    enum Stone
    {
        Empty,
        White,
        Black
    }

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
            //state[4, 2] = IntersectionState.Black;
            //state[1, 4] = IntersectionState.Black;
            //state[6, 2] = IntersectionState.Black;
            //state[1, 2] = IntersectionState.White;
            //state[6, 5] = IntersectionState.White;
            //state[3, 3] = IntersectionState.White;
        }

        public override bool Equals(object obj)
        {
            if (obj is BoardState other)
            {
                if (CurrentPlayer != other.CurrentPlayer)
                {
                    return false;
                }
                return CurrentPlayer == other.CurrentPlayer &&
                    state.Cast<Stone>().SequenceEqual(other.state.Cast<Stone>());
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
            if(play is PassPlay)
            {
                Stone[,] newState = new Stone[state.GetLength(0), state.GetLength(1)];
                Array.Copy(state, newState, state.Length);
                Player nextPlayer = CurrentPlayer == Player.Black ? Player.White : Player.Black;
                return new BoardState(new BoardStateNode(this), newState, nextPlayer);
            }
            else if(play is MovePlay move)
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
            Stone[,] newState = new Stone[state.GetLength(0), state.GetLength(1)];
            Array.Copy(state, newState, state.Length);
            newState[intersection.Y, intersection.X] = (Stone)CurrentPlayer;
            Player nextPlayer = CurrentPlayer == Player.Black ? Player.White : Player.Black;
            BoardState newBoardState = new BoardState(new BoardStateNode(this), newState, nextPlayer);

            //TODO: handle captures

            for (BoardStateNode node = newBoardState.Previous; node != null; node = node.Previous)
            {
                //repetition of a previous state is not allowed
                if (node.State.CurrentPlayer == CurrentPlayer)
                {
                    if (Enumerable.SequenceEqual(
                        newBoardState.state.Cast<Stone>(),
                        node.State.state.Cast<Stone>()))
                    {
                        return null;
                    }
                }
            }
            return newBoardState;
        }
    }
}
