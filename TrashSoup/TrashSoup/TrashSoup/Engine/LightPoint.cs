using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TrashSoup.Engine
{
    public class LightPoint : GameObject, IXmlSerializable
    {
        #region constants

        const float POINT_CAM_NEAR_PLANE = 1.0f;
        const float POINT_CAM_FAR_PLANE = 50.0f;

        #endregion

        #region variables

        private float attenuation;

        #endregion

        #region properties

        public Vector3 LightColor { get; set; }
        public Vector3 LightSpecularColor { get; set; }
        public float Attenuation 
        { 
            get
            {
                return attenuation;
            }
            set
            {
                attenuation = value;
                if(MyTransform != null)
                {
                    attenuation *= MyTransform.Scale;
                }
            }
        }
        public bool CastShadows { get; set; }
        public RenderTargetCube ShadowMapRenderTarget512 { get; set; }
        public Camera[] Cameras { get; set; }

        #endregion

        #region methods

        public LightPoint(uint uniqueID, string name)
            : base(uniqueID, name)
        {
            
        }

        public LightPoint(uint uniqueID, string name, Vector3 lightColor, Vector3 lightSpecularColor, float attenuation, bool castShadows)
            : base(uniqueID, name)
        {
            this.LightColor = lightColor;
            this.LightSpecularColor = lightSpecularColor;
            this.Attenuation = attenuation;
            this.CastShadows = castShadows;
        }

        public void MultiplyAttenuationByScale()
        {
            if (MyTransform != null)
            {
                this.Attenuation *= this.MyTransform.Scale;
            }
            else
            {
                Debug.Log("LightPoint: No Transfrom attached");
            }
        }

        public void GenerateShadowMap()
        {
            if (!this.CastShadows || TrashSoupGame.Instance.ActualRenderTarget != TrashSoupGame.Instance.DefaultRenderTarget || Cameras == null)
            {
                return;
            }

            Effect myShadowEffect = ResourceManager.Instance.Effects[@"Effects\ShadowMapUnnormalizedEffect"];

            myShadowEffect.Parameters["LightPos"].SetValue(MyTransform.Position);
            for(int i = 0; i < 6 ; ++i)
            {
                TrashSoupGame.Instance.GraphicsDevice.SetRenderTarget(ShadowMapRenderTarget512, (CubeMapFace)i);
                TrashSoupGame.Instance.GraphicsDevice.Clear(Color.Black);
                ResourceManager.Instance.CurrentScene.DrawAll(Cameras[i], myShadowEffect, TrashSoupGame.Instance.TempGameTime, false);
            }
            TrashSoupGame.Instance.ActualRenderTarget = TrashSoupGame.Instance.DefaultRenderTarget;
        }

        public void SetupShadowRender()
        {
            this.Cameras = new Camera[6];

            Vector3 pos = this.MyTransform.Position;

            this.Cameras[0] = new Camera(0, "", Vector3.Zero, pos, Vector3.Right, Vector3.Up,
                MathHelper.PiOver2, 1.0f, POINT_CAM_NEAR_PLANE, POINT_CAM_FAR_PLANE);
            this.Cameras[1] = new Camera(0, "", Vector3.Zero, pos, Vector3.Left, Vector3.Up,
                MathHelper.PiOver2, 1.0f, POINT_CAM_NEAR_PLANE, POINT_CAM_FAR_PLANE);
            this.Cameras[2] = new Camera(0, "", Vector3.Zero, pos, Vector3.Up, Vector3.Backward,
                MathHelper.PiOver2, 1.0f, POINT_CAM_NEAR_PLANE, POINT_CAM_FAR_PLANE);
            this.Cameras[3] = new Camera(0, "", Vector3.Zero, pos, Vector3.Down, Vector3.Forward,
                MathHelper.PiOver2, 1.0f, POINT_CAM_NEAR_PLANE, POINT_CAM_FAR_PLANE);
            this.Cameras[4] = new Camera(0, "", Vector3.Zero, pos, Vector3.Forward, Vector3.Up,
                MathHelper.PiOver2, 1.0f, POINT_CAM_NEAR_PLANE, POINT_CAM_FAR_PLANE);
            this.Cameras[5] = new Camera(0, "", Vector3.Zero, pos, Vector3.Backward, Vector3.Up,
                MathHelper.PiOver2, 1.0f, POINT_CAM_NEAR_PLANE, POINT_CAM_FAR_PLANE); 

            this.ShadowMapRenderTarget512 = new RenderTargetCube(
                        TrashSoupGame.Instance.GraphicsDevice,
                        512,
                        false,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.MultiSampleCount,
                        RenderTargetUsage.DiscardContents
                        );

            this.MyTransform.PositionChanged += new Transform.PositionChangedEventHandler(UpdateCameras);
        }

        private void UpdateCameras(object sender, EventArgs e)
        {
            foreach(Camera cam in Cameras)
            {
                cam.Translation = ((Transform)sender).Position;
                cam.Update(null);
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            CastShadows = reader.ReadElementContentAsBoolean("CastShadows", "");
            reader.ReadStartElement("Color");
            LightColor = new Vector3(reader.ReadElementContentAsFloat("R", ""),
                reader.ReadElementContentAsFloat("G", ""),
                reader.ReadElementContentAsFloat("B", ""));
            reader.ReadEndElement();

            reader.ReadStartElement("SpecularColor");
            LightSpecularColor = new Vector3(reader.ReadElementContentAsFloat("R", ""),
                reader.ReadElementContentAsFloat("G", ""),
                reader.ReadElementContentAsFloat("B", ""));
            reader.ReadEndElement();

            Attenuation = reader.ReadElementContentAsFloat("Attenuation", "");

            base.ReadXml(reader);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("CastShadows", XmlConvert.ToString(CastShadows));

            writer.WriteStartElement("Color");
            writer.WriteElementString("R", LightColor.X.ToString());
            writer.WriteElementString("G", LightColor.Y.ToString());
            writer.WriteElementString("B", LightColor.Z.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("SpecularColor");
            writer.WriteElementString("R", LightSpecularColor.X.ToString());
            writer.WriteElementString("G", LightSpecularColor.Y.ToString());
            writer.WriteElementString("B", LightSpecularColor.Z.ToString());
            writer.WriteEndElement();

            writer.WriteElementString("Attenuation", XmlConvert.ToString(Attenuation));

            base.WriteXml(writer);
        }

        #endregion
    }
}
