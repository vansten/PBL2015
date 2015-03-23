using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SkinningModelLibrary
{
    /// <summary>
    /// It's in charge of decoding bone position matrices from an animation clip.
    /// </summary>
    public class AnimationPlayer
    {
        #region variables

        // information about currently playing animation clip
        AnimationClip currentClipValue;
        TimeSpan currentTimeValue;
        int currentKeyframe;

        // current animation transform matrices
        Matrix[] boneTransforms;
        Matrix[] worldTransforms;
        Matrix[] skinTransfomrs;

        // backlink to the bind pose and skeleton hierarchy data
        SkinningData skinningDataValue;

        #endregion

        #region properties

        public AnimationClip CurrentClip
        {
            get { return currentClipValue; }
        }

        public TimeSpan CurrentTime
        {
            get { return currentTimeValue; }
        }

        #endregion

        #region methods

        public AnimationPlayer(SkinningData skinningData)
        {
            if (skinningData == null) throw new ArgumentNullException("skinningData");
            this.skinningDataValue = skinningData;
            boneTransforms = new Matrix[skinningData.BindPose.Count];
            worldTransforms = new Matrix[skinningData.BindPose.Count];
            skinTransfomrs = new Matrix[skinningData.BindPose.Count];
        }

        public void StartClip(AnimationClip clip)
        {
            if (clip == null) throw new ArgumentNullException("clip");

            currentClipValue = clip;
            currentTimeValue = TimeSpan.Zero;
            currentKeyframe = 0;

            // initialize bone transforms to the bind pose
            skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
        }

        public void Update(TimeSpan time, bool relativeToCurrentTime, Matrix rootTransform)
        {
            UpdateBoneTransforms(time, relativeToCurrentTime);
            UpdateWorldTransforms(rootTransform);
            UpdateSkinTransforms();
        }

        public void UpdateBoneTransforms(TimeSpan time, bool relativeToCurrentTime)
        {
            if(currentClipValue == null)
            {
                throw new InvalidOperationException("AnimationPlayer.Update was called before StartClip");
            }

            // update the animation position
            if(relativeToCurrentTime)
            {
                time += currentTimeValue;

                while (time >= currentClipValue.Duration)
                    time -= currentClipValue.Duration;
            }

            if((time < TimeSpan.Zero) || (time >= currentClipValue.Duration))
            {
                throw new ArgumentOutOfRangeException("time");
            }

            // if the positon moved backwards, reset the keyframe index
            if(time < currentTimeValue)
            {
                currentKeyframe = 0;
                skinningDataValue.BindPose.CopyTo(boneTransforms, 0);
            }

            currentTimeValue = time;

            // read keyframe matrices
            IList<Keyframe> keyframes = currentClipValue.Keyframes;

            Keyframe keyframe;
            while(currentKeyframe < keyframes.Count)
            {
                keyframe = keyframes[currentKeyframe];

                // stop when we've read up to the current time position
                if (keyframe.Time > currentTimeValue) break;

                boneTransforms[keyframe.Bone] = keyframe.Transform;

                currentKeyframe++;
            }
        }

        public void UpdateWorldTransforms(Matrix rootTransform)
        {
            // root bone
            worldTransforms[0] = boneTransforms[0] * rootTransform;

            // child bones
            int parentBone;
            for(int bone = 1; bone < worldTransforms.Length; ++bone)
            {
                parentBone = skinningDataValue.SkeletonHierarchy[bone];
                worldTransforms[bone] = boneTransforms[bone] * worldTransforms[parentBone];
            }
        }

        public void UpdateSkinTransforms()
        {
            for(int bone = 0; bone < skinTransfomrs.Length; ++bone)
            {
                skinTransfomrs[bone] = skinningDataValue.InverseBindPose[bone] * worldTransforms[bone];
            }
        }

        public Matrix[] GetBoneTransforms()
        {
            return boneTransforms;
        }

        public Matrix[] GetWorldTransforms()
        {
            return worldTransforms;
        }

        public Matrix[] GetSkinTransforms()
        {
            return skinTransfomrs;
        }

        #endregion
    }
}
