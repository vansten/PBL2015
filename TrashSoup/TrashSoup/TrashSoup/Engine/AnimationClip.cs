using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrashSoup.Engine
{
    /// <summary>
    /// Holds all the keyframes needed to describe a single animation.
    /// </summary>
    public class AnimationClip
    {
        #region properties

        public TimeSpan Duration { get; private set; }
        public List<Keyframe> Keyframes { get; private set; }

        #endregion

        #region methods

        public AnimationClip(TimeSpan duration, List<Keyframe> keyframes)
        {
            this.Duration = duration;
            this.Keyframes = keyframes;
        }

        #endregion
    }
}
