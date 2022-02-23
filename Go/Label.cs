using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Go
{
    class Label : IDrawable
    {
        public SpriteFont Font;
        public string Text;
        public Vector2 Position;
        public Color Color;
        public Vector2 Scale;

        public Rectangle Bounds => new Rectangle(Position.ToPoint(), Font.MeasureString(Text).ToPoint());

        public Label(SpriteFont font, string text, Vector2 position, Color color, Vector2 scale)
        {
            Font = font;
            Text = text;
            Position = position;
            Color = color;
            Scale = scale;
        }

        public Label(SpriteFont font, string text, Vector2 position, Color color, float scale)
            : this(font, text, position, color, new Vector2(scale))
        { }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Text, Position, Color, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }

        public bool LeftClicked()
        {
            return Game1.State.LeftClick() && Bounds.Contains(Game1.State.CurrentMouseState.Position);
        }
    }
}
