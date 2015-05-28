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

        private const int MINUTES_MAX = 60 * 24;
        #endregion

        #region variables
        private GameObject sun;
        private LightDirectional lightDay;
        private LightDirectional lightNight;
        private LightAmbient ambient;
        private CustomModel myModel;
        private SkyboxMaterial myMaterial;
        private TextureCube[] textures = new TextureCube[TEXTURE_COUNT];
        private PlayerTime cTime;
        private int time;
        private int prevTime;
        private Vector3 startDaylightColor;
        private Vector3 startDaylightSpecular;
        private Vector3 rotationAxe;
        #endregion

        #region properties
        public uint SunID { get; set; }
        public uint LightDayID { get; set; }
        public uint LightNightID { get; set; }
        public string[] TextureNames { get; set; }
        public int SunriseMinutes { get; set; }
        public int SunsetMinutes { get; set; }
        public int StateChangeMinutes { get; set; }
        #endregion

        #region methods

        public DaytimeChange(GameObject go)
            : base(go)
        {
            rotationAxe = new Vector3(-1.0f, 0.2f, 1.0f);
            rotationAxe.Normalize();
        }

        public DaytimeChange(GameObject go, DaytimeChange cc)
            : base(go, cc)
        {
            SunID = cc.SunID;
            LightDayID = cc.LightDayID;
            LightNightID = cc.LightNightID;
            TextureNames = cc.TextureNames;
            SunriseMinutes = cc.SunriseMinutes;
            SunsetMinutes = cc.SunsetMinutes;
            StateChangeMinutes = cc.StateChangeMinutes;
            rotationAxe = new Vector3(-1.0f, 0.2f, 1.0f);
            rotationAxe.Normalize();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            prevTime = time;
            time = 60 * cTime.Hours + cTime.Minutes;
            if(time < 0 || time > MINUTES_MAX)
            {
                Debug.Log("DaytimeChange: Time is invalid. Clamping.");
                time = (int)MathHelper.Clamp((float)time, 0.0f, (float)MINUTES_MAX);
            }

            // tu bedzie if

            Vector4 probes;
            ConvertTimeToProbes(time, out probes);
            myMaterial.Probes = probes;

            Vector3 lightDir;
            ConvertTimeToLightDirection(time, out lightDir);
            lightDay.LightDirection = lightDir;
            lightNight.LightDirection = -lightDir;

            if(lightDir.Y <= 0)
            {
                Vector3 lightCol, lightSpec, ambCol;
                ConvertTimeToDaylightColor(time, out lightCol, out lightSpec, out ambCol);
                lightDay.LightColor = lightCol;
                lightDay.LightSpecularColor = lightSpec;
                ambient.LightColor = ambCol;
                lightDay.Enabled = true;
                lightNight.Enabled = false;
            }
            else
            {
                lightDay.Enabled = false;
                lightNight.Enabled = true;
            }
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
            lightDay = ResourceManager.Instance.CurrentScene.DirectionalLights[LightDayID];
            lightNight = ResourceManager.Instance.CurrentScene.DirectionalLights[LightNightID];
            ambient = ResourceManager.Instance.CurrentScene.AmbientLight;

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

            if(sun == null || lightDay == null || lightNight == null || myModel == null || myMaterial == null || ambient == null)
            {
                throw new ArgumentNullException("DaytimeChange: Some of the objects do not exist!");
            }

            startDaylightColor = lightDay.LightColor;
            startDaylightSpecular = lightDay.LightSpecularColor;

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
            Vector4 state0 = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
            Vector4 state1 = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
            Vector4 state2 = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);
            Vector4 state3 = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
            float lerpValue;

            if (minutes < ((SunriseMinutes - StateChangeMinutes) % MINUTES_MAX) || minutes > (SunsetMinutes + StateChangeMinutes))
            {
                probes = state3;
            }
            else if(minutes > SunriseMinutes + StateChangeMinutes && minutes < SunsetMinutes - StateChangeMinutes)
            {
                probes = state1;
            }
            else if(minutes == SunriseMinutes)
            {
                probes = state0;
            }
            else if(minutes == SunsetMinutes)
            {
                probes = state2;
            }
            else if(minutes >= SunriseMinutes - StateChangeMinutes && minutes < SunriseMinutes)
            {
                //state3 vs state0
                float x = (float)(SunriseMinutes - StateChangeMinutes);
                float y = (float)SunriseMinutes;
                lerpValue = (((float)minutes) - x) / (y - x);
                probes = Vector4.Lerp(state3, state0, lerpValue);
            }
            else if(minutes > SunriseMinutes && minutes <= SunriseMinutes + StateChangeMinutes)
            {
                //state0 vs state1
                float x = (float)(SunriseMinutes);
                float y = (float)(SunriseMinutes + StateChangeMinutes);
                lerpValue = (((float)minutes) - x) / (y - x);
                probes = Vector4.Lerp(state0, state1, lerpValue);
            }
            else if(minutes >= SunsetMinutes - StateChangeMinutes && minutes < SunsetMinutes)
            {
                //state1 vs state2
                float x = (float)(SunsetMinutes - StateChangeMinutes);
                float y = (float)(SunsetMinutes);
                lerpValue = (((float)minutes) - x) / (y - x);
                probes = Vector4.Lerp(state1, state2, lerpValue);
            }
            else if(minutes > SunsetMinutes && minutes <= SunsetMinutes + StateChangeMinutes)
            {
                //state2 vs state3
                float x = (float)(SunsetMinutes);
                float y = (float)(SunsetMinutes + StateChangeMinutes);
                lerpValue = (((float)minutes) - x) / (y - x);
                probes = Vector4.Lerp(state2, state3, lerpValue);
            }
            else
            {
                Debug.Log("DaytimeChange: ConvertTimeToProbes has fucked up somehow. Time given is " + minutes.ToString());
                probes = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            }
        }

        private void ConvertTimeToLightDirection(int minutes, out Vector3 direction)
        {
            direction = new Vector3(-1.0f, -1.0f, -1.0f);   // dla minutes = 720
            //direction = Vector3.Transform(direction, Matrix.CreateRotationY(-MathHelper.PiOver4 / 1.5f));

            float rotation = ((float)((minutes - MINUTES_MAX / 2) % MINUTES_MAX) / (float)MINUTES_MAX) * MathHelper.Pi;
            //Debug.Log(rotation.ToString());
            direction = Vector3.Transform(direction, Matrix.CreateFromAxisAngle(rotationAxe, rotation));

            direction.Z = -direction.Z;
            direction.Normalize();
        }

        private void ConvertTimeToDaylightColor(int minutes, out Vector3 color, out Vector3 specular, out Vector3 ambientColor)
        {
            color = new Vector3(1.0f, 1.0f, 1.0f);
            specular = new Vector3(1.0f, 1.0f, 1.0f);
            ambientColor = new Vector3(0.0f, 0.0f, 0.0f);
        }

        #endregion
    }
}
