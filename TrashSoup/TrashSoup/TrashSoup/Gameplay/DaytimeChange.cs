using Microsoft.Xna.Framework;
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
        private LightDirectional light;
        private CustomModel myModel;
        private SkyboxMaterial myMaterial;
        private TextureCube[] textures = new TextureCube[TEXTURE_COUNT];
        private PlayerTime cTime;
        private int time;
        private int prevTime;
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
            prevTime = time;
            time = 60 * cTime.Hours + cTime.Minutes;

            // tu bedzie if

            Vector4 probes;
            ConvertTimeToProbes(time, out probes);
            myMaterial.Probes = probes;

            Vector3 lightDir;
            ConvertTimeToLightDirection(time, out lightDir);
            light.LightDirection = lightDir;
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
                    if(myModel.Mat[0].GetType() != typeof(SkyboxMaterial))
                    {
                        throw new InvalidOperationException("DaytimeChange: Skybox's material is not SkyboxMaterial!");
                    }
                    myMaterial = (SkyboxMaterial)myModel.Mat[0];
                    break;
                }
            }

            if(sun == null || light == null || myModel == null || myMaterial == null)
            {
                throw new ArgumentNullException("DaytimeChange: Some of the objects do not exist!");
            }

            for (int i = 0; i < TEXTURE_COUNT; ++i )
            {
                if(TextureNames[i] != null)
                    textures[i] = ResourceManager.Instance.LoadTextureCube(TextureNames[i]);
            }

            if (textures[0] != null)
                myMaterial.CubeMap = textures[0];
            if (textures[1] != null)
                myMaterial.CubeMap1 = textures[1];
            if (textures[2] != null)
                myMaterial.CubeMap2 = textures[2];
            if (textures[3] != null)
                myMaterial.CubeMap3 = textures[3];

            GameObject pt = ResourceManager.Instance.CurrentScene.GetObject("PlayerTime");
            if(pt == null)
            {
                throw new ArgumentNullException("DaytimeChange: PlayerTime object does not exist!");
            }

            foreach(ObjectComponent comp in pt.Components)
            {
                if(comp.GetType() == typeof(PlayerTime))
                {
                    cTime = (PlayerTime)comp;
                }
            }

            if(cTime == null)
            {
                throw new ArgumentNullException("DaytimeChange: PlayerTime object has no PlayerTime component!");
            }

            base.Initialize();
        }

        private void ConvertTimeToProbes(int minutes, out Vector4 probes)
        {
            probes = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
        }

        private void ConvertTimeToLightDirection(int minutes, out Vector3 direction)
        {
            direction = new Vector3(-1.0f, -1.0f, 1.0f);



            direction.Z = -direction.Z;
            direction.Normalize();
        }

        #endregion
    }
}
