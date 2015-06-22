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
    /// Interaction logic for RatTurningTrigger.xaml
    /// </summary>
    public partial class RatTurningTrigger : UserControl
    {
        private TrashSoup.Gameplay.RatTurningTrigger myObject;

        public string MyRatID
        {
            get
            {
                return this.myObject.MyRatID.ToString();
            }
            set
            {
                uint newValue = this.myObject.MyRatID;
                if(uint.TryParse(value, out newValue))
                {
                    this.myObject.MyRatID = newValue;
                }
            }
        }

        public RatTurningTrigger(TrashSoup.Gameplay.RatTurningTrigger myObject)
        {
            this.myObject = myObject;
            InitializeComponent();
        }
    }
}
