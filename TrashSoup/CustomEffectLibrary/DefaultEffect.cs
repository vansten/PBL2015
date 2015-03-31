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

namespace CustomEffectLibrary
{
    public class DefaultEffect : BasicEffect
    {
        #region enums

        public enum EffectTechniques
        {
            Main
        }

        #endregion

        #region properties

        public float Transparency { get; set; }

        public EffectTechniques Technique;

        #endregion

        #region methods

        public DefaultEffect(GraphicsDevice device) : base(device)
        {
            this.Name = "DefaultEffect";
            this.Transparency = 1.0f;
        }

        protected override void OnApply()
        {
            base.OnApply();

            Parameters["Transparency"].SetValue(this.Transparency);

            switch(Technique)
            {
                case EffectTechniques.Main:
                    CurrentTechnique = Techniques["Main"];
                    break;
                default:
                    CurrentTechnique = Techniques["Main"];
                    break;
            }
        }

        #endregion
    }
}
