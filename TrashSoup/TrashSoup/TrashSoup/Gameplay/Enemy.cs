using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    public class Enemy : ObjectComponent, IXmlSerializable
    {
        #region variables
        protected int hitPoints;
        protected bool isDead;
        private bool deathAnimPlayed = false;

        public Action OnDead;

        private GameObject hpBar;
        private Billboard hpBarBillboardComp;
        private Texture2D myHpBarTexture;
        public uint MyHPBarID;
        #endregion

        #region properties
        public int HitPoints
        {
            get { return hitPoints; }
            set 
            {
                hitPoints = value;
                //if (this.myHpBarTexture != null)
                //{
                //    Color[] tcolor = new Color[this.myHpBarTexture.Width * this.myHpBarTexture.Height];
                //    this.myHpBarTexture.GetData<Color>(tcolor);
                //    int size = this.myHpBarTexture.Width * this.myHpBarTexture.Height;
                //    for (int i = (int)((float)this.hitPoints / 100.0f * (float)size); i < size; ++i)
                //    {
                //        tcolor[i].A = 0;
                //    }
                //    this.myHpBarTexture.SetData<Color>(tcolor);
                //}
                if(this.hpBarBillboardComp != null)
                {
                    this.hpBarBillboardComp.Size = new Vector2(hitPoints / 100.0f, this.hpBarBillboardComp.Size.Y);
                }
            }
        }

        public bool IsDead
        {
            get { return isDead; }
            set { isDead = value; }
        }
        #endregion

        #region methods
        public Enemy(GameObject obj) : base(obj)
        {
            this.hitPoints = 100;
            this.MyHPBarID = 0;
        }

        public Enemy(GameObject obj, Enemy e) : base(obj, e)
        {
            this.HitPoints = 100;
            this.MyHPBarID = e.MyHPBarID;
        }

        public Enemy(GameObject obj, int hitPoints):base(obj)
        {
            this.MyHPBarID = 0;
            this.hitPoints = hitPoints;
        }
        #endregion

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (this.HitPoints <= 0)
                IsDead = true;
            if (IsDead)
            {
                if(!deathAnimPlayed && OnDead != null)
                {
                    deathAnimPlayed = true;
                    OnDead();
                }
                //this.MyObject.Enabled = false;
                return;
            }
            else if(this.hpBar != null)
            {
                this.hpBar.MyTransform.Position = this.MyObject.MyTransform.Position + Vector3.Up * 2.0f;
            }

#if DEBUG
            GUIManager.Instance.DrawText(TrashSoupGame.Instance.Content.Load<SpriteFont>("Fonts/FontTest"),
                "RAT: " + this.HitPoints, new Vector2(0.8f, 0.8f), Color.Red);
#endif
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {
            
        }

        public override void Initialize()
        {
            this.hpBar = ResourceManager.Instance.CurrentScene.GetObject(this.MyHPBarID);
            if(this.hpBar != null)
            {
                hpBarBillboardComp = (Billboard)this.hpBar.GetComponent<Billboard>();
                this.myHpBarTexture = hpBarBillboardComp.Mat.DiffuseMap;
            }
            base.Initialize();
        }

        protected override void Start()
        {
            IsDead = false;
        }

        public override System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.MoveToContent();
            reader.ReadStartElement();

            reader.ReadStartElement("MyHPBarID");
            this.MyHPBarID = (uint)reader.ReadContentAsInt();
            reader.ReadEndElement();
            base.ReadXml(reader);

            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("MyHPBarID");
            writer.WriteValue(this.MyHPBarID);
            writer.WriteEndElement();
            base.WriteXml(writer);
        }
    }
}
