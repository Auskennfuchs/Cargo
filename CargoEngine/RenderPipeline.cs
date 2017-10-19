using System;
using CargoEngine.Parameter;
using SharpDX;
using SharpDX.Direct3D11;

namespace CargoEngine
{
    public class RenderPipeline : IDisposable
    {

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

        public void Dispose() {
            OutputMerger.Dispose();
            if (DevContext != null) {
                DevContext.Dispose();
                DevContext = null;
            }
        }

        public void FinishCommandList() {
            CommandList = DevContext.FinishCommandList(false);
            ClearStates();
        }

        public void ExecuteCommandList(CommandList cmdList) {
            if (cmdList != null && !cmdList.IsDisposed) {
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
            if (CommandList != null) {
                CommandList.Dispose();
            }
        }

        public void ApplyRenderTargets() {
            OutputMerger.ApplyRenderTargets(DevContext);
        }

        public void ClearTargets(Color4 col, float depth, byte stencil) {
            var rtCount = OutputMerger.CurrentState.GetRenderTargetCount();
            for (int i = 0; i < rtCount; i++) {
                DevContext.ClearRenderTargetView(OutputMerger.CurrentState.RenderTarget.States[i], col);
            }
            if (OutputMerger.CurrentState.DepthStencilView.State != null) {
                DevContext.ClearDepthStencilView(OutputMerger.CurrentState.DepthStencilView.State, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, depth, stencil);
            }
        }

        public void ApplyOutputResources() {
            OutputMerger.ApplyDesiredState(DevContext, ParameterManager);
        }

        public void ApplyShaderResources() {
            var vs = (CargoEngine.Shader.VertexShader)VertexShader.Shader;
            if (vs!=null && InputAssembler.DesiredState.InputElements.NeedUpdate) {
                var il = vs.GetInputLayout(InputAssembler.InputElements);
                DevContext.InputAssembler.InputLayout = il;
                InputAssembler.CurrentState.InputElements.State = InputAssembler.DesiredState.InputElements.State;
                InputAssembler.DesiredState.InputElements.ResetTracking();
            }

            InputAssembler.ApplyDesiredState(DevContext, ParameterManager);

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
