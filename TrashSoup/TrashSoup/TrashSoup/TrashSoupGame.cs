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
        public RenderTarget2D DefaultRenderTarget { get; set; }
        public GameTime TempGameTime { get; private set; }

#if DEBUG
        //Variables that allow us to display fps counter :) only in debug mode
        private int frames = 0;
        private float timer = 0.0f;
        private SpriteFont font;
        float fps = 0.0f;
#endif

        private RenderTarget2D actualRenderTarget;
        public RenderTarget2D ActualRenderTarget
        {
            get
            {
                return actualRenderTarget;
            }
            set
            {
                actualRenderTarget = value;
                GraphicsDevice.SetRenderTarget(actualRenderTarget);
            }

        }

        private bool f5pressed = false;
        private bool f6pressed = false;

        public GraphicsDeviceManager GraphicsManager { get; protected set; }
        SpriteBatch spriteBatch;

        public TrashSoupGame()
        {
            GraphicsManager = new GraphicsDeviceManager(this);
            GraphicsManager.PreferredBackBufferHeight = 720;
            GraphicsManager.PreferredBackBufferWidth = 1280;
            GraphicsManager.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(SetToPreserve);
#if DEBUG
            //Turning of lock to 60 fps :) only in debug mode
            this.IsFixedTimeStep = false;
            this.GraphicsManager.SynchronizeWithVerticalRetrace = false;
#endif
            GraphicsManager.ApplyChanges();
            Content.RootDirectory = ROOT_DIRECTIORY;
            this.IsMouseVisible = false;
            Instance = this;

            Debug.Log("Engine start");
        }

        protected override void Initialize()
        {
            if (!this.EditorMode)
            {
                DefaultRenderTarget = null;
            }
            ActualRenderTarget = DefaultRenderTarget;

            this.TempGameTime = new GameTime();

            base.Initialize();
        }

        public void EditorLoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ResourceManager.Instance.LoadEffects(this);
            //this.LoadContent();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ResourceManager.Instance.LoadContent(this);
#if DEBUG
            //Loading font for fps counter :) only in debug mode
            font = Content.Load<SpriteFont>("Fonts/FontTest");
#endif
        }

        protected override void UnloadContent()
        {

        }

        public void EditorUpdate()
        {
            //ResourceManager.Instance.CurrentScene.Cam.Update(new GameTime());
            this.Update(new GameTime());
        }

        protected override void Update(GameTime gameTime)
        {
            if(!this.EditorMode)
            {
#if DEBUG
                //Drawing fps counter ;) only if debug mode
                //Just for check if added functionality makes our game to slow :( Like a physics...
                ++frames;
                timer += gameTime.ElapsedGameTime.Milliseconds;
                if (timer > 1000.0f)
                {
                    fps = frames / (timer / 1000.0f);
                    timer = 0.0f;
                    frames = 0;
                }
                GUIManager.Instance.DrawText(this.font, "FPS: " + fps.ToString(), new Vector2(0.45f, 0.1f), Color.Lime);
#endif
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    this.Exit();

                //Updating input manager and GUI manager 
                //because of the fact that they don't want to be a game component
                InputManager.Instance.Update(gameTime);
                GUIManager.Instance.Update(gameTime);
                AudioManager.Instance.Update(gameTime);

                PhysicsManager.Instance.CheckForTriggers();
                ResourceManager.Instance.CurrentScene.UpdateAll(gameTime);

                //TESTING PARTICLES
                //Generate a direction within 15 degrees of (0, 1, 0)
                Vector3 offset = new Vector3(MathHelper.ToRadians(10.0f));
                Vector3 randAngle = Vector3.Up + new Vector3(
                    -offset.X + (float)SingleRandom.Instance.rnd.NextDouble() * (offset.X - (-offset.X)),
                    -offset.Y + (float)SingleRandom.Instance.rnd.NextDouble() * (offset.Y - (-offset.Y)),
                    -offset.Z + (float)SingleRandom.Instance.rnd.NextDouble() * (offset.Z - (-offset.Z))
                    );
                //ResourceManager.Instance.ps.AddParticle(new Vector3(0.0f, 10.0f, -5.0f),
                    //randAngle, 20.0f);
                //ResourceManager.Instance.ps.Update();

                //TESTING SAVE
                //if (Keyboard.GetState().IsKeyDown(Keys.F5) && !f5pressed)
                //{
                //    SaveManager.Instance.GetXmlPath();
                //    SaveManager.Instance.SaveFileAction();
                //    Debug.Log("Save Completed");
                //    f5pressed = true;
                //}
                //if (Keyboard.GetState().IsKeyUp(Keys.F5) && f5pressed)
                //{
                //    f5pressed = false;
                //}
                if (Keyboard.GetState().IsKeyDown(Keys.F6) && !f6pressed)
                {
                    SaveManager.Instance.GetXmlPath();
                    SaveManager.Instance.LoadFileAction();
                    Debug.Log("Load Completed");
                    f6pressed = true;
                }
                if (Keyboard.GetState().IsKeyUp(Keys.F6) && f6pressed)
                {
                    f6pressed = false;
                }

                #region TETIN

                if (InputManager.Instance.GetKeyboardButtonDown(Keys.T))
                {
                    ResourceManager.Instance.CurrentScene.ObjectsDictionary[3].MyTransform.Position = new Vector3(-50.0f, 2.0f, -30.0f);
                    ResourceManager.Instance.CurrentScene.ObjectsDictionary[3].MyTransform.Update(gameTime);
                }

                #endregion

                base.Update(gameTime);
            }
            else
            {
                //Editor mode camera control ;)
                ResourceManager.Instance.CurrentScene.UpdateAll(gameTime);
                if(ResourceManager.Instance.CurrentScene != null && ResourceManager.Instance.CurrentScene.Cam != null)
                {
                    ResourceManager.Instance.CurrentScene.Cam.Update(gameTime);
                }
            }
        }

        public void EditorDraw()
        {
            this.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if(ResourceManager.Instance.CurrentScene != null)
            {
                ResourceManager.Instance.CurrentScene.DrawAll(null, null, TempGameTime, true);
            }

            if(ResourceManager.Instance.ps != null)
            {
                ResourceManager.Instance.ps.Draw();
            }
            GUIManager.Instance.Render(this.spriteBatch);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            ResourceManager.Instance.CurrentScene.DrawAll(null, null, gameTime, true);

            if (ResourceManager.Instance.ps != null)
            {
                //ResourceManager.Instance.ps.Draw();
            }

            base.Draw(gameTime);

            GUIManager.Instance.Render(this.spriteBatch);
        }

        protected void SetToPreserve(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        public SpriteBatch GetSpriteBatch()
        {
            return this.spriteBatch;
        }
    }
}
