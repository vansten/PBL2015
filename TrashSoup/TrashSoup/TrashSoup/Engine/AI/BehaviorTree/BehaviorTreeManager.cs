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
                        bt.Tick(gameTime);
                        this.milisecondsTillLastTick = 0;
                    }
                    else
                    {
                        if (bt.CurrentRunning != null)
                        {
                            Debug.Log("Current Running: " + bt.CurrentRunning.GetType().Name);
                            TickStatus ts = bt.CurrentRunning.Tick(gameTime, out bt.CurrentRunning);
                            if(ts != TickStatus.RUNNING)
                            {
                                this.milisecondsTillLastTick = this.btTickFrequency;
                            }
                        }
                        else
                        {
                            Debug.Log("Current Running: Nothing");
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

        public void Reload()
        {
            this.behaviorTrees.Clear();
        }
    }
}
