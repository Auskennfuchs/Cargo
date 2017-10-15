using System.Windows.Forms;
using CargoEngine.Exception;
using SharpDX.D3DCompiler;
using CBuffer = SharpDX.D3DCompiler.ConstantBuffer;
using Buffer = SharpDX.Direct3D11.Buffer;
using VShader = SharpDX.Direct3D11.VertexShader;
using PShader = SharpDX.Direct3D11.PixelShader;
using System.Collections.Generic;
using System;
using SharpDX.Direct3D11;
using CargoEngine.Parameter;

namespace CargoEngine.Shader {
    public class ShaderLoader {
        private static Dictionary<string, VertexShader> vertexShaders = new Dictionary<string, VertexShader>();
        private static Dictionary<string, PixelShader> pixelShaders = new Dictionary<string, PixelShader>();

        public static VertexShader LoadVertexShader(Renderer renderer, string file, string entryfunction) {
            if(vertexShaders.ContainsKey(file)) {
                return vertexShaders[file];
            }
            var shader = LoadShader<VertexShader,VShader>(renderer, file, entryfunction, "vs_5_0");
            vertexShaders.Add(file, shader);
            return shader;
        }

        public static PixelShader LoadPixelShader(Renderer renderer, string file, string entryfunction) {
            if (pixelShaders.ContainsKey(file)) {
                return pixelShaders[file];
            }
            var shader = LoadShader<PixelShader,PShader>(renderer, file, entryfunction, "ps_5_0");
            pixelShaders.Add(file, shader);
            return shader;
        }

        private static T LoadShader<T,U>(Renderer renderer, string file, string entryfunction, string profile) where T: ShaderBase<U> where U: DeviceChild {
            try {
                ShaderFlags sFlags = ShaderFlags.PackMatrixRowMajor;
#if DEBUG
                sFlags |= ShaderFlags.Debug;
#endif
                using (var bytecode = ShaderBytecode.CompileFromFile(file, entryfunction, profile, sFlags, EffectFlags.None)) {
                    if (bytecode.Message != null) {
                        MessageBox.Show(bytecode.Message);
                        return null;
                    }
                    return ReflectBytecode<T,U>(renderer, bytecode);
                }
            }
            catch (System.Exception exc) {
                throw CargoEngineException.Create("Error loading Shader", exc);
            }
        }

        private static T ReflectBytecode<T,U>(Renderer renderer, ShaderBytecode bytecode) where T: ShaderBase<U> where U: DeviceChild {
            var inputSignature = ShaderSignature.GetInputSignature(bytecode);
            var shaderPtr = Activator.CreateInstance(typeof(U),new object[] { renderer.Device, bytecode.Data, null }) as U;
            using (var reflection = new ShaderReflection(bytecode)) {
                var (textures,samplers) = ReflectResources(reflection);
                var constantBuffers = ReflectConstantBuffers(renderer, reflection);
                return Activator.CreateInstance(typeof(T), new object[] { inputSignature, shaderPtr, constantBuffers, textures, samplers }) as T;
            }
        }

        private static (Dictionary<int,string>, Dictionary<int, string>) ReflectResources(ShaderReflection reflection) {
            var textures = new Dictionary<int, string>();
            var samplers = new Dictionary<int, string>();
            for (var resIndex = 0; resIndex < reflection.Description.BoundResources; resIndex++) {
                var resDesc = reflection.GetResourceBindingDescription(resIndex);
                switch (resDesc.Type) {
                    case ShaderInputType.Texture:
                        textures[resDesc.BindPoint] = resDesc.Name;
                        break;
                    case ShaderInputType.Sampler:
                        samplers[resDesc.BindPoint] = resDesc.Name;
                        break;
                }
            }
            return (textures,samplers);
        }

        private static List<ConstantBuffer> ReflectConstantBuffers(Renderer renderer, ShaderReflection reflection) {
            var constantBuffers = new List<ConstantBuffer>();
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
                constantBuffers.Add(constantBuffer);
            }
            return constantBuffers;
        }
    }
}
