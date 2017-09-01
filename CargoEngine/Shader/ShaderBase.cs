using System.Collections.Generic;
using CargoEngine.Parameter;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using CBuffer = SharpDX.D3DCompiler.ConstantBuffer;
using Buffer = SharpDX.Direct3D11.Buffer;
using CargoEngine.Exception;

namespace CargoEngine.Shader {
    public abstract class ShaderBase {
        private static int ShaderIdCounter = 1;

        public List<ConstantBuffer> ConstantBuffers {
            get; private set;
        }

        public ShaderSignature InputSignature {
            get; protected set;
        }

        public int ID {
            get; private set;
        }

        public Dictionary<int, string> Textures {
            get; private set;
        }

        public ShaderBase() {
            ID = ShaderIdCounter++;
            ConstantBuffers = new List<ConstantBuffer>();
            Textures = new Dictionary<int, string>();
        }

        ~ShaderBase() {
            Dispose();
        }

        public void Dispose() {
            if (InputSignature != null) {
                InputSignature.Dispose();
            }
            if (ConstantBuffers != null) {
                foreach (ConstantBuffer cb in ConstantBuffers) {
                    cb.Dispose();
                }
            }
        }

        public abstract void Apply(DeviceContext context, ParameterManager paramManager);

        public void SetParameterMatrix(string name, Matrix m) {
            foreach (ConstantBuffer cb in ConstantBuffers) {
                cb.SetParameterMatrix(name, m);
            }
        }

        protected void ReflectBytecode(Renderer renderer, ShaderBytecode bytecode) {
            using (var reflection = new ShaderReflection(bytecode)) {
                ReflectResources(reflection);
                ReflectConstantBuffers(renderer, reflection);
            }
        }

        private void ReflectResources(ShaderReflection reflection) {
            for (var resIndex = 0; resIndex < reflection.Description.BoundResources; resIndex++) {
                var resDesc = reflection.GetResourceBindingDescription(resIndex);
                switch (resDesc.Type) {
                    case ShaderInputType.Texture:
                        Textures[resDesc.BindPoint] = resDesc.Name;
                        break;
                }
            }
        }

        private void ReflectConstantBuffers(Renderer renderer, ShaderReflection reflection) {
            for (int cBufferIndex = 0; cBufferIndex < reflection.Description.ConstantBuffers; cBufferIndex++) {
                CBuffer cb = reflection.GetConstantBuffer(cBufferIndex);
                Buffer buf = new Buffer(renderer.Device, cb.Description.Size, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, sizeof(float));
                ConstantBuffer constantBuffer = new ConstantBuffer(buf);
                for (int i = 0; i < cb.Description.VariableCount; i++) {
                    var refVar = cb.GetVariable(i);
                    var type = refVar.GetVariableType();
                    switch (type.Description.Type) {
                        case ShaderVariableType.Float:
                            if (type.Description.RowCount == 4 && type.Description.ColumnCount == 4) {
                                var matParam = new MatrixParameter(refVar.Description.StartOffset);
                                if (matParam.Size != refVar.Description.Size) {
                                    throw CargoEngineException.Create("Error ConstantBufferParamtersize");
                                }
                                constantBuffer.AddParameter(refVar.Description.Name, matParam);
                            }
                            if (type.Description.RowCount == 1 && type.Description.ColumnCount == 3) {
                                var vec3Param = new Vector3Parameter(refVar.Description.StartOffset);
                                if (vec3Param.Size != refVar.Description.Size) {
                                    throw CargoEngineException.Create("Error ConstantBufferParamtersize");
                                }
                                constantBuffer.AddParameter(refVar.Description.Name, vec3Param);
                            }
                            break;
                    }
                }
                ConstantBuffers.Add(constantBuffer);
            }
        }
    }
}
