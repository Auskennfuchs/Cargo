using CargoEngine;
using CargoEngine.Shader;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowTest
{
    class ShadowMapRenderTask : RenderTask
    {

        public RenderTargetGroup RenderTargets;
        private VertexShader vs;
        private PixelShader ps;

        public CargoEngine.Texture.Texture2D shadowMap;
        private SharpDX.Direct3D11.DepthStencilView dsv;
        public SharpDX.Direct3D11.ShaderResourceView srv;

        public ShadowMapRenderTask() {
            RenderTargets = new RenderTargetGroup(1024, 1024, SharpDX.DXGI.Format.R32G32B32A32_Float);
            RenderTargets.AddDepthStencil();
            vs = Renderer.ShaderLoader.LoadVertexShader("assets/shader/shadowmap.hlsl", "VSMain");
            ps = Renderer.ShaderLoader.LoadPixelShader("assets/shader/shadowmap.hlsl", "PSMain");

            var texDesc = new SharpDX.Direct3D11.Texture2DDescription {
                Width = 1024,
                Height = 1024,
                MipLevels = 1,
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.R32_Typeless,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = SharpDX.Direct3D11.ResourceUsage.Default,
                BindFlags = SharpDX.Direct3D11.BindFlags.DepthStencil | SharpDX.Direct3D11.BindFlags.ShaderResource,
                CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None,
            };

            var depthDesc = new SharpDX.Direct3D11.DepthStencilViewDescription {
                Format = SharpDX.DXGI.Format.D32_Float,
                Dimension = SharpDX.Direct3D11.DepthStencilViewDimension.Texture2D,
                Texture2D = new SharpDX.Direct3D11.DepthStencilViewDescription.Texture2DResource {
                    MipSlice = 0,
                },
            };

            var srvDesc = new SharpDX.Direct3D11.ShaderResourceViewDescription {
                Format = SharpDX.DXGI.Format.R32_Float,
                Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D,
                Texture2D = new SharpDX.Direct3D11.ShaderResourceViewDescription.Texture2DResource {
                    MipLevels = texDesc.MipLevels,
                    MostDetailedMip = 0,
                },
            };
            using (var sMap = new SharpDX.Direct3D11.Texture2D(Renderer.Dev, texDesc)) {
                dsv = new SharpDX.Direct3D11.DepthStencilView(Renderer.Dev, sMap, depthDesc);
                srv = new SharpDX.Direct3D11.ShaderResourceView(Renderer.Dev, sMap, srvDesc);
                shadowMap = new CargoEngine.Texture.Texture2D(sMap, srv);
            }
        }

        public override void Dispose() {
            RenderTargets.Dispose();
            shadowMap?.Dispose();
            dsv?.Dispose();
            srv?.Dispose();
        }

        public override void Render(RenderPipeline pipeline) {
//            pipeline.OutputMerger.RenderTarget.SetState(0, RenderTargets.RenderTargets[0].View);
            pipeline.OutputMerger.DepthStencil = dsv;
//            pipeline.OutputMerger.DepthStencilState = Renderer.Dev.;
            pipeline.ApplyOutputResources();
            pipeline.ClearTargets(Color4.Black, 1.0f, 0);
            pipeline.VertexShader.Shader = vs;
            pipeline.PixelShader.Shader = ps;
            pipeline.ParameterManager.SetViewMatrix(ViewMatrix);
            pipeline.ParameterManager.SetProjectionMatrix(ProjectionMatrix);
            pipeline.Rasterizer.Viewport = new Viewport(0, 0, RenderTargets.RenderTargets[0].Width, RenderTargets.RenderTargets[0].Height);

            if (Scene != null) {
                Scene.Render(pipeline);
            }
        }

        public override void QueueRender() {
            Renderer.Instance.QueueTask(this);
        }
    }
}
