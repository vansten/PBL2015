using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using CustomEffectLibrary;

namespace CustomEffectPipeline
{
    [ContentProcessor(DisplayName = "Effect Default Processor - TrashSoup")]
    public class DefaultEffectProcessor : EffectProcessor
    {
        public override CompiledEffectContent Process(EffectContent input, ContentProcessorContext context)
        {
            CompiledEffectContent compiled = base.Process(input, context);

            return compiled;
        }
    }
}
