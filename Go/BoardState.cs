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

    partial struct BoardState
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
    }
}
