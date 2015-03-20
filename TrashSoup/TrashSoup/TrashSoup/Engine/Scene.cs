using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace TrashSoup.Engine
{
    [Serializable]
    public class SceneParams
    {
        #region properties
        public uint UniqueID { get; set; }
        public string Name { get; set; }
        #endregion

        #region methods
        public SceneParams(uint uniqueID, string name)
        {
            this.UniqueID = uniqueID;
            this.Name = name;
        }
        #endregion
    }

    [Serializable]
    public class Scene
    {
        #region properties

        public SceneParams Params { get; set; }
        public Camera Cam { get; set; }

        public Dictionary<uint, GameObject> ObjectsDictionary { get; protected set; }
        public QuadTree<GameObject> ObjectsQT { get; protected set; }
        // place for bounding sphere tree

        #endregion

        #region methods
        public Scene(SceneParams par)
        {
            this.Params = par;

            ObjectsDictionary = new Dictionary<uint, GameObject>();
            ObjectsQT = new QuadTree<GameObject>();
        }

        public void AddObject(GameObject obj)
        {

        }

        public bool DeleteObject(uint uniqueID)
        {
            return false;
        }

        public GameObject GetObject(uint uniqueID)
        {
            return null;
        }

        public List<GameObject> GetObjectsOfType(Type type)
        {
            return null;
        }

        public List<ObjectComponent> GetComponentsOfType(Type type)
        {
            return null;
        }

        public List<GameObject> GetObjectsWithinFrustum(BoundingFrustum frustum)
        {
            return null;
        }

        public List<GameObject> GetObjectsWhichCollide(BoundingSphere bSphere)
        {
            return null;
        }

        public void UpdateAll(GameTime gameTime)
        {
            //[vansten] Added testing code for physics simulation
            Cam.Update(gameTime);
            foreach (GameObject obj in ObjectsDictionary.Values)
            {
                obj.Update(gameTime);
                if (gameTime.TotalGameTime.Seconds > 8)  //time to turn off physics
                {
                    if (obj.MyPhysicalObject != null)
                    {
                        obj.MyPhysicalObject = null;
                    }
                }
                else if (gameTime.TotalGameTime.Seconds > 4) //time to awake
                {
                    if (obj.MyPhysicalObject != null && obj.MyPhysicalObject.Sleeping)
                    {
                        obj.MyPhysicalObject.Awake();
                    }
                }
                else if (gameTime.TotalGameTime.Seconds > 3) //time to go sleep for a while
                {
                    if (obj.MyPhysicalObject != null && !obj.MyPhysicalObject.Sleeping)
                    {
                        obj.MyPhysicalObject.Sleep();
                    }
                }
            }
        }

        // draws all gameobjects linearly
        public void DrawAll(GameTime gameTime)
        {
            foreach (GameObject obj in ObjectsDictionary.Values)
            {
                obj.Draw(gameTime);
            }
        }

        // draws gameobjects that are inside frustum
        public void DrawAll(BoundingFrustum frustum, GameTime gameTime)
        {
            // not implemented yet
        }

        #endregion
    }
}
