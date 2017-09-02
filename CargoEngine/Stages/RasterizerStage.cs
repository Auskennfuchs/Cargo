using CargoEngine.Parameter;
using SharpDX.Direct3D11;

namespace CargoEngine.Stages {
    public class RasterizerStage : Stage<RasterizerStageState> {
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
