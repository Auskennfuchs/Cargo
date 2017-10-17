using CargoEngine.Parameter;
using SharpDX.Direct3D11;

namespace CargoEngine.Stages {

    public enum ShaderStages {
        VERTEX=0,
        PIXEL,
        GEOMETRY,
        HULL,
        NUM_SHADERSTAGES
    }

    public abstract class ShaderStage<ShaderClass> : Stage<ShaderStageState<ShaderClass>> where ShaderClass : DeviceChild{

        public TStateArrayMonitor<ConstantBuffer> ConstantBuffer {
            get {
                return DesiredState.ConstantBuffer;
            }
        }

        public TStateArrayMonitor<ShaderResourceView> Resource {
            get {
                return DesiredState.Resources;
            }
        }

        public TStateArrayMonitor<SamplerState> Sampler {
            get {
                return DesiredState.Samplers;
            }
        }

        protected SharpDX.Direct3D11.Buffer[] cBuffers = new SharpDX.Direct3D11.Buffer[ShaderStageState<ShaderClass>.NUM_CONSTANTBUFFERS];
        protected SharpDX.Direct3D11.ShaderResourceView[] SRVs = new ShaderResourceView[ShaderStageState<ShaderClass>.NUM_SHADERRESOURCES];

        public override void OnApplyDesiredState(DeviceContext dc, ParameterManager paramManager) {
            if (DesiredState.Shader.NeedUpdate) {
                BindShader(dc, paramManager);
            }
            UpdateConstantBufferParameter(dc, paramManager);
            if (DesiredState.ConstantBuffer.NeedUpdate) {
                UpdateConstantBufferArray();
                BindConstantBuffers(dc, paramManager);
            }
            UpdateShaderResources(paramManager);
            if (DesiredState.Resources.NeedUpdate) {
                BindShaderResources(dc);
            }
            if (DesiredState.Samplers.NeedUpdate) {
                BindSamplerResources(dc);
            }
        }

        protected abstract void BindShader(DeviceContext dc, ParameterManager paramManager);
        protected abstract void BindConstantBuffers(DeviceContext dc, ParameterManager paramManager);
        protected abstract void BindShaderResources(DeviceContext dc);
        protected abstract void BindSamplerResources(DeviceContext dc);

            public void UpdateConstantBufferParameter(DeviceContext dc, ParameterManager paramManager) {
            for (var i = 0; i < DesiredState.ConstantBuffer.NumSlots; i++) {
                if (DesiredState.ConstantBuffer.States[i] != null) {
                    DesiredState.ConstantBuffer.States[i].UpdateBuffer(dc, paramManager);
                }
            }
        }

        private void UpdateConstantBufferArray() {
            for (var i = 0; i < DesiredState.ConstantBuffer.Range; i++) {
                var slotPos = DesiredState.ConstantBuffer.StartSlot + i;
                if (DesiredState.ConstantBuffer.States[slotPos] != null) {
                    cBuffers[i] = DesiredState.ConstantBuffer.States[slotPos].Buffer;
                } else {
                    cBuffers[i] = null;
                }
            }
        }

        private void UpdateShaderResources(ParameterManager paramManager) {
            if(DesiredState.Shader.State!=null) {
                foreach(var tex in DesiredState.Shader.State.Textures) {
                    DesiredState.Resources.SetState(tex.Key, paramManager.GetSRVParameter(tex.Value));
                }
                foreach (var sampler in DesiredState.Shader.State.Samplers) {
                    DesiredState.Samplers.SetState(sampler.Key, paramManager.GetSamplerStateParameter(sampler.Value));
                }
            }
        }
    }
}
