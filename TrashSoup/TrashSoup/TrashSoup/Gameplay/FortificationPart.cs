using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        private float health;
        private float hpPerMs;

        #endregion

        #region properties

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
                    MyObject.Visible = false;
                    MyObject.MyCollider.Enabled = false;

                    model.Mat = selectionMat;
                    selectionMat[0].DiffuseColor = NotBuiltColor;
                }
                else if(state == PartState.BUILT)
                {
                    MyObject.Visible = true;
                    MyObject.MyCollider.Enabled = true;

                    model.Mat = currentMat;
                    selectionMat[0].DiffuseColor = BuiltColor;
                }
                else if(state == PartState.BUILDING)
                {
                    MyObject.Visible = true;
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

            hpPerMs = (float)MaxHealth / ((float)TimeToBuild * 1000.0f);

            base.Initialize();
        }

        public void BuildUp(GameTime gameTime)
        {
            health += hpPerMs * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            float lerpFactor = health / (float)MaxHealth;

            selectionMat[0].DiffuseColor = Vector3.Lerp(NotBuiltColor, BuiltColor, lerpFactor);
        }

        #endregion
    }
}
