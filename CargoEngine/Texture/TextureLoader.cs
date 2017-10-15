using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using SharpDX.WIC;


namespace CargoEngine.Texture
{
    public class TextureLoader
    {

        private static ImagingFactory imagingFactory = new ImagingFactory();

        public static Texture2D Create2D(int width, int height, Format format, object[] objData = null, int objectElementSize = 0) {
            //todo
            var tex = new Texture2D(Renderer.Instance.Device, new Texture2DDescription {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Width = width,
                Height = height,
                Format = format,
                MipLevels = 0,
                Usage = ResourceUsage.Default,
            });
            return tex;
        }

        public static Texture2D FromFile(string file) {
            using (var bitmapDecoder = new BitmapDecoder(imagingFactory, file, DecodeOptions.CacheOnDemand))
            using (var formatConverter = new FormatConverter(imagingFactory)) {
                formatConverter.Initialize(
                    bitmapDecoder.GetFrame(0),
                    PixelFormat.Format32bppPRGBA,
                    BitmapDitherType.None,
                    null,
                    0.0,
                    BitmapPaletteType.Custom);
                return CreateTexture2DFromBitmap(formatConverter);
            }
        }

        private static Texture2D CreateTexture2DFromBitmap(BitmapSource bitmapSource) {
            // Allocate DataStream to receive the WIC image pixels
            int stride = bitmapSource.Size.Width * 4;
            using (var buffer = new SharpDX.DataStream(bitmapSource.Size.Height * stride, true, true)) {
                // Copy the content of the WIC to the buffer
                bitmapSource.CopyPixels(stride, buffer);
                var tex = new Texture2D(Renderer.Instance.Device, new Texture2DDescription() {
                    Width = bitmapSource.Size.Width,
                    Height = bitmapSource.Size.Height,
                    Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                    ArraySize = 1,
                    BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                    Usage = ResourceUsage.Default,
                    CpuAccessFlags = CpuAccessFlags.None,
                    MipLevels = GetNumMipLevels(bitmapSource.Size.Width, bitmapSource.Size.Height),
                    OptionFlags = ResourceOptionFlags.GenerateMipMaps,
                    SampleDescription = new SampleDescription(1, 0),
                });
                Renderer.Instance.Device.ImmediateContext.UpdateSubresource(tex, 0, null, buffer.DataPointer, stride, 0);
                return tex;
            }
        }

        private static int GetNumMipLevels(int width, int height) {
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
