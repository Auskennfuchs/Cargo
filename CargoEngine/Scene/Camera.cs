using SharpDX;

namespace CargoEngine.Scene {
    public class Camera : SceneNode {
        public Matrix ProjectionMatrix {
            get; private set;
        }

        public RenderTask RenderTask {
            get; set;
        }

        public Scene Scene {
            get; set;
        }

        public Camera() {
            ProjectionMatrix = Matrix.Identity;
        }

        public void SetProjection(float zNear, float zFar, float aspect, float fov) {
            ProjectionMatrix = Matrix.PerspectiveFovLH(fov, aspect, zNear, zFar);
        }

        public Matrix ViewMatrix {
            get {
                return Matrix.Translation(Transform.GetTransformedPosition() * -1.0f) * Matrix.RotationQuaternion(-Transform.GetTransformedRotation());
            }
        }

        public new void QueueRender() {
            if (RenderTask != null) {
                RenderTask.ViewMatrix = ViewMatrix;
                RenderTask.ProjectionMatrix = ProjectionMatrix;
                RenderTask.Scene = Scene;
                RenderTask.QueueRender();
            }
        }
    }
}
