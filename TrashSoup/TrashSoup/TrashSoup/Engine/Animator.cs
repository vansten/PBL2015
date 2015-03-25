using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinningModelLibrary;

namespace TrashSoup.Engine
{
    public class Animator : ObjectComponent
    {
        #region variables

        protected SkinningData skinningData;
        protected Dictionary<string, AnimationPlayer> animationPlayers = new Dictionary<string,AnimationPlayer>();

        #endregion

        #region methods

        public Animator(GameObject go) : base(go)
        {

        }

        public Animator(GameObject go, Model baseAnim) : base(go)
        {
            skinningData = baseAnim.Tag as SkinningData;
            if (skinningData == null) throw new InvalidOperationException("LOD 0 doesn't contain skinning data tag");

            animationPlayers.Add(skinningData.AnimationClips.Keys.ElementAt(0), new AnimationPlayer(skinningData, skinningData.AnimationClips.Keys.ElementAt(0), 1.0f));

            animationPlayers[skinningData.AnimationClips.Keys.ElementAt(0)].StartClip();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(KeyValuePair<string, AnimationPlayer> ap in animationPlayers)
                ap.Value.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
        }

        public override void Draw(GameTime gameTime)
        {
            // draw nothing
        }

        protected override void Start()
        {
            // do nothing again
        }

        public Matrix[] GetSkinTransforms()
        {
            return GetCurrentlyPlayedInterpolated();
        }

        public void AddAnimation(KeyValuePair<string, AnimationClip> newClip)
        {
            this.skinningData.AnimationClips.Add(newClip.Key, newClip.Value);
            this.animationPlayers.Add(newClip.Key, new AnimationPlayer(this.skinningData, newClip.Key, 1.0f));
        }

        public void AddAnimation(KeyValuePair<string, AnimationClip> newClip, float interpolation)
        {
            this.skinningData.AnimationClips.Add(newClip.Key, newClip.Value);
            this.animationPlayers.Add(newClip.Key, new AnimationPlayer(this.skinningData, newClip.Key, interpolation));
        }

        public void RemoveAnimaton(string key)
        {
            this.skinningData.AnimationClips.Remove(key);
            animationPlayers.Remove(key);
        }

        public void StartAnimation(string key)
        {
            animationPlayers[key].StartClip();
        }

        public void StopAnimation(string key)
        {
            throw new NotImplementedException();
        }

        public void PauseAnimation(string key)
        {
            throw new NotImplementedException();
        }

        public void ResumeAnimation(string key)
        {
            throw new NotImplementedException();
        }

        protected Matrix[] GetCurrentlyPlayedInterpolated()
        {
            Matrix[] interpolated;
            if(animationPlayers.Count > 1)
            {
                //tbc
                interpolated = animationPlayers.ElementAt(0).Value.GetSkinTransforms();
                return interpolated;
            }
            else if(animationPlayers.Count == 1)
            {
                interpolated = animationPlayers.ElementAt(0).Value.GetSkinTransforms();
                return interpolated;
            }
            else
            {
                interpolated = new Matrix[skinningData.BindPose.Count];
                for(int i = 0; i < skinningData.BindPose.Count; ++i)
                {
                    interpolated[i] = Matrix.Identity;
                }
                return interpolated;
            }
        }

        #endregion
    }
}
