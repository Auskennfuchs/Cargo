using SharpDX;
using System;

namespace CargoEngine {
    public abstract class RenderTask : IDisposable{

        public Matrix ViewMatrix {
            get; set;
        }
        public Matrix ProjectionMatrix {
            get; set;
        }

        public Scene.Scene Scene {
            get; set;
        }

        public RenderTask() {
            ViewMatrix = ProjectionMatrix = Matrix.Identity;
        }

        public abstract void Render(RenderPipeline pipeline);
        public abstract void QueueRender();

        public virtual void Dispose() { }
    }
}
