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
    /// Interaction logic for Transform.xaml
    /// </summary>
    public partial class Transform : UserControl, INotifyPropertyChanged
    {
        private TrashSoup.Engine.Transform transform;

        public string XPosition
        {
            get
            {
                return this.transform.Position.X.ToString();
            }
            set
            {
                this.transform.Position = new Microsoft.Xna.Framework.Vector3(Convert.ToInt32(value), this.transform.Position.Y, this.transform.Position.Z);
                OnPropertyChanged();
            }
        }

        public string YPosition
        {
            get
            {
                return this.transform.Position.Y.ToString();
            }
            set
            {
                this.transform.Position = new Microsoft.Xna.Framework.Vector3(this.transform.Position.X, Convert.ToInt32(value), this.transform.Position.Z);
                OnPropertyChanged();
            }
        }

        public string ZPosition
        {
            get
            {
                return this.transform.Position.Z.ToString();
            }
            set
            {
                this.transform.Position = new Microsoft.Xna.Framework.Vector3(this.transform.Position.X, this.transform.Position.Y, Convert.ToInt32(value));
                OnPropertyChanged();
            }
        }

        public string XRotation
        {
            get
            {
                return this.transform.Rotation.X.ToString();
            }
            set
            {
                this.transform.Rotation = new Microsoft.Xna.Framework.Vector3(Convert.ToInt32(value), this.transform.Rotation.Y, this.transform.Rotation.Z);
                OnPropertyChanged();
            }
        }

        public string YRotation
        {
            get
            {
                return this.transform.Rotation.Y.ToString();
            }
            set
            {
                this.transform.Rotation = new Microsoft.Xna.Framework.Vector3(this.transform.Rotation.X, Convert.ToInt32(value), this.transform.Rotation.Z);
                OnPropertyChanged();
            }
        }

        public string ZRotation
        {
            get
            {
                return this.transform.Rotation.Z.ToString();
            }
            set
            {
                this.transform.Rotation = new Microsoft.Xna.Framework.Vector3(this.transform.Rotation.X, this.transform.Rotation.Y, Convert.ToInt32(value));
                OnPropertyChanged();
            }
        }

        public string Scale
        {
            get
            {
                return this.transform.Scale.ToString();
            }
            set
            {
                this.transform.Scale = (float)Convert.ToDouble(value);
                OnPropertyChanged();
            }
        }

        public Transform(TrashSoup.Engine.Transform comp)
        {
            InitializeComponent();
            this.transform = comp;
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
