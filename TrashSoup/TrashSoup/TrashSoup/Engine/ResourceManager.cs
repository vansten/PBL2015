﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using TrashSoup;
using TrashSoup.Gameplay;

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
        public List<Effect> Effects = new List<Effect>();
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
            AddAnimation("Animations/Test/walking_1");
            AddAnimation("Animations/Test/idle_1");
            AddAnimation("Animations/Test/jump_1");

            // loading materials
            List<Material> testPlayerMats = new List<Material>();
            Material testPlayerMat = new Material("testPlayerMat", this.Effects[3], Textures[@"Textures\Test\cargo"]);
            testPlayerMats.Add(testPlayerMat);
            testPlayerMat.NormalMap = Textures[@"Textures\Test\cargo_NRM"];
            testPlayerMat.Glossiness = 40.0f;
            testPlayerMat.Transparency = 1.0f;
            this.Materials.Add(testPlayerMat.Name, testPlayerMat);

            List<Material> testPlayerMats2 = new List<Material>();
            Material testPlayerMat2 = new Material("testPlayerMat2", this.Effects[7], Textures[@"Textures\Test\cargo"]);
            testPlayerMats2.Add(testPlayerMat2);
            testPlayerMat2.NormalMap = Textures[@"Textures\Test\cargo_NRM"];
            testPlayerMat2.CubeMap = TexturesCube[@"Textures\Skyboxes\Sunset"];
            testPlayerMat2.Glossiness = 40.0f;
            testPlayerMat2.ReflectivityColor = new Vector3(1.0f, 0.0f, 1.0f);
            testPlayerMat2.ReflectivityBias = 0.7f;
            testPlayerMat2.Transparency = 1.0f;
            this.Materials.Add(testPlayerMat2.Name, testPlayerMat2);

            List<Material> playerMats = LoadBasicMaterialsFromModel(Models["Models/Test/TestGuy"], this.Effects[5]);

            List<Material> testTerMats = new List<Material>();
            Material testTerMat = new Material("testTerMat", this.Effects[2], Textures[@"Textures\Test\metal01_d"]);
            testTerMat.SpecularColor = new Vector3(0.1f, 0.1f, 0.0f);
            testTerMat.Glossiness = 10.0f;
            this.Materials.Add(testTerMat.Name, testTerMat);
            testTerMats.Add(testTerMat);

            List<Material> testSBMats = new List<Material>();
            Material testSBMat = new Material("testSBMat", this.Effects[6]);
            testSBMat.CubeMap = TexturesCube[@"Textures\Skyboxes\Sunset"];
            testSBMat.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
            testSBMat.Glossiness = 100.0f;
            this.Materials.Add(testSBMat.Name, testSBMat);
            testSBMats.Add(testSBMat);

            // loading gameobjects
            GameObject testBox = new GameObject(1, "Player");
            testBox.MyTransform = new Transform(testBox, new Vector3(0.0f, 0.0f, -40.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 0.0f), 0.2f);
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

            GameObject testTer = new GameObject(2, "testTer");
            testTer.MyTransform = new Transform(testTer, new Vector3(0.0f, -10.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 0.0f), 5.0f);
            testTer.Components.Add(new CustomModel(testTer, new Model[] { Models["Models/Test/TestTerrain"], null, null }, 3, testTerMats));

            GameObject testBox2 = new GameObject(3, "testBox2");
            testBox2.MyTransform = new Transform(testBox2, new Vector3(-100.0f, 40.0f, 70.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 0.0f), 5.0f);
            testBox2.Components.Add(new CustomModel(testBox2, new Model[] { Models["Models/Test/TestSphere"], null, null }, 3, testPlayerMats));
            testBox2.MyCollider = new BoxCollider(testBox2);

            GameObject testBox3 = new GameObject(5, "testBox3");
            testBox3.MyTransform = new Transform(testBox3, new Vector3(100.0f, 40.0f, 70.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 0.0f), 5.0f);
            testBox3.Components.Add(new CustomModel(testBox3, new Model[] { Models["Models/Test/TestSphere"], null, null }, 3, testPlayerMats2));
            testBox3.MyCollider = new BoxCollider(testBox3);

            GameObject skyBox = new GameObject(4, "skyBox");
            skyBox.MyTransform = new Transform(skyBox, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), 1000.0f);
            skyBox.Components.Add(new CustomModel(skyBox, new Model[] { Models["Models/Test/TestCube"], null, null }, 3, testSBMats));

            // adding lights
            LightAmbient amb = new LightAmbient(100, "LightAmbient", new Vector3(0.05f, 0.05f, 0.1f));
            LightDirectional ldr = new LightDirectional(101, "LightDirectional1", new Vector3(0.5f, 0.4f, 0.3f), new Vector3(1.0f, 0.8f, 0.8f), new Vector3(-1.0f, -1.0f, -1.0f));
            LightPoint lp1 = new LightPoint(110, "LightPoint1", new Vector3(0.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), 200.0f);
            lp1.MyTransform = new Transform(lp1, new Vector3(-40.0f, 40.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), 1.0f);

            // loading scene
            CurrentScene = new Scene(new SceneParams(0, "test"));

            Camera cam = null;

            if(TrashSoupGame.Instance.EditorMode)
            {
                //Editor camera
                cam = new EditorCamera(1, "editorCam", Vector3.Transform(new Vector3(0.0f, 10.0f, -50.0f), Matrix.CreateRotationX(MathHelper.PiOver4 * 1.5f)),
                     new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 10.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), MathHelper.Pi / 3.0f, 0.1f, 2000.0f);
            }
            else
            {
                //Game camera
                cam = new Camera(1, "playerCam", Vector3.Transform(new Vector3(0.0f, 10.0f, -1.0f), Matrix.CreateRotationX(MathHelper.PiOver4 * 1.5f)) + new Vector3(0.0f, 4.0f, 0.0f),
                     new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 25.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), MathHelper.Pi / 3.0f, 0.1f, 2000.0f);
                cam.Components.Add(new CameraBehaviourComponent(cam, testBox));
            }
            CurrentScene.Cam = cam;

            // adding items to scene
            CurrentScene.ObjectsDictionary.Add(skyBox.UniqueID, skyBox);
            CurrentScene.ObjectsDictionary.Add(testTer.UniqueID, testTer);
            CurrentScene.ObjectsDictionary.Add(testBox.UniqueID, testBox);
            CurrentScene.ObjectsDictionary.Add(testBox2.UniqueID, testBox2);
            CurrentScene.ObjectsDictionary.Add(testBox3.UniqueID, testBox3);

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
        private void AddModel(String path)
        {
            Models.Add(path, TrashSoupGame.Instance.Content.Load<Model>(path));
        }

        private void AddAnimation(String path)
        {
            Animations.Add(path, TrashSoupGame.Instance.Content.Load<Model>(path));
        }

        /// <summary>
        /// Used to load a single texture. If it doesn't exist in resourceManager, Content.Load is called.
        /// </summary>
        /// <param name="texturePath"></param>
        private Texture2D LoadTexture(string texturePath)
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
        private TextureCube LoadTextureCube(string texturePath)
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
        /// 
        /// Load every texture from content to textures list
        /// IMPORTANT!!! SET NAME FOR EVERY ELEMENT
        /// </summary>
        /// <param name="game"></param>
        private void LoadTextures(Game game)
        {
            // Adding "default" textures for all maps containing only one pixel in one color
            uint diffColor = 0xFFFFFFFF;
            uint normColor = 0x0F0FFFFF;
            uint blackColor = 0x000000FF;
            Texture2D defDiff = new Texture2D(TrashSoupGame.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            defDiff.SetData<uint>(new uint[] { diffColor });
            Textures.Add("DefaultDiffuse", defDiff);
            Texture2D defNrm = new Texture2D(TrashSoupGame.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            defNrm.SetData<uint>(new uint[] { normColor });
            Textures.Add("DefaultNormal", defNrm);
            TextureCube defCbc = new TextureCube(TrashSoupGame.Instance.GraphicsDevice, 1, false, SurfaceFormat.Color);
            defCbc.SetData<uint>(CubeMapFace.NegativeX, new uint[] { blackColor });
            defCbc.SetData<uint>(CubeMapFace.PositiveX, new uint[] { blackColor });
            defCbc.SetData<uint>(CubeMapFace.NegativeY, new uint[] { blackColor });
            defCbc.SetData<uint>(CubeMapFace.PositiveY, new uint[] { blackColor });
            defCbc.SetData<uint>(CubeMapFace.NegativeZ, new uint[] { blackColor });
            defCbc.SetData<uint>(CubeMapFace.PositiveZ, new uint[] { blackColor });
            TexturesCube.Add("DefaultCube", defCbc);
            ///////////////////////////////////////////

            // FOR TETIN
            TexturesCube.Add(@"Textures\Skyboxes\Sunset", game.Content.Load<TextureCube>(@"Textures\Skyboxes\Sunset"));

            Textures.Add(@"Textures\Test\cargo", game.Content.Load<Texture2D>(@"Textures\Test\cargo"));
            Textures.Add(@"Textures\Test\cargo_NRM", game.Content.Load<Texture2D>(@"Textures\Test\cargo_NRM"));
            Textures.Add(@"Textures\Test\metal01_d", game.Content.Load<Texture2D>(@"Textures\Test\metal01_d"));
            Textures.Add(@"Textures\ParticleTest\Particle", game.Content.Load<Texture2D>(@"Textures\ParticleTest\Particle"));
        }

        /// <summary>
        /// 
        /// Load every effect to effect list.
        /// IMPORTANT!!! SET NAME FOR EVERY ELEMENT
        /// </summary>
        /// <param name="game"></param>
        private void LoadEffects(Game game)
        {
            //Effects.Add(game.Content.Load<Effect>(@"Effects\Particle"));
            //Effects.ElementAt(0).CurrentTechnique = Effects.ElementAt(0).Techniques["Technique1"];
            //Effects.ElementAt(0).Parameters["theTexture"].SetValue(Textures[@"Textures\ParticleTest\Particle"]);

            //Effects.Add(game.Content.Load<Effect>(@"Effects\Billboard"));
            //Effects.ElementAt(1).CurrentTechnique = Effects.ElementAt(1).Techniques["Technique1"];
            Effect ef = TrashSoupGame.Instance.Content.Load<Effect>(@"Effects\DefaultEffect");
            Effects.Add(new BasicEffect(TrashSoupGame.Instance.GraphicsDevice));
            Effects.Add(new SkinnedEffect(TrashSoupGame.Instance.GraphicsDevice));
            Effects.Add(ef);
            Effects.Add(TrashSoupGame.Instance.Content.Load<Effect>(@"Effects\NormalEffect"));
            Effects.Add(TrashSoupGame.Instance.Content.Load<Effect>(@"Effects\DefaultSkinnedEffect"));
            Effects.Add(TrashSoupGame.Instance.Content.Load<Effect>(@"Effects\NormalSkinnedEffect"));
            Effects.Add(TrashSoupGame.Instance.Content.Load<Effect>(@"Effects\SkyboxEffect"));
            Effects.Add(TrashSoupGame.Instance.Content.Load<Effect>(@"Effects\CubeNormalEffect"));
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
                    effectToAdd = Effects[0];
                }
                else if (model.Meshes[0].Effects[0] is SkinnedEffect)
                {
                    effectToAdd = Effects[1];
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

                this.Materials.Add(mat.Name, mat);
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
                while (!(strDivided[j]).Equals(rootDir) && !(strDivided[j]).Equals(pRootDir) && j >= 0)
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