using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Go
{
    class Board : IDrawable
    {
        public Color Color;
        public Rectangle Bounds { get; }
        public int NumLines;
        private Line[] horizontalLines;
        private Line[] verticalLines;

        public Board(Rectangle bounds, int numLines, Color boardColor, Color lineColor, float lineThickness)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                throw new ArgumentException("Board dimensions are invalid");
            }

            Bounds = bounds;
            NumLines = numLines;
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

        public bool Contains(Point intersection)
        {
            return 0 <= intersection.X && intersection.X < NumLines && 0 <= intersection.Y && intersection.Y < NumLines;
        }

        public Point ClosestIntersection(Vector2 position)
        {
            int x = (int)Math.Round((position.X - Bounds.X) / (Bounds.Width / NumLines) - 0.5f);
            int y = (int)Math.Round((position.Y - Bounds.Y) / (Bounds.Height / NumLines) - 0.5f);
            return new Point(x, y);
        }

        public Vector2 IntersectionPosition(Point intersection)
        {
            float x = Bounds.X + (float)Bounds.Width / NumLines * (intersection.X + 0.5f);
            float y = Bounds.Y + (float)Bounds.Height / NumLines * (intersection.Y + 0.5f);
            return new Vector2(x, y);
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
