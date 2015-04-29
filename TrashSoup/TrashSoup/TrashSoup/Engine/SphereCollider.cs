using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    public class SphereCollider : Collider
    {
        #region Variables

        public BoundingSphere Sphere;

        private BoundingSphere initialSphere;
        private Vector3 center;
        private float radius;
        private CustomModel model;
        private List<Vector3> verticesToDraw = new List<Vector3>();
        private List<short> indices = new List<short>();

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

        public SphereCollider(GameObject go, SphereCollider sc) : base(go, sc)
        {

        }

        public override void Draw(Camera cam, Effect effect, GameTime gameTime)
        {
            if(TrashSoupGame.Instance.EditorMode)
            {
                verticesToDraw.Clear();
                indices.Clear();
                Vector3 right = Vector3.Right * this.Sphere.Radius;
                Vector3 verticalCirclePoint;
                Vector3 horizontalCirclePoint;
                Vector3 rightUp = Vector3.Right + Vector3.Up;
                Vector3 rightDown = Vector3.Right + Vector3.Down;
                rightUp.Normalize();
                rightDown.Normalize();
                rightDown *= this.Sphere.Radius;
                rightUp *= this.Sphere.Radius;
                Vector3 pointToAdd;
                for (short i = 0; i < 360; ++i)
                {
                    if (i - 1 > 0)
                    {
                        indices.Add((short)(i - 1));
                    }
                    verticalCirclePoint = Vector3.Transform(right, Matrix.CreateRotationZ(MathHelper.ToRadians(i)));
                    indices.Add(i);
                    verticalCirclePoint.X += this.MyObject.MyTransform.Position.X;
                    verticalCirclePoint.Y += this.MyObject.MyTransform.Position.Y;
                    verticalCirclePoint.Z -= this.MyObject.MyTransform.Position.Z;
                    verticesToDraw.Add(verticalCirclePoint);
                }

                for (short i = 0; i < 360; ++i)
                {
                    if (i - 1 > 0)
                    {
                        indices.Add((short)(360 + i - 1));
                    }
                    horizontalCirclePoint = Vector3.Transform(right, Matrix.CreateRotationY(MathHelper.ToRadians(i)));
                    indices.Add((short)(360 + i));
                    horizontalCirclePoint.X += this.MyObject.MyTransform.Position.X;
                    horizontalCirclePoint.Y += this.MyObject.MyTransform.Position.Y;
                    horizontalCirclePoint.Z -= this.MyObject.MyTransform.Position.Z;
                    verticesToDraw.Add(horizontalCirclePoint);
                }

                for (short i = 0; i < 360; ++i)
                {
                    if (i - 1 > 0)
                    {
                        indices.Add((short)(720 + i - 1));
                    }
                    pointToAdd = Vector3.Transform(rightUp, Matrix.CreateRotationZ(MathHelper.ToRadians(i)) * Matrix.CreateRotationY(MathHelper.ToRadians(45.0f)));
                    indices.Add((short)(720 + i));
                    pointToAdd.X += this.MyObject.MyTransform.Position.X;
                    pointToAdd.Y += this.MyObject.MyTransform.Position.Y;
                    pointToAdd.Z -= this.MyObject.MyTransform.Position.Z;
                    verticesToDraw.Add(pointToAdd);
                }

                for (short i = 0; i < 360; ++i)
                {
                    if (i - 1 > 0)
                    {
                        indices.Add((short)(1080 + i - 1));
                    }
                    pointToAdd = Vector3.Transform(rightDown, Matrix.CreateRotationZ(MathHelper.ToRadians(i)) * Matrix.CreateRotationY(MathHelper.ToRadians(-45.0f)));
                    indices.Add((short)(1080 + i));
                    pointToAdd.X += this.MyObject.MyTransform.Position.X;
                    pointToAdd.Y += this.MyObject.MyTransform.Position.Y;
                    pointToAdd.Z -= this.MyObject.MyTransform.Position.Z;
                    verticesToDraw.Add(pointToAdd);
                }

                Vector3[] vertices = verticesToDraw.ToArray();
                VertexPositionColor[] primitiveList = new VertexPositionColor[vertices.Length];
                for (int i = 0; i < vertices.Length; ++i)
                {
                    primitiveList[i] = new VertexPositionColor(vertices[i], Color.White);
                }

                BasicEffect lineEffect = new BasicEffect(TrashSoupGame.Instance.GraphicsDevice);
                lineEffect.LightingEnabled = false;
                lineEffect.TextureEnabled = false;
                lineEffect.VertexColorEnabled = true;

                GraphicsDevice gd = TrashSoupGame.Instance.GraphicsDevice;
                VertexBuffer buffer = new VertexBuffer(gd, typeof(VertexPositionColor), primitiveList.Length, BufferUsage.None);
                buffer.SetData(primitiveList);
                short[] indicesArray = indices.ToArray();
                IndexBuffer ib = new IndexBuffer(gd, IndexElementSize.SixteenBits, indicesArray.Length, BufferUsage.WriteOnly);
                ib.SetData(indicesArray);
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
                    gd.DrawUserIndexedPrimitives(PrimitiveType.LineList, primitiveList, 0, 1440, indicesArray, 0, 4 * 359);
                }

                base.Draw(cam, effect, gameTime);
            }
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
                            transformedPosition = Vector3.Transform(transformedPosition, mesh.ParentBone.Transform);
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
                            transformedPosition = Vector3.Transform(transformedPosition, mesh.ParentBone.Transform);

                            if(Vector3.Distance(this.center, transformedPosition) > this.radius)
                            {
                                this.radius = Vector3.Distance(this.center, transformedPosition);
                            }
                        }
                    }
                }
            }
            else
            {
                this.center = this.MyObject.MyTransform.Position;
                this.radius = 1.0f;
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

            if (!this.MyBoundingSphere.Intersects(poCollider.MyBoundingSphere))
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
