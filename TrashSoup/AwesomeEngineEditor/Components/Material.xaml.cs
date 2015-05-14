using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class Material : UserControl
    {
        TrashSoup.Engine.Material material;
        List<TrashSoup.Engine.Material> materialsCollection;
        MainWindow mw;

        public Material(TrashSoup.Engine.Material m, List<TrashSoup.Engine.Material> materials, MainWindow mw)
        {
            this.material = m;
            this.materialsCollection = materials;
            InitializeComponent();
            if(this.material != null)
            {
                this.MaterialName.Text = this.material.Name;
                this.AddMaterial.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                this.MaterialName.Text = "None";
                this.RemoveMaterial.Visibility = System.Windows.Visibility.Hidden;
            }
            this.mw = mw;
        }

        private void AddMaterial_Click(object sender, RoutedEventArgs e)
        {
            //Load material from disk
            //Load metarial in game
            //Add material to list
            OpenFileDialog ofd = new OpenFileDialog(); string initialDirectory = System.IO.Directory.GetCurrentDirectory();
            for (int i = 0; i < 3; ++i)
            {
                System.IO.DirectoryInfo di = System.IO.Directory.GetParent(initialDirectory);
                initialDirectory = di.FullName;
            }
            initialDirectory += "\\TrashSoup\\TrashSoupContent\\Materials";
            ofd.InitialDirectory = initialDirectory;
            ofd.Filter = "XML files (*xml) | *.xml";
            string path = "";
            if (ofd.ShowDialog() == true)
            {
               path = ofd.FileName;
            }
            else
            {
                return;
            }
            TrashSoup.Engine.Material m = TrashSoup.Engine.ResourceManager.Instance.LoadMaterial(path);
            this.materialsCollection.Add(m);
            this.mw.GenerateDetailsText();
        }

        private void RemoveMaterial_Click(object sender, RoutedEventArgs e)
        {
            this.materialsCollection.Remove(this.material);
            this.mw.GenerateDetailsText();
        }
    }
}
