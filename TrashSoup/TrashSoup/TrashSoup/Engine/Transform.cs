using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TrashSoup.Engine
{
    public class Transform : ObjectComponent
    {
        #region variables

        protected Matrix worldMatrix;
        protected Vector3 position;
        protected Vector3 rotation;
        protected Vector3 forward;
        protected float scale;
        protected Matrix preRotation;

        #endregion

        #region properties

        public Vector3 Position
        { 
            get
            {
                return position;
            }
            set
            {
                position = value;
                CalculateWorldMatrix();
            }
        }
        public Vector3 Rotation 
        { 
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                CalculateWorldMatrix();
            }
        }
        public Vector3 Forward
        {
            get
            {
                return forward;
            }
            set
            {
                forward = value;
                RotateAsForward();
                CalculateWorldMatrix();
            }
        }
        public float Scale 
        { 
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                CalculateWorldMatrix();
            }
        }

        #endregion

        #region methods

        public Transform(GameObject obj) : base(obj)
        {
            this.Position = new Vector3(0.0f, 0.0f, 0.0f);
            this.Rotation = new Vector3(0.0f, 0.0f, 0.0f);
            this.Scale = 1.0f;
            this.preRotation = Matrix.CreateRotationX(-MathHelper.PiOver2);
        }

        public Transform(GameObject obj, Vector3 position, Vector3 forward, Vector3 rotation, float scale)
            : this(obj)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
            this.Forward = forward;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // do nothing since we change worldMatrix in properties
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // do nothing
        }

        public Matrix GetWorldMatrix()
        {
            return worldMatrix;
        }

        protected override void Start()
        {
            /// do nothing
        }

        protected void CalculateWorldMatrix()
        {
            Matrix translation, rotation, scale;
            translation = Matrix.CreateTranslation(new Vector3(this.Position.X, -this.Position.Y, this.Position.Z));
            rotation = Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            scale = Matrix.CreateScale(this.Scale);

            this.worldMatrix = rotation *translation *scale * preRotation;
        }

        protected void RotateAsForward()
        {
            float rotY = (float)Math.Atan2(this.Forward.X, this.Forward.Z);
            this.Rotation = new Vector3(this.Rotation.X, rotY, this.Rotation.Z);
        }

        #endregion
    }
}
