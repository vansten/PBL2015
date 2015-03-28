using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    public class ParticleSystem
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
        #endregion

        #region methods
        public ParticleSystem(GraphicsDevice graphicsDevice,
            Texture2D texture, int particleCount,
            Vector2 particleSize, float lifespan,
            Vector3 wind, float fadeInTime)
        {
            this.particleCount = particleCount;
            this.particleSize = particleSize;
            this.lifespan = lifespan;
            this.graphicsDevice = graphicsDevice;
            this.wind = wind;
            this.texture = texture;
            this.fadeInTime = fadeInTime;

            //Create vertex and index buffers to accomodate all particles
            verts = new VertexBuffer(graphicsDevice, typeof(ParticleVertex),
                particleCount * 4, BufferUsage.WriteOnly);

            inds = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits,
                particleCount * 6, BufferUsage.WriteOnly);

            GenerateParticles();

            effect = TrashSoupGame.Instance.Content.Load<Effect>("Effects/ParticleEffect");

            start = DateTime.Now;
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

        public void Update()
        {
            float now = (float)(DateTime.Now - start).TotalSeconds;

            int startIndex = activeStart;
            int end = nActive;

            //For each particle marked as active
            for(int i = 0; i < end; i++)
            {
                //If this particle has gotten older than 'lifespan'
                if(particles[activeStart].StartTime < now - lifespan)
                {
                    //Advance the active particle start position past
                    //the particle's index and reduce the number of
                    //active particles

                    activeStart++;
                    nActive--;

                    if (activeStart == particles.Length)
                        activeStart = 0;
                }
            }

            //Update the vertex and index buffers
            verts.SetData<ParticleVertex>(particles);
            inds.SetData<int>(indices);
        }

        public void Draw()
        {
            //Set the vertex and index buffers to the graphics card
            graphicsDevice.SetVertexBuffer(verts);
            graphicsDevice.Indices = inds;

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
            graphicsDevice.BlendState = BlendState.AlphaBlend;
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
        #endregion
    }
}
