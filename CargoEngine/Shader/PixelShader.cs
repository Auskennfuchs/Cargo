using CargoEngine.Exception;
using CargoEngine.Parameter;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

using PShader = SharpDX.Direct3D11.PixelShader;

namespace CargoEngine.Shader {
    public class PixelShader : ShaderBase {
        public PShader PixelShaderPtr {
            get; private set;
        }

        public PixelShader(Renderer renderer, string file, string entryfunction) : base() {
            try {
                ShaderFlags sFlags = ShaderFlags.PackMatrixRowMajor;
#if DEBUG
                sFlags |= ShaderFlags.Debug;
#endif
                using (var bytecode = ShaderBytecode.CompileFromFile(file, entryfunction, "ps_5_0", sFlags, EffectFlags.None)) {
                    InputSignature = ShaderSignature.GetInputSignature(bytecode);
                    PixelShaderPtr = new PShader(renderer.Device, bytecode);
                    ReflectBytecode(renderer, bytecode);
                }
            }
            catch (System.Exception exc) {
                throw CargoEngineException.Create("Error loading PixelShader", exc);
            }
        }

        ~PixelShader() {
            Dispose();
        }

        public new void Dispose() {
            if (PixelShaderPtr != null) {
                PixelShaderPtr.Dispose();
            }
        }

        public override void Apply(DeviceContext context, ParameterManager paramManager) {
            context.PixelShader.Set(PixelShaderPtr);
            for (int i = 0; i < ConstantBuffers.Count; i++) {
                ConstantBuffers[i].UpdateBuffer(context, paramManager);
                context.PixelShader.SetConstantBuffer(i, ConstantBuffers[i].Buffer);
            }
        }
    }
}
