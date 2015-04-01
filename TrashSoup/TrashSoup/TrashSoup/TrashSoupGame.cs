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
        public const string ROOT_DIRECTIORY = "Content";
        public const string ROOT_DIRECTIORY_PROJECT = "TrashSoupContent";
        public bool EditorMode = false;
        private bool f5pressed = false;
        private bool f6pressed = false;

        public GraphicsDeviceManager GraphicsManager { get; protected set; }
        SpriteBatch spriteBatch;

        public TrashSoupGame()
        {
            GraphicsManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = ROOT_DIRECTIORY;
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
                //Generate a direction within 15 degrees of (0, 1, 0)
                Vector3 offset = new Vector3(MathHelper.ToRadians(10.0f));
                Vector3 randAngle = Vector3.Up + new Vector3(
                    -offset.X + (float)SingleRandom.Instance.rnd.NextDouble() * (offset.X - (-offset.X)),
                    -offset.Y + (float)SingleRandom.Instance.rnd.NextDouble() * (offset.Y - (-offset.Y)),
                    -offset.Z + (float)SingleRandom.Instance.rnd.NextDouble() * (offset.Z - (-offset.Z))
                    );
                ResourceManager.Instance.ps.AddParticle(new Vector3(0.0f, 10.0f, -5.0f),
                    randAngle, 20.0f);
                ResourceManager.Instance.ps.Update();

                //TESTING SAVE
                if (Keyboard.GetState().IsKeyDown(Keys.F5) && !f5pressed)
                {
                    SaveManager.Instance.SaveFileAction();
                    Debug.Log("Save Completed");
                    f5pressed = true;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.F5) && f5pressed)
                {
                    f5pressed = false;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.F6) && !f6pressed)
                {
                    SaveManager.Instance.LoadFileAction();
                    Debug.Log("Load Completed");
                    f6pressed = true;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.F6) && f6pressed)
                {
                    f6pressed = false;
                }

                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            ResourceManager.Instance.CurrentScene.DrawAll(gameTime);

            ResourceManager.Instance.ps.Draw();

            base.Draw(gameTime);

            GUIManager.Instance.Render(this.spriteBatch);
        }

    }
}
