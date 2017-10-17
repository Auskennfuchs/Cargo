using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using DXSwapChain = SharpDX.DXGI.SwapChain;
using Resource = SharpDX.Direct3D11.Resource;

namespace CargoEngine
{
    public class SwapChain : Event.EventManager, IDisposable
    {
        private DXSwapChain swapChain;

        public RenderTargetGroup RenderTarget {
            get; private set;
        }

        public Viewport Viewport {
            get { return RenderTarget.Viewport; }
            set { RenderTarget.Viewport = value; }
        }

        public DXSwapChain DXSwapChain {
            get { return swapChain; }
        }

        private int formWidth, formHeight, fullScreenWidth, fullScreenHeight;

        private bool isResizing = false;

        public event EventHandler<Event.SResizeEvent> Resize;

        public SwapChain(Form form, Renderer renderer)
            : this(form, renderer, 0, 0) { }

        public SwapChain(Form form, Renderer renderer, int fullScreenWidth, int fullScreenHeight) {
            if (fullScreenWidth == 0 || fullScreenHeight == 0) {
                fullScreenWidth = SystemInformation.VirtualScreen.Width;
                fullScreenHeight = SystemInformation.VirtualScreen.Height;
            }
            this.fullScreenWidth = fullScreenWidth;
            this.fullScreenHeight = fullScreenHeight;

            formWidth = form.ClientSize.Width;
            formHeight = form.ClientSize.Height;

            var swapChainDescriptor = new SwapChainDescription() {
                BufferCount = 1,
                Usage = Usage.RenderTargetOutput,
                Flags = SwapChainFlags.AllowModeSwitch,
                IsWindowed = true,
                ModeDescription = new ModeDescription(form.ClientSize.Width, form.ClientSize.Height, new Rational(0, 1), Format.R8G8B8A8_UNorm),
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard
            };

            using (var factory = new Factory1()) {
                swapChain = new DXSwapChain(factory, renderer.Device, swapChainDescriptor);
            }
            using (var resource = Resource.FromSwapChain<Texture2D>(swapChain, 0)) {
                RenderTarget = new RenderTargetGroup(resource);
            }

            using (var fac = swapChain.GetParent<Factory>()) {
                fac.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAltEnter);
            }

            form.ResizeBegin += (o, e) => {
                formHeight = ((Form)o).Height;
                formWidth = ((Form)o).Width;
            };
            form.ResizeBegin += (o, e) => {
                isResizing = true;
            };
            form.ResizeEnd += (o, e) => {
                isResizing = false;
                HandleResize(o, e);
            };
            form.KeyDown += HandleKeyDown;

            form.SizeChanged += HandleResize;
        }

        public void Dispose() {
            if (RenderTarget != null) {
                RenderTarget.Dispose();
            }
            if (swapChain != null) {
                swapChain.Dispose();
            }
        }

        public void Present() {
            swapChain.Present(0, PresentFlags.None);
        }

        private void HandleResize(object sender, System.EventArgs e) {
            Form f = (Form)sender;
            if ((f.ClientSize.Width != formWidth || f.ClientSize.Height != formHeight)
                && !isResizing
                && !(f.WindowState == FormWindowState.Minimized)) {
                formWidth = f.ClientSize.Width;
                formHeight = f.ClientSize.Height;

                DoResize(formWidth, formHeight);
            }
        }

        private void HandleKeyDown(object sender, KeyEventArgs e) {
            if (e.Alt && e.KeyCode == Keys.Enter) {
                if (!swapChain.IsFullScreen) {
                    DoResize(fullScreenWidth, fullScreenHeight);
                }
                else {
                    DoResize(formWidth, formHeight);
                }
                swapChain.IsFullScreen = !swapChain.IsFullScreen;
            }
        }

        private void DoResize(int width, int height) {
            var rt = RenderTarget.RenderTargets[0];
            rt.Clear();
            swapChain.ResizeBuffers(1, width, height, Format.Unknown, SwapChainFlags.AllowModeSwitch);
            using (var tex = Resource.FromSwapChain<Texture2D>(swapChain, 0)) {
                rt.Update(null, new RenderTargetView(Renderer.Instance.Device, tex));
            }
            Viewport = new Viewport(0, 0, width, height);
            Resize?.Invoke(this, new Event.SResizeEvent {
                Size = new System.Drawing.Size(width, height)
            });
            ProcessEvent(new Event.EventResize(new Event.SResizeEvent {
                Size = new System.Drawing.Size(width, height)
            }));
        }
    }
}
