using CargoEngine;
using CargoEngine.Scene;
using CargoEngine.Shader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using CargoEngine.Texture;
using CargoEngine.Parameter;
using SharpDX.Direct3D11;

namespace Cargo
{
    class ShadowProjectionRenderTask : RenderTask
    {

        private Camera sceneCam, lightCam;
        private CargoEngine.Shader.VertexShader vs;
        private CargoEngine.Shader.PixelShader ps;

        private RenderTarget destination;
        private Texture lightMap, colorMap;

        private ParameterCollection parameterCollection = new ParameterCollection();

        private BlendState blendState;

        public ShadowProjectionRenderTask(Camera sceneCam, Camera lightCam, RenderTarget destination, Texture lightMap, Texture depthMap, Texture normalMap, Texture colorMap) {
            this.sceneCam = sceneCam;
            this.lightCam = lightCam;
            this.destination = destination;
            this.lightMap = lightMap;
            this.colorMap = colorMap;

            vs = Renderer.ShaderLoader.LoadVertexShader("assets/shader/shadowprojection.hlsl", "VSMain");
            ps = Renderer.ShaderLoader.LoadPixelShader("assets/shader/shadowprojection.hlsl", "PSMain");

            parameterCollection.SetParameter("shadowMap", lightMap);
            parameterCollection.SetParameter("depthMap", depthMap);
            parameterCollection.SetParameter("colorMap", colorMap);
            parameterCollection.SetParameter("normalMap", normalMap);
            parameterCollection.SetParameter("samplerClamp", Renderer.Instance.CreateSamplerState(TextureAddressMode.Clamp, Filter.MinMagMipPoint));

            /*            blendState = new BlendState(Renderer.Dev, new BlendStateDescription());
                        blendState.Description.RenderTarget[0]= new RenderTargetBlendDescription {                
                            BlendOperation=BlendOperation.Add
                        }*/
        }

        public override void QueueRender() {
            Renderer.Instance.QueueTask(this);
        }

        public override void Render(RenderPipeline pipeline) {
            pipeline.OutputMerger.ClearDesiredState();
/*            Vector4 orig = new Vector4(100.0f, 50.0f, 200.0f, 1.0f);
            orig = Vector4.Transform(orig, sceneCam.RenderTask.ViewMatrix);
            orig = Vector4.Transform(orig, sceneCam.RenderTask.ProjectionMatrix);
            orig /= orig.W;

            var reverse = Vector4.Transform(orig, sceneCam.InvViewProjectionMatrix);
            reverse /= reverse.W;

            var lightPos = Vector4.Transform(reverse, lightCam.ViewMatrix);
            lightPos = Vector4.Transform(lightPos, lightCam.ProjectionMatrix);
            lightPos /= lightPos.W;*/

            pipeline.OutputMerger.RenderTarget.SetState(0, destination.View);
            pipeline.ApplyOutputResources();
            pipeline.ClearTargets(Color4.Black, 1.0f, 0);
            pipeline.Rasterizer.Viewport = new Viewport(0, 0, destination.Width, destination.Height);
            pipeline.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            pipeline.VertexShader.Shader = vs;
            pipeline.PixelShader.Shader = ps;
            parameterCollection.SetParameter("LightView", lightCam.ViewMatrix);
            parameterCollection.SetParameter("LightProj", lightCam.ProjectionMatrix);
            parameterCollection.SetParameter("InvViewProj", sceneCam.InvViewProjectionMatrix);
            parameterCollection.SetParameter("LightPos", lightCam.Transform.Position);
            pipeline.ParameterManager.ApplyCollection(parameterCollection);
            pipeline.ApplyOutputResources();
            pipeline.ApplyShaderResources();
            pipeline.Draw(3, 0);
        }
    }
}
