using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup
{
    #region GUIElement class

    class GUIElement
    {
        #region Variables

        protected Vector2 position;
        protected float width;
        protected float height;

        #endregion

        #region Methods

        public GUIElement(Vector2 pos, float width, float height)
        {
            this.position = pos;
            this.width = width;
            this.height = height;
        }

        public GUIElement(Rectangle rect)
        {
            this.position = new Vector2(rect.X, rect.Y);
            this.width = rect.Width;
            this.height = rect.Height;
        }

        #endregion

        #region Methods

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        #endregion
    }

    #endregion

    #region GUITexture class

    class GUITexture : GUIElement
    {
        #region Variables

        private Texture2D texture;

        #endregion

        #region Methods

        public GUITexture(Texture2D texture, Rectangle rect) : base(rect)
        {
            this.texture = texture;
        }

        public GUITexture(Texture2D texture, Vector2 position, float width, float height) : base(position, width, height)
        {
            this.texture = texture;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.texture, this.position, Color.White);
            base.Draw(spriteBatch);
        }

        #endregion
    }

    #endregion

    #region GUIText class

    class GUIText : GUIElement
    {
        #region Variables

        private SpriteFont font;
        private string text;
        private Color color;

        #endregion

        #region Methods

        public GUIText(SpriteFont font, string text, Color color, Vector2 position)
            : base(position, 0.0f, 0.0f)
        {
            this.font = font;
            this.text = text;
            this.color = color;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(this.font, this.text, this.position, this.color);
            base.Draw(spriteBatch);
        }

        #endregion
    }

    #endregion

    #region GUIManager class

    class GUIManager : Singleton<GUIManager>
    {
        #region Variables

        private List<GUIElement> elementsToDraw = new List<GUIElement>();

        #endregion 

        #region Methods

        public GUIManager()
        {

        }

        /// <summary>
        /// 
        /// Updates state of GUIElements
        /// </summary>
        public void Update(GameTime gameTime)
        {
            foreach (GUIElement element in this.elementsToDraw)
            {
                element.Update(gameTime);
            }
        }

        /// <summary>
        /// 
        /// Draws texture on top of everything, should be called per frame for each GUI texture that has to be drawn
        /// </summary>
        public void DrawTexture(Texture2D texture, Vector2 position, float width, float height)
        {
            GUITexture newTexture = new GUITexture(texture, position, width, height);
            this.elementsToDraw.Add(newTexture);
        }

        /// <summary>
        /// 
        /// Draws texture on top of everything, should be called per frame for each GUI texture that has to be drawn
        /// </summary>
        public void DrawTexture(Texture2D texture, Rectangle rect)
        {
            GUITexture newTexture = new GUITexture(texture, rect);
            this.elementsToDraw.Add(newTexture);
        }

        /// <summary>
        /// 
        /// Draw given guiTexture on top of everything, should be called per frame for each GUI texture that has to be drawn
        /// </summary>
        public void DrawTexture(GUITexture guiTexture)
        {
            this.elementsToDraw.Add(guiTexture);
        }

        /// <summary>
        /// 
        /// Draws text on top of everything, should be called per frame for each text that has to be drawn
        /// </summary>
        public void DrawText(SpriteFont font, string text, Vector2 position, Color color)
        {
            GUIText newText = new GUIText(font, text, color, position);
            this.elementsToDraw.Add(newText);
        }

        /// <summary>
        /// 
        /// Draws given guiText on top of everything, should be called per frame for each text that has to be drawn
        /// </summary>
        public void DrawText(GUIText guiText)
        {
            this.elementsToDraw.Add(guiText);
        }

        /// <summary>
        /// 
        /// Renders current list of GUIElements passed in Draw* methods
        /// </summary>
        public void Render(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (GUIElement element in this.elementsToDraw)
            {
                element.Draw(spriteBatch);
            }

            elementsToDraw.Clear();

            spriteBatch.End();
        }

        #endregion
    }

    #endregion
}
