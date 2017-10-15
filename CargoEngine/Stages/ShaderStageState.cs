using CargoEngine.Shader;
using SharpDX.Direct3D11;

namespace CargoEngine.Stages {
    public class ShaderStageState<ShaderClass> : IStageState where ShaderClass : DeviceChild { 
        public static int NUM_CONSTANTBUFFERS = 128;
        public static int NUM_SHADERRESOURCES = 128;
        public static int NUM_SAMPLERS = 16;

        public TStateMonitor<ShaderBase<ShaderClass>> Shader {
            get; private set;
        }

        public TStateArrayMonitor<ConstantBuffer> ConstantBuffer {
            get; private set;
        }

        public TStateArrayMonitor<ShaderResourceView> Resources {
            get; private set;
        }

        public TStateArrayMonitor<SamplerState> Samplers {
            get; private set;
        }

        private ShaderStageState<ShaderClass> sisterState;
        public ShaderStageState<ShaderClass> SisterState {
            get { return sisterState; }
            set {
                sisterState = value;
                Shader.Sister = sisterState.Shader;
            }
        }

        public ShaderStageState() {
            Shader = new TStateMonitor<ShaderBase<ShaderClass>>(null);
            ConstantBuffer = new TStateArrayMonitor<ConstantBuffer>(NUM_CONSTANTBUFFERS, null);
            Resources = new TStateArrayMonitor<ShaderResourceView>(NUM_SHADERRESOURCES, null);
            Samplers = new TStateArrayMonitor<SamplerState>(NUM_SAMPLERS, null);
        }

        public void ResetTracking() {
            Shader.ResetTracking();
            ConstantBuffer.ResetTracking();
            Resources.ResetTracking();
            Samplers.ResetTracking();
        }

        public void ClearState() {
            Shader.InitializeState();
            ConstantBuffer.InitializeState();
            Resources.InitializeState();
            Samplers.InitializeState();
        }

        public void Clone(IStageState src) {
            Shader.State = ((ShaderStageState<ShaderClass>)src).Shader.State;
            for (var i = 0; i < NUM_CONSTANTBUFFERS; i++) {
                ConstantBuffer.States[i] = ((ShaderStageState<ShaderClass>)src).ConstantBuffer.States[i];
            }
            for (var i = 0; i < NUM_SHADERRESOURCES; i++) {
                Resources.States[i] = ((ShaderStageState<ShaderClass>)src).Resources.States[i];
            }
            for (var i = 0; i < NUM_SAMPLERS; i++) {
                Samplers.States[i] = ((ShaderStageState<ShaderClass>)src).Samplers.States[i];
            }
        }

        public void SetSisterState(IStageState sister) {
            var sis = (ShaderStageState<ShaderClass>)sister;
            Shader.Sister = sis.Shader;
            ConstantBuffer.Sister = sis.ConstantBuffer;
            Resources.Sister = sis.Resources;
            Samplers.Sister = sis.Samplers;
        }
    }
}
