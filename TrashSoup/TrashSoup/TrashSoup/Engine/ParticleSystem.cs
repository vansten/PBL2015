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

        //Particles and indices
        ParticleVertex[] particles;
        int[] indices;

        //Queue variables
        int activeStart = 0, nActive = 0;

        //Time particle was created
        DateTime start;

        private bool isLooped;
        private bool isStopped;

        // propertyz
        private Vector3 offset;

        #endregion

        #region properties

        public Vector3 Offset 
        { 
            get
            {
                return offset;
            }
            set
            {
                offset = value;

                RandAngle = Vector3.Up + new Vector3(
                -Offset.X + (float)SingleRandom.Instance.rnd.NextDouble() * (Offset.X - (-Offset.X)),
                -Offset.Y + (float)SingleRandom.Instance.rnd.NextDouble() * (Offset.Y - (-Offset.Y)),
                -Offset.Z + (float)SingleRandom.Instance.rnd.NextDouble() * (Offset.Z - (-Offset.Z))
                );
            }
        }
        public Vector3 RandAngle { get; private set; }
        public Vector3 Wind { get; set; }
        public Vector3 PositionOffset { get; set; }
        public Vector2 ParticleSize { get; set; }
        public Texture2D Texture { get; set; }
        public float Lifespan { get; set; }
        public float FadeInTime { get; set; }
        public float Speed { get; set; }
        public int ParticleCount { get; set; }
        public bool IgnoreScale { get; set; }

        #endregion

        #region methods

        public ParticleSystem(GameObject obj) : base(obj)
        {
            Start();
        }

        public ParticleSystem(GameObject obj, ParticleSystem ps) : base(obj, ps)
        {
            ParticleCount = ps.ParticleCount;
            ParticleSize = ps.ParticleSize;
            PositionOffset = ps.PositionOffset;
            Lifespan = ps.Lifespan;
            Wind = ps.Wind;
            FadeInTime = ps.FadeInTime;
            Texture = ps.Texture;
            Offset = ps.Offset;
            IgnoreScale = ps.IgnoreScale;
            Speed = ps.Speed;
        }

        protected override void Start()
        {
            this.ParticleCount = 400;
            this.ParticleSize = new Vector2(2);
            this.PositionOffset = Vector3.Zero;
            this.Lifespan = 1;
            this.Wind = Vector3.Zero;
            this.FadeInTime = 0.5f;
            this.Texture = null;
            this.Offset = new Vector3(MathHelper.ToRadians(10.0f));
            this.IgnoreScale = true;
            this.Speed = 20.0f;
        }

        void GenerateParticles()
        {
            //Create particle and index arrays
            particles = new ParticleVertex[ParticleCount * 4];
            indices = new int[ParticleCount * 6];

            Vector3 z = Vector3.Zero;
            int x = 0;

            //Initialize particle settings and fill index and vertex arrays
            for(int i = 0; i < ParticleCount * 4; i += 4)
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
            if (nActive + 4 == ParticleCount * 4)
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
                    if (particles[activeStart].StartTime < now - Lifespan)
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

                RandAngle = Vector3.Up + new Vector3(
                    -Offset.X + (float)SingleRandom.Instance.rnd.NextDouble() * (Offset.X - (-Offset.X)),
                    -Offset.Y + (float)SingleRandom.Instance.rnd.NextDouble() * (Offset.Y - (-Offset.Y)),
                    -Offset.Z + (float)SingleRandom.Instance.rnd.NextDouble() * (Offset.Z - (-Offset.Z))
                    );
                AddParticle(this.MyObject.MyTransform.Position, RandAngle, Speed);
            }
        }

        public override void Draw(Camera cam, Effect effect, GameTime gameTime)
        {
            if (!TrashSoupGame.Instance.EditorMode)
            {
                Camera camera = ResourceManager.Instance.CurrentScene.Cam;
                //if(camera == null)
                //{
                //    camera = ResourceManager.Instance.CurrentScene.Cam;
                //}

                //Set the vertex and index buffers to the graphics card
                graphicsDevice.SetVertexBuffer(verts);
                graphicsDevice.Indices = inds;

                effect = TrashSoupGame.Instance.Content.Load<Effect>(@"Effects/ParticleEffect");

                //Set the effect parameters
                effect.Parameters["ParticleTexture"].SetValue(Texture);

                Matrix world;
                world = MyObject.MyTransform.GetWorldMatrix();
                Vector3 trans, scl;
                Quaternion rot;
                world.Decompose(out scl, out rot, out trans);
                world = IgnoreScale ? Matrix.CreateTranslation(trans + PositionOffset) : (Matrix.CreateScale(scl) * Matrix.CreateTranslation(trans + PositionOffset));

                effect.Parameters["WorldViewProj"].SetValue(world * camera.ViewProjMatrix);
                effect.Parameters["Time"].SetValue((float)(DateTime.Now - start).TotalSeconds);
                effect.Parameters["Lifespan"].SetValue(Lifespan);
                effect.Parameters["Wind"].SetValue(Wind);
                effect.Parameters["Size"].SetValue(ParticleSize / 2.0f);
                effect.Parameters["Up"].SetValue(camera.Up);
                effect.Parameters["Side"].SetValue(camera.Right);
                effect.Parameters["FadeInTime"].SetValue(FadeInTime);

                //Enable blending render states
                //graphicsDevice.BlendState = BlendState.AlphaBlend;
                //graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

                //Apply the effect
                effect.CurrentTechnique.Passes[0].Apply();

                //Draw the billboards
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                    0, 0, ParticleCount * 4, 0, ParticleCount * 2);

                //Un-set the buffers
                graphicsDevice.SetVertexBuffer(null);
                graphicsDevice.Indices = null;

                //Reset render states
                //graphicsDevice.BlendState = BlendState.Opaque;
                //graphicsDevice.DepthStencilState = DepthStencilState.Default;   
            }
        }

        public override void Initialize()
        {
            particles = new ParticleVertex[ParticleCount * 4];
            indices = new int[ParticleCount * 6];

            //to serialize
            //this.particleCount = 100;
            //this.particleSize = new Vector2(2);
            //this.lifespan = 1;
            //this.wind = Vector3.Zero;
            //this.fadeInTime = 0.5f;

            //to serialize
            //this.isLooped = false;
            //this.isStopped = true;

            //to serialize
            //this.texture = TrashSoupGame.Instance.Content.Load<Texture2D>(@"Textures/ParticleTest/Particle");
            this.graphicsDevice = TrashSoupGame.Instance.GraphicsDevice;

            //Create vertex and index buffers to accomodate all particles
            verts = new VertexBuffer(graphicsDevice, typeof(ParticleVertex),
                ParticleCount * 4, BufferUsage.WriteOnly);

            inds = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits,
                ParticleCount * 6, BufferUsage.WriteOnly);

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

            ParticleCount = reader.ReadElementContentAsInt("ParticleCount", "");

            if (reader.Name == "ParticleSize")
            {
                reader.ReadStartElement();
                ParticleSize = new Vector2(reader.ReadElementContentAsFloat("X", ""),
                    reader.ReadElementContentAsFloat("Y", ""));
                reader.ReadEndElement();
            }

            if (reader.Name == "PositionOffset")
            {
                reader.ReadStartElement();
                PositionOffset = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                    reader.ReadElementContentAsFloat("Y", ""),
                    reader.ReadElementContentAsFloat("Z", ""));
                reader.ReadEndElement();
            }

            if (reader.Name == "Offset")
            {
                reader.ReadStartElement();
                Offset = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                    reader.ReadElementContentAsFloat("Y", ""),
                    reader.ReadElementContentAsFloat("Z", ""));
                reader.ReadEndElement();
            }

            Lifespan = reader.ReadElementContentAsInt("LifeSpan", "");

            if (reader.Name == "Wind")
            {
                reader.ReadStartElement();
                Wind = new Vector3(reader.ReadElementContentAsFloat("X", ""),
                    reader.ReadElementContentAsFloat("Y", ""),
                    reader.ReadElementContentAsFloat("Z", ""));
                reader.ReadEndElement();
            }

            FadeInTime = reader.ReadElementContentAsFloat("FadeInTime", "");
            isLooped = reader.ReadElementContentAsBoolean("IsLooped", "");
            isStopped = reader.ReadElementContentAsBoolean("IsStopped", "");
            IgnoreScale = reader.ReadElementContentAsBoolean("IgnoreScale", "");
            Speed = reader.ReadElementContentAsFloat("Speed", "");
            Texture = ResourceManager.Instance.LoadTexture(reader.ReadElementString("TexturePath", ""));

            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);
            writer.WriteElementString("ParticleCount", XmlConvert.ToString(ParticleCount));

            writer.WriteStartElement("ParticleSize");
            writer.WriteElementString("X", XmlConvert.ToString(ParticleSize.X));
            writer.WriteElementString("Y", XmlConvert.ToString(ParticleSize.Y));
            writer.WriteEndElement();

            writer.WriteStartElement("PositionOffset");
            writer.WriteElementString("X", XmlConvert.ToString(PositionOffset.X));
            writer.WriteElementString("Y", XmlConvert.ToString(PositionOffset.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(PositionOffset.Z));
            writer.WriteEndElement();

            writer.WriteStartElement("Offset");
            writer.WriteElementString("X", XmlConvert.ToString(Offset.X));
            writer.WriteElementString("Y", XmlConvert.ToString(Offset.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(Offset.Z));
            writer.WriteEndElement();

            writer.WriteElementString("LifeSpan", XmlConvert.ToString(Lifespan));
            
            writer.WriteStartElement("Wind");
            writer.WriteElementString("X", XmlConvert.ToString(Wind.X));
            writer.WriteElementString("Y", XmlConvert.ToString(Wind.Y));
            writer.WriteElementString("Z", XmlConvert.ToString(Wind.Z));
            writer.WriteEndElement();

            writer.WriteElementString("FadeInTime", XmlConvert.ToString(FadeInTime));
            writer.WriteElementString("IsLooped", XmlConvert.ToString(isLooped));
            writer.WriteElementString("IsStopped", XmlConvert.ToString(isStopped));
            writer.WriteElementString("IgnoreScale", XmlConvert.ToString(IgnoreScale));
            writer.WriteElementString("Speed", XmlConvert.ToString(Speed));
            writer.WriteElementString("TexturePath", ResourceManager.Instance.Textures.FirstOrDefault(x => x.Value == Texture).Key);
        }
    }
}
