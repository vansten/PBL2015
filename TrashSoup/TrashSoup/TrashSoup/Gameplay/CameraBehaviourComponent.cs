using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    public class CameraBehaviourComponent : ObjectComponent
    {
        #region variables

        protected Camera cam;

        protected float totalYaw = MathHelper.PiOver2 - 0.01f;
        protected float currentYaw = 0.0f;
        protected float tempYaw;
        protected float totalPitch = MathHelper.PiOver2 - 1.0f;
        protected float currentPitch = 0.0f;
        protected float tempPitch;

        #endregion

        #region methods

        public CameraBehaviourComponent(GameObject obj) : base(obj)
        {
            Start();
        }

        public override void Update(GameTime gameTime)
        {
            tempYaw = -MathHelper.PiOver4 / 45.0f * (InputManager.Instance.GetRightStickValue().X);
            tempPitch = MathHelper.PiOver4 / 135.0f * (InputManager.Instance.GetRightStickValue().Y);
            cam.Right = Vector3.Cross(cam.Direction, cam.Up);

            if (Math.Abs(currentPitch + tempPitch) < totalPitch)
            {
                currentPitch += tempPitch;
                cam.Direction = Vector3.Transform(cam.Direction,
                    Matrix.CreateFromAxisAngle(cam.Right, tempPitch));
            }

            if (Math.Abs(currentYaw + tempYaw) < totalYaw)
            {
                currentYaw += tempYaw;
                cam.Direction = Vector3.Transform(cam.Direction,
                    Matrix.CreateFromAxisAngle(cam.Up, tempYaw));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // do nothing since it's camera
        }

        protected override void Start()
        {
            cam = (Camera)myObject;
        }

        #endregion
    }
}
