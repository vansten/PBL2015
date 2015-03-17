using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    public class PlayerController : ObjectComponent
    {
        #region constants

        protected const float PLAYER_SPEED = 1.0f;
        protected const float SPRINT_MULTIPLIER = 10.0f;

        #endregion

        #region variables

        protected Camera playerCam;
        protected float tempMoveForward;
        protected float tempMoveSide;
        protected float tempMoveVertical;
        protected Vector3 movement;

        #endregion

        #region methods

        public PlayerController(GameObject obj) : base(obj)
        {
            Start();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            tempMoveForward = InputManager.Instance.GetLeftStickValue().Y;
            tempMoveSide = InputManager.Instance.GetLeftStickValue().X;
            tempMoveVertical = (InputManager.Instance.GetGamePadButton(Buttons.RightTrigger) ? 1.0f : 0.0f) - (InputManager.Instance.GetGamePadButton(Buttons.LeftTrigger) ? 1.0f : 0.0f);

            float sprint = (InputManager.Instance.GetGamePadButton(Buttons.B)) ? SPRINT_MULTIPLIER : 1.0f;
            this.movement = new Vector3(PLAYER_SPEED * sprint * tempMoveSide, tempMoveVertical, PLAYER_SPEED * sprint * tempMoveForward);
            myObject.MyTransform.Position += movement;

            // Player Camera is automatically controlled by transform
            //if(playerCam != null) playerCam.Translation += new Vector3(movement.X, movement.Y, -movement.Z);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Draw nothing
        }

        protected override void Start()
        {
            // Player Camera is automatically controlled by transform
            playerCam = ResourceManager.Instance.CurrentScene.Cam;
            if (playerCam == null) Debug.Log("ERROR: No player camera in scene!\n");
        }

        #endregion
    }
}
