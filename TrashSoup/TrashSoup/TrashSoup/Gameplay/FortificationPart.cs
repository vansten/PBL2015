using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace TrashSoup.Gameplay
{
    public class FortificationPart : ObjectComponent
    {
        #region enums

        public enum PartState
        {
            NEXT_BUILD,
            PENDING,
            BUILDING,
            BUILT
        }

        #endregion

        #region variables

        private static Vector3 NotBuiltColor = new Vector3(0.8f, 0.8f, 1.0f);
        private static Vector3 BuiltColor = new Vector3(0.1f, 1.0f, 0.2f);

        private PartState state;
        private List<Material> selectionMat;
        private List<Material> currentMat;
        private CustomModel model;
        private HideoutStash hs;
        private ParticleSystem ps;

        private float health;
        private float priceHelper = 0.0f;

        private SoundEffectInstance jebutSound;

        #endregion

        #region properties

        public bool Visible
        {
            get
            {
                return model.Visible;
            }
            set
            {
                model.Visible = value;
            }
        }

        public PartState State 
        { 
            get
            {
                return state;
            }
            set
            {
                state = value;

                if (state == PartState.PENDING || state == PartState.NEXT_BUILD)
                {
                    Visible = false;
                    MyObject.MyCollider.Enabled = false;

                    model.Mat = selectionMat;
                    selectionMat[0].DiffuseColor = NotBuiltColor;
                }
                else if(state == PartState.BUILT)
                {
                    Visible = true;
                    MyObject.MyCollider.Enabled = true;

                    model.Mat = currentMat;
                    selectionMat[0].DiffuseColor = BuiltColor;
                }
                else if(state == PartState.BUILDING)
                {
                    Visible = true;
                    MyObject.MyCollider.Enabled = false;

                    model.Mat = selectionMat;
                    selectionMat[0].DiffuseColor = NotBuiltColor;
                }
            }
        }
        public uint Price { get; set; }
        public uint TimeToBuild { get; set; }
        public uint Health 
        { 
            get
            {
                return (uint)health;
            }
            set
            {
                health = (float)value;
            }
        }
        public uint MaxHealth { get; set; }
        public float HpPerMs { get; private set; }
        public float PricePerMs { get; private set; }

        #endregion

        #region methods

        public FortificationPart(GameObject go)
            : base(go)
        {
            Start();
        }

        public FortificationPart(GameObject go, FortificationPart ff)
            : base(go, ff)
        {
            State = ff.State;
            Price = ff.Price;
            TimeToBuild = ff.TimeToBuild;
            Health = ff.Health;
            MaxHealth = ff.MaxHealth;

            Start();
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(Camera cam, Effect effect, GameTime gameTime)
        {

        }

        protected override void Start()
        {
            selectionMat = new List<Material>();
        }

        public override void Initialize()
        {
            Material selMat = new Material(MyObject.Name + "FortificationSelectionMat", ResourceManager.Instance.LoadEffect(@"Effects\DefaultEffect"));
            selMat.DiffuseMap = ResourceManager.Instance.Textures["DefaultDiffuseWhite"];
            selMat.DiffuseColor = NotBuiltColor;
            selMat.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
            selMat.Transparency = 0.25f;

            model = (CustomModel)MyObject.GetComponent<CustomModel>();

            for (int i = 0; i < model.Mat.Count; ++i)
                selectionMat.Add(selMat);

            currentMat = model.Mat;

            GameObject player = ResourceManager.Instance.CurrentScene.ObjectsDictionary[1];
            hs = (HideoutStash)player.GetComponent<HideoutStash>();

            ps = new ParticleSystem(this.MyObject);
            ps.Textures.Add(ResourceManager.Instance.LoadTexture("Textures/Particles/Particle_metal01"));
            ps.ParticleCount = 70;
            ps.ParticleSize = new Vector2(0.25f, 0.25f);
            ps.LifespanSec = 5.5f;
            ps.Wind = new Vector3(0.0f, 0.1f, 0.0f);
            ps.FadeInTime = 1.0f;
            ps.Offset = new Vector3(MathHelper.PiOver2);
            ps.Speed = 1.0f;
            ps.PositionOffset = new Vector3(0.0f, -1.0f, 0.0f) * MyObject.MyTransform.Scale;

            ps.Initialize();

            ps.Stop();

            MyObject.Components.Add(ps);

            HpPerMs = (float)MaxHealth / ((float)TimeToBuild * 1000.0f);
            PricePerMs = (float)Price / ((float)TimeToBuild * 1000.0f);

            SoundEffect se = TrashSoupGame.Instance.Content.Load<SoundEffect>(@"Audio/Actions/fortificationJebut");
            jebutSound = se.CreateInstance();

            base.Initialize();
        }

        public void BuildUp(GameTime gameTime)
        {
            health += HpPerMs * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            hs.CurrentTrashFloat -= PricePerMs * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            float lerpFactor = health / (float)MaxHealth;

            selectionMat[0].DiffuseColor = Vector3.Lerp(NotBuiltColor, BuiltColor, lerpFactor);
        }

        public void Destroy()
        {
            Debug.Log("Boom!");

            jebutSound.Play();
            ps.Play();
            // particles flying here
        }

        #endregion
    }
}
