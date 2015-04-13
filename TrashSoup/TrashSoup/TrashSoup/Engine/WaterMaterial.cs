using Microsoft.Xna.Framework;
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

        protected bool isRendering = false;
        protected GameTime tempGameTime;

        protected Camera refractionCamera;
        protected Camera reflectionCamera;
        protected Camera tempCamera;

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
                        DepthFormat.Depth24
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
                        DepthFormat.Depth24
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

            reflectionCamera = new Camera((uint)name.GetHashCode(), name + "ReflCamera", new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f), MathHelper.Pi / 3.0f,
                0.1f,
                2000.0f);

            refractionCamera = new Camera((uint)name.GetHashCode(), name + "RefrCamera", new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f), MathHelper.Pi / 3.0f,
                0.1f,
                2000.0f);
        }

        public override void UpdateEffect(Matrix world, Matrix worldViewProj, LightAmbient amb, LightDirectional[] dirs, Vector3[] pointColors,
            Vector3[] pointSpeculars, float[] pointAttenuations, Vector3[] pointPositions, uint pointCount, Vector3 eyeVector, BoundingFrustum frustum)
        {
            if (!isRendering)
            {
                isRendering = true;

                DrawRefractionMap(world);

                DrawReflectionMap(world);

                isRendering = false;
            }

            base.UpdateEffect(world, worldViewProj, amb, dirs, pointColors, pointSpeculars, pointAttenuations, pointPositions, pointCount, eyeVector, frustum);
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

            //Matrix currentView = ResourceManager.Instance.CurrentScene.Cam.ViewMatrix;
            //Matrix currentProj = ResourceManager.Instance.CurrentScene.Cam.ProjectionMatrix;
            //Matrix absViewProj = currentView * currentProj;
            //Matrix inverseAbsViewProj = Matrix.Invert(absViewProj);
            //inverseAbsViewProj = Matrix.Transpose(inverseAbsViewProj);

            //planeCoeffs = Vector4.Transform(planeCoeffs, inverseAbsViewProj);
            return planeCoeffs;
        }

        protected void DrawRefractionMap(Matrix wm)
        {
            Vector4 refractionClip = CreatePlane(wm, false);

            Matrix vm = Matrix.CreateLookAt(new Vector3(refractionClip.X, refractionClip.Y, refractionClip.Z) * refractionClip.W,
                new Vector3(refractionClip.X, refractionClip.Y, refractionClip.Z) * 2.0f * refractionClip.W, new Vector3(0.0f, 1.0f, 0.0f));

            Matrix pm = Matrix.CreatePerspectiveFieldOfView
            (
                MathHelper.Pi - 0.0001f,
                (float)TrashSoupGame.Instance.Window.ClientBounds.Width / (float)TrashSoupGame.Instance.Window.ClientBounds.Height,
                0.0001f,
                100.0f
            );

            refractionCamera.Position = ResourceManager.Instance.CurrentScene.Cam.Position;
            refractionCamera.Translation = ResourceManager.Instance.CurrentScene.Cam.Translation;
            refractionCamera.Target = ResourceManager.Instance.CurrentScene.Cam.Target;
            refractionCamera.Up = ResourceManager.Instance.CurrentScene.Cam.Up;

            refractionCamera.Update(tempGameTime);

            refractionCamera.Bounds.Matrix = vm * pm;

            tempCamera = ResourceManager.Instance.CurrentScene.Cam;
            ResourceManager.Instance.CurrentScene.Cam = refractionCamera;

            TrashSoupGame.Instance.GraphicsDevice.SetRenderTarget(RefractionRenderTarget);
            ResourceManager.Instance.CurrentScene.DrawAll(tempGameTime);
            TrashSoupGame.Instance.GraphicsDevice.SetRenderTarget(null);

            ResourceManager.Instance.CurrentScene.Cam = tempCamera;

            this.RefractionMap = RefractionRenderTarget;

            System.IO.FileStream stream = new System.IO.FileStream("dupa.jpg", System.IO.FileMode.Create);
            this.RefractionMap.SaveAsJpeg(stream, 1280, 720);
            stream.Close();

            EffectParameter param = null;
            this.parameters.TryGetValue("ReflectionMap", out param);
            if (param != null)
            {
                param.SetValue(this.ReflectionMap);
            }
        }

        protected void DrawReflectionMap(Matrix wm)
        {

        }

        #endregion
    }
}
