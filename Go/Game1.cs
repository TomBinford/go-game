using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Go
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private static Texture2D _pixel;
        public static Texture2D Pixel => _pixel;

        public static GameState State;
        Line line;

        static Game1()
        {
            State = new GameState();
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferMultiSampling = true; //Enable anti-aliasing
            _pixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
            IsMouseVisible = true;

            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            line = new Line(new Vector2(100, 100), new Vector2(50, 200), Color.Black, 1);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            State.Update();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            // TODO: Add your drawing code here
            spriteBatch.Draw(line);
            spriteBatch.Draw(Pixel, line.A, null, Color.Red, 0, new Vector2(0.5f), 5f, SpriteEffects.None, 0);
            spriteBatch.Draw(Pixel, line.B, null, Color.Green, 0, new Vector2(0.5f), 5f, SpriteEffects.None, 0);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
