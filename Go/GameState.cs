using Microsoft.Xna.Framework.Input;
using System;

namespace Go
{
    public class GameState
    {
        public KeyboardState CurrentKeyboardState;
        public KeyboardState LastKeyboardState;
        public MouseState CurrentMouseState;
        public MouseState LastMouseState;

        public void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
        }
    }
}