using System.Collections.Generic;

namespace CargoEngine.Scene {
    public class SceneNode {
        private List<SceneNode> childs = new List<SceneNode>();
        private List<EntityComponent> components = new List<EntityComponent>();

        public Transform Transform {
            get; set;
        }

        public SceneNode() {
            Transform = new Transform();
        }

        public SceneNode AddChild(SceneNode c) {
            childs.Add(c);
            return this;
        }

        public SceneNode AddComponent(EntityComponent component) {
            component.Parent = this;
            components.Add(component);
            return this;
        }

        public SceneNode RemoveComponent(EntityComponent component) {
            if (components.Contains(component)) {
                components.Remove(component);
                component.Parent = null;
            }
            return this;
        }

        public void OnUpdate(float elapsed) {
            Update(elapsed);

            if (components.Count > 0) {
                components.ForEach((c) => {
                    c.Update(elapsed);
                });
            }
            if (childs.Count > 0) {
                childs.ForEach((c) => {
                    c.OnUpdate(elapsed);
                });
            }
        }

        public virtual void Update(float elapsed) { }

        public void OnRender(RenderPipeline renderer) {
            renderer.ParameterManager.SetWorldMatrix(Transform.WorldMatrix);
            if (components.Count > 0) {
                components.ForEach((c) => {
                    c.Render(renderer);
                });
            }
            if (childs.Count > 0) {
                childs.ForEach((c) => {
                    c.OnRender(renderer);
                });
            }
        }

        public virtual void PreRender(Renderer renderer) { }

        public void QueueRender() {
            components.ForEach((c) => {
                c.QueueRender();
            });
            childs.ForEach((c) => {
                c.QueueRender();
            });
        }

        public void Clear() {
            components.ForEach((c) => {
                c.Dispose();
            });
            components.Clear();

            childs.ForEach((c) => {
                c.Clear();
            });
            childs.Clear();
        }
    }
}