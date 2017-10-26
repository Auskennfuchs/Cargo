using SharpDX;
using System.Collections.Generic;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.DXGI;
using CargoEngine.Exception;
using System;
using SharpDX.Direct3D;
using System.Linq;

namespace CargoEngine
{

    public enum Topology
    {
        TriangleList = PrimitiveTopology.TriangleList,
        TriangleStrip = PrimitiveTopology.TriangleStrip,
        PointList = PrimitiveTopology.PointList,
        LineList = PrimitiveTopology.LineList,
    }

    public class Mesh : IDisposable
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

        private Vector3[] binormals;
        public Vector3[] BiNormals {
            get {
                return binormals;
            }
            set {
                binormals = value;
                Modified = true;
            }
        }
        private Vector3[] tangents;
        public Vector3[] Tangents {
            get {
                return tangents;
            }
            set {
                tangents = value;
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

        private readonly Object thisLock = new Object();

        public Mesh() {

        }

        public void Clear() {
            buffers.ForEach(buf => {
                if (buf.Buffer != null) {
                    buf.Buffer.Dispose();
                }
            });
            buffers.Clear();
            if (indexBuffer != null) {
                indexBuffer.Dispose();
            }
            VertexCount = 0;
            NumIndices = 0;
            InputElements.Clear();
        }

        public void Apply(RenderPipeline pipeline) {
            UpdateBuffers();
            pipeline.InputAssembler.PrimitiveTopology = (PrimitiveTopology)Topology;
            pipeline.InputAssembler.IndexBuffer = indexBuffer;
            pipeline.InputAssembler.IndexBufferFormat = indexBufferFormat;
            pipeline.InputAssembler.VertexBuffer.SetStates(0, buffers.ToArray());
            pipeline.InputAssembler.InputElements = InputElements;
        }

        public void GenerateNormals() {
            if (indices == null || vertices == null) {
                return;
            }
            normals = new Vector3[vertices.Length];
            for (var i = 0; i < normals.Length; i++) {
                normals[i] = Vector3.Up;
            }
            for (var i = 0; i < indices.Length; i += 3) {
                var p1 = indices[i];
                var p2 = indices[i + 1];
                var p3 = indices[i + 2];
                var v1 = vertices[p1];
                var v2 = vertices[p2];
                var v3 = vertices[p3];

                var u = Vector3.Normalize(v2 - v1);
                var v = Vector3.Normalize(v3 - v1);
                normals[p1] += Vector3.Cross(u, v);
            }
            foreach (var n in normals) {
                n.Normalize();
            }
            Modified = true;
        }

        private void UpdateBuffers() {
            lock (thisLock) {
                if (!Modified) {
                    // nothing to do here
                    return;
                }
                Clear();

                if (Vertices != null && Vertices.Length > 0) {
                    AddBuffer(Buffer.Create(Renderer.Instance.Device, BindFlags.VertexBuffer, Vertices), "POSITION", Format.R32G32B32_Float);
                }
                if (Normals != null && Normals.Length > 0) {
                    AddBuffer(Buffer.Create(Renderer.Instance.Device, BindFlags.VertexBuffer, Normals), "NORMAL", Format.R32G32B32_Float);
                }
                if (UVs != null && UVs.Length > 0) {
                    AddBuffer(Buffer.Create(Renderer.Instance.Device, BindFlags.VertexBuffer, UVs), "TEXCOORD", Format.R32G32_Float);
                }
                if (BiNormals != null && BiNormals.Length > 0) {
                    AddBuffer(Buffer.Create(Renderer.Instance.Device, BindFlags.VertexBuffer, BiNormals), "BINORMAL", Format.R32G32B32_Float);
                }
                if (Tangents != null && Tangents.Length > 0) {
                    AddBuffer(Buffer.Create(Renderer.Instance.Device, BindFlags.VertexBuffer, Tangents), "TANGENT", Format.R32G32B32_Float);
                }

                if (Indices != null && Indices.Length > 0) {
                    NumIndices = Indices.Length;
                    if (VertexCount <= ushort.MaxValue) {
                        indexBuffer = Buffer.Create(Renderer.Instance.Device, BindFlags.IndexBuffer, Array.ConvertAll(Indices, i => (ushort)i));
                        indexBufferFormat = Format.R16_UInt;
                    }
                    else {
                        indexBuffer = Buffer.Create(Renderer.Instance.Device, BindFlags.IndexBuffer, Indices);
                        indexBufferFormat = Format.R32_UInt;
                    }
                }
                Modified = false;
            }
        }

        private void AddBuffer(Buffer buf, string inputName, Format format, int stride = 0) {
            if (stride == 0) {
                stride = format.SizeOfInBytes();
            }
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

        public void Dispose() {
            Clear();
        }
    }
}
