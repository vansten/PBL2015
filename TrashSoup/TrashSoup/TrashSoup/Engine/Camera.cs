﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    public class Camera : GameObject
    {
        #region variables
        
        #endregion

        #region properties

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
        public Vector3 Translation { get; set; }
        public Vector3 Direction { get; set; }
        public Vector3 Target { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Right { get; set; }
        public float Speed { get; set; }
        public float FOV { get; set; }
        public float Near { get; set; }
        public float Far { get; set; }


        #endregion

        #region methods
        public Camera(uint uniqueID, string name, Vector3 pos, Vector3 translation, Vector3 target, Vector3 up, float fov, float near, float far) 
            : base(uniqueID, name)
        {
            this.Position = pos;
            this.Translation = translation;
            this.Direction = target - pos;
            this.Target = target;
            this.Direction.Normalize();
            this.Up = up;
            this.Up.Normalize();
            this.Right = Vector3.Cross(this.Direction, this.Up);
            this.FOV = fov;
            this.Near = near;
            this.Far = far;

            CreateLookAt();

            CreateProjection(fov, near, far);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.Direction = this.Target - this.Position;
            this.Direction = this.Direction / this.Direction.Length();
            this.Up.Normalize();
            this.Direction.Normalize();

            CreateLookAt();
        }

        public Vector3 GetDirection() { return Direction; }

        public void CreateProjection(float fov, float near, float far)
        {
            this.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView
            (
                this.FOV,
                (float)TrashSoupGame.Instance.Window.ClientBounds.Width / (float)TrashSoupGame.Instance.Window.ClientBounds.Height,
                this.Near,
                this.Far
            );
        }

        protected void CreateLookAt()
        {
            this.ViewMatrix = Matrix.CreateLookAt(Position + Translation, Target + Translation, Up);
        }
        #endregion
    }
}
