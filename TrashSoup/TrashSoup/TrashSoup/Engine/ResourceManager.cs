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
            LoadEffects(game);
            AudioManager.Instance.LoadContent();
            LoadCues();

            // because it pisses me off - Mav
            //AudioManager.Instance.PlayCue(GetCueFromCueList("Track1")); //default music from tutorial, just to check that everything works xD

            // FOR TETIN
            Textures.Add(game.Content.Load<Texture2D>(@"Textures\Test\cargo"));
            Models.Add(game.Content.Load<Model>(@"Models\Test\TestBox"));

            CurrentScene = new Scene(new SceneParams(0, "test"));
            Camera cam = new Camera(1, "playerCam", new Vector3(25.0f, 25.0f, 25.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), MathHelper.PiOver4, 0.01f, 2000.0f);
            cam.Components.Add(new CameraBehaviourComponent(cam));

            GameObject testBox = new GameObject(1, "testBox");
            List<Material> matList = new List<Material>();
            matList.Add(new Material(Textures[0], new BasicEffect(TrashSoupGame.Instance.GraphicsDevice)));
            testBox.MyTransform = new Transform(testBox);
            //testBox.MyTransform.Scale = 0.1f;
            testBox.Components.Add(new CustomModel(testBox, new Model[] { Models[0], null, null }, 3, matList));

            CurrentScene.Cam = cam;
            CurrentScene.ObjectsDictionary.Add(testBox.UniqueID, testBox);
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
        /// Load every effect to effect list.
        /// </summary>
        /// <param name="game"></param>
        private void LoadEffects(Game game)
        {
            Effects.Add(game.Content.Load<Effect>(@"Effects\Particle"));
        }
        #endregion
    }
}