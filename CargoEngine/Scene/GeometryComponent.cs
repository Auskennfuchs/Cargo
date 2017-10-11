using CargoEngine.Executors;

namespace CargoEngine.Scene {
    public class GeometryComponent : EntityComponent {

        public DrawExecutor Executor {
            get; private set;
        }

        public GeometryComponent() : base() {
            Executor = new DrawExecutor();
        }

        public override void Dispose() {
            Executor.Dispose();
        }

        public override void Render(RenderPipeline renderer) {
            Executor.Render(renderer);
        }
    }
}
