using CargoEngine;
using CargoEngine.Scene;
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

        private RenderTask clearRenderTask, lightRenderTask, combineRenderTask, fxaaRenderTask;

        private bool fxaa;
        public bool FXAA {
            get {
                return fxaa;
            } set {
                fxaa = value;
                if (fxaa) {
                    ((CombineRenderTask)combineRenderTask).Destination = renderTargets.RenderTargets[4];                    
                } else {
                    ((CombineRenderTask)combineRenderTask).Destination = swapChain.RenderTarget.RenderTargets[0];
                }
            }
        }

        public DeferredRenderTask(SwapChain swapChain) {
            this.swapChain = swapChain;
            renderTargets = new RenderTargetGroup(swapChain, Format.R8G8B8A8_UNorm); // Albedo
            renderTargets.AddDepthStencil();
            renderTargets.AddRenderTarget(Format.R8G8B8A8_UNorm); //Normals
            renderTargets.AddRenderTarget(Format.R16G16B16A16_Float); //Position
            renderTargets.AddRenderTarget(Format.R8G8B8A8_UNorm); //Light
            renderTargets.AddRenderTarget(Format.R8G8B8A8_UNorm); //Combine

            swapChain.OnResize += (o, e) => {
                renderTargets.Resize(e.Size.Width, e.Size.Height);
            };
            Init();

            FXAA = true;
        }

        private void Init() {
            vsRender = Renderer.ShaderLoader.LoadVertexShader("assets/shader/deferredRender.hlsl", "VSMain");
            psRender = Renderer.ShaderLoader.LoadPixelShader("assets/shader/deferredRender.hlsl", "PSMain");

            var rasterizerStateDescription = RasterizerStateDescription.Default();
            rasterizerStateDescription.CullMode = CullMode.Back;
            rasterizerStateDescription.FillMode = FillMode.Solid;
            rasterizerState = new RasterizerState(Renderer.Dev, rasterizerStateDescription);

            clearRenderTask = new ClearRenderTask(renderTargets);
            lightRenderTask = new LightRenderTask(renderTargets.RenderTargets[3], renderTargets.RenderTargets[1], renderTargets.RenderTargets[2]);
            combineRenderTask = new CombineRenderTask(renderTargets.RenderTargets[4], renderTargets.RenderTargets[0], renderTargets.RenderTargets[3]);
            fxaaRenderTask = new PostProcessFXAA(swapChain.RenderTarget.RenderTargets[0],renderTargets.RenderTargets[4]);
        }


        public override void QueueRender() {
            Renderer.Instance.QueueTask(clearRenderTask);
            Renderer.Instance.QueueTask(this);
            Renderer.Instance.QueueTask(lightRenderTask);
            Renderer.Instance.QueueTask(combineRenderTask);
            if (FXAA) {
                Renderer.Instance.QueueTask(fxaaRenderTask);
            }
        }

        public override void Render(RenderPipeline pipeline) {
            RenderScene(pipeline);
        }

        private void RenderScene(RenderPipeline pipeline) {
            pipeline.OutputMerger.RenderTarget.SetStates(0, renderTargets.GetRenderTargetViews());
            pipeline.OutputMerger.DepthStencil = renderTargets.DepthStencilView;
            pipeline.OutputMerger.DepthStencilState = pipeline.OutputMerger.DefaultDepthStencilState;            
            pipeline.VertexShader.Shader = vsRender;
            pipeline.PixelShader.Shader = psRender;
            pipeline.Rasterizer.Viewport = renderTargets.Viewport;
            pipeline.Rasterizer.RasterizerState = rasterizerState;
            pipeline.ParameterManager.SetViewMatrix(ViewMatrix);
            pipeline.ParameterManager.SetProjectionMatrix(ProjectionMatrix);
            pipeline.ParameterManager.SetParameter("viewPosition", ViewMatrix.TranslationVector);

            pipeline.ApplyOutputResources();

            if (Scene != null) {
                Scene.Render(pipeline);
            }
        }

        public override void Dispose() {
            renderTargets.Dispose();
            if (rasterizerState != null) {
                rasterizerState.Dispose();
            }
        }
    }
}
