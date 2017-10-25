using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CargoEngine
{

    public class RenderTarget : Texture.Texture
    {
        public RenderTargetView View {
            get; private set;
        }

        public event EventHandler<Event.SResizeEvent> OnResize;

        public RenderTarget(ShaderResourceView srv, RenderTargetView rtv) {
            using (var tex = rtv.ResourceAs<Texture2D>()) {
                Dimension = Texture.Dimension.Texture2D;
                Width = tex.Description.Width;
                Height = tex.Description.Height;
                Format = tex.Description.Format;
            }
            Update(srv, rtv);
        }

        public RenderTarget(Texture2D tex) {
            Dimension = Texture.Dimension.Texture2D;
            Width = tex.Description.Width;
            Height = tex.Description.Height;
            Format = tex.Description.Format;
            View = new RenderTargetView(Renderer.Instance.Device, tex);
            if (tex.Description.BindFlags.HasFlag(BindFlags.ShaderResource)) {
                SRV = new ShaderResourceView(Renderer.Instance.Device, tex);
            }
        }

        public RenderTarget(int width, int height, Format format) {
            Dimension = Texture.Dimension.Texture2D;
            Width = width;
            Height = height;
            Format = format;
            using (var tex = CreateRenderTargetTexture()) {
                View = new RenderTargetView(Renderer.Instance.Device, tex);
                SRV = new ShaderResourceView(Renderer.Instance.Device, tex);
            }
        }

        public override void Dispose() {
            base.Dispose();
            Clear();
        }

        public void Clear() {
            if (View != null) {
                View.Dispose();
                View = null;
            }
            if (SRV != null) {
                SRV.Dispose();
                SRV = null;
            }
        }

        public void Resize(int newWidth, int newHeight) {
            Clear();
            Width = newWidth;
            Height = newHeight;
            using (var tex = CreateRenderTargetTexture()) {
                View = new RenderTargetView(Renderer.Instance.Device, tex);
                SRV = new ShaderResourceView(Renderer.Instance.Device, tex);
            }
        }

        public void SendResizeEvent() {
            OnResize?.Invoke(this, new Event.SResizeEvent {
                Size = new System.Drawing.Size(Width, Height)
            });
        }

        public void Update(ShaderResourceView srv, RenderTargetView rtv) {
            Clear();
            using (var tex = rtv.ResourceAs<Texture2D>()) {
                Width = tex.Description.Width;
                Height = tex.Description.Height;
                Format = tex.Description.Format;
                SRV = srv;
                View = rtv;
            }
        }

        private Texture2D CreateRenderTargetTexture() {
            return new Texture2D(Renderer.Instance.Device, new Texture2DDescription() {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format,
                Height = Height,
                Width = Width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1, 0) // no MSAA
            });
        }
    }

    public class RenderTargetGroup : IDisposable
    {

        public List<RenderTarget> RenderTargets {
            get; private set;
        } = new List<RenderTarget>();

        public DepthStencilView DepthStencilView {
            get; private set;
        }

        public DepthStencilState DepthStencilState {
            get; private set;
        }

        public int Width {
            get; private set;
        }

        public int Height {
            get; private set;
        }

        public Viewport Viewport {
            get; set;
        }

        public RenderTargetGroup(int width, int height, Format firstFormat) {
            Width = width;
            Height = height;
            AddRenderTarget(firstFormat);

            Viewport = new Viewport(0, 0, width, height);
        }

        public RenderTargetGroup(SwapChain swapChain, Format firstFormat) : this(swapChain.Viewport.Width, swapChain.Viewport.Height, firstFormat) {
            swapChain.OnResize += (o, e) => {
                Resize(e.Size.Width, e.Size.Height);
            };
        }

        public RenderTargetGroup(SwapChain swapChain, Texture2D tex) : this(tex) {
            swapChain.OnResize += (o, e) => {
                Resize(e.Size.Width, e.Size.Height);
            };
        }

        public RenderTargetGroup(Texture2D tex) {
            Width = tex.Description.Width;
            Height = tex.Description.Height;
            Viewport = new Viewport(0, 0, Width, Height);
            AddRenderTarget(tex);
        }

        public void AddRenderTarget(Format format) {
            var rtv = new RenderTarget(Width, Height, format);
            RenderTargets.Add(rtv);
        }

        public void AddRenderTarget(Texture2D tex) {
            var rtv = new RenderTarget(tex);
            RenderTargets.Add(rtv);
        }

        private Texture2D CreateRenderTargetTexture(Format format) {
            return new Texture2D(Renderer.Instance.Device, new Texture2DDescription() {
                ArraySize = 1,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = format,
                Height = Height,
                Width = Width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1, 0) // no MSAA
            });
        }

        public void Dispose() {
            foreach (var rt in RenderTargets) {
                rt.Dispose();
            }
            RenderTargets.Clear();
            DepthStencilState?.Dispose();
            DepthStencilView?.Dispose();
        }

        public void Deactivate(RenderPipeline pipeline) {
            var rts = new RenderTargetView[RenderTargets.Count];
            pipeline.DevContext.OutputMerger.SetRenderTargets(null, rts);

        }

        public void AddDepthStencil() {
            var depthBufferDescription = new Texture2DDescription {
                Format = SharpDX.DXGI.Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = Width,
                Height = Height,
                SampleDescription = new SampleDescription(1, 0),
                BindFlags = BindFlags.DepthStencil,
            };
            var depthStencilViewDescription = new DepthStencilViewDescription {
                Dimension = DepthStencilViewDimension.Texture2D
            };
            var depthStencilStateDescription = new DepthStencilStateDescription {
                IsDepthEnabled = true,
                DepthComparison = Comparison.Less,
                DepthWriteMask = DepthWriteMask.All,
                IsStencilEnabled = false,
                StencilReadMask = 0xff,
                StencilWriteMask = 0xff,
                FrontFace = new DepthStencilOperationDescription {
                    Comparison = Comparison.Always,
                    PassOperation = StencilOperation.Keep,
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Increment
                },
                BackFace = new DepthStencilOperationDescription {
                    Comparison = Comparison.Always,
                    PassOperation = StencilOperation.Keep,
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Decrement
                }
            };
            using (var depthBuffer = new Texture2D(Renderer.Instance.Device, depthBufferDescription)) {
                DepthStencilState = new DepthStencilState(Renderer.Instance.Device, depthStencilStateDescription);
                DepthStencilView = new DepthStencilView(Renderer.Instance.Device, depthBuffer, depthStencilViewDescription);
            }
        }

        public void Resize(int newWidth, int newHeight) {
            Width = newWidth;
            Height = newHeight;
            Viewport = new Viewport(0, 0, Width, Height);
            foreach (var rt in RenderTargets) {
                rt.Resize(newWidth, newHeight);
            }
            if (DepthStencilView != null) {
                DepthStencilView.Dispose();
                AddDepthStencil();
            }
        }

        public void UpdateSlot(int slot, ShaderResourceView srv, RenderTargetView rtv) {
            var rt = RenderTargets[slot];
            rt.Update(srv, rtv);
        }

        public RenderTargetView[] GetRenderTargetViews() {
            return RenderTargets.Select(rt => rt.View).ToArray();
        }
    }
}
