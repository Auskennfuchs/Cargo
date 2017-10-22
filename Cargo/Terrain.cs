using CargoEngine;
using CargoEngine.Scene;
using CargoEngine.Texture;
using SharpDX;
using SharpDX.Direct3D11;

namespace Cargo
{
    class Terrain : SceneNode
    {
        private const uint MAP_SIZE = 256;
        private const uint CHUNK_SIZE = 64;

        private CargoEngine.Texture.Texture2D texture;
        private SamplerState sampler;

        public Terrain() {
            var heightMap = GenerateTerrain();

            texture = TextureLoader.FromFile("assets/textures/textureGrid_1k.jpg");
            sampler = Renderer.Instance.CreateSamplerState(TextureAddressMode.Wrap, Filter.Anisotropic, Comparison.Never, 16);

            for (uint z = 0; z < MAP_SIZE / CHUNK_SIZE; z++) {
                for (uint x = 0; x < MAP_SIZE / CHUNK_SIZE; x++) {
                    var chunk = new TerrainChunk(heightMap, CHUNK_SIZE, x * CHUNK_SIZE, z * CHUNK_SIZE, texture.SRV, sampler);
                    this.AddChild(chunk);
                }
            }

        }

        ~Terrain() {
            if (texture != null) {
                texture.Dispose();
            }
            if (sampler != null) {
                sampler.Dispose();
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
            var fullMapSize = heightMap.GetLength(1) - 1;
            var numPoints = mapSize * 2 + 1;
            var triPoints = new Vector3[numPoints * numPoints];
            var triUV = new Vector2[numPoints * numPoints];
            for (var z = 0; z < numPoints; z++) {
                for (var x = 0; x < numPoints; x++) {
                    if (z % 2 == 0) {
                        if (x % 2 == 0) {
                            triPoints[x + z * numPoints] = new Vector3(x / 2 + offsetX - 0.5f,
                                GetHeight(heightMap, x / 2 + offsetX, z / 2 + offsetY, -1, -1),
                                fullMapSize - (z / 2 + offsetY - 0.5f));
                        }
                        else {
                            triPoints[x + z * numPoints] = new Vector3(x / 2 + offsetX - 0.0f,
                                GetHeight(heightMap, x / 2 + offsetX, z / 2 + offsetY, 0, -1),
                                fullMapSize - (z / 2 + offsetY - 0.5f));
                        }

                    }
                    else {
                        if (x % 2 == 0) {
                            triPoints[x + z * numPoints] = new Vector3(x / 2 + offsetX - 0.5f,
                                GetHeight(heightMap, x / 2 + offsetX, z / 2 + offsetY, -1, 0),
                                fullMapSize - (z / 2 + offsetY - 0));
                        }
                        else {
                            triPoints[x + z * numPoints] = new Vector3(x / 2 + offsetX - 0.0f,
                                GetHeight(heightMap, x / 2 + offsetX, z / 2 + offsetY, 0, 0),
                                fullMapSize - (z / 2 + offsetY - 0));
                        }
                    }
                    triUV[x + z * numPoints] = new Vector2(x / (numPoints - 1.0f), z / (numPoints - 1.0f));
                }
            }

            var indices = new uint[mapSize * mapSize * 24];
            var count = 0;
            for (uint y = 0; y < numPoints - 1; y++) {
                for (uint x = 0; x < numPoints - 1; x++) {
                    var mapSize2 = numPoints;
                    var fx = x;
                    if ((x % 2 == 0 && y % 2 == 0) || (x % 2 != 0 && y % 2 != 0)) {
                        indices[count++] = fx + y * mapSize2;
                        indices[count++] = fx + 1 + y * mapSize2;
                        indices[count++] = fx + 1 + (y + 1) * mapSize2;

                        indices[count++] = fx + y * mapSize2;
                        indices[count++] = fx + 1 + (y + 1) * mapSize2;
                        indices[count++] = fx + (y + 1) * mapSize2;

                    }
                    if ((x % 2 != 0 && y % 2 == 0) || (x % 2 == 0 && y % 2 != 0)) {
                        indices[count++] = fx + y * mapSize2;
                        indices[count++] = fx + 1 + y * mapSize2;
                        indices[count++] = fx + (y + 1) * mapSize2;

                        indices[count++] = fx + (y + 1) * mapSize2;
                        indices[count++] = fx + 1 + y * mapSize2;
                        indices[count++] = fx + 1 + (y + 1) * mapSize2;
                    }
                }
            }

            var geo = new GeometryComponent();
            geo.Executor.Geometry.Vertices = triPoints;
            geo.Executor.Geometry.Normals = CalcNormals(triPoints, numPoints);
            geo.Executor.Geometry.Indices = indices;
            geo.Executor.Geometry.UVs = triUV;
            geo.Executor.Geometry.Topology = Topology.TriangleList;
            geo.Executor.RenderParameter.SetParameter("AlbedoTexture", texture);
            geo.Executor.RenderParameter.SetParameter("Sampler", sampler);
            this.AddComponent(geo);
        }

        private float GetHeight(float[,] heightMap, long px, long py, int ox, int oy) {
            if (px >= heightMap.GetLength(0)) {
                px = heightMap.GetLength(0) - 1;
            }
            if (py >= heightMap.GetLength(1)) {
                py = heightMap.GetLength(1) - 1;
            }
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
            return (heightMap[px, py] + heightMap[px + ox, py + oy]) / 2.0f;
        }

        private Vector3[] CalcNormals(Vector3[] points, uint mapSize) {
            var normals = new Vector3[mapSize * mapSize];
            for (var y = 0; y < mapSize; y++) {
                for (var x = 0; x < mapSize; x++) {
                    var normal = Vector3.Up;
                    float nw, n, ne, e, se, s, sw, w;
                    nw = n = ne = e = se = s = sw = w = n = points[x + y * mapSize].Y;
                    var faces = 0;
                    if (x > 0) {
                        n = points[x - 1 + (y) * mapSize].Y;
                        faces++;
                    }
                    if (x < mapSize - 1) {
                        s = points[x + 1 + (y) * mapSize].Y;
                        faces++;
                    }
                    if (y > 0) {
                        w = points[x + (y - 1) * mapSize].Y;
                        faces++;
                    }
                    if (y < mapSize - 1) {
                        e = points[x + (y + 1) * mapSize].Y;
                        faces++;
                    }
                    if (x > 0 && y > 0 && x < mapSize - 1 && y < mapSize - 1) {
                        nw = points[x - 1 + (y - 1) * mapSize].Y;
                        ne = points[x - 1 + (y + 1) * mapSize].Y;
                        se = points[x + 1 + (y + 1) * mapSize].Y;
                        sw = points[x + 1 + (y - 1) * mapSize].Y;
                        faces += 4;
                    }
                    var sizeFactor = 1.0f / 8.0f;
                    float dydx = ((ne + 2 * e + se) - (nw + 2 * w + sw)) * sizeFactor;
                    float dydz = ((sw + 2 * s + se) - (nw + 2 * n + ne)) * sizeFactor;

                    normal = new Vector3(-dydx, 1.0f, -dydz);

                    normals[x + y * mapSize] = Vector3.Normalize(normal);
                }
            }
            return normals;
        }

    }
}
