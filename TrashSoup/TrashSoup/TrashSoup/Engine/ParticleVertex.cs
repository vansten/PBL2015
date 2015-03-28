using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    struct ParticleVertex : IVertexType
    {
        #region variables
        Vector3 startPosition;
        Vector2 uv;
        Vector3 direction;
        float speed;
        float startTime;
        #endregion

        #region properties
        //Starting position of that particle (when t = 0)
        public Vector3 StartPosition
        {
            get { return startPosition; }
            set { startPosition = value; }
        }

        //UV coordinates used for texturing and to offset vertex in shader
        public Vector2 UV
        {
            get { return uv; }
            set { uv = value; }
        }

        //Movement direction of the particle
        public Vector3 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        //Speed of the particle in units/second
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        //Time since the particle was created that this
        //particle came into use
        public float StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
        #endregion

        #region methods
        public ParticleVertex(Vector3 StartPosition, Vector2 Uv, Vector3 Direction,
            float Speed, float StartTime)
        {
            this.startPosition = StartPosition;
            this.uv = Uv;
            this.direction = Direction;
            this.speed = Speed;
            this.startTime = StartTime;
        }

        public readonly static VertexDeclaration VertexDeclaration =
            new VertexDeclaration(
            //Position
                new VertexElement(0, VertexElementFormat.Vector3,
                    VertexElementUsage.Position, 0),
            //UV
                new VertexElement(12, VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate, 0),
            //Direction
                new VertexElement(20, VertexElementFormat.Vector3,
                    VertexElementUsage.TextureCoordinate, 1),
            //Speed
                new VertexElement(32, VertexElementFormat.Single,
                    VertexElementUsage.TextureCoordinate, 2),
            //Start time
                new VertexElement(36, VertexElementFormat.Single,
                    VertexElementUsage.TextureCoordinate, 3)
                );
        #endregion
    }
}
