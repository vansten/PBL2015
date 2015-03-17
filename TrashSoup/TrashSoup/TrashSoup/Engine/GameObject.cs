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
        #region staticVariables
        public enum LODStateEnum
        {
            HI,
            MED,
            LO
        }
        #endregion

        #region variables

        protected GraphicsDeviceManager graphicsManager;

        #endregion

        #region properties
        public uint UniqueID { get; protected set; }
        public string Name { get; protected set; }
        public List<string> Tags { get; set; }

        public LODStateEnum LODState { get; set; }   // <-- I think this will go to the CustomModel class
        public List<ObjectComponent> Components { get; set; }

        #endregion

        #region methods
        public GameObject(uint uniqueID, string name)
        {
            this.UniqueID = uniqueID;
            this.Name = name;

            Components = new List<ObjectComponent>();
            graphicsManager = TrashSoupGame.Instance.GraphicsManager;
            LODState = LODStateEnum.HI;
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach(ObjectComponent obj in Components)
            {
                obj.Update(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime)
        {
            foreach (ObjectComponent obj in Components)
            {
                obj.Draw(gameTime);
            }
        }
        #endregion
    }
}
