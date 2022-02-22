using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Go
{
    enum IntersectionState
    {
        Empty,
        White,
        Black
    }

    enum Player
    {
        White = IntersectionState.White,
        Black = IntersectionState.Black
    }

    struct BoardState
    {
        private readonly IntersectionState[,] state;
        public IntersectionState this[int x, int y] => state[y, x]; //row, column
        public IntersectionState this[Point intersection] => this[intersection.X, intersection.Y];

        public Player CurrentPlayer { get; private set; }

        private BoardState(IntersectionState[,] state, Player player)
        {
            this.state = state;
            CurrentPlayer = player;
        }

        public BoardState(int numLines, Player firstPlayer)
            : this(new IntersectionState[numLines, numLines], firstPlayer)
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
                    state.Cast<IntersectionState>().SequenceEqual(other.state.Cast<IntersectionState>());
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CurrentPlayer, state);
        }

        public static bool operator ==(BoardState a, BoardState b) => a.Equals(b);
        public static bool operator !=(BoardState a, BoardState b) => !a.Equals(b);

        public bool Contains(Point intersection)
        {
            return 0 <= intersection.X && intersection.X < state.GetLength(1) && 0 <= intersection.Y && intersection.Y < state.GetLength(0);
        }

        public BoardState? MakePlay(Point intersection)
        {
            if (!Contains(intersection) || this[intersection] != IntersectionState.Empty)
            {
                return null;
            }
            IntersectionState[,] newState = new IntersectionState[state.GetLength(0), state.GetLength(1)];
            Array.Copy(state, newState, state.Length);
            newState[intersection.Y, intersection.X] = (IntersectionState)CurrentPlayer;
            Player nextPlayer = CurrentPlayer == Player.Black ? Player.White : Player.Black;
            return new BoardState(newState, nextPlayer);
        }
    }
}
