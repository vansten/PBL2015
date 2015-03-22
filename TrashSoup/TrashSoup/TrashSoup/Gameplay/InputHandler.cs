using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    class InputHandler
    {
        private InputHandler instance;

        public InputHandler Instance
        {
            get
            {
                if(this.instance == null)
                {
                    this.instance = new InputHandler();
                }
                return this.instance;
            }
        }

        private InputHandler()
        {

        }

        /// <summary>
        /// 
        /// Returns true if sqrt(LeftStickValue().Length()) > 0.75f
        /// </summary>
        public bool IsSprinting()
        {
            return InputManager.Instance.GetLeftStickValue().LengthSquared() > 0.75f;
        }

        /// <summary>
        /// 
        /// Returns true if player pushed right trigger (once and is not holding)
        /// </summary>
        public bool IsAttacking()
        {
            return InputManager.Instance.GetGamePadButtonDown(Microsoft.Xna.Framework.Input.Buttons.RightTrigger);
        }

        /// <summary>
        /// 
        /// Returns true if player hold X button on X360 gamepad
        /// </summary>
        public bool Action()
        {
            return InputManager.Instance.GetGamePadButton(Microsoft.Xna.Framework.Input.Buttons.X);
        }
    }
}
