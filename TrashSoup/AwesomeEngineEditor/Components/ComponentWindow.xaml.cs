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
    /// Interaction logic for ComponentWindow.xaml
    /// </summary>
    public partial class ComponentWindow : UserControl
    {

        private TrashSoup.Engine.ObjectComponent component;

        public string ComponentName
        {
            get
            {
                return GenerateComponentName();
            }
        }
        
        public ComponentWindow(TrashSoup.Engine.ObjectComponent objectComponent)
        {
            InitializeComponent();
            this.component = objectComponent;
        }

        private string GenerateComponentName()
        {
            string name = this.component.ToString();
            int i = name.Length - 1;
            while(name[i] != '.')
            {
                --i;
            }
            if(name[i] == '.')
            {
                ++i;
            }

            name = name.Substring(i);

            return name;
        }
    }
}
