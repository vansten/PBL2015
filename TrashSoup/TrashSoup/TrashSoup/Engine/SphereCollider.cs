using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    class SphereCollider : Collider
    {
        #region Variables

        public BoundingSphere Sphere;

        private BoundingSphere initialSphere;
        private Vector3 center;
        private float radius;
        private CustomModel model;

        #endregion

        #region Properties

        #endregion

        #region Methods

        public SphereCollider(GameObject go) : base(go)
        {

        }

        public SphereCollider(GameObject go, bool isTrigger) : base(go, isTrigger)
        {

        }

        public override void Draw(Camera cam, Effect effect, GameTime gameTime)
        {
            //Dunno how to draw bounding sphere :(
            BoundingBox box = BoundingBox.CreateFromSphere(this.Sphere);
            short[] bBoxIndices = {
                                    0, 1, 1, 2, 2, 3, 3, 0, // Front edges
                                    4, 5, 5, 6, 6, 7, 7, 4, // Back edges
                                    0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
                                  };

            Vector3[] corners = box.GetCorners();
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

        protected override void Start()
        {
            base.Start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

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

            this.radius = float.MinValue;
            this.center = Vector3.Zero;
            int verticesNum = 0;

            if(this.model != null)
            {
                foreach(ModelMesh mesh in this.model.LODs[0].Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        int vertexStride = part.VertexBuffer.VertexDeclaration.VertexStride;
                        int vertexBufferSize = part.NumVertices * vertexStride;
                        float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                        part.VertexBuffer.GetData<float>(vertexData);
                        for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                        {
                            Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), this.worldMatrix);

                            this.center += transformedPosition;
                            ++verticesNum;
                        }
                    }
                }

                this.center /= verticesNum;

                foreach (ModelMesh mesh in this.model.LODs[0].Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        int vertexStride = part.VertexBuffer.VertexDeclaration.VertexStride;
                        int vertexBufferSize = part.NumVertices * vertexStride;
                        float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                        part.VertexBuffer.GetData<float>(vertexData);
                        for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                        {
                            Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), this.worldMatrix);

                            if(Vector3.Distance(this.center, transformedPosition) > this.radius)
                            {
                                this.radius = Vector3.Distance(this.center, transformedPosition);
                            }
                        }
                    }
                }
            }

            this.initialSphere = new BoundingSphere(this.center, this.radius);

            base.CreateCollider();
        }

        protected override void UpdateCollider()
        {
            Vector3 newCenter = Vector3.Zero;
            newCenter = Vector3.Transform(this.center, this.worldMatrix);
            this.Sphere.Center = newCenter;
            this.Sphere = this.initialSphere.Transform(this.worldMatrix);
        }

        public override bool Intersects(PhysicalObject po)
        {
            Collider poCollider = po.MyObject.MyCollider;
            if (poCollider == null)
            {
                return false;
            }

            if (poCollider.GetType() == typeof(BoxCollider))
            {
                return this.IntersectsWithAABB(po, ((BoxCollider)poCollider).Box);
            }
            else if (poCollider.GetType() == typeof(SphereCollider))
            {
                return this.IntersectsWithSphere(po, ((SphereCollider)poCollider).Sphere);
            }

            return false;
        }

        private bool IntersectsWithSphere(PhysicalObject po, BoundingSphere boundingSphere)
        {
            if (this.Sphere.Intersects(boundingSphere))
            {
                Vector3 positionChange = po.MyObject.MyTransform.PositionChangeNormal;
                if (positionChange != Vector3.Zero)
                {
                    positionChange.Normalize();
                }

                Vector3 direction = this.Sphere.Center - boundingSphere.Center;
                float intersectionValue = this.Sphere.Radius + boundingSphere.Radius - direction.Length();
                direction.Normalize();

                direction *= intersectionValue;

                if (po.Velocity.X == 0.0f)
                {
                    direction.X *= positionChange.X;
                }
                if (po.Velocity.Y == 0.0f)
                {
                    direction.Y *= positionChange.Y;
                }
                if (po.Velocity.Z == 0.0f)
                {
                    direction.Z *= positionChange.Z;
                }

                this.IntersectionVector = direction;

                return true;
            }
            return false;
        }

        private bool IntersectsWithAABB(PhysicalObject po, BoundingBox boundingBox)
        {
            if (this.Sphere.Intersects(boundingBox))
            {
                float intersectionPointMaxX = this.Sphere.Center.X + this.Sphere.Radius;
                float intersectionPointMaxY = this.Sphere.Center.Y + this.Sphere.Radius;
                float intersectionPointMaxZ = this.Sphere.Center.Z + this.Sphere.Radius;
                float intersectionPointMinX = this.Sphere.Center.X - this.Sphere.Radius;
                float intersectionPointMinY = this.Sphere.Center.Y - this.Sphere.Radius;
                float intersectionPointMinZ = this.Sphere.Center.Z - this.Sphere.Radius;

                Vector3 min = new Vector3(intersectionPointMinX, intersectionPointMinY, intersectionPointMinZ);
                Vector3 max = new Vector3(intersectionPointMaxX, intersectionPointMaxY, intersectionPointMaxZ);

                Vector3 positionChange = po.MyObject.MyTransform.PositionChangeNormal;
                if (positionChange != Vector3.Zero)
                {
                    positionChange.Normalize();
                }

                float x, y, z;
                float x1, x2, y1, y2, z1, z2;
                x1 = x2 = y1 = y2 = z1 = z2 = 0.0f;
                x = y = z = 0.0f;

                x1 = boundingBox.Max.X - min.X;
                if (x1 < 0.0f || boundingBox.Max.X > max.X)
                {
                    x1 = 0.0f;
                }

                x2 = boundingBox.Min.X - max.X;
                if (x2 > 0.0f || boundingBox.Min.X < min.X)
                {
                    x2 = 0.0f;
                }

                y1 = boundingBox.Max.Y - min.Y;
                if (y1 < 0.0f || boundingBox.Max.Y > max.Y)
                {
                    y1 = 0.0f;
                }

                y2 = boundingBox.Min.Y - max.Y;
                if (y2 > 0.0f || boundingBox.Min.Y < min.Y)
                {
                    y2 = 0.0f;
                }

                z1 = boundingBox.Max.Z - min.Z;
                if (z1 < 0.0f || boundingBox.Max.Z > max.Z)
                {
                    z1 = 0.0f;
                }

                z2 = boundingBox.Min.Z - max.Z;
                if (z2 > 0.0f || boundingBox.Min.Z < min.Z)
                {
                    z2 = 0.0f;
                }

                x = Math.Abs(x1) > Math.Abs(x2) ? x1 : x2;
                y = Math.Abs(y1) > Math.Abs(y2) ? y1 : y2;
                z = Math.Abs(z1) > Math.Abs(z2) ? z1 : z2;

                if (po.Velocity.X == 0.0f)
                {
                    x *= positionChange.X;
                }
                if (po.Velocity.Y == 0.0f)
                {
                    y *= positionChange.Y;
                }
                if (po.Velocity.Z == 0.0f)
                {
                    z *= positionChange.Z;
                }

                this.IntersectionVector = new Vector3(x, y, z);

                return true;
            }

            return false;
        }

        #endregion
    }
}
