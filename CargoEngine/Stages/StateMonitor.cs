﻿using System;

namespace CargoEngine.Stages {
    public class TStateMonitor<T> {

        private T state, initialState;

        public bool NeedUpdate {
            get; private set;
        }

        public TStateMonitor<T> Sister {
            get; set;
        }

        public T State {
            set {
                state = value;
                NeedUpdate = !SameAsSister();
            }
            get {
                return state;
            }
        }

        public bool SameAsSister() {
            if (Sister != null) {
                if (state != null) {
                    return state.Equals(Sister.state);
                } else {
                    return null == Sister.state;
                }
            }
            return false;
        }

        public TStateMonitor(T initialState) {
            this.initialState = state = initialState;
            NeedUpdate = false;
        }

        public void ResetTracking() {
            NeedUpdate = false;
        }

        public void InitializeState() {
            State = initialState;
        }
    }

    public class TStateMonitorEnum<T> where T : IConvertible {

        private T state, initialState;

        public bool NeedUpdate {
            get; private set;
        }

        public TStateMonitorEnum<T> Sister {
            get; set;
        }

        public T State {
            set {
                state = value;
                NeedUpdate = !SameAsSister();
            }
            get {
                return state;
            }
        }

        public bool SameAsSister() {
            if (Sister != null) {
                if (state != null) {
                    return state.Equals(Sister.state);
                } else {
                    return null == Sister.state;
                }
            }
            return false;
        }

        public TStateMonitorEnum(T initialState) {
            this.initialState = state = initialState;
            NeedUpdate = false;
        }

        public void ResetTracking() {
            NeedUpdate = false;
        }

        public void InitializeState() {
            state = initialState;
        }
    }
}
