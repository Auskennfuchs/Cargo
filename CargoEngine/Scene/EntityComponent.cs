using System;

namespace CargoEngine.Scene {
    public abstract class EntityComponent : IDisposable {
        private SceneNode parent;
        public SceneNode Parent {
            get { return parent; }
            set {
                if (parent != null) {
                    parent.RemoveComponent(this);
                }
                parent = value;
                if (parent != null) {
                    Transform = parent.Transform;
                }
            }
        }

        public Transform Transform {
            get;
            protected set;
        }

        public virtual void Update(float elapsed) { }

        public virtual void Render(RenderPipeline renderer) { }

        public virtual void QueueRender() { }

        public abstract void Dispose();
    }
}
