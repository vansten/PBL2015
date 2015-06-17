using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Gameplay;

namespace TrashSoup.Engine
{
    public class Destructible : ObjectComponent
    {
        #region constants
        private const float PART_MASS = 3.0f;
        private const float PART_TTL = 800.0f;
        #endregion

        #region variables

        private ParticleSystem ps;
        private CustomModel mModel;
        private Equipment eq;
        private Vector3 intersectionVector = Vector3.Zero;
        private int maxHealth;

        #endregion

        #region properties

        public int PartCount { get; set; }
        public int PartHealth { get; private set; }
        public bool Shattered { get; private set; }

        /// <summary>
        /// setting this property will result in PartHealth resetting to this level!!!
        /// </summary>
        public int MaxHealth
        { 
            get
            {
                return maxHealth;
            }
            set
            {
                maxHealth = value;
                PartHealth = value;
            }
        }
        public int HitDamage { get; set; }

        #endregion

        #region methods

        public Destructible(GameObject myObj) : base(myObj)
        {

        }

        public Destructible(GameObject myObj, Destructible cc) : base(myObj, cc)
        {
            this.PartCount = cc.PartCount;
            MaxHealth = cc.MaxHealth;
            PartHealth = cc.PartHealth;
            HitDamage = cc.HitDamage;
            Shattered = cc.Shattered;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {
            // nothing
        }

        protected override void Start()
        {
            
        }

        public override void Initialize()
        {
            PlayerController controller = (PlayerController)ResourceManager.Instance.CurrentScene.GetObject(1).GetComponent<PlayerController>();
            controller.MyAttackTriggerComponent.AttackEvent += new PlayerAttackTrigger.AttackEventHandler(AttackTriggerHandler);

            eq = controller.Equipment;

            mModel = (CustomModel)MyObject.GetComponent<CustomModel>();

            ps = new ParticleSystem(this.MyObject);
            ps.Textures.Add(ResourceManager.Instance.LoadTexture("Textures/Particles/Particle_metal01"));
            ps.Textures.Add(ResourceManager.Instance.LoadTexture("Textures/Particles/Particle_metal02"));
            ps.ParticleCount = PartCount;
            ps.ParticleSize = new Vector2(0.5f, 0.5f);
            ps.ParticleSizeVariation = new Vector2(0.3f, 0.3f);
            ps.LifespanSec = PART_TTL / 1000.0f;
            ps.Wind = Vector3.Zero;
            ps.Offset = new Vector3(MathHelper.PiOver2);
            ps.Speed = 10.0f;
            ps.RotationMode = ParticleSystem.ParticleRotationMode.DIRECTION_Z;
            ps.LoopMode = ParticleSystem.ParticleLoopMode.NONE;
            ps.ParticleRotation = new Vector3(0.0f, 0.0f, MathHelper.PiOver4);
            ps.FadeInTime = 0.0f;
            ps.FadeOutTime = 0.05f;
            ps.BlendMode = BlendState.AlphaBlend;
            ps.UseGravity = true;
            ps.Mass = 0.000001f * PART_MASS;
            ps.Initialize();

            ps.Stop();

            GameObject child = new GameObject(MyObject.UniqueID + (uint)SingleRandom.Instance.rnd.Next(), MyObject.Name + "DestructibleHelper");
            child.Components.Add(ps);

            MyObject.AddChild(child);

            base.Initialize();
        }

        private void AttackTriggerHandler(object sender, CollisionEventArgs e)
        {
            Debug.Log("JEB!");
            PartHealth -= HitDamage;

            if(PartHealth < 0)
            {
                intersectionVector = e.CollisionObj.MyCollider.IntersectionVector;
                Shatter();
            }
        }

        private void Shatter()
        {
            Debug.Log("ALE ÓRWAŁ!");

            mModel.Visible = false;
            Vector3 lPos, lRot;
            MyObject.MyTransform.BakeTransformFromCarrier();
            //lPos = MyObject.MyTransform.Position;
            //lRot = MyObject.MyTransform.Rotation;
            eq.DropWeapon(eq.CurrentWeapon.MyObject);

            //MyObject.MyTransform.Position = lPos;
            //MyObject.MyTransform.Rotation = lRot;

            ps.Wind = -intersectionVector;

            ps.Play();

            PlayerController controller = (PlayerController)ResourceManager.Instance.CurrentScene.GetObject(1).GetComponent<PlayerController>();
            controller.MyAttackTriggerComponent.AttackEvent -= AttackTriggerHandler;
        }

        #endregion
    }
}
