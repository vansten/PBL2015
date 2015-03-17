﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    public class CameraBehaviourComponent : ObjectComponent
    {
        #region constants

        protected const float CAM_YAW_SENSITIVITY = MathHelper.PiOver4 / 30.0f;
        protected const float CAM_PITCH_SENSITIVITY = MathHelper.PiOver4 / 30.0f;
        protected const float CAM_TOTAL_PITCH = MathHelper.PiOver2;
        protected const float CAM_DISTANCE = 60.0f;

        #endregion

        #region variables

        protected Camera cam;

        protected float tempYaw;
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
            tempYaw = CAM_YAW_SENSITIVITY * (InputManager.Instance.GetRightStickValue().X);
            tempPitch = -CAM_PITCH_SENSITIVITY * (InputManager.Instance.GetRightStickValue().Y);
            cam.Right = Vector3.Cross(cam.Direction, cam.Up);

            if (Math.Abs(currentPitch + tempPitch) < CAM_TOTAL_PITCH)
            {
                currentPitch += tempPitch;
                cam.Position = cam.Position / cam.Position.Length();
                cam.Position = CAM_DISTANCE * cam.Position;
                cam.Position = Vector3.Transform(cam.Position,
                    Matrix.CreateFromAxisAngle(cam.Right, tempPitch));
            }

            cam.Position = Vector3.Transform(cam.Position,
                    Matrix.CreateFromAxisAngle(cam.Up, tempYaw));
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
