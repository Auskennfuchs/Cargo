using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoEngine.Texture
{
    public enum Dimension
    {
        Unknown=-1,
        None = 0,
        Texture1D,
        Texture2D,
        Texture3D,
        Cube,
        Texture2DArray,
        Texture3DArray,
        CubeArray
    }

    public abstract class Texture : IDisposable
    {
        public Dimension Dimension {
            get; protected set;
        }

        public int Width {
            get; protected set;
        }

        public int Height {
            get; protected set;
        }

        public Format Format {
            get; protected set;
        }

        public ShaderResourceView SRV {
            get; protected set;
        }

        public Texture() {
        }

        public Texture(Dimension dimension, int width, int height, Format format) {
            Dimension = dimension;
            Width = width;
            Height = height;
            Format = format;
        }

        public virtual void Dispose() {
            if (SRV != null) {
                SRV.Dispose();
                SRV = null;
            }
        }
    }
}
