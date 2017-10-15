using CargoEngine;
using SharpDX.Direct3D11;
using VertexShader = CargoEngine.Shader.VertexShader;
using PixelShader = CargoEngine.Shader.PixelShader;
using CargoEngine.Shader;
using CargoEngine.Scene;
using SharpDX;

namespace Cargo {
    class SimpleRenderTask : RenderTask {

        private RenderTargetGroup renderTarget;

        private VertexShader vShader;
        private PixelShader pShader;

        private RasterizerState rasterizerState;

        public SimpleRenderTask(RenderTargetGroup rt) {
            renderTarget = rt;

            vShader = ShaderLoader.LoadVertexShader(Renderer.Instance, "assets/shader/simple.hlsl", "VSMain");
            pShader = ShaderLoader.LoadPixelShader(Renderer.Instance, "assets/shader/simple.hlsl", "PSMain");

            var rasterizerStateDescription = RasterizerStateDescription.Default();
            rasterizerStateDescription.CullMode = CullMode.Back;
           rasterizerState = new RasterizerState(Renderer.Instance.Device, rasterizerStateDescription);
        }

        ~SimpleRenderTask() {
            vShader.Dispose();
            pShader.Dispose();
        }

        public override void Render(RenderPipeline pipeline) {
            pipeline.OutputMerger.ClearDesiredState();
            pipeline.OutputMerger.RenderTarget[0] = renderTarget.RenderTargets[0];
            pipeline.OutputMerger.DepthStencil = renderTarget.DepthStencilView;
            pipeline.OutputMerger.ApplyRenderTargets(pipeline.DevContext);
            pipeline.ClearTargets(new Color4(0.2f, 0.2f, 0.2f, 1.0f), 1.0f, 0);
            pipeline.Rasterizer.RasterizerState = rasterizerState;
            pipeline.Rasterizer.Viewport = renderTarget.Viewport;
            pipeline.VertexShader.Shader = vShader;
            pipeline.PixelShader.Shader = pShader;
            pipeline.ParameterManager.SetViewMatrix(ViewMatrix);
            pipeline.ParameterManager.SetProjectionMatrix(ProjectionMatrix);
            if(Scene!=null) {
                Scene.Render(pipeline);
            }
        }

        public override void QueueRender() {
            Renderer.Instance.QueueTask(this);
        }
    }
}
