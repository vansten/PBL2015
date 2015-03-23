using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Reflection;

namespace AwesomeEngineEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public About AboutWindow;
        private TrashSoup.Engine.GameObject selectedObject = null;

        private Visibility isXYZVisible;
        private Visibility isMoreLessVisible;
        private Visibility isTranslateRotateScaleVisible;
        private bool isSaveSceneMIEnabled = false;
        private ObservableCollection<TrashSoup.Engine.ObjectComponent> objectComponents = new ObservableCollection<TrashSoup.Engine.ObjectComponent>();
        private ObservableCollection<TrashSoup.Engine.GameObject> gameObjects = new ObservableCollection<TrashSoup.Engine.GameObject>();
        private TrashSoup.TrashSoupGame myGame;

        public ObservableCollection<TrashSoup.Engine.GameObject> GameObjects
        {
            get
            {
                return this.gameObjects;
            }
            set
            {
                this.gameObjects = value;
                OnPropertyChanged();
            }
        }

        public System.Windows.Visibility IsTranslateRotateScaleVisible
        {
            get
            {
                return this.isTranslateRotateScaleVisible;
            }
            set
            {
                this.isTranslateRotateScaleVisible = value;
                OnPropertyChanged();
            }
        }

        public System.Windows.Visibility IsXYZVisible
        {
            get
            {
                return this.isXYZVisible;
            }
            set
            {
                this.isXYZVisible = value;
                OnPropertyChanged();
            }
        }

        public System.Windows.Visibility IsMoreLessVisible
        {
            get
            {
                return this.isMoreLessVisible;
            }
            set
            {
                this.isMoreLessVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsSaveSceneMIEnabled
        {
            get
            {
                return this.isSaveSceneMIEnabled;
            }
            set
            {
                this.isSaveSceneMIEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsRemoveModelMIEnabled
        {
            get
            {
                return this.selectedObject != null;
            }
        }

        private void FillObjectComponents()
        {
            Type[] types = typeof(TrashSoup.Engine.ObjectComponent).Assembly.GetTypes();
            foreach (Type t in types)
            {
                if (t.IsSubclassOf(typeof(TrashSoup.Engine.ObjectComponent)) && t != typeof(TrashSoup.Engine.Transform))
                {
                    ConstructorInfo[] ci = t.GetConstructors();
                    ConstructorInfo ciWithLeastParameters = ci[0];
                    foreach (ConstructorInfo c in ci)
                    {
                        if (c.GetParameters().Length < ciWithLeastParameters.GetParameters().Length)
                        {
                            ciWithLeastParameters = c;
                        }
                    }
                    List<object> parameters = new List<object>();
                    for (int i = 0; i < ciWithLeastParameters.GetParameters().Length; ++i)
                    {
                        parameters.Add(null);
                    }
                    object obj = Activator.CreateInstance(t, parameters.ToArray());
                    this.objectComponents.Add((TrashSoup.Engine.ObjectComponent)obj);
                }
            }

            TrashSoup.Engine.Debug.Log("Editor completed loading list of object components");
        }

        public MainWindow()
        {
            TrashSoup.Engine.Debug.Log("Editor started");
            InitializeComponent();
            this.DataContext = this;
            this.Closing += MainWindow_Closing;
            this.IsXYZVisible = System.Windows.Visibility.Hidden;
            this.IsMoreLessVisible = System.Windows.Visibility.Hidden;
            this.IsTranslateRotateScaleVisible = System.Windows.Visibility.Hidden;
            this.DetailsInfo.Visibility = System.Windows.Visibility.Hidden;
            this.FillObjectComponents();
            this.ObjectComponents.ItemsSource = this.objectComponents;
            this.myGame = new TrashSoup.TrashSoupGame();
            this.myGame.EditorMode = true;
            this.myGame.RunOneFrame();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.isSaveSceneMIEnabled)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save scene?", "Warning", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    this.SaveScene();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void HierarchyTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.selectedObject = ((TrashSoup.Engine.GameObject)this.HierarchyTreeView.SelectedItem);
            if(this.selectedObject != null)
            {
                OnPropertyChanged("IsRemoveModelMIEnabled");
                this.DetailsInfo.Visibility = System.Windows.Visibility.Visible;
                this.IsTranslateRotateScaleVisible = System.Windows.Visibility.Visible;
                this.GenerateDetailsText();
            }
            else
            {
                this.DetailsInfo.Visibility = System.Windows.Visibility.Hidden;
                this.IsTranslateRotateScaleVisible = System.Windows.Visibility.Hidden;
            }
        }

        private void HierarchyTreeView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeView treeView = sender as TreeView;
            if (treeView != null)
            {
                for (int i = 0; i < treeView.Items.Count; ++i)
                {
                    TreeViewItem item = (TreeViewItem)(treeView.ItemContainerGenerator.ContainerFromIndex(i));
                    item.IsSelected = false;
                    this.DetailsInfo.Visibility = System.Windows.Visibility.Hidden;
                    this.IsTranslateRotateScaleVisible = System.Windows.Visibility.Hidden;
                    this.selectedObject = null;
                }
                treeView.Focus();
                OnPropertyChanged("IsRemoveModelMIEnabled");
            }
        }

        private void TranslateToggle_Click(object sender, RoutedEventArgs e)
        {
            this.IsXYZVisible = this.TranslateToggle.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            this.RotateToggle.IsChecked = false;
            this.ScaleToggle.IsChecked = false;
        }

        private void RotateToggle_Click(object sender, RoutedEventArgs e)
        {
            this.IsXYZVisible = this.RotateToggle.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            this.TranslateToggle.IsChecked = false;
            this.ScaleToggle.IsChecked = false;
        }

        private void ScaleToggle_Click(object sender, RoutedEventArgs e)
        {
            this.IsXYZVisible = this.ScaleToggle.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            this.RotateToggle.IsChecked = false;
            this.TranslateToggle.IsChecked = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if(handler != null && name != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private void XToggle_Click(object sender, RoutedEventArgs e)
        {
            this.IsMoreLessVisible = this.XToggle.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            this.YToggle.IsChecked = false;
            this.ZToggle.IsChecked = false;
        }

        private void YToggle_Click(object sender, RoutedEventArgs e)
        {
            this.IsMoreLessVisible = this.YToggle.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            this.XToggle.IsChecked = false;
            this.ZToggle.IsChecked = false;
        }

        private void ZToggle_Click(object sender, RoutedEventArgs e)
        {
            this.IsMoreLessVisible = this.ZToggle.IsChecked.Value ? Visibility.Visible : Visibility.Hidden;
            this.YToggle.IsChecked = false;
            this.XToggle.IsChecked = false;
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            this.ChangeTransform(1.0f);
        }

        private void LessButton_Click(object sender, RoutedEventArgs e)
        {
            this.ChangeTransform(-1.0f);
        }

        private void ChangeTransform(float value)
        {
            if (this.selectedObject == null) return;
            if (this.TranslateToggle.IsChecked.Value)
            {
                if (this.XToggle.IsChecked.Value)
                {
                    //Position.X += value
                    this.selectedObject.MyTransform.Position += new Microsoft.Xna.Framework.Vector3(value, 0, 0);
                }
                else if (this.YToggle.IsChecked.Value)
                {
                    //Position.Y += value
                    this.selectedObject.MyTransform.Position += new Microsoft.Xna.Framework.Vector3(0, value, 0);
                }
                else if (this.ZToggle.IsChecked.Value)
                {
                    //Position.Z += value
                    this.selectedObject.MyTransform.Position += new Microsoft.Xna.Framework.Vector3(0, 0, value);
                }
            }
            else if (this.RotateToggle.IsChecked.Value)
            {
                if (this.XToggle.IsChecked.Value)
                {
                    //Rotation.X += value
                    this.selectedObject.MyTransform.Rotation += new Microsoft.Xna.Framework.Vector3(value, 0, 0);
                }
                else if (this.YToggle.IsChecked.Value)
                {
                    //Rotation.Y += value
                    this.selectedObject.MyTransform.Rotation += new Microsoft.Xna.Framework.Vector3(0, value, 0);
                }
                else if (this.ZToggle.IsChecked.Value)
                {
                    //Rotation.Z += value
                    this.selectedObject.MyTransform.Rotation += new Microsoft.Xna.Framework.Vector3(0, 0, value);
                }
            }
            else if (this.ScaleToggle.IsChecked.Value)
            {
                this.selectedObject.MyTransform.Scale += value;
            }


            this.GenerateDetailsText();
        }

        private void AboutMI_Click(object sender, RoutedEventArgs e)
        {
            if(this.AboutWindow == null)
            {
                this.AboutWindow = new About(this);
                this.AboutWindow.Show();
                this.AboutWindow.Focus();
            }
            else
            {
                this.AboutWindow.Focus();
            }
        }

        private void OpenSceneMI_Click(object sender, RoutedEventArgs e)
        {
            this.IsSaveSceneMIEnabled = true;
        }

        private void SaveSceneMI_Click(object sender, RoutedEventArgs e)
        {
            //Scene save
        }

        private void SaveScene()
        {

        }

        private void QuitMI_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddGameObjectMI_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            uint r = (uint)rnd.Next(0,1000);
            TrashSoup.Engine.GameObject go = new TrashSoup.Engine.GameObject(r, "GameObject " + r);
            go.MyTransform = new TrashSoup.Engine.Transform(go);
            this.GameObjects.Add(go);
        }

        private void RemoveGameObjectMI_Click(object sender, RoutedEventArgs e)
        {
            //Remove game object
            this.GameObjects.Remove(((TrashSoup.Engine.GameObject)this.HierarchyTreeView.SelectedItem));
            this.selectedObject = null;
            this.DetailsInfo.Visibility = System.Windows.Visibility.Hidden;
            OnPropertyChanged("IsRemoveModelMIEnabled");
        }

        private void ObjectComponents_Selected(object sender, RoutedEventArgs e)
        {
            this.ObjectComponents.SelectedItem = null;
        }

        private void ObjectComponents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.ObjectComponents.SelectedItem != null)
            {
                if (this.selectedObject == null) return;
                if (this.ObjectComponents.SelectedItem.GetType() == typeof(TrashSoup.Engine.PhysicalObject))
                {
                    this.selectedObject.MyPhysicalObject = new TrashSoup.Engine.PhysicalObject(this.selectedObject);
                }
                else
                {
                    int i = this.ObjectComponents.SelectedIndex;
                    Type t = this.objectComponents[i].GetType();
                    ConstructorInfo[] ci = t.GetConstructors();
                    ConstructorInfo ciWithLeastParameters = ci[0];
                    foreach (ConstructorInfo c in ci)
                    {
                        if (c.GetParameters().Length < ciWithLeastParameters.GetParameters().Length)
                        {
                            ciWithLeastParameters = c;
                        }
                    }
                    List<object> parameters = new List<object>();
                    int stop = ciWithLeastParameters.GetParameters().Length;
                    for (int j = 0; j < stop; ++j)
                    {
                        parameters.Add(null);
                    }
                    object obj = Activator.CreateInstance(t, parameters.ToArray());
                    Type objType = obj.GetType();
                    foreach(TrashSoup.Engine.ObjectComponent oc in this.selectedObject.Components)
                    {
                        if(oc.GetType() == objType)
                        {
                            this.ObjectComponents.SelectedItem = null;
                            this.GenerateDetailsText();
                            return;
                        }
                    }
                    this.selectedObject.Components.Add((TrashSoup.Engine.ObjectComponent)obj);
                }
                this.ObjectComponents.SelectedItem = null;
                this.GenerateDetailsText();
            }
        }

        private void GenerateDetailsText()
        {
            if (this.selectedObject == null) return;
            this.Test.Text = "ID: " + ((TrashSoup.Engine.GameObject)this.selectedObject).UniqueID + "\n";
            this.Test.Text += "Name: " + ((TrashSoup.Engine.GameObject)this.selectedObject).Name + "\n";
            this.Test.Text += "Position: " + ((TrashSoup.Engine.GameObject)this.selectedObject).MyTransform.Position + "\n";
            this.Test.Text += "Rotation: " + ((TrashSoup.Engine.GameObject)this.selectedObject).MyTransform.Rotation + "\n";
            this.Test.Text += "Scale: " + ((TrashSoup.Engine.GameObject)this.selectedObject).MyTransform.Scale + "\n";
            if(this.selectedObject.MyPhysicalObject != null)
            {
                this.Test.Text += "Drag factor: " + ((TrashSoup.Engine.GameObject)this.selectedObject).MyPhysicalObject.DragFactor + "\n";
                this.Test.Text += "Is using gravity: " + ((TrashSoup.Engine.GameObject)this.selectedObject).MyPhysicalObject.IsUsingGravity + "\n";
                this.Test.Text += "Mass: " + ((TrashSoup.Engine.GameObject)this.selectedObject).MyPhysicalObject.Mass + "\n";
                this.Test.Text += "Position Constratints: " + ((TrashSoup.Engine.GameObject)this.selectedObject).MyPhysicalObject.PositionConstraints + "\n";
                this.Test.Text += "Rotation Constratints: " + ((TrashSoup.Engine.GameObject)this.selectedObject).MyPhysicalObject.RotationConstraints + "\n";
            }
            if(this.selectedObject.Components.Count > 0)
            {
                this.Test.Text += "\n\nAttached components:\n";
                foreach (TrashSoup.Engine.ObjectComponent oc in this.selectedObject.Components)
                {
                    this.Test.Text += oc.ToString() + "\n";
                }
            }
        }
    }
}
