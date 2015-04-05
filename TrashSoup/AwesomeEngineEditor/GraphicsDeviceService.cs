using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AwesomeEngineEditor
{
    class GraphicsDeviceService : IGraphicsDeviceService
    {
        private static GraphicsDeviceService instance;
        private static int referenceCount;

        public static GraphicsDeviceService Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new GraphicsDeviceService();
                }
                return instance;
            }
        }

        private PresentationParameters parameters;
        public GraphicsDevice GraphicsDevice { get; set; }

        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        GraphicsDeviceService()
        {

        }

        private void CreateDevice(IntPtr windowHandle)
        {
            parameters = new PresentationParameters();
            parameters.BackBufferWidth = 480;
            parameters.BackBufferHeight = 320;
            parameters.BackBufferFormat = SurfaceFormat.Color;
            parameters.DeviceWindowHandle = windowHandle;
            parameters.DepthStencilFormat = DepthFormat.Depth24Stencil8;
            parameters.IsFullScreen = false;

            GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, parameters);
            
            if(DeviceCreated != null)
            {
                DeviceCreated(this, EventArgs.Empty);
            }
        }

        public static GraphicsDeviceService AddRef(IntPtr windowHandle)
        {
            if(Interlocked.Increment(ref referenceCount) == 1)
            {
                Instance.CreateDevice(windowHandle);
            }

            return instance;
        }

        public void Release()
        {
            if(Interlocked.Decrement(ref referenceCount) == 0)
            {
                if(DeviceDisposing != null)
                {
                    DeviceDisposing(this, EventArgs.Empty);
                }

                GraphicsDevice.Dispose();

                GraphicsDevice = null;
            }
        }
    }
}
