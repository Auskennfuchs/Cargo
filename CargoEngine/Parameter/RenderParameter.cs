namespace CargoEngine.Parameter {
    public enum RenderParameterType {
        Matrix,
        Vector3,
        Vector4,
        SRV,
        SamplerState,
        NumElements
    }

    public abstract class RenderParameter {
        public object Value {
            get; set;
        }

        public RenderParameterType Type {
            get;
        }

        protected RenderParameter(RenderParameterType type, object value) {
            Value = value;
            Type = type;
        }
    }
}
