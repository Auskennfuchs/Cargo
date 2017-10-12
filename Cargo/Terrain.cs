using System;
using CargoEngine;
using CargoEngine.Scene;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Cargo {
    class Terrain : SceneNode {
        private const int MAP_SIZE = 513;
        private const int CHUNK_SIZE = 64;
        public Terrain() {
            var points = GenerateTerrain();

            var mapSize = MAP_SIZE;

            var triPoints = new Vector3[mapSize, mapSize];
            for (var z = 0; z < mapSize; z++) {
                for (var x = 0; x < mapSize; x++) {
                    triPoints[x, z] = new Vector3(x, points[x, z] - 128.0f, z);
                }
            }

            var normals = CalcNormals(triPoints, mapSize);

            for (var z = 0; z < MAP_SIZE / CHUNK_SIZE; z++) {
                for (var x = 0; x < MAP_SIZE / CHUNK_SIZE; x++) {
                    var chunk = new TerrainChunk(triPoints, normals, CHUNK_SIZE, x * CHUNK_SIZE, z * CHUNK_SIZE);
                    this.AddChild(chunk);
                }
            }
        }

        private float[,] GenerateTerrain() {
            var points = new float[MAP_SIZE, MAP_SIZE];
            var quality = 2.0f;
            var zVal = 12.0f;
            for (var j = 0; j < 4; j++) {
                for (var y = 0; y < MAP_SIZE; y++) {
                    for (var x = 0; x < MAP_SIZE; x++) {
                        points[x, y] += (float)Perlin.perlin(x / quality, y / quality, zVal / quality) * quality;
                    }
                }
                quality *= 4.0f;
            }

            return points;
        }

        private Vector3[,] CalcNormals(Vector3[,] points, int mapSize) {
            var normals = new Vector3[mapSize, mapSize];

            for (var y = 0; y < mapSize; y++) {
                for (var x = 0; x < mapSize; x++) {
                    var normal = Vector3.Up;
                    if (x < mapSize - 1 && y < mapSize - 1) {
                        var v1 = points[x, y] - points[x + 1, y];
                        var v2 = points[x, y] - points[x, y + 1];
                        normal += Vector3.Cross(v2, v1);
                    }
                    if (x > 0 && y < mapSize - 1) {
                        var v1 = points[x - 1, y] - points[x, y];
                        var v2 = points[x, y] - points[x, y + 1];
                        normal += Vector3.Cross(v2, v1);
                    }

                    if (x > 0 && y > 0) {
                        var v1 = points[x - 1, y] - points[x, y];
                        var v2 = points[x, y - 1] - points[x, y];
                        normal += Vector3.Cross(v2, v1);
                    }

                    if (x < mapSize - 1 && y > 0) {
                        var v1 = points[x, y] - points[x + 1, y];
                        var v2 = points[x, y - 1] - points[x, y];
                        normal += Vector3.Cross(v2, v1);
                    }

                    normal.Normalize();

                    normals[x, y] = normal;
                }
            }
            return normals;
        }
    }

    class TerrainChunk : SceneNode {
        public TerrainChunk(Vector3[,] points, Vector3[,] normals, int chunkSize, int offsetX, int offsetY) {
            var mapSize = chunkSize + 1;
            var triPoints = new Vector3[mapSize * mapSize];
            var triNormals = new Vector3[mapSize * mapSize];
            for (var z = 0; z < mapSize; z++) {
                for (var x = 0; x < mapSize; x++) {
                    triPoints[x + z * mapSize] = points[x + offsetX, z + offsetY];
                    triNormals[x + z * mapSize] = normals[x + offsetX, z + offsetY];
                }
            }

            var indices = new int[(mapSize - 1) * (mapSize - 1) * 6];
            var count = 0;
            for (var y = 0; y < mapSize - 1; y++) {
                for (var x = 0; x < mapSize - 1; x++) {
                    indices[count++] = x + y * mapSize;
                    indices[count++] = x + (y + 1) * mapSize;
                    indices[count++] = x + 1 + y * mapSize;

                    indices[count++] = x + (y + 1) * mapSize;
                    indices[count++] = x + 1 + (y + 1) * mapSize;
                    indices[count++] = x + 1 + y * mapSize;
                }
            }

            var geo = new GeometryComponent();
            geo.Executor.Geometry.AddBuffer(Buffer.Create<Vector3>(Renderer.Instance.Device, BindFlags.VertexBuffer, triPoints), "POSITION", Format.R32G32B32_Float, Utilities.SizeOf<Vector3>());
            geo.Executor.Geometry.AddBuffer(Buffer.Create<Vector3>(Renderer.Instance.Device, BindFlags.VertexBuffer, triNormals), "NORMAL", Format.R32G32B32_Float, Utilities.SizeOf<Vector3>());
            geo.Executor.Geometry.SetIndexBuffer(Buffer.Create<int>(Renderer.Instance.Device, BindFlags.IndexBuffer, indices), indices.Length);
            geo.Executor.Geometry.Topology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            this.AddComponent(geo);
        }
    }
}
