using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TrashSoup.Engine
{
    public struct RectangleWS
    {
        public Vector3 Min;
        public Vector3 Max;

        public RectangleWS(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        public bool IntersectsPlane(Plane plane)
        {
            return false;
        }
    }

    public class QuadTreeNode
    {
        #region properties

        public QuadTreeNode Parent { get; set; }
        public List<GameObject> Objects { get; set; }
        public RectangleWS Rect { get; set; }
        public QuadTreeNode ChildTL { get; set; }
        public QuadTreeNode ChildTR { get; set; }
        public QuadTreeNode ChildBR { get; set; }
        public QuadTreeNode ChildBL { get; set; }
        
        #endregion

        #region methods
        public QuadTreeNode(QuadTreeNode parent, RectangleWS rect)
        {
            this.Parent = parent;
            this.Objects = new List<GameObject>();
            this.Rect = rect;
            this.ChildTL = null;
            this.ChildTR = null;
            this.ChildBR = null;
            this.ChildBL = null;
        }

        public RectangleWS GetRectForChildTL()
        {
            float distX = Math.Abs(Rect.Max.X - Rect.Min.X);
            float distZ = Math.Abs(Rect.Max.Z - Rect.Min.Z);

            distX /= 2.0f;
            distZ /= 2.0f;

            return new RectangleWS(new Vector3(Rect.Min.X, Rect.Min.Y, Rect.Min.Z + distZ), new Vector3(Rect.Max.X - distX, Rect.Max.Y, Rect.Max.Z));
        }

        public RectangleWS GetRectForChildTR()
        {
            float distX = Math.Abs(Rect.Max.X - Rect.Min.X);
            float distZ = Math.Abs(Rect.Max.Z - Rect.Min.Z);

            distX /= 2.0f;
            distZ /= 2.0f;

            return new RectangleWS(new Vector3(Rect.Min.X + distX, Rect.Min.Y, Rect.Min.Z + distZ), Rect.Max);
        }

        public RectangleWS GetRectForChildBR()
        {
            float distX = Math.Abs(Rect.Max.X - Rect.Min.X);
            float distZ = Math.Abs(Rect.Max.Z - Rect.Min.Z);

            distX /= 2.0f;
            distZ /= 2.0f;

            return new RectangleWS(new Vector3(Rect.Min.X + distX, Rect.Min.Y, Rect.Min.Z), new Vector3(Rect.Max.X, Rect.Max.Y, Rect.Max.Z - distZ));
        }

        public RectangleWS GetRectForChildBL()
        {
            float distX = Math.Abs(Rect.Max.X - Rect.Min.X);
            float distZ = Math.Abs(Rect.Max.Z - Rect.Min.Z);

            distX /= 2.0f;
            distZ /= 2.0f;

            return new RectangleWS(Rect.Min, new Vector3(Rect.Max.X - distX, Rect.Max.Y, Rect.Max.Z - distZ));
        }
        #endregion
    }

    public class QuadTree
    {
        #region variables
        private QuadTreeNode root;
        private float sceneWidth;
        private float sceneHeight;
        private Dictionary<uint, GameObject> objs;
        #endregion

        #region methods
        public QuadTree(Dictionary<uint, GameObject> objs, float sceneWidth, float sceneHeight)
        {
            this.objs = objs;
            this.sceneWidth = sceneWidth;
            this.sceneHeight = sceneHeight;
            root = null;
        }

        public void Generate()
        {
            root = new QuadTreeNode(null, new RectangleWS(new Vector3(-sceneWidth / 2.0f, 0.0f, -sceneHeight / 2.0f), new Vector3(sceneWidth / 2.0f, 0.0f, sceneHeight / 2.0f)));

            GameObject obj;
            QuadTreeNode current;
            QuadTreeNode parent;
            bool finished;
            SphereCollider tempSphereCollider;
            BoxCollider tempBoxCollider;

            foreach(KeyValuePair<uint, GameObject> val in objs)
            {
                obj = val.Value;
                current = root;
                parent = null;
                finished = false;

                RectangleWS rectObj = new RectangleWS();

                if(obj.MyCollider is BoxCollider)
                {
                    tempBoxCollider = (BoxCollider)obj.MyCollider;
                    rectObj.Min = tempBoxCollider.Box.Min;
                    rectObj.Max = tempBoxCollider.Box.Max;
                    rectObj.Min.Y = 0.0f;
                    rectObj.Max.Y = 0.0f;
                }
                else if(obj.MyCollider is SphereCollider)
                {
                    tempSphereCollider = (SphereCollider)obj.MyCollider;
                    rectObj.Min = Vector3.Zero;
                    rectObj.Max = Vector3.Zero;

                    rectObj.Min.X = tempSphereCollider.MyBoundingSphere.Center.X - tempSphereCollider.MyBoundingSphere.Radius;
                    rectObj.Min.Z = tempSphereCollider.MyBoundingSphere.Center.Z - tempSphereCollider.MyBoundingSphere.Radius;
                    rectObj.Max.X = tempSphereCollider.MyBoundingSphere.Center.X + tempSphereCollider.MyBoundingSphere.Radius;
                    rectObj.Max.Z = tempSphereCollider.MyBoundingSphere.Center.Z + tempSphereCollider.MyBoundingSphere.Radius;
                }
                else
                {
                    Debug.Log("QUADTREE: Collider not found for object: " + obj.Name + ", ID " + obj.UniqueID.ToString());
                    rectObj.Min = root.Rect.Min;
                    rectObj.Max = root.Rect.Max;
                }

                while(!finished)
                {
                    // check if we are alone in our node, if yes, add ourselves to it
                    if(current.Objects.Count == 0)
                    {
                        current.Objects.Add(obj);
                        finished = true;
                        break;
                    }

                    // generate rects for leaf nodes
                    RectangleWS childTLRect = current.GetRectForChildTL();
                    RectangleWS childTRRect = current.GetRectForChildTR();
                    RectangleWS childBRRect = current.GetRectForChildBR();
                    RectangleWS childBLRect = current.GetRectForChildBL();

                    // check if we fit in any
                    if(CheckIfObjectFits(ref childTLRect, ref rectObj))
                    {
                        // yay we fit, let's generate that child if necessary and proceed further
                        if(current.ChildTL == null)
                        {
                            current.ChildTL = new QuadTreeNode(current, childTLRect);
                        }
                        parent = current;
                        current = current.ChildTL;
                        continue;
                    }
                    else if (CheckIfObjectFits(ref childTRRect, ref rectObj))
                    {
                        if (current.ChildTR == null)
                        {
                            current.ChildTR = new QuadTreeNode(current, childTRRect);
                        }
                        parent = current;
                        current = current.ChildTR;
                        continue;
                    }
                    else if (CheckIfObjectFits(ref childBRRect, ref rectObj))
                    {
                        if (current.ChildBR == null)
                        {
                            current.ChildBR = new QuadTreeNode(current, childBRRect);
                        }
                        parent = current;
                        current = current.ChildBR;
                        continue;
                    }
                    else if (CheckIfObjectFits(ref childBLRect, ref rectObj))
                    {
                        if (current.ChildBL == null)
                        {
                            current.ChildBL = new QuadTreeNode(current, childBLRect);
                        }
                        parent = current;
                        current = current.ChildBL;
                        continue;
                    }
                    else
                    {
                        // holy shit we dont fit in any so let's add ourselves to current node
                        current.Objects.Add(obj);
                        finished = true;
                        break;
                    }
                }
            }
        }

        public void Update()
        {

        }

        private bool CheckIfObjectFits(ref RectangleWS rectOut, ref RectangleWS rectIn)
        {
            if (rectIn.Min.X > rectOut.Min.X &&
                rectIn.Min.Z > rectOut.Min.Z &&
                rectIn.Max.X < rectOut.Max.X &&
                rectIn.Max.Z < rectOut.Max.Z)
            {
                return true;
            }
            else return false;
        }

        //public void Add(T obj)
        //{
        //    // TODO: implement
        //}

        //public bool Remove(uint uniqueID)
        //{
        //    // TOOD: implement
        //    return false;
        //}

        public QuadTreeNode GetRoot()
        {
            return root;
        }
        #endregion
    }
}
