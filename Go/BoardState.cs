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

    struct BoardState
    {
        private IntersectionState[,] state;
        public IntersectionState this[int x, int y] => state[y, x]; //row, column
        public IntersectionState this[Point point] => this[point.X, point.Y];

        public BoardState(int numLines)
        {
            state = new IntersectionState[numLines, numLines];
        }
    }
}
