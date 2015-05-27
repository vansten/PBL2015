using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;
using TrashSoup.Engine.AI.BehaviorTree;

namespace TrashSoup.Gameplay.RatAI
{
    class RatStay : TrashSoup.Engine.AI.BehaviorTree.Action
    {
        private float timer = 0.0f;
        private float idleTime = 3.0f;

        public override Engine.AI.BehaviorTree.TickStatus Tick(Microsoft.Xna.Framework.GameTime gameTime, out Engine.AI.BehaviorTree.INode node)
        {
            if(this.blackboard.GetBool("TargetSeen"))
            {
                node = null;
                return TickStatus.FAILURE;
            }
            if(timer > idleTime)
            {
                this.blackboard.SetBool("Idle", false);
                timer = 0.0f;
                node = null;
                return TickStatus.SUCCESS;
            }

            node = this;
            timer += gameTime.ElapsedGameTime.Milliseconds * 0.001f;
            return TickStatus.RUNNING;
        }
    }
}
