using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrashSoup.Engine
{
    /// <summary>
    /// 
    /// Base class for every colliders we will have
    /// </summary>
    public class Collider : ObjectComponent
    {
        #region Variables

        protected Matrix worldMatrix;

        #endregion

        #region Properties

        public bool IsTrigger
        {
            get;
            protected set;
        }

        #endregion

        #region Methods

        public Collider(GameObject go) : base(go)
        {
            worldMatrix = Matrix.Identity;
            this.IsTrigger = false;
            this.CreateCollider();
        }

        public Collider(GameObject go, bool isTrigger) : base(go)
        {
            worldMatrix = Matrix.Identity;
            this.IsTrigger = isTrigger;
            this.CreateCollider();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            /*Vector3 tmp = this.MyObject.MyTransform.Position;
            tmp.Z = -tmp.Z;
            this.worldMatrix = Matrix.CreateScale(this.MyObject.MyTransform.Scale) * Matrix.CreateFromYawPitchRoll(this.MyObject.MyTransform.Rotation.Y, this.MyObject.MyTransform.Rotation.X, this.MyObject.MyTransform.Rotation.Z) * Matrix.CreateTranslation(tmp);
            */
            this.worldMatrix = this.MyObject.MyTransform.GetWorldMatrix();
            this.UpdateCollider();
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {

        }

        protected override void Start()
        {

        }

        /// <summary>
        /// 
        /// Creates collider and add this collider to PhysicsManager so PM can manage collisions
        /// </summary>
        protected virtual void CreateCollider()
        {
            PhysicsManager.Instance.AddCollider(this);
        }

        /// <summary>
        /// 
        /// Checks if collision was detected
        /// </summary>
        public virtual bool Intersects(PhysicalObject po)
        {
            return false;
        }

        /// <summary>
        /// 
        /// Updates position of collider
        /// </summary>
        protected virtual void UpdateCollider()
        {

        }

        #endregion
    }
}
