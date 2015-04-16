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
    /// Interaction logic for PhysicalObject.xaml
    /// </summary>
    public partial class PhysicalObject : UserControl, INotifyPropertyChanged
    {

        private TrashSoup.Engine.PhysicalObject physicalObject;

        public string ObjectMass
        {
            get
            {
                return this.physicalObject.Mass.ToString();
            }
            set
            {
                this.physicalObject.Mass = (float)Convert.ToDouble(value);
                OnPropertyChanged();
            }
        }

        public string ObjectDragFactor
        {
            get
            {
                return this.physicalObject.DragFactor.ToString();
            }
            set
            {
                this.physicalObject.DragFactor = (float)Convert.ToDouble(value);
                OnPropertyChanged();
            }
        }

        public bool ObjectUsingGravity
        {
            get
            {
                return this.physicalObject.IsUsingGravity;
            }
            set
            {
                this.physicalObject.IsUsingGravity = value;
                OnPropertyChanged();
            }
        }

        public PhysicalObject(TrashSoup.Engine.PhysicalObject po)
        {
            this.physicalObject = po;
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
