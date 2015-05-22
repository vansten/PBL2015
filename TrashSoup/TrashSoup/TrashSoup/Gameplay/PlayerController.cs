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

        protected const float PLAYER_SPEED = 10.0f;
        protected const float SPRINT_MULTIPLIER = 5.0f;
        protected const float SPRINT_ACCELERATION = 3.0f;
        protected const float SPRINT_DECELERATION = 2.5f*SPRINT_ACCELERATION;
        protected const float ROTATION_SPEED = 0.2f;
        public const float MAX_HEALTH = 50.0f;
        public const float MAX_POPULARITY = 100.0f;

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

        private Equipment equipment;

        private float hitPoints = MAX_HEALTH;
        private float popularity = 0.0f;
        private float popularityDecreaseSpeed = 3.0f;

        private bool isDead = false;

        private Texture2D interactionTexture;
        private Vector2 textPosition = new Vector2(0.43f, 0.35f);

        #endregion

        #region properties

        public float HitPoints 
        { 
            get { return hitPoints; }
            set { hitPoints = value; }
        }

        public float Popularity
        {
            get { return popularity; }
            set { popularity = value; }
        }

        public bool IsDead
        { 
            get { return isDead; }
            set { isDead = value; }
        }

        public Equipment Equipment
        {
            get { return this.equipment; }
        }

        #endregion

        #region methods
        public PlayerController() { }


        public PlayerController(GameObject obj) : base(obj)
        {
            Start();
        }

        public PlayerController(GameObject obj, PlayerController pc) : base(obj)
        {
            Start();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (isDead)
                return;

            //FOR TETIN
            if (InputManager.Instance.GetKeyboardButtonDown(Keys.PageDown))
                DecreaseHealth(1);
            if (InputManager.Instance.GetKeyboardButtonDown(Keys.PageUp))
                IncreaseHealth(20);

            equipment.Update(gameTime);
            
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
                        this.MyObject.MyPhysicalObject.AddForce(Vector3.Up * 50.0f);
                    }
                }
            }

            if(InputHandler.Instance.IsAttacking())
            {
                if(MyObject.MyAnimator != null)
                {
                    if(MyObject.MyAnimator.ThirdState == null)
                    {
                        MyObject.MyAnimator.ChangeState("Jump");
                    }
                }
            }

            if(this.collisionWithTrash)
            {
                if(!this.collectedTrash)
                {
                    this.collisionFakeTime = gameTime.TotalGameTime.TotalSeconds;
                    GUIManager.Instance.DrawTexture(this.interactionTexture, new Vector2(0.475f, 0.775f), 0.05f, 0.05f);
                }
            }

            if(!this.collectedTrash && this.collisionWithTrash && InputHandler.Instance.Action())
            {
                this.collectedTrash = true;
                this.collectedFakeTime = gameTime.TotalGameTime.TotalSeconds;
                this.trash.Enabled = false;
                equipment.AddJunk(1);
            }

            if(this.collectedTrash)
            {
                GUIManager.Instance.DrawText(TrashSoupGame.Instance.Content.Load<SpriteFont>("Fonts/FontTest"),
                    "Trash collected", textPosition, Color.Red);
                if(gameTime.TotalGameTime.TotalSeconds - this.collectedFakeTime > 2.0)
                {
                    this.collectedTrash = false;
                }
                textPosition.Y -= 0.002f;
            }

            this.collisionWithTrash = false;

            if(InputManager.Instance.GetKeyboardButtonDown(Keys.F))
            {
                this.Popularity += 10.0f;
            }

            this.Popularity -= gameTime.ElapsedGameTime.Milliseconds * 0.001f * this.popularityDecreaseSpeed;
            this.Popularity = MathHelper.Clamp(this.Popularity, 0.0f, MAX_POPULARITY);
        }

        public override void Initialize()
        {
            this.interactionTexture = ResourceManager.Instance.LoadTexture(@"Textures/HUD/x_button");
            base.Initialize();
        }

        public override void Draw(Camera cam, Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Draw nothing
        }

        protected override void Start()
        {
            sprint = 1.0f;
            sprintM = 0.0f;
            equipment = new Equipment(this.MyObject);
            if(this.MyObject.GetComponent<Weapons.Fists>() == null)
                this.MyObject.Components.Add(equipment.CurrentWeapon);

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
