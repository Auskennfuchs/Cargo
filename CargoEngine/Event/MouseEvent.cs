using System.Windows.Forms;
using SharpDX;

namespace CargoEngine.Event {
    public struct SMouseEvent {
        public Point Position;
        public MouseButtons Button;
    }

    public abstract class MouseEvent : IEvent{
        public SMouseEvent Data {
            get;
        }

        public MouseEvent(SMouseEvent ev) {
            Data = ev;
        }

        public abstract string GetName();
        public abstract EventType GetEventType();
    }

    public class MouseDownEvent : MouseEvent {

        public MouseDownEvent(SMouseEvent ev) : base(ev) {
        }

        public override EventType GetEventType() {
            return EventType.MOUSE_DOWN;
        }

        public override string GetName() {
            return "mouse_down";
        }
    }

    public class MouseMoveEvent : MouseEvent {

        public MouseMoveEvent(SMouseEvent ev) : base(ev) {
        }

        public override EventType GetEventType() {
            return EventType.MOUSE_MOVE;
        }

        public override string GetName() {
            return "mouse_move";
        }
    }

    public class MouseUpEvent : MouseEvent {

        public MouseUpEvent(SMouseEvent ev) : base(ev) {
        }

        public override EventType GetEventType() {
            return EventType.MOUSE_UP;
        }

        public override string GetName() {
            return "mouse_up";
        }
    }
}
