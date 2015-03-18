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

        #endregion

        #region Properties

        public Vector3 Gravity { get; set; }

        #endregion

        #region Methods

        public PhysicsManager()
        {
            this.physicalObjects = new List<GameObject>();
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

        public void Update(GameTime gameTime)
        {
            //Do nothing right now
        }

        #endregion
    }
}
