﻿using System.Collections.Generic;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

using VShader = SharpDX.Direct3D11.VertexShader;

namespace CargoEngine.Shader {
    public class VertexShader : ShaderBase<VShader> {

        private Dictionary<int, InputLayout> inputLayouts = new Dictionary<int, InputLayout>();

        public VertexShader(ShaderSignature inputSignature, VShader shader, List<ConstantBuffer> constantBuffers, Dictionary<int, string> textures, Dictionary<int, string> samplers)
            : base(shader, inputSignature, constantBuffers, textures, samplers) { 
        }

        internal InputLayout GetInputLayout(InputElementList elements) {
            if(elements==null || elements.HashCode==0) {
                return null;
            }
            if(inputLayouts.ContainsKey(elements.HashCode)) {
                return inputLayouts[elements.HashCode];
            }
            return AddInputLayout(elements);
        }

        public new void Dispose() {
            base.Dispose();
            foreach(var il in inputLayouts) {
                if (il.Value != null) {
                    il.Value.Dispose();
                }
            }
            inputLayouts.Clear();
        }

        private InputLayout AddInputLayout(InputElementList elements) {
            var il = new InputLayout(Renderer.Instance.Device, InputSignature.Data, elements.InputElements.ToArray());
            inputLayouts[elements.HashCode] = il;
            return il;
        }
    }
}
