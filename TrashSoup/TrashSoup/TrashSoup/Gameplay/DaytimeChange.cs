using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    public class DaytimeChange : ObjectComponent
    {
        #region constants
        private const int TEXTURE_COUNT = 4;
        private const int TEXTURE_DAWN = 0;
        private const int TEXTURE_NOON = 1;
        private const int TEXTURE_DUSK = 2;
        private const int TEXTURE_NIGHT = 3;
        #endregion

        #region variables
        private GameObject sun;
        private GameObject light;
        private CustomModel myModel;
        private Material myMaterial;
        private TextureCube[] textures = new TextureCube[TEXTURE_COUNT];
        #endregion

        #region properties
        public uint SunID { get; set; }
        public uint LightID { get; set; }
        public string[] TextureNames { get; set; }
        #endregion

        #region methods

        public DaytimeChange(GameObject go)
            : base(go)
        {
        }

        public DaytimeChange(GameObject go, DaytimeChange cc)
            : base(go, cc)
        {
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // nth
        }

        public override void Draw(Engine.Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {
            // nth
        }

        protected override void Start()
        {
            // nth
        }

        public override void Initialize()
        {
            if(TextureNames == null)
            {
                throw new ArgumentNullException("DaytimeChange: Textures not set!");
            }
            if(TextureNames.Count() < TEXTURE_COUNT)
            {
                throw new ArgumentException("DaytimeChange: Texture count less than " + TEXTURE_COUNT.ToString() + "!");
            }

            sun = ResourceManager.Instance.CurrentScene.GetObject(SunID);
            light = ResourceManager.Instance.CurrentScene.DirectionalLights[LightID];

            foreach(ObjectComponent comp in this.MyObject.Components)
            {
                if(comp.GetType() == typeof(CustomModel))
                {
                    myModel = (CustomModel)comp;
                    if (myModel == null)
                        break;
                    myMaterial = myModel.Mat[0];
                    break;
                }
            }

            if(sun == null || light == null || myModel == null || myMaterial == null)
            {
                throw new ArgumentNullException("DaytimeChange: Some of the objects does not exist!");
            }

            for (int i = 0; i < TEXTURE_COUNT; ++i )
            {
                textures[i] = ResourceManager.Instance.LoadTextureCube(TextureNames[i]);
            }

            base.Initialize();
        }

        #endregion
    }
}
