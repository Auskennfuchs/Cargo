using CargoEngine.Parameter;
using SharpDX.Direct3D11;

namespace CargoEngine.Stages {
    public class InputAssemblerStage : Stage<InputAssemblerStageState> {
        public override void OnApplyDesiredState(DeviceContext dc, ParameterManager paramManager) {
            if (DesiredState.InputLayout.NeedUpdate) {
                dc.InputAssembler.InputLayout = DesiredState.InputLayout.State;
            }
            if (DesiredState.PrimitiveTopology.NeedUpdate) {
                dc.InputAssembler.PrimitiveTopology = DesiredState.PrimitiveTopology.State;
            }
            if (DesiredState.VertexBuffers.NeedUpdate) {
                dc.InputAssembler.SetVertexBuffers(DesiredState.VertexBuffers.StartSlot, DesiredState.VertexBuffers.ChangedStates);
            }
        }
    }
}
