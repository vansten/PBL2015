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
        private float damage = 5.0f;
        private PlayerController target;
        private Vector3 difference;
        private Vector3 forward;

        public override void Initialize()
        {
            GameObject go = ResourceManager.Instance.CurrentScene.GetObject(1);
            if(go != null)
            {
                target = (PlayerController)go.GetComponent<PlayerController>();
                Debug.Log("Target: " + (target != null).ToString());
            }
            base.Initialize();
        } 

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
                this.blackboard.SetBool("TargetSeen", false);
                node = null;
                return TickStatus.FAILURE;
            }

            this.difference = this.targetPos - this.myPos;
            this.difference.Y = 0.0f;
            this.difference.Normalize();
            this.forward = Vector3.Transform(Vector3.Right, Matrix.CreateRotationY(-this.blackboard.Owner.MyTransform.Rotation.Y));
            this.forward.Y = 0.0f;
            this.forward.Normalize();
            float angle = (float)Math.Atan2(-(this.difference.Z - this.forward.Z), -(this.difference.X - this.forward.X));
            float sign = Math.Sign(angle);

            while (Vector3.Dot(this.forward, this.difference) < 0.99f)
            {
                this.blackboard.Owner.MyTransform.Rotation += sign * Vector3.Up * 0.01f;
                this.forward = Vector3.Transform(Vector3.Right, Matrix.CreateRotationY(-this.blackboard.Owner.MyTransform.Rotation.Y));
                this.forward.Y = 0.0f;
                this.forward.Normalize();
            }

            if(timer > attackCooldown)
            {
                Debug.Log("Attacking");
                if(target != null)
                {
                    target.DecreaseHealth(this.damage);
                }
                timer = 0.0f;
            }

            timer += gameTime.ElapsedGameTime.Milliseconds * 0.001f;

            node = this;
            return TickStatus.RUNNING;
        }
    }
}
