using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using TrashSoup;
using TrashSoup.Gameplay;
using System.Xml.Serialization;
using System.IO;

namespace TrashSoup.Engine
{
    class SingleRandom : Singleton<SingleRandom>
    {
        public Random rnd = new Random();
    }

    public class ResourceManager : Singleton<ResourceManager>
    {
        #region Constants

        public const int DIRECTIONAL_MAX_LIGHTS = 3;
        public const int POINT_MAX_LIGHTS_PER_OBJECT = 10; 

        #endregion

        #region Variables
        public Scene CurrentScene;
        public Dictionary<string, Model> Models = new Dictionary<string, Model>();
        public Dictionary<string, Model> Animations = new Dictionary<string, Model>();
        public Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public Dictionary<string, TextureCube> TexturesCube = new Dictionary<string, TextureCube>();
        public List<SpriteFont> Fonts = new List<SpriteFont>();
        public Dictionary<string, Effect> Effects = new Dictionary<string, Effect>();
        public Dictionary<string, Material> Materials = new Dictionary<string,Material>();
        public List<Cue> Sounds = new List<Cue>();
        public ParticleSystem ps;
        //public List<Particle> Particles = new List<Particle>();
        #endregion

        #region Methods

        public ResourceManager()
        {

        }

        public void LoadContent(Game game)
        {
            LoadTextures(game);
            LoadEffects(game);
            AudioManager.Instance.LoadContent();
            LoadCues();

            // because it pisses me off - Mav
            //AudioManager.Instance.PlayCue(GetCueFromCueList("Track1")); //default music from tutorial, just to check that everything works xD

            // FOR TETIN
            AddModel("Models/Test/TestBox");
            AddModel("Models/Test/TestCube");
            AddModel("Models/Test/TestTerrain");
            AddModel("Models/Test/TestGuy");
            AddModel("Models/Test/TestSphere");
            AddModel("Models/Test/TestMirror");
            AddModel("Models/Test/TestSquarePlane");
            AddModel("Models/Test/TestSquarePlane");
            AddModel("Models/Enemies/Rat");
            AddAnimation("Animations/Enemies/Rat_attack");
            AddAnimation("Animations/Test/walking_1");
            AddAnimation("Animations/Test/idle_1");
            AddAnimation("Animations/Test/jump_1");
            AddModel("Models/Enviro/Buildings/Big_house");    //Wika i Kasia

            // loading materials
            List<Material> testPlayerMats = new List<Material>();
            Material testPlayerMat = new Material("testPlayerMat", this.Effects[@"Effects\NormalEffect"], Textures[@"Textures\Test\cargo"]);
            testPlayerMats.Add(testPlayerMat);
            testPlayerMat.NormalMap = Textures[@"Textures\Test\cargo_NRM"];
            testPlayerMat.Glossiness = 40.0f;
            testPlayerMat.Transparency = 1.0f;
            testPlayerMat.RecieveShadows = true;
            if(!this.Materials.ContainsKey(testPlayerMat.Name))
            {
                this.Materials.Add(testPlayerMat.Name, testPlayerMat);
            }

            List<Material> testPlayerMats2 = new List<Material>();
            Material testPlayerMat2 = new Material("testPlayerMat2", this.Effects[@"Effects\CubeNormalEffect"], Textures[@"Textures\Test\cargo"]);
            testPlayerMats2.Add(testPlayerMat2);
            testPlayerMat2.NormalMap = Textures[@"Textures\Test\cargo_NRM"];
            testPlayerMat2.CubeMap = TexturesCube[@"Textures\Skyboxes\Sunset"];
            testPlayerMat2.Glossiness = 40.0f;
            testPlayerMat2.ReflectivityColor = new Vector3(1.0f, 0.0f, 1.0f);
            testPlayerMat2.ReflectivityBias = 0.7f;
            testPlayerMat2.Transparency = 1.0f;
            testPlayerMat2.RecieveShadows = true;
            if(!this.Materials.ContainsKey(testPlayerMat2.Name))
            {
                this.Materials.Add(testPlayerMat2.Name, testPlayerMat2);
            }

            List<Material> testMirrorMats = new List<Material>();
            Material testMirrorMat = new MirrorMaterial("testMirrorMat", this.Effects[@"Effects\MirrorEffect"]);
            testMirrorMats.Add(testMirrorMat);
            testMirrorMat.Glossiness = 100.0f;
            testMirrorMat.ReflectivityBias = 1.0f;
            if(!this.Materials.ContainsKey(testMirrorMat.Name))
            {
                this.Materials.Add(testMirrorMat.Name, testMirrorMat);
            }

            List<Material> testWaterMats = new List<Material>();
            Material testWaterMat = new WaterMaterial("testWaterMat", this.Effects[@"Effects\WaterEffect"]);
            testWaterMats.Add(testWaterMat);
            testWaterMat.DiffuseMap = LoadTexture(@"Textures\Test\dirtywater");
            testWaterMat.NormalMap = LoadTexture(@"Textures\Test\water");
            testWaterMat.Glossiness = 200.0f;
            testWaterMat.ReflectivityBias = 0.6f;
            if(!this.Materials.ContainsKey(testWaterMat.Name))
            {
                this.Materials.Add(testWaterMat.Name, testWaterMat);
            }

            List<Material> playerMats = LoadBasicMaterialsFromModel(Models["Models/Test/TestGuy"], this.Effects[@"Effects\NormalEffect"]);

            List<Material> ratMats = LoadBasicMaterialsFromModel(Models["Models/Enemies/Rat"], this.Effects[@"Effects\NormalEffect"]);

            List<Material> testTerMats = new List<Material>();
            Material testTerMat = new Material("testTerMat", this.Effects[@"Effects\DefaultEffect"], Textures[@"Textures\Test\metal01_d"]);
            testTerMat.SpecularColor = new Vector3(0.1f, 0.1f, 0.0f);
            testTerMat.Glossiness = 10.0f;
            testTerMat.RecieveShadows = true;
            if(!this.Materials.ContainsKey(testTerMat.Name))
            {
                this.Materials.Add(testTerMat.Name, testTerMat);
            }
            if(!testTerMats.Contains(testTerMat))
            {
                testTerMats.Add(testTerMat);
            }

            List<Material> testSBMats = new List<Material>();
            Material testSBMat = new Material("testSBMat", this.Effects[@"Effects\SkyboxEffect"]);
            testSBMat.CubeMap = TexturesCube[@"Textures\Skyboxes\Sunset"];
            testSBMat.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
            testSBMat.Glossiness = 100.0f;
            if(!this.Materials.ContainsKey(testSBMat.Name))
            {
                this.Materials.Add(testSBMat.Name, testSBMat);
            }
            if(!testSBMats.Contains(testSBMat))
            {
                testSBMats.Add(testSBMat);
            }


            //WIKA I KASIA Testowanie modeli
            List<Material> awsomeTestMats = new List<Material>();
            Material awsomeTestMat = new Material("awsomeTestMat", this.Effects[@"Effects\NormalEffect"], Textures[@"Textures\Enviro\Buildings\Big_house_D"]);
            awsomeTestMats.Add(awsomeTestMat);
            awsomeTestMats.Add(awsomeTestMat);
            awsomeTestMat.NormalMap = Textures[@"Textures\Enviro\Buildings\Big_house_N"];
            awsomeTestMat.Glossiness = 40.0f;
            awsomeTestMat.ReflectivityColor = new Vector3(1.0f, 0.0f, 1.0f);
            awsomeTestMat.ReflectivityBias = 0.0f;
            awsomeTestMat.Transparency = 1.0f;
            awsomeTestMat.RecieveShadows = true;
            if (!this.Materials.ContainsKey(awsomeTestMat.Name))
            {
                this.Materials.Add(awsomeTestMat.Name, awsomeTestMat);
            }


            // loading gameobjects
            GameObject testBox = new GameObject(1, "Player");
            testBox.MyTransform = new Transform(testBox, new Vector3(0.0f, 0.0f, -4.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 0.0f), 0.01f);
            CustomModel skModel = new CustomModel(testBox, new Model[] { Models["Models/Test/TestGuy"], null, null }, 3, playerMats);
            Animator playerAnimator = new Animator(testBox, skModel.LODs[0]);
            playerAnimator.AddAnimationClip(LoadAnimationFromModel(skModel.LODs[0], this.Animations["Animations/Test/walking_1"], "walking_1"));
            playerAnimator.AddAnimationClip(LoadAnimationFromModel(skModel.LODs[0], this.Animations["Animations/Test/idle_1"], "idle_1"));
            playerAnimator.AddAnimationClip(LoadAnimationFromModel(skModel.LODs[0], this.Animations["Animations/Test/jump_1"], "jump_1"));
            testBox.Components.Add(skModel);
            testBox.MyAnimator = playerAnimator;
            testBox.Components.Add(new PlayerController(testBox));
            testBox.MyPhysicalObject = new PhysicalObject(testBox, 1.0f, 0.05f, false);
            testBox.MyCollider = new SphereCollider(testBox);  //Add a box collider to test collisions

            // loading gameobjects
            GameObject rat = new GameObject(50, "Rat");
            rat.MyTransform = new Transform(rat, new Vector3(0.0f, 3.4f, -15.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 0.0f), 0.001f);
            CustomModel ratModel = new CustomModel(rat, new Model[] { Models["Models/Enemies/Rat"], null, null }, 3, ratMats);
            Animator ratAnimator = new Animator(rat, ratModel.LODs[0]);
            ratAnimator.AddAnimationClip(LoadAnimationFromModel(ratModel.LODs[0], this.Animations["Animations/Enemies/Rat_attack"], "Rat_TAnim"));
            ratAnimator.AvailableStates.Add("Walk", new AnimatorState("Walk", ratAnimator.GetAnimationPlayer("Rat_TAnim")));
            ratAnimator.CurrentState = ratAnimator.AvailableStates["Walk"];
            rat.Components.Add(ratModel);
            rat.MyAnimator = ratAnimator;
                       

            GameObject testTer = new GameObject(2, "Terrain");
            testTer.MyTransform = new Transform(testTer, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 0.0f), 1.0f);
            testTer.Components.Add(new CustomModel(testTer, new Model[] { Models["Models/Test/TestTerrain"], null, null }, 3, testTerMats));

