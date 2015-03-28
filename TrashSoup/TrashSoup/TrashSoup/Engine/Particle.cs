using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    class ParticleSettings
    {
        #region Variables
        //Size of particle
        public int maxSize;

        //Life of particles
        public int minLife, maxLife;

        //Particles per round
        public int minParticlesPerRound, maxParticlesPerRound;

        //Round time
        public int minRoundTime, maxRoundTime;

        //Number of particles
        public int minParticles, maxParticles;
        #endregion

        #region Methods
        public ParticleSettings(int maxSize, int minLife, int maxLife,
            int minParticlesPerRound, int maxParticlesPerRound,
            int minRoundTime, int maxRoundTime, int minParticles, int maxParticles)
        {
            this.maxSize = maxSize;
            this.minLife = minLife;
            this.maxLife = maxLife;
            this.minParticlesPerRound = minParticlesPerRound;
            this.maxParticlesPerRound = maxParticlesPerRound;
            this.minRoundTime = minRoundTime;
            this.maxRoundTime = maxRoundTime;
            this.minParticles = minParticles;
            this.maxParticles = maxParticles;
        }
        #endregion
    }

    class Particle
    {
        #region Variables
        //Settings
        protected ParticleSettings particleSettings;

        //Particle arrays and vertex buffer
        protected VertexPositionTexture[] vertices;
        protected Vector3[] vertexDirectionArray;
        protected Color[] vertexColorArray;
        protected Color[] colors;
        protected VertexBuffer particleVertexBuffer;

        /// <summary>
        /// The position from which new particles will emanate.
        /// </summary>
        protected Vector3 position;

        /// <summary>
        /// How much life is left before particles start being deleted. 
        /// </summary>
        protected int lifeLeft;
        protected int life;

        //Rounds and particle counts
        protected int numParticlesPerRound;
        protected int roundTime;
        protected int timeSinceLastRound = 0;

        //Vertex and graphics info
        protected GraphicsDevice graphicsDevice;

        //Effect
        protected Effect particleEffect;
        protected Effect billboardEffect;

        //Textures
        protected Texture2D particleColorsTexture;

        //Array indices
        protected int endOfLiveParticlesIndex = 0;
        protected int endOfDeadParticlesIndex = 0;
        #endregion

        #region Properties
        public bool IsDead { get { return endOfDeadParticlesIndex == particleSettings.maxParticles; } }
        #endregion

        #region Methods
        public Particle(GraphicsDevice graphicsDevice, Vector3 position,
            int lifeLeft, int roundTime, int numParticlesPerRound,
            Texture2D particleColorsTexture, ParticleSettings particleSettings,
            Effect particleEffect)
        {
            this.graphicsDevice = graphicsDevice;
            this.position = position;
            this.lifeLeft = lifeLeft;
            this.life = lifeLeft;
            this.roundTime = roundTime;
            this.numParticlesPerRound = numParticlesPerRound;
            this.particleColorsTexture = particleColorsTexture;
            this.particleSettings = particleSettings;
            this.particleEffect = particleEffect;
            this.billboardEffect = ResourceManager.Instance.Effects.ElementAt(1);

            InitializeParticleVertices();
        }

        /// <summary>
        /// 
        /// Will instantiate the particle arrays and vertex buffer, 
        /// setting positions, random directions, random colors, and random sizes for each particle.
        /// </summary>
        public virtual void InitializeParticleVertices()
        {
            //Instantiate all particle arrays
            vertices = new VertexPositionTexture[particleSettings.maxParticles * 4];
            vertexDirectionArray = new Vector3[particleSettings.maxParticles];
            vertexColorArray = new Color[particleSettings.maxParticles];

            //Get color data from colors texture
            colors = new Color[particleColorsTexture.Width *
                particleColorsTexture.Height];
            particleColorsTexture.GetData(colors);

            //Instantiate vertex buffer
            particleVertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), vertices.Length, BufferUsage.None); 

        }

        public virtual void Update(GameTime gameTime)
        {
            //Decrement life left until it's gone
            if (lifeLeft > 0)
            {
                lifeLeft -= gameTime.ElapsedGameTime.Milliseconds;
            }

            //Check if it's time for new round
            timeSinceLastRound += gameTime.ElapsedGameTime.Milliseconds;
            if(timeSinceLastRound > roundTime)
            {
                //New round - add and remove particles
                timeSinceLastRound -= roundTime;

                //Increment end of live particles index each round
                if(endOfLiveParticlesIndex < particleSettings.maxParticles)
                {
                    endOfLiveParticlesIndex += numParticlesPerRound;
                    if (endOfLiveParticlesIndex > particleSettings.maxParticles)
                        endOfLiveParticlesIndex = particleSettings.maxParticles;
                }
                if (lifeLeft <= 0)
                {
                    //Increment end of dead particles index each round
                    if (endOfDeadParticlesIndex < particleSettings.maxParticles)
                    {
                        endOfDeadParticlesIndex += numParticlesPerRound;
                        if (endOfDeadParticlesIndex > particleSettings.maxParticles)
                        {
                            endOfDeadParticlesIndex = particleSettings.maxParticles;
                        }
                    }
                }
            }

            //Update position of all live particles
            for(int i = endOfDeadParticlesIndex; i < endOfLiveParticlesIndex; ++i)
            {
                vertices[i * 4].Position += vertexDirectionArray[i];
                vertices[(i * 4) + 1].Position += vertexDirectionArray[i];
                vertices[(i * 4) + 2].Position += vertexDirectionArray[i];
                vertices[(i * 4) + 3].Position += vertexDirectionArray[i];
            }
        }

        public virtual void Draw(Camera camera) 
        {
            graphicsDevice.SetVertexBuffer(particleVertexBuffer);

            //Only draw if there are live particles
            if(Math.Abs(endOfLiveParticlesIndex - endOfDeadParticlesIndex) > 0)
            {
                for(int i = endOfDeadParticlesIndex; i < endOfLiveParticlesIndex; ++i)
                {
                    //Draw billboarded particles
                    billboardEffect.Parameters["World"].SetValue(Matrix.Identity);
                    billboardEffect.Parameters["View"].SetValue(camera.ViewMatrix);
                    billboardEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
                    billboardEffect.Parameters["CamPos"].SetValue(camera.Position + camera.GetDirection());
                    billboardEffect.Parameters["AllowedRotDir"].SetValue(new Vector3(1, 1, 0));
                    billboardEffect.Parameters["particleColor"].SetValue(
                         vertexColorArray[i].ToVector4());
                    billboardEffect.Parameters["BillboardTexture"].SetValue(ResourceManager.Instance.Textures[@"Textures\ParticleTest\Particle"]);

                    foreach (EffectPass pass in billboardEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        RasterizerState stat = new RasterizerState();
                        stat.CullMode = CullMode.None;
                        graphicsDevice.RasterizerState = stat;
                        graphicsDevice.DrawUserPrimitives<VertexPositionTexture>(
                            PrimitiveType.TriangleStrip, vertices, i * 4, 2);

                    }
                }
            }
        }

        /// <summary>
        /// 
        /// Important for looping the particle. Sets proper variables to default
        /// </summary>
        public virtual void SetEnabled()
        { 
        }
        #endregion
    }

    class Emitter
    {
        private Particle p;

        public Emitter(Particle p)
        {
            this.p = p;
        }

        public void RunEmitter(GameTime gameTime)
        {
            p.Update(gameTime);
            if (p.IsDead)
            {
                p.SetEnabled();
            }
        }

    }
}
