using CargoEngine;
using CargoEngine.Parameter;
using CargoEngine.Shader;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cargo
{
    class PostProcessFXAA : RenderTask
    {
        private VertexShader vs;
        private PixelShader ps;
        private RenderTarget destination;

        private ParameterCollection paramCollection = new ParameterCollection();

        public PostProcessFXAA(RenderTarget dest, RenderTarget source) {
            destination = dest;
            vs = Renderer.ShaderLoader.LoadVertexShader("assets/shader/fxaa.hlsl", "FxaaVertexShader");
            ps = Renderer.ShaderLoader.LoadPixelShader("assets/shader/fxaa.hlsl", "FxaaPixelShader");
            paramCollection.SetParameter("rcpFrameInput", new Vector4(1.0f / destination.Width, 1.0f / destination.Height, 0, 0));
            paramCollection.SetParameter("tex.smpl", Renderer.Instance.CreateSamplerState(SharpDX.Direct3D11.TextureAddressMode.Wrap, SharpDX.Direct3D11.Filter.MinMagMipPoint, 1));
            paramCollection.SetParameter("tex.tex", source.SRV);
        }

        public override void QueueRender() {
            throw new NotImplementedException();
        }

        public override void Render(RenderPipeline pipeline) {
            pipeline.OutputMerger.RenderTarget.SetState(0, destination.View);

            pipeline.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            pipeline.Rasterizer.Viewport = new Viewport(0, 0, destination.Width, destination.Height);

            pipeline.VertexShader.Shader = vs;
            pipeline.PixelShader.Shader = ps;
            pipeline.ParameterManager.ApplyCollection(paramCollection);
            pipeline.ApplyOutputResources();
            pipeline.ApplyShaderResources();
            pipeline.Draw(3, 0);
        }
    }
}
