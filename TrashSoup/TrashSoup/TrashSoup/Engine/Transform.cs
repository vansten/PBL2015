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
        protected float scale;
        protected Matrix preRotation;

        protected Camera transformableCamera;

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
            this.transformableCamera = null;
            CalculateWorldMatrix();
        }

        public Transform(GameObject obj, Vector3 position, Vector3 rotation, float scale)
            : this(obj)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.Scale = scale;
            CalculateWorldMatrix();
        }

        public Transform(GameObject obj, Vector3 position, Vector3 rotation, Camera camera, float scale)
            : this(obj, position, rotation, scale)
        {
            this.transformableCamera = camera;
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

            if(this.transformableCamera != null)
            {
                transformableCamera.Translation = new Vector3(this.Position.X, this.Position.Y, -this.Position.Z);
            }

            this.worldMatrix = translation * rotation * scale * preRotation;
        }

        #endregion
    }
}
