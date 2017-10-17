using SharpDX.DXGI;
using System;
using SharpDX.WIC;


namespace CargoEngine.Texture
{
    public class TextureLoader
    {

        private static ImagingFactory imagingFactory = new ImagingFactory();

        public static Texture2D Create2D(int width, int height, Format format, byte[] objData = null) {
            var tex = new CargoEngine.Texture.Texture2D(width, height, format);
            tex.SetPixels(objData);
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
                var tex = new Texture2D(formatConverter.Size.Width, formatConverter.Size.Height, Format.R8G8B8A8_UNorm);
                int stride = tex.Width * tex.Format.SizeOfInBytes();
                formatConverter.CopyPixels(tex.GetPixels(), stride);
                tex.ApplyChanges();
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
