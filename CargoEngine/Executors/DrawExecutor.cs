using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using VertexShader = CargoEngine.Shader.VertexShader;

namespace CargoEngine.Executors {
    public class DrawExecutor : Executor {
        public Geometry Geometry {
            get; private set;
        }

        private Dictionary<int, InputLayout> inputLayouts = new Dictionary<int, InputLayout>();

        public DrawExecutor() {
            Geometry = new Geometry();
        }

        public override void Dispose() {
            Geometry.Dispose();
        }

        public override void Render(RenderPipeline pipeline) {
            var vShader = pipeline.VertexShader.Shader;
            var inputLayout = GetInputLayout(vShader);
            if (inputLayout == null) {
                inputLayout = AddInputLayout(vShader, Geometry.Elements.ToArray());
            }

            pipeline.InputAssembler.ClearDesiredState();

            pipeline.InputAssembler.InputLayout = inputLayout;
            Geometry.Apply(pipeline);
            pipeline.ApplyInputResources();
            pipeline.ApplyShaderResources();
            if (Geometry.NumIndices > 0) {
                pipeline.DrawIndexed(Geometry.NumIndices, 0, 0);
            } else {
                pipeline.Draw(Geometry.VertexCount, 0);
            }
        }

        public InputLayout GetInputLayout(VertexShader vShader) {
            if (vShader != null && inputLayouts.ContainsKey(vShader.ID)) {
                return inputLayouts[vShader.ID];
            }
            return null;
        }

        public InputLayout AddInputLayout(VertexShader vShader, InputElement[] elements) {
            if (vShader == null) {
                return null;
            }
            var il = new InputLayout(Renderer.Instance.Device, vShader.InputSignature.Data, elements);
            inputLayouts[vShader.ID] = il;
            return il;
        }
    }
}
