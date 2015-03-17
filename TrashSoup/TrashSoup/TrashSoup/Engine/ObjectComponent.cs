using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TrashSoup.Engine
{
    public abstract class ObjectComponent
    {
        #region variables
        protected GameObject myObject;
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
