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
        public int NumLines { get; }
        public BoardState State;
        private Line[] horizontalLines { get; }
        private Line[] verticalLines { get; }

        public Board(Rectangle bounds, int numLines, Color boardColor, Color lineColor, float lineThickness)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0)
            {
                throw new ArgumentException("Board dimensions are invalid");
            }

            NumLines = numLines;
            State = new BoardState(numLines, firstPlayer: Player.Black);

            Bounds = bounds;
            Color = boardColor;
            horizontalLines = new Line[numLines];
            verticalLines = new Line[numLines];

            float ySpacing = (float)bounds.Height / numLines;
            Vector2 left = new Vector2(bounds.Left, bounds.Y + ySpacing / 2);
            Vector2 right = new Vector2(bounds.Right, bounds.Y + ySpacing / 2);
            for (int i = 0; i < numLines; i++)
            {
                Line line = new Line(left, right, lineColor, lineThickness);
                horizontalLines[i] = line;
                left.Y += ySpacing;
                right.Y += ySpacing;
            }

            float xSpacing = (float)bounds.Width / numLines;
            Vector2 top = new Vector2(bounds.X + xSpacing / 2, bounds.Top);
            Vector2 bottom = new Vector2(bounds.X + xSpacing / 2, bounds.Bottom);
            for (int i = 0; i < numLines; i++)
            {
                Line line = new Line(top, bottom, lineColor, lineThickness);
                verticalLines[i] = line;
                top.X += xSpacing;
                bottom.X += xSpacing;
            }
        }

        public bool Contains(Point intersection) => State.Contains(intersection);

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
        public Vector2 IntersectionPosition(int x, int y) => IntersectionPosition(new Point(x, y));

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

            for (int x = 0; x < NumLines; x++)
            {
                for (int y = 0; y < NumLines; y++)
                {
                    if (State[y, x] != Stone.Empty)
                    {
                        Color stoneColor = State[y, x] == Stone.White ? Color.White : Color.Black;
                        spriteBatch.Draw(Game1.Pixel, IntersectionPosition(x, y), null, stoneColor, 0f, new Vector2(0.5f), 10f, SpriteEffects.None, 0);
                    }
                }
            }
        }
    }
}
