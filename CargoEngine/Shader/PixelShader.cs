using System.Collections.Generic;
using SharpDX.D3DCompiler;

using PShader = SharpDX.Direct3D11.PixelShader;

namespace CargoEngine.Shader {
    public class PixelShader : ShaderBase<PShader> {
        public PixelShader(ShaderSignature inputSignature, PShader shader, List<ConstantBuffer> constantBuffers, Dictionary<int, string> textures, Dictionary<int, string> samplers) 
            :base(shader, inputSignature,constantBuffers,textures, samplers) {
        }
    }
}
