using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace CargoEngine.Stages {
    public class InputAssemblerStageState : IStageState {
        public static int NUM_INPUTSLOTS = 32;

        public TStateMonitor<PrimitiveTopology> PrimitiveTopology {
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

        internal TStateMonitor<InputElementList> InputElements {
            get; private set;
        }

        public InputAssemblerStageState() {
            PrimitiveTopology = new TStateMonitor<SharpDX.Direct3D.PrimitiveTopology>(SharpDX.Direct3D.PrimitiveTopology.Undefined);
            VertexBuffers = new TStateArrayMonitorStruct<VertexBufferBinding>(NUM_INPUTSLOTS, default(VertexBufferBinding));
            IndexBuffer = new TStateMonitor<Buffer>(null);
            IndexBufferFormat = new TStateMonitorEnum<Format>(Format.R32_UInt);
            InputElements = new TStateMonitor<InputElementList>(null);
        }

        public void ClearState() {
            PrimitiveTopology.InitializeState();
            VertexBuffers.InitializeState();
            IndexBuffer.InitializeState();
            IndexBufferFormat.InitializeState();
            InputElements.InitializeState();
        }

        public void Clone(IStageState src) {
            PrimitiveTopology.State = ((InputAssemblerStageState)src).PrimitiveTopology.State;
            for (var i = 0; i < NUM_INPUTSLOTS; i++) {
                VertexBuffers.States[i] = ((InputAssemblerStageState)src).VertexBuffers.States[i];
            }
            IndexBuffer.State = ((InputAssemblerStageState)src).IndexBuffer.State;
            IndexBufferFormat.State = ((InputAssemblerStageState)src).IndexBufferFormat.State;
            InputElements.State = ((InputAssemblerStageState)src).InputElements.State;
        }

        public void ResetTracking() {
            PrimitiveTopology.ResetTracking();
            VertexBuffers.ResetTracking();
            IndexBuffer.ResetTracking();
            IndexBufferFormat.ResetTracking();
//            InputElements.ResetTracking(); // reset nicht hier, weil in VertexShader gesetzt
        }

        public void SetSisterState(IStageState sister) {
            var sis = (InputAssemblerStageState)sister;
            PrimitiveTopology.Sister = sis.PrimitiveTopology;
            VertexBuffers.Sister = sis.VertexBuffers;
            IndexBuffer.Sister = sis.IndexBuffer;
            IndexBufferFormat.Sister = sis.IndexBufferFormat;
            InputElements.Sister = sis.InputElements;
        }
    }
}
