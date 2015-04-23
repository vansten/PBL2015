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

        const float DIRECTIONAL_DISTANCE = 20.0f;
        const float DIRECTIONAL_CAM_NEAR_PLANE = 0.2f;
        const float DIRECTIONAL_CAM_FAR_PLANE = 50.0f;
        const float DIRECTIONAL_SHADOW_RANGE = 15.0f;
        const int DIRECTIONAL_SHADOW_MAP_SIZE = 2048;
        const int BLUR_FACTOR = 2;
        const float BLUR_FACTOR_IN = 0.4f;
        
        #endregion

        #region variables

        private Vector3 lightDirection;
        private Camera shadowDrawCamera;
        private Effect myShadowEffect;
        private Effect myShadowBlurredEffect;
        private Effect myBlurEffect;

        private RenderTarget2D blurredRenderTarget;
        private RenderTarget2D blurredRenderTarget2;
        private RenderTarget2D tempRenderTarget;
        private Matrix orthoMatrix;

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

            this.blurredRenderTarget = new RenderTarget2D(
                        TrashSoupGame.Instance.GraphicsDevice,
                        DIRECTIONAL_SHADOW_MAP_SIZE / BLUR_FACTOR,
                        DIRECTIONAL_SHADOW_MAP_SIZE / BLUR_FACTOR,
                        false,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.MultiSampleCount,
                        RenderTargetUsage.DiscardContents
                        );

            this.blurredRenderTarget2 = new RenderTarget2D(
                        TrashSoupGame.Instance.GraphicsDevice,
                        DIRECTIONAL_SHADOW_MAP_SIZE / BLUR_FACTOR,
                        DIRECTIONAL_SHADOW_MAP_SIZE / BLUR_FACTOR,
                        false,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.MultiSampleCount,
                        RenderTargetUsage.DiscardContents
                        );

            this.orthoMatrix = Matrix.CreateOrthographicOffCenter(0.0f, (float)DIRECTIONAL_SHADOW_MAP_SIZE,
                    (float)DIRECTIONAL_SHADOW_MAP_SIZE, 0, 0, 1);

            myShadowEffect = ResourceManager.Instance.Effects[@"Effects\ShadowMapEffect"];
            myShadowBlurredEffect = ResourceManager.Instance.Effects[@"Effects\ShadowMapBlurredEffect"];
            myBlurEffect = ResourceManager.Instance.Effects[@"Effects\POSTBlurEffect"];
        }

        public LightDirectional(uint uniqueID, string name, Vector3 color, Vector3 specular, Vector3 direction, bool castShadows)
            : this(uniqueID, name)
        {
            this.LightColor = color;
            this.LightSpecularColor = specular;
            this.LightDirection = direction;
            this.CastShadows = castShadows;
        }

        public void GenerateShadowMap(bool ifBlurred)
        {
            if (!this.CastShadows || TrashSoupGame.Instance.ActualRenderTarget != TrashSoupGame.Instance.DefaultRenderTarget)
            {
                return;
            }

            if(tempRenderTarget != null)
            {
                ShadowMapRenderTarget2048 = tempRenderTarget;
                tempRenderTarget = null;
            }

            // setting up camera properly
            Camera cam = ResourceManager.Instance.CurrentScene.Cam;

            shadowDrawCamera.Target = cam.Direction * DIRECTIONAL_SHADOW_RANGE + (cam.Target + cam.Translation);
            shadowDrawCamera.Position = DIRECTIONAL_DISTANCE * new Vector3(-LightDirection.X, -LightDirection.Y, -LightDirection.Z) + (cam.Target + cam.Translation) + cam.Direction * DIRECTIONAL_SHADOW_RANGE;
            shadowDrawCamera.Update(null);
            ///////////////


            TrashSoupGame.Instance.ActualRenderTarget = ShadowMapRenderTarget2048;
            TrashSoupGame.Instance.GraphicsDevice.Clear(Color.Black);

            ResourceManager.Instance.CurrentScene.DrawAll(this.ShadowDrawCamera, this.myShadowEffect, TrashSoupGame.Instance.TempGameTime, false);

            TrashSoupGame.Instance.ActualRenderTarget = TrashSoupGame.Instance.DefaultRenderTarget;
            
            if(ifBlurred)
            {
                TrashSoupGame.Instance.ActualRenderTarget = blurredRenderTarget;
                TrashSoupGame.Instance.GraphicsDevice.Clear(Color.Black);
                ResourceManager.Instance.CurrentScene.DrawAll(null, this.myShadowBlurredEffect, TrashSoupGame.Instance.TempGameTime, false);
                TrashSoupGame.Instance.ActualRenderTarget = TrashSoupGame.Instance.DefaultRenderTarget;

                myBlurEffect.Parameters["WorldViewProj"].SetValue(orthoMatrix);
                myBlurEffect.Parameters["ScreenWidth"].SetValue(BLUR_FACTOR_IN * (float)TrashSoupGame.Instance.Window.ClientBounds.Width);
                myBlurEffect.Parameters["ScreenHeight"].SetValue(BLUR_FACTOR_IN * (float)TrashSoupGame.Instance.Window.ClientBounds.Height);

                myBlurEffect.CurrentTechnique = myBlurEffect.Techniques["BlurHorizontal"];

                TrashSoupGame.Instance.ActualRenderTarget = blurredRenderTarget2;
                TrashSoupGame.Instance.GraphicsDevice.Clear(Color.Black);
                SpriteBatch batch = TrashSoupGame.Instance.GetSpriteBatch();
                batch.Begin(SpriteSortMode.Texture, TrashSoupGame.Instance.GraphicsDevice.BlendState,
                    TrashSoupGame.Instance.GraphicsDevice.SamplerStates[1], TrashSoupGame.Instance.GraphicsDevice.DepthStencilState,
                    TrashSoupGame.Instance.GraphicsDevice.RasterizerState, myBlurEffect);

                batch.Draw(blurredRenderTarget, new Rectangle(0, 0, DIRECTIONAL_SHADOW_MAP_SIZE, DIRECTIONAL_SHADOW_MAP_SIZE), Color.White);

                batch.End();

                TrashSoupGame.Instance.ActualRenderTarget = TrashSoupGame.Instance.DefaultRenderTarget;

                myBlurEffect.CurrentTechnique = myBlurEffect.Techniques["BlurVertical"];

                TrashSoupGame.Instance.ActualRenderTarget = blurredRenderTarget;
                TrashSoupGame.Instance.GraphicsDevice.Clear(Color.Black);

                batch.Begin(SpriteSortMode.Texture, TrashSoupGame.Instance.GraphicsDevice.BlendState,
                    TrashSoupGame.Instance.GraphicsDevice.SamplerStates[1], TrashSoupGame.Instance.GraphicsDevice.DepthStencilState,
                    TrashSoupGame.Instance.GraphicsDevice.RasterizerState, myBlurEffect);

                batch.Draw(blurredRenderTarget2, new Rectangle(0, 0, DIRECTIONAL_SHADOW_MAP_SIZE, DIRECTIONAL_SHADOW_MAP_SIZE), Color.White);

                batch.End();

                TrashSoupGame.Instance.ActualRenderTarget = TrashSoupGame.Instance.DefaultRenderTarget;

                tempRenderTarget = ShadowMapRenderTarget2048;
                ShadowMapRenderTarget2048 = blurredRenderTarget;
            }

            //System.IO.FileStream stream = new System.IO.FileStream("Dupa.jpg", System.IO.FileMode.Create);
            //ShadowMapRenderTarget2048.SaveAsJpeg(stream, 1024, 1024);
            //stream.Close();
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
