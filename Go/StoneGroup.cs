using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Go
{
    class StoneGroup
    {
        public Stone Stone;
        public List<Point> Intersections;

        public StoneGroup(Stone stone, List<Point> intersections)
        {
            Stone = stone;
            Intersections = intersections;
        }
    }
}
