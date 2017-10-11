using SharpDX;

namespace CargoEngine {
    public abstract class RenderTask {

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
    }
}
