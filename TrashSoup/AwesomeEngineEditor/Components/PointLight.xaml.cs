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
    /// Interaction logic for PointLight.xaml
    /// </summary>
    public partial class PointLight : UserControl, INotifyPropertyChanged
    {
        private TrashSoup.Engine.LightPoint pointLight;

        public string LightColorR
        {
            get
            {
                return this.pointLight.LightColor.X.ToString();
            }
            set
            {
                float newR = this.pointLight.LightColor.X;
                if(float.TryParse(value, out newR))
                {
                    this.pointLight.LightColor = new Microsoft.Xna.Framework.Vector3(newR, this.pointLight.LightColor.Y, this.pointLight.LightColor.Z);
                }
                OnPropertyChanged();
            }
        }

        public string LightColorG
        {
            get
            {
                return this.pointLight.LightColor.Y.ToString();
            }
            set
            {
                float newG = this.pointLight.LightColor.Y;
                if (float.TryParse(value, out newG))
                {
                    this.pointLight.LightColor = new Microsoft.Xna.Framework.Vector3(this.pointLight.LightColor.X, newG, this.pointLight.LightColor.Z);
                }
                OnPropertyChanged();
            }
        }

        public string LightColorB
        {
            get
            {
                return this.pointLight.LightColor.Z.ToString();
            }
            set
            {
                float newB = this.pointLight.LightColor.Z;
                if (float.TryParse(value, out newB))
                {
                    this.pointLight.LightColor = new Microsoft.Xna.Framework.Vector3(this.pointLight.LightColor.X, this.pointLight.LightColor.Y, newB);
                }
                OnPropertyChanged();
            }
        }

        public string LightSpecularColorR
        {
            get
            {
                return this.pointLight.LightSpecularColor.X.ToString();
            }
            set
            {
                float newR = this.pointLight.LightSpecularColor.X;
                if (float.TryParse(value, out newR))
                {
                    this.pointLight.LightSpecularColor = new Microsoft.Xna.Framework.Vector3(newR, this.pointLight.LightSpecularColor.Y, this.pointLight.LightSpecularColor.Z);
                }
                OnPropertyChanged();
            }
        }

        public string LightSpecularColorG
        {
            get
            {
                return this.pointLight.LightSpecularColor.Y.ToString();
            }
            set
            {
                float newG = this.pointLight.LightSpecularColor.Y;
                if (float.TryParse(value, out newG))
                {
                    this.pointLight.LightSpecularColor = new Microsoft.Xna.Framework.Vector3(this.pointLight.LightSpecularColor.X, newG, this.pointLight.LightSpecularColor.Z);
                }
                OnPropertyChanged();
            }
        }

        public string LightSpecularColorB
        {
            get
            {
                return this.pointLight.LightSpecularColor.Z.ToString();
            }
            set
            {
                float newB = this.pointLight.LightSpecularColor.Z;
                if (float.TryParse(value, out newB))
                {
                    this.pointLight.LightSpecularColor = new Microsoft.Xna.Framework.Vector3(this.pointLight.LightSpecularColor.X, this.pointLight.LightSpecularColor.Y, newB);
                }
                OnPropertyChanged();
            }
        }

        public string Attenuation
        {
            get
            {
                return this.pointLight.Attenuation.ToString();
            }
            set
            {
                float newA = this.pointLight.Attenuation;
                if(float.TryParse(value, out newA))
                {
                    this.pointLight.Attenuation = newA;
                }
                OnPropertyChanged();
            }
        }

        public bool CastShadows
        {
            get
            {
                return this.pointLight.CastShadows;
            }
            set
            {
                this.pointLight.CastShadows = value;
                OnPropertyChanged();
            }
        }

        public PointLight(TrashSoup.Engine.LightPoint pl)
        {
            this.pointLight = pl;
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
