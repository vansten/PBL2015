using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using TrashSoup.Engine;
using TrashSoup.Engine.AI.BehaviorTree;

namespace TrashSoup.Gameplay
{
    class Rat : ObjectComponent
    {
        private BehaviorTree myBehavior;
        public Blackboard MyBlackBoard;
        private Enemy myEnemyScript;

        public Rat(GameObject go) : base(go)
        {
            Start();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if(TrashSoupGame.Instance.EditorMode)
            {
                return;
            }
        }

        public override void Draw(Camera cam, Microsoft.Xna.Framework.Graphics.Effect effect, Microsoft.Xna.Framework.GameTime gameTime)
        {

        }

        protected override void Start()
        {

        }

        public override void Initialize()
        {
            this.MyObject.MyAnimator.AvailableStates.Add("Walk", new AnimatorState("Walk", this.MyObject.MyAnimator.GetAnimationPlayer("Animations/Enemies/Rat_walk")));
            this.MyObject.MyAnimator.AvailableStates.Add("Run", new AnimatorState("Run", this.MyObject.MyAnimator.GetAnimationPlayer("Animations/Enemies/Rat_run")));
            this.MyObject.MyAnimator.AvailableStates.Add("Attack", new AnimatorState("Attack", this.MyObject.MyAnimator.GetAnimationPlayer("Animations/Enemies/Rat_attack"), AnimatorState.StateType.SINGLE));
            this.MyObject.MyAnimator.AvailableStates.Add("Die", new AnimatorState("Die", this.MyObject.MyAnimator.GetAnimationPlayer("Animations/Enemies/Rat_dying")));
            this.MyObject.MyAnimator.AvailableStates.Add("Idle", new AnimatorState("Idle", this.MyObject.MyAnimator.GetAnimationPlayer("Animations/Enemies/Rat_idle")));
            string[] states = new string[] { "Idle", "Walk", "Run", "Die", "Attack" };
            for (int i = 0; i < states.Length; ++i)
            {
                for (int j = 0; j < states.Length; ++j)
                {
                    if (j != i)
                    {
                        this.MyObject.MyAnimator.AvailableStates[states[i]].AddTransition(this.MyObject.MyAnimator.AvailableStates[states[j]], new TimeSpan(0, 0, 0, 0, 200));
                    }
                }
            }
            MyObject.MyAnimator.CurrentState = MyObject.MyAnimator.AvailableStates["Idle"];
            
            XmlSerializer serializer = new XmlSerializer(typeof(BehaviorTree));
            string path = "";
            if (TrashSoupGame.Instance != null && TrashSoupGame.Instance.EditorMode)
            {
                path += "../";
            }
            path += "../../../../TrashSoupContent/BehaviorTrees/RatAI.xml";
            try
            {
                using (FileStream file = new FileStream(Path.GetFullPath(path), FileMode.Open))
                {
                    myBehavior = (BehaviorTree)serializer.Deserialize(file);
                    MyBlackBoard = myBehavior.Blackboard;
                }
                myBehavior.Run();
            }
            catch
            {

            }
            this.myEnemyScript = (Enemy)this.MyObject.GetComponent<Enemy>();
            this.myEnemyScript.OnDead += this.OnDead;
            base.Initialize();
        }

        void OnDead()
        {
            this.myBehavior.Stop();
        }

        public override void OnCollision(GameObject other)
        {
            base.OnCollision(other);
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();

            base.ReadXml(reader);

            reader.ReadEndElement();
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            base.WriteXml(writer);
        }
    }
}
