using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrashSoup.Engine
{
    public class MirrorMaterial : Material
    {
        #region constants

        public const int MIRROR_BUFFER_SIZE = 256;

        #endregion

        #region effectParameters

        protected EffectParameter epMirrorMap;

        #endregion

        #region variables

        protected static RenderTarget2D mirrorRenderTarget;

        protected GameTime tempGameTime;

        protected Camera myCamera;
        protected Camera tempCamera;

        #endregion

        #region properties

        protected static RenderTarget2D MirrorRenderTarget
        {
            get
            {
                if(mirrorRenderTarget == null)
                {
                    mirrorRenderTarget = new RenderTarget2D(
                        TrashSoupGame.Instance.GraphicsDevice,
                        MIRROR_BUFFER_SIZE,
                        MIRROR_BUFFER_SIZE,
                        false,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.BackBufferFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                        TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.MultiSampleCount,
                        RenderTargetUsage.DiscardContents
                        );
                }
                return mirrorRenderTarget;
            }
            set
            {
                mirrorRenderTarget = value;
            }
        }

        protected Texture2D MirrorMap { get; set; }

        #endregion

        #region methods

        public MirrorMaterial(string name, Effect effect)
            : base(name, effect)
        {
            tempGameTime = new GameTime();

            myCamera = new Camera((uint)name.GetHashCode(), name + "OwnCamera", new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f), MathHelper.PiOver2,
                0.1f,
                2000.0f);
        }

         public override void UpdateEffect(Matrix world, Matrix worldViewProj, LightAmbient amb, LightDirectional[] dirs, Vector3[] pointColors,
            Vector3[] pointSpeculars, float[] pointAttenuations, Vector3[] pointPositions, uint pointCount, Vector3 eyeVector, BoundingFrustumExtended frustum,
             GameTime gameTime)
        {
            if(!isRendering)
            {
                isRendering = true;

                TrashSoupGame.Instance.GraphicsDevice.SetRenderTarget(MirrorRenderTarget);
                
                SetupCamera(world);
                tempCamera = ResourceManager.Instance.CurrentScene.Cam;
                ResourceManager.Instance.CurrentScene.Cam = myCamera;

                ResourceManager.Instance.CurrentScene.DrawAll(tempGameTime);

                ResourceManager.Instance.CurrentScene.Cam = tempCamera;

                TrashSoupGame.Instance.GraphicsDevice.SetRenderTarget(null);
                isRendering = false;
                this.MirrorMap = MirrorRenderTarget;
               
                if(epMirrorMap != null)
                {
                    epMirrorMap.SetValue(this.MirrorMap);
                }
            }

            base.UpdateEffect(world, worldViewProj, amb, dirs, pointColors, pointSpeculars, pointAttenuations, pointPositions, pointCount, eyeVector, frustum, gameTime);
        }

        protected void SetupCamera(Matrix wm)
        {
            Camera currentCam = ResourceManager.Instance.CurrentScene.Cam;

            Vector3 cCamPos = currentCam.Position + currentCam.Translation;
            Vector3 cCamTgt = currentCam.Target;
            Vector3 objectPosition, objectScale;
            Quaternion objectRotation;
            wm.Decompose(out objectScale, out objectRotation, out objectPosition);

            Vector3 newTarget = objectPosition - cCamPos;
            newTarget = Vector3.Reflect(Vector3.Normalize(newTarget), Vector3.Normalize(Vector3.Transform(new Vector3(0.0f, 0.0f, -1.0f), objectRotation)));

            // transforming target as is our model transformed
            newTarget = newTarget + objectPosition;

            this.myCamera.Position = objectPosition;
            this.myCamera.Target = newTarget;

            myCamera.Update(tempGameTime);
        }

        protected override void AssignParamsInitialize()
        {
            base.AssignParamsInitialize();

            int pNameHash;
            int mmHc = ("MirrorMap").GetHashCode();
            foreach (EffectParameter p in MyEffect.Parameters)
            {
                pNameHash = p.Name.GetHashCode();
                if (pNameHash.Equals(mmHc))
                {
                    epMirrorMap = p;
                }
            }
        }

        #endregion
    }
}
