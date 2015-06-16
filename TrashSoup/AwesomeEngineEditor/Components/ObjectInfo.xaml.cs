using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ObjectInfo.xaml
    /// </summary>
    public partial class ObjectInfo : UserControl, INotifyPropertyChanged
    {
        private TrashSoup.Engine.GameObject gameObject;
        private MainWindow mw;

        public string ObjectName
        {
            get
            {
                return this.gameObject.Name;
            }
            set
            {
                this.gameObject.Name = value;
                OnPropertyChanged();
            }
        }

        public string ObjectID
        {
            get
            {
                return this.gameObject.UniqueID.ToString();
            }
        }

        public ObjectInfo(TrashSoup.Engine.GameObject go, MainWindow mw)
        {
            this.gameObject = go;
            this.mw = mw;
            InitializeComponent();
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
