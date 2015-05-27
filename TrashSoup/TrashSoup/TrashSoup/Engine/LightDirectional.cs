using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    public class LightDirectional : GameObject, IXmlSerializable
    {
        #region constants

        const float DIRECTIONAL_DISTANCE = 60.0f;
        const float DIRECTIONAL_CAM_NEAR_PLANE = 0.02f;
        const float DIRECTIONAL_CAM_FAR_PLANE = 120.0f;
        const float DIRECTIONAL_SHADOW_RANGE = 15.0f;
        const int DIRECTIONAL_SHADOW_MAP_SIZE = 2048;

        const int BLUR_FACTOR = 1;
        
        #endregion

        #region variables

        private Vector3 lightDirection;
        private Camera shadowDrawCamera;
        private Matrix myProj;
        private BoundingFrustumExtended bf;
        private bool varsSet = false;

        private RenderTarget2D tempRenderTarget01;
        private RenderTarget2D tempRenderTarget02;
        private Matrix deferredOrthoMatrix;
        #endregion

        #region properties

        public RenderTarget2D ShadowMapRenderTarget2048 { get; set; }

        public Vector3 LightColor { get; set; }
        public Vector3 LightSpecularColor { get; set; }
        public Vector3 LightDirection 
        { 
            get
            {
                return lightDirection;
            }
            set
            {
                lightDirection = Vector3.Normalize(value);
            }
        }
        public bool CastShadows { get; set; }
        public Camera ShadowDrawCamera 
        {
            get
            {
                return shadowDrawCamera;
            }
            private set
            {
                shadowDrawCamera = value;
            }
        }

        #endregion

        #region methods

        public LightDirectional(uint uniqueID, string name)
            : base(uniqueID, name)
        {
            this.ShadowDrawCamera = new Camera(0, "", new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f),
                 MathHelper.PiOver2, 1.0f, DIRECTIONAL_CAM_NEAR_PLANE, DIRECTIONAL_CAM_FAR_PLANE);
            this.ShadowDrawCamera.OrthoWidth = DIRECTIONAL_SHADOW_MAP_SIZE / 1000 * DIRECTIONAL_DISTANCE;
            this.ShadowDrawCamera.OrthoHeight = DIRECTIONAL_SHADOW_MAP_SIZE / 1000 * DIRECTIONAL_DISTANCE;
            this.ShadowDrawCamera.Ortho = true;

            this.ShadowMapRenderTarget2048 = new RenderTarget2D(
                        TrashSoupGame.Instance.GraphicsDevice,
                        DIRECTIONAL_SHADOW_MAP_SIZE,
                        DIRECTIONAL_SHADOW_MAP_SIZE,
                        false,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.MultiSampleCount,
                        RenderTargetUsage.DiscardContents
                        );
            tempRenderTarget01 = new RenderTarget2D(
                        TrashSoupGame.Instance.GraphicsDevice,
                        TrashSoupGame.Instance.Window.ClientBounds.Width / BLUR_FACTOR,
                        TrashSoupGame.Instance.Window.ClientBounds.Height / BLUR_FACTOR,
                        false,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.MultiSampleCount,
                        RenderTargetUsage.DiscardContents
                        );

            deferredOrthoMatrix = Matrix.CreateOrthographicOffCenter(0.0f, (float)TrashSoupGame.Instance.Window.ClientBounds.Width,
                    (float)TrashSoupGame.Instance.Window.ClientBounds.Height, 0, 0, 1);

            //tempRenderTarget02 = ShadowMapRenderTarget2048;
            //ShadowMapRenderTarget2048 = tempRenderTarget01;

        }

        public LightDirectional(uint uniqueID, string name, Vector3 color, Vector3 specular, Vector3 direction, bool castShadows)
            : this(uniqueID, name)
        {
            this.LightColor = color;
            this.LightSpecularColor = specular;
            this.LightDirection = direction;
            this.CastShadows = castShadows;
        }

        public void GenerateShadowMap()
        {
            if (!this.CastShadows || TrashSoupGame.Instance.ActualRenderTarget != TrashSoupGame.Instance.DefaultRenderTarget)
            {
                return;
            }

            //// tu bedzie if
            //{
            //    tempRenderTarget01 = ShadowMapRenderTarget2048;
            //    ShadowMapRenderTarget2048 = tempRenderTarget02;
            //}

            SetCamera();

            Effect myShadowEffect = ResourceManager.Instance.Effects[@"Effects\ShadowMapEffect"];

            // setting up camera properly
            Camera cam = ResourceManager.Instance.CurrentScene.Cam;

            TrashSoupGame.Instance.ActualRenderTarget = ShadowMapRenderTarget2048;
            RasterizerState rs = TrashSoupGame.Instance.GraphicsDevice.RasterizerState;
            TrashSoupGame.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            TrashSoupGame.Instance.GraphicsDevice.Clear(Color.Black);

            ResourceManager.Instance.CurrentScene.DrawAll(this.ShadowDrawCamera, myShadowEffect, TrashSoupGame.Instance.TempGameTime, false);

            TrashSoupGame.Instance.ActualRenderTarget = TrashSoupGame.Instance.DefaultRenderTarget;
            TrashSoupGame.Instance.GraphicsDevice.RasterizerState = rs;

            //// tu bedzie if
            //{
            //    Effect myBlurEffect = ResourceManager.Instance.Effects[@"Effects\POSTLogBlurEffect"];

            //    myBlurEffect.Parameters["WorldViewProj"].SetValue(deferredOrthoMatrix);
            //    myBlurEffect.Parameters["ScreenWidth"].SetValue((float)TrashSoupGame.Instance.Window.ClientBounds.Width);
            //    myBlurEffect.Parameters["ScreenHeight"].SetValue((float)TrashSoupGame.Instance.Window.ClientBounds.Height);

            //    TrashSoupGame.Instance.ActualRenderTarget = tempRenderTarget01;
            //    TrashSoupGame.Instance.GraphicsDevice.Clear(Color.Black);
            //    SpriteBatch batch = TrashSoupGame.Instance.GetSpriteBatch();
            //    batch.Begin(SpriteSortMode.Texture, TrashSoupGame.Instance.GraphicsDevice.BlendState,
            //        TrashSoupGame.Instance.GraphicsDevice.SamplerStates[1], TrashSoupGame.Instance.GraphicsDevice.DepthStencilState,
            //        TrashSoupGame.Instance.GraphicsDevice.RasterizerState, myBlurEffect);

            //    batch.Draw(ShadowMapRenderTarget2048, new Rectangle(0, 0, TrashSoupGame.Instance.Window.ClientBounds.Width, TrashSoupGame.Instance.Window.ClientBounds.Height), Color.White);

            //    batch.End();

            //    TrashSoupGame.Instance.ActualRenderTarget = TrashSoupGame.Instance.DefaultRenderTarget;

            //    tempRenderTarget02 = ShadowMapRenderTarget2048;
            //    ShadowMapRenderTarget2048 = tempRenderTarget01;
            //}

            //System.IO.FileStream stream = new System.IO.FileStream("Dupa.jpg", System.IO.FileMode.Create);
            //ShadowMapRenderTarget2048.SaveAsJpeg(stream, 1024, 1024);
            //stream.Close();
        }

        private void SetCamera()
        {
            Camera cam = ResourceManager.Instance.CurrentScene.Cam;
            if (cam == null)
                return;
            else if(!varsSet)
            {
                myProj = Matrix.CreatePerspectiveFieldOfView(cam.FOV, cam.Ratio, cam.Near, 50.0f);
                bf = new BoundingFrustumExtended(cam.ViewMatrix * myProj);
                varsSet = true;
            }

            bf.Matrix = cam.ViewMatrix * myProj;

            Vector3[] corners = bf.GetCorners();
            uint cornerCount = (uint)corners.Count();

            //for (uint i = 0; i < cornerCount; ++i )
            //{
            //    corners[i] = Vector3.Transform(corners[i], shadowDrawCamera.ViewMatrix);
            //}
            Vector3 middlePoint = Vector3.Zero;
            for (uint i = 0; i < cornerCount; ++i)
            {
                middlePoint.X += corners[i].X;
                middlePoint.Y += corners[i].Y;
                middlePoint.Z += corners[i].Z;
            }
            middlePoint.X /= cornerCount;
            middlePoint.Y = Math.Max(middlePoint.Y, 0.0f);
            middlePoint.Y /= cornerCount;
            middlePoint.Z /= cornerCount;

            shadowDrawCamera.Position = middlePoint + DIRECTIONAL_DISTANCE * new Vector3(-LightDirection.X, -LightDirection.Y, -LightDirection.Z);
            shadowDrawCamera.Target = middlePoint;
            //shadowDrawCamera.Translation = cam.Translation;
            shadowDrawCamera.Update(null);
        }

        private float CurveAngle(float from, float to, float step)
        {
            if (step == 0) return from;
            if (from == to || step == 1) return to;

            Vector2 fromVector = new Vector2((float)Math.Cos(from), (float)Math.Sin(from));
            Vector2 toVector = new Vector2((float)Math.Cos(to), (float)Math.Sin(to));

            Vector2 currentVector = Slerp(fromVector, toVector, step);

            float toReturn = (float)Math.Atan2(currentVector.Y, currentVector.X);

            return toReturn;
        }

        private Vector2 Slerp(Vector2 from, Vector2 to, float step)
        {
            if (step == 0) return from;
            if (from == to || step == 1) return to;

            double dot = (double)Vector2.Dot(from, to);

            // clampin'!
            if (dot > 1) dot = 1;
            else if (dot < -1) dot = -1;

            double theta = Math.Acos(dot);
            if (theta == 0) return to;

            double sinTheta = Math.Sin(theta);
            
            Vector2 toReturn = (float)(Math.Sin((1 - step) * theta) / sinTheta) * from + (float)(Math.Sin(step * theta) / sinTheta) * to;

            if(float.IsNaN(toReturn.X) || float.IsNaN(toReturn.Y))
            {
                Debug.Log("PLAYERCONTROLLER ERROR: NaN detected in Slerp()");
                throw new InvalidOperationException("PLAYERCONTROLLER ERROR: NaN detected in Slerp()");
            }

            return toReturn;
        }
       
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            //reader.ReadStartElement();
            CastShadows = reader.ReadElementContentAsBoolean("CastShadows", "");

            reader.ReadStartElement("LightColor");
            LightColor = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                reader.ReadElementContentAsFloat("Y", ""),
                reader.ReadElementContentAsFloat("Z", ""));
            reader.ReadEndElement();

            reader.ReadStartElement("LightSpecularColor");
            LightSpecularColor = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                reader.ReadElementContentAsFloat("Y", ""),
                reader.ReadElementContentAsFloat("Z", ""));
            reader.ReadEndElement();

            reader.ReadStartElement("LightDirection");
            LightDirection = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                reader.ReadElementContentAsFloat("Y", ""),
                reader.ReadElementContentAsFloat("Z", ""));
            reader.ReadEndElement();

            base.ReadXml(reader);
        }

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("CastShadows", XmlConvert.ToString(CastShadows));

            writer.WriteStartElement("LightColor");
            writer.WriteElementString("X", XmlConvert.ToString(LightColor.X));
            writer.WriteElementString("Y", XmlConvert.ToString(LightColor.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(LightColor.Z));
            writer.WriteEndElement();

            writer.WriteStartElement("LightSpecularColor");
            writer.WriteElementString("X", XmlConvert.ToString(LightSpecularColor.X));
            writer.WriteElementString("Y", XmlConvert.ToString(LightSpecularColor.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(LightSpecularColor.Z));
            writer.WriteEndElement();

            writer.WriteStartElement("LightDirection");
            writer.WriteElementString("X", XmlConvert.ToString(LightDirection.X));
            writer.WriteElementString("Y", XmlConvert.ToString(LightDirection.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(LightDirection.Z));
            writer.WriteEndElement();

            base.WriteXml(writer);
        }

        #endregion
    }
}
