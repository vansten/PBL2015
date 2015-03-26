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

        protected AnimatorState newState;
        protected Dictionary<string, AnimationPlayer> animationPlayers = new Dictionary<string, AnimationPlayer>();
        protected bool ifInterpolate = false;
        protected uint currentInterpolationTimeMS = 0;
        protected float interDirection = 1.0f;

        #endregion

        #region properties

        public SkinningData SkinningData { get; set; }
        public AnimatorState CurrentState { get; set; }
        public Dictionary<string, AnimatorState> AvailableStates { get; set; }
        public float CurrentInterpolation { get; set; }

        #endregion

        #region methods

        public Animator(GameObject go) : base(go)
        {
            AvailableStates = new Dictionary<string, AnimatorState>();
            this.CurrentInterpolation = 0.0f;
        }

        public Animator(GameObject go, Model baseAnim) : base(go)
        {
            this.CurrentInterpolation = 0.0f;
            SkinningData = baseAnim.Tag as SkinningData;
            if (SkinningData == null) throw new InvalidOperationException("LOD 0 doesn't contain skinning data tag");
            AvailableStates = new Dictionary<string, AnimatorState>();
            CurrentState = null;
            newState = null;
        }

        public override void Update(GameTime gameTime)
        {
            if(ifInterpolate)
            {
                CalculateInterpolationAmount(gameTime, this.interDirection);
                if(CurrentInterpolation >= 1.0f)
                {
                    this.CurrentState = newState;
                    this.newState = null;
                    this.CurrentInterpolation = 0.0f;
                    this.currentInterpolationTimeMS = 0;
                    this.ifInterpolate = false;
                }
                else if(CurrentInterpolation <= 0.0f)
                {
                    this.interDirection = 1.0f;
                    this.newState = null;
                    this.CurrentInterpolation = 0.0f;
                    this.currentInterpolationTimeMS = 0;
                    this.ifInterpolate = false;
                }
            }

            if (CurrentState != null)
            {
                CurrentState.Update(gameTime);
            }
            if (newState != null)
            {
                newState.Update(gameTime);
            }
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
            return GetTransformsInterpolated((CurrentState != null) ? CurrentState.Animation : null, (newState != null) ? newState.Animation : null, CurrentInterpolation);
        }

        public void AddAnimationClip(KeyValuePair<string, AnimationClip> newClip)
        {
            this.SkinningData.AnimationClips.Add(newClip.Key, newClip.Value);
            this.animationPlayers.Add(newClip.Key, new AnimationPlayer(this.SkinningData, newClip.Key));
        }

        public void RemoveAnimatonClip(string key)
        {
            this.SkinningData.AnimationClips.Remove(key);
            animationPlayers.Remove(key);
        }

        public AnimationClip GetAnimationClip(string key)
        {
            return SkinningData.AnimationClips[key];
        }

        public AnimationPlayer GetAnimationPlayer(string key)
        {
            return animationPlayers[key];
        }

        public void StartAnimation()
        {
            CurrentState.Animation.StartClip();
        }

        public void StopAnimation()
        {
            throw new NotImplementedException();
        }

        public void PauseAnimation()
        {
            throw new NotImplementedException();
        }

        public void ResumeAnimation()
        {
            throw new NotImplementedException();
        }

        public void ChangeState(string stateName)
        {
            AnimatorState newState = null;

            foreach (KeyValuePair<uint, AnimatorState> kp in CurrentState.Transitions)
            {
                if (kp.Value.Name.Equals(stateName))
                {
                    newState = kp.Value;
                    currentInterpolationTimeMS = kp.Key;
                }
            }

            if (newState == null)
            {
                Debug.Log("Transition not found in current state's dictionary");
                return;
            }

            this.newState = newState;
            ifInterpolate = true;
            CurrentInterpolation = 0.0f;
            newState.Animation.StartClip();
        }

        public void ChangeState(string stateName, float startInterp)
        {
            AnimatorState newState = null;

            foreach (KeyValuePair<uint, AnimatorState> kp in CurrentState.Transitions)
            {
                if (kp.Value.Name.Equals(stateName))
                {
                    newState = kp.Value;
                    currentInterpolationTimeMS = kp.Key;
                }
            }

            if (newState == null)
            {
                Debug.Log("Transition not found in current state's dictionary");
                return;
            }

            this.newState = newState;
            ifInterpolate = true;
            CurrentInterpolation = startInterp;
            newState.Animation.StartClip();
        }

        public void ChangeState(string oldState, string newState, float startInterp)
        {
            AnimatorState oldS = this.AvailableStates[oldState];
            AnimatorState newS = this.AvailableStates[newState];
            this.newState = newS;
            this.CurrentState = oldS;
            ifInterpolate = true;
            CurrentInterpolation = startInterp;
            if (this.CurrentState.Animation.MyState == AnimationPlayer.animationStates.STOPPED)
                this.CurrentState.Animation.StartClip();
            if (this.newState.Animation.MyState == AnimationPlayer.animationStates.STOPPED)
                this.newState.Animation.StartClip();
        }

        public void SetBlendState(string stateName)
        {
            AnimatorState newState = null;

            foreach (KeyValuePair<uint, AnimatorState> kp in CurrentState.Transitions)
            {
                if (kp.Value.Name.Equals(stateName))
                {
                    newState = kp.Value;
                    currentInterpolationTimeMS = kp.Key;
                }
            }

            if (newState == null)
            {
                Debug.Log("ANIMATOR ERROR: Transition not found in current state's dictionary");
                return;
            }

            this.newState = newState;
            CurrentInterpolation = 0.0f;
            newState.Animation.StartClip();
        }

        public void RemoveBlendStateToCurrent()
        {
            if(this.CurrentState != null && this.newState != null)
            {
                ChangeState(this.CurrentState.Name, this.newState.Name, this.CurrentInterpolation);
                this.interDirection = -1.0f;
                uint newTime = 0;
                foreach (KeyValuePair<uint, AnimatorState> p in newState.Transitions)
                {
                    if (p.Value == CurrentState) newTime = p.Key;
                }
                this.currentInterpolationTimeMS = newTime;
            }
            else
            {
                Debug.Log("ANIMATOR ERROR: Either currentState or blendState is NULL");
            }
        }

        public void RemoveBlendStateToNew()
        {
            if (this.CurrentState != null && this.newState != null)
            {
                this.ChangeState(this.newState.Name);
            }
            else
            {
                Debug.Log("ANIMATOR ERROR: Either currentState or blendState is NULL");
            }
        }

        protected void CalculateInterpolationAmount(GameTime gameTime, float direction)
        {
            float nextAmount = (gameTime.ElapsedGameTime.Milliseconds) / MathHelper.Max((float)currentInterpolationTimeMS, 1.0f);
            this.CurrentInterpolation += direction * nextAmount;
        }

        protected Matrix[] GetTransformsInterpolated(AnimationPlayer one, AnimationPlayer two, float interp)
        {
            if (one == null && two != null) return two.GetSkinTransforms();
            else if (two == null && one != null) return one.GetSkinTransforms();
            else if (one == null && two == null)
            {
                Matrix[] oldMatrices = new Matrix[SkinningData.BindPose.Count];
                for (int i = 0; i < SkinningData.BindPose.Count; ++i)
                {
                    oldMatrices[i] = Matrix.Identity;
                }
                return oldMatrices;
            }
            else
            {
                Matrix[] oneM = one.GetSkinTransforms();
                Matrix[] twoM = two.GetSkinTransforms();
                Matrix[] newMatrices = new Matrix[SkinningData.BindPose.Count];

                for (int j = 0; j < SkinningData.BindPose.Count; ++j)
                {
                    newMatrices[j] = Matrix.Lerp(oneM[j], twoM[j], MathHelper.Clamp(interp, 0.0f, 1.0f));
                }

                return newMatrices;
            }
        }

        #endregion
    }
}
