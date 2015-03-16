using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace TrashSoup
{
    [Serializable]
    public class SceneParams
    {
        #region variables
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
        #region variables
        public SceneParams Params { get; set; }

        public Camera Cam { get; set; }

        protected List<GameObject> objectsList;
        protected QuadTree<GameObject> objectsQT;
        // place for bounding sphere tree
        #endregion

        #region methods
        public Scene(SceneParams par)
        {
            this.Params = par;

            objectsList = new List<GameObject>();
            objectsQT = new QuadTree<GameObject>();
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

        #endregion
    }
}
