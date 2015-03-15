using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TrashSoup
{
    class InputManager : Singleton<InputManager>
    {
        #region Variables

        private Vector2 deltaMousePosition;
        private Vector2 prevMousePosition;

        #endregion

        #region Methods

        public InputManager()
        {
            this.prevMousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            this.deltaMousePosition = Vector2.Zero;
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            this.deltaMousePosition = new Vector2(mouse.X, mouse.Y) - this.prevMousePosition;
            this.prevMousePosition = new Vector2(mouse.X, mouse.Y);
        }

        /// <summary>
        /// 
        /// Returns left stick value or keyboard WASD value. For example if player clicks keys W and D the returned value will be (1,1). If the player clicks S and D keys the returned value will be (1,-1).
        /// </summary>
        public Vector2 GetLeftStickValue()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            float x = 0.0f;
            float y = 0.0f;

            if (keyboardState.IsKeyDown(Keys.W)) y += 1.0f;
            if (keyboardState.IsKeyDown(Keys.S)) y -= 1.0f;
            if (keyboardState.IsKeyDown(Keys.A)) x -= 1.0f;
            if (keyboardState.IsKeyDown(Keys.D)) x += 1.0f;

            x += gamePadState.ThumbSticks.Left.X;
            y += gamePadState.ThumbSticks.Left.Y;

            return new Vector2(x, y);
        }

        /// <summary>
        ///
        /// Returns right stick value or delta mouse position. Similar to GetLeftStickValue() method
        /// </summary>
        public Vector2 GetRightStickValue()
        {
            float x = 0.0f;
            float y = 0.0f;

            x += this.deltaMousePosition.X;
            y += this.deltaMousePosition.Y;

            x /= 40.0f;
            y /= 40.0f;

            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
            x += gamePad.ThumbSticks.Right.X;
            y += gamePad.ThumbSticks.Right.Y;

            return new Vector2(x, y);
        }

        /// <summary>
        /// 
        /// Returns flag that tell you if the player is attacking (clicks Left Mouse Button or gamepad attack button)
        /// </summary>
        public bool IsAttacking()
        {
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);
            MouseState mouse = Mouse.GetState();

            return gamePad.IsButtonDown(Buttons.RightTrigger) || (mouse.LeftButton == ButtonState.Pressed);
        }


        /// <summary>
        /// 
        /// Returns current mouse position
        /// </summary>
        public Vector2 GetMousePosition()
        {
            MouseState mouse = Mouse.GetState();

            return new Vector2(mouse.X, mouse.Y);
        }

        /// <summary>
        /// 
        /// Returns true if LMB is pressed
        /// </summary>
        public bool IsLeftMouseButtonDown()
        {
            return Mouse.GetState().LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// 
        /// Returns true if LMB is released
        /// </summary>
        public bool IsLeftMouseButtonUp()
        {
            return Mouse.GetState().LeftButton == ButtonState.Released;
        }
        
        #endregion
    }
}
