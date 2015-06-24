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
    /// Interaction logic for MutantTrigger.xaml
    /// </summary>
    public partial class MutantTrigger : UserControl
    {
        private TrashSoup.Gameplay.MutantTrigger myObject;

        public string MyMutantID
        {
            get
            {
                return this.myObject.MyMutantID.ToString();
            }
            set
            {
                uint newValue = this.myObject.MyMutantID;
                if (uint.TryParse(value, out newValue))
                {
                    this.myObject.MyMutantID = newValue;
                }
            }
        }
        public MutantTrigger(TrashSoup.Gameplay.MutantTrigger mt)
        {
            this.myObject = mt;
            InitializeComponent();
        }
    }
}
