using System.Collections.Generic;
using System.Linq;

namespace CargoEngine.Scene
{

    internal class ComponentList
    {
        Dictionary<string, EntityComponent> components = new Dictionary<string, EntityComponent>();

        public int Count {
            get {
                return components.Count;
            }
        }

        public IReadOnlyCollection<EntityComponent> Components {
            get {
                return components.Values;
            }
        }

        public T Get<T>() where T : EntityComponent {
            if (components.ContainsKey(typeof(T).ToString())) {
                return (T)components[typeof(T).ToString()];
            }
            return null;
        }

        public void Set<T>(T value) where T : EntityComponent {
            components[typeof(T).ToString()] = value;
        }

        public T Delete<T>() where T : EntityComponent {
            if (components.ContainsKey(typeof(T).ToString())) {
                var component = (T)components[typeof(T).ToString()];
                components.Remove(typeof(T).ToString());
                return component;
            }
            return null;
        }

        public bool Delete(EntityComponent component) {
            if (components.ContainsValue(component)) {
                foreach (var item in components.Where(kvp => kvp.Value == component).ToList()) {
                    components.Remove(item.Key);
                }
                return true;
            }
            return false;
        }

        public void Clear() {
            components.Clear();
        }
    }

    public class SceneNode
    {
        private List<SceneNode> childs = new List<SceneNode>();
        private ComponentList componentList = new ComponentList();

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

        public SceneNode AddComponent<T>(T component) where T : EntityComponent {
            component.Parent = this;
            componentList.Set<T>(component);
            return this;
        }

        public T GetComponent<T>() where T: EntityComponent {
            return componentList.Get<T>();
        }

        public SceneNode RemoveComponent<T>() where T : EntityComponent {
            var component = componentList.Delete<T>();
            if (component != null) {
                component.Parent = null;
            }
            return this;
        }       

        public bool RemoveComponent(EntityComponent component) {
            return componentList.Delete(component);
        }

        public void OnUpdate(float elapsed) {
            Update(elapsed);

            if (componentList.Count > 0) {
                foreach (var c in componentList.Components) {
                    c.Update(elapsed);
                }
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
            if (componentList.Count > 0) {
                foreach (var c in componentList.Components) {
                    c.Render(renderer);
                }
            }
            if (childs.Count > 0) {
                childs.ForEach((c) => {
                    c.OnRender(renderer);
                });
            }
        }

        public virtual void PreRender(Renderer renderer) { }

        public void QueueRender() {
            if (componentList.Count > 0) {
                foreach (var c in componentList.Components) {
                    c.QueueRender();
                }
            }
            childs.ForEach((c) => {
                c.QueueRender();
            });
        }

        public void Clear() {
            foreach (var c in componentList.Components) {
                c.Dispose();
            }
            componentList.Clear();

            childs.ForEach((c) => {
                c.Clear();
            });
            childs.Clear();
        }
    }
}