﻿using System;
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

        protected const float PLAYER_SPEED = 10.0f;
        protected const float SPRINT_MULTIPLIER = 5.0f;
        protected const float SPRINT_ACCELERATION = 3.0f;
        protected const float SPRINT_DECELERATION = 2.5f*SPRINT_ACCELERATION;
        protected const float ROTATION_SPEED = 0.2f;
        protected const float MAX_HEALTH = 50.0f;

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

        private bool collisionWithTrash = false;
        private bool collectedTrash = false;
        private double collisionFakeTime = 0.0;
        private double collectedFakeTime = 0.0;
        private GameObject trash;

        private bool collisionWithGround = false;

        private float hitPoints = MAX_HEALTH;
        private bool isDead = false;

        //for tetin
        private PlayerTime playerTime;

        #endregion

        #region properties

        public float HitPoints 
        { 
            get { return hitPoints; }
            set { hitPoints = value; }
        }

        public bool IsDead
        { 
            get { return isDead; }
            set { isDead = value; }
        }

        #endregion

        #region methods
        public PlayerController() { }


        public PlayerController(GameObject obj) : base(obj)
        {
            Start();
            playerTime = new PlayerTime(20, 55);
        }

        public PlayerController(GameObject obj, PlayerController pc) : base(obj)
        {
            Start();
            playerTime = new PlayerTime(20, 55);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(!TrashSoupGame.Instance.EditorMode)
            {
                playerTime.Start(gameTime);
                //for tetin if dynamical change of time works
                if (playerTime.Hours == 21)
                {
                    playerTime.Stop();
                    playerTime.SetTime(22, 50);
                    playerTime.Start(gameTime);
                }
                GUIManager.Instance.DrawText(TrashSoupGame.Instance.Content.Load<SpriteFont>("Fonts/FontTest"), "HEALTH: " + HitPoints.ToString(), new Vector2(0.1f, 0.3f), Color.Red);
                playerTime.Draw();
            }

            if (isDead)
                return;

            //FOR TETIN
            if (InputManager.Instance.GetKeyboardButtonDown(Keys.PageDown))
                DecreaseHealth(1);
            if (InputManager.Instance.GetKeyboardButtonDown(Keys.PageUp))
                IncreaseHealth(20);
            
            Vector2 movementVector = InputHandler.Instance.GetMovementVector();
            tempMove = new Vector3(movementVector.X,
                (InputManager.Instance.GetKeyboardButton(Keys.Q) ? 1.0f : 0.0f) - (InputManager.Instance.GetKeyboardButton(Keys.Z) ? 1.0f : 0.0f),
                movementVector.Y);

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
                rotation = CurveAngle(rotM, rotation, 3.0f*ROTATION_SPEED);                 // to się chyba gdzieś tu wywala ale nie jestem pewien
                tempMoveRotated = Vector3.Transform(tempMove, Matrix.CreateRotationY(rotation));

                if (float.IsNaN(tempMoveRotated.X) || float.IsNaN(tempMoveRotated.Y) || float.IsNaN(tempMoveRotated.Z))
                {
                    Debug.Log("PLAYERCONTROLLER ERROR: NaN detected in tempMoveRotated");
                }

                MyObject.MyTransform.Forward = Vector3.Lerp(prevForward, tempMoveRotated, ROTATION_SPEED);
                MyObject.MyTransform.Rotation = RotateAsForward(MyObject.MyTransform.Forward, MyObject.MyTransform.Rotation);
               
                if (InputHandler.Instance.IsSprinting() && tempMove.Length() >= 0.8f)
                {
                    sprint = MathHelper.Lerp(1.0f, SPRINT_MULTIPLIER, sprintM);
                    sprintM += SPRINT_ACCELERATION * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f);
                    sprintM = MathHelper.Min(sprintM, 1.0f);
                }
                else if(sprintM != 0.0f || sprint != 1.0f)
                {
                    sprint = MathHelper.Lerp(1.0f, SPRINT_MULTIPLIER, sprintM);
                    sprintM -= SPRINT_DECELERATION * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f);
                    sprintM = MathHelper.Max(sprintM, 0.0f);
                }

                MyObject.MyTransform.Position += (MyObject.MyTransform.Forward * PLAYER_SPEED * sprint * ((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f));
            }
            else
            {
                if (moving == true)
                {
                    moving = false;
                    MyObject.MyAnimator.RemoveBlendStateToCurrent();
                }
            }

            if (InputHandler.Instance.IsJumping())
            {
                if (MyObject.MyAnimator != null)
                {
                    if(MyObject.MyAnimator.ThirdState == null)
                    {
                        // jump!
                        //Debug.Log("Jump!");
                        MyObject.MyAnimator.ChangeState("Jump");
                        this.MyObject.MyPhysicalObject.IsUsingGravity = true;
                        this.MyObject.MyPhysicalObject.AddForce(Vector3.Up * 40.0f);
                    }
                }
            }

            if(this.collisionWithTrash)
            {
                if(!this.collectedTrash)
                {
                    this.collisionFakeTime = gameTime.TotalGameTime.TotalSeconds;
                    GUIManager.Instance.DrawText(TrashSoupGame.Instance.Content.Load<SpriteFont>("Fonts/FontTest"), "Click X on pad to collect trash", new Vector2(0.55f, 0.1f), Color.Red);
                }
            }

            if(!this.collectedTrash && this.collisionWithTrash && InputHandler.Instance.Action())
            {
                this.collectedTrash = true;
                this.collectedFakeTime = gameTime.TotalGameTime.TotalSeconds;
                this.trash.Enabled = false;
            }

            if(this.collectedTrash)
            {
                GUIManager.Instance.DrawText(TrashSoupGame.Instance.Content.Load<SpriteFont>("Fonts/FontTest"), "Trash collected", new Vector2(0.6f, 0.1f), Color.Red);
                if(gameTime.TotalGameTime.TotalSeconds - this.collectedFakeTime > 2.0)
                {
                    this.collectedTrash = false;
                }
            }

            if(!this.collisionWithGround)
            {
                this.MyObject.MyPhysicalObject.IsUsingGravity = true;
            }

            this.collisionWithTrash = false;
            this.collisionWithGround = false;
        }

        public override void Draw(Camera cam, Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Draw nothing
        }

        protected override void Start()
        {
            sprint = 1.0f;
            sprintM = 0.0f;

            if (MyObject == null) return;

            if(MyObject.MyAnimator != null)
            {
                MyObject.MyAnimator.AvailableStates.Add("Idle", new AnimatorState("Idle", MyObject.MyAnimator.GetAnimationPlayer("idle_1")));
                MyObject.MyAnimator.AvailableStates.Add("Walk", new AnimatorState("Walk", MyObject.MyAnimator.GetAnimationPlayer("walking_1")));
                MyObject.MyAnimator.AvailableStates.Add("Jump", new AnimatorState("Jump", MyObject.MyAnimator.GetAnimationPlayer("jump_1"), AnimatorState.StateType.SINGLE));
                MyObject.MyAnimator.AvailableStates["Idle"].AddTransition(MyObject.MyAnimator.AvailableStates["Walk"], new TimeSpan(0, 0, 0, 0, 200));
                MyObject.MyAnimator.AvailableStates["Idle"].AddTransition(MyObject.MyAnimator.AvailableStates["Jump"], new TimeSpan(0, 0, 0, 0, 500));
                MyObject.MyAnimator.AvailableStates["Walk"].AddTransition(MyObject.MyAnimator.AvailableStates["Idle"], new TimeSpan(0, 0, 0, 0, 250));
                MyObject.MyAnimator.AvailableStates["Walk"].AddTransition(MyObject.MyAnimator.AvailableStates["Jump"], new TimeSpan(0, 0, 0, 0, 200));
                MyObject.MyAnimator.AvailableStates["Jump"].AddTransition(MyObject.MyAnimator.AvailableStates["Walk"], new TimeSpan(0, 0, 0, 0, 100));
                MyObject.MyAnimator.AvailableStates["Jump"].AddTransition(MyObject.MyAnimator.AvailableStates["Idle"], new TimeSpan(0, 0, 0, 0, 500));
                MyObject.MyAnimator.CurrentState = MyObject.MyAnimator.AvailableStates["Idle"];
                //MyObject.MyAnimator.SetBlendState("Walk");
            }
        }

        public override void OnTrigger(GameObject other)
        {
            if(other.Name == "Trash")
            {
                this.collisionWithTrash = true;
                this.trash = other;
            }
            base.OnTrigger(other);
        }

        public override void OnCollision(GameObject other)
        {
            if(other.Name.Contains("Terrain"))
            {
                this.MyObject.MyPhysicalObject.IsUsingGravity = false;
                this.collisionWithGround = true;
            }
            base.OnCollision(other);
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

            float toReturn = (float)Math.Atan2(currentVector.Y, currentVector.X);

            return toReturn;
        }

        private Vector2 Slerp(Vector2 from, Vector2 to, float step)
        {
            if (step == 0) return from;
            if (from == to || step == 1) return to;

            double dot = (double)Vector2.Dot(from, to);

            // clampin'!
            if (dot > 1) dot = 1;
            else if (dot < -1) dot = -1;

            double theta = Math.Acos(dot);
            if (theta == 0) return to;

            double sinTheta = Math.Sin(theta);
            
            Vector2 toReturn = (float)(Math.Sin((1 - step) * theta) / sinTheta) * from + (float)(Math.Sin(step * theta) / sinTheta) * to;

            if(float.IsNaN(toReturn.X) || float.IsNaN(toReturn.Y))
            {
                Debug.Log("PLAYERCONTROLLER ERROR: NaN detected in Slerp()");
                throw new InvalidOperationException("PLAYERCONTROLLER ERROR: NaN detected in Slerp()");
            }

            return toReturn;
        }

        public void IncreaseHealth(float value)
        {
            if (value > MAX_HEALTH - HitPoints)
                HitPoints = MAX_HEALTH;
            else
                HitPoints += value;
        }

        public void DecreaseHealth(float value)
        {
            HitPoints -= value;
            if (HitPoints <= 0)
            {
                Debug.Log("YOU'RE DEAD");
                isDead = true;
            }
        }

        public override System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();

            base.ReadXml(reader);
            //MyObject = ResourceManager.Instance.CurrentScene.GetObject(tmp);

            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
        }

        #endregion
    }
}
