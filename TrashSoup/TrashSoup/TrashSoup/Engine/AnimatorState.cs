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
        #region enums

        public enum StateType
        {
            SINGLE,
            LOOPING
        }

        #endregion
        #region properties

        public string Name { get; set; }
        public AnimationPlayer Animation { get; set; }
        public StateType Type { get; set; }
        public bool IsFinished { get; set; }

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
            this.Type = StateType.LOOPING;
            this.IsFinished = false;
        }

        public AnimatorState(string name, AnimationPlayer animation) : this()
        {
            this.Name = name;
            this.Animation = animation;
            this.Animation.StartClip();
        }

        public AnimatorState(string name, AnimationPlayer animation, StateType type) : this(name, animation)
        {
            this.Type = type;
        }

        public void Update(GameTime gameTime)
        {
            if(this.Type == StateType.SINGLE)
            {
                if(Animation.CurrentTime >= Animation.GetDuration() - TimeSpan.FromMilliseconds(10))
                {
                    Animation.StopClip();
                    this.IsFinished = true;
                }
            }
            Animation.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
        }

        #endregion
    }
}
