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
        private object selectedObject = null;

        private Visibility isXYZVisible;
        private Visibility isMoreLessVisible;
        private Visibility isTranslateRotateScaleVisible;
        private bool isSaveSceneMIEnabled = false;
        private ObservableCollection<TrashSoup.Engine.ObjectComponent> objectComponents = new ObservableCollection<TrashSoup.Engine.ObjectComponent>();

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
            List<Type> forbiddenTypes = new List<Type>();
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
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Closing += MainWindow_Closing;
            this.IsXYZVisible = System.Windows.Visibility.Hidden;
            this.IsMoreLessVisible = System.Windows.Visibility.Hidden;
            this.IsTranslateRotateScaleVisible = System.Windows.Visibility.Hidden;
            this.DetailsInfo.Visibility = System.Windows.Visibility.Hidden;
            this.FillObjectComponents();
            this.ObjectComponents.ItemsSource = this.objectComponents;
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
            this.selectedObject = this.HierarchyTreeView.SelectedItem;
            if(this.selectedObject != null)
            {
                OnPropertyChanged("IsRemoveModelMIEnabled");
                this.DetailsInfo.Visibility = System.Windows.Visibility.Visible;
                this.IsTranslateRotateScaleVisible = System.Windows.Visibility.Visible;
                this.Test.Text = ((TreeViewItem)this.selectedObject).Header.ToString();
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
                TreeViewItem item = (TreeViewItem)treeView.SelectedItem;
                if(item != null)
                {
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
            if (this.TranslateToggle.IsChecked.Value)
            {
                if (this.XToggle.IsChecked.Value)
                {
                    //Position.X += value
                }
                else if (this.YToggle.IsChecked.Value)
                {
                    //Position.Y += value
                }
                else if (this.ZToggle.IsChecked.Value)
                {
                    //Position.Z += value
                }
            }
            else if (this.RotateToggle.IsChecked.Value)
            {
                if (this.XToggle.IsChecked.Value)
                {
                    //Rotation.X += value
                }
                else if (this.YToggle.IsChecked.Value)
                {
                    //Rotation.Y += value
                }
                else if (this.ZToggle.IsChecked.Value)
                {
                    //Rotation.Z += value
                }
            }
            else if (this.ScaleToggle.IsChecked.Value)
            {
                if (this.XToggle.IsChecked.Value)
                {
                    //Scale.X += value
                }
                else if (this.YToggle.IsChecked.Value)
                {
                    //Scale.Y += value
                }
                else if (this.ZToggle.IsChecked.Value)
                {
                    //Scale.Z += value
                }
            }
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
            TreeViewItem newItem = new TreeViewItem();
            Random rnd = new Random();
            newItem.Header = "GameObject " + rnd.Next(0, 1000).ToString();
            if(this.selectedObject != null)
            {
                ((TreeViewItem)this.selectedObject).Items.Add(newItem);
            }
            else
            {
                this.HierarchyTreeView.Items.Add(newItem);
            }
        }

        private void RemoveGameObjectMI_Click(object sender, RoutedEventArgs e)
        {
            //Remove model
            TreeViewItem curItem = ((TreeViewItem)this.selectedObject);
            Type curItemParentType = curItem.Parent.GetType();
            if(curItemParentType == curItem.GetType())
            {
                TreeViewItem parent = ((TreeViewItem)curItem.Parent);
                parent.Items.Remove(curItem);
            }
            else
            {
                TreeView parent = ((TreeView)curItem.Parent);
                parent.Items.Remove(curItem);
            }
            curItem.IsSelected = false;
            foreach(TreeViewItem tvi in this.HierarchyTreeView.Items)
            {
                tvi.IsSelected = false;
            }
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
                this.ObjectComponents.SelectedItem = null;
            }
        }
    }
}
