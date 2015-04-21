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
        const float DIRECTIONAL_SHADOW_RANGE = 20.0f;
        const int DIRECTIONAL_SHADOW_MAP_SIZE = 2048;

        #endregion

        #region variables

        private Vector3 lightDirection;
        private Camera shadowDrawCamera;

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

        }

        public LightDirectional(uint uniqueID, string name, Vector3 color, Vector3 specular, Vector3 direction, bool castShadows)
            : base(uniqueID, name)
        {
            this.LightColor = color;
            this.LightSpecularColor = specular;
            this.LightDirection = direction;
            this.CastShadows = castShadows;

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
        }

        public void GenerateShadowMap()
        {
            if (!this.CastShadows || TrashSoupGame.Instance.ActualRenderTarget != TrashSoupGame.Instance.DefaultRenderTarget)
            {
                return;
            }

            // setting up camera properly
            Camera cam = ResourceManager.Instance.CurrentScene.Cam;

            shadowDrawCamera.Target = cam.Direction * DIRECTIONAL_SHADOW_RANGE + (cam.Target + cam.Translation);
            shadowDrawCamera.Position = DIRECTIONAL_DISTANCE * new Vector3(-LightDirection.X, -LightDirection.Y, -LightDirection.Z) + (cam.Target + cam.Translation) + cam.Direction * DIRECTIONAL_SHADOW_RANGE;
            shadowDrawCamera.Update(null);
            ///////////////

            TrashSoupGame.Instance.ActualRenderTarget = ShadowMapRenderTarget2048;
            TrashSoupGame.Instance.GraphicsDevice.Clear(Color.Black);

            ResourceManager.Instance.CurrentScene.DrawAll(this.ShadowDrawCamera, ResourceManager.Instance.Effects[@"Effects\ShadowMapEffect"], TrashSoupGame.Instance.TempGameTime, false);

            TrashSoupGame.Instance.ActualRenderTarget = TrashSoupGame.Instance.DefaultRenderTarget;

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
