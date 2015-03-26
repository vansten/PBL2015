using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkinningModelLibrary;
using Microsoft.Xna.Framework;

namespace TrashSoup.Engine
{
    public class AnimatorState
    {
        #region properties

        public string Name { get; set; }
        public AnimationPlayer Animation { get; set; }

        /// <summary>
        /// A transition is described by a time it takes in ms and an another animation that we transite to.
        /// </summary>
        public Dictionary<uint, AnimatorState> Transitions { get; set; }

        #endregion

        #region methods

        public AnimatorState()
        {
            this.Transitions = new Dictionary<uint, AnimatorState>();
            this.Transitions.Add(0, this);
        }

        public AnimatorState(string name, AnimationPlayer animation) : this()
        {
            this.Name = name;
            this.Animation = animation;
        }

        public void Update(GameTime gameTime)
        {
            Animation.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
        }

        #endregion
    }
}
