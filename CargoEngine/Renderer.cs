using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;

namespace CargoEngine {
    public class Renderer: IDisposable {
        public Device Device {
            get; private set;
        }
        DeviceContext devContext;

        public RenderPipeline ImmPipeline {
            get; private set;
        }

        public Renderer() {
            DeviceCreationFlags devFlags = 0;
#if DEBUG
            devFlags |= DeviceCreationFlags.Debug;
#endif
            Device = new Device(SharpDX.Direct3D.DriverType.Hardware, devFlags);
            devContext = Device.ImmediateContext;

            ImmPipeline = new RenderPipeline(devContext);
        }

        ~Renderer() {
            this.Dispose();
        }

        public void Dispose() {
            if (Device != null) {
                Device.Dispose();
            }
        }
    }
}
