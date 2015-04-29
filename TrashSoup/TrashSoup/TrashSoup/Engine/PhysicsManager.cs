﻿using System;
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
            this.allColliders.Clear();
            this.physicalObjects.Clear();
        }

        /// <summary>
        /// 
        /// Checking for collision, detectig them, deciding if they are trigger enters or collisions, preventing object from colliding with itself
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Ticks < 1) return;
            foreach(Collider col in this.AllColliders)
            {
                foreach(GameObject po in this.physicalObjects)
                {
                    if(col.MyObject != po && col.MyObject.Enabled && po.Enabled)
                    {
                        if(col.Intersects(po.MyPhysicalObject))
                        {
                            if (col.IsTrigger || po.MyCollider.IsTrigger)
                            {
                                Debug.Log("Trigger found: " + col.MyObject.Name + " vs. " + po.Name + " at time: " + gameTime.TotalGameTime.Seconds + " s.");
                                col.MyObject.OnTrigger(po);
                                po.OnTrigger(col.MyObject);
                            }
                            else
                            {
                                Debug.Log("Collision found: " + col.MyObject.Name + " vs. " + po.Name + " at time: " + gameTime.TotalGameTime.Seconds + " s.");
                                po.MyTransform.Position -= col.IntersectionVector;
                                po.MyPhysicalObject.Velocity = Vector3.Zero;
                                col.MyObject.OnCollision(po);
                                po.OnCollision(col.MyObject);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
