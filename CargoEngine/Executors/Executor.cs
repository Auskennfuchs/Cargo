using System;

namespace CargoEngine.Executors {
    public abstract class Executor : IDisposable {
        public abstract void Render(RenderPipeline pipeline);
        public abstract void Dispose();
    }
}