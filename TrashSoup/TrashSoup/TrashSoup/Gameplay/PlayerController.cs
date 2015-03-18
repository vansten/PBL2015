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

        protected Vector3 tempMove;
        protected Vector3 tempMoveRotated;
        protected float sprint;
        protected float rotation;

        #endregion

        #region methods

        public PlayerController(GameObject obj) : base(obj)
        {
            Start();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            tempMove = new Vector3(InputManager.Instance.GetLeftStickValue().X,
                (InputManager.Instance.GetGamePadButton(Buttons.RightTrigger) ? 1.0f : 0.0f) - (InputManager.Instance.GetGamePadButton(Buttons.LeftTrigger) ? 1.0f : 0.0f),
                InputManager.Instance.GetLeftStickValue().Y);

            if(tempMove.Length() > 0.0f)
            {
                // now to rotate that damn vector as camera direction is rotated
                rotation = (float)Math.Atan2(playerCam.Direction.X, -playerCam.Direction.Z);
                tempMoveRotated = Vector3.Transform(tempMove, Matrix.CreateRotationY(rotation));
                myObject.MyTransform.Forward = tempMoveRotated;

                sprint = (InputManager.Instance.GetGamePadButton(Buttons.B)) ? SPRINT_MULTIPLIER : 1.0f;

                myObject.MyTransform.Position += (myObject.MyTransform.Forward * PLAYER_SPEED * sprint);
                // object rotation to forward vector is automatically controlled by transform
            }
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
