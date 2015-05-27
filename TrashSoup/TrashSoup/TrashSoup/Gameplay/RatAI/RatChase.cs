using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;
using TrashSoup.Engine.AI.BehaviorTree;

namespace TrashSoup.Gameplay.RatAI
{
    class RatChase : TrashSoup.Engine.AI.BehaviorTree.Action
    {
        private Vector3 targetPos;
        private Vector3 myPos;
        private float chaseSpeed = 8.0f;
        private Vector3 chaseVector = Vector3.Zero;

        public override TickStatus Tick(Microsoft.Xna.Framework.GameTime gameTime, out INode node)
        {
            if(!this.blackboard.GetBool("TargetSeen"))
            {
                node = null;
                return TickStatus.FAILURE;
            }

            this.targetPos = this.blackboard.GetVector3("TargetPosition");
            this.myPos = this.blackboard.Owner.MyTransform.Position;
            float distance = Vector3.Distance(this.myPos, this.targetPos);

            if(distance < 4.5f)
            {
                node = null;
                return TickStatus.SUCCESS;
            }

            //Trzeba obracac w strone gracza

            this.chaseVector = this.targetPos - this.myPos;
            this.chaseVector.Y = 0.0f;
            this.chaseVector.Normalize();

            this.blackboard.Owner.MyTransform.Position += this.chaseVector * gameTime.ElapsedGameTime.Milliseconds * 0.001f * chaseSpeed;
            node = this;
            return TickStatus.RUNNING;
        }
    }
}
