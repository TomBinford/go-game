using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Go
{
    class BoardState
    {
        private IntersectionState[,] state;
        public IntersectionState this[int x, int y]
        {
            get
            {
                return state[y, x];
            }
        }
        public IntersectionState this[Point point] => this[point.X, point.Y];

        public BoardState(int numLines)
        {
            state = new IntersectionState[numLines, numLines];
        }
    }
}
