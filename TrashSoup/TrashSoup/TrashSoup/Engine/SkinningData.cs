﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace TrashSoup.Engine
{
    /// <summary>
    /// Combines all the data needed to render and animate a skinned object
    /// </summary>
    public class SkinningData
    {
        #region properties

        public Dictionary<string, AnimationClip> AnimationClips { get; private set; }
        public List<Matrix> BindPose { get; private set; }
        public List<Matrix> InverseBindPose { get; private set; }
        public List<int> SkeletonHierarchy { get; private set; }

        #endregion

        #region methods

        public SkinningData(Dictionary<string, AnimationClip> animationClips,
                            List<Matrix> bindPose, List<Matrix> inverseBindPose,
                            List<int> skeletonHierarchy)
        {
            this.AnimationClips = animationClips;
            this.BindPose = bindPose;
            this.InverseBindPose = inverseBindPose;
            this.SkeletonHierarchy = skeletonHierarchy;
        }

        #endregion
    }
}
