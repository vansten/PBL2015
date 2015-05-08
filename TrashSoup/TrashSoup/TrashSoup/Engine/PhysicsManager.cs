using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TrashSoup.Engine
{
    //TODO:
    //Add collision detection
    class PhysicsManager : Singleton<PhysicsManager>
    {
        #region Variables

        private List<GameObject> physicalObjects;
        private Vector3 intersectionVector;

        #endregion

        #region Properties

        public Vector3 Gravity { get; set; }
        public List<Collider> AllColliders { get; private set; }

        #endregion

        #region Methods

        public PhysicsManager()
        {
            this.physicalObjects = new List<GameObject>();
            this.AllColliders = new List<Collider>();
            this.Gravity = new Vector3(0.0f, -9.81f, 0.0f);
        }

        /// <summary>
        /// 
        /// Adds a game object to physical object list, so it can interact with other game objects
        /// It is done by setter in game object's MyPhysicalObject property (there is no need to calling it by you)
        /// </summary>
        public void AddPhysicalObject(GameObject go)
        {
            this.physicalObjects.Add(go);
        }

        /// <summary>
        /// 
        /// Removes game object from physical object list, so it can't interact with other game objects without physical object component
        /// It is done by setter in game object's MyPhysicalObject property (there is no need to calling it by you)
        /// </summary>
        public void RemovePhysicalObject(GameObject go)
        {
            if(this.physicalObjects.Contains(go))
            {
                this.physicalObjects.Remove(go);
            }
        }

        /// <summary>
        /// 
        /// Adds a collider to list, so it can be itterated to find a collider that collides with one of physical objects
        /// </summary>
        public void AddCollider(Collider col)
        {
            this.AllColliders.Add(col);
        }

        /// <summary>
        /// 
        /// Removes collider so it can't longer collide with anything
        /// </summary>
        public void RemoveCollider(Collider col)
        {
            this.AllColliders.Remove(col);
        }

        public void Reload()
        {
            this.AllColliders.Clear();
            this.physicalObjects.Clear();
        }

        public bool CanMove(GameObject go)
        {
            if (go.MyTransform == null) return true;
            if (go.MyCollider == null) return true;

            foreach(Collider col in this.AllColliders)
            {
                if(col != go.MyCollider && col.MyObject.Enabled && go.Enabled)
                {
                    if(col.Intersects(go.MyCollider))
                    {
                        if(col.IsTrigger || go.MyCollider.IsTrigger)
                        {
                            col.MyObject.OnTrigger(go);
                            go.OnTrigger(col.MyObject);
                        }
                        else
                        {
                            if (go.MyPhysicalObject != null) go.MyPhysicalObject.Velocity = Vector3.Zero;
                            if (col.MyObject.MyPhysicalObject != null) col.MyObject.MyPhysicalObject.Velocity = Vector3.Zero;
                            col.MyObject.OnCollision(go);
                            go.OnCollision(col.MyObject);
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        #endregion
    }
}
