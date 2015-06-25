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
        
        public string CustomScaleX
        {
            get
            {
                return myCollider.CustomScale.X.ToString();
            }
            set
            {
                float newX = myCollider.CustomScale.X;
                if (!float.TryParse(value, out newX))
                {
                    TrashSoup.Engine.Debug.Log("Wrong format of string!");
                }
                myCollider.CustomScale = new Microsoft.Xna.Framework.Vector3(newX, myCollider.CustomScale.Y, myCollider.CustomScale.Z);
                myCollider.UpdateCollider();
                OnPropertyChanged();
            }
        }

        public string CustomScaleY
        {
            get
            {
                return myCollider.CustomScale.Y.ToString();
            }
            set
            {
                float newY = myCollider.CustomScale.Y;
                if (!float.TryParse(value, out newY))
                {
                    TrashSoup.Engine.Debug.Log("Wrong format of string!");
                }
                myCollider.CustomScale = new Microsoft.Xna.Framework.Vector3(myCollider.CustomScale.X, newY, myCollider.CustomScale.Z);
                myCollider.UpdateCollider();
                OnPropertyChanged();
            }
        }

        public string CustomScaleZ
        {
            get
            {
                return myCollider.CustomScale.Z.ToString();
            }
            set
            {
                float newZ = myCollider.CustomScale.Z;
                if (!float.TryParse(value, out newZ))
                {
                    TrashSoup.Engine.Debug.Log("Wrong format of string!");
                }
                myCollider.CustomScale = new Microsoft.Xna.Framework.Vector3(myCollider.CustomScale.X, myCollider.CustomScale.Y, newZ);
                myCollider.UpdateCollider();
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
