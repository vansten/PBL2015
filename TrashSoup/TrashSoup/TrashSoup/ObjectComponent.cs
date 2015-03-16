using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TrashSoup
{
    public abstract class ObjectComponent
    {
        #region variables
        GameObject myObject;
        #endregion
        #region methods
        public ObjectComponent(GameObject myObj)
        {
            this.myObject = myObj;
        }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);

        protected abstract void Start();

        #endregion
    }
}
