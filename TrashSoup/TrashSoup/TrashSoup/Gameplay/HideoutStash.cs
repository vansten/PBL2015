using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;
using Microsoft.Xna.Framework;

namespace TrashSoup.Gameplay
{
    public class HideoutStash : ObjectComponent
    {
        #region constants

        private const int MAX_TRASH = 2000;

        #endregion

        #region variables

        private int currentTrash;

        #endregion

        #region properties

        public int CurrentTrash 
        { 
            get
            {
                return currentTrash;
            }
            set
            {
                if(value >= 0 && value <= MAX_TRASH)
                {
                    currentTrash = value;
                }
                else
                {
                    Debug.Log("HideoutStash: Trash value is invalid!");
                }
            }
        }

        #endregion

        #region methods

        public HideoutStash(GameObject go) : base(go)
        {

        }

        public HideoutStash(GameObject go, HideoutStash hs) : base(go, hs)
        {
            CurrentTrash = hs.CurrentTrash;
        }

        public override void Update(GameTime gameTime)
        {
            //Debug.Log(CurrentTrash.ToString());
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, GameTime gameTime)
        {
            
        }

        protected override void Start()
        {
            
        }

        #endregion
        
    }
}
