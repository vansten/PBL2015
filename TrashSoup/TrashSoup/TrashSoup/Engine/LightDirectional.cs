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

        private const int CAMERA_COUNT = 3;
        private float[] CAM_DIRECTIONAL_DISTANCE_MULTIPLIERS = { 13.0f, 50.0f, 150.0f };
        private float[] CAM_DIRECTIONAL_BOUNDARIES = { 10.0f, 30.0f, 100.0f };
        private const float CAM_DIRECTIONAL_DISTANCE = 60.0f;
        private const float CAM_DIRECTIONAL_NEAR_PLANE = 0.02f;
        private const float CAM_DIRECTIONAL_FAR_PLANE = 120.0f;

        private const float DIRECTIONAL_SHADOW_RANGE = 15.0f;
        private const int DIRECTIONAL_SHADOW_MAP_SIZE = 1024;

        private const int BLUR_FACTOR = 1;
        
        #endregion

        #region variables

        private Vector3 lightColor;
        private Vector3 lightSpecular;
        private Vector3 lightDirection;
        private Camera[] shadowDrawCameras = new Camera[CAMERA_COUNT];
        private Matrix[] myProjs = new Matrix[CAMERA_COUNT];
        private BoundingFrustumExtended[] bfs = new BoundingFrustumExtended[CAMERA_COUNT];
        private bool varsSet = false;
        private SamplerState ss;
        private bool rtSwitched = false;

        private RenderTarget2D[] tempRenderTargets;
        private RenderTarget2D[] veryTempRenderTargets;
        private Matrix deferredOrthoMatrix;
        #endregion

        #region properties

        public RenderTarget2D[] ShadowMapRenderTargets { get; set; }

        public Vector3 LightColor
        { 
            get
            {
                if(Enabled)
                {
                    return lightColor;
                }
                else
                {
                    return new Vector3(0.0f, 0.0f, 0.0f);
                }
            }
            set
            {
                lightColor = value;
            }
        }
        public Vector3 LightSpecularColor 
        { 
            get
            {
                if(Enabled)
                {
                    return lightSpecular;
                }
                else
                {
                    return new Vector3(0.0f, 0.0f, 0.0f);
                }
            }
            set
            {
                lightSpecular = value;
            }
        }
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
        public Camera[] ShadowDrawCameras
        {
            get
            {
                return shadowDrawCameras;
            }
            private set
            {
                shadowDrawCameras = value;
            }
        }

        #endregion

        #region methods

        public LightDirectional(uint uniqueID, string name)
            : base(uniqueID, name)
        {
            ShadowMapRenderTargets = new RenderTarget2D[CAMERA_COUNT];
            tempRenderTargets = new RenderTarget2D[CAMERA_COUNT];

            ss = new SamplerState();
            ss.Filter = TextureFilter.Point;

            for (int i = 0; i < CAMERA_COUNT; ++i )
            {
                shadowDrawCameras[i] = new Camera(this.UniqueID + 32 + (uint)i, "", new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f),
                     MathHelper.PiOver2, 1.0f, CAM_DIRECTIONAL_NEAR_PLANE, CAM_DIRECTIONAL_FAR_PLANE);
                shadowDrawCameras[i].OrthoWidth = CAM_DIRECTIONAL_DISTANCE_MULTIPLIERS[i];
                shadowDrawCameras[i].OrthoHeight = CAM_DIRECTIONAL_DISTANCE_MULTIPLIERS[i];
                shadowDrawCameras[i].Ortho = true;

                ShadowMapRenderTargets[i] = new RenderTarget2D(
                            TrashSoupGame.Instance.GraphicsDevice,
                            DIRECTIONAL_SHADOW_MAP_SIZE,
                            DIRECTIONAL_SHADOW_MAP_SIZE,
                            false,
                            SurfaceFormat.Vector2,
                            TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                            TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.MultiSampleCount,
                            RenderTargetUsage.DiscardContents
                            );

                tempRenderTargets[i] = new RenderTarget2D(
                        TrashSoupGame.Instance.GraphicsDevice,
                        TrashSoupGame.Instance.Window.ClientBounds.Width / BLUR_FACTOR,
                        TrashSoupGame.Instance.Window.ClientBounds.Height / BLUR_FACTOR,
                        false,
                        SurfaceFormat.Vector2,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.MultiSampleCount,
                        RenderTargetUsage.DiscardContents
                        );
            }

                
            

            deferredOrthoMatrix = Matrix.CreateOrthographicOffCenter(0.0f, (float)TrashSoupGame.Instance.Window.ClientBounds.Width,
                    (float)TrashSoupGame.Instance.Window.ClientBounds.Height, 0, 0, 1);

            bool blurred = true;

            if(blurred)
            {
                SwitchRTs();
            }
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
            if (!this.CastShadows || TrashSoupGame.Instance.ActualRenderTarget != TrashSoupGame.Instance.DefaultRenderTarget || !this.Enabled)
            {
                return;
            }

            SetCamera();

            bool blurred = true;

            Effect myShadowEffect = ResourceManager.Instance.Effects[@"Effects\ShadowMapEffect"];
            Effect myBlurEffect = ResourceManager.Instance.Effects[@"Effects\POSTBoxBlurEffect"];
            SpriteBatch batch = TrashSoupGame.Instance.GetSpriteBatch();

            if(blurred)
            {
                myBlurEffect.Parameters["WorldViewProj"].SetValue(deferredOrthoMatrix);
                myBlurEffect.Parameters["ScreenWidth"].SetValue((float)TrashSoupGame.Instance.Window.ClientBounds.Width);
                myBlurEffect.Parameters["ScreenHeight"].SetValue((float)TrashSoupGame.Instance.Window.ClientBounds.Height);

                SwitchRTs();
            }

            // setting up camera properly
            Camera cam = ResourceManager.Instance.CurrentScene.Cam;
            RasterizerState rs = TrashSoupGame.Instance.GraphicsDevice.RasterizerState;
            SamplerState sp = TrashSoupGame.Instance.GraphicsDevice.SamplerStates[0];
            BlendState bs = TrashSoupGame.Instance.GraphicsDevice.BlendState;
            TrashSoupGame.Instance.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            TrashSoupGame.Instance.GraphicsDevice.SamplerStates[0] = ss;
            TrashSoupGame.Instance.GraphicsDevice.BlendState = BlendState.Opaque;

            for (int i = 0; i < CAMERA_COUNT; ++i )
            {
                TrashSoupGame.Instance.ActualRenderTarget = ShadowMapRenderTargets[i];
                TrashSoupGame.Instance.GraphicsDevice.Clear(Color.Black);
                ResourceManager.Instance.CurrentScene.DrawAll(ShadowDrawCameras[i], myShadowEffect, TrashSoupGame.Instance.TempGameTime, false);
                
                
                if(blurred)
                {
                    TrashSoupGame.Instance.ActualRenderTarget = tempRenderTargets[i];
                    TrashSoupGame.Instance.GraphicsDevice.Clear(Color.Black);

                    batch.Begin(SpriteSortMode.Texture, TrashSoupGame.Instance.GraphicsDevice.BlendState,
                        SamplerState.PointWrap, TrashSoupGame.Instance.GraphicsDevice.DepthStencilState,
                        TrashSoupGame.Instance.GraphicsDevice.RasterizerState, myBlurEffect);

                    batch.Draw(ShadowMapRenderTargets[i], new Rectangle(0, 0, TrashSoupGame.Instance.Window.ClientBounds.Width, TrashSoupGame.Instance.Window.ClientBounds.Height), Color.White);

                    batch.End();
                }

                TrashSoupGame.Instance.ActualRenderTarget = TrashSoupGame.Instance.DefaultRenderTarget;
            }

            TrashSoupGame.Instance.GraphicsDevice.SamplerStates[0] = sp;
            TrashSoupGame.Instance.GraphicsDevice.RasterizerState = rs;
            TrashSoupGame.Instance.GraphicsDevice.BlendState = bs;

            if (blurred)
            {
                SwitchRTs();
            }

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
            //ShadowMapRenderTargets[0].SaveAsJpeg(stream, 1024, 1024);
            //stream.Close();
        }

        private void SetCamera()
        {
            Camera cam = ResourceManager.Instance.CurrentScene.Cam;
            if (cam == null)
                return;
            else if(!varsSet)
            {
                myProjs[0] = Matrix.CreatePerspectiveFieldOfView(cam.FOV, cam.Ratio, cam.Near, CAM_DIRECTIONAL_BOUNDARIES[0]);
                myProjs[1] = Matrix.CreatePerspectiveFieldOfView(cam.FOV, cam.Ratio, CAM_DIRECTIONAL_BOUNDARIES[0], CAM_DIRECTIONAL_BOUNDARIES[1]);
                myProjs[2] = Matrix.CreatePerspectiveFieldOfView(cam.FOV, cam.Ratio, CAM_DIRECTIONAL_BOUNDARIES[1], CAM_DIRECTIONAL_BOUNDARIES[2]);
                bfs[0] = new BoundingFrustumExtended(cam.ViewMatrix * myProjs[0]);
                bfs[1] = new BoundingFrustumExtended(cam.ViewMatrix * myProjs[1]);
                bfs[2] = new BoundingFrustumExtended(cam.ViewMatrix * myProjs[2]);
                varsSet = true;
            }

            for (int i = 0; i < CAMERA_COUNT; ++i )
            {
                bfs[i].Matrix = cam.ViewMatrix * myProjs[i];

                Vector3[] corners = bfs[i].GetCorners();
                uint cornerCount = (uint)corners.Count();


                Vector3 middlePoint = Vector3.Zero;
                for (uint j = 0; j < cornerCount; ++j)
                {
                    middlePoint.X += corners[j].X;
                    middlePoint.Y += corners[j].Y;
                    middlePoint.Z += corners[j].Z;
                }
                middlePoint.X /= cornerCount;
                middlePoint.Y = Math.Max(middlePoint.Y, 0.0f);
                middlePoint.Y /= cornerCount;
                middlePoint.Z /= cornerCount;

                shadowDrawCameras[i].Position = middlePoint + CAM_DIRECTIONAL_DISTANCE * new Vector3(-LightDirection.X, -LightDirection.Y, -LightDirection.Z);
                shadowDrawCameras[i].Target = middlePoint;
                shadowDrawCameras[i].Update(null);
            }
        }

        private void SwitchRTs()
        {
            if(!rtSwitched)
            {
                veryTempRenderTargets = ShadowMapRenderTargets;
                ShadowMapRenderTargets = tempRenderTargets;
                tempRenderTargets = veryTempRenderTargets;
                rtSwitched = true;
            }
            else
            {
                veryTempRenderTargets = tempRenderTargets;
                tempRenderTargets = ShadowMapRenderTargets;
                ShadowMapRenderTargets = veryTempRenderTargets;
                rtSwitched = false;
            }
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
