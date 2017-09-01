using System;
using CargoEngine.Stages;
using SharpDX.Direct3D11;

namespace CargoEngine {
    public class RenderPipeline : IDisposable{

        DeviceContext devContext;

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

        public RenderPipeline(Device device) {
            devContext = new DeviceContext(device);
            InitStages();
        }

        public RenderPipeline(DeviceContext deviceContext) {
            devContext = deviceContext;
            InitStages();
        }

        private void InitStages() {
            shaderStages[(int)ShaderStages.VERTEX] = new Stages.VertexShaderStage();
            shaderStages[(int)ShaderStages.PIXEL] = new Stages.PixelShaderStage();

            RasterizerStage = new Stages.RasterizerStage();
        }

        ~RenderPipeline() {
            this.Dispose();
        }

        public void Dispose() {
            if(devContext!=null) {
                devContext.Dispose();
            }
        }

        public void FinishCommandList() {
            CommandList = devContext.FinishCommandList(false);
        }

        public void ExecuteCommandList() {
            if(!CommandList.IsDisposed) {
                devContext.ExecuteCommandList(CommandList, false);
            }
        }

        public void ReleaseCommandList() {
            CommandList.Dispose();
        }
    }
}
