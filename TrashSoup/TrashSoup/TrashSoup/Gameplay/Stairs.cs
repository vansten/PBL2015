using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    public class Stairs : ObjectComponent
    {
        public Stairs(GameObject go) : base(go)
        {

        }

        public Stairs(GameObject go, Stairs s) : base(go)
        {

        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {
        }

        protected override void Start()
        {
        }

        public override void OnTriggerEnter(GameObject other)
        {
            if(other.UniqueID == 1)
            {
                if (other.MyPhysicalObject != null)
                {
                    other.MyPhysicalObject.Velocity = Vector3.Zero;
                    other.MyPhysicalObject.IsUsingGravity = false;
                }
                Vector3 position = other.MyTransform.Position;
                position.Y = this.MyObject.MyTransform.Position.Y + 1.075f;
                other.MyTransform.Position = position;
            }
            base.OnTriggerEnter(other);
        }

        public override void OnTriggerExit(GameObject other)
        {
            if(other.UniqueID == 1)
            {
                if (other.MyPhysicalObject != null)
                {
                    other.MyPhysicalObject.IsUsingGravity = true;
                }
            }
            base.OnTriggerExit(other);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();

            base.ReadXml(reader);

            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
        }
    }
}
