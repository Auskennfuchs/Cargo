using CargoEngine.Parameter;
using SharpDX.Direct3D11;

namespace CargoEngine.Stages {
    public class OutputMergerStage : Stage<OutputMergerStageState> {

        public BlendState BlendState {
            get {
                return DesiredState.BlendState.State;
            }
            set {
                DesiredState.BlendState.State = value;
            }
        }

        public TStateArrayMonitor<RenderTargetView> RenderTarget {
            get {
                return DesiredState.RenderTarget;
            }
        }

        public DepthStencilView DepthStencil {
            get {
                return DesiredState.DepthStencilView.State;
            }
            set {
                DesiredState.DepthStencilView.State = value;
            }
        }

        public DepthStencilState DepthStencilState {
            get {
                return DesiredState.DepthStencilState.State;
            }
            set {
                DesiredState.DepthStencilState.State = value;
            }
        }

        public override void OnApplyDesiredState(DeviceContext dc, ParameterManager paramManager) {
            dc.OutputMerger.BlendState = DesiredState.BlendState.State;
            if(DesiredState.DepthStencilState.NeedUpdate) {
                dc.OutputMerger.DepthStencilState = DesiredState.DepthStencilState.State;
                DesiredState.DepthStencilState.ResetTracking();
            }
            ApplyRenderTargets(dc);
        }

        public void ApplyRenderTargets(DeviceContext dc) {
            dc.OutputMerger.SetRenderTargets(DesiredState.DepthStencilView.State, DesiredState.RenderTarget.ChangedStates);
            CurrentState.DepthStencilView.State = DesiredState.DepthStencilView.State;
            for(int i=0;i<OutputMergerStageState.NUM_RENDERTARGETS;i++) {
                CurrentState.RenderTarget.SetState(i, DesiredState.RenderTarget.States[i]);
            }
            DesiredState.DepthStencilView.ResetTracking();
            DesiredState.RenderTarget.ResetTracking();
        }
    }
}
