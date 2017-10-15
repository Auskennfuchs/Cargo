using CargoEngine.Parameter;
using SharpDX.Direct3D11;

namespace CargoEngine.Stages
{
    public class OutputMergerStage : Stage<OutputMergerStageState>
    {

        public BlendState BlendState {
            get {
                return DesiredState.BlendState.State;
            }
            set {
                DesiredState.BlendState.State = value;
            }
        }

        public DepthStencilState DefaultDepthStencilState {
            get; private set;
        }

        public DepthStencilState NoDepthStencilState {
            get; private set;
        }

        public OutputMergerStage() {
            DefaultDepthStencilState = new DepthStencilState(Renderer.Instance.Device, new DepthStencilStateDescription {
                DepthComparison = Comparison.Less,
                IsDepthEnabled = true,
                IsStencilEnabled = true,
                DepthWriteMask = DepthWriteMask.All,
                StencilReadMask = 0xFF,
                StencilWriteMask = 0xFF,
                FrontFace = new DepthStencilOperationDescription {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Increment,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                },
                BackFace = new DepthStencilOperationDescription {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Decrement,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                }
            });

            NoDepthStencilState = new DepthStencilState(Renderer.Instance.Device, new DepthStencilStateDescription {
                DepthComparison = Comparison.Always,
                IsDepthEnabled = false,
                IsStencilEnabled = false
            });
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
            if (DesiredState.DepthStencilState.NeedUpdate) {
                dc.OutputMerger.DepthStencilState = DesiredState.DepthStencilState.State;
                DesiredState.DepthStencilState.ResetTracking();
            }
            ApplyRenderTargets(dc);
        }

        public void ApplyRenderTargets(DeviceContext dc) {
            if (DesiredState.DepthStencilView.NeedUpdate || DesiredState.RenderTarget.NeedUpdate) {
                dc.OutputMerger.SetRenderTargets(DesiredState.DepthStencilView.State, DesiredState.RenderTarget.ChangedStates);
                CurrentState.DepthStencilView.State = DesiredState.DepthStencilView.State;
                CurrentState.RenderTarget.SetStates(0, DesiredState.RenderTarget.States);
                DesiredState.DepthStencilView.ResetTracking();
                DesiredState.RenderTarget.ResetTracking();
            }
        }
    }
}
