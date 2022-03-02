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

    partial class BoardState
    {
        public BoardState Previous { get; }
        private readonly Stone[,] state;
        public Stone this[int y, int x] => state[y, x];
        public Stone this[Point intersection] => this[intersection.Y, intersection.X]; //row, column
        public Player CurrentPlayer { get; }
        public bool WasPass { get; }
        public bool Terminal { get; }

        private BoardState(BoardState previous, Stone[,] state, Player player, bool wasPass = false, bool terminal = false)
        {
            Previous = previous;
            this.state = state;
            CurrentPlayer = player;
            WasPass = wasPass;
            Terminal = terminal;
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

        public bool Contains(Point intersection)
        {
            return 0 <= intersection.X && intersection.X < state.GetLength(1) && 0 <= intersection.Y && intersection.Y < state.GetLength(0);
        }
    }
}
