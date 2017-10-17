using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CargoEngine.Texture
{
    public class Texture2D : Texture
    {
        private SharpDX.Direct3D11.Texture2D dxTexture;

        public int MipMapCount {
            get; private set;
        }

        private byte[] pixels;

        internal ref byte[] GetPixels() {
            return ref pixels;
        }

        public Texture2D(int width, int height, Format format, bool mipMaps = true) : base(Dimension.Texture2D, width, height, format) {
            MipMapCount = 0;// mipMaps ? GetNumMipLevels(width, height) : 0;
            dxTexture = new SharpDX.Direct3D11.Texture2D(Renderer.Dev, new Texture2DDescription {
                Width = width,
                Height = height,
                Format = format,
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                MipLevels = MipMapCount,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
            });
            SRV = new ShaderResourceView(Renderer.Dev, dxTexture);

            pixels = new byte[width * height * format.SizeOfInBytes()];
        }

        public void SetPixels(byte[] data) {
            if (data.Length <= pixels.Length) {
                data.CopyTo(pixels, 0);
            }
        }

        public void SetPixel(int x, int y, Color col) {
            var pixelSize = Format.SizeOfInBytes();
            if (pixelSize > 0) {
                pixels[(x + y * Width) * pixelSize + 0] = col.R;
            }
            if (pixelSize > 1) {
                pixels[(x + y * Width) * pixelSize + 1] = col.G;
            }
            if (pixelSize > 2) {
                pixels[(x + y * Width) * pixelSize + 2] = col.B;
            }
            if (pixelSize > 3) {
                pixels[(x + y * Width) * pixelSize + 3] = col.A;
            }
        }

        public void ApplyChanges() {
            unsafe {
                fixed (void* ptr = pixels) {
                    IntPtr iPtr = (IntPtr)ptr;
                    Renderer.Dev.ImmediateContext.UpdateSubresource(new DataBox(iPtr,Width*Format.SizeOfInBytes(),Height*Width*Format.SizeOfInBytes()), dxTexture);
                }
            }
            if (MipMapCount > 0 && SRV != null) {
                Renderer.Dev.ImmediateContext.GenerateMips(SRV);
            }
        }

        private int GetNumMipLevels(int width, int height) {
            var numLevels = 1;
            while (width > 1 && height > 1) {
                width = Math.Max(width / 2, 1);
                height = Math.Max(height / 2, 1);
                numLevels++;
            }

            return numLevels;
        }
    }
}
