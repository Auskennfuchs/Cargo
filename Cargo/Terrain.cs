using CargoEngine;
using CargoEngine.Scene;
using SharpDX;

namespace Cargo
{
    class Terrain : SceneNode
    {
        private const uint MAP_SIZE = 513;
        private const uint CHUNK_SIZE = 256;
        public Terrain() {
            var points = GenerateTerrain();

            var mapSize = MAP_SIZE;

            var triPoints = new Vector3[mapSize, mapSize];
            for (var z = 0; z < mapSize; z++) {
                for (var x = 0; x < mapSize; x++) {
                    triPoints[x, z] = new Vector3(x, points[x, z] - 128.0f, MAP_SIZE - z);
                }
            }

            var normals = CalcNormals(triPoints, mapSize);

            for (uint z = 0; z < MAP_SIZE / CHUNK_SIZE; z++) {
                for (uint x = 0; x < MAP_SIZE / CHUNK_SIZE; x++) {
                    var chunk = new TerrainChunk(triPoints, normals, CHUNK_SIZE, x * CHUNK_SIZE, z * CHUNK_SIZE);
                    this.AddChild(chunk);
                }
            }
        }

        private float[,] GenerateTerrain() {
            var points = new float[MAP_SIZE, MAP_SIZE];
            var quality = 1.0f;
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

        private Vector3[,] CalcNormals(Vector3[,] points, uint mapSize) {
            var normals = new Vector3[mapSize, mapSize];

            for (var y = 0; y < mapSize; y++) {
                for (var x = 0; x < mapSize; x++) {
                    var normal = Vector3.Up;
                    if (x < mapSize - 1 && y < mapSize - 1) {
                        var v1 = points[x, y] - points[x + 1, y];
                        var v2 = points[x, y] - points[x, y + 1];
                        normal += Vector3.Cross(v1, v2);
                    }
                    if (x > 0 && y < mapSize - 1) {
                        var v1 = points[x - 1, y] - points[x, y];
                        var v2 = points[x, y] - points[x, y + 1];
                        normal += Vector3.Cross(v1, v2);
                    }

                    if (x > 0 && y > 0) {
                        var v1 = points[x - 1, y] - points[x, y];
                        var v2 = points[x, y - 1] - points[x, y];
                        normal += Vector3.Cross(v1, v2);
                    }

                    if (x < mapSize - 1 && y > 0) {
                        var v1 = points[x, y] - points[x + 1, y];
                        var v2 = points[x, y - 1] - points[x, y];
                        normal += Vector3.Cross(v1, v2);
                    }

                    normal.Normalize();

                    normals[x, y] = normal;
                }
            }
            return normals;
        }
    }

    class TerrainChunk : SceneNode
    {
        public TerrainChunk(Vector3[,] points, Vector3[,] normals, uint chunkSize, uint offsetX, uint offsetY) {
            var mapSize = chunkSize + 1;
            var triPoints = new Vector3[mapSize * mapSize];
            var triNormals = new Vector3[mapSize * mapSize];
            for (var z = 0; z < mapSize; z++) {
                for (var x = 0; x < mapSize; x++) {
                    triPoints[x + z * mapSize] = points[x + offsetX, z + offsetY];
                    triNormals[x + z * mapSize] = normals[x + offsetX, z + offsetY];
                }
            }

            var indices = new uint[(mapSize - 1) * (mapSize - 1) * 6];
            var count = 0;
            for (uint y = 0; y < mapSize - 1; y++) {
                for (uint x = 0; x < mapSize - 1; x++) {
                    indices[count++] = x + y * mapSize;
                    indices[count++] = x + 1 + y * mapSize;
                    indices[count++] = x + (y + 1) * mapSize;

                    indices[count++] = x + (y + 1) * mapSize;
                    indices[count++] = x + 1 + y * mapSize;
                    indices[count++] = x + 1 + (y + 1) * mapSize;
                }
            }

            var geo = new GeometryComponent();
            geo.Executor.Geometry.Vertices = triPoints;
            geo.Executor.Geometry.Normals = triNormals;
            geo.Executor.Geometry.Indices = indices;
            geo.Executor.Geometry.Topology = Topology.TriangleList;
            this.AddComponent(geo);
        }
    }
}
