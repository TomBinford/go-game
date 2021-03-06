using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

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

        const int boardLines = 9;
        Board board;
        Label statusLabel;
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
            board = new Board(new Rectangle(50, 50, 9 * 80, 9 * 80), boardLines, Color.BurlyWood, Color.Black, 2);
            statusLabel = new Label(font, "Player to move", new Vector2(board.Bounds.Right + 50, board.Bounds.Top), Color.Pink, 1.5f);
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
                return;
            }
            State.Update();

            // TODO: Add your update logic here
            if (passLabel.LeftClicked())
            {
                if (!board.State.Terminal)
                {
                    BoardState nextState = board.State.MakePlay(Play.Pass());
                    if (nextState != null)
                    {
                        board.State = nextState;
                    }
                }
                else
                {
                    board.State = new BoardState(boardLines, firstPlayer: Player.Black);
                }
            }
            else if (State.LeftClick())
            {
                Point intersection = board.ClosestIntersection(State.CurrentMouseState.Position.ToVector2());
                BoardState nextState = board.State.MakePlay(Play.Move(intersection));
                if (nextState != null)
                {
                    board.State = nextState;
                }
            }

            if (board.State.Terminal)
            {
                if (board.State.Winner == Player.None)
                {
                    statusLabel.Text = "Game is a draw";
                }
                else
                {
                    statusLabel.Text = $"Player {board.State.Winner} wins!";
                    passLabel.Text = "Play Again";
                }
            }
            else
            {
                statusLabel.Text = $"{board.State.CurrentPlayer} to move";
                statusLabel.Text += $"\n{board.State.StoneGroups(Stone.Black, Stone.White).Count} stone groups";
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
                spriteBatch.Draw(Pixel, board.IntersectionPosition(closestIntersection), null, Color.Red, 0, new Vector2(0.5f), 10f, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(statusLabel);
            spriteBatch.Draw(passLabel);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
