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
        public RectangleWS Rect;
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
        #region constants
        private const uint PLANE_COUNT = 4;
        #endregion

        #region variables
        private QuadTreeNode root;
        private float sceneSize;
        private Dictionary<uint, GameObject> objs;
        private List<GameObject> dynamicObjects;
        Plane[] planesToCheck;
        #endregion

        #region methods
        public QuadTree(Dictionary<uint, GameObject> objs, float sceneSize)
        {
            this.objs = objs;
            this.sceneSize = sceneSize;
            this.dynamicObjects = new List<GameObject>();
            this.planesToCheck = new Plane[PLANE_COUNT];
            root = new QuadTreeNode(null, new RectangleWS(new Vector3(-sceneSize / 2.0f, 0.0f, -sceneSize / 2.0f), new Vector3(sceneSize / 2.0f, 0.0f, sceneSize / 2.0f)));
        }


        public void Update()
        {

        }

        public void Draw(BoundingFrustum frustum)
        {
            planesToCheck[0] = frustum.Near;
            planesToCheck[1] = frustum.Far;
            planesToCheck[2] = frustum.Left;
            planesToCheck[3] = frustum.Right;
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

        public void Add(GameObject obj)
        {
            QuadTreeNode current;
            QuadTreeNode parent;
            bool finished;

            current = root;
            parent = null;
            finished = false;

            RectangleWS rectObj = GenerateRectangle(obj);

            uint ctr = 0;
            while (!finished)
            {
                ++ctr;
                // generate rects for leaf nodes
                RectangleWS childTLRect = current.GetRectForChildTL();
                RectangleWS childTRRect = current.GetRectForChildTR();
                RectangleWS childBRRect = current.GetRectForChildBR();
                RectangleWS childBLRect = current.GetRectForChildBL();

                // check if we fit in any
                if (CheckIfObjectFits(ref childTLRect, ref rectObj))
                {
                    // yay we fit, let's generate that child if necessary and proceed further
                    if (current.ChildTL == null)
                    {
                        current.ChildTL = new QuadTreeNode(current, childTLRect);
                        current.ChildTL.Parent = current;
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
                        current.ChildTR.Parent = current;
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
                        current.ChildBR.Parent = current;
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
                        current.ChildBL.Parent = current;
                    }
                    parent = current;
                    current = current.ChildBL;
                    continue;
                }
                else
                {
                    // holy shit we dont fit in any so let's add ourselves to current node
                    current.Objects.Add(obj);
                    obj.MyNode = current;
                    if(obj.Dynamic)
                    {
                        dynamicObjects.Add(obj);
                    }
                    finished = true;
                    Debug.Log("Object " + obj.Name + " is on level " + ctr.ToString());
                    break;
                }
            }
        }

        public void Remove(GameObject obj)
        {
            if(obj.MyNode != null)
            {
                obj.MyNode.Objects.Remove(obj);
            }
            else
            {
                Debug.Log("QUADTREE: Something went srsly wrong while removing object");
            }
        }

        public QuadTreeNode GetRoot()
        {
            return root;
        }

        private RectangleWS GenerateRectangle(GameObject obj)
        {
            RectangleWS rectObj = new RectangleWS();
            SphereCollider tempSphereCollider;
            BoxCollider tempBoxCollider;

            if (obj.MyCollider is BoxCollider)
            {
                tempBoxCollider = (BoxCollider)obj.MyCollider;

                Vector3 pos, scl; Quaternion rot;
                obj.MyTransform.GetWorldMatrix().Decompose(out scl, out rot, out pos);
                rectObj.Min = Vector3.Transform(tempBoxCollider.Box.Min, Matrix.CreateScale(scl) * Matrix.CreateTranslation(pos));
                rectObj.Max = Vector3.Transform(tempBoxCollider.Box.Max, Matrix.CreateScale(scl) * Matrix.CreateTranslation(pos));
                rectObj.Min.Y = 0.0f;
                rectObj.Max.Y = 0.0f;
            }
            else if (obj.MyCollider is SphereCollider)
            {
                tempSphereCollider = (SphereCollider)obj.MyCollider;
                rectObj.Min = Vector3.Zero;
                rectObj.Max = Vector3.Zero;

                Vector3 pos, scl; Quaternion rot;
                obj.MyTransform.GetWorldMatrix().Decompose(out scl, out rot, out pos);
                Vector3 center = Vector3.Transform(tempSphereCollider.Sphere.Center, Matrix.CreateTranslation(pos));
                float newRadius = tempSphereCollider.Radius * obj.MyTransform.Scale;

                rectObj.Min.X = center.X - newRadius;
                rectObj.Min.Z = center.Z - newRadius;
                rectObj.Max.X = center.X + newRadius;
                rectObj.Max.Z = center.Z + newRadius;
            }
            else
            {
                Debug.Log("QUADTREE: Collider not found for object: " + obj.Name + ", ID " + obj.UniqueID.ToString());
                rectObj.Min = root.Rect.Min;
                rectObj.Max = root.Rect.Max;
            }

            return rectObj;
        }

        private bool CheckIntersection(ref Plane plane, ref RectangleWS rect)
        {
            Vector3 planePoint = plane.Normal * plane.D;
            Vector3 vecMin = planePoint - rect.Min;
            Vector3 vecMax = planePoint - rect.Max;

            Vector3.Normalize(vecMin);
            Vector3.Normalize(vecMax);

            float angleMin = Math.Abs( (float) Math.Acos((double)Vector3.Dot(plane.Normal, vecMin)));
            float angleMax = Math.Abs( (float) Math.Acos((double)Vector3.Dot(plane.Normal, vecMax)));

            if(angleMin >= MathHelper.PiOver2 || angleMax >= MathHelper.PiOver2)
            {
                return true;
            }
            else return false;
        }
        #endregion
    }
}
