using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Go
{
    abstract class Play
    {
        public static MovePlay Move(Point intersection)
        {
            return new MovePlay(intersection);
        }

        public static PassPlay Pass()
        {
            return new PassPlay();
        }
    }

    class MovePlay : Play
    {
        public Point Intersection { get; }

        public MovePlay(Point intersection)
        {
            Intersection = intersection;
        }
    }

    class PassPlay : Play
    {
    }
}
