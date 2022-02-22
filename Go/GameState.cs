using Microsoft.Xna.Framework.Input;

namespace Go
{
    public class GameState
    {
        public KeyboardState CurrentKeyboardState;
        public KeyboardState PreviousKeyboardState;
        public MouseState CurrentMouseState;
        public MouseState PreviousMouseState;

        public void Update()
        {
            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }

        public bool LeftClick()
        {
            return CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released;
        }

        public bool RightClick()
        {
            return CurrentMouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Released;
        }
    }
}