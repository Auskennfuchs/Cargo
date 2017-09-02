using CargoEngine;
using SharpDX.Direct3D11;
using SharpDX;
using SharpDX.DXGI;
using VertexShader = CargoEngine.Shader.VertexShader;
using PixelShader = CargoEngine.Shader.PixelShader;
using CargoEngine.Shader;

namespace Cargo {
    class SimpleRenderTask : RenderTask {

        private RenderTarget renderTarget;

        private Geometry triangle;

        private VertexShader vShader;
        private PixelShader pShader;

        private RasterizerState rasterizerState;

        public SimpleRenderTask(RenderTarget rt) {
            var tris = new Vector3[3] {
                new Vector3(-1.0f,-1.0f,1.0f),
                new Vector3( 0.0f, 1.0f,1.0f),
                new Vector3( 1.0f,-1.0f,1.0f),
            };
            renderTarget = rt;

            triangle = new Geometry();
            triangle.AddBuffer(Buffer.Create<Vector3>(Renderer.Instance.Device, BindFlags.VertexBuffer, tris), "POSITION", Format.R32G32B32_Float, Utilities.SizeOf<Vector3>());

            vShader = ShaderLoader.LoadVertexShader(Renderer.Instance, "assets/shader/simple.hlsl", "VSMain");
            pShader = ShaderLoader.LoadPixelShader(Renderer.Instance, "assets/shader/simple.hlsl", "PSMain");

            var rasterizerStateDescription = RasterizerStateDescription.Default();
            rasterizerStateDescription.CullMode = CullMode.None;
            rasterizerState = new RasterizerState(Renderer.Instance.Device, rasterizerStateDescription);
        }

        ~SimpleRenderTask() {
            vShader.Dispose();
            pShader.Dispose();
        }

        public override void Render(RenderPipeline pipeline) {
            pipeline.OutputMerger.ClearDesiredState();
            pipeline.OutputMerger.RenderTarget[0] = renderTarget.View;
            pipeline.OutputMerger.ApplyRenderTargets(pipeline.DevContext);
            pipeline.ClearTargets(new SharpDX.Color4(0.0f, 0.0f, 0.0f, 1.0f), 1.0f, 0);
            pipeline.Rasterizer.RasterizerState = rasterizerState;
            pipeline.Rasterizer.Viewport = renderTarget.Viewport;
            pipeline.VertexShader.Shader = vShader;
            pipeline.PixelShader.Shader = pShader;

            var inputLayout = Renderer.Instance.GetInputLayout(vShader);
            if(inputLayout==null) {
                inputLayout = Renderer.Instance.AddInputLayout(vShader, triangle.Elements.ToArray());
            }

            triangle.Apply(pipeline);

            pipeline.InputAssembler.InputLayout = inputLayout;
            pipeline.ApplyInputResources();
            pipeline.ApplyShaderResources();
            pipeline.Draw(3, 0);
        }
    }
}
