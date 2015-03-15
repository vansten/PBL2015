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

namespace TrashSoup
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Just for GUIButton testing
        //Will be removed after everyone sees it
        private bool disabled = false;
        private GUIButton myButton;
        private Texture2D buttonNormalTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ResourceManager.Instance.LoadContent();

            //Just for GUIButton testing
            //Will be removed after everyone sees it
            this.buttonNormalTexture = this.Content.Load<Texture2D>("Textures/GUITest/ButtonNormal");
            this.myButton = new GUIButton(this.buttonNormalTexture, this.Content.Load<Texture2D>("Textures/GUITest/ButtonHover"), this.Content.Load<Texture2D>("Textures/GUITest/ButtonPressed"), this.Content.Load<Texture2D>("Textures/GUITest/ButtonDisabled"), this.ButtonPressedCallback, Vector2.Zero, this.buttonNormalTexture.Width, this.buttonNormalTexture.Height);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            base.Update(gameTime);

            //Just for GUIButton testing
            //Will be removed after everyone sees it
            if(!this.disabled)
            {
                GUIManager.Instance.DrawButton(this.myButton);
            }
            else
            {
                if(Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    this.disabled = false;
                    this.myButton.Enable();
                }
            }

            //Updating input manager and GUI manager 
            //because of the fact that they don't want to be a game component
            InputManager.Instance.Update(gameTime);
            GUIManager.Instance.Update(gameTime);
            AudioManager.Instance.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);

            GUIManager.Instance.Render(this.spriteBatch);
        }

        //Just for GUIButton testing
        //Will be removed after everyone sees it
        private void ButtonPressedCallback()
        {
            if(this.disabled)
            {
                this.myButton.Enable();
            }
            else
            {
                this.myButton.Disable();
            }
            this.disabled = !this.disabled;
        }
    }
}
