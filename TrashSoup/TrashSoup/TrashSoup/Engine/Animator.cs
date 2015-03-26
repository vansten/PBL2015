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

        protected Dictionary<string, AnimationPlayer> animationPlayers = new Dictionary<string, AnimationPlayer>();
        protected bool ifInterpolate = false;
        protected uint currentInterpolationTimeMS = 0;
        protected float interDirection = 1.0f;
        protected Stack<AnimatorState> animatorStack = new Stack<AnimatorState>();
        protected AnimatorState helperState;
        protected Matrix[] tmpTransforms = null;

        protected float currentInterpolation;
        protected float sentInterpolation;

        protected bool ifHelperInterpolate = false;
        protected uint hCurrentInterpolationTimeMS = 0;
        protected float hInterDirection = 1.0f;
        protected float hCurrentInterpolation = 0.0f;

        #endregion

        #region properties

        public SkinningData SkinningData { get; set; }
        public AnimatorState CurrentState { get; set; }
        public AnimatorState NewState { get; set; }
        public Dictionary<string, AnimatorState> AvailableStates { get; set; }

        public float CurrentInterpolation 
        { 
            get
            {
                return currentInterpolation;
            }
            set
            {
                if(!this.Locked && !this.ifInterpolate)
                {
                    currentInterpolation = value;
                }
                else
                {
                    sentInterpolation = value;
                }
            }
        }

        public bool Locked { get; set; }

        #endregion

        #region methods

        public Animator(GameObject go) : base(go)
        {
            AvailableStates = new Dictionary<string, AnimatorState>();
            this.CurrentInterpolation = 0.0f;
            this.Locked = false;
        }

        public Animator(GameObject go, Model baseAnim) : base(go)
        {
            this.Locked = false;
            this.CurrentInterpolation = 0.0f;
            SkinningData = baseAnim.Tag as SkinningData;
            if (SkinningData == null) throw new InvalidOperationException("LOD 0 doesn't contain skinning data tag");
            AvailableStates = new Dictionary<string, AnimatorState>();
            CurrentState = null;
            NewState = null;
        }

        public override void Update(GameTime gameTime)
        {
            if(ifInterpolate)
            {
                CalculateInterpolationAmount(gameTime, this.currentInterpolationTimeMS, this.interDirection, ref this.currentInterpolation);
                if(currentInterpolation >= 1.0f)
                {
                    this.CurrentState = NewState;
                    this.NewState = null;
                    this.currentInterpolation = 0.0f;
                    this.currentInterpolationTimeMS = 0;
                    this.ifInterpolate = false;
                }
                else if(currentInterpolation <= 0.0f)
                {
                    this.interDirection = 1.0f;
                    this.NewState = null;
                    this.currentInterpolation = 0.0f;
                    this.currentInterpolationTimeMS = 0;
                    this.ifInterpolate = false;
                }
            }

            if (ifHelperInterpolate)
            {
                CalculateInterpolationAmount(gameTime, this.hCurrentInterpolationTimeMS, this.hInterDirection, ref this.hCurrentInterpolation);
                if (hCurrentInterpolation >= 1.0f || hCurrentInterpolationTimeMS <= 0.0f)
                {
                    this.helperState = null;
                    this.hCurrentInterpolation = 0.0f;
                    this.hCurrentInterpolationTimeMS = 0;
                    this.ifHelperInterpolate = false;
                }
            }

            if (CurrentState != null)
            {
                CurrentState.Update(gameTime);
            }
            if (NewState != null)
            {
                NewState.Update(gameTime);
            }
            //Debug.Log(CurrentState.Name + " " + gameTime.TotalGameTime.Seconds.ToString());
            if(CurrentState.Type == AnimatorState.StateType.SINGLE)
            {
                if(CurrentState.IsFinished)
                {
                    this.Locked = false;
                    this.tmpTransforms = null;
                    if (animatorStack.Count == 2)
                    {
                        this.helperState = this.CurrentState;
                        this.NewState = animatorStack.Pop();
                        this.CurrentState = animatorStack.Pop();
                        this.ifHelperInterpolate = true;
                        this.hCurrentInterpolation = 0.0f;

                        foreach (KeyValuePair<uint, AnimatorState> kp in helperState.Transitions)
                        {
                            if (kp.Value.Name.Equals(CurrentState.Name))
                            {
                                this.hCurrentInterpolationTimeMS = kp.Key;
                            }
                        }


                    }
                    else if(animatorStack.Count == 1)
                    {
                        ChangeState(animatorStack.Pop().Name);
                    }
                }
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
            Matrix[] startTransforms = null;
            if (CurrentState != null)
            {
                startTransforms = CurrentState.Animation.GetSkinTransforms();
            }
            if(tmpTransforms != null && this.Locked && this.ifInterpolate)
            {
                startTransforms = tmpTransforms;
            }

            Matrix[] toReturn = GetTransformsInterpolated(startTransforms,
                (NewState != null) ? NewState.Animation.GetSkinTransforms() : null, currentInterpolation);

            if(ifHelperInterpolate && helperState != null)
            {
                return GetTransformsInterpolated(helperState.Animation.GetSkinTransforms(), toReturn, hCurrentInterpolation);
            }
            else
            {
                return toReturn;
            }
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
            CurrentState.Animation.StopClip();
        }

        public void PauseAnimation()
        {
            CurrentState.Animation.PauseClip();
        }

        public void ChangeState(string stateName)
        {
            if (this.Locked)
                return;

            AnimatorState newS = null;

            foreach (KeyValuePair<uint, AnimatorState> kp in CurrentState.Transitions)
            {
                if (kp.Value.Name.Equals(stateName))
                {
                    newS = kp.Value;
                    currentInterpolationTimeMS = kp.Key;
                }
            }

            if (newS == null)
            {
                Debug.Log("ANIMATOR ERROR: Transition not found in current state's dictionary");
                return;
            }

            if (newS.Type == AnimatorState.StateType.SINGLE)
            {
                tmpTransforms = GetSkinTransforms();
                newS.IsFinished = false;
                this.Locked = true;
                if (this.CurrentState != null) animatorStack.Push(this.CurrentState);
                if (this.NewState != null) animatorStack.Push(this.NewState);
            }

            this.NewState = newS;
            ifInterpolate = true;
            currentInterpolation = 0.0f;
            newS.Animation.StartClip();
        }

        public void ChangeState(string stateName, float startInterp)
        {
            if (this.Locked)
                return;

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
                Debug.Log("ANIMATOR ERROR: ChangeState: Transition not found in current state's dictionary");
                return;
            }

            this.NewState = newState;
            ifInterpolate = true;
            currentInterpolation = startInterp;
            newState.Animation.StartClip();
        }

        public void ChangeState(string oldState, string newState, float startInterp)
        {
            if (this.Locked)
                return;

            AnimatorState oldS = this.AvailableStates[oldState];
            AnimatorState newS = this.AvailableStates[newState];
            this.NewState = newS;
            this.CurrentState = oldS;
            ifInterpolate = true;
            currentInterpolation = startInterp;
            if (this.CurrentState.Animation.MyState == AnimationPlayer.animationStates.STOPPED)
                this.CurrentState.Animation.StartClip();
            if (this.NewState.Animation.MyState == AnimationPlayer.animationStates.STOPPED)
                this.NewState.Animation.StartClip();
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
                Debug.Log("ANIMATOR ERROR: SetBlendState: Transition not found in current state's dictionary");
                return;
            }

            if (this.Locked && this.animatorStack.Count == 1)
            {
                this.animatorStack.Push(newState);
            }

            if (this.Locked)
                return;

            this.NewState = newState;
            currentInterpolation = 0.0f;
            newState.Animation.StartClip();
        }

        public void RemoveBlendStateToCurrent()
        {
            if (this.Locked)
                return;

            if(this.CurrentState != null && this.NewState != null)
            {
                ChangeState(this.CurrentState.Name, this.NewState.Name, this.currentInterpolation);
                this.interDirection = -1.0f;
                uint newTime = 0;
                foreach (KeyValuePair<uint, AnimatorState> p in NewState.Transitions)
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
            if (this.Locked)
                return;

            if (this.CurrentState != null && this.NewState != null)
            {
                this.ChangeState(this.NewState.Name);
            }
            else
            {
                Debug.Log("ANIMATOR ERROR: Either currentState or blendState is NULL");
            }
        }

        protected void CalculateInterpolationAmount(GameTime gameTime, uint time, float direction, ref float interp)
        {
            float nextAmount = (gameTime.ElapsedGameTime.Milliseconds) / MathHelper.Max((float)time, 1.0f);
            interp += direction * nextAmount;
        }

        protected Matrix[] GetTransformsInterpolated(Matrix[] one, Matrix[] two, float interp)
        {
            if (one == null && two != null) return two;
            else if (two == null && one != null) return one;
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
                Matrix[] newMatrices = new Matrix[SkinningData.BindPose.Count];

                for (int j = 0; j < SkinningData.BindPose.Count; ++j)
                {
                    newMatrices[j] = Matrix.Lerp(one[j], two[j], MathHelper.Clamp(interp, 0.0f, 1.0f));
                }

                return newMatrices;
            }
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
