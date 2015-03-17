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

        public GraphicsDeviceManager GraphicsManager { get; protected set; }
        SpriteBatch spriteBatch;

        #region Teting GUI

        private bool disabled = false;
        private List<GUIButton> myButtons = new List<GUIButton>();
        private Texture2D buttonNormalTexture;
        private int currButton = 0;

        #endregion

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

            #region Teting GUI

            this.buttonNormalTexture = this.Content.Load<Texture2D>("Textures/GUITest/ButtonNormal");
            this.myButtons.Add(new GUIButton(this.buttonNormalTexture, this.Content.Load<Texture2D>("Textures/GUITest/ButtonHover"), this.Content.Load<Texture2D>("Textures/GUITest/ButtonPressed"), this.Content.Load<Texture2D>("Textures/GUITest/ButtonDisabled"), this.ButtonPressedCallback, new Vector2(0.1f, 0.0f), 0.25f, 0.1f));
            this.myButtons.Add(new GUIButton(this.buttonNormalTexture, this.Content.Load<Texture2D>("Textures/GUITest/ButtonHover"), this.Content.Load<Texture2D>("Textures/GUITest/ButtonPressed"), this.Content.Load<Texture2D>("Textures/GUITest/ButtonDisabled"), this.Button2PressedCallback, new Vector2(0.1f, 0.4f), 0.25f, 0.1f));

            #endregion
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            #region Teting GUI for game pad

            //Just for GUIButton testing
            //Will be removed after everyone sees it
            if(!this.disabled)
            {
                if(InputManager.Instance.GetGamePadButtonDown(Buttons.DPadDown))
                {
                    this.currButton = this.currButton - 1;
                    if (this.currButton < 0)
                    {
                        this.currButton = this.myButtons.Count - 1;
                    }
                }
                else if (InputManager.Instance.GetGamePadButtonDown(Buttons.DPadUp))
                {
                    this.currButton = this.currButton + 1;
                    if (this.currButton > this.myButtons.Count - 1)
                    {
                        this.currButton = 0;
                    }
                }

                this.myButtons[currButton].ChangeState(GUIButtonState.HOVER);
                foreach (GUIButton button in this.myButtons)
                {
                    if(button != this.myButtons[currButton])
                    {
                        button.ChangeState(GUIButtonState.NORMAL);
                    }
                }

                GUIManager.Instance.DrawButton(this.myButtons[0]);
                GUIManager.Instance.DrawButton(this.myButtons[1]);
            }
            else
            {
                if(InputManager.Instance.GetGamePadButtonDown(Buttons.B))
                {
                    this.disabled = false;
                    this.myButtons[0].Enable();
                    this.myButtons[1].Enable();
                }
            }

            #endregion

            //Updating input manager and GUI manager 
            //because of the fact that they don't want to be a game component
            InputManager.Instance.Update(gameTime);
            GUIManager.Instance.Update(gameTime);
            AudioManager.Instance.Update(gameTime);

            ResourceManager.Instance.CurrentScene.Cam.Update(gameTime);
            //Updating scene, only for testing?
            //[vansten] Of course it's only for testing, this should be in Scene update method
            //[vansten] Added testing code for physics simulation
            ResourceManager.Instance.CurrentScene.Cam.Update(gameTime);
            foreach(GameObject obj in ResourceManager.Instance.CurrentScene.ObjectsDictionary.Values)
            {
                obj.Update(gameTime);
                if(gameTime.TotalGameTime.Seconds > 8)  //time to turn off physics
                {
                    if(obj.MyPhysicalObject != null)
                    {
                        obj.MyPhysicalObject = null;
                    }
                }
                else if(gameTime.TotalGameTime.Seconds > 4) //time to awake
                {
                    if(obj.MyPhysicalObject != null && obj.MyPhysicalObject.Sleeping)
                    {
                        obj.MyPhysicalObject.Awake();
                    }
                }
                else if(gameTime.TotalGameTime.Seconds > 2) //time to go sleep for a while
                {
                    if (obj.MyPhysicalObject != null && !obj.MyPhysicalObject.Sleeping)
                    {
                        obj.MyPhysicalObject.Sleep();
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Drawing scene, only for testing?
            foreach (GameObject obj in ResourceManager.Instance.CurrentScene.ObjectsDictionary.Values)
            {
                obj.Draw(gameTime);
            }

            base.Draw(gameTime);

            GUIManager.Instance.Render(this.spriteBatch);
        }

        #region Functions for teting GUI

        private void ButtonPressedCallback()
        {
            if(this.disabled)
            {
                this.myButtons[0].Enable();
                this.myButtons[1].Enable();
            }
            else
            {
                this.myButtons[0].Disable();
                this.myButtons[1].Disable();
            }
            this.disabled = !this.disabled;
        }

        private void Button2PressedCallback()
        {
            if (this.disabled)
            {
                this.myButtons[0].Enable();
                this.myButtons[1].Enable();
            }
            else
            {
                this.myButtons[0].Disable();
                this.myButtons[1].Disable();
            }
            this.disabled = !this.disabled;
        }

        #endregion
    }
}
