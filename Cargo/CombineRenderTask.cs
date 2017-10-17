using CargoEngine;
using CargoEngine.Shader;
using SharpDX.Direct3D11;

namespace Cargo
{
    class CombineRenderTask : RenderTask
    {

        private SwapChain swapChain;
        private CargoEngine.Shader.VertexShader vsCombine;
        private CargoEngine.Shader.PixelShader psCombine;

        private RenderTarget albedo, light;
        private SamplerState sampler;

        public CombineRenderTask(SwapChain swapChain, RenderTarget albedo, RenderTarget light) {
            this.swapChain = swapChain;
            this.albedo = albedo;
            this.light = light;
            vsCombine = ShaderLoader.LoadVertexShader(Renderer.Instance, "assets/shader/deferredCombine.hlsl", "VSMain");
            psCombine = ShaderLoader.LoadPixelShader(Renderer.Instance, "assets/shader/deferredCombine.hlsl", "PSMain");

            sampler = Renderer.Instance.CreateSamplerState(TextureAddressMode.Wrap, Filter.Anisotropic, 16);
        }

        public override void QueueRender() {
        }

        public override void Render(RenderPipeline pipeline) {
            pipeline.OutputMerger.RenderTarget.SetState(0, swapChain.RenderTarget.RenderTargets[0].View);
            pipeline.OutputMerger.DepthStencilState = pipeline.OutputMerger.NoDepthStencilState;
            pipeline.VertexShader.Shader = vsCombine;
            pipeline.PixelShader.Shader = psCombine;
            pipeline.PixelShader.Sampler.SetState(0, sampler);
            pipeline.ParameterManager.SetParameter("AlbedoTexture", albedo.SRV);
            pipeline.ParameterManager.SetParameter("LightTexture", light.SRV);
            pipeline.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            pipeline.Rasterizer.Viewport = swapChain.Viewport;

            pipeline.ApplyOutputResources();
            pipeline.ApplyInputResources();
            pipeline.ApplyShaderResources();
            pipeline.Draw(3, 0);
        }
    }
}
