using System.Collections.Generic;
using System.Windows.Forms;
using CargoEngine.Exception;
using CargoEngine.Parameter;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

using VShader = SharpDX.Direct3D11.VertexShader;

namespace CargoEngine.Shader {
    public class VertexShader : ShaderBase<VShader> {
        public VertexShader(ShaderSignature inputSignature, VShader shader, List<ConstantBuffer> constantBuffers, Dictionary<int, string> textures)
            : base(shader, inputSignature, constantBuffers, textures) { 
        }
    }
}
