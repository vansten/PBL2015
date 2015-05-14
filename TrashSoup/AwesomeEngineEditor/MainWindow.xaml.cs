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
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Microsoft.Win32;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

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
        private TrashSoup.Engine.Camera normalCamera;

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

        public string TypeName
        {
            get
            {
                return "XYZ";
            }
        }

        private ObservableCollection<UserControl> LoadedComponents = new ObservableCollection<UserControl>();

        private void FillObjectComponents()
        {
            Type[] types = typeof(TrashSoup.Engine.ObjectComponent).Assembly.GetTypes();
            foreach (Type t in types)
            {
                if (t.IsSubclassOf(typeof(TrashSoup.Engine.ObjectComponent)) && t != typeof(TrashSoup.Engine.Transform) && t != typeof(TrashSoup.Gameplay.CameraBehaviourComponent) && !t.IsAbstract) 
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
            this.Components.ItemsSource = this.LoadedComponents;
            this.DataContext = this;
            this.Closing += MainWindow_Closing;
            this.SetInitialValues();
            this.FillObjectComponents();
            this.ObjectComponents.ItemsSource = this.objectComponents;
            this.myGame = new TrashSoup.TrashSoupGame();
            this.myGame.EditorMode = true;
            this.myGame.GraphicsManager.ApplyChanges();
            this.XNAImage.DrawFunction += Draw;
        }

        private void SetInitialValues()
        {
            this.IsXYZVisible = System.Windows.Visibility.Hidden;
            this.IsMoreLessVisible = System.Windows.Visibility.Hidden;
            this.IsTranslateRotateScaleVisible = System.Windows.Visibility.Hidden;
            this.DetailsInfo.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Draw(GraphicsDevice obj)
        {
            if(this.isSaveSceneMIEnabled)
            {
                this.myGame.EditorUpdate();
                this.myGame.EditorDraw();
            }
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
                this.GenerateRemovableComponentsList();
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
                    if(this.selectedObject.GetType() != typeof(TrashSoup.Engine.Camera))
                    {
                        this.selectedObject.MyTransform.Position += new Microsoft.Xna.Framework.Vector3(value, 0, 0);
                    }
                    else
                    {
                        ((TrashSoup.Engine.Camera)this.selectedObject).Position += new Microsoft.Xna.Framework.Vector3(value, 0, 0);
                    }
                }
                else if (this.YToggle.IsChecked.Value)
                {
                    //Position.Y += value
                    if (this.selectedObject.GetType() != typeof(TrashSoup.Engine.Camera))
                    {
                        this.selectedObject.MyTransform.Position += new Microsoft.Xna.Framework.Vector3(0, value, 0);
                    }
                    else
                    {
                        ((TrashSoup.Engine.Camera)this.selectedObject).Position += new Microsoft.Xna.Framework.Vector3(0, value, 0);
                    }
                }
                else if (this.ZToggle.IsChecked.Value)
                {
                    //Position.Z += value
                    if (this.selectedObject.GetType() != typeof(TrashSoup.Engine.Camera))
                    {
                        this.selectedObject.MyTransform.Position += new Microsoft.Xna.Framework.Vector3(0, 0, value);
                    }
                    else
                    {
                        ((TrashSoup.Engine.Camera)this.selectedObject).Position += new Microsoft.Xna.Framework.Vector3(0, 0, value);
                    }
                }
            }
            else if (this.RotateToggle.IsChecked.Value)
            {
                if (this.XToggle.IsChecked.Value)
                {
                    //Rotation.X += value
                    if(this.selectedObject.GetType() != typeof(TrashSoup.Engine.Camera))
                    {
                        this.selectedObject.MyTransform.Rotation += new Microsoft.Xna.Framework.Vector3(value, 0, 0);
                    }
                }
                else if (this.YToggle.IsChecked.Value)
                {
                    //Rotation.Y += value
                    if (this.selectedObject.GetType() != typeof(TrashSoup.Engine.Camera))
                    {
                        this.selectedObject.MyTransform.Rotation += new Microsoft.Xna.Framework.Vector3(0, value, 0);
                    }
                }
                else if (this.ZToggle.IsChecked.Value)
                {
                    //Rotation.Z += value
                    if (this.selectedObject.GetType() != typeof(TrashSoup.Engine.Camera))
                    {
                        this.selectedObject.MyTransform.Rotation += new Microsoft.Xna.Framework.Vector3(0, 0, value);
                    }
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
            if(this.isSaveSceneMIEnabled)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save scene?", "Warning", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    this.SaveScene();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }

                this.GameObjects.Clear();
                this.GameObjects = new ObservableCollection<TrashSoup.Engine.GameObject>();
                SetInitialValues();
            }
            this.XNAImage.GraphicsDevice = this.myGame.GraphicsDevice;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML files (scenes ofc) (*.xml) | *.xml";
            string initialDirectory = System.IO.Directory.GetCurrentDirectory();
            for(int i = 0; i < 3; ++i)
            {
                System.IO.DirectoryInfo di = System.IO.Directory.GetParent(initialDirectory);
                initialDirectory = di.FullName;
            }
            initialDirectory += "\\TrashSoup\\TrashSoupContent\\Scenes";
            ofd.InitialDirectory = initialDirectory;
            string filepath = "";
            if (ofd.ShowDialog() == true)
            {
                filepath = ofd.FileName;
            }
            else
            {
                this.IsSaveSceneMIEnabled = false;
                return;
            }

            if (filepath != "")
            {
                this.IsSaveSceneMIEnabled = true;

                TrashSoup.Engine.SaveManager.Instance.EditorLoadFileAction(filepath);
                TrashSoup.TrashSoupGame.Instance.EditorLoadContent();
                this.XNAImage.GraphicsDevice = this.myGame.GraphicsDevice;
                this.myGame.ActualRenderTarget = this.XNAImage.ImageSource.RenderTarget;
                this.myGame.DefaultRenderTarget = this.XNAImage.ImageSource.RenderTarget;
            }

            foreach(TrashSoup.Engine.GameObject go in TrashSoup.Engine.ResourceManager.Instance.CurrentScene.ObjectsDictionary.Values)
            {
                if(go != null)
                {
                    this.GameObjects.Add(go);
                }
            }
            foreach(TrashSoup.Engine.LightDirectional l in TrashSoup.Engine.ResourceManager.Instance.CurrentScene.DirectionalLights)
            {
                if(l != null)
                {
                    this.GameObjects.Add(l);
                }
            }
            foreach (TrashSoup.Engine.LightPoint l in TrashSoup.Engine.ResourceManager.Instance.CurrentScene.PointLights)
            {
                if(l != null)
                {
                    this.GameObjects.Add(l);
                }
            }
            this.GameObjects.Add(TrashSoup.Engine.ResourceManager.Instance.CurrentScene.AmbientLight);

            this.normalCamera = TrashSoup.Engine.ResourceManager.Instance.CurrentScene.Cam;

            TrashSoup.Engine.ResourceManager.Instance.CurrentScene.EditorCam = new TrashSoup.Engine.EditorCamera(1, "editorCam", Vector3.Transform(new Vector3(0.0f, 10.0f, -50.0f), Microsoft.Xna.Framework.Matrix.CreateRotationX(MathHelper.PiOver4 * 1.5f)), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 10.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), MathHelper.Pi / 3.0f, 0.1f, 2000.0f);
            TrashSoup.Engine.ResourceManager.Instance.CurrentScene.Cam = TrashSoup.Engine.ResourceManager.Instance.CurrentScene.EditorCam;
            this.GameObjects.Add(this.normalCamera);

            this.IsSaveSceneMIEnabled = true;
        }

        private void SaveSceneMI_Click(object sender, RoutedEventArgs e)
        {
            //Scene save
            this.SaveScene();
        }

        private void SaveScene()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "XML files (*.xml) | *.xml";
            string filepath = "";
            if (sfd.ShowDialog() == true)
            {
                filepath = sfd.FileName;
            }
            else
            {
                return;
            }

            TrashSoup.Engine.ResourceManager.Instance.CurrentScene.ObjectsDictionary = new Dictionary<uint, TrashSoup.Engine.GameObject>();
            this.GameObjects.Remove(this.normalCamera);

            List<TrashSoup.Engine.GameObject> toRemove = new List<TrashSoup.Engine.GameObject>();

            foreach (TrashSoup.Engine.GameObject go in this.GameObjects)
            {
                if (go.GetType() == typeof(TrashSoup.Engine.LightAmbient) || go.GetType() == typeof(TrashSoup.Engine.LightDirectional) || go.GetType() == typeof(TrashSoup.Engine.LightPoint))
                {
                    toRemove.Add(go);
                }
            }

            foreach (TrashSoup.Engine.GameObject go in toRemove)
            {
                this.GameObjects.Remove(go);
            }

            foreach (TrashSoup.Engine.GameObject go in this.GameObjects)
            {
                if (!TrashSoup.Engine.ResourceManager.Instance.CurrentScene.ObjectsDictionary.ContainsKey(go.UniqueID))
                {
                    TrashSoup.Engine.ResourceManager.Instance.CurrentScene.ObjectsDictionary.Add(go.UniqueID, go);
                }
                else
                {
                    TrashSoup.Engine.ResourceManager.Instance.CurrentScene.ObjectsDictionary[go.UniqueID] = go;
                }
            }

            TrashSoup.Engine.ResourceManager.Instance.CurrentScene.Cam = this.normalCamera;

            //TrashSoup.Engine.SaveManager.Instance.SaveFileAction();
            TrashSoup.Engine.SaveManager.Instance.EditorSaveFileAction(filepath);
            TrashSoup.Engine.Debug.Log("Save completed");

            TrashSoup.Engine.ResourceManager.Instance.CurrentScene.Cam = TrashSoup.Engine.ResourceManager.Instance.CurrentScene.EditorCam;
            this.GameObjects.Add(this.normalCamera);
            foreach (TrashSoup.Engine.GameObject go in toRemove)
            {
                this.GameObjects.Add(go);
            }
        }

        private void QuitMI_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddGameObjectMI_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            uint r = (uint)rnd.Next(0, 1000);
            while(TrashSoup.Engine.ResourceManager.Instance.CurrentScene.ObjectsDictionary.ContainsKey(r))
            {
                r = (uint)rnd.Next(0, 1000);
            }
            TrashSoup.Engine.GameObject go = new TrashSoup.Engine.GameObject(r, "GameObject " + r);
            go.MyTransform = new TrashSoup.Engine.Transform(go);
            go.Components = new List<TrashSoup.Engine.ObjectComponent>();
            go.Components.Add(new TrashSoup.Engine.Transform(go));
            this.GameObjects.Add(go);
            TrashSoup.Engine.ResourceManager.Instance.CurrentScene.ObjectsDictionary.Add(go.UniqueID, go);
        }

        private void RemoveGameObjectMI_Click(object sender, RoutedEventArgs e)
        {
            //Remove game object
            TrashSoup.Engine.ResourceManager.Instance.CurrentScene.ObjectsDictionary.Remove(((TrashSoup.Engine.GameObject)HierarchyTreeView.SelectedItem).UniqueID);
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
                    if(this.selectedObject.MyPhysicalObject == null)
                    {
                        this.selectedObject.MyPhysicalObject = new TrashSoup.Engine.PhysicalObject(this.selectedObject);
                    }
                }
                else if(this.ObjectComponents.SelectedItem.GetType() == typeof(TrashSoup.Engine.BoxCollider))
                {
                    this.selectedObject.MyCollider = new TrashSoup.Engine.BoxCollider(this.selectedObject);
                }
                else if(this.ObjectComponents.SelectedItem.GetType() == typeof(TrashSoup.Engine.SphereCollider))
                {
                    this.selectedObject.MyCollider = new TrashSoup.Engine.SphereCollider(this.selectedObject);
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
                        Type parameterType = ciWithLeastParameters.GetParameters()[j].ParameterType;
                        if(parameterType == typeof(TrashSoup.Engine.GameObject))
                        {
                            parameters.Add(this.selectedObject);
                        }
                        else
                        {
                            parameters.Add(null);
                        }
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
                    ((TrashSoup.Engine.ObjectComponent)obj).MyObject = this.selectedObject;
                }
                this.ObjectComponents.SelectedItem = null;
                this.GenerateDetailsText();
            }
        }

        public void GenerateDetailsText()
        {
            if (this.selectedObject == null) return;
            this.LoadedComponents.Clear();
            this.LoadedComponents.Add(new Components.ObjectInfo(this.selectedObject));
            if (this.selectedObject.GetType().IsSubclassOf(typeof(TrashSoup.Engine.Camera)) || this.selectedObject.GetType() == typeof(TrashSoup.Engine.Camera))
            {
                this.GenerateDetailsForCamera();
            }
            else if(this.selectedObject.GetType() == typeof(TrashSoup.Engine.LightPoint) || this.selectedObject.GetType() == typeof(TrashSoup.Engine.LightDirectional) || this.selectedObject.GetType() == typeof(TrashSoup.Engine.LightAmbient))
            {
                System.Type t = this.selectedObject.GetType();
                if(t == typeof(TrashSoup.Engine.LightAmbient))
                {
                    this.GenerateDetailsForAmbientLight();
                }
                else if(t == typeof(TrashSoup.Engine.LightDirectional))
                {
                    this.GenerateDetailsForDirectionalLight();
                }
                else if(t == typeof(TrashSoup.Engine.LightPoint))
                {
                    this.GenerateDetailsForPointLight();
                }
            }
            else
            {
                this.GenerateDetailsForGameObject();
            }
        }

        private void GenerateDetailsForCamera()
        {
            Components.Camera cameraWindow = new Components.Camera();
            this.LoadedComponents.Add(cameraWindow);
        }

        private void GenerateDetailsForAmbientLight()
        {
            TrashSoup.Engine.LightAmbient al = ((TrashSoup.Engine.LightAmbient)this.selectedObject);
            if (al == null) return;

            if(al.MyTransform != null)
            {
                Components.Transform t = new Components.Transform(al.MyTransform);
                this.LoadedComponents.Add(t);
            }
            this.LoadedComponents.Add(new Components.AttachedComponentText());
            this.LoadedComponents.Add(new Components.AmbientLight(al));
        }

        private void GenerateDetailsForDirectionalLight()
        {
            TrashSoup.Engine.LightDirectional dl = ((TrashSoup.Engine.LightDirectional)this.selectedObject);
            if (dl == null) return;

            if (dl.MyTransform != null)
            {
                Components.Transform t = new Components.Transform(dl.MyTransform);
                this.LoadedComponents.Add(t);
            }
            this.LoadedComponents.Add(new Components.AttachedComponentText());
            this.LoadedComponents.Add(new Components.DirectionalLight(dl));
        }

        private void GenerateDetailsForPointLight()
        {
            TrashSoup.Engine.LightPoint pl = ((TrashSoup.Engine.LightPoint)this.selectedObject);
            if (pl == null) return;

            if (pl.MyTransform != null)
            {
                Components.Transform t = new Components.Transform(pl.MyTransform);
                this.LoadedComponents.Add(t);
            }
            this.LoadedComponents.Add(new Components.AttachedComponentText());
            this.LoadedComponents.Add(new Components.PointLight(pl));
        }

        private void GenerateDetailsForGameObject()
        {
            Components.Transform transformWindow = new Components.Transform(this.selectedObject.MyTransform);
            this.LoadedComponents.Add(transformWindow);
            if (this.selectedObject.MyPhysicalObject != null)
            {
                Components.PhysicalObject po = new Components.PhysicalObject(this.selectedObject.MyPhysicalObject);
                this.LoadedComponents.Add(po);
            }
            if (this.selectedObject.MyAnimator != null)
            {
                Components.Animator animatorWindow = new Components.Animator(this.selectedObject.MyAnimator);
                this.LoadedComponents.Add(animatorWindow);
            }

            if(this.selectedObject.MyCollider != null)
            {
                Components.ComponentWindow cw = new Components.ComponentWindow(this.selectedObject.MyCollider);
                this.LoadedComponents.Add(cw);
            }

            if (this.selectedObject.Components.Count > 0)
            {
                this.LoadedComponents.Add(new Components.AttachedComponentText());
                foreach (TrashSoup.Engine.ObjectComponent oc in this.selectedObject.Components)
                {
                    if (oc.GetType() == typeof(TrashSoup.Engine.CustomModel))
                    {
                        Components.CustomModel cm = new Components.CustomModel(((TrashSoup.Engine.CustomModel)oc));
                        this.LoadedComponents.Add(cm);
                        Components.Materials m = new Components.Materials(((TrashSoup.Engine.CustomModel)oc).Mat, this);
                        this.LoadedComponents.Add(m);
                    }
                    else
                    {
                        Components.ComponentWindow cw = new Components.ComponentWindow(oc);
                        this.LoadedComponents.Add(cw);
                    }
                }
            }
        }

        private void CurrentlyAddedComponents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.CurrentlyAddedComponents.SelectedItem == null) return;
            Type t = this.CurrentlyAddedComponents.SelectedItem.GetType();
            if(this.selectedObject.MyAnimator != null && this.selectedObject.MyAnimator.GetType() == t)
            {
                this.selectedObject.MyAnimator = null;
            }
            if (this.selectedObject.MyCollider != null && this.selectedObject.MyCollider.GetType() == t)
            {
                this.selectedObject.MyCollider = null;
            }
            if (this.selectedObject.MyPhysicalObject != null && this.selectedObject.MyPhysicalObject.GetType() == t)
            {
                this.selectedObject.MyPhysicalObject = null;
            }

            foreach(TrashSoup.Engine.ObjectComponent oc in this.selectedObject.Components)
            {
                if(oc.GetType() == t)
                {
                    this.selectedObject.Components.Remove(oc);
                    break;
                }
            }

            this.GenerateRemovableComponentsList();
            this.GenerateDetailsText();
        }

        private void GenerateRemovableComponentsList()
        {
            List<TrashSoup.Engine.ObjectComponent> ocs = new List<TrashSoup.Engine.ObjectComponent>();
            foreach (TrashSoup.Engine.ObjectComponent oc in this.selectedObject.Components)
            {
                ocs.Add(oc);
            }

            if (this.selectedObject.MyCollider != null)
            {
                ocs.Add(this.selectedObject.MyCollider);
            }
            if (this.selectedObject.MyAnimator != null)
            {
                ocs.Add(this.selectedObject.MyAnimator);
            }
            if (this.selectedObject.MyPhysicalObject != null)
            {
                ocs.Add(this.selectedObject.MyPhysicalObject);
            }

            this.CurrentlyAddedComponents.ItemsSource = ocs;
        }

        private void DuplicateGameObjectMI_Click(object sender, RoutedEventArgs e)
        {
            if (this.selectedObject == null) return;
            if (this.selectedObject.GetType() == typeof(TrashSoup.Engine.Camera))
            {
                TrashSoup.Engine.Debug.Log("CAN'T DUPLICATE CAMERA!!!!!!!!!!!!!!!!!!!!!");
                return;
            }

            Random r = new Random();
            uint uid = (uint)r.Next(0, 1000);
            while (TrashSoup.Engine.ResourceManager.Instance.CurrentScene.ObjectsDictionary.ContainsKey(uid))
            {
                uid = (uint)r.Next(0, 1000);
            }

            TrashSoup.Engine.GameObject newGo = new TrashSoup.Engine.GameObject(uid, this.selectedObject.Name + " (clone)");
            newGo.MyTransform = new TrashSoup.Engine.Transform(newGo, this.selectedObject.MyTransform);
            newGo.Enabled = this.selectedObject.Enabled;
            newGo.Visible = this.selectedObject.Visible;
            newGo.Tags = new List<string>();
            if(this.selectedObject.Tags != null)
            {
                foreach (string tag in this.selectedObject.Tags)
                {
                    newGo.Tags.Add(tag);
                }
            }

            if(this.selectedObject.MyAnimator != null)
            {
                newGo.MyAnimator = new TrashSoup.Engine.Animator(newGo, this.selectedObject.MyAnimator);
            }
            if(this.selectedObject.MyCollider != null)
            {
                if(this.selectedObject.MyCollider.GetType() == typeof(TrashSoup.Engine.BoxCollider))
                {
                    newGo.MyCollider = new TrashSoup.Engine.BoxCollider(newGo);
                }
                else if(this.selectedObject.MyCollider.GetType() == typeof(TrashSoup.Engine.SphereCollider))
                {
                    newGo.MyCollider = new TrashSoup.Engine.SphereCollider(newGo);
                }
            }
            if(this.selectedObject.MyPhysicalObject != null)
            {
                newGo.MyPhysicalObject = new TrashSoup.Engine.PhysicalObject(newGo, this.selectedObject.MyPhysicalObject);
            }
            TrashSoup.Engine.CustomModel cm = (TrashSoup.Engine.CustomModel)(this.selectedObject.GetComponent<TrashSoup.Engine.CustomModel>());
            if(cm != null)
            {
                newGo.Components.Add(new TrashSoup.Engine.CustomModel(newGo, cm));
            }

            foreach(TrashSoup.Engine.ObjectComponent oc in this.selectedObject.Components)
            {
                if(oc.GetType() != typeof(TrashSoup.Engine.CustomModel))
                {
                    Type t = oc.GetType();
                    List<object> parameters = new List<object>();
                    parameters.Add(newGo);
                    parameters.Add(oc);
                    object obj = Activator.CreateInstance(t, parameters.ToArray());
                    newGo.Components.Add((TrashSoup.Engine.ObjectComponent)obj);
                    ((TrashSoup.Engine.ObjectComponent)obj).MyObject = newGo;
                }
            }

            this.GameObjects.Add(newGo);
            TrashSoup.Engine.ResourceManager.Instance.CurrentScene.ObjectsDictionary.Add(uid, newGo);
        }
    }
}
