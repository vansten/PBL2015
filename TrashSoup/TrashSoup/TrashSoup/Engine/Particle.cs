using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrashSoup.Engine
{
    public class Particle
    {
        #region enums

        public enum ParticleState
        {
            FADING_IN,
            ACTIVE,
            FADING_OUT,
            INACTIVE
        }

        #endregion

        #region effectParameters

        private EffectParameter epWorldViewProj;
        private EffectParameter epDiffuseMap;
        private EffectParameter epDiffuseColor;
        private EffectParameter epPosition;
        private EffectParameter epSize;
        private EffectParameter epUp;
        private EffectParameter epSide;
        private EffectParameter epTransparency;
        private EffectParameter epRotation;

        #endregion

        #region variables

        private Effect myEffect;

        private float transparency = 0.0f;
        private float fadeInPerMs;
        private float fadeOutPerMs;
        private float timeLiving = 0.0f;

        #endregion

        #region properties

        public Vector3 StartPosition { get; set; }
        public Vector3 CurrentPosition { get; private set; }
        public Vector3 Speed { get; set; }
        public Vector3 DiffuseColor { get; set; }
        public Vector3 WindVariation { get; set; }
        public Vector2 Size { get; set; }
        public Texture2D Texture { get; set; }
        public ParticleState MyState { get; set; }
        public float FadeInMs { get; set; }
        public float FadeOutMs { get; set; }
        public float TimeToLiveMs { get; set; }
        public float Rotation { get; set; }

        #endregion

        #region methods

        public Particle(Effect e)
        {
            myEffect = e;

            DiffuseColor = Color.White.ToVector3();
        }

        public void Update(GameTime gameTime, Vector3 wind)
        {
            if(MyState != ParticleState.INACTIVE)
            {
                // update current time & position
                CurrentPosition += (Speed + wind + WindVariation) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                timeLiving += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                // update states if necessary
                if (timeLiving > FadeInMs && MyState == ParticleState.FADING_IN)
                {
                    MyState = ParticleState.ACTIVE;
                    transparency = 1.0f;
                }
                else if (timeLiving > (TimeToLiveMs - FadeOutMs) && MyState == ParticleState.ACTIVE)
                {
                    MyState = ParticleState.FADING_OUT;
                }
                else if (timeLiving > TimeToLiveMs && MyState == ParticleState.FADING_OUT)
                {
                    MyState = ParticleState.INACTIVE;
                    transparency = 0.0f;
                }

                // update transparencies
                if (MyState == ParticleState.FADING_IN)
                {
                    transparency = MathHelper.Lerp(0.0f, 1.0f, MathHelper.Clamp(timeLiving / MathHelper.Max(FadeInMs, 1.0f), 0.0f, 1.0f));
                }
                else if (MyState == ParticleState.FADING_OUT)
                {
                    transparency = MathHelper.Lerp(0.0f, 1.0f, MathHelper.Clamp((TimeToLiveMs - timeLiving) / MathHelper.Max(FadeOutMs, 1.0f), 0.0f, 1.0f));
                }
            }
        }

        public void Draw(Camera cam, Effect effect, GameTime gameTime, ref Matrix worldMatrix)
        {
            epWorldViewProj.SetValue(worldMatrix * cam.ViewProjMatrix);
            epDiffuseMap.SetValue(Texture);
            epDiffuseColor.SetValue(DiffuseColor);
            epPosition.SetValue(CurrentPosition);
            epSize.SetValue(Size / 2.0f);
            epUp.SetValue(-Vector3.Cross(cam.Direction, -cam.Right));
            epSide.SetValue(cam.Right);
            epTransparency.SetValue(transparency);
            epRotation.SetValue(Matrix.CreateRotationZ(Rotation));

            myEffect.CurrentTechnique.Passes[0].Apply();

            TrashSoupGame.Instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
        }

        public void Initialize()
        {
            MyState = ParticleState.FADING_IN;

            fadeInPerMs = 1.0f / MathHelper.Max(FadeInMs, 1.0f);
            fadeOutPerMs = 1.0f / MathHelper.Max(FadeOutMs, 1.0f);

            CurrentPosition = StartPosition;

            epWorldViewProj = myEffect.Parameters["WorldViewProj"];
            epDiffuseMap = myEffect.Parameters["DiffuseMap"];
            epDiffuseColor = myEffect.Parameters["DiffuseColor"];
            epPosition = myEffect.Parameters["Position"];
            epSize = myEffect.Parameters["Size"];
            epUp = myEffect.Parameters["Up"];
            epSide = myEffect.Parameters["Side"];
            epTransparency = myEffect.Parameters["Transparency"];
            epRotation = myEffect.Parameters["Rotation"];
        }

        public void Flush()
        {
            MyState = ParticleState.FADING_IN;
            transparency = 0.0f;
            timeLiving = 0.0f;
            CurrentPosition = StartPosition;
        }

        #endregion
    }
}
