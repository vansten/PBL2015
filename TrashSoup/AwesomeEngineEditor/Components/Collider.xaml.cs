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
    /// Interaction logic for Collider.xaml
    /// </summary>
    public partial class Collider : UserControl, INotifyPropertyChanged
    {
        private TrashSoup.Engine.Collider myCollider;

        public bool IsTrigger
        {
            get
            {
                return this.myCollider.IsTrigger;
            }
            set
            {
                this.myCollider.IsTrigger = value;
                OnPropertyChanged();
            }
        }

        public Collider(TrashSoup.Engine.Collider col)
        {
            InitializeComponent();
            this.myCollider = col;
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