            GameObject testBox2 = new GameObject(3, "testBox2");
            testBox2.MyTransform = new Transform(testBox2, new Vector3(10.0f, 2.0f, -10.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 0.0f), 1.4f);
            testBox2.Components.Add(new CustomModel(testBox2, new Model[] { Models["Models/Test/TestSphere"], null, null }, 3, testPlayerMats));
            testBox2.MyCollider = new BoxCollider(testBox2, false);


            GameObject testBox3 = new GameObject(5, "testBox3");
            testBox3.MyTransform = new Transform(testBox3, new Vector3(8.0f, 2.0f, 6.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, MathHelper.Pi, 0.0f), 1.2f);
            testBox3.Components.Add(new CustomModel(testBox3, new Model[] { Models["Models/Test/TestSphere"], null, null }, 3, testPlayerMats2));
            testBox3.MyCollider = new SphereCollider(testBox3);

            GameObject testMirror = new GameObject(6, "testMirror");
            testMirror.MyTransform = new Transform(testMirror, new Vector3(-10.0f, 2.0f, -10.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, -MathHelper.PiOver2, 0.0f), 1.0f);
            testMirror.Components.Add(new CustomModel(testMirror, new Model[] { Models["Models/Test/TestMirror"], null, null }, 3, testMirrorMats));
            testMirror.MyCollider = new BoxCollider(testMirror, false);

