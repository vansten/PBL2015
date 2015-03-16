using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup
{
    public class Camera
    {
        #region variables
        public Matrix ViewMatrix
        {
            get;
            protected set;
        }

        public Matrix ProjectionMatrix
        {
            get;
            protected set;
        }

        public Vector3 Position { get; set; }
        public Vector3 Direction { get; protected set; }
        public Vector3 Up { get; protected set; }
        public Vector3 Right { get; protected set; }
        public float Speed { get; protected set; }
        public float FOV { get; protected set; }
        public float Near { get; protected set; }
        public float Far { get; protected set; }

        private float totalYaw = MathHelper.PiOver4 - 0.01f;
        private float currentYaw = 0.0f;
        private float tempYaw;
        private float totalPitch = MathHelper.PiOver2 - 1.0f;
        private float currentPitch = 0.0f;
        private float tempPitch;

        protected ObjectComponent cameraBehaviour;
        #endregion

        #region methods
        public Camera(Vector3 pos, Vector3 target, Vector3 up, float fov, float near, float far)
        {
            this.Position = pos;
            this.Direction = target - pos;
            this.Direction.Normalize();
            this.Up = up;
            this.Right = Vector3.Cross(this.Direction, this.Up);
            this.FOV = fov;
            this.Near = near;
            this.Far = far;

            CreateLookAt();

            CreateProjection(fov, near, far);
        }

        public void AddBehaviour(ObjectComponent beh)
        {
            this.cameraBehaviour = beh;
        }

        public void Update(GameTime gameTime)
        {
            //tempYaw = -MathHelper.PiOver4 / 45.0f * (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X);
            //tempPitch = MathHelper.PiOver4 / 135.0f * (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y);
            //Right = Vector3.Cross(Direction, Up);

            //if (Math.Abs(currentPitch + tempPitch) < totalPitch)
            //{
            //    currentPitch += tempPitch;
            //    Direction = Vector3.Transform(Direction,
            //        Matrix.CreateFromAxisAngle(Right, tempPitch));
            //}

            //if (Math.Abs(currentYaw + tempYaw) < totalYaw)
            //{
            //    currentYaw += tempYaw;
            //    Direction = Vector3.Transform(Direction,
            //        Matrix.CreateFromAxisAngle(Up, tempYaw));
            //}
            if (cameraBehaviour != null) cameraBehaviour.Update(gameTime);

            Direction.Normalize();

            CreateLookAt();
        }

        public Vector3 GetDirection() { return Direction; }

        public void CreateProjection(float fov, float near, float far)
        {
            this.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView
            (
                this.FOV,
                TrashSoupGame.WindowWidth / TrashSoupGame.WindowHeight,
                this.Near,
                this.Far
            );
        }

        private void CreateLookAt()
        {
            this.ViewMatrix = Matrix.CreateLookAt(Position, Position + Direction, Up);
        }
        #endregion
    }
}
