using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    class InputHandler
    {
        private static InputHandler instance;

        public static InputHandler Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new InputHandler();
                }
                return instance;
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
        /// Returns true if player pushed A (once and is not holding) or Left alt
        /// </summary>
        public bool IsJumping()
        {
            return (InputManager.Instance.GetGamePadButtonDown(Microsoft.Xna.Framework.Input.Buttons.A) ||
                InputManager.Instance.GetKeyboardButtonDown(Keys.LeftAlt));
        }

        /// <summary>
        /// 
        /// Returns true if player hold X button on X360 gamepad or E on keyboard
        /// </summary>
        public bool Action()
        {
            return InputManager.Instance.GetGamePadButton(Microsoft.Xna.Framework.Input.Buttons.X) || 
                InputManager.Instance.GetKeyboardButton(Keys.E);
        }

        /// <summary>
        /// 
        /// Returns movement vector of the player, works on gamepad as well as on keyboard
        /// </summary>
        public Vector2 GetMovementVector()
        {
            Vector2 toReturn;
            if ((toReturn = InputManager.Instance.GetLeftStickValue()).Length() != 0)
            {
                return toReturn;
            }
            else
            {
                if(InputManager.Instance.GetKeyboardButton(Keys.W) && InputManager.Instance.GetKeyboardButton(Keys.D))
                {
                    toReturn = new Vector2(1.0f, 1.0f);
                }
                else if (InputManager.Instance.GetKeyboardButton(Keys.W) && InputManager.Instance.GetKeyboardButton(Keys.A))
                {
                    toReturn = new Vector2(-1.0f, 1.0f);
                }
                else if (InputManager.Instance.GetKeyboardButton(Keys.S) && InputManager.Instance.GetKeyboardButton(Keys.D))
                {
                    toReturn = new Vector2(1.0f, -1.0f);
                }
                else if (InputManager.Instance.GetKeyboardButton(Keys.S) && InputManager.Instance.GetKeyboardButton(Keys.A))
                {
                    toReturn = new Vector2(-1.0f, -1.0f);
                }
                else if (InputManager.Instance.GetKeyboardButton(Keys.W))
                {
                    toReturn = new Vector2(0.0f, 1.0f);
                }
                else if (InputManager.Instance.GetKeyboardButton(Keys.S))
                {
                    toReturn = new Vector2(0.0f, -1.0f);
                }
                else if (InputManager.Instance.GetKeyboardButton(Keys.A))
                {
                    toReturn = new Vector2(-1.0f, 0.0f);
                }
                else if (InputManager.Instance.GetKeyboardButton(Keys.D))
                {
                    toReturn = new Vector2(1.0f, 0.0f);
                }
            }
            return Vector2.Normalize(toReturn);
        }

        /// <summary>
        /// 
        /// Returns camera vector, works on gamepad as well as on keyboard
        /// </summary>
        public Vector2 GetCameraVector()
        {
            Vector2 toReturn;
            if ((toReturn = InputManager.Instance.GetRightStickValue()).Length() != 0)
            {
                return toReturn;
            }
            else
            {
                toReturn = InputManager.Instance.GetMouseRelativeValue() * 0.2f;
                toReturn.Y = - toReturn.Y;
            }
            return toReturn;
        }
    }
}
