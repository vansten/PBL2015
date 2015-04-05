using Microsoft.Xna.Framework.Graphics;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AwesomeEngineEditor
{
    /// <summary>
    /// Interaction logic for XNAControl.xaml
    /// </summary>
    public partial class XNAControl : UserControl
    {
        private GraphicsDeviceService graphicsService;
        private XNAImageSource imageSource;

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return graphicsService.GraphicsDevice;
            }

            set
            {
                graphicsService.GraphicsDevice = value;

                imageSource = new XNAImageSource(GraphicsDevice, (int)ActualWidth, (int)ActualHeight);
                rootImage.Source = imageSource.WriteableBitmap;
            }
        }

        public Action<GraphicsDevice> DrawFunction;

        public XNAControl()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(XnaControl_Loaded);
        }

        ~XNAControl()
        {
            if(imageSource != null)
            {
                imageSource.Dispose();
            }

            if(graphicsService != null)
            {
                graphicsService.Release();
            }
        }

        private void XnaControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(DesignerProperties.GetIsInDesignMode(this) == false)
            {
                InitializeGraphicsDevice();
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if(DesignerProperties.GetIsInDesignMode(this) == false && graphicsService != null)
            {
                imageSource.Dispose();
                imageSource = new XNAImageSource(GraphicsDevice, (int)ActualWidth, (int)ActualHeight);
                rootImage.Source = imageSource.WriteableBitmap;
            }

            base.OnRenderSizeChanged(sizeInfo);
        }

        private void InitializeGraphicsDevice()
        {
            if(graphicsService == null)
            {
                graphicsService = GraphicsDeviceService.AddRef((PresentationSource.FromVisual(this) as HwndSource).Handle);

                imageSource = new XNAImageSource(GraphicsDevice, (int)ActualWidth, (int)ActualHeight);
                rootImage.Source = imageSource.WriteableBitmap;

                CompositionTarget.Rendering += CompositionTarget_Rendering;
            }
        }

        protected virtual void Render()
        {
            if(DrawFunction != null)
            {
                DrawFunction(GraphicsDevice);
            }
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            GraphicsDevice.SetRenderTarget(imageSource.RenderTarget);
            Render();
            GraphicsDevice.SetRenderTarget(null);
            imageSource.Commit();
        }
    }
}
