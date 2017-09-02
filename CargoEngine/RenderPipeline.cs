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

        public Stages.VertexShaderStage VertexShader {
            get; private set;
        }

        public Stages.PixelShaderStage PixelShader {
            get; private set;
        }

        public Stages.RasterizerStage Rasterizer {
            get; private set;
        }

        public Stages.InputAssemblerStage InputAssembler {
            get; private set;
        }

        public Stages.OutputMergerStage OutputMerger {
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
            Rasterizer = new Stages.RasterizerStage();
            OutputMerger = new Stages.OutputMergerStage();
            VertexShader = new Stages.VertexShaderStage();
            PixelShader = new Stages.PixelShaderStage();
            InputAssembler = new Stages.InputAssemblerStage();
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
            ClearStates();
        }

        public void ExecuteCommandList(CommandList cmdList) {
            if(cmdList!=null && !cmdList.IsDisposed) {
                DevContext.ExecuteCommandList(cmdList, false);
            }
            ClearStates();
        }

        private void ClearStates() {
            VertexShader.ClearCurrentState();
            VertexShader.ClearDesiredState();

            PixelShader.ClearCurrentState();
            PixelShader.ClearDesiredState();

            OutputMerger.ClearCurrentState();
            OutputMerger.ClearDesiredState();

            InputAssembler.ClearCurrentState();
            InputAssembler.ClearDesiredState();

            Rasterizer.ClearCurrentState();
            Rasterizer.ClearDesiredState();
        }

        public void ReleaseCommandList() {
            CommandList.Dispose();
        }

        public void ApplyRenderTargets() {
            OutputMerger.ApplyRenderTargets(DevContext);
        }

        public void ClearTargets(Color4 col, float depth, byte stencil) {
            var rtCount = OutputMerger.CurrentState.GetRenderTargetCount();
            for(int i=0;i<rtCount;i++) {
                DevContext.ClearRenderTargetView(OutputMerger.CurrentState.RenderTarget.States[i], col);
            }
            if (OutputMerger.CurrentState.DepthStencilView.State != null) {
                DevContext.ClearDepthStencilView(OutputMerger.CurrentState.DepthStencilView.State, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, depth, stencil);
            }
        }

        public void ApplyInputResources() {
            InputAssembler.ApplyDesiredState(DevContext, ParameterManager);
        }

        public void ApplyShaderResources() {
            VertexShader.ApplyDesiredState(DevContext, ParameterManager);
            PixelShader.ApplyDesiredState(DevContext, ParameterManager);

            Rasterizer.ApplyDesiredState(DevContext, ParameterManager);
        }

        public void Draw(int vertexCount, int startVertex) {
            DevContext.Draw(vertexCount, startVertex);
        }

        public void DrawIndexed(int indexCount, int startIndex, int baseVertex) {
            DevContext.DrawIndexed(indexCount, startIndex, baseVertex);
        }
    }
}
