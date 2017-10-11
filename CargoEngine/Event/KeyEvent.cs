using System.Windows.Forms;

namespace CargoEngine.Event {
    public struct SKeyEvent {
        public Keys KeyCode;
        public bool Shift;
        public bool Alt;
        public bool Control;
    }

    public abstract class KeyEvent : IEvent {
        public SKeyEvent Data {
            get;
        }

        public KeyEvent(SKeyEvent ev) {
            Data = ev;
        }

        public abstract string GetName();
        public abstract EventType GetEventType();
    }

    public class KeyDownEvent : KeyEvent {

        public KeyDownEvent(SKeyEvent ev) : base(ev) {
        }

        public override EventType GetEventType() {
            return EventType.KEYDOWN;
        }

        public override string GetName() {
            return "keyboard_keydown";
        }
    }

    public class KeyUpEvent : KeyEvent {

        public KeyUpEvent(SKeyEvent ev) : base(ev) {
        }

        public override EventType GetEventType() {
            return EventType.KEYUP;
        }

        public override string GetName() {
            return "keyboard_keyup";
        }

    }
}
