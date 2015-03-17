using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TrashSoup.Engine
{
    class InputManager : Singleton<InputManager>
    {
        #region Variables

        private GamePadState currentGamePadState;
        private GamePadState previousGamePadState;

        #endregion

        #region Methods

        public InputManager()
        {
            this.currentGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        public void Update(GameTime gameTime)
        {
            this.previousGamePadState = this.currentGamePadState;
            this.currentGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// 
        /// Returns left stick value or keyboard WASD value. For example if player clicks keys W and D the returned value will be (1,1). If the player clicks S and D keys the returned value will be (1,-1).
        /// </summary>
        public Vector2 GetLeftStickValue()
        {
            float x = this.currentGamePadState.ThumbSticks.Left.X;
            float y = this.currentGamePadState.ThumbSticks.Left.Y;

            return new Vector2(x, y);
        }

        /// <summary>
        ///
        /// Returns right stick value or delta mouse position. Similar to GetLeftStickValue() method
        /// </summary>
        public Vector2 GetRightStickValue()
        {
            float x = this.currentGamePadState.ThumbSticks.Right.X;
            float y = this.currentGamePadState.ThumbSticks.Right.Y;

            return new Vector2(x, y);
        }

        /// <summary>
        /// 
        /// Returns true if button passed is pressed once
        /// </summary>
        public bool GetGamePadButtonDown(Buttons button)
        {
            bool condition = this.currentGamePadState.IsButtonDown(button) && this.previousGamePadState.IsButtonUp(button);
            return condition;
        }

        /// <summary>
        /// 
        /// Returns true if button passed is not pressed
        /// </summary>
        public bool GetGamePadButtonUp(Buttons button)
        {
            bool condition = this.currentGamePadState.IsButtonUp(button) && this.previousGamePadState.IsButtonDown(button);
            return condition;
        }

        /// <summary>
        /// 
        /// Returns true if button pass is being pressed
        /// </summary>
        public bool GetGamePadButton(Buttons button)
        {
            return this.currentGamePadState.IsButtonDown(button);
        }
        
        #endregion
    }
}
