using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace TrashSoup
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
        public readonly uint UniqueID;
        public readonly string Name;
        public List<string> Tags;

        public LODStateEnum LODState;   // <-- I think this will go to the CustomModel class
        public List<ObjectComponent> Components { get; set; }

        public GraphicsDeviceManager graphicsDevice;
        #endregion
        #region methods
        public GameObject(uint uniqueID, string name)
        {
            this.UniqueID = uniqueID;
            this.Name = name;

            LODState = LODStateEnum.HI;
        }

        public void Update(GameTime gameTime)
        {
            foreach(ObjectComponent obj in Components)
            {
                obj.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (ObjectComponent obj in Components)
            {
                obj.Draw(gameTime);
            }
        }
        #endregion
    }
}
