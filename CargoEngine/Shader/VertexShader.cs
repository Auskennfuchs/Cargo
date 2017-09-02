using System.Windows.Forms;
using CargoEngine.Exception;
using CargoEngine.Parameter;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

using VShader = SharpDX.Direct3D11.VertexShader;

namespace CargoEngine.Shader {
    public class VertexShader : ShaderBase {
        public VShader VertexShaderPtr {
            get; private set;
        }

        public VertexShader(Renderer renderer, string file, string entryfunction) : base() {
            try {
                ShaderFlags sFlags = ShaderFlags.PackMatrixRowMajor;
#if DEBUG
                sFlags |= ShaderFlags.Debug;
#endif
                using (var bytecode = ShaderBytecode.CompileFromFile(file, entryfunction, "vs_5_0", sFlags, EffectFlags.None)) {
                    if (bytecode.Message != null) {
                        MessageBox.Show(bytecode.Message);
                        return;
                    }
                    InputSignature = ShaderSignature.GetInputSignature(bytecode);
                    VertexShaderPtr = new VShader(renderer.Device, bytecode);
                    ReflectBytecode(renderer, bytecode);
                }
            }
            catch (System.Exception exc) {
                throw CargoEngineException.Create("Error loading VertexShader", exc);
            }
        }

        ~VertexShader() {
            Dispose();
        }

        public new void Dispose() {
            if (VertexShaderPtr != null) {
                VertexShaderPtr.Dispose();
            }
        }

        public override void Apply(DeviceContext context, ParameterManager paramManager) {
            context.VertexShader.Set(VertexShaderPtr);
            for (int i = 0; i < ConstantBuffers.Count; i++) {
                ConstantBuffers[i].UpdateBuffer(context, paramManager);
                context.VertexShader.SetConstantBuffer(i, ConstantBuffers[i].Buffer);
            }
        }
    }
}
