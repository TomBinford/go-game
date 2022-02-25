using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Go
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static GameState State;

        private static Texture2D _pixel;
        public static Texture2D Pixel => _pixel;

        SpriteFont font;

        Board board;
        Label playerTurnLabel;
        Label passLabel;

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
            font = Content.Load<SpriteFont>("Font");
            board = new Board(new Rectangle(50, 50, 9 * 80, 9 * 80), 9, Color.BurlyWood, Color.Black, 2);
            playerTurnLabel = new Label(font, "Player to move", new Vector2(board.Bounds.Right + 50, board.Bounds.Top), Color.Pink, 1.5f);
            passLabel = new Label(font, "Pass", new Vector2(board.Bounds.Right + 50, board.Bounds.Bottom - 50), Color.Cornsilk, 1f);
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
            if (passLabel.LeftClicked())
            {
                board.State = board.State.MakePlay(Play.Pass()) ?? board.State;
            }
            else if (State.LeftClick())
            {
                Point intersection = board.ClosestIntersection(State.CurrentMouseState.Position.ToVector2());
                BoardState? nextState = board.State.MakePlay(Play.Move(intersection));
                if (nextState.HasValue)
                {
                    board.State = nextState.Value;
                }
            }

            playerTurnLabel.Text = $"{board.State.CurrentPlayer} to move";
            playerTurnLabel.Text += $"\n{board.State.StoneGroups(Stone.Black, Stone.White).Count} stone groups";

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
                spriteBatch.Draw(Pixel, board.IntersectionPosition(closestIntersection), null, Color.Red, 0, new Vector2(0.5f), 10f, SpriteEffects.None, 0);
            }

            for (int x = 0; x < board.NumLines; x++)
            {
                for (int y = 0; y < board.NumLines; y++)
                {
                    Point intersection = new Point(x, y);
                    if (!board.State.HasLiberties(intersection))
                    {
                        spriteBatch.Draw(Pixel, board.IntersectionPosition(intersection), null, new Color(255, 0, 0, 50), 0, new Vector2(0.5f), 25f, SpriteEffects.None, 0);
                    }
                }
            }

            spriteBatch.Draw(playerTurnLabel);
            spriteBatch.Draw(passLabel);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
