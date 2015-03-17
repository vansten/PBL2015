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

        #region properties

        public bool Enabled { get; set; }
        public bool Visible { get; set; }

        #endregion

        #region methods
        public ObjectComponent(GameObject myObj)
        {
            this.myObject = myObj;
            this.Enabled = true;
            this.Visible = true;
        }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);

        protected abstract void Start();

        #endregion
    }
}
