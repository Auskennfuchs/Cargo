using SharpDX;
using SharpDX.Direct3D11;

namespace CargoEngine.Stages {
    public class RasterizerStageState : IStageState {

        public TStateMonitor<Viewport> Viewport {
            get; private set;
        }

        public TStateMonitor<RasterizerState> RasterizerState {
            get; private set;
        }

        public RasterizerStageState() {
            Viewport = new TStateMonitor<SharpDX.Viewport>(new SharpDX.Viewport(0, 0, 0, 0));
            RasterizerState = new TStateMonitor<SharpDX.Direct3D11.RasterizerState>(null);
        }

        public void ClearState() {
            Viewport.InitializeState();
            RasterizerState.InitializeState();
        }

        public void Clone(IStageState src) {
            Viewport.State = ((RasterizerStageState)src).Viewport.State;
            RasterizerState.State = ((RasterizerStageState)src).RasterizerState.State;
        }

        public void ResetTracking() {
            Viewport.ResetTracking();
            RasterizerState.ResetTracking();
        }

        public void SetSisterState(IStageState sister) {
            var sis = (RasterizerStageState)sister;
            Viewport.Sister = sis.Viewport;
            RasterizerState.Sister = sis.RasterizerState;
        }
    }
}
