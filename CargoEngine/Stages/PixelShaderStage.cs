using CargoEngine.Parameter;
using SharpDX.Direct3D11;
using PixelShader = CargoEngine.Shader.PixelShader;

namespace CargoEngine.Stages {
    public class PixelShaderStage : ShaderStage<SharpDX.Direct3D11.PixelShader> {

        public PixelShader Shader {
            get {
                return DesiredState.Shader.State as PixelShader;
            }
            set {
                DesiredState.Shader.State = value;
            }
        }

        protected override void BindShader(DeviceContext dc, ParameterManager paramManager) {
            if (DesiredState.Shader.State != null) {
                var ps = (PixelShader)DesiredState.Shader.State;
                dc.PixelShader.Set(ps.ShaderPtr);
                DesiredState.ConstantBuffer.SetStates(0, ps.ConstantBuffers);
            } else {
                dc.PixelShader.Set(null);
            }
        }

        protected override void BindConstantBuffers(DeviceContext dc, ParameterManager paramManager) {
            dc.PixelShader.SetConstantBuffers(DesiredState.ConstantBuffer.StartSlot, DesiredState.ConstantBuffer.Range, cBuffers);
        }

        protected override void BindShaderResources(DeviceContext dc) {
            dc.PixelShader.SetShaderResources(DesiredState.Resources.StartSlot, DesiredState.Resources.Range, DesiredState.Resources.ChangedStates);
        }
    }
}
