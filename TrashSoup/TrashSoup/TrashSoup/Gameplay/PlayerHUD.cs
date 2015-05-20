using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;
using Microsoft.Xna.Framework;

namespace TrashSoup.Gameplay
{
    class PlayerHUD : ObjectComponent
    {
        private Texture2D hpTexture;
        private Texture2D hpBGTexture;
        private Texture2D heartTexture;
        private Texture2D popularitySadTexture;
        private Texture2D populartiyHappyTexture;
        private Texture2D popularityBGTexture;
        private Texture2D popularityFillTexture;
        private Texture2D backpackTexture;
        private Texture2D burgerTexture;
        private SpriteFont equipmentFont;

        private PlayerController myPlayerController;

        private float maxHP = 100.0f;
        private float currentHP = 0.0f;
        private float maxWidth = 0.2f;
        private float maxPopularity = 100.0f;
        private float currentPopularity = 0.0f;
        private int currentJunkCount = 0;
        private int currentFoodCount = 0;
        private int maxJunkCount = Equipment.MAX_JUNK_CAPACITY;
        private int maxFoodCount = Equipment.MAX_FOOD_CAPACITY;
        private Equipment myEq;
        private Color junkColor = Color.White;
        private Color foodColor = Color.White;

        public PlayerHUD(GameObject go) : base(go)
        {
            Start();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(TrashSoupGame.Instance.EditorMode)
            {
                return;
            }
            this.currentHP = this.myPlayerController.HitPoints;
            this.currentPopularity = this.myPlayerController.Popularity;

            //Drawing HP
            GUIManager.Instance.DrawTexture(this.hpBGTexture, new Vector2(0.09f, 0.0875f), this.maxWidth * 1.001f, 0.0125f);
            GUIManager.Instance.DrawTexture(this.hpTexture, new Vector2(0.09f, 0.0875f), this.maxWidth * this.currentHP / this.maxHP, 0.0125f);
            GUIManager.Instance.DrawTexture(this.heartTexture, new Vector2(0.06f, 0.05f), 0.04f, 0.05f);

            //Drawing popularity bar
            GUIManager.Instance.DrawTexture(this.popularityBGTexture, new Vector2(0.11f, 0.175f), this.maxWidth * 1.001f, 0.0125f);
            GUIManager.Instance.DrawTexture(this.popularityFillTexture, new Vector2(0.11f, 0.175f), this.maxWidth * this.currentPopularity / this.maxPopularity, 0.0125f);
            if(this.currentPopularity > 0.0f)
            {
                GUIManager.Instance.DrawTexture(this.populartiyHappyTexture, new Vector2(0.09f, 0.15f), 0.03f, 0.03f);
            }
            else
            {
                GUIManager.Instance.DrawTexture(this.popularitySadTexture, new Vector2(0.09f, 0.15f), 0.03f, 0.03f);
            }

            //Equipment
            this.currentFoodCount = this.myEq.FoodCount;
            this.currentJunkCount = this.myEq.JunkCount;
            GUIManager.Instance.DrawTexture(this.backpackTexture, new Vector2(0.75f, 0.15f), 0.03f, 0.03f);
            this.junkColor = this.currentJunkCount == Equipment.MAX_JUNK_CAPACITY ? Color.Red : Color.White;
            GUIManager.Instance.DrawText(this.equipmentFont, this.currentJunkCount + "/" + this.maxJunkCount, new Vector2(0.79f, 0.16f), this.junkColor);
            GUIManager.Instance.DrawTexture(this.burgerTexture, new Vector2(0.87f, 0.14f), 0.035f, 0.045f);
            this.foodColor = this.currentFoodCount == Equipment.MAX_FOOD_CAPACITY ? Color.Red : Color.White;
            GUIManager.Instance.DrawText(this.equipmentFont, this.currentFoodCount + "/" + this.maxFoodCount, new Vector2(0.91f, 0.16f), this.foodColor);
            GUIManager.Instance.DrawText(this.equipmentFont, "DAY 1", new Vector2(0.88f, 0.1f), Color.Red);
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {

        }

        protected override void Start()
        {

        }

        public override void Initialize()
        {
            myPlayerController = (PlayerController)this.MyObject.GetComponent<PlayerController>();
            if(myPlayerController == null)
            {
                Debug.Log("There is some error in getting player controller");
            }
            this.maxHP = this.myPlayerController.HitPoints;
            this.currentHP = this.maxHP;
            this.myEq = this.myPlayerController.Equipment;
            this.hpTexture = ResourceManager.Instance.LoadTexture(@"Textures/HUD/HP");
            this.hpBGTexture = ResourceManager.Instance.LoadTexture(@"Textures/HUD/HPBG");
            this.heartTexture = ResourceManager.Instance.LoadTexture(@"Textures/HUD/heart");
            this.popularityBGTexture = ResourceManager.Instance.LoadTexture(@"Textures/HUD/fameBG");
            this.popularityFillTexture = ResourceManager.Instance.LoadTexture(@"Textures/HUD/fame");
            this.popularitySadTexture = ResourceManager.Instance.LoadTexture(@"Textures/HUD/sad");
            this.populartiyHappyTexture = ResourceManager.Instance.LoadTexture(@"Textures/HUD/smile");
            this.backpackTexture = ResourceManager.Instance.LoadTexture(@"Textures/HUD/backpack"); ;
            this.burgerTexture = ResourceManager.Instance.LoadTexture(@"Textures/HUD/burger");
            this.equipmentFont = TrashSoupGame.Instance.Content.Load<SpriteFont>(@"Fonts/FontTest");

            base.Initialize();
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
    }
}
