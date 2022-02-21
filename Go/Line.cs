using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Draw(SpriteBatch spritebatch)
        {
            Vector2 delta = B - A;
            float distance = delta.Length();
            float heading = (float)Math.Atan2(delta.Y, delta.X) - MathHelper.PiOver2;
            Vector2 origin = new Vector2(0.5f, 0); //Rotate the line about one edge of the pixel
            spritebatch.Draw(Game1.Pixel, A, null, Color, heading, origin, new Vector2(Thickness, distance), SpriteEffects.None, 0);
        }
    }
}
