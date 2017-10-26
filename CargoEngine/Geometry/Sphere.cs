using CargoEngine.Scene;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoEngine.Geometry
{
    public class Sphere : SceneNode
    {
        public Sphere(float diameter = 1.0f, int tessellation = 16) {
            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            var vertices = new Vector3[(verticalSegments + 1) * (horizontalSegments + 1)];
            var normals = new Vector3[(verticalSegments + 1) * (horizontalSegments + 1)];
            var uvs = new Vector2[(verticalSegments + 1) * (horizontalSegments + 1)];
            var indices = new uint[(verticalSegments) * (horizontalSegments + 1) * 6];

            float radius = diameter / 2;

            int vertexCount = 0;
            // Create rings of vertices at progressively higher latitudes.
            for (int i = 0; i <= verticalSegments; i++) {
                float v = 1.0f - (float)i / verticalSegments;

                var latitude = (float)((i * Math.PI / verticalSegments) - Math.PI / 2.0);
                var dy = (float)Math.Sin(latitude);
                var dxz = (float)Math.Cos(latitude);

                // Create a single ring of vertices at this latitude.
                for (int j = 0; j <= horizontalSegments; j++) {
                    float u = (float)j / horizontalSegments;

                    var longitude = (float)(j * 2.0 * Math.PI / horizontalSegments);
                    var dx = (float)Math.Sin(longitude);
                    var dz = (float)Math.Cos(longitude);

                    dx *= dxz;
                    dz *= dxz;

                    var normal = new Vector3(dx, dy, dz);
                    var textureCoordinate = new Vector2(u, v);

                    vertices[vertexCount] = normal * radius;
                    normals[vertexCount] = normal;
                    uvs[vertexCount] = textureCoordinate;
                    vertexCount++;
                }
            }

            // Fill the index buffer with triangles joining each pair of latitude rings.
           uint stride = (uint)horizontalSegments + 1;

            int indexCount = 0;
            for (uint i = 0; i < verticalSegments; i++) {
                for (uint j = 0; j <= horizontalSegments; j++) {
                    uint nextI = i + 1;
                    uint nextJ = (j + 1) % stride;

                    indices[indexCount++] = (i * stride + j);
                    indices[indexCount++] = (i * stride + nextJ);
                    indices[indexCount++] = (nextI * stride + j);

                    indices[indexCount++] = (i * stride + nextJ);
                    indices[indexCount++] = (nextI * stride + nextJ);
                    indices[indexCount++] = (nextI * stride + j);
                }
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
