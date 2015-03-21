using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    class ExplosionParticle : Particle
    {
        public ExplosionParticle(GraphicsDevice graphicsDevice, Vector3 position,
            int lifeLeft, int roundTime, int numParticlesPerRound,
            Texture2D particleColorsTexture, ParticleSettings particleSettings,
            Effect particleEffect) : base(graphicsDevice, position, lifeLeft,
            roundTime, numParticlesPerRound, particleColorsTexture, particleSettings, particleEffect)
        {

        }

        public override void InitializeParticleVertices()
        {
            base.InitializeParticleVertices();

            Explosion();
        }

        private void Explosion()
        {
            //Loop until max particles
            for(int i = 0; i < particleSettings.maxParticles; ++i)
            {
                float size = (float)SingleRandom.Instance.rnd.NextDouble() * particleSettings.maxSize;

                //Set position, direction and size of particle
                vertices[i * 4] = new VertexPositionTexture(position, new Vector2(0, 0));
                vertices[(i * 4) + 1] = new VertexPositionTexture(new Vector3(position.X,
                    position.Y + size, position.Z), new Vector2(0, 1));
                vertices[(i * 4) + 2] = new VertexPositionTexture(new Vector3(position.X + size,
                    position.Y, position.Z), new Vector2(1, 0));
                vertices[(i * 4) + 3] = new VertexPositionTexture(new Vector3(position.X + size,
                    position.Y + size, position.Z), new Vector2(1, 1));

                Vector3 direction = new Vector3(
                    (float)SingleRandom.Instance.rnd.NextDouble() - 0.5f,
                    (float)SingleRandom.Instance.rnd.NextDouble() * 2 + 2,
                    0.0f);
                direction.Normalize();

                //Make sure that all particles move at random speeds
                direction *= (float)SingleRandom.Instance.rnd.NextDouble();

                //Set direction of particle
                vertexDirectionArray[i] = direction;

                //Set color of particle by getting a random color from the texture
                vertexColorArray[i] = colors[(
                    SingleRandom.Instance.rnd.Next(0, particleColorsTexture.Height) * particleColorsTexture.Width) +
                    SingleRandom.Instance.rnd.Next(0, particleColorsTexture.Width)];
            }
        }

        public override void SetEnabled()
        {
            lifeLeft = life;
            endOfDeadParticlesIndex = 0;
            endOfLiveParticlesIndex = 0;
            InitializeParticleVertices();
        }
    }
}
