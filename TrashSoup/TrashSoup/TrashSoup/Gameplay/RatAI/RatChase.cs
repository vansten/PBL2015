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
        private Vector3 forward = Vector3.Forward;

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
            
            this.chaseVector = this.targetPos - this.myPos;
            this.chaseVector.Y = 0.0f;
            this.chaseVector.Normalize();
            this.forward = Vector3.Transform(Vector3.Right, Matrix.CreateRotationY(-this.blackboard.Owner.MyTransform.Rotation.Y));
            this.forward.Y = 0.0f;
            this.forward.Normalize();
            float angle = (float)Math.Atan2(-(this.chaseVector.Z - this.forward.Z), -(this.chaseVector.X - this.forward.X));
            float sign = Math.Sign(angle);

            while (Vector3.Dot(this.forward, this.chaseVector) < 0.99f)
            {
                this.blackboard.Owner.MyTransform.Rotation += sign * Vector3.Up * 0.01f;
                this.forward = Vector3.Transform(Vector3.Right, Matrix.CreateRotationY(-this.blackboard.Owner.MyTransform.Rotation.Y));
                this.forward.Y = 0.0f;
                this.forward.Normalize();
            }

            this.blackboard.Owner.MyTransform.Position += this.chaseVector * gameTime.ElapsedGameTime.Milliseconds * 0.001f * chaseSpeed;
            node = this;
            return TickStatus.RUNNING;
        }
    }
}
