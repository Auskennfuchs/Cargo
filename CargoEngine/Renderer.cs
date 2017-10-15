using System;
using System.Collections.Generic;
using CargoEngine.Exception;
using SharpDX;
using SharpDX.Direct3D11;

namespace CargoEngine {

    public class Renderer: IDisposable {

        private const int NUM_THREADS = 4;

        public static Renderer Instance {
            get; private set;
        }

        public Device Device {
            get; private set;
        }
        DeviceContext devContext;

        public RenderPipeline ImmPipeline {
            get; private set;
        }

        private RenderPipeline[] deferredPipelines = new RenderPipeline[NUM_THREADS];

        private Dictionary<int,InputLayout> inputLayouts;

        private Queue<RenderTask> taskQueue = new Queue<RenderTask>();

        public Renderer() {
            if(Instance!=null) {
                throw CargoEngineException.Create("multiple instances of renderer");
            }
            Instance = this;

            DeviceCreationFlags devFlags = 0;
#if DEBUG
            devFlags |= DeviceCreationFlags.Debug;
#endif
            Device = new Device(SharpDX.Direct3D.DriverType.Hardware, devFlags);
            devContext = Device.ImmediateContext;

            ImmPipeline = new RenderPipeline(devContext);

            inputLayouts = new Dictionary<int, InputLayout>();

            for(var i = 0; i < NUM_THREADS; i++) {
                var pipeline = new RenderPipeline(Device);
                deferredPipelines[i] = pipeline;
            }
        }

        public void Dispose() {
            foreach(var rp in deferredPipelines) {
                rp.Dispose();
            }
            if (Device != null) {
                Device.Dispose();
            }
        }

        public void QueueTask(RenderTask task) {
            taskQueue.Enqueue(task);
        }

        public void ExecuteTasks() {
            var taskCount = taskQueue.Count;
            for (int i = 0, count=0; i < taskCount; i+=count) {
                var j = 0;
                for(; j < NUM_THREADS && i + j < taskCount; j++) {
                    count++;
                    var task = taskQueue.Dequeue();
                    task.Render(deferredPipelines[0]);
                }
                deferredPipelines[0].FinishCommandList();
                ImmPipeline.ExecuteCommandList(deferredPipelines[0].CommandList);
                deferredPipelines[0].ReleaseCommandList();
/*                for (var k = 0; k < j; k++) {
                    deferredPipelines[k].FinishCommandList();
                    ImmPipeline.ExecuteCommandList(deferredPipelines[k].CommandList);
                    deferredPipelines[k].ReleaseCommandList();
                }*/
            }

            taskQueue.Clear();
        }

        public void SetGlobalParameter(string name,Matrix mat) {
            foreach(var pipeline in deferredPipelines) {
                pipeline.ParameterManager.SetParameter(name, mat);
            }
        }

        public SamplerState CreateSamplerState(TextureAddressMode texMode, Filter filter, int aniso) {
            var samplerStateDescription = new SamplerStateDescription {
                AddressU = texMode,
                AddressV = texMode,
                AddressW = texMode,
                Filter = filter,
                MaximumAnisotropy = 16,
            };
            return new SamplerState(Renderer.Instance.Device, samplerStateDescription);
        }
    }
}
