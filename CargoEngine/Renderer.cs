using System;
using System.Collections.Generic;
using CargoEngine.Exception;
using SharpDX;
using SharpDX.Direct3D11;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using CargoEngine.Shader;

namespace CargoEngine
{

    public class Renderer : IDisposable
    {

        private struct TaskPayload
        {
            public RenderTask task;
            public int pipelineNumber;
        }

        private const int NUM_THREADS = 4;

        public static Renderer Instance {
            get; private set;
        }

        public static Device Dev {
            get { return Instance.Device; }
        }

        internal Device Device {
            get; private set;
        }

        public RenderPipeline ImmPipeline {
            get; private set;
        }

        public ShaderLoader Shaders {
            get;
        }

        public static ShaderLoader ShaderLoader {
            get { return Instance.Shaders; }
        }

        public bool RenderingInProgress {
            get; private set;
        }

        private RenderPipeline[] deferredPipelines = new RenderPipeline[NUM_THREADS];

        private Dictionary<int, InputLayout> inputLayouts;

        private ConcurrentQueue<RenderTask> taskQueue = new ConcurrentQueue<RenderTask>();

        public Renderer() {
            if (Instance != null) {
                throw CargoEngineException.Create("multiple instances of renderer");
            }
            Instance = this;

            DeviceCreationFlags devFlags = 0;
#if DEBUG
            devFlags |= DeviceCreationFlags.Debug;
#endif
            Device = new Device(SharpDX.Direct3D.DriverType.Hardware, devFlags);

            ImmPipeline = new RenderPipeline(Device.ImmediateContext);

            inputLayouts = new Dictionary<int, InputLayout>();

            for (var i = 0; i < NUM_THREADS; i++) {
                var pipeline = new RenderPipeline(Device);
                deferredPipelines[i] = pipeline;
            }

            Shaders = new ShaderLoader(this);
        }

        public void Dispose() {
            Shaders.Dispose();
            foreach (var rp in deferredPipelines) {
                rp.Dispose();
            }
            deferredPipelines = null;
            ImmPipeline.Dispose();
            ImmPipeline = null;
            if (Device != null) {
                Device.Dispose();
            }
            Instance = null;
        }

        public void QueueTask(RenderTask task) {
            taskQueue.Enqueue(task);
        }

        public void ExecuteTasks() {
            RenderingInProgress = true;
            var taskCount = taskQueue.Count;
            List<Task> tasks = new List<Task>();
            for (int i = 0, count = 0; i < taskCount; i += count) {
                var j = 0;
                tasks.Clear();
                for (count = 0; j < NUM_THREADS && i + j < taskCount; j++) {
                    count++;
                    RenderTask task;
                    if (!taskQueue.TryDequeue(out task)) {
                        throw new SystemException("error dequeueing rendertask");
                    }
                    var payload = new TaskPayload {
                        pipelineNumber = j,
                        task = task
                    };
                    var t = new Task(() => { RunTask(payload); });
                    t.Start();
                    tasks.Add(t);
                }
                Task.WaitAll(tasks.ToArray());
                for (var k = 0; k < j; k++) {
                    deferredPipelines[k].FinishCommandList();
                    ImmPipeline.ExecuteCommandList(deferredPipelines[k].CommandList);
                    deferredPipelines[k].ReleaseCommandList();
                }
            }
            RenderingInProgress = false;
        }

        private void RunTask(TaskPayload payload) {
            payload.task.Render(deferredPipelines[payload.pipelineNumber]);
        }

        public void SetGlobalParameter(string name, Matrix mat) {
            foreach (var pipeline in deferredPipelines) {
                pipeline.ParameterManager.SetParameter(name, mat);
            }
        }

        public SamplerState CreateSamplerState(TextureAddressMode texMode, Filter filter, int aniso) {
            var samplerStateDescription = new SamplerStateDescription {
                AddressU = texMode,
                AddressV = texMode,
                AddressW = texMode,
                Filter = filter,
                MaximumAnisotropy = Math.Max(1, aniso),
            };
            var hash = samplerStateDescription.GetHashCode();
            return new SamplerState(Renderer.Instance.Device, samplerStateDescription);
        }
    }
}
