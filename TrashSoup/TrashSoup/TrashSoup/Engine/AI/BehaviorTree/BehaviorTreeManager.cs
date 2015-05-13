using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrashSoup.Engine.AI.BehaviorTree
{
    public class BehaviorTreeManager
    {
        private static BehaviorTreeManager instance;

        public static BehaviorTreeManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BehaviorTreeManager();
                }
                return instance;
            }
        }

        private List<BehaviorTree> behaviorTrees = new List<BehaviorTree>();
        private int btTickFrequency = 300;
        private int milisecondsTillLastTick = 0;

        public void AddBehaviorTree(BehaviorTree bt)
        {
            this.behaviorTrees.Add(bt);
        }

        public void Update(GameTime gameTime)
        {
            foreach (BehaviorTree bt in this.behaviorTrees)
            {
                if (bt.Enabled)
                {
                    if (this.milisecondsTillLastTick >= this.btTickFrequency)
                    {
                        Debug.Log("Normal tick of BT");
                        bt.Tick();
                        this.milisecondsTillLastTick = 0;
                    }
                    else
                    {
                        Debug.Log("Wait for next normal tick of BT");
                        if (bt.CurrentRunning != null)
                        {
                            bt.CurrentRunning.Tick(out bt.CurrentRunning);
                        }
                    }
                    this.milisecondsTillLastTick += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }
        }

        /*Set frequency in miliseconds*/
        public void SetFrequency(int value)
        {
            this.btTickFrequency = value;
        }
    }
}
