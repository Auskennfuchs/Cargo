using System;
using CargoEngine;
using CargoEngine.Scene;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Cargo {
    class Terrain : SceneNode {
        private const int MAP_SIZE = 256;
        public Terrain() {
            var points = new float[MAP_SIZE, MAP_SIZE];
            for(var y = 0; y < MAP_SIZE; y++) {
                for (var x = 0; x < MAP_SIZE; x++) {
                    points[x,y] = (float)(Math.Sin(x * 0.3)+Math.Sin(y*0.1))*3.0f;
                }
            }

            var triPoints = new Vector3[(MAP_SIZE) * (MAP_SIZE)];
            for (var z = 0; z < MAP_SIZE; z++) {
                for (var x = 0; x < MAP_SIZE; x++) {
                    triPoints[x + z * MAP_SIZE] = new Vector3(x, points[x, z], z);
                }
            }

            var indices = new int[(MAP_SIZE-1) * (MAP_SIZE-1) * 6];
            var count = 0;
            for (var y = 0; y < MAP_SIZE-1; y++) {
                for (var x = 0; x < MAP_SIZE-1; x++) {
                    indices[count++] = x + y * MAP_SIZE;
                    indices[count++] = x + (y + 1) * MAP_SIZE;
                    indices[count++] = x + 1 + y * MAP_SIZE;

                    indices[count++] = x + (y + 1) * MAP_SIZE;
                    indices[count++] = x + 1 + (y + 1) * MAP_SIZE;
                    indices[count++] = x + 1 + y * MAP_SIZE;
                }
            }


            var geo = new GeometryComponent();
            geo.Executor.Geometry.AddBuffer(Buffer.Create<Vector3>(Renderer.Instance.Device, BindFlags.VertexBuffer, triPoints), "POSITION", Format.R32G32B32_Float, Utilities.SizeOf<Vector3>());
            geo.Executor.Geometry.SetIndexBuffer(Buffer.Create<int>(Renderer.Instance.Device, BindFlags.IndexBuffer, indices), indices.Length);
            geo.Executor.Geometry.Topology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            this.AddComponent(geo);
        }
    }
}
