using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Go
{
    class Line : IDrawable
    {
        public Vector2 A;
        public Vector2 B;
        public Color Color;
        public float Thickness;

        public Line(Vector2 a, Vector2 b, Color color, float thickness)
        {
            A = a;
            B = b;
            Color = color;
            Thickness = thickness;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 delta = B - A;
            float distance = delta.Length();
            float heading = (float)Math.Atan2(delta.Y, delta.X);
            Vector2 origin = new Vector2(0, 0.5f); //Rotate the line about one edge of the pixel
            spriteBatch.Draw(Game1.Pixel, A, null, Color, heading, origin, new Vector2(distance, Thickness), SpriteEffects.None, 0);
        }
    }
}
