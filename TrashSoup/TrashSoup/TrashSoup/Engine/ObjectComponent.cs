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

        public GameObject MyObject;

        #endregion

        #region properties

        public bool Enabled { get; set; }
        public bool Visible { get; set; }

        #endregion

        #region methods
        public ObjectComponent(GameObject myObj)
        {
            this.MyObject = myObj;
            this.Enabled = true;
            this.Visible = true;
        }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);

        protected abstract void Start();

        public virtual void OnCollision(GameObject other)
        {

        }

        public virtual void OnTrigger(GameObject other)
        {

        }

        #endregion
    }
}
