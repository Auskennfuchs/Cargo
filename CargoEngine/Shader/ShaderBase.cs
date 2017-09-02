using System.Collections.Generic;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace CargoEngine.Shader {
    public abstract class ShaderBase<ShaderClass> where ShaderClass : DeviceChild {
        private static int ShaderIdCounter = 1;

        public List<ConstantBuffer> ConstantBuffers {
            get; private set;
        }

        public ShaderSignature InputSignature {
            get; private set;
        }

        public int ID {
            get; private set;
        }

        public Dictionary<int, string> Textures {
            get; private set;
        }

        public ShaderClass ShaderPtr {
            get; private set;
        }

        public ShaderBase(ShaderClass shader, ShaderSignature inputSignature, List<ConstantBuffer> constantBuffers, Dictionary<int,string> textures) {
            ID = ShaderIdCounter++;
            ShaderPtr = shader;
            InputSignature = inputSignature;
            ConstantBuffers = constantBuffers;
            Textures = textures;
        }

        ~ShaderBase() {
            Dispose();
        }

        public void Dispose() {
            if (InputSignature != null) {
                InputSignature.Dispose();
            }
            if (ConstantBuffers != null) {
                ConstantBuffers.ForEach(buf => buf.Dispose());
            }
        }

        public void SetParameterMatrix(string name, Matrix m) {
            ConstantBuffers.ForEach(buf => buf.SetParameterMatrix(name, m));
        }

    }
}
