using CargoEngine;
using CargoEngine.Scene;
using CargoEngine.Shader;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SwapChain = CargoEngine.SwapChain;

namespace Cargo
{
    class DeferredRenderTask : RenderTask
    {

        private CargoEngine.Shader.VertexShader vsClear, vsRender, vsCombine;
        private CargoEngine.Shader.PixelShader psClear, psRender, psCombine;

        private RenderTargetGroup renderTargets;

        private RasterizerState rasterizerState;

        private DepthStencilState clearDepthStencilState, renderDepthStencilState;

        private SwapChain swapChain;

        public DeferredRenderTask(SwapChain swapChain) {
            this.swapChain = swapChain;
            renderTargets = new RenderTargetGroup(swapChain, Format.R8G8B8A8_UNorm); // Albedo
            renderTargets.AddDepthStencil();
            renderTargets.AddRenderTarget(Format.R8G8B8A8_UNorm); //Normals
            renderTargets.AddRenderTarget(Format.R16G16B16A16_Float); //Position
            Init();
        }

        private void Init() {
            vsClear = ShaderLoader.LoadVertexShader(Renderer.Instance, "assets/shader/deferredCleanup.hlsl", "VSMain");
            psClear = ShaderLoader.LoadPixelShader(Renderer.Instance, "assets/shader/deferredCleanup.hlsl", "PSMain");

            vsRender = ShaderLoader.LoadVertexShader(Renderer.Instance, "assets/shader/deferredRender.hlsl", "VSMain");
            psRender = ShaderLoader.LoadPixelShader(Renderer.Instance, "assets/shader/deferredRender.hlsl", "PSMain");

            vsCombine = ShaderLoader.LoadVertexShader(Renderer.Instance, "assets/shader/deferredCombine.hlsl", "VSMain");
            psCombine = ShaderLoader.LoadPixelShader(Renderer.Instance, "assets/shader/deferredCombine.hlsl", "PSMain");

            var rasterizerStateDescription = RasterizerStateDescription.Default();
            rasterizerStateDescription.CullMode = CullMode.Back;
            rasterizerState = new RasterizerState(Renderer.Instance.Device, rasterizerStateDescription);
            clearDepthStencilState = new DepthStencilState(Renderer.Instance.Device, new DepthStencilStateDescription {
                DepthComparison = Comparison.Always,
                IsDepthEnabled = false,
                IsStencilEnabled = false
            });
            renderDepthStencilState = new DepthStencilState(Renderer.Instance.Device, new DepthStencilStateDescription {
                DepthComparison = Comparison.Less,
                IsDepthEnabled = true,
                IsStencilEnabled = true,
                DepthWriteMask = DepthWriteMask.All,
                StencilReadMask = 0xFF,
                StencilWriteMask = 0xFF,
                FrontFace = new DepthStencilOperationDescription {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Increment,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                },
                BackFace = new DepthStencilOperationDescription {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Decrement,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Always
                }
            });
        }


        public override void QueueRender() {
            Renderer.Instance.QueueTask(this);
        }

        public override void Render(RenderPipeline pipeline) {
            ClearRenderTargets(pipeline);
            RenderScene(pipeline);
            Present(pipeline);
        }

        private void ClearRenderTargets(RenderPipeline pipeline) {
            pipeline.OutputMerger.RenderTarget.SetStates(0, renderTargets.RenderTargets.ToArray());
            pipeline.OutputMerger.DepthStencil = renderTargets.DepthStencilView;
            pipeline.OutputMerger.DepthStencilState = clearDepthStencilState;
            pipeline.VertexShader.Shader = vsClear;
            pipeline.PixelShader.Shader = psClear;
            pipeline.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            pipeline.Rasterizer.RasterizerState = rasterizerState;
            pipeline.Rasterizer.Viewport = renderTargets.Viewport;
            pipeline.ApplyOutputResources();
            pipeline.ApplyInputResources();
            pipeline.ApplyShaderResources();

            pipeline.ClearTargets(new SharpDX.Color4(0.2f, 0.2f, 0.2f, 1.0f), 1.0f, 0);

            pipeline.Draw(3, 0);
        }

        private void RenderScene(RenderPipeline pipeline) {
            pipeline.OutputMerger.RenderTarget.SetStates(0, renderTargets.RenderTargets.ToArray());
            pipeline.OutputMerger.DepthStencilState = renderDepthStencilState;
            pipeline.VertexShader.Shader = vsRender;
            pipeline.PixelShader.Shader = psRender;
            pipeline.ApplyOutputResources();
            pipeline.ParameterManager.SetViewMatrix(ViewMatrix);
            pipeline.ParameterManager.SetProjectionMatrix(ProjectionMatrix);
            if (Scene != null) {
                Scene.Render(pipeline);
            }
        }

        private void Present(RenderPipeline pipeline) {
            pipeline.OutputMerger.ClearDesiredState();
            pipeline.OutputMerger.RenderTarget.SetState(0, swapChain.RenderTarget.View);
            pipeline.OutputMerger.DepthStencilState = clearDepthStencilState;
            pipeline.VertexShader.Shader = vsCombine;
            pipeline.PixelShader.Shader = psCombine;
            pipeline.ParameterManager.SetParameter("AlbedoTexture", renderTargets.ShaderResourceViews[0]);
            //            pipeline.ParameterManager.SetParameter("NormalTextureInput", renderTargets.ShaderResourceViews[1]);
            pipeline.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            pipeline.ApplyOutputResources();
            pipeline.ApplyInputResources();
            pipeline.ApplyShaderResources();
            pipeline.Draw(3, 0);
        }
    }
}