            GameObject testWater = new GameObject(7, "tesWtater");
            testWater.MyTransform = new Transform(testWater, new Vector3(0.0f, -1.5f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), 10.0f);
            testWater.Components.Add(new CustomModel(testWater, new Model[] { Models["Models/Test/TestSquarePlane"], null, null }, 3, testWaterMats));

            GameObject skyBox = new GameObject(4, "skyBox");
            skyBox.MyTransform = new Transform(skyBox, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), 1000.0f);
            skyBox.Components.Add(new CustomModel(skyBox, new Model[] { Models["Models/Test/TestCube"], null, null }, 3, testSBMats));

            //Wika i Kasia testowanie modeli
            GameObject awsomeTest = new GameObject(8, "testground");
            awsomeTest.MyTransform = new Transform(awsomeTest, new Vector3(10.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 0.0f), 1.4f);
            awsomeTest.Components.Add(new CustomModel(awsomeTest, new Model[] { Models["Models/Enviro/Buildings/Big_house"], null, null }, 3, awsomeTestMats));
            awsomeTest.MyCollider = new BoxCollider(awsomeTest, false);

            // adding lights
            LightAmbient amb = new LightAmbient(100, "LightAmbient", new Vector3(0.05f, 0.05f, 0.1f));
            LightDirectional ldr = new LightDirectional(101, "LightDirectional1", new Vector3(0.5f, 0.4f, 0.3f), new Vector3(1.0f, 0.8f, 0.8f), new Vector3(-1.0f, -1.0f, -1.0f), true);
            LightPoint lp1 = new LightPoint(110, "LightPoint1", new Vector3(0.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), 1.0f, true);
            lp1.MyTransform = new Transform(lp1, new Vector3(-3.0f, 3.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), 1.0f);
            lp1.SetupShadowRender();

