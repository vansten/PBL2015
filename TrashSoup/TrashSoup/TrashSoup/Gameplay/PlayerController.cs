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

        protected const float PLAYER_SPEED = 100.0f;
        protected const float SPRINT_MULTIPLIER = 5.0f;
        protected const float SPRINT_ACCELERATION = 3.0f;
        protected const float ROTATION_SPEED = 10.0f;

        #endregion

        #region variables

        protected Vector3 tempMove;
        protected Vector3 tempMoveRotated;
        protected float sprint;
        protected float sprintM;
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

            if(tempMove.Length() > 0.0f &&
                ResourceManager.Instance.CurrentScene.Cam != null)
            {
                // now to rotate that damn vector as camera direction is rotated
                rotation = (float)Math.Atan2(ResourceManager.Instance.CurrentScene.Cam.Direction.X,
                    -ResourceManager.Instance.CurrentScene.Cam.Direction.Z);
                tempMoveRotated = Vector3.Transform(tempMove, Matrix.CreateRotationY(rotation));
                myObject.MyTransform.Forward = tempMoveRotated;
                Debug.Log(myObject.MyTransform.Forward.X.ToString() + " " + myObject.MyTransform.Forward.Y.ToString() + " " + myObject.MyTransform.Forward.Z.ToString());
                if (InputManager.Instance.GetGamePadButton(Buttons.B))
                {
                    sprint = MathHelper.Lerp(1.0f, SPRINT_MULTIPLIER, sprintM);
                    sprintM += SPRINT_ACCELERATION * (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                    sprintM = MathHelper.Min(sprintM, 1.0f);
                }
                else if(sprintM != 0.0f || sprint != 1.0f)
                {
                    sprint = MathHelper.Lerp(1.0f, SPRINT_MULTIPLIER, sprintM);
                    sprintM -= 2.0f*SPRINT_ACCELERATION * (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                    sprintM = MathHelper.Max(sprintM, 0.0f);
                }

                myObject.MyTransform.Position += (myObject.MyTransform.Forward * PLAYER_SPEED * sprint * (gameTime.ElapsedGameTime.Milliseconds / 1000.0f));
                
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Draw nothing
        }

        protected override void Start()
        {
            sprint = 1.0f;
            sprintM = 0.0f;
        }

        #endregion
    }
}
