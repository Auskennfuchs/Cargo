using System.Collections.Generic;
using CargoEngine.Exception;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace CargoEngine {
    public class Geometry {
        public List<Buffer> Buffers {
            get; private set;
        }

        public Buffer Indices {
            get; private set;
        }

        public List<InputElement> Elements {
            get; private set;
        }

        public List<VertexBufferBinding> BufferBindings {
            get; private set;
        }

        public int NumIndices {
            get; private set;
        }

        public int VertexCount {
            get; private set;
        }

        public PrimitiveTopology Topology {
            get; set;
        }

        public Geometry() {
            Buffers = new List<Buffer>();
            Elements = new List<InputElement>();
            BufferBindings = new List<VertexBufferBinding>();
            Topology = PrimitiveTopology.TriangleList;
        }

        ~Geometry() {
            Buffers.ForEach(buf => { buf.Dispose(); });
            Buffers.Clear();
            Elements.Clear();
            BufferBindings.Clear();
            if(Indices!=null) {
                Indices.Dispose();
            }
        }

        public void AddBuffer(Buffer buf, string inputName, Format format, int stride) {
            var elementCount = buf.Description.SizeInBytes / stride;
            if(Buffers.Count==0) {
                VertexCount = elementCount;
            } else {
                if(elementCount!=VertexCount) {
                    throw CargoEngineException.Create("elementCount of Buffer doesn't match Vertexcount");
                }
            }
            int index = 0;
            Elements.ForEach(element => {
                if (element.SemanticName.Equals(inputName)) {
                    index++;
                }
            });
            var el = new InputElement(inputName, index, format, Buffers.Count);
            Elements.Add(el);
            Buffers.Add(buf);

            var bufferBinding = new VertexBufferBinding(buf, stride, 0);
            BufferBindings.Add(bufferBinding);
        }

        public void SetIndexBuffer(Buffer indices, int numIndices) {
            Indices = indices;
            NumIndices = numIndices;
        }

        public void Apply(RenderPipeline pipeline) {
            pipeline.InputAssembler.PrimitiveTopology = Topology;
            pipeline.InputAssembler.VertexBuffer.SetStates(0, BufferBindings.ToArray());
        }
    }
}
