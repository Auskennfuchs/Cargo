﻿using System;
using System.Windows.Forms;
using CargoEngine;
using CargoEngine.Event;
using CargoEngine.Scene;
using SharpDX;
using CargoEngine.Geometry;

namespace Cargo
{
    public partial class Form1 : Form
    {

        private Renderer renderer;
        private CargoEngine.SwapChain swapChain;

        private Scene scene;

        private Camera cam, shadowCam;

        private CargoEngine.Timer timer = new CargoEngine.Timer();

        private EventManager eventManager = new EventManager();

        private int fpsCount;
        private float fpsTimeCount;

        private RenderTask shadowProjectionRenderTask;

        public Form1() {
            InitializeComponent();
            renderer = new Renderer();
            swapChain = new CargoEngine.SwapChain(this, renderer);
            swapChain.RenderTarget.AddDepthStencil();

            scene = new Scene();

            var terrain = new Terrain();
            scene.RootNode.AddChild(terrain);
            //            terrain.Transform.Scale = new Vector3(3.0f, 3.0f, 3.0f);

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

            cam = new Camera();
            cam.Transform.Position = new Vector3(0, 50.0f, 0.0f);
            cam.SetProjection(0.1f, 1000.0f, (float)this.Width / (float)this.Height, (float)Math.PI / 4.0f);
            cam.RenderTask = new DeferredRenderTask(swapChain);
            cam.Scene = scene;
            cam.AddComponent(new FreeLookComponent(eventManager));
            cam.AddComponent(new FreeMoveComponent(eventManager) {
                Speed = 20.0f
            });
            scene.RootNode.AddChild(cam);

            shadowCam = new Camera();
            shadowCam.Transform.SetLookAt(new Vector3(100.0f, 100.0f, 256.0f),new Vector3(256.0f,0.0f,256.0f), Vector3.Up);
            shadowCam.SetProjection(0.1f, 200.0f, 1.0f, (float)Math.PI / 2.0f);
            shadowCam.RenderTask = new ShadowMapRenderTask();
            shadowCam.Scene = scene;
            shadowCam.AddComponent(new LightVisualizer(shadowCam));

            scene.RootNode.AddChild(shadowCam);

            shadowProjectionRenderTask = new ShadowProjectionRenderTask(cam, shadowCam, 
                swapChain.RenderTarget.RenderTargets[0], 
                ((ShadowMapRenderTask)shadowCam.RenderTask).shadowMap, 
                ((DeferredRenderTask)cam.RenderTask).RenderTargets.RenderTargets[2],
                ((DeferredRenderTask)cam.RenderTask).RenderTargets.RenderTargets[1],
                ((DeferredRenderTask)cam.RenderTask).RenderTargets.RenderTargets[4]);
                
            timer.Start();
            AddEvents();
        }

        public void MainLoop() {
            fpsCount++;
            float elapsed = timer.Restart();
            fpsTimeCount += elapsed;
            if (fpsTimeCount > 1.0f) {
                var fxaa = ((DeferredRenderTask)cam.RenderTask).FXAA;
                var fps = fpsCount / fpsTimeCount;
                var renderTime = (fpsTimeCount / fpsCount * 1000.0f);
                this.Text = renderTime.ToString() + "ms (" + fps.ToString() + " fps) FXAA:" + (fxaa ? "enabled" : "disabled");
                fpsTimeCount = 0;
                fpsCount = 0;
            }
            scene.Update(elapsed);
            cam.QueueRender();
            shadowCam.QueueRender();
            shadowProjectionRenderTask.QueueRender();
            renderer.ExecuteTasks();
            swapChain.Present();
        }

        private void OnClose(object sender, FormClosingEventArgs e) {
            //wait for last Frame to finish
            while (Renderer.Instance.RenderingInProgress) { }

            cam.RenderTask.Dispose();
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
                if (e.KeyCode == Keys.F) {
                    ((DeferredRenderTask)cam.RenderTask).FXAA = !((DeferredRenderTask)cam.RenderTask).FXAA;
                }
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
