using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

    class Board : IDrawable
    {
        public Color Color;
        public Rectangle Bounds { get; }
        private Line[] horizontalLines;
        private Line[] verticalLines;

        public Board(Rectangle bounds, int numLines, Color boardColor, Color lineColor, float lineThickness)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                throw new ArgumentException("Board dimensions are invalid");
            }

            Bounds = bounds;
            Color = boardColor;
            horizontalLines = new Line[numLines];
            verticalLines = new Line[numLines];

            float ySpacing = (float)bounds.Height / numLines;
            float y = bounds.Y + ySpacing / 2;
            for (int i = 0; i < numLines; i++)
            {
                Line line = new Line(new Vector2(bounds.Left, y), new Vector2(bounds.Right, y), lineColor, lineThickness);
                horizontalLines[i] = line;
                y += ySpacing;
            }

            float xSpacing = (float)bounds.Width / numLines;
            float x = bounds.X + xSpacing / 2;
            for (int i = 0; i < numLines; i++)
            {
                Line line = new Line(new Vector2(x, bounds.Top), new Vector2(x, bounds.Bottom), lineColor, lineThickness);
                verticalLines[i] = line;
                x += xSpacing;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Game1.Pixel, Bounds, Color);
            foreach (Line line in horizontalLines)
            {
                spriteBatch.Draw(line);
            }
            foreach (Line line in verticalLines)
            {
                spriteBatch.Draw(line);
            }
        }
    }
}
