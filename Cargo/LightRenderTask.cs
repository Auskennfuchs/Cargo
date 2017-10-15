using CargoEngine;
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
        private SamplerState sampler;

        private Vector3 lightDir = new Vector3(1.0f, -1.0f, 0.0f);
        private Color3 lightColor = new Color3(1.0f, 0.5f, 0.8f);
        private Color3 ambientColor = new Color3(0.2f, 0.2f, 0.2f);

        public LightRenderTask(RenderTarget lightTarget, RenderTarget normalTarget, RenderTarget positionTarget) {
            this.lightTarget = lightTarget;
            this.normalTarget = normalTarget;
            this.positionTarget = positionTarget;
            vsDirectional = ShaderLoader.LoadVertexShader(Renderer.Instance, "assets/shader/directionalLight.hlsl", "VSMain");
            psDirectional = ShaderLoader.LoadPixelShader(Renderer.Instance, "assets/shader/directionalLight.hlsl", "PSMain");

            sampler = Renderer.Instance.CreateSamplerState(TextureAddressMode.Wrap, Filter.Anisotropic, 16);
        }


        public override void QueueRender() {
        }

        public override void Render(RenderPipeline pipeline) {
            pipeline.OutputMerger.ClearDesiredState();
            pipeline.InputAssembler.ClearDesiredState();
            pipeline.OutputMerger.RenderTarget.SetState(0, lightTarget.View);
            pipeline.VertexShader.Shader = vsDirectional;
            pipeline.PixelShader.Shader = psDirectional;
            pipeline.PixelShader.Sampler.SetState(0, sampler);
            pipeline.ParameterManager.SetParameter("NormalTextureInput", normalTarget.SRV);
            pipeline.ParameterManager.SetParameter("PositionTextureInput", positionTarget.SRV);
            pipeline.ParameterManager.SetParameter("lightDir", lightDir);
            pipeline.ParameterManager.SetParameter("lightColor", lightColor);
            pipeline.ParameterManager.SetParameter("ambientColor", ambientColor);

            pipeline.ApplyOutputResources();
            pipeline.ApplyShaderResources();

            pipeline.Draw(3, 0);
        }
    }
}
