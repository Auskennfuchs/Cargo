using CargoEngine.Parameter;
using System;

namespace CargoEngine.Executors {
    public abstract class Executor : IDisposable {

        public ParameterCollection RenderParameter {
            get; private set;
        } = new ParameterCollection();

        public abstract void Render(RenderPipeline pipeline);
        public abstract void Dispose();
    }
}