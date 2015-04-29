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
    /// Interaction logic for DirectionalLight.xaml
    /// </summary>
    public partial class DirectionalLight : UserControl, INotifyPropertyChanged
    {
        private TrashSoup.Engine.LightDirectional directionalLight;

        public string LightColorR
        {
            get
            {
                return this.directionalLight.LightColor.X.ToString();
            }
            set
            {
                float newR = this.directionalLight.LightColor.X;
                if (float.TryParse(value, out newR))
                {
                    this.directionalLight.LightColor = new Microsoft.Xna.Framework.Vector3(newR, this.directionalLight.LightColor.Y, this.directionalLight.LightColor.Z);
                }
                OnPropertyChanged();
            }
        }

        public string LightColorG
        {
            get
            {
                return this.directionalLight.LightColor.Y.ToString();
            }
            set
            {
                float newG = this.directionalLight.LightColor.Y;
                if (float.TryParse(value, out newG))
                {
                    this.directionalLight.LightColor = new Microsoft.Xna.Framework.Vector3(this.directionalLight.LightColor.X, newG, this.directionalLight.LightColor.Z);
                }
                OnPropertyChanged();
            }
        }

        public string LightColorB
        {
            get
            {
                return this.directionalLight.LightColor.Z.ToString();
            }
            set
            {
                float newB = this.directionalLight.LightColor.Z;
                if (float.TryParse(value, out newB))
                {
                    this.directionalLight.LightColor = new Microsoft.Xna.Framework.Vector3(this.directionalLight.LightColor.X, this.directionalLight.LightColor.Y, newB);
                }
                OnPropertyChanged();
            }
        }

        public string LightSpecularColorR
        {
            get
            {
                return this.directionalLight.LightSpecularColor.X.ToString();
            }
            set
            {
                float newR = this.directionalLight.LightSpecularColor.X;
                if (float.TryParse(value, out newR))
                {
                    this.directionalLight.LightSpecularColor = new Microsoft.Xna.Framework.Vector3(newR, this.directionalLight.LightSpecularColor.Y, this.directionalLight.LightSpecularColor.Z);
                }
                OnPropertyChanged();
            }
        }

        public string LightSpecularColorG
        {
            get
            {
                return this.directionalLight.LightSpecularColor.Y.ToString();
            }
            set
            {
                float newG = this.directionalLight.LightSpecularColor.Y;
                if (float.TryParse(value, out newG))
                {
                    this.directionalLight.LightSpecularColor = new Microsoft.Xna.Framework.Vector3(this.directionalLight.LightSpecularColor.X, newG, this.directionalLight.LightSpecularColor.Z);
                }
                OnPropertyChanged();
            }
        }

        public string LightSpecularColorB
        {
            get
            {
                return this.directionalLight.LightSpecularColor.Z.ToString();
            }
            set
            {
                float newB = this.directionalLight.LightSpecularColor.Z;
                if (float.TryParse(value, out newB))
                {
                    this.directionalLight.LightSpecularColor = new Microsoft.Xna.Framework.Vector3(this.directionalLight.LightSpecularColor.X, this.directionalLight.LightSpecularColor.Y, newB);
                }
                OnPropertyChanged();
            }
        }

        public bool CastShadows
        {
            get
            {
                return this.directionalLight.CastShadows;
            }
            set
            {
                this.directionalLight.CastShadows = value;
                OnPropertyChanged();
            }
        }

        public string LightDirectionX
        {
            get
            {
                return this.directionalLight.LightDirection.X.ToString();
            }
            set
            {
                float newX = this.directionalLight.LightDirection.X;
                if(float.TryParse(value, out newX))
                {
                    this.directionalLight.LightDirection = new Microsoft.Xna.Framework.Vector3(newX, this.directionalLight.LightDirection.Y, this.directionalLight.LightDirection.Z);
                }
                OnPropertyChanged();
            }
        }

        public string LightDirectionY
        {
            get
            {
                return this.directionalLight.LightDirection.Y.ToString();
            }
            set
            {
                float newY = this.directionalLight.LightDirection.Y;
                if (float.TryParse(value, out newY))
                {
                    this.directionalLight.LightDirection = new Microsoft.Xna.Framework.Vector3(this.directionalLight.LightDirection.X, newY, this.directionalLight.LightDirection.Z);
                }
                OnPropertyChanged();
            }
        }

        public string LightDirectionZ
        {
            get
            {
                return this.directionalLight.LightDirection.Z.ToString();
            }
            set
            {
                float newZ = this.directionalLight.LightDirection.Z;
                if (float.TryParse(value, out newZ))
                {
                    this.directionalLight.LightDirection = new Microsoft.Xna.Framework.Vector3(this.directionalLight.LightDirection.X, this.directionalLight.LightDirection.Y, newZ);
                }
                OnPropertyChanged();
            }
        }

        public DirectionalLight(TrashSoup.Engine.LightDirectional dl)
        {
            this.directionalLight = dl;
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
