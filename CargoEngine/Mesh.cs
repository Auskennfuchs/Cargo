using SharpDX;
using System.Collections.Generic;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.DXGI;
using CargoEngine.Exception;
using System;
using SharpDX.Direct3D;

namespace CargoEngine
{

    public enum Topology
    {
        TriangleList = PrimitiveTopology.TriangleList,
        TriangleStrip = PrimitiveTopology.TriangleStrip,
        PointList = PrimitiveTopology.PointList,
    }

    public class Mesh
    {
        #region Properties

        public bool Modified {
            get; private set;
        }

        private Vector3[] vertices;
        public Vector3[] Vertices {
            get {
                return vertices;
            }
            set {
                vertices = value;
                Modified = true;
            }
        }

        private Vector3[] normals;
        public Vector3[] Normals {
            get {
                return normals;
            }
            set {
                normals = value;
                Modified = true;
            }
        }

        private Vector2[] uvs;
        public Vector2[] UVs {
            get {
                return uvs;
            }
            set {
                uvs = value;
                Modified = true;
            }
        }

        public int VertexCount {
            get; private set;
        }

        private uint[] indices;
        public uint[] Indices {
            get {
                return indices;
            }
            set {
                indices = value;
                Modified = true;
            }
        }

        public int NumIndices {
            get; private set;
        }

        internal int InputElementHash {
            get; private set;
        }

        internal InputElementList InputElements {
            get; private set;
        } = new InputElementList();

        public Topology Topology {
            get; set;
        } = Topology.TriangleList;

        #endregion

        private List<VertexBufferBinding> buffers = new List<VertexBufferBinding>();
        private Buffer indexBuffer;
        private Format indexBufferFormat;        

        public Mesh() {

        }

        public void Apply(RenderPipeline pipeline) {
            UpdateBuffers();
            pipeline.InputAssembler.PrimitiveTopology = (PrimitiveTopology)Topology;
            pipeline.InputAssembler.IndexBuffer = indexBuffer;
            pipeline.InputAssembler.IndexBufferFormat = indexBufferFormat;
            pipeline.InputAssembler.VertexBuffer.SetStates(0, buffers.ToArray());
            pipeline.InputAssembler.InputElements = InputElements;
        }

        private void UpdateBuffers() {
            if (!Modified) {
                // nothing to do here
                return;
            }
            Clear();
            Modified = false;

            if (Vertices != null && Vertices.Length > 0) {
                AddBuffer(Buffer.Create(Renderer.Instance.Device, BindFlags.VertexBuffer, Vertices),"POSITION",Format.R32G32B32_Float,Utilities.SizeOf<Vector3>());
            }
            if (Normals != null && Normals.Length > 0) {
                AddBuffer(Buffer.Create(Renderer.Instance.Device, BindFlags.VertexBuffer, Normals), "NORMAL", Format.R32G32B32_Float, Utilities.SizeOf<Vector3>());
            }
            if (UVs != null && UVs.Length > 0) {
                AddBuffer(Buffer.Create(Renderer.Instance.Device, BindFlags.VertexBuffer, UVs), "TEXCOORD", Format.R32G32_Float, Utilities.SizeOf<Vector2>());
            }

            if (Indices!=null && Indices.Length > 0) {
                NumIndices = Indices.Length;
                if (VertexCount <= ushort.MaxValue) {
                    indexBuffer = Buffer.Create(Renderer.Instance.Device, BindFlags.IndexBuffer, Array.ConvertAll(Indices, i => (ushort)i));
                    indexBufferFormat = Format.R16_UInt;
                } else {
                    indexBuffer = Buffer.Create(Renderer.Instance.Device, BindFlags.IndexBuffer, Indices);
                    indexBufferFormat = Format.R32_UInt;
                }
            }
        }

        public void Clear() {
            buffers.Clear();
            if (indexBuffer != null) {
                indexBuffer.Dispose();
            }
            VertexCount = 0;
            NumIndices = 0;
            InputElements.Clear();
        }

        private void AddBuffer(Buffer buf, string inputName, Format format, int stride) {
            var elementCount = buf.Description.SizeInBytes / stride;
            if (buffers.Count == 0) {
                VertexCount = elementCount;
            }
            else {
                if (elementCount != VertexCount) {
                    throw CargoEngineException.Create("elementCount of Buffer doesn't match Vertexcount");
                }
            }
            InputElements.AddElement(inputName, format, buffers.Count);

            var bufferBinding = new VertexBufferBinding(buf, stride, 0);
            buffers.Add(bufferBinding);
        }
    }
}
