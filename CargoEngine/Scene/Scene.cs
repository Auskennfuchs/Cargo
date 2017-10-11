namespace CargoEngine.Scene {
    public class Scene {
        public SceneNode RootNode {
            get; private set;
        }

        public Scene() {
            RootNode = new SceneNode();
        }

        public void Update(float elapsed) {
            RootNode.OnUpdate(elapsed);
        }

        public void Render(RenderPipeline renderer) {
            RootNode.OnRender(renderer);
        }

        public void QueueRender() {
            RootNode.QueueRender();
        }

        public void Clear() {
            RootNode.Clear();
        }
    }
}
