using System;
using System.Windows.Forms;
using CargoEngine;
using CargoEngine.Event;
using CargoEngine.Scene;
using SharpDX;

namespace Cargo
{
    public partial class Form1 : Form
    {

        private Renderer renderer;
        private CargoEngine.SwapChain swapChain;

        private Scene scene;

        private Camera cam;

        private CargoEngine.Timer timer = new CargoEngine.Timer();

        private EventManager eventManager = new EventManager();

        private int fpsCount;
        private float fpsTimeCount;

        public Form1() {
            InitializeComponent();
            renderer = new Renderer();
            swapChain = new CargoEngine.SwapChain(this, renderer);

            scene = new Scene();

            var terrain = new Terrain();
            scene.RootNode.AddChild(terrain);
//            terrain.Transform.Scale = new Vector3(3.0f, 1.0f, 3.0f);


            cam = new Camera();
            cam.Transform.Position = new Vector3(0, 0, -10.0f);
            cam.SetProjection(0.1f, 1000.0f, (float)this.Width / (float)this.Height, (float)Math.PI / 4.0f);
            cam.RenderTask = new DeferredRenderTask(swapChain);
            cam.Scene = scene;
            cam.AddComponent(new FreeLookComponent(eventManager));
            cam.AddComponent(new FreeMoveComponent(eventManager) {
                Speed = 10.0f
            });
            scene.RootNode.AddChild(cam);

            timer.Start();
            AddEvents();

            var fl = cam.GetComponent<FreeLookComponent>();
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
                    Position = new Point(e.Location.X, e.Location.Y),
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
