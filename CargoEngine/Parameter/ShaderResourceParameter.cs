namespace CargoEngine.Parameter {
    class ShaderResourceParameter : RenderParameter {
        public ShaderResourceParameter(object value = null) : base(RenderParameterType.SRV, value) {
        }
    }

    class TextureParameter : RenderParameter
    {
        public TextureParameter(object value=null):base(RenderParameterType.Texture, value) {
        }
    }
}
