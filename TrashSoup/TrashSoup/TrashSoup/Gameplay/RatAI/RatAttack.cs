using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;
using TrashSoup.Engine.AI.BehaviorTree;
using Microsoft.Xna.Framework;

namespace TrashSoup.Gameplay.RatAI
{
    class RatAttack : TrashSoup.Engine.AI.BehaviorTree.Action
    {
        private Vector3 targetPos;
        private Vector3 myPos;
        private float attackCooldown = 2.0f;
        private float timer = 0.0f;

        public override TickStatus Tick(GameTime gameTime, out INode node)
        {
            if(!this.blackboard.GetBool("TargetSeen"))
            {
                node = null;
                return TickStatus.FAILURE;
            }

            this.targetPos = this.blackboard.GetVector3("TargetPosition");
            this.myPos = this.blackboard.Owner.MyTransform.Position;
            float distance = Vector3.Distance(this.targetPos, this.myPos);

            if(distance > 5.0f)
            {
                node = null;
                return TickStatus.FAILURE;
            }

            //Powinien sie obracac w dobra strone (w strone gracza)

            if(timer > attackCooldown)
            {
                Debug.Log("Attacking");
                timer = 0.0f;
            }

            timer += gameTime.ElapsedGameTime.Milliseconds * 0.001f;

            node = this;
            return TickStatus.RUNNING;
        }
    }
}
