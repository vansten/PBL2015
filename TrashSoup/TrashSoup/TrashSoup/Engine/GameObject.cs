using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace TrashSoup.Engine
{
    [Serializable]
    public class GameObject
    {
        #region variables

        private PhysicalObject myPhisicalObject;

        #endregion

        #region properties
        public uint UniqueID { get; protected set; }
        public string Name { get; protected set; }
        public List<string> Tags { get; set; }
        public bool Enabled { get; set; }
        public bool Visible { get; set; }

        public Transform MyTransform { get; set; }

        public PhysicalObject MyPhysicalObject 
        {
            get
            {
                return this.myPhisicalObject;
            }
            set
            {
                if(value == null)
                {
                    PhysicsManager.Instance.RemovePhysicalObject(this);
                }
                else
                {
                    PhysicsManager.Instance.AddPhysicalObject(this);
                }

                this.myPhisicalObject = value;
            }
        }

        public List<ObjectComponent> Components { get; set; }
        public GraphicsDeviceManager GraphicsManager { get; protected set; }

        #endregion

        #region methods
        public GameObject(uint uniqueID, string name)
        {
            this.UniqueID = uniqueID;
            this.Name = name;

            this.Components = new List<ObjectComponent>();
            this.GraphicsManager = TrashSoupGame.Instance.GraphicsManager;

            this.Enabled = true;
            this.Visible = true;
        }

        public virtual void Update(GameTime gameTime)
        {
            if(this.Enabled)
            {
                if(this.MyPhysicalObject != null)
                {
                    this.MyPhysicalObject.Update(gameTime);
                }

                foreach (ObjectComponent obj in Components)
                {
                    obj.Update(gameTime);
                }
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            if(this.Visible)
            {
                foreach (ObjectComponent obj in Components)
                {
                    obj.Draw(gameTime);
                }
            }
        }
        #endregion
    }
}
