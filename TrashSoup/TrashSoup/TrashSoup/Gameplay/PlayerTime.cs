using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrashSoup.Engine;

namespace TrashSoup.Gameplay
{
    public class PlayerTime
    {
        #region variables
        private int initHours;
        private int initMinutes;
        private int hours;
        private int minutes;
        private bool isTime;
        #endregion

        #region properties
        public int Hours
        {
            get { return hours; }
            set { hours = value; }
        }

        public int Minutes
        {
            get { return minutes; }
            set { minutes = value; }
        }
        #endregion

        #region methods
        public PlayerTime()
        {
            this.initHours = 12;
            this.initMinutes = 0;
            Hours = initHours;
            Minutes = initMinutes;
        }

        public PlayerTime(int initHours, int initMinutes)
        {
            this.initHours = initHours;
            this.initMinutes = initMinutes;
            Hours = initHours;
            Minutes = initMinutes;
        }

        public void Start(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Minutes = (initMinutes + gameTime.TotalGameTime.Seconds) % 60;
            Hours = initHours;
            if (Minutes == 0)
            {
                if(isTime)
                {
                    initHours += 1;
                    isTime = false;
                }
            }
            else
                isTime = true;
        }

        public void Stop()
        {
            Minutes = 0;
            Hours = 0;
        }

        public void SetTime(int hours, int minutes)
        {
            this.initHours = hours;
            this.initMinutes = minutes;
        }

        public void Draw()
        {
            GUIManager.Instance.DrawText(TrashSoupGame.Instance.Content.Load<SpriteFont>("Fonts/FontTest"), "TIME: " + Hours.ToString("00") + ":" + Minutes.ToString("00"), new Vector2(0.1f, 0.4f), Color.Red);
        }
        #endregion
    }
}
