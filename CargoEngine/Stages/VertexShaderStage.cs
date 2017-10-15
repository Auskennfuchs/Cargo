using CargoEngine.Parameter;
using SharpDX.Direct3D11;
using VertexShader = CargoEngine.Shader.VertexShader;

namespace CargoEngine.Stages {
    public class VertexShaderStage : ShaderStage<SharpDX.Direct3D11.VertexShader> {

        public VertexShader Shader {
            get {
                return DesiredState.Shader.State as VertexShader;
            }
            set {
                DesiredState.Shader.State = value;
            }
        }

        protected override void BindShader(DeviceContext dc, ParameterManager paramManager) {
            if (DesiredState.Shader.State != null) {
                var vs = (VertexShader)DesiredState.Shader.State;
                dc.VertexShader.Set(vs.ShaderPtr);
                DesiredState.ConstantBuffer.SetStates(0,vs.ConstantBuffers);
            } else {
                dc.VertexShader.Set(null);
            }
        }

        protected override void BindConstantBuffers(DeviceContext dc, ParameterManager paramManager) {
            dc.VertexShader.SetConstantBuffers(DesiredState.ConstantBuffer.StartSlot, DesiredState.ConstantBuffer.Range, cBuffers);
        }

        protected override void BindShaderResources(DeviceContext dc) {
            dc.VertexShader.SetShaderResources(DesiredState.Resources.StartSlot, DesiredState.Resources.Range, DesiredState.Resources.ChangedStates);
        }

        protected override void BindSamplerResources(DeviceContext dc) {
            dc.VertexShader.SetSamplers(DesiredState.Samplers.StartSlot, DesiredState.Samplers.Range, DesiredState.Samplers.ChangedStates);
        }
    }
}
