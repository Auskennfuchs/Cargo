using CargoEngine.Parameter;
using SharpDX.Direct3D11;

namespace CargoEngine.Stages {
    public interface IStageState {
        void ClearState();
        void Clone(IStageState src);
        void ResetTracking();
        void SetSisterState(IStageState sister);
    }

    public abstract class Stage<T> where T : IStageState, new() {
        public T DesiredState {
            get; private set;
        }

        public T CurrentState {
            get; private set;
        }

        public Stage() {
            DesiredState = new T();
            CurrentState = new T();
            DesiredState.SetSisterState(CurrentState);
        }

        public void ApplyDesiredState(DeviceContext dc, ParameterManager paramManager) {
            OnApplyDesiredState(dc, paramManager);
            DesiredState.ResetTracking();
            CurrentState.Clone(DesiredState);
        }

        public abstract void OnApplyDesiredState(DeviceContext dc, ParameterManager paramManager);

        public void ClearDesiredState() {
            DesiredState.ClearState();
        }

        public void ClearCurrentState() {
            CurrentState.ClearState();
        }
    }
}
