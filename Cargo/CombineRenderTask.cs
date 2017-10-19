using CargoEngine;
using CargoEngine.Parameter;
using CargoEngine.Shader;
using SharpDX.Direct3D11;

namespace Cargo
{
    class CombineRenderTask : RenderTask
    {

        private RenderTarget destination;
        private CargoEngine.Shader.VertexShader vsCombine;
        private CargoEngine.Shader.PixelShader psCombine;

        private RenderTarget albedo, light;

        private ParameterCollection parameterCollection = new ParameterCollection();

        public CombineRenderTask(RenderTarget destination, RenderTarget albedo, RenderTarget light) {
            this.destination = destination;
            this.albedo = albedo;
            this.light = light;
            vsCombine = Renderer.ShaderLoader.LoadVertexShader("assets/shader/deferredCombine.hlsl", "VSMain");
            psCombine = Renderer.ShaderLoader.LoadPixelShader("assets/shader/deferredCombine.hlsl", "PSMain");

            var sampler = Renderer.Instance.CreateSamplerState(TextureAddressMode.Wrap, Filter.MinMagMipPoint, 1);
            parameterCollection.SetParameter("Sampler", sampler);
        }

        public override void QueueRender() {
        }

        public override void Render(RenderPipeline pipeline) {
            pipeline.OutputMerger.RenderTarget.SetState(0, destination.View);
            pipeline.OutputMerger.DepthStencilState = pipeline.OutputMerger.NoDepthStencilState;
            pipeline.VertexShader.Shader = vsCombine;
            pipeline.PixelShader.Shader = psCombine;
            pipeline.ParameterManager.ApplyCollection(parameterCollection);
            pipeline.ParameterManager.SetParameter("AlbedoTexture", albedo.SRV);
            pipeline.ParameterManager.SetParameter("LightTexture", light.SRV);
            pipeline.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            pipeline.Rasterizer.Viewport = new SharpDX.Viewport(0,0,destination.Width,destination.Height);

            pipeline.ApplyOutputResources();
            pipeline.ApplyShaderResources();
            pipeline.Draw(3, 0);
        }
    }
}
