using CargoEngine;
using CargoEngine.Parameter;
using CargoEngine.Shader;
using SharpDX;
using SharpDX.Direct3D11;

namespace Cargo
{
    class LightRenderTask : RenderTask
    {

        private CargoEngine.Shader.VertexShader vsDirectional;
        private CargoEngine.Shader.PixelShader psDirectional;
        private RenderTarget lightTarget, normalTarget, positionTarget;

        private ParameterCollection parameterCollection = new ParameterCollection();

        public LightRenderTask(RenderTarget lightTarget, RenderTarget normalTarget, RenderTarget positionTarget) {
            this.lightTarget = lightTarget;
            this.normalTarget = normalTarget;
            this.positionTarget = positionTarget;
            vsDirectional = Renderer.Instance.Shaders.LoadVertexShader("assets/shader/directionalLight.hlsl", "VSMain");
            psDirectional = Renderer.Instance.Shaders.LoadPixelShader("assets/shader/directionalLight.hlsl", "PSMain");

            parameterCollection.SetParameter("lightDir", new Vector3(0.0f, -1.0f, -1.0f));
            parameterCollection.SetParameter("lightColor", new Color3(1.0f, 1.0f, 1.0f));
            parameterCollection.SetParameter("ambientColor", new Color3(0.2f, 0.2f, 0.2f));
            parameterCollection.SetParameter("NormalTextureInput", normalTarget);
            parameterCollection.SetParameter("PositionTextureInput", positionTarget);

        }

        public override void QueueRender() {
            Renderer.Instance.QueueTask(this);
        }

        public override void Render(RenderPipeline pipeline) {
            pipeline.OutputMerger.RenderTarget.SetState(0, lightTarget.View);

            pipeline.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            pipeline.Rasterizer.Viewport = new Viewport(0, 0, lightTarget.Width, lightTarget.Height);

            pipeline.VertexShader.Shader = vsDirectional;
            pipeline.PixelShader.Shader = psDirectional;
            pipeline.ParameterManager.ApplyCollection(parameterCollection);
            pipeline.ApplyOutputResources();
            pipeline.ApplyShaderResources();

            pipeline.Draw(3, 0);
        }
    }
}
