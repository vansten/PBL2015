using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AwesomeEngineEditor
{
    public class XNAImageSource : IDisposable
    {
        private RenderTarget2D renderTarget;

        private WriteableBitmap writeableBitmap;

        private byte[] buffer;

        public RenderTarget2D RenderTarget
        {
            get
            {
                return renderTarget;
            }
        }

        public WriteableBitmap WriteableBitmap
        {
            get
            {
                return writeableBitmap;
            }
        }

        public XNAImageSource(GraphicsDevice device, int width, int height)
        {
            renderTarget = new RenderTarget2D(device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, TrashSoup.TrashSoupGame.Instance.GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.PreserveContents);

            buffer = new byte[width * height * 4];
            writeableBitmap = new WriteableBitmap(width, height, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null);
        }

        ~XNAImageSource()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            renderTarget.Dispose();
            if(disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public void Commit()
        {
            renderTarget.GetData(buffer);

            for(int i = 0; i < buffer.Length - 2; i += 4)
            {
                byte r = buffer[i];
                buffer[i] = buffer[i + 2];
                buffer[i + 2] = r;
            }

            writeableBitmap.Lock();
            Marshal.Copy(buffer, 0, writeableBitmap.BackBuffer, buffer.Length);
            writeableBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, renderTarget.Width, renderTarget.Height));
            writeableBitmap.Unlock();
        }
    }
}
