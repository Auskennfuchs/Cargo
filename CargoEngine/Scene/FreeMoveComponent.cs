using CargoEngine.Event;

namespace CargoEngine.Scene {
    public class FreeMoveComponent : EventListenerComponent {
        enum ControlKeys {
            FORWARD,
            BACKWARD,
            LEFT,
            RIGHT,
            NUM_KEYS
        }

        private bool[] keyPressed = new bool[(int)ControlKeys.NUM_KEYS];

        public float Speed {
            get; set;
        }

        public FreeMoveComponent(EventManager em) : base(em) {
            RegisterEvent(EventType.KEYDOWN);
            RegisterEvent(EventType.KEYUP);
            Speed = 1.0f;
        }

        public override void Dispose() {
        }

        public override bool HandleEvent(IEvent ev) {
            switch (ev.GetEventType()) {
                case EventType.KEYDOWN: {
                        var kev = (KeyDownEvent)ev;
                        switch (kev.Data.KeyCode) {
                            case System.Windows.Forms.Keys.D:
                                keyPressed[(int)ControlKeys.RIGHT] = true;
                                return true;
                            case System.Windows.Forms.Keys.A:
                                keyPressed[(int)ControlKeys.LEFT] = true;
                                return true;
                            case System.Windows.Forms.Keys.W:
                                keyPressed[(int)ControlKeys.FORWARD] = true;
                                return true;
                            case System.Windows.Forms.Keys.S:
                                keyPressed[(int)ControlKeys.BACKWARD] = true;
                                return true;
                        }
                    }
                    break;
                case EventType.KEYUP: {
                        var kev = (KeyUpEvent)ev;
                        switch (kev.Data.KeyCode) {
                            case System.Windows.Forms.Keys.D:
                                keyPressed[(int)ControlKeys.RIGHT] = false;
                                return true;
                            case System.Windows.Forms.Keys.A:
                                keyPressed[(int)ControlKeys.LEFT] = false;
                                return true;
                            case System.Windows.Forms.Keys.W:
                                keyPressed[(int)ControlKeys.FORWARD] = false;
                                return true;
                            case System.Windows.Forms.Keys.S:
                                keyPressed[(int)ControlKeys.BACKWARD] = false;
                                return true;
                        }
                    }
                    break;
            }
            return false;
        }

        public override void Update(float elapsed) {
            if (keyPressed[(int)ControlKeys.RIGHT]) {
                Transform.Position += Transform.GetRightVector() * (Speed * elapsed);
            }
            if (keyPressed[(int)ControlKeys.LEFT]) {
                Transform.Position += Transform.GetLeftVector() * (Speed * elapsed);
            }
            if (keyPressed[(int)ControlKeys.FORWARD]) {
                Transform.Position += Transform.GetForwardVector() * (Speed * elapsed);
            }
            if (keyPressed[(int)ControlKeys.BACKWARD]) {
                Transform.Position += Transform.GetBackwardVector() * (Speed * elapsed);
            }
        }
    }
}
