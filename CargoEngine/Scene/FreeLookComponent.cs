using CargoEngine.Event;
using SharpDX;

namespace CargoEngine.Scene {
    public class FreeLookComponent : EventListenerComponent {
        private bool mousedown = false;
        private Point mousepos, delta;

        public float Speed {
            get; set;
        }

        public FreeLookComponent(Event.EventManager em) : base(em) {
            Speed = 1.0f;
            RegisterEvent(EventType.MOUSE_DOWN);
            RegisterEvent(EventType.MOUSE_UP);
            RegisterEvent(EventType.MOUSE_MOVE);
        }

        public override void Dispose() {

        }

        public override bool HandleEvent(IEvent ev) {
            switch (ev.GetEventType()) {
                case EventType.MOUSE_DOWN: {
                        var mev = (MouseEvent)ev;
                        if (mev.Data.Button == System.Windows.Forms.MouseButtons.Left) {
                            mousedown = true;
                            mousepos = mev.Data.Position;
                        }
                        break;
                    }
                case EventType.MOUSE_UP: {
                        var mev = (MouseEvent)ev;
                        if (mev.Data.Button == System.Windows.Forms.MouseButtons.Left) {
                            mousedown = false;
                        }
                        break;
                    }
                case EventType.MOUSE_MOVE: {
                        var mev = (MouseEvent)ev;
                        if (mousedown) {
                            delta = new Point(delta.X + mousepos.X - mev.Data.Position.X,
                                              delta.Y + mousepos.Y - mev.Data.Position.Y);
                            mousepos = mev.Data.Position;
                        }
                        break;
                    }
            }
            return false;
        }

        public override void Update(float elapsed) {
            if (mousedown) {
                Transform.AddRotation(Vector3.Up, delta.X *Speed* elapsed);
                Transform.AddRotation(Transform.GetRightVector(), delta.Y *Speed* elapsed);
                delta = Point.Zero;
            }
        }
    }
}
