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
    /// Interaction logic for DisappearingTrash.xaml
    /// </summary>
    public partial class DisappearingTrash : UserControl
    {
        private TrashSoup.Gameplay.DisappearingTrash myTrash;

        public string TrashCount
        {
            get
            {
                return myTrash.TrashCount.ToString();
            }
            set
            {
                int newValue = this.myTrash.TrashCount;
                if(int.TryParse(value, out newValue))
                {
                    this.myTrash.TrashCount = newValue;
                }
            }
        }

        public DisappearingTrash(TrashSoup.Gameplay.DisappearingTrash mt)
        {
            this.myTrash = mt;
            InitializeComponent();
        }
    }
}
