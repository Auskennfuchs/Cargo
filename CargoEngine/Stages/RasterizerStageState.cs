using SharpDX;

namespace CargoEngine.Stages {
    class RasterizerStageState : IStageState {

        public TStateMonitor<Viewport> Viewport {
            get; private set;
        }

        public RasterizerStageState() {
            Viewport = new TStateMonitor<SharpDX.Viewport>(new SharpDX.Viewport(0, 0, 0, 0));
        }

        public void ClearState() {
            Viewport.InitializeState();
        }

        public void Clone(IStageState src) {
            Viewport.State = ((RasterizerStageState)src).Viewport.State;
        }

        public void ResetTracking() {
            Viewport.ResetTracking();
        }
    }
}
