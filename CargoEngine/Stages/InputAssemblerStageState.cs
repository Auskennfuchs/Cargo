using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace CargoEngine.Stages {
    public class InputAssemblerStageState : IStageState {
        public static int NUM_INPUTSLOTS = 32;

        public TStateMonitor<PrimitiveTopology> PrimitiveTopology {
            get; private set;
        }

        public TStateMonitor<InputLayout> InputLayout {
            get; private set;
        }

        public TStateArrayMonitorStruct<VertexBufferBinding> VertexBuffers {
            get; private set;
        }

        public TStateMonitor<Buffer> IndexBuffer {
            get; private set;
        }

        public TStateMonitorEnum<Format> IndexBufferFormat {
            get; private set;
        }

        public InputAssemblerStageState() {
            PrimitiveTopology = new TStateMonitor<SharpDX.Direct3D.PrimitiveTopology>(SharpDX.Direct3D.PrimitiveTopology.Undefined);
            InputLayout = new TStateMonitor<SharpDX.Direct3D11.InputLayout>(null);
            VertexBuffers = new TStateArrayMonitorStruct<VertexBufferBinding>(NUM_INPUTSLOTS, default(VertexBufferBinding));
            IndexBuffer = new TStateMonitor<Buffer>(null);
            IndexBufferFormat = new TStateMonitorEnum<Format>(Format.R32_UInt);
        }

        public void ClearState() {
            PrimitiveTopology.InitializeState();
            InputLayout.InitializeState();
            VertexBuffers.InitializeState();
            IndexBuffer.InitializeState();
            IndexBufferFormat.InitializeState();
        }

        public void Clone(IStageState src) {
            PrimitiveTopology.State = ((InputAssemblerStageState)src).PrimitiveTopology.State;
            InputLayout.State = ((InputAssemblerStageState)src).InputLayout.State;
            for (var i = 0; i < NUM_INPUTSLOTS; i++) {
                VertexBuffers.States[i] = ((InputAssemblerStageState)src).VertexBuffers.States[i];
            }
            IndexBuffer.State = ((InputAssemblerStageState)src).IndexBuffer.State;
            IndexBufferFormat.State = ((InputAssemblerStageState)src).IndexBufferFormat.State;
        }

        public void ResetTracking() {
            PrimitiveTopology.ResetTracking();
            InputLayout.ResetTracking();
            VertexBuffers.ResetTracking();
            IndexBuffer.ResetTracking();
            IndexBufferFormat.ResetTracking();
        }
    }
}
