using CargoEngine;
using CargoEngine.Parameter;
using SharpDX;
using SharpDX.Direct3D11;
using System;

namespace Cargo
{
    class PostProcessFXAA : RenderTask
    {
        private CargoEngine.Shader.VertexShader vs;
        private CargoEngine.Shader.PixelShader ps;
        private RenderTarget destination;

        private ParameterCollection paramCollection = new ParameterCollection();

        private RasterizerState rasterizer;
        private BlendState blendstate;

        public PostProcessFXAA(RenderTarget dest, RenderTarget source) {
            destination = dest;
            vs = Renderer.ShaderLoader.LoadVertexShader("assets/shader/fxaa.hlsl", "FxaaVS");
            ps = Renderer.ShaderLoader.LoadPixelShader("assets/shader/fxaa.hlsl", "FxaaPS");
            paramCollection.SetParameter("RCPFrame", new Vector4(1.0f / destination.Width, 1.0f / destination.Height, 0, 0));
            paramCollection.SetParameter("InputSampler", Renderer.Instance.CreateSamplerState(SharpDX.Direct3D11.TextureAddressMode.Clamp, SharpDX.Direct3D11.Filter.MinMagLinearMipPoint,
                SharpDX.Direct3D11.Comparison.Always, 4));
            paramCollection.SetParameter("InputTexture", source);

            rasterizer = new RasterizerState(Renderer.Dev, new RasterizerStateDescription {
                CullMode = CullMode.Back,
                FillMode = FillMode.Solid,
                IsDepthClipEnabled = true,
                IsMultisampleEnabled = true,
            });

            var bsd = BlendStateDescription.Default();
            bsd.IndependentBlendEnable = false;
            bsd.RenderTarget[0].IsBlendEnabled = false;
            bsd.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            blendstate = new BlendState(Renderer.Dev, bsd);

            destination.OnResize += (o, e) => {
                paramCollection.SetParameter("RCPFrame", new Vector4(1.0f / e.Size.Width, 1.0f / e.Size.Height, 0, 0));
            };
        }

        public override void Dispose() {
            base.Dispose();
            rasterizer.Dispose();
        }

        public override void QueueRender() {
            throw new NotImplementedException();
        }

        public override void Render(RenderPipeline pipeline) {
            pipeline.OutputMerger.RenderTarget.SetState(0, destination.View);
            pipeline.OutputMerger.BlendState = blendstate;

            pipeline.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            pipeline.Rasterizer.Viewport = new Viewport(0, 0, destination.Width, destination.Height);
            pipeline.Rasterizer.RasterizerState = rasterizer;

            pipeline.VertexShader.Shader = vs;
            pipeline.PixelShader.Shader = ps;
            pipeline.ParameterManager.ApplyCollection(paramCollection);
            pipeline.ApplyOutputResources();
            pipeline.ApplyShaderResources();
            pipeline.Draw(4, 0);
        }
    }
}
