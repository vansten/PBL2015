using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TrashSoup.Engine;
using TrashSoup.Gameplay;

namespace TrashSoup
{
    public class TrashSoupGame : Microsoft.Xna.Framework.Game
    {
        public static TrashSoupGame Instance { get; protected set; }
        public bool EditorMode = false;

        public GraphicsDeviceManager GraphicsManager { get; protected set; }
        SpriteBatch spriteBatch;

        public TrashSoupGame()
        {
            GraphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            Instance = this;
            Debug.Log("Engine start");
        }
        
        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ResourceManager.Instance.LoadContent(this);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if(!this.EditorMode)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    this.Exit();

                //Update Physics first !!
                PhysicsManager.Instance.Update(gameTime);

                //Updating input manager and GUI manager 
                //because of the fact that they don't want to be a game component
                InputManager.Instance.Update(gameTime);
                GUIManager.Instance.Update(gameTime);
                AudioManager.Instance.Update(gameTime);

                ResourceManager.Instance.CurrentScene.UpdateAll(gameTime);

                //TESTING PARTICLES
                for (int i = 0; i < ResourceManager.Instance.Particles.Count; ++i)
                {
                    ResourceManager.Instance.Particles[i].Update(gameTime);
                    if (ResourceManager.Instance.Particles[i].IsDead)
                        ResourceManager.Instance.Particles[i].SetEnabled();
                }

                //TESTING SAVE
                if (Keyboard.GetState().IsKeyDown(Keys.F5))
                    SaveManager.Instance.SaveFileAction();

                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            ResourceManager.Instance.CurrentScene.DrawAll(gameTime);

            //TESTING PARTICLES
            foreach(Particle p in ResourceManager.Instance.Particles)
                p.Draw(ResourceManager.Instance.CurrentScene.Cam);

            base.Draw(gameTime);

            GUIManager.Instance.Render(this.spriteBatch);
        }

    }
}
