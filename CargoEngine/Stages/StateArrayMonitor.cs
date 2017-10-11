using System;
using System.Collections.Generic;

namespace CargoEngine.Stages {
    public class TStateArrayMonitor<T> where T : class {

        public bool NeedUpdate {
            get; private set;
        }

        private T initialState;
        public int NumSlots {
            get; private set;
        }

        public int StartSlot {
            get; private set;
        }
        public int EndSlot {
            get; private set;
        }

        public int Range {
            get {
                return Math.Max(0, EndSlot - StartSlot + 1);
            }
        }

        public T[] States {
            get; private set;
        }

        public T this[int i] {
            get {
                return States[i];
            }

            set {
                SetState(i, value);
            }
        }

        private T[] changedStates;

        public T[] ChangedStates {
            get {
                if (Range == 0) {
                    return null;
                }
                if (NeedUpdate) {
                    for (var i = 0; i < Range; i++) {
                        changedStates[i] = States[StartSlot + i];
                    }
                }
                return changedStates;
            }
        }

        public TStateArrayMonitor<T> Sister {
            get; set;
        }

        public TStateArrayMonitor(int numSlots, T initialState) {
            States = new T[numSlots];
            changedStates = new T[numSlots];
            this.initialState = initialState;
            NumSlots = numSlots;
            ResetTracking();
        }

        public void SetState(int slot, T state) {
            States[slot] = state;
            bool needUpdate = !SameSister(slot);
            if (needUpdate) {
                NeedUpdate = true;
                if (slot < StartSlot) {
                    StartSlot = slot;
                }
                if (slot > EndSlot) {
                    EndSlot = slot;
                }
            }
        }

        public void SetStates(int startSlot, T[] states) {
            var i = startSlot;
            foreach(var s in states) {
                SetState(i++, s);
            }
        }

        public void SetStates(int startSlot, List<T> states) {
            var i = startSlot;
            foreach (var s in states) {
                SetState(i++, s);
            }
        }

        private bool SameSister(int slot) {
            if (Sister != null) {
                return States[slot] == Sister.States[slot];
            }
            return false;
        }

        public void ResetTracking() {
            NeedUpdate = false;
            StartSlot = 0;
            EndSlot = -1;
        }

        public void InitializeState() {
            for (int i = 0; i < NumSlots; i++) {
                States[i] = initialState;
                changedStates[i] = initialState;
            }
        }
    }

    public class TStateArrayMonitorStruct<T> where T : struct {

        public bool NeedUpdate {
            get; private set;
        }

        private T initialState;
        public int NumSlots {
            get; private set;
        }

        public int StartSlot {
            get; private set;
        }
        public int EndSlot {
            get; private set;
        }

        public int Range {
            get {
                return Math.Max(0, EndSlot - StartSlot + 1);
            }
        }

        public T[] States {
            get; private set;
        }

        private T[] changedStates;

        public T[] ChangedStates {
            get {
                if (Range == 0) {
                    return null;
                }
                if (NeedUpdate) {
                    for (var i = 0; i < Range; i++) {
                        changedStates[i] = States[StartSlot + i];
                    }
                }
                return changedStates;
            }
        }

        public TStateArrayMonitorStruct<T> Sister {
            get; set;
        }

        public TStateArrayMonitorStruct(int numSlots, T initialState) {
            States = new T[numSlots];
            changedStates = new T[numSlots];
            this.initialState = initialState;
            NumSlots = numSlots;
            ResetTracking();
        }

        public T this[int i] {
            get {
                return States[i];
            }

            set {
                SetState(i, value);
            }
        }

        public void SetState(int slot, T state) {
            States[slot] = state;
            bool needUpdate = !SameSister(slot);
            if (needUpdate) {
                NeedUpdate = true;
                if (slot < StartSlot) {
                    StartSlot = slot;
                }
                if (slot > EndSlot) {
                    EndSlot = slot;
                }
            }
        }

        public void SetStates(int startSlot, T[] states) {
            var i = startSlot;
            foreach (var s in states) {
                SetState(i++, s);
            }
        }

        private bool SameSister(int slot) {
            if (Sister != null) {
                return States[slot].Equals(Sister.States[slot]);
            }
            return false;
        }

        public void ResetTracking() {
            NeedUpdate = false;
            StartSlot = 0;
            EndSlot = -1;
        }

        public void InitializeState() {
            for (int i = 0; i < NumSlots; i++) {
                States[i] = initialState;
                changedStates[i] = initialState;
            }
        }
    }
}
