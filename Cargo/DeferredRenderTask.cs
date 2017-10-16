using CargoEngine;
using CargoEngine.Scene;
using CargoEngine.Shader;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SwapChain = CargoEngine.SwapChain;

namespace Cargo
{
    class DeferredRenderTask : RenderTask
    {

        private CargoEngine.Shader.VertexShader vsRender;
        private CargoEngine.Shader.PixelShader psRender;

        private RenderTargetGroup renderTargets;

        private RasterizerState rasterizerState;

        private SwapChain swapChain;

        private RenderTask clearRenderTask,lightRenderTask,combineRenderTask;

        public DeferredRenderTask(SwapChain swapChain) {
            this.swapChain = swapChain;
            renderTargets = new RenderTargetGroup(swapChain, Format.R8G8B8A8_UNorm); // Albedo
            renderTargets.AddDepthStencil();
            renderTargets.AddRenderTarget(Format.R8G8B8A8_UNorm); //Normals
            renderTargets.AddRenderTarget(Format.R16G16B16A16_Float); //Position
            renderTargets.AddRenderTarget(Format.R8G8B8A8_UNorm); //Light
            Init();
        }

        private void Init() {
            vsRender = ShaderLoader.LoadVertexShader(Renderer.Instance, "assets/shader/deferredRender.hlsl", "VSMain");
            psRender = ShaderLoader.LoadPixelShader(Renderer.Instance, "assets/shader/deferredRender.hlsl", "PSMain");

            var rasterizerStateDescription = RasterizerStateDescription.Default();
            rasterizerStateDescription.CullMode = CullMode.Back;
            rasterizerStateDescription.FillMode = FillMode.Solid;
            rasterizerState = new RasterizerState(Renderer.Instance.Device, rasterizerStateDescription);

            clearRenderTask = new ClearRenderTask(renderTargets);
            lightRenderTask = new LightRenderTask(renderTargets.RenderTargets[3], renderTargets.RenderTargets[1], renderTargets.RenderTargets[2]);
            combineRenderTask = new CombineRenderTask(swapChain, renderTargets.RenderTargets[0], renderTargets.RenderTargets[3]);
        }


        public override void QueueRender() {
            Renderer.Instance.QueueTask(clearRenderTask);
            Renderer.Instance.QueueTask(this);
            Renderer.Instance.QueueTask(lightRenderTask);
            Renderer.Instance.QueueTask(combineRenderTask);
        }

        public override void Render(RenderPipeline pipeline) {
            RenderScene(pipeline);
        }

        private void RenderScene(RenderPipeline pipeline) {
            pipeline.OutputMerger.RenderTarget.SetStates(0, renderTargets.GetRenderTargetViews());
            pipeline.OutputMerger.DepthStencilState = pipeline.OutputMerger.DefaultDepthStencilState;            
            pipeline.VertexShader.Shader = vsRender;
            pipeline.PixelShader.Shader = psRender;
//            pipeline.Rasterizer.RasterizerState = rasterizerState;
            pipeline.ApplyOutputResources();
            pipeline.ParameterManager.SetViewMatrix(ViewMatrix);
            pipeline.ParameterManager.SetProjectionMatrix(ProjectionMatrix);
            pipeline.ParameterManager.SetParameter("viewPosition", ViewMatrix.TranslationVector);
            if (Scene != null) {
                Scene.Render(pipeline);
            }
        }
    }
}
