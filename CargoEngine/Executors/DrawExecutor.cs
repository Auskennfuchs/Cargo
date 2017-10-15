
namespace CargoEngine.Executors {
    public class DrawExecutor : Executor {
        public Mesh Geometry {
            get; private set;
        } = new Mesh();

        public DrawExecutor() {
        }

        public override void Dispose() {
            Geometry.Clear();
        }

        public override void Render(RenderPipeline pipeline) {
            pipeline.InputAssembler.ClearDesiredState();

            Geometry.Apply(pipeline);
            pipeline.ApplyInputResources();
            pipeline.ApplyShaderResources();
            if (Geometry.NumIndices > 0) {
                pipeline.DrawIndexed(Geometry.NumIndices, 0, 0);
            } else {
                pipeline.Draw(Geometry.VertexCount, 0);
            }
        }
    }
}
