using Microsoft.Xna.Framework.Graphics;

namespace Go
{
    interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch);
    }

    static class IDrawableExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, IDrawable drawable)
        {
            drawable.Draw(spriteBatch);
        }
    }
}