namespace CargoEngine.Parameter {
    public enum RenderParameterType {
        MATRIX,
        VECTOR3,
        VECTOR4,
        SRV,
        NUM_ELEM
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
