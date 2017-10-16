using CargoEngine;
using CargoEngine.Scene;
using CargoEngine.Texture;
using SharpDX;
using SharpDX.Direct3D11;

namespace Cargo
{
    class Terrain : SceneNode
    {
        private const uint MAP_SIZE = 2;
        private const uint CHUNK_SIZE = 2;

        private CargoEngine.Texture.Texture2D texture;
        private SamplerState sampler;

        public Terrain() {
            var heightMap = GenerateTerrain();

            texture = TextureLoader.FromFile("assets/textures/textureGrid_1k.jpg");
            sampler = Renderer.Instance.CreateSamplerState(TextureAddressMode.Wrap, Filter.Anisotropic, 16);

            for (uint z = 0; z < MAP_SIZE / CHUNK_SIZE; z++) {
                for (uint x = 0; x < MAP_SIZE / CHUNK_SIZE; x++) {
                    var chunk = new TerrainChunk(heightMap, CHUNK_SIZE, x * CHUNK_SIZE, z * CHUNK_SIZE, texture.SRV, sampler);
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

                    normals[x, y] = Vector3.Normalize(normal);
                }
            }
            return normals;
        }
    }

    class TerrainChunk : SceneNode
    {
        public TerrainChunk(float[,] heightMap, uint chunkSize, uint offsetX, uint offsetY, ShaderResourceView texture, SamplerState sampler) {
            var mapSize = chunkSize;
            var triPoints = new Vector3[mapSize * mapSize * 9];
            var triNormals = new Vector3[mapSize * mapSize * 9];
            var triUV = new Vector2[mapSize * mapSize * 9];
            for (var z = 0; z < mapSize; z++) {
                for (var x = 0; x < mapSize; x++) {
                    triPoints[(x + z * mapSize) * 9 + 0] = new Vector3(x + offsetX - 0.5f, GetHeight(heightMap, x + offsetX, z + offsetY, -1, -1), z + offsetY - 0.5f);
                    triPoints[(x + z * mapSize) * 9 + 1] = new Vector3(x + offsetX - 0.0f, GetHeight(heightMap, x + offsetX, z + offsetY, 0, -1), z + offsetY - 0.5f);
                    triPoints[(x + z * mapSize) * 9 + 2] = new Vector3(x + offsetX + 0.5f, GetHeight(heightMap, x + offsetX, z + offsetY, 1, -1), z + offsetY - 0.5f);
                    triPoints[(x + z * mapSize) * 9 + 3] = new Vector3(x + offsetX - 0.5f, GetHeight(heightMap, x + offsetX, z + offsetY, -1, 0), z + offsetY - 0);
                    triPoints[(x + z * mapSize) * 9 + 4] = new Vector3(x + offsetX - 0.0f, GetHeight(heightMap, x + offsetX, z + offsetY, 0, 0), z + offsetY - 0);
                    triPoints[(x + z * mapSize) * 9 + 5] = new Vector3(x + offsetX + 0.5f, GetHeight(heightMap, x + offsetX, z + offsetY, 1, 0), z + offsetY - 0);
                    triPoints[(x + z * mapSize) * 9 + 6] = new Vector3(x + offsetX - 0.5f, GetHeight(heightMap, x + offsetX, z + offsetY, -1, 1), z + offsetY + 0.5f);
                    triPoints[(x + z * mapSize) * 9 + 7] = new Vector3(x + offsetX - 0.0f, GetHeight(heightMap, x + offsetX, z + offsetY, 0, 1), z + offsetY + 0.5f);
                    triPoints[(x + z * mapSize) * 9 + 8] = new Vector3(x + offsetX + 0.5f, GetHeight(heightMap, x + offsetX, z + offsetY, 1, 1), z + offsetY + 0.5f);
                }
            }

            var indices = new uint[mapSize * mapSize * 24];
            var count = 0;
            for (uint y = 0; y < mapSize; y++) {
                for (uint x = 0; x < mapSize; x++) {
                    indices[count++] = x + y * mapSize * 3;
                    indices[count++] = x + 1 + y * mapSize * 3;
                    indices[count++] = x + 1 + (y + 1) * mapSize * 3;

                    indices[count++] = x + y * mapSize * 3;
                    indices[count++] = x + (y + 1) * mapSize * 3;
                    indices[count++] = x + 1 + (y + 1) * mapSize * 3;

                    indices[count++] = x + 1 + y * mapSize * 3;
                    indices[count++] = x + 2 + y * mapSize * 3;
                    indices[count++] = x + 1 + (y + 1) * mapSize * 3;

                    indices[count++] = x + 1 + (y + 1) * mapSize * 3;
                    indices[count++] = x + 2 + y * mapSize * 3;
                    indices[count++] = x + 2 + (y + 1) * mapSize * 3;

                    indices[count++] = x + (y + 1) * mapSize * 3;
                    indices[count++] = x + 1 + (y + 1) * mapSize * 3;
                    indices[count++] = x + (y + 2) * mapSize * 3;

                    indices[count++] = x + (y + 2) * mapSize * 3;
                    indices[count++] = x + 1 + (y + 1) * mapSize * 3;
                    indices[count++] = x + 1 + (y + 2) * mapSize * 3;

                    indices[count++] = x + 1 + (y + 1) * mapSize * 3;
                    indices[count++] = x + 2 + (y + 1) * mapSize * 3;
                    indices[count++] = x + 2 + (y + 2) * mapSize * 3;

                    indices[count++] = x + 1 + (y + 1) * mapSize * 3;
                    indices[count++] = x + 1 + (y + 2) * mapSize * 3;
                    indices[count++] = x + 2 + (y + 2) * mapSize * 3;
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

        private float GetHeight(float[,] heightMap, long px, long py, int ox, int oy) {
            if (px + ox < 0) {
                ox = 0;
            }
            if (py + oy < 0) {
                oy = 0;
            }
            if (px + ox >= heightMap.GetLength(0)) {
                ox = 0;
            }
            if (py + oy >= heightMap.GetLength(1)) {
                oy = 0;
            }
            //            return (heightMap[px, py] + heightMap[px + ox, py + oy]) / 2.0f * 0.3f;
            return 0.0f;
        }
    }
}
