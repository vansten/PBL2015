using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AwesomeEngineEditor.Components
{
    /// <summary>
    /// Interaction logic for Enemy.xaml
    /// </summary>
    public partial class Enemy : UserControl
    {
        private TrashSoup.Gameplay.Enemy myEnemy;

        public string MyHPBarID
        {
            get
            {
                return this.myEnemy.MyHPBarID.ToString();
            }
            set
            {
                uint newValue = this.myEnemy.MyHPBarID;
                if(uint.TryParse(value, out newValue))
                {
                    this.myEnemy.MyHPBarID = newValue;
                }
                else
                {
                    TrashSoup.Engine.Debug.Log("Something failed");
                }
            }
        }

        public Enemy(TrashSoup.Gameplay.Enemy e)
        {
            this.myEnemy = e;
            InitializeComponent();
        }
    }
}
