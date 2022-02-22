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
        Board board;

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
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.Viewport.Width * 2;
            graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.Viewport.Height * 2;

            _pixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            board = new Board(new Rectangle(50, 50, 9 * 80, 9 * 80), 9, Color.BurlyWood, Color.Black, 2);
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
            if (State.LeftClick())
            {
                Point intersection = board.ClosestIntersection(State.CurrentMouseState.Position.ToVector2());
                board.State = board.State.MakePlay(intersection) ?? board.State;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            // TODO: Add your drawing code here
            spriteBatch.Draw(board);
            Point closestIntersection = board.ClosestIntersection(State.CurrentMouseState.Position.ToVector2());
            if (board.Contains(closestIntersection))
            {
                spriteBatch.Draw(Pixel, board.IntersectionPosition(closestIntersection), null, Color.Red, 0f, new Vector2(0.5f), 10f, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
