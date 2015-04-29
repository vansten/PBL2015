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
    /// Interaction logic for AmbientLight.xaml
    /// </summary>
    public partial class AmbientLight : UserControl, INotifyPropertyChanged
    {
        private TrashSoup.Engine.LightAmbient ambientLight;

        public string LightColorR
        {
            get
            {
                return this.ambientLight.LightColor.X.ToString();
            }
            set
            {
                float newR = this.ambientLight.LightColor.X;
                if(float.TryParse(value, out newR))
                {
                    this.ambientLight.LightColor = new Microsoft.Xna.Framework.Vector3(newR, this.ambientLight.LightColor.Y, this.ambientLight.LightColor.Z);
                }
                OnPropertyChanged();
            }
        }

        public string LightColorG
        {
            get
            {
                return this.ambientLight.LightColor.Y.ToString();
            }
            set
            {
                float newG = this.ambientLight.LightColor.Y;
                if (float.TryParse(value, out newG))
                {
                    this.ambientLight.LightColor = new Microsoft.Xna.Framework.Vector3(this.ambientLight.LightColor.X, newG, this.ambientLight.LightColor.Z);
                }
                OnPropertyChanged();
            }
        }

        public string LightColorB
        {
            get
            {
                return this.ambientLight.LightColor.Z.ToString();
            }
            set
            {
                float newB = this.ambientLight.LightColor.Z;
                if (float.TryParse(value, out newB))
                {
                    this.ambientLight.LightColor = new Microsoft.Xna.Framework.Vector3(this.ambientLight.LightColor.X, this.ambientLight.LightColor.Y, newB);
                }
                OnPropertyChanged();
            }
        }

        public AmbientLight(TrashSoup.Engine.LightAmbient al)
        {
            this.ambientLight = al;
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
