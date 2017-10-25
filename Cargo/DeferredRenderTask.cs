using CargoEngine;
using CargoEngine.Scene;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using SwapChain = CargoEngine.SwapChain;

namespace Cargo
{
    class DeferredRenderTask : RenderTask
    {

        private CargoEngine.Shader.VertexShader vsRender;
        private CargoEngine.Shader.PixelShader psRender;

        public RenderTargetGroup RenderTargets;

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
                    ((CombineRenderTask)combineRenderTask).Destination = RenderTargets.RenderTargets[4];                    
                } else {
                    ((CombineRenderTask)combineRenderTask).Destination = swapChain.RenderTarget.RenderTargets[0];
                }
            }
        }

        public DeferredRenderTask(SwapChain swapChain) {
            this.swapChain = swapChain;
            RenderTargets = new RenderTargetGroup(swapChain, Format.R8G8B8A8_UNorm); // Albedo
            RenderTargets.AddRenderTarget(Format.R8G8B8A8_UNorm); //Normals
            RenderTargets.AddRenderTarget(Format.R16G16B16A16_Float); //Position
            RenderTargets.AddRenderTarget(Format.R8G8B8A8_UNorm); //Light
            RenderTargets.AddRenderTarget(Format.R8G8B8A8_UNorm); //Combine
            RenderTargets.AddDepthStencil();

            swapChain.OnResize += (o, e) => {
                RenderTargets.Resize(e.Size.Width, e.Size.Height);
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

            clearRenderTask = new ClearRenderTask(RenderTargets);
            lightRenderTask = new LightRenderTask(RenderTargets.RenderTargets[3], RenderTargets.RenderTargets[1], RenderTargets.RenderTargets[2]);
            combineRenderTask = new CombineRenderTask(RenderTargets.RenderTargets[4], RenderTargets.RenderTargets[0], RenderTargets.RenderTargets[3]);
            fxaaRenderTask = new PostProcessFXAA(swapChain.RenderTarget.RenderTargets[0],RenderTargets.RenderTargets[4]);
        }


        public override void QueueRender() {
            clearRenderTask.QueueRender();
            Renderer.Instance.QueueTask(this);
            lightRenderTask.QueueRender();
            combineRenderTask.QueueRender();
            if (FXAA) {
                fxaaRenderTask.QueueRender();
            }
        }

        public override void Render(RenderPipeline pipeline) {
            RenderScene(pipeline);
        }

        private void RenderScene(RenderPipeline pipeline) {
            pipeline.OutputMerger.RenderTarget.SetStates(0, RenderTargets.GetRenderTargetViews());
            pipeline.OutputMerger.DepthStencil = RenderTargets.DepthStencilView;
            pipeline.OutputMerger.DepthStencilState = pipeline.OutputMerger.DefaultDepthStencilState;            
            pipeline.VertexShader.Shader = vsRender;
            pipeline.PixelShader.Shader = psRender;
            pipeline.Rasterizer.Viewport = RenderTargets.Viewport;
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
            RenderTargets.Dispose();
            rasterizerState?.Dispose();
        }
    }
}
