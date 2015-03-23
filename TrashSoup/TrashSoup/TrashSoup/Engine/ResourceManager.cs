using System;
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

    class ResourceManager : Singleton<ResourceManager>
    {
        #region Variables
        public Scene CurrentScene;
        public List<Model> Models = new List<Model>();
        public List<Texture2D> Textures = new List<Texture2D>();
        public List<SpriteFont> Fonts = new List<SpriteFont>();
        public List<Effect> Effects = new List<Effect>();
        //place for materials
        public List<Cue> Sounds = new List<Cue>();
        public List<Particle> Particles = new List<Particle>();
        #endregion

        #region Methods
        public ResourceManager()
        {

        }

        public void LoadContent(Game game)
        {
            LoadModels(game);
            LoadTextures(game);
            LoadEffects(game);
            AudioManager.Instance.LoadContent();
            LoadCues();

            // because it pisses me off - Mav
            //AudioManager.Instance.PlayCue(GetCueFromCueList("Track1")); //default music from tutorial, just to check that everything works xD

            // FOR TETIN
            //Textures.Add(game.Content.Load<Texture2D>(@"Textures\Test\cargo"));
            //Textures.Add(game.Content.Load<Texture2D>(@"Textures\Test\metal01_d"));
            //Models.Add(game.Content.Load<Model>(@"Models\Test\TestBox"));
            //Models.Add(game.Content.Load<Model>(@"Models\Test\TestTerrain"));

            GameObject testBox = new GameObject(1, "testBox");
            List<Material> matList = new List<Material>();
            matList.Add(new Material(Textures[0], new BasicEffect(TrashSoupGame.Instance.GraphicsDevice)));
            testBox.MyTransform = new Transform(testBox, new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 0.0f), 1.0f);
            testBox.Components.Add(new CustomModel(testBox, new Model[] { Models[0], null, null }, 3, matList));
            testBox.Components.Add(new PlayerController(testBox));

            GameObject testTer = new GameObject(2, "testTer");
            List<Material> matList2 = new List<Material>();
            matList2.Add(new Material(Textures[1], new BasicEffect(TrashSoupGame.Instance.GraphicsDevice)));
            testTer.MyTransform = new Transform(testTer, new Vector3(0.0f, -10.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 0.0f), 5.0f);
            testTer.Components.Add(new CustomModel(testTer, new Model[] { Models[1], null, null }, 3, matList2));

            GameObject testBox2 = new GameObject(3, "testBox2");
            List<Material> matList3 = new List<Material>();
            matList3.Add(new Material(Textures[1], new BasicEffect(TrashSoupGame.Instance.GraphicsDevice)));
            testBox2.MyTransform = new Transform(testBox2, new Vector3(0.0f, 10.0f, 30.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 0.0f), 1.0f);
            testBox2.Components.Add(new CustomModel(testBox2, new Model[] { Models[0], null, null }, 3, matList3));
            testBox2.MyPhysicalObject = new PhysicalObject(testBox2, 1.0f, 0.05f, true);
            testBox2.MyPhysicalObject.AddForce(new Vector3(0.0f, 100.0f, 20.0f));

            CurrentScene = new Scene(new SceneParams(0, "test"));
            Camera cam = new Camera(1, "playerCam", Vector3.Transform(new Vector3(0.0f, 10.0f, -1.0f), Matrix.CreateRotationX(MathHelper.PiOver4 * 1.5f)),
                 new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 10.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), MathHelper.Pi / 3.0f, 0.1f, 2000.0f);
            cam.Components.Add(new CameraBehaviourComponent(cam, testBox));
            CurrentScene.Cam = cam;


            CurrentScene.ObjectsDictionary.Add(testTer.UniqueID, testTer);
            CurrentScene.ObjectsDictionary.Add(testBox.UniqueID, testBox);
            CurrentScene.ObjectsDictionary.Add(testBox2.UniqueID, testBox2);

            //TESTING PARTICLES
            ParticleSettings testSettings = new ParticleSettings(2, 1000, 2000, 100, 600, 16, 50, 2000, 3000);
            Particles.Add(new ExplosionParticle(game.GraphicsDevice, new Vector3(0.0f, 10.0f, -10.0f),
                SingleRandom.Instance.rnd.Next(testSettings.minLife, testSettings.maxLife),
                SingleRandom.Instance.rnd.Next(testSettings.minRoundTime, testSettings.maxRoundTime),
                SingleRandom.Instance.rnd.Next(testSettings.minParticlesPerRound, testSettings.maxParticlesPerRound),
                game.Content.Load<Texture2D>(@"Textures\ParticleTest\ParticleColors"),
                testSettings, Effects.ElementAt(0)));
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
        /// Method gets model from our list searching for elements which tag is "name" parameter;
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Model GetModelFromModelsList(String name)
        {
            return Models.Find(x => x.Tag.Equals(name));
        }


        /// <summary>
        /// 
        /// Method gets texture from our list searching for elements which name contains "name" parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Texture2D GetTextureFromTexturesList(String name)
        {
            return Textures.Find(x => x.Name.Contains(name));
        }

        /// <summary>
        /// 
        /// Method gets effect from our list searching for elements which name contains "name" parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Effect GetEffectFromEffectsList(String name)
        {
            return Effects.Find(x => x.Name.Contains(name));
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
        /// Load every model to model list.
        /// IMPORTANT!!! ADD TAGS TO EVERY MODEL
        /// </summary>
        /// <param name="game"></param>
        private void LoadModels(Game game)
        {
            Models.Add(game.Content.Load<Model>(@"Models/Test/TestBox"));
            Models[0].Tag = "Models/Test/TestBox";
            Models.Add(game.Content.Load<Model>(@"Models/Test/TestTerrain"));
            Models[1].Tag = "Models/Test/TestTerrain";
        }

        /// <summary>
        /// 
        /// Load every texture from content to textures list
        /// IMPORTANT!!! ADD NAME TO EVERY TEXTURE
        /// </summary>
        /// <param name="game"></param>
        private void LoadTextures(Game game)
        {
            // FOR TETIN
            Textures.Add(game.Content.Load<Texture2D>(@"Textures/Test/cargo"));
            Textures[0].Name = "Textures/Test/cargo";
            Textures.Add(game.Content.Load<Texture2D>(@"Textures/Test/metal01_d"));
            Textures[1].Name = "Textures/Test/metal01_d";
            Textures.Add(game.Content.Load<Texture2D>(@"Textures/ParticleTest/Particle"));
            Textures[2].Name = "Textures/ParticleTest/Particle";

        }

        /// <summary>
        /// 
        /// Load every effect to effect list.
        /// IMPORTANT!!! ADD NAME TO EVERY EFFECT
        /// </summary>
        /// <param name="game"></param>
        private void LoadEffects(Game game)
        {
            Effects.Add(game.Content.Load<Effect>(@"Effects/Particle"));
            Effects.ElementAt(0).CurrentTechnique = Effects.ElementAt(0).Techniques["Technique1"];
            Effects.ElementAt(0).Parameters["theTexture"].SetValue(Textures.ElementAt(2));
            Effects.ElementAt(0).Name = "Effects/Particle";

            Effects.Add(game.Content.Load<Effect>(@"Effects/Billboard"));
            Effects.ElementAt(1).CurrentTechnique = Effects.ElementAt(1).Techniques["Technique1"];
            Effects.ElementAt(1).Name = "Effects/Billboard";
        }
        #endregion
    }
}
