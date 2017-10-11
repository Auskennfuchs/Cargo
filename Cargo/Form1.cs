using System;
using System.Windows.Forms;
using CargoEngine;
using CargoEngine.Event;
using CargoEngine.Scene;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Cargo {
    public partial class Form1 : Form {

        private Renderer renderer;
        private CargoEngine.SwapChain swapChain;

        private Scene scene;

        private Camera cam;

        private RasterizerState rasterizerState;

        private CargoEngine.Timer timer = new CargoEngine.Timer();

        private EventManager eventManager = new EventManager();

        private int fpsCount;
        private float fpsTimeCount;

        public Form1() {
            InitializeComponent();
            renderer = new Renderer();
            swapChain = new CargoEngine.SwapChain(this, renderer);
            swapChain.RenderTarget.AddDepthStencil();

            scene = new Scene();
            var tris = new Vector3[4] {
                new Vector3(-1.0f, 1.0f,1.0f),
                new Vector3(-1.0f,-1.0f,1.0f),
                new Vector3( 1.0f,-1.0f,1.0f),
                new Vector3( 1.0f, 1.0f,1.0f),
            };

            var indices = new short[6] {
                0,2,1,
                0,3,2
            };
            var geo = new GeometryComponent();
            geo.Executor.Geometry.AddBuffer(Buffer.Create<Vector3>(Renderer.Instance.Device, BindFlags.VertexBuffer, tris), "POSITION", Format.R32G32B32_Float, Utilities.SizeOf<Vector3>());
            geo.Executor.Geometry.SetIndexBuffer(Buffer.Create<short>(Renderer.Instance.Device, BindFlags.IndexBuffer, indices), indices.Length);
            var node = new SceneNode();
            node.AddComponent(geo);
            scene.RootNode.AddChild(node);
            geo.Transform.Position += new Vector3(0.0f, 0, 0);

            var terrain = new Terrain();
            scene.RootNode.AddChild(terrain);
            terrain = new Terrain();
            terrain.Transform.Position = new Vector3(0, 0, -255.0f);
            scene.RootNode.AddChild(terrain);
            terrain = new Terrain();
            terrain.Transform.Position = new Vector3(-255.0f, 0, 0f);
            scene.RootNode.AddChild(terrain);
            terrain = new Terrain();
            terrain.Transform.Position = new Vector3(-255.0f, 0, -255.0f);
            scene.RootNode.AddChild(terrain);


            cam = new Camera();
            cam.Transform.Position = new Vector3(0, 0, -10.0f);
            cam.SetProjection(0.1f, 1000.0f, (float)this.Width / (float)this.Height, (float)Math.PI / 4.0f);
            cam.RenderTask = new SimpleRenderTask(swapChain.RenderTarget);
            cam.Scene = scene;
            cam.AddComponent(new FreeLookComponent(eventManager));
            cam.AddComponent(new FreeMoveComponent(eventManager) {
                Speed=10.0f
            });
            scene.RootNode.AddChild(cam);

            var rasterizerStateDescription = RasterizerStateDescription.Default();
            rasterizerStateDescription.CullMode = CullMode.None;
            rasterizerState = new RasterizerState(Renderer.Instance.Device, rasterizerStateDescription);

            timer.Start();
            AddEvents();
        }

        public void MainLoop() {
            fpsCount++;
            float elapsed = timer.Restart();
            fpsTimeCount += elapsed;
            if (fpsTimeCount > 1.0f) {
                var fps = fpsCount / fpsTimeCount;
                var renderTime = (fpsTimeCount / fpsCount * 1000.0f);
                this.Text = renderTime.ToString() + "ms (" + fps.ToString() + " fps)";
                fpsTimeCount = 0;
                fpsCount = 0;
            }
            scene.Update(elapsed);
            cam.QueueRender();
            renderer.ExecuteTasks();
            swapChain.Present();
        }

        private void OnClose(object sender, FormClosingEventArgs e) {
            scene.Clear();
            renderer.Dispose();
        }

        private void AddEvents() {
            this.KeyDown += (o, e) => {
                eventManager.ProcessEvent(new KeyDownEvent(new SKeyEvent() {
                    KeyCode = e.KeyCode,
                    Alt = e.Alt,
                    Control = e.Control,
                    Shift = e.Shift
                }));
            };
            this.KeyUp += (o, e) => {
                eventManager.ProcessEvent(new KeyUpEvent(new SKeyEvent() {
                    KeyCode = e.KeyCode,
                    Alt = e.Alt,
                    Control = e.Control,
                    Shift = e.Shift
                }));
            };

            this.MouseDown += (o, e) => {
                eventManager.ProcessEvent(new MouseDownEvent(new SMouseEvent() {
                    Position = new Point(e.Location.X, e.Location.Y),
                    Button = e.Button
                }));
            };
            this.MouseUp += (o, e) => {
                eventManager.ProcessEvent(new MouseUpEvent(new SMouseEvent() {
                    Position = new Point(e.Location.X,e.Location.Y),
                    Button = e.Button
                }));
            };
            this.MouseMove += (o, e) => {
                eventManager.ProcessEvent(new MouseMoveEvent(new SMouseEvent() {
                    Position = new Point(e.Location.X, e.Location.Y)
                }));
            };
        }
    }
}
