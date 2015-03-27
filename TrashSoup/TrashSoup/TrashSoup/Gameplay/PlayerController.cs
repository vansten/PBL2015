using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    public class PlayerController : ObjectComponent, IXmlSerializable
    {
        #region constants

        protected const float PLAYER_SPEED = 100.0f;
        protected const float SPRINT_MULTIPLIER = 5.0f;
        protected const float SPRINT_ACCELERATION = 3.0f;
        protected const float SPRINT_DECELERATION = 2.5f*SPRINT_ACCELERATION;
        protected const float ROTATION_SPEED = 0.2f;

        #endregion

        #region variables

        protected Vector3 tempMove;
        protected Vector3 tempMoveRotated;
        protected Vector3 prevForward;
        protected float nextForwardAngle;
        protected float rotM;
        protected float sprint;
        protected float sprintM;
        protected float rotation;

        protected float rotY;
        protected float prevRotY;

        protected bool moving = false;

        #endregion

        #region methods

        public PlayerController(GameObject obj) : base(obj)
        {
            Start();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            tempMove = new Vector3(InputManager.Instance.GetLeftStickValue().X,
                (InputManager.Instance.GetGamePadButton(Buttons.RightTrigger) ? 1.0f : 0.0f) - (InputManager.Instance.GetGamePadButton(Buttons.LeftTrigger) ? 1.0f : 0.0f),
                InputManager.Instance.GetLeftStickValue().Y);

            if(tempMove.Length() > 0.0f &&
                ResourceManager.Instance.CurrentScene.Cam != null)
            {
                if (moving == false)
                {
                    moving = true;

                    if (MyObject.MyAnimator != null)
                    {
                        MyObject.MyAnimator.SetBlendState("Walk");
                    }
                }
                if(moving == true)
                {
                    MyObject.MyAnimator.CurrentInterpolation = MathHelper.Clamp(tempMove.Length(), 0.0f, 1.0f);
                }
                // now to rotate that damn vector as camera direction is rotated
                rotM = rotation;
                prevForward = MyObject.MyTransform.Forward;

                rotation = (float)Math.Atan2(ResourceManager.Instance.CurrentScene.Cam.Direction.X,
                    -ResourceManager.Instance.CurrentScene.Cam.Direction.Z);
                rotation = CurveAngle(rotM, rotation, 3.0f*ROTATION_SPEED);
                tempMoveRotated = Vector3.Transform(tempMove, Matrix.CreateRotationY(rotation));

                MyObject.MyTransform.Forward = Vector3.Lerp(prevForward, tempMoveRotated, ROTATION_SPEED);
                MyObject.MyTransform.Rotation = RotateAsForward(MyObject.MyTransform.Forward, MyObject.MyTransform.Rotation);
               
                if (InputManager.Instance.GetGamePadButton(Buttons.B) && tempMove.Length() >= 0.8f)
                {
                    sprint = MathHelper.Lerp(1.0f, SPRINT_MULTIPLIER, sprintM);
                    sprintM += SPRINT_ACCELERATION * (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                    sprintM = MathHelper.Min(sprintM, 1.0f);
                }
                else if(sprintM != 0.0f || sprint != 1.0f)
                {
                    sprint = MathHelper.Lerp(1.0f, SPRINT_MULTIPLIER, sprintM);
                    sprintM -= SPRINT_DECELERATION * (gameTime.ElapsedGameTime.Milliseconds / 1000.0f);
                    sprintM = MathHelper.Max(sprintM, 0.0f);
                }

                MyObject.MyTransform.Position += (MyObject.MyTransform.Forward * PLAYER_SPEED * sprint * (gameTime.ElapsedGameTime.Milliseconds / 1000.0f));
            }
            else
            {
                if (moving == true)
                {
                    moving = false;
                    MyObject.MyAnimator.RemoveBlendStateToCurrent();
                }
            }

            if (InputManager.Instance.GetGamePadButton(Buttons.A))
            {
                if (MyObject.MyAnimator != null)
                {
                    if(MyObject.MyAnimator.CurrentState != null)
                    // jump!
                    MyObject.MyAnimator.ChangeState("Jump");
                }
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Draw nothing
        }

        protected override void Start()
        {
            sprint = 1.0f;
            sprintM = 0.0f;

            if(MyObject.MyAnimator != null)
            {
                MyObject.MyAnimator.AvailableStates.Add("Idle", new AnimatorState("Idle", MyObject.MyAnimator.GetAnimationPlayer("idle_1")));
                MyObject.MyAnimator.AvailableStates.Add("Walk", new AnimatorState("Walk", MyObject.MyAnimator.GetAnimationPlayer("walking_1")));
                MyObject.MyAnimator.AvailableStates.Add("Jump", new AnimatorState("Jump", MyObject.MyAnimator.GetAnimationPlayer("jump_1"), AnimatorState.StateType.SINGLE));
                MyObject.MyAnimator.AvailableStates["Idle"].Transitions.Add(500, MyObject.MyAnimator.AvailableStates["Walk"]);
                MyObject.MyAnimator.AvailableStates["Idle"].Transitions.Add(501, MyObject.MyAnimator.AvailableStates["Jump"]);
                MyObject.MyAnimator.AvailableStates["Walk"].Transitions.Add(250, MyObject.MyAnimator.AvailableStates["Idle"]);
                MyObject.MyAnimator.AvailableStates["Walk"].Transitions.Add(200, MyObject.MyAnimator.AvailableStates["Jump"]);
                MyObject.MyAnimator.AvailableStates["Jump"].Transitions.Add(100, MyObject.MyAnimator.AvailableStates["Walk"]);
                MyObject.MyAnimator.AvailableStates["Jump"].Transitions.Add(1001, MyObject.MyAnimator.AvailableStates["Idle"]);
                MyObject.MyAnimator.CurrentState = MyObject.MyAnimator.AvailableStates["Idle"];
                //MyObject.MyAnimator.SetBlendState("Walk");
            }
        }

        protected Vector3 RotateAsForward(Vector3 forward, Vector3 rotation)
        {
            this.prevRotY = this.rotY;
            this.rotY = (float)Math.Atan2(-forward.X, forward.Z);
            this.rotY = CurveAngle(prevRotY, rotY, ROTATION_SPEED);
            return rotation = new Vector3(rotation.X, rotY, rotation.Z);
        }

        private float CurveAngle(float from, float to, float step)
        {
            if (step == 0) return from;
            if (from == to || step == 1) return to;

            Vector2 fromVector = new Vector2((float)Math.Cos(from), (float)Math.Sin(from));
            Vector2 toVector = new Vector2((float)Math.Cos(to), (float)Math.Sin(to));

            Vector2 currentVector = Slerp(fromVector, toVector, step);

            return (float)Math.Atan2(currentVector.Y, currentVector.X);
        }

        private Vector2 Slerp(Vector2 from, Vector2 to, float step)
        {
            if (step == 0) return from;
            if (from == to || step == 1) return to;

            double theta = Math.Acos(Vector2.Dot(from, to));
            if (theta == 0) return to;

            double sinTheta = Math.Sin(theta);
            return (float)(Math.Sin((1 - step) * theta) / sinTheta) * from + (float)(Math.Sin(step * theta) / sinTheta) * to;
        }


        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            
        }

        #endregion
    }
}
