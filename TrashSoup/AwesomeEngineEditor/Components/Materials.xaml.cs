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
    /// Interaction logic for Material.xaml
    /// </summary>
    public partial class Materials : UserControl
    {
        public Materials(List<TrashSoup.Engine.Material> materials, MainWindow mw)
        {
            InitializeComponent();
            List<UserControl> materialsComponents = new List<UserControl>();
            foreach(TrashSoup.Engine.Material m in materials)
            {
                materialsComponents.Add(new Material(m, materials, mw));
            }
            materialsComponents.Add(new Material(null, materials, mw));
            this.MaterialsList.ItemsSource = materialsComponents;
        }
    }
}
