using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CargoEngine.Exception;
using SharpDX.Direct3D11;
using VertexShader = CargoEngine.Shader.VertexShader;

namespace CargoEngine {
    public class Renderer: IDisposable {

        public static Renderer Instance {
            get; private set;
        }

        public Device Device {
            get; private set;
        }
        DeviceContext devContext;

        public RenderPipeline ImmPipeline {
            get; private set;
        }

        private Dictionary<int,InputLayout> inputLayouts;

        public Renderer() {
            if(Instance!=null) {
                throw CargoEngineException.Create("multiple instances of renderer");
            }
            Instance = this;

            DeviceCreationFlags devFlags = 0;
#if DEBUG
            devFlags |= DeviceCreationFlags.Debug;
#endif
            Device = new Device(SharpDX.Direct3D.DriverType.Hardware, devFlags);
            devContext = Device.ImmediateContext;

            ImmPipeline = new RenderPipeline(devContext);

            inputLayouts = new Dictionary<int, InputLayout>();
        }

        ~Renderer() {
            this.Dispose();
        }

        public void Dispose() {
            if (Device != null) {
                Device.Dispose();
            }
        }

        public InputLayout GetInputLayout(VertexShader vShader) {
            if (vShader != null && inputLayouts.ContainsKey(vShader.ID)) {
                return inputLayouts[vShader.ID];
            }
            return null;
        }

        public InputLayout AddInputLayout(VertexShader vShader,InputElement[] elements) {
            var il = new InputLayout(Device, vShader.InputSignature.Data, elements);
            inputLayouts[vShader.ID] = il;
            return il;
        }

    }
}
