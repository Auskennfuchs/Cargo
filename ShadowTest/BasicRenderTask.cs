using CargoEngine;
using CargoEngine.Parameter;
using CargoEngine.Scene;
using CargoEngine.Shader;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowTest
{
    class BasicRenderTask : RenderTask
    {

        private CargoEngine.Shader.VertexShader vs;
        private CargoEngine.Shader.PixelShader ps;

        private RenderTargetGroup destination;

        private ParameterCollection parameterCollection;
        private Camera shadowCam;

        public BasicRenderTask(RenderTargetGroup dest, ShaderResourceView srv, Camera shadowCam) {
            destination = dest;
            this.shadowCam = shadowCam;
            vs = Renderer.ShaderLoader.LoadVertexShader("assets/shader/scenerender.hlsl", "VSMain");
            ps = Renderer.ShaderLoader.LoadPixelShader("assets/shader/scenerender.hlsl", "PSMain");

            parameterCollection = new ParameterCollection();
            parameterCollection.SetParameter("ShadowMap", srv);
            var shadowSampler = Renderer.Instance.CreateSamplerState(TextureAddressMode.Clamp, Filter.MinMagMipPoint);
            parameterCollection.SetParameter("ShadowSampler", shadowSampler);
        }

        public override void QueueRender() {
            Renderer.Instance.QueueTask(this);
        }

        public override void Render(RenderPipeline pipeline) {
            pipeline.OutputMerger.RenderTarget.SetState(0, destination.RenderTargets[0].View);
            pipeline.OutputMerger.DepthStencil = destination.DepthStencilView;
            pipeline.OutputMerger.DepthStencilState = destination.DepthStencilState;            
            pipeline.Rasterizer.Viewport = destination.Viewport;
            pipeline.VertexShader.Shader = vs;
            pipeline.PixelShader.Shader = ps;
            pipeline.ParameterManager.SetViewMatrix(ViewMatrix);
            pipeline.ParameterManager.SetProjectionMatrix(ProjectionMatrix);
            pipeline.ApplyOutputResources();
            pipeline.ClearTargets(new Color4(0.0f,0.0f,0.5f,1.0f), 1.0f, 0);

            parameterCollection.SetParameter("LightView", shadowCam.ViewMatrix);
            parameterCollection.SetParameter("LightProj", shadowCam.ProjectionMatrix);
            parameterCollection.SetParameter("LightPos", shadowCam.Transform.Position);
            //            parameterCollection.SetParameter("InvViewProj", this.c.InvViewProjectionMatrix);
            pipeline.ParameterManager.ApplyCollection(parameterCollection);

            if (Scene != null) {
                Scene.Render(pipeline);
            }
        }
    }
}
