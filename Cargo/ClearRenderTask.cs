using CargoEngine;
using CargoEngine.Shader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cargo
{
    class ClearRenderTask : RenderTask
    {
        private CargoEngine.Shader.VertexShader vsClear;
        private CargoEngine.Shader.PixelShader psClear;

        RenderTargetGroup renderTargets;

        public ClearRenderTask(RenderTargetGroup renderTargets) {
            this.renderTargets = renderTargets;
            vsClear = ShaderLoader.LoadVertexShader(Renderer.Instance, "assets/shader/deferredCleanup.hlsl", "VSMain");
            psClear = ShaderLoader.LoadPixelShader(Renderer.Instance, "assets/shader/deferredCleanup.hlsl", "PSMain");
        }

        public override void QueueRender() {
        }

        public override void Render(RenderPipeline pipeline) {
            pipeline.OutputMerger.RenderTarget.SetStates(0, renderTargets.GetRenderTargetViews());
            pipeline.OutputMerger.DepthStencil = renderTargets.DepthStencilView;
            pipeline.OutputMerger.DepthStencilState = pipeline.OutputMerger.NoDepthStencilState;
            pipeline.VertexShader.Shader = vsClear;
            pipeline.PixelShader.Shader = psClear;
            pipeline.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            pipeline.Rasterizer.Viewport = renderTargets.Viewport;
            pipeline.ApplyOutputResources();
            pipeline.ApplyInputResources();
            pipeline.ApplyShaderResources();

            pipeline.ClearTargets(new SharpDX.Color4(0.2f, 0.2f, 0.2f, 1.0f), 1.0f, 0);

            pipeline.Draw(3, 0);
        }
    }
}
