using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrashSoup.Engine
{
    public class DestructiblePart : GameObject
    {
        #region constants

        private const float ROTATION_REDUCTION = 3.0f;

        #endregion

        #region variables

        private bool stopped = false;

        #endregion

        #region properties

        public Vector3 Offset { get; set; }
        public Vector3 Rotation { get; set; }

        #endregion

        #region methods

        public DestructiblePart(uint uniqueID, string name)
            :base(uniqueID, name)
        {
        }

        public override void OnCollision(GameObject otherGO)
        {
            base.OnCollision(otherGO);

            if (!stopped)
            {
                Rotation *= ROTATION_REDUCTION;

                MyPhysicalObject.IsUsingGravity = false;
                MyCollider.IsTrigger = true;
                Vector3 forceVec = Vector3.Normalize(MyTransform.PositionChangeNormal);
                forceVec.Y = 0.0f;
                //Debug.Log(forceVec.ToString());
                MyPhysicalObject.AddForce(forceVec * 500.0f);
                stopped = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if(stopped && MyPhysicalObject != null)
            {
                MyPhysicalObject.Velocity *= 0.95f;
                Rotation *= 0.995f;
                //Debug.Log(MyPhysicalObject.Velocity.ToString());
            }

            base.Update(gameTime);
        }

        #endregion
    }
}
