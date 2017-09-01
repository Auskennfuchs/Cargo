using System;
using CargoEngine.Parameter;
using CargoEngine.Stages;
using SharpDX;
using SharpDX.Direct3D11;

namespace CargoEngine {
    public class RenderPipeline : IDisposable{

        public DeviceContext DevContext {
            get; private set;
        }

        public ParameterManager ParameterManager {
            get; private set;
        }

        public CommandList CommandList {
            get; private set;
        }

        private ShaderStage[] shaderStages = new ShaderStage[(int)ShaderStages.NUM_SHADERSTAGES];

        public Stages.VertexShaderStage VertexShaderStage {
            get { return ((Stages.VertexShaderStage)shaderStages[(int)ShaderStages.VERTEX]); }
        }

        public Stages.RasterizerStage RasterizerStage {
            get; private set;
        }

        public Stages.OutputMergerStage OutputMergerStage {
            get; private set;
        }

        public RenderPipeline(Device device) {
            DevContext = new DeviceContext(device);
            InitStages();
        }

        public RenderPipeline(DeviceContext deviceContext) {
            DevContext = deviceContext;
            InitStages();
        }

        private void InitStages() {
            ParameterManager = new ParameterManager();

            shaderStages[(int)ShaderStages.VERTEX] = new Stages.VertexShaderStage();
            shaderStages[(int)ShaderStages.PIXEL] = new Stages.PixelShaderStage();

            RasterizerStage = new Stages.RasterizerStage();
            OutputMergerStage = new Stages.OutputMergerStage();
        }

        ~RenderPipeline() {
            this.Dispose();
        }

        public void Dispose() {
            if(DevContext!=null) {
                DevContext.Dispose();
            }
        }

        public void FinishCommandList() {
            CommandList = DevContext.FinishCommandList(false);
        }

        public void ExecuteCommandList() {
            if(!CommandList.IsDisposed) {
                DevContext.ExecuteCommandList(CommandList, false);
            }
        }

        public void ReleaseCommandList() {
            CommandList.Dispose();
        }

        public void ApplyRenderTargets() {
            OutputMergerStage.ApplyRenderTargets(DevContext);
        }

        public void ClearBuffer(Color4 col, float depth, byte stencil) {
            var rtCount = OutputMergerStage.CurrentState.GetRenderTargetCount();
            for(int i=0;i<rtCount;i++) {
                DevContext.ClearRenderTargetView(OutputMergerStage.CurrentState.RenderTarget.States[i], col);
            }
            if (OutputMergerStage.CurrentState.DepthStencilView.State != null) {
                DevContext.ClearDepthStencilView(OutputMergerStage.CurrentState.DepthStencilView.State, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, depth, stencil);
            }
        }
    }
}
