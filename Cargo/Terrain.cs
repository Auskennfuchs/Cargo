using CargoEngine;
using CargoEngine.Scene;
using CargoEngine.Texture;
using SharpDX;
using SharpDX.Direct3D11;

namespace Cargo
{
    class Terrain : SceneNode
    {
        private const uint MAP_SIZE = 513;
        private const uint CHUNK_SIZE = 256;

        private CargoEngine.Texture.Texture2D texture;        
        private SamplerState sampler;

        private CargoEngine.Texture.Texture2D test;

        public Terrain() {
            var points = GenerateTerrain();

            var mapSize = MAP_SIZE;

            var triPoints = new Vector3[mapSize, mapSize];
            for (var z = 0; z < mapSize; z++) {
                for (var x = 0; x < mapSize; x++) {
                    triPoints[x, z] = new Vector3(x, (points[x, z] - 128.0f) * 0.5f, MAP_SIZE - z);
                }
            }

            var normals = CalcNormals(triPoints, mapSize);

            texture = TextureLoader.FromFile("assets/textures/textureGrid_1k.jpg");
            sampler = Renderer.Instance.CreateSamplerState(TextureAddressMode.Wrap, Filter.Anisotropic, 16);

            for (uint z = 0; z < MAP_SIZE / CHUNK_SIZE; z++) {
                for (uint x = 0; x < MAP_SIZE / CHUNK_SIZE; x++) {
                    var chunk = new TerrainChunk(triPoints, normals, CHUNK_SIZE, x * CHUNK_SIZE, z * CHUNK_SIZE, texture.SRV, sampler);
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
            var sizeFactor = 1.0f / 8.0f;
            for (var y = 0; y < mapSize; y++) {
                for (var x = 0; x < mapSize; x++) {
                    var normal = Vector3.Up;
                    if (x > 0 && y > 0 && x < mapSize - 1 && y < mapSize - 1) {
                        float nw = points[x - 1, y - 1].Y;
                        float n = points[x - 1, y].Y;
                        float ne = points[x - 1, y + 1].Y;
                        float e = points[x, y + 1].Y;
                        float se = points[x + 1, y + 1].Y;
                        float s = points[x + 1, y].Y;
                        float sw = points[x + 1, y - 1].Y;
                        float w = points[x, y - 1].Y;

                        float dydx = ((ne + 2 * e + se) - (nw + 2 * w + sw)) * sizeFactor;
                        float dydz = ((sw + 2 * s + se) - (nw + 2 * n + ne)) * sizeFactor;

                        normal = new Vector3(-dydx, 1.0f, -dydz);
                    }
/*                    if (x < mapSize - 1 && y < mapSize - 1) {
                        var v1 = points[x, y] - points[x + 1, y];
                        var v2 = points[x, y] - points[x, y + 1];
                        normal += Vector3.Normalize(Vector3.Cross(v1, v2));
                    }
                    if (x > 0 && y < mapSize - 1) {
                        var v1 = points[x - 1, y] - points[x, y];
                        var v2 = points[x, y] - points[x, y + 1];
                        normal += Vector3.Normalize(Vector3.Cross(v1, v2));
                    }

                    if (x > 0 && y > 0) {
                        var v1 = points[x - 1, y] - points[x, y];
                        var v2 = points[x, y - 1] - points[x, y];
                        normal += Vector3.Normalize(Vector3.Cross(v1, v2));
                    }

                    if (x < mapSize - 1 && y > 0) {
                        var v1 = points[x, y] - points[x + 1, y];
                        var v2 = points[x, y - 1] - points[x, y];
                        normal += Vector3.Normalize(Vector3.Cross(v1, v2));
                    }*/

                    normals[x, y] = Vector3.Normalize(normal);
                }
            }
            return normals;
        }
    }

    class TerrainChunk : SceneNode
    {
        public TerrainChunk(Vector3[,] points, Vector3[,] normals, uint chunkSize, uint offsetX, uint offsetY, ShaderResourceView texture, SamplerState sampler) {
            var mapSize = chunkSize + 1;
            var triPoints = new Vector3[mapSize * mapSize];
            var triNormals = new Vector3[mapSize * mapSize];
            var triUV = new Vector2[mapSize * mapSize];
            for (var z = 0; z < mapSize; z++) {
                for (var x = 0; x < mapSize; x++) {
                    triPoints[x + z * mapSize] = points[x + offsetX, z + offsetY];
                    triNormals[x + z * mapSize] = normals[x + offsetX, z + offsetY];
                    triUV[x + z * mapSize] = new Vector2((float)x / mapSize, (float)z / mapSize);
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
            geo.Executor.Geometry.UVs = triUV;
            geo.Executor.Geometry.Topology = Topology.TriangleList;
            geo.Executor.RenderParameter.SetParameter("AlbedoTexture", texture);
            geo.Executor.RenderParameter.SetParameter("Sampler", sampler);
            this.AddComponent(geo);
        }
    }
}
