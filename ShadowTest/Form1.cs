using CargoEngine;
using CargoEngine.Event;
using CargoEngine.Geometry;
using CargoEngine.Scene;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShadowTest
{
    public partial class Form1 : Form
    {

        private Renderer renderer;
        private CargoEngine.SwapChain swapChain;

        private Scene scene;

        private EventManager eventManager = new EventManager();

        private int fpsCount;
        private float fpsTimeCount;
        private CargoEngine.Timer timer = new CargoEngine.Timer();

        private Camera cam, shadowCam;

        float sinCount = 0;

        public Form1() {
            InitializeComponent();
            InitEngine();
        }

        private void InitEngine() {
            renderer = new Renderer();
            swapChain = new CargoEngine.SwapChain(this, renderer);
            swapChain.RenderTarget.AddDepthStencil();

            AddEvents();

            scene = new Scene();

            shadowCam = new Camera();
            shadowCam.Transform.SetLookAt(new Vector3(50.0f, 200.0f, 256.0f), new Vector3(256.0f, 0.0f, 256.0f), Vector3.ForwardLH);
            shadowCam.SetProjection(0.1f, 500.0f, 1.0f, (float)Math.PI / 4.0f);
            shadowCam.RenderTask = new ShadowMapRenderTask();
            shadowCam.Scene = scene;
                       shadowCam.AddComponent(new LightVisualizer(shadowCam));
            scene.RootNode.AddChild(shadowCam);

            cam = new Camera();
            cam.Transform.Position = new Vector3(0, 50.0f, 0.0f);
            cam.SetProjection(0.1f, 1000.0f, (float)this.Width / (float)this.Height, (float)Math.PI / 4.0f);
            cam.RenderTask = new BasicRenderTask(swapChain.RenderTarget, ((ShadowMapRenderTask)shadowCam.RenderTask).srv, shadowCam);
            cam.Scene = scene;
            cam.AddComponent(new FreeLookComponent(eventManager));
            cam.AddComponent(new FreeMoveComponent(eventManager) {
                Speed = 20.0f
            });
            scene.RootNode.AddChild(cam);

            var terrain = new Terrain();
            scene.RootNode.AddChild(terrain);

            var sphere = new Sphere();
            sphere.Transform.Position = new Vector3(256, 60, 256);
            sphere.Transform.Scale = new Vector3(10, 10, 10);
            scene.RootNode.AddChild(sphere);

            sphere = new Sphere();
            sphere.Transform.Position = new Vector3(200, 60, 256);
            sphere.Transform.Scale = new Vector3(15, 15, 15);
            scene.RootNode.AddChild(sphere);

            var cube = new Cube();
            cube.Transform.Position = new Vector3(150, 60, 256);
            cube.Transform.Scale = new Vector3(15, 15, 15);
            scene.RootNode.AddChild(cube);

            timer.Start();
        }

        public void MainLoop() {
            fpsCount++;
            float elapsed = timer.Restart();
            fpsTimeCount += elapsed;
            if (fpsTimeCount > 1.0f) {
                //                var fxaa = ((DeferredRenderTask)cam.RenderTask).FXAA;
                var fps = fpsCount / fpsTimeCount;
                var renderTime = (fpsTimeCount / fpsCount * 1000.0f);
                this.Text = renderTime.ToString() + "ms (" + fps.ToString() + " fps)";
                fpsTimeCount = 0;
                fpsCount = 0;
            }
            sinCount += elapsed;
            shadowCam.Transform.SetLookAt(new Vector3(256+150.0f*(float)Math.Sin(sinCount), 200.0f, 256.0f), new Vector3(256.0f, 0.0f, 256.0f), Vector3.ForwardLH);
            scene.Update(elapsed);
            shadowCam.QueueRender();
            cam.QueueRender();
            renderer.ExecuteTasks();
            swapChain.Present();
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            scene.Clear();
            swapChain.Dispose();
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
