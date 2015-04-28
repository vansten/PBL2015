using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaction logic for CustomModel.xaml
    /// </summary>
    public partial class CustomModel : UserControl, INotifyPropertyChanged
    {
        private TrashSoup.Engine.CustomModel model;

        public Visibility LOD1Visibility
        {
            get
            {
                if(this.model.LODs[0] != null)
                {
                    return System.Windows.Visibility.Visible;
                }
                return System.Windows.Visibility.Hidden;
            }
        }

        public Visibility LOD2Visibility
        {
            get
            {
                if (this.model.LODs[1] != null)
                {
                    return System.Windows.Visibility.Visible;
                }
                return System.Windows.Visibility.Hidden;
            }
        }

        public string LOD0Name
        {
            get
            {
                if(this.model.Paths.Count == 0)
                {
                    return "None";
                }

                string tmp = this.model.Paths[0];
                if(tmp == null || tmp == "")
                {
                    tmp = "None";
                }
                else
                {
                    int i = tmp.Length - 1;
                    while(tmp[i] != '/' && i > 0)
                    {
                        --i;
                    }
                    tmp = tmp.Substring(i + 1);
                }
                return tmp;
            }
        }

        public string LOD1Name
        {
            get
            {
                if(this.model.Paths.Count > 1)
                {
                    string tmp = this.model.Paths[1];
                    if (tmp == null || tmp == "")
                    {
                        tmp = "None";
                    }
                    else
                    {
                        int i = tmp.Length - 1;
                        while (tmp[i] != '/' && i > 0)
                        {
                            --i;
                        }
                        tmp = tmp.Substring(i + 1);
                    }
                    return tmp;
                }
                return "None";
            }
        }

        public string LOD2Name
        {
            get
            {

                if (this.model.Paths.Count > 2)
                {
                    string tmp = this.model.Paths[2];
                    if (tmp == null || tmp == "")
                    {
                        tmp = "None";
                    }
                    else
                    {
                        int i = tmp.Length - 1;
                        while (tmp[i] != '/' && i > 0)
                        {
                            --i;
                        }
                        tmp = tmp.Substring(i + 1);
                    }
                    return tmp;
                }
                return "None";
            }
        }

        public CustomModel(TrashSoup.Engine.CustomModel m)
        {
            this.model = m;
            InitializeComponent();
        }

        private void LOD0Open_Click(object sender, RoutedEventArgs e)
        {
            string path = this.OpenFileDialogForFBXFile();
            if(path == null || path == "")
            {
                return;
            }
            TrashSoup.Engine.ResourceManager.Instance.AddModel(path);
            this.model.LODs[0] = TrashSoup.Engine.ResourceManager.Instance.Models[path];
            if(this.model.Paths.Count > 0)
            {
                this.model.Paths[0] = path;
            }
            else
            {
                this.model.Paths.Add(path);
            }
            OnPropertyChanged("LOD0Name");
            OnPropertyChanged("LOD1Visibility");
        }

        private void LOD1Open_Click(object sender, RoutedEventArgs e)
        {
            string path = this.OpenFileDialogForFBXFile();
            if (path == null || path == "")
            {
                return;
            }
            TrashSoup.Engine.ResourceManager.Instance.AddModel(path);
            this.model.LODs[1] = TrashSoup.Engine.ResourceManager.Instance.Models[path];
            if (this.model.Paths.Count > 1)
            {
                this.model.Paths[1] = path;
            }
            else
            {
                this.model.Paths.Add(path);
            }
            OnPropertyChanged("LOD1Name");
            OnPropertyChanged("LOD2Visibility");
        }

        private void LOD2Open_Click(object sender, RoutedEventArgs e)
        {
            string path = this.OpenFileDialogForFBXFile();
            if (path == null || path == "")
            {
                return;
            }
            TrashSoup.Engine.ResourceManager.Instance.AddModel(path);
            this.model.LODs[2] = TrashSoup.Engine.ResourceManager.Instance.Models[path];
            if (this.model.Paths.Count > 2)
            {
                this.model.Paths[2] = path;
            }
            else
            {
                this.model.Paths.Add(path);
            }
            OnPropertyChanged("LOD2Name");
        }

        private string OpenFileDialogForFBXFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "FBX files (*fbx) | *fbx";
            if (ofd.ShowDialog() == true)
            {
                string path = ofd.FileName;

                return this.GetRelativePathToContentDirectory(path);
            }

            return null;
        }

        private string GetRelativePathToContentDirectory(string path)
        {
            int i = 6;
            if (i > path.Length)
            {
                return "";
            }
            bool found = false;
            string TrashSoupContentDir = "TrashSoupContent";
            while (!found && i < path.Length)
            {
                if (this.CompareStrings(TrashSoupContentDir, path.Substring(i, TrashSoupContentDir.Length)))
                {
                    found = true;
                }
                ++i;
            }
            if (!found)
            {
                MessageBoxResult mbr = MessageBox.Show("Given path doesn't contains XNA content folder. Please, check path and give path with \"Content\" in some folder name.", "ERROR", MessageBoxButton.OK);

                return "";
            }
            if(path[i] != 'M')
            {
                while (path[i] != '\\')
                {
                    ++i;
                }
            }
            path = path.Substring(i + 1);
            i = 0;
            string tmp = "";
            while (i < path.Length)
            {
                if (path[i] == '\\')
                {
                    tmp += '/';
                }
                else
                {
                    tmp += path[i];
                }
                ++i;
            }
            path = tmp;
            path = path.Substring(0, path.Length - 4);

            return path;
        }

        private bool CompareStrings(string str1, string str2)
        {
            if (str1.Length != str2.Length) return false;
            for(int i = 0; i < str1.Length; ++i)
            {
                if (str1[i] != str2[i]) return false;
            }

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null && name != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
