using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    public class ParticleSystem : ObjectComponent, IXmlSerializable
    {
        #region variables
        //Vertex and index buffers
        VertexBuffer verts;
        IndexBuffer inds;

        //Graphics device and effect
        GraphicsDevice graphicsDevice;
        Effect effect;

        //Particle settings
        int particleCount;
        Vector2 particleSize;
        float lifespan = 1;
        Vector3 wind;
        Texture2D texture;
        float fadeInTime;

        //Particles and indices
        ParticleVertex[] particles;
        int[] indices;

        //Queue variables
        int activeStart = 0, nActive = 0;

        //Time particle was created
        DateTime start;

        private Vector3 offset;
        private Vector3 randAngle;
        private bool isLooped;
        private bool isStopped;
        #endregion

        #region methods

        public ParticleSystem(GameObject obj) : base(obj)
        {
            this.particleCount = 400;
            this.particleSize = new Vector2(2);
            this.lifespan = 1;
            this.wind = Vector3.Zero;
            this.fadeInTime = 0.5f;
        }

        public ParticleSystem(GameObject obj,
            Texture2D texture, int particleCount,
            Vector2 particleSize, float lifespan,
            Vector3 wind, float fadeInTime) : base(obj)
        {
            this.particleCount = particleCount;
            this.particleSize = particleSize;
            this.lifespan = lifespan;
            this.wind = wind;
            this.texture = texture;
            this.fadeInTime = fadeInTime;
        }

        void GenerateParticles()
        {
            //Create particle and index arrays
            particles = new ParticleVertex[particleCount * 4];
            indices = new int[particleCount * 6];

            Vector3 z = Vector3.Zero;
            int x = 0;

            //Initialize particle settings and fill index and vertex arrays
            for(int i = 0; i < particleCount * 4; i += 4)
            {
                particles[i] = new ParticleVertex(z, new Vector2(0, 0),
                    z, 0, -1);
                particles[i + 1] = new ParticleVertex(z, new Vector2(0, 1),
                    z, 0, -1);
                particles[i + 2] = new ParticleVertex(z, new Vector2(1, 1),
                    z, 0, -1);
                particles[i + 3] = new ParticleVertex(z, new Vector2(1, 0),
                    z, 0, -1);

                //Add 6 indices to form two triangles
                indices[x++] = i;
                indices[x++] = i + 3;
                indices[x++] = i + 2;
                indices[x++] = i + 2;
                indices[x++] = i + 1;
                indices[x++] = i;
            }
        }

        /// <summary>
        /// 
        /// Marks another particle as active and applies the given settings to it
        /// </summary>
        /// <param name="position"></param>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        public void AddParticle(Vector3 position, Vector3 direction, float speed)
        {
            //If there are no active particles, give up
            if (nActive + 4 == particleCount * 4)
                return;

            //Determine the index at which this particle should be created
            int index = OffsetIndex(activeStart, nActive);
            nActive += 4;

            //Determine the start time
            float startTime = (float)(DateTime.Now - start).TotalSeconds;

            //Set the particle settings to each of the particle's vertices
            for(int i = 0; i < 4; i++)
            {
                particles[index + i].StartPosition = position;
                particles[index + i].Direction = direction;
                particles[index + i].Speed = speed;
                particles[index + i].StartTime = startTime;
            }
        }

        /// <summary>
        /// 
        /// Increases the 'start' parameter by 'count' positions
        /// </summary>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        int OffsetIndex(int start, int count)
        {
            for(int i = 0; i < count; i++)
            {
                start++;

                if (start == particles.Length)
                    start = 0;
            }
            return start;
        }

        public void Play()
        {
            activeStart = 0;
            nActive = 0;
            this.isStopped = false;
        }

        public void Stop()
        {
            this.isStopped = true;
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (InputManager.Instance.GetKeyboardButtonDown(Microsoft.Xna.Framework.Input.Keys.P))
            {
                Play();
            }
            if (InputManager.Instance.GetKeyboardButtonDown(Microsoft.Xna.Framework.Input.Keys.O))
            {
                Stop();
            }
            if (!TrashSoupGame.Instance.EditorMode && !isStopped)
            {
                float now = (float)(DateTime.Now - start).TotalSeconds;

                int startIndex = activeStart;
                int end = nActive;

                //For each particle marked as active
                for (int i = 0; i < end; i++)
                {
                    //If this particle has gotten older than 'lifespan'
                    if (particles[activeStart].StartTime < now - lifespan)
                    {
                        //Advance the active particle start position past
                        //the particle's index and reduce the number of
                        //active particles

                        if (activeStart == particles.Length - 1)
                        {
                            if (isLooped)
                                activeStart = -1;
                            else
                            {
                                isStopped = true;
                                return;
                            }
                        }

                        activeStart++;
                        nActive--;
                    }
                }

                //Update the vertex and index buffers
                verts.SetData<ParticleVertex>(particles);
                inds.SetData<int>(indices);

                randAngle = Vector3.Up + new Vector3(
                    -offset.X + (float)SingleRandom.Instance.rnd.NextDouble() * (offset.X - (-offset.X)),
                    -offset.Y + (float)SingleRandom.Instance.rnd.NextDouble() * (offset.Y - (-offset.Y)),
                    -offset.Z + (float)SingleRandom.Instance.rnd.NextDouble() * (offset.Z - (-offset.Z))
                    );
                AddParticle(this.MyObject.MyTransform.Position, randAngle, 20.0f);
            }
        }

        public override void Draw(Camera cam, Effect effect, GameTime gameTime)
        {
            if (!TrashSoupGame.Instance.EditorMode)
            {
                //Set the vertex and index buffers to the graphics card
                graphicsDevice.SetVertexBuffer(verts);
                graphicsDevice.Indices = inds;

                effect = TrashSoupGame.Instance.Content.Load<Effect>(@"Effects/ParticleEffect");

                //Set the effect parameters
                effect.Parameters["ParticleTexture"].SetValue(texture);
                effect.Parameters["View"].SetValue(ResourceManager.Instance.CurrentScene.Cam.ViewMatrix);
                effect.Parameters["Projection"].SetValue(ResourceManager.Instance.CurrentScene.Cam.ProjectionMatrix);
                effect.Parameters["Time"].SetValue((float)(DateTime.Now - start).TotalSeconds);
                effect.Parameters["Lifespan"].SetValue(lifespan);
                effect.Parameters["Wind"].SetValue(wind);
                effect.Parameters["Size"].SetValue(particleSize / 2.0f);
                effect.Parameters["Up"].SetValue(ResourceManager.Instance.CurrentScene.Cam.Up);
                effect.Parameters["Side"].SetValue(ResourceManager.Instance.CurrentScene.Cam.Right);
                effect.Parameters["FadeInTime"].SetValue(fadeInTime);

                //Enable blending render states
                //graphicsDevice.BlendState = BlendState.AlphaBlend;
                graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

                //Apply the effect
                effect.CurrentTechnique.Passes[0].Apply();

                //Draw the billboards
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                    0, 0, particleCount * 4, 0, particleCount * 2);

                //Un-set the buffers
                graphicsDevice.SetVertexBuffer(null);
                graphicsDevice.Indices = null;

                //Reset render states
                graphicsDevice.BlendState = BlendState.Opaque;
                graphicsDevice.DepthStencilState = DepthStencilState.Default;   
            }
        }

        protected override void Start()
        {
  
        }

        public override void Initialize()
        {
            particles = new ParticleVertex[particleCount * 4];
            indices = new int[particleCount * 6];

            //to serialize
            this.particleCount = 100;
            this.particleSize = new Vector2(2);
            this.lifespan = 1;
            this.wind = Vector3.Zero;
            this.fadeInTime = 0.5f;

            //to serialize
            this.isLooped = false;
            this.isStopped = true;

            //to serialize
            this.texture = TrashSoupGame.Instance.Content.Load<Texture2D>(@"Textures/ParticleTest/Particle");
            this.graphicsDevice = TrashSoupGame.Instance.GraphicsDevice;

            offset = new Vector3(MathHelper.ToRadians(10.0f));
            randAngle = Vector3.Up + new Vector3(
                -offset.X + (float)SingleRandom.Instance.rnd.NextDouble() * (offset.X - (-offset.X)),
                -offset.Y + (float)SingleRandom.Instance.rnd.NextDouble() * (offset.Y - (-offset.Y)),
                -offset.Z + (float)SingleRandom.Instance.rnd.NextDouble() * (offset.Z - (-offset.Z))
                );

            //Create vertex and index buffers to accomodate all particles
            verts = new VertexBuffer(graphicsDevice, typeof(ParticleVertex),
                particleCount * 4, BufferUsage.WriteOnly);

            inds = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits,
                particleCount * 6, BufferUsage.WriteOnly);

            GenerateParticles();

            effect = TrashSoupGame.Instance.Content.Load<Effect>("Effects/ParticleEffect");

            start = DateTime.Now;
        }

        public override XmlSchema GetSchema()
        {
            return base.GetSchema();
        }

        public override void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement();

            base.ReadXml(reader);

            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
        }
    }
}