            // loading scene
            CurrentScene = new Scene(new SceneParams(0, "test", new Vector2(0.0f, 0.1f), DateTime.Now, false, true, true, true));

            Camera cam = null;

            if(TrashSoupGame.Instance.EditorMode)
            {
                //Editor camera
                CurrentScene.EditorCam = new EditorCamera(1, "editorCam", Vector3.Transform(new Vector3(0.0f, 10.0f, -50.0f), Matrix.CreateRotationX(MathHelper.PiOver4 * 1.5f)),
                     new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 10.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), MathHelper.Pi / 3.0f, 0.1f, 2000.0f);
            }
                //Game camera
            cam = new Camera(1, "playerCam", Vector3.Transform(new Vector3(0.0f, 1.0f, -0.1f), Matrix.CreateRotationX(MathHelper.PiOver4 * 1.5f)) + new Vector3(0.0f, 0.4f, 0.0f),
                    new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.5f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), MathHelper.Pi / 3.0f,
                    (float)TrashSoupGame.Instance.Window.ClientBounds.Width / (float)TrashSoupGame.Instance.Window.ClientBounds.Height, 0.1f, 2000.0f);
            cam.Components.Add(new CameraBehaviourComponent(cam, testBox));

            CurrentScene.Cam = cam;

            // adding items to scene
            CurrentScene.AddObject(skyBox);
            CurrentScene.AddObject(testTer);
            CurrentScene.AddObject(testBox);
            CurrentScene.AddObject(testBox2);
            CurrentScene.AddObject(testBox3);
            CurrentScene.AddObject(testMirror);
            CurrentScene.AddObject(testWater);
            CurrentScene.AddObject(awsomeTest);//Wika i kasia
            CurrentScene.AddObject(rat);

            CurrentScene.AmbientLight = amb;
            CurrentScene.DirectionalLights[0] = ldr;
            CurrentScene.PointLights.Add(lp1);

            ////TESTING PARTICLES
            ps = new ParticleSystem(TrashSoupGame.Instance.GraphicsDevice, 
                TrashSoupGame.Instance.Content.Load<Texture2D>(@"Textures/ParticleTest/Particle"), 400, new Vector2(2), 1, Vector3.Zero, 0.5f);
        }

        /// <summary>
        /// 
        /// Method gets cue from our list searching for elements which name contains "name" parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Cue GetCueFromCueList(String name)
        {
            return Sounds.Find(x => x.Name.Contains(name));
        }

        /// <summary>
        /// 
        /// Load every sound to cue list.
        /// </summary>
        private void LoadCues()
        {
            Sounds.Add(AudioManager.Instance.GetCue("Track1"));
        }

        /// <summary>
        /// 
        /// Load every model from content to textures list
        /// IMPORTANT!!! SET TAG FOR EVERY ELEMENT
        /// </summary>
        /// <param name="game"></param>
        public void AddModel(String path)
        {
            if(!Models.ContainsKey(path))
            {
                Models.Add(path, TrashSoupGame.Instance.Content.Load<Model>(path));
            }
        }

        private void AddAnimation(String path)
        {
            if(!Animations.ContainsKey(path))
            {
                Animations.Add(path, TrashSoupGame.Instance.Content.Load<Model>(path));
            }
        }

        /// <summary>
        /// 
        /// Checks if model with certain path exists in dictionary. If yes method returns it, if not, loads it, adds
        /// to proper dictionary and returns the model.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Model LoadModel(String path)
        {
            Model output = null;
            if (Models.TryGetValue(path, out output))
                Debug.Log("Model successfully loaded - " + path);
            else
            {
                output = TrashSoupGame.Instance.Content.Load<Model>(path);
                Models.Add(path, output);
                Debug.Log("New model successfully loaded - " + path);
            }
            return output;
        }

        /// <summary>
        /// 
        /// Checks if animation with certain path exists in dictionary. If yes method returns it, if not, loads it, adds
        /// to proper dictionary and returns the animation.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Model LoadAnimation(String path)
        {
            Model output = null;
            if (Animations.TryGetValue(path, out output))
                Debug.Log("Animation successfully loaded - " + path);
            else
            {
                output = TrashSoupGame.Instance.Content.Load<Model>(path);
                Animations.Add(path, output);
                Debug.Log("New animation successfully loaded - " + path);
            }
            return output;
        }

        public Material LoadMaterial(String path)
        {
            string newName = Path.GetFileNameWithoutExtension(path);
            Material output = new Material();
            if (!ResourceManager.Instance.Materials.TryGetValue(newName, out output))
            {
                Material tmp = new Material();
                XmlSerializer serializer = new XmlSerializer(typeof(Material));
                using (FileStream file = new FileStream(path, FileMode.Open))
                {
                    tmp = (Material)serializer.Deserialize(file);
                    tmp.Name = newName;
                }
                output = tmp;
                Debug.Log("Material successfully loaded - " + newName);
                return output;
            }
            else
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Material));
                using (FileStream file = new FileStream(path, FileMode.Open))
                {
                    output = (Material)serializer.Deserialize(file);
                    output.Name = newName;
                }
                if (!ResourceManager.Instance.Materials.ContainsKey(newName))
                {
                    ResourceManager.Instance.Materials.Add(newName, output);
                }
                Debug.Log("New material successfully loaded - " + newName);
                return output;
            }
        }

        /// <summary>
        /// Used to load a single texture. If it doesn't exist in resourceManager, Content.Load is called.
        /// </summary>
        /// <param name="texturePath"></param>
        public Texture2D LoadTexture(string texturePath)
        {
            if (Textures.ContainsKey(texturePath))
                return Textures[texturePath];
            else
            {
                Texture2D newTex = TrashSoupGame.Instance.Content.Load<Texture2D>(texturePath);
                Textures.Add(texturePath, newTex);
                return newTex;
            }
        }

        /// <summary>
        /// Used to load a single cube texture. If it doesn't exist in resourceManager, Content.Load is called.
        /// </summary>
        /// <param name="texturePath"></param>
        public TextureCube LoadTextureCube(string texturePath)
        {
            if (TexturesCube.ContainsKey(texturePath))
                return TexturesCube[texturePath];
            else
            {
                TextureCube newTex = TrashSoupGame.Instance.Content.Load<TextureCube>(texturePath);
                TexturesCube.Add(texturePath, newTex);
                return newTex;
            }
        }

        /// <summary>
        /// Checks if effect with certain path exists in dictionary. If yes method returns it, if not, loads it, adds
        /// to proper dictionary and returns the effect.
        /// </summary>
        /// <param name="effectPath"></param>
        /// <returns></returns>
        public Effect LoadEffect(string effectPath)
        {
            Effect output = null;
            if (!Effects.TryGetValue(effectPath, out output))
                //Debug.Log("Effect successfully loaded - " + effectPath);
            //else
            {
                output = TrashSoupGame.Instance.Content.Load<Effect>(effectPath);
                Effects.Add(effectPath, output);
                //Debug.Log("New effect successfully loaded - " + effectPath);
            }
            return output;
        }

        /// <summary>
        /// 
        /// Load every texture from content to textures list
        /// IMPORTANT!!! SET NAME FOR EVERY ELEMENT
        /// </summary>
        /// <param name="game"></param>
        private void LoadTextures(Game game)
        {
            // Adding "default" textures for all maps containing only one pixel in one color
            //uint diffColor = 0xFFFFFFFF;
            uint normColor = 0xFFFF0F0F;
            uint blackColor = 0xFF000000;
            if (!Textures.ContainsKey("DefaultDiffuse"))
            {
                Texture2D defDiff = new Texture2D(TrashSoupGame.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                defDiff.SetData<uint>(new uint[] { blackColor });
                Textures.Add("DefaultDiffuse", defDiff);
            }
            if(!Textures.ContainsKey("DefaultNormal"))
            {
                Texture2D defNrm = new Texture2D(TrashSoupGame.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                defNrm.SetData<uint>(new uint[] { normColor });
                Textures.Add("DefaultNormal", defNrm);
            }
            if (!TexturesCube.ContainsKey("DefaultCube"))
            {
                TextureCube defCbc = new TextureCube(TrashSoupGame.Instance.GraphicsDevice, 1, false, SurfaceFormat.Color);
                defCbc.SetData<uint>(CubeMapFace.NegativeX, new uint[] { blackColor });
                defCbc.SetData<uint>(CubeMapFace.PositiveX, new uint[] { blackColor });
                defCbc.SetData<uint>(CubeMapFace.NegativeY, new uint[] { blackColor });
                defCbc.SetData<uint>(CubeMapFace.PositiveY, new uint[] { blackColor });
                defCbc.SetData<uint>(CubeMapFace.NegativeZ, new uint[] { blackColor });
                defCbc.SetData<uint>(CubeMapFace.PositiveZ, new uint[] { blackColor });
                TexturesCube.Add("DefaultCube", defCbc);
            }
            ///////////////////////////////////////////

            // FOR TETIN

            LoadTextureCube(@"Textures\Skyboxes\Sunset");
            LoadTexture(@"Textures\Test\cargo");
            LoadTexture(@"Textures\Test\cargo_NRM");
            LoadTexture(@"Textures\Test\metal01_d");
            LoadTexture(@"Textures\ParticleTest\Particle");

            //Wika i Kasia
            LoadTexture(@"Textures\Enviro\Buildings\Big_house_D");
            LoadTexture(@"Textures\Enviro\Buildings\Big_house_N");
        }

        /// <summary>
        /// 
        /// Load every effect to effect list.
        /// IMPORTANT!!! SET NAME FOR EVERY ELEMENT
        /// </summary>
        /// <param name="game"></param>
        public void LoadEffects(Game game)
        {
            if(!Effects.ContainsKey("BasicEffect"))
            {
                Effects.Add("BasicEffect", new BasicEffect(TrashSoupGame.Instance.GraphicsDevice));
            }
            if(!Effects.ContainsKey("SkinnedEffect"))
            {
                Effects.Add("SkinnedEffect", new SkinnedEffect(TrashSoupGame.Instance.GraphicsDevice));
            }

            string path = @"Effects\DefaultEffect";
            LoadEffect(path);
            path = @"Effects\NormalEffect";
            LoadEffect(path);
            path = @"Effects\SkyboxEffect";
            LoadEffect(path);
            path = @"Effects\CubeNormalEffect";
            LoadEffect(path);
            path = @"Effects\MirrorEffect";
            LoadEffect(path);
            path = @"Effects\WaterEffect";
            LoadEffect(path);
            path = @"Effects\ShadowMapEffect";
            LoadEffect(path);
            path = @"Effects\ShadowMapBlurredEffect";
            LoadEffect(path);
            path = @"Effects\POSTBlurEffect";
            LoadEffect(path);
            path = @"Effects\ShadowMapUnnormalizedEffect";
            LoadEffect(path);
        }

        /// <summary>
        /// Loads animation clip from Model object, when we load just an animated skeleton from Animations folder
        /// </summary>
        /// <param name="model"></param>
        /// <param name="animation"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        private KeyValuePair<string, SkinningModelLibrary.AnimationClip> LoadAnimationFromModel(Model model, Model animation, string newName)
        {
            // need to extract AnimationClips from animation and save it into new SkinningData with the rest
            // of the data from original skinned Model. 
            if((model.Tag as object[])[0] == null || (animation.Tag as object[])[0] == null) throw new InvalidOperationException("Either destination model or animation is not a skinned model");
            SkinningModelLibrary.SkinningData modelData = (model.Tag as object[])[0] as SkinningModelLibrary.SkinningData;
            SkinningModelLibrary.SkinningData animationData = (animation.Tag as object[])[0] as SkinningModelLibrary.SkinningData;
            if (modelData.SkeletonHierarchy.Count != animationData.SkeletonHierarchy.Count) throw new InvalidOperationException("Model hierarchy is not the same as the animation's");

            return new KeyValuePair<string, SkinningModelLibrary.AnimationClip>(newName, animationData.AnimationClips.Values.ElementAt(0));
        }

        /// <summary>
        /// Returns list of materials that are used by every given model's ModelMeshPart
        /// Loads these materials to material library as well.
        /// Effect types can be: BasicEffect, SkinnedEffect
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private List<Material> LoadBasicMaterialsFromModel(Model model, Effect toBeAdded)
        {
            List<Material> materials = new List<Material>();

            List<SkinnedModelLibrary.MaterialModel> materialModels = (List<SkinnedModelLibrary.MaterialModel>)(model.Tag as object[])[1];
            for (int i = 0; i < materialModels.Count; ++i )
                materialModels[i] = ProcessMaterialModelFilenames(materialModels[i]);

            List<string> materialNames = (List<string>)(model.Tag as object[])[2];

            Effect effectToAdd;

            if(toBeAdded == null)
            {
                if (model.Meshes[0].Effects[0] is BasicEffect)
                {
                    effectToAdd = Effects["BasicEffect"];
                }
                else if (model.Meshes[0].Effects[0] is SkinnedEffect)
                {
                    effectToAdd = Effects["SkinnedEffect"];
                }
                else
                {
                    Debug.Log("MATERIAL LOADING FAILED: Unrecognized effect type in model.");
                    return materials;
                }
            }
            else
            {
                effectToAdd = toBeAdded;
            }
            
            foreach(SkinnedModelLibrary.MaterialModel mm in materialModels)
            {
                Material mat = new Material(mm.MaterialName, effectToAdd);
                if (mm.MaterialTextureNames[0] != null) mat.DiffuseMap = LoadTexture(mm.MaterialTextureNames[0]);
                if (mm.MaterialTextureNames[1] != null) mat.NormalMap = LoadTexture(mm.MaterialTextureNames[1]);
                if (mm.MaterialTextureNames[2] != null) mat.CubeMap = LoadTextureCube(mm.MaterialTextureNames[2]);

                if(!this.Materials.ContainsKey(mat.Name))
                {
                    this.Materials.Add(mat.Name, mat);
                }
            }

            foreach(string matName in materialNames)
            {
                materials.Add(this.Materials[matName]);
            }

            return materials;
        }

        private SkinnedModelLibrary.MaterialModel ProcessMaterialModelFilenames(SkinnedModelLibrary.MaterialModel matModel)
        {
            string rootDir = TrashSoupGame.ROOT_DIRECTIORY;
            string pRootDir = TrashSoupGame.ROOT_DIRECTIORY_PROJECT;
            for (int i = 0; i < SkinnedModelLibrary.MaterialModel.TEXTURE_COUNT; ++i )
            {
                if (matModel.MaterialTextureNames[i] == null)
                    continue;

                string path = matModel.MaterialTextureNames[i];
                string newPath = "";
                string[] strDivided = path.Split(new char[] { '\\' });

                string fileName = (strDivided.Last()).Split(new char[] { '.' })[0];
                newPath += fileName;

                int j = strDivided.Count() - 2;     // points to pre-last element, which is first part of tha path
                string tmpPathElement = fileName;
                while (j >= 0 && !(strDivided[j]).Equals(rootDir) && !(strDivided[j]).Equals(pRootDir))
                {
                    tmpPathElement = strDivided[j];
                    newPath = tmpPathElement + "\\" + newPath;
                    --j;
                }

                matModel.MaterialTextureNames[i] = newPath;
            }
            return matModel;
        }

        #endregion
    }
}