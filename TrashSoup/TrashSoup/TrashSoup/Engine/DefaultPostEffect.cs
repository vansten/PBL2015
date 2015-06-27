using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TrashSoup.Engine
{
    public class DefaultPostEffect : PostEffect
    {
        #region effectParameters

        private EffectParameter epColorAddition;
        private EffectParameter epColorMultiplication;
        private EffectParameter epVignetteColor;
        private EffectParameter epVignetteRadius;

        #endregion

        #region properties

        public Vector3 ColorAddition { get; set; }
        public Vector3 ColorMultiplication { get; set; }
        public Vector3 VignetteColor { get; set; }
        public Vector2 VignetteRadius { get; set; }

        #endregion

        #region methods

        public DefaultPostEffect(string name) : base(name)
        {
            ColorAddition = Vector3.Zero;
            ColorMultiplication = Vector3.One;
            VignetteColor = Vector3.One;
            VignetteRadius = new Vector2(0.6f, 0.1f);
        }

        public override void UpdateEffect()
        {
            epColorAddition.SetValue(ColorAddition);
            epColorMultiplication.SetValue(ColorMultiplication);
            epVignetteColor.SetValue(VignetteColor);
            epVignetteRadius.SetValue(VignetteRadius);
        }

        protected override void LoadAssociatedEffect()
        {
            MyEffect = ResourceManager.Instance.LoadEffect(@"Effects\POSTDefaultEffect");

            epColorAddition = MyEffect.Parameters["ColorAddition"];
            epColorMultiplication = MyEffect.Parameters["ColorMultiplication"];
            epVignetteColor = MyEffect.Parameters["VignetteColor"];
            epVignetteRadius = MyEffect.Parameters["VignetteRadius"];
        }

        #endregion
    }
}
