using CargoEngine.Parameter;
using SharpDX;
using SharpDX.Direct3D11;

namespace CargoEngine.Stages {
    public class RasterizerStage : Stage<RasterizerStageState> {

        public RasterizerState RasterizerState {
            get {
                return DesiredState.RasterizerState.State;
            }
            set {
                DesiredState.RasterizerState.State = value;
            }
        }

        public Viewport Viewport {
            get {
                return DesiredState.Viewport.State;
            }
            set {
                DesiredState.Viewport.State = value;
            }
        }

        public override void OnApplyDesiredState(DeviceContext dc, ParameterManager paramManager) {
            if(DesiredState.RasterizerState.NeedUpdate) {
                dc.Rasterizer.State = DesiredState.RasterizerState.State;
            }
            if(DesiredState.Viewport.NeedUpdate) {
                dc.Rasterizer.SetViewport(DesiredState.Viewport.State);
            }
        }
    }
}
