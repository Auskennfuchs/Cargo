using CargoEngine.Scene;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoEngine.Geometry
{
    public class Cube : SceneNode
    {
        private const int CubeFaceCount = 6;

        private static readonly Vector3[] faceNormals = new Vector3[CubeFaceCount]
            {
                    new Vector3(0, 0, 1),
                    new Vector3(0, 0, -1),
                    new Vector3(1, 0, 0),
                    new Vector3(-1, 0, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(0, -1, 0),
            };

        private static readonly Vector2[] textureCoordinates = new Vector2[4]
            {
                    new Vector2(1, 0),
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
            };

        public Cube(float size = 1.0f) {
            var vertices = new Vector3[CubeFaceCount * 4];
            var normals = new Vector3[CubeFaceCount * 4];
            var uvs = new Vector2[CubeFaceCount * 4];
            var indices = new uint[CubeFaceCount * 6];

            size /= 2.0f;

            int vertexCount = 0;
            uint indexCount = 0;
            // Create each face in turn.
            for (uint i = 0; i < CubeFaceCount; i++) {
                Vector3 normal = faceNormals[i];

                // Get two vectors perpendicular both to the face normal and to each other.
                Vector3 basis = (i >= 4) ? Vector3.UnitZ : Vector3.UnitY;

                Vector3 side1;
                Vector3.Cross(ref normal, ref basis, out side1);

                Vector3 side2;
                Vector3.Cross(ref normal, ref side1, out side2);

                // Six indices (two triangles) per face.
                uint vbase = i * 4;
                indices[indexCount++] = (vbase + 0);
                indices[indexCount++] = (vbase + 2);
                indices[indexCount++] = (vbase + 1);

                indices[indexCount++] = (vbase + 0);
                indices[indexCount++] = (vbase + 3);
                indices[indexCount++] = (vbase + 2);

                // Four vertices per face.
                vertices[vertexCount] = (normal - side1 - side2) * size;
                normals[vertexCount] = normal;
                uvs[vertexCount] = textureCoordinates[0];
                vertexCount++;
                vertices[vertexCount] = (normal - side1 + side2) * size;
                normals[vertexCount] = normal;
                uvs[vertexCount] = textureCoordinates[1];
                vertexCount++;
                vertices[vertexCount] = (normal + side1 + side2) * size;
                normals[vertexCount] = normal;
                uvs[vertexCount] = textureCoordinates[2];
                vertexCount++;
                vertices[vertexCount] = (normal + side1 - side2) * size;
                normals[vertexCount] = normal;
                uvs[vertexCount] = textureCoordinates[3];
                vertexCount++;

            }

            var geo = new GeometryComponent();
            geo.Executor.Geometry.Vertices = vertices;
            geo.Executor.Geometry.Normals = normals;
            geo.Executor.Geometry.UVs = uvs;
            geo.Executor.Geometry.Indices = indices;
            geo.Executor.Geometry.Topology = CargoEngine.Topology.TriangleList;

            this.AddComponent(geo);
        }
    }
}
