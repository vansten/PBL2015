using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrashSoup.Engine
{
    public class Billboard : ObjectComponent, IXmlSerializable
    {
        #region constants
        private static VertexPositionTexture[] vertices = 
        {
            new VertexPositionTexture(new Vector3(-1.0f, 1.0f, 0.0f), new Vector2(0.0f, 0.0f)),
            new VertexPositionTexture(new Vector3(1.0f, 1.0f, 0.0f), new Vector2(1.0f, 0.0f)),
            new VertexPositionTexture(new Vector3(1.0f, -1.0f, 0.0f), new Vector2(1.0f, 1.0f)),
            new VertexPositionTexture(new Vector3(1.0f, -1.0f, 0.0f), new Vector2(0.0f, 1.0f))
        };

        private static int[] indices = 
        {
            0, 1, 2, 0, 2, 3
        };
        #endregion

        #region variables
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        #endregion

        #region properties
        public Material Mat { get; set; }
        public Vector2 Size { get; set; }
        #endregion

        #region methods

        public Billboard(GameObject go)
            : base(go)
        {

        }

        public Billboard(GameObject go, Billboard bi)
            : base(go, bi)
        {

        }

        public override void Update(GameTime gameTime)
        {
            // do nth
        }

        public override void Draw(Camera cam, Effect effect, GameTime gameTime)
        {
            if(this.Visible && Mat != null)
            {
                Camera camera;
                if (cam == null)
                    camera = ResourceManager.Instance.CurrentScene.Cam;
                else
                    camera = cam;

                Transform transform = MyObject.MyTransform;
                GraphicsDevice device = TrashSoupGame.Instance.GraphicsDevice;

                if (transform == null)
                    return;

                Mat.UpdateEffect(
                                 effect,
                                 transform.GetWorldMatrix(),
                                 transform.GetWorldMatrix() * camera.ViewProjMatrix,
                                 ResourceManager.Instance.CurrentScene.AmbientLight,
                                 ResourceManager.Instance.CurrentScene.DirectionalLights,
                                 MyObject.LightsAffecting,
                                 ResourceManager.Instance.CurrentScene.GetGlobalShadowMap(),
                                 ResourceManager.Instance.CurrentScene.GetPointLight0ShadowMap(),
                                 camera.Position + camera.Translation,
                                 camera.Bounds,
                                 null,
                                 gameTime);
                SetAdditionalParameters(camera);

                Mat.MyEffect.CurrentTechnique.Passes[0].Apply();

                device.SetVertexBuffer(vertexBuffer);
                device.Indices = indexBuffer;

                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount);

                device.SetVertexBuffer(null);
                device.Indices = null;
                Mat.FlushMaterialEffect();
            }
        }

        protected override void Start()
        {
            // nth
        }

        public override void Initialize()
        {
            vertexBuffer = new VertexBuffer(TrashSoupGame.Instance.GraphicsDevice,
                                            typeof(VertexPositionTexture),
                                            vertices.Count(),
                                            BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionTexture>(vertices);

            indexBuffer = new IndexBuffer(TrashSoupGame.Instance.GraphicsDevice,
                                            IndexElementSize.ThirtyTwoBits,
                                            indices.Count(),
                                            BufferUsage.WriteOnly);
            indexBuffer.SetData<int>(indices);

            base.Initialize();
        }

        private void SetAdditionalParameters(Camera cam)
        {
            EffectParameter ep;

            ep = Mat.MyEffect.Parameters["Size"];
            if (ep != null)
                ep.SetValue(this.Size);

            ep = Mat.MyEffect.Parameters["CameraUp"];
            if (ep != null)
                ep.SetValue(cam.Up);

            ep = Mat.MyEffect.Parameters["CameraRight"];
            if (ep != null)
                ep.SetValue(cam.Right);
        }

        #endregion
    }
}
