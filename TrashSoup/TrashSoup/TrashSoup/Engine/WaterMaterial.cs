﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrashSoup.Engine
{
    public class WaterMaterial : Material
    {
        #region constants

        public const float REFLECTION_BUFFER_SIZE_MULTIPLIER = 0.25f;
        public const float REFRACTION_BUFFER_SIZE_MULTIPLIER = 1.0f;

        #endregion

        #region variables

        protected static RenderTarget2D reflectionRenderTarget;
        protected static RenderTarget2D refractionRenderTarget;

        protected GameTime tempGameTime;

        protected Matrix reflectionMatrix;
        protected Vector2 tempWind;

        #endregion

        #region properties

        protected static RenderTarget2D ReflectionRenderTarget
        {
            get
            {
                if (reflectionRenderTarget == null)
                {
                    reflectionRenderTarget = new RenderTarget2D(
                        TrashSoupGame.Instance.GraphicsDevice,
                        (int)(TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth * REFLECTION_BUFFER_SIZE_MULTIPLIER),
                        (int)(TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight * REFLECTION_BUFFER_SIZE_MULTIPLIER),
                        false,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.MultiSampleCount,
                        RenderTargetUsage.DiscardContents
                        );
                }
                return reflectionRenderTarget;
            }
            set
            {
                reflectionRenderTarget = value;
            }
        }

        protected static RenderTarget2D RefractionRenderTarget
        {
            get
            {
                if (refractionRenderTarget == null)
                {
                    refractionRenderTarget = new RenderTarget2D(
                        TrashSoupGame.Instance.GraphicsDevice,
                        (int)(TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth * REFRACTION_BUFFER_SIZE_MULTIPLIER),
                        (int)(TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight * REFRACTION_BUFFER_SIZE_MULTIPLIER),
                        false,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.MultiSampleCount,
                        RenderTargetUsage.DiscardContents
                        );
                }
                return refractionRenderTarget;
            }
            set
            {
                refractionRenderTarget = value;
            }
        }

        protected Texture2D ReflectionMap { get; set; }
        protected Texture2D RefractionMap { get; set; }

        #endregion

        #region methods

        public WaterMaterial(string name, Effect effect)
            : base(name, effect)
        {
            tempGameTime = new GameTime();
        }

        public override void UpdateEffect(Matrix world, Matrix worldViewProj, LightAmbient amb, LightDirectional[] dirs, Vector3[] pointColors,
            Vector3[] pointSpeculars, float[] pointAttenuations, Vector3[] pointPositions, uint pointCount, Vector3 eyeVector, BoundingFrustumExtended frustum,
            GameTime gameTime)
        {
            if (!isRendering)
            {
                isRendering = true;

                DrawRefractionMap(world);

                DrawReflectionMap(world);

                tempWind += ResourceManager.Instance.CurrentScene.Params.Wind * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
                EffectParameter param = null;
                this.parameters.TryGetValue("WindVector", out param);
                if (param != null)
                {
                    param.SetValue(tempWind);
                }
                isRendering = false;
            }

            base.UpdateEffect(world, worldViewProj, amb, dirs, pointColors, pointSpeculars, pointAttenuations, pointPositions, pointCount, eyeVector, frustum, gameTime);
        }

        protected Vector4 CreatePlane(Matrix wm, bool clipSide)
        {
            Vector3 objectPosition, objectScale;
            Quaternion objectRotation;
            wm.Decompose(out objectScale, out objectRotation, out objectPosition);

            float planeHeight = -objectPosition.Y;
            Vector3 normal = new Vector3(0.0f, 1.0f, 0.0f);

            Vector4 planeCoeffs = new Vector4(normal, planeHeight);
            if (clipSide)
                planeCoeffs *= -1.0f;

            return planeCoeffs;
        }

        protected void DrawRefractionMap(Matrix wm)
        {
            Vector4 refractionClip = CreatePlane(wm, false);

            ResourceManager.Instance.CurrentScene.Cam.Bounds.AdditionalClip.D = refractionClip.W;
            ResourceManager.Instance.CurrentScene.Cam.Bounds.AdditionalClip.Normal.X = refractionClip.X;
            ResourceManager.Instance.CurrentScene.Cam.Bounds.AdditionalClip.Normal.Y = refractionClip.Y;
            ResourceManager.Instance.CurrentScene.Cam.Bounds.AdditionalClip.Normal.Z = refractionClip.Z;

            TrashSoupGame.Instance.GraphicsDevice.SetRenderTarget(RefractionRenderTarget);
            ResourceManager.Instance.CurrentScene.DrawAll(tempGameTime);
            TrashSoupGame.Instance.GraphicsDevice.SetRenderTarget(null);

            ResourceManager.Instance.CurrentScene.Cam.Bounds.ZeroAllAdditionals();

            this.RefractionMap = RefractionRenderTarget;

            EffectParameter param = null;
            this.parameters.TryGetValue("RefractionMap", out param);
            if (param != null)
            {
                param.SetValue(this.RefractionMap);
            }
        }

        protected void DrawReflectionMap(Matrix wm)
        {
            Vector4 refractionClip = CreatePlane(wm, true);
            float z = wm.Translation.Y;
            Camera cCam = ResourceManager.Instance.CurrentScene.Cam;

            cCam.Bounds.AdditionalClip.D = refractionClip.W;
            cCam.Bounds.AdditionalClip.Normal.X = refractionClip.X;
            cCam.Bounds.AdditionalClip.Normal.Y = refractionClip.Y;
            cCam.Bounds.AdditionalClip.Normal.Z = refractionClip.Z;

            Vector3 prevPos = cCam.Position;
            Vector3 prevTrans = cCam.Translation;
            Vector3 prevTgt = cCam.Target;

            cCam.Position = new Vector3(prevPos.X, -prevPos.Y + 2.0f*z , prevPos.Z);
            cCam.Translation = new Vector3(prevTrans.X, -prevTrans.Y + 2.0f*z, prevTrans.Z);
            cCam.Target = new Vector3(prevTgt.X, -prevTgt.Y + 2.0f*z, prevTgt.Z);

            cCam.Update(tempGameTime);
            reflectionMatrix = wm * cCam.ViewProjMatrix;

            TrashSoupGame.Instance.GraphicsDevice.SetRenderTarget(ReflectionRenderTarget);
            ResourceManager.Instance.CurrentScene.DrawAll(tempGameTime);
            TrashSoupGame.Instance.GraphicsDevice.SetRenderTarget(null);

            Debug.Log(cCam.Position.ToString());

            cCam.Position = prevPos;
            cCam.Translation = prevTrans;
            cCam.Target = prevTgt;

            cCam.Update(tempGameTime);

            cCam.Bounds.ZeroAllAdditionals();

            this.ReflectionMap = ReflectionRenderTarget;

            //System.IO.FileStream stream = new System.IO.FileStream("Dupa.jpg", System.IO.FileMode.Create);
            //this.ReflectionMap.SaveAsJpeg(stream, 1280, 720);
            //stream.Close();

            EffectParameter param = null;
            this.parameters.TryGetValue("ReflectViewProj", out param);
            if (param != null)
            {
                param.SetValue(this.reflectionMatrix);
            }

            param = null;
            this.parameters.TryGetValue("ReflectionMap", out param);
            if (param != null)
            {
                param.SetValue(this.ReflectionMap);
            }
        }

        #endregion
    }
}
