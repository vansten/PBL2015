using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    /// <summary>
    /// 
    /// Simple box collider class. It creates a box arround the game object
    /// </summary>
    public class BoxCollider : Collider
    {
        #region Variables

        public BoundingBox Box;
        private CustomModel model;
        //private CustomSkinnedModel skinned;
        private Vector3 min;
        private Vector3 max;
        private Vector3[] corners;

        #endregion

        #region Properties

        #endregion

        #region Methods

        public BoxCollider(GameObject go) : base(go)
        {

        }

        public BoxCollider(GameObject go, bool isTrigger) : base(go, isTrigger)
        {

        }

        /// <summary>
        /// 
        /// Debug drawing collider, no one draws colliders in game :D
        /// </summary>
        public override void Draw(Camera cam, Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(TrashSoupGame.Instance.EditorMode)
            {
                short[] bBoxIndices = {
                                    0, 1, 1, 2, 2, 3, 3, 0, // Front edges
                                    4, 5, 5, 6, 6, 7, 7, 4, // Back edges
                                    0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
                                  };

                Vector3[] corners = this.Box.GetCorners();
                VertexPositionColor[] primitiveList = new VertexPositionColor[corners.Length];

                // Assign the 8 box vertices
                for (int i = 0; i < corners.Length; i++)
                {
                    primitiveList[i] = new VertexPositionColor(corners[i], Color.White);
                }

                BasicEffect lineEffect = new BasicEffect(TrashSoupGame.Instance.GraphicsDevice);
                lineEffect.LightingEnabled = false;
                lineEffect.TextureEnabled = false;
                lineEffect.VertexColorEnabled = true;

                GraphicsDevice gd = TrashSoupGame.Instance.GraphicsDevice;
                VertexBuffer buffer = new VertexBuffer(gd, typeof(VertexPositionColor), primitiveList.Length, BufferUsage.None);
                buffer.SetData(primitiveList);
                IndexBuffer ib = new IndexBuffer(gd, IndexElementSize.SixteenBits, bBoxIndices.Length, BufferUsage.WriteOnly);
                ib.SetData(bBoxIndices);
                gd.SetVertexBuffer(buffer);
                gd.Indices = ib;

                if (cam == null)
                    cam = ResourceManager.Instance.CurrentScene.Cam;

                lineEffect.World = Matrix.Identity;
                lineEffect.View = cam.ViewMatrix;
                lineEffect.Projection = cam.ProjectionMatrix;
                foreach (EffectPass pass in lineEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    gd.DrawUserIndexedPrimitives(PrimitiveType.LineList, primitiveList, 0, 8, bBoxIndices, 0, 12);
                }

                base.Draw(cam, effect, gameTime);
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// 
        /// This function creates box collider but it can create collider only for object that contains CustomModel or CustomSkinnedModel with first state of LOD (LODs[0] mustn't be null)
        /// </summary>
        protected override void CreateCollider()
        {
            if (this.MyObject == null) return;
            foreach(ObjectComponent oc in this.MyObject.Components)
            {
                if(oc.GetType() == typeof(CustomModel))
                {
                    this.model = (CustomModel)oc;
                }
            }

            min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            if (this.model != null)
            {
                foreach (ModelMesh mesh in this.model.LODs[0].Meshes)
                {
                    this.MyBoundingSphere = BoundingSphere.CreateMerged(this.MyBoundingSphere, mesh.BoundingSphere);
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        int vertexStride = part.VertexBuffer.VertexDeclaration.VertexStride;
                        int vertexBufferSize = part.NumVertices * vertexStride;
                        float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                        part.VertexBuffer.GetData<float>(vertexData);
                        for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                        {
                            Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), this.worldMatrix);

                            min = Vector3.Min(min, transformedPosition);
                            max = Vector3.Max(max, transformedPosition);
                        }
                        min = Vector3.Transform(min, mesh.ParentBone.Transform);
                        max = Vector3.Transform(max, mesh.ParentBone.Transform);
                    }
                }
            }

            this.Box = new BoundingBox(min, max);
            this.corners = this.Box.GetCorners();

            base.CreateCollider();
        }

        /// <summary>
        /// 
        /// This function accepts a PhysicalObject as parameter and checks if bounding boxes (or spheres in future) intersect each other. Return true if collision detected
        /// </summary>
        public override bool Intersects(PhysicalObject po)
        {
            Collider poCollider = po.MyObject.MyCollider;
            if (poCollider == null)
            {
                return false;
            }

            if (!this.MyBoundingSphere.Intersects(poCollider.MyBoundingSphere))
            {
                return false;
            }
            
            if(poCollider.GetType() == typeof(BoxCollider))
            {
                return this.IntersectsWithAABB(po, ((BoxCollider)poCollider).Box);   
            }
            else if(poCollider.GetType() == typeof(SphereCollider))
            {
                return this.IntersectsWithSphere(po, ((SphereCollider)poCollider).Sphere);
            }

            return false;
        }

        private bool IntersectsWithSphere(PhysicalObject po, BoundingSphere boundingSphere)
        {
            return false;
        }

        private bool IntersectsWithAABB(PhysicalObject po, BoundingBox boundingBox)
        {
            return false;
        }

        /// <summary>
        /// 
        /// This function updates collider so it can move when game object moves :)
        /// </summary>
        protected override void UpdateCollider()
        {
            min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            Vector3[] newCorners = this.Box.GetCorners();
            for (int i = 0; i < corners.Length; ++i)
            {
                newCorners[i] = Vector3.Transform(corners[i], this.worldMatrix);
                min = Vector3.Min(min, newCorners[i]);
                max = Vector3.Max(max, newCorners[i]);
            }
            this.Box.Min = min;
            this.Box.Max = max;
        }

        #endregion
    }
}
