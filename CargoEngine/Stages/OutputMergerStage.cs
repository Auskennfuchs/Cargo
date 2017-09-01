using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CargoEngine.Parameter;
using SharpDX.Direct3D11;

namespace CargoEngine.Stages {
    public class OutputMergerStage : Stage<OutputMergerStageState> {
        public override void OnApplyDesiredState(DeviceContext dc, ParameterManager paramManager) {
            dc.OutputMerger.BlendState = DesiredState.BlendState.State;
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
