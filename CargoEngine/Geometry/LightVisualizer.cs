using CargoEngine.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CargoEngine;
using SharpDX;

namespace CargoEngine.Geometry
{
    public class LightVisualizer : EntityComponent
    {
        private Mesh mesh;

        private CargoEngine.Shader.VertexShader vs;
        private CargoEngine.Shader.PixelShader ps;

        private Matrix mat;

        public LightVisualizer(Camera cam) {
            mat = Matrix.Multiply(cam.ViewMatrix,cam.ProjectionMatrix);
            mat.Invert();
            mesh = new Mesh();
            var verts = new Vector3[] {
                new Vector3(-1.0f,-1.0f, 0.0f),
                new Vector3( 1.0f,-1.0f, 0.0f),

                new Vector3(-1.0f, 1.0f, 0.0f),
                new Vector3( 1.0f, 1.0f, 0.0f),

                new Vector3(-1.0f,-1.0f, 0.0f),
                new Vector3(-1.0f, 1.0f, 0.0f),

                new Vector3( 1.0f,-1.0f, 0.0f),
                new Vector3( 1.0f, 1.0f, 0.0f),

                new Vector3(-1.0f,-1.0f, 1.0f),
                new Vector3( 1.0f,-1.0f, 1.0f),

                new Vector3(-1.0f,-1.0f, 1.0f),
                new Vector3(-1.0f, 1.0f, 1.0f),

                new Vector3(-1.0f, 1.0f, 1.0f),
                new Vector3( 1.0f, 1.0f, 1.0f),

                new Vector3( 1.0f,-1.0f, 1.0f),
                new Vector3( 1.0f, 1.0f, 1.0f),

                new Vector3(-1.0f,-1.0f, 0.0f),
                new Vector3(-1.0f,-1.0f, 1.0f),

                new Vector3( 1.0f,-1.0f, 0.0f),
                new Vector3( 1.0f,-1.0f, 1.0f),

                new Vector3(-1.0f, 1.0f, 0.0f),
                new Vector3(-1.0f, 1.0f, 1.0f),

                new Vector3( 1.0f, 1.0f, 0.0f),
                new Vector3( 1.0f, 1.0f, 1.0f),
            };
            mesh.Vertices = verts;
            mesh.Normals = new Vector3[24];
            mesh.UVs = new Vector2[24];
            mesh.Topology = Topology.LineList;

            vs = Renderer.ShaderLoader.LoadVertexShader("assets/shader/simple.hlsl", "VSMain");
            ps = Renderer.ShaderLoader.LoadPixelShader("assets/shader/simple.hlsl", "PSMain");
        }

        public override void Dispose() {
            mesh.Dispose();
        }

        public override void Render(RenderPipeline pipeline) {
            var cam = (Camera)Parent;
            mat = Matrix.Multiply(cam.ViewMatrix, cam.ProjectionMatrix);
            mat.Invert();
            pipeline.ParameterManager.SetWorldMatrix(mat);
//            pipeline.VertexShader.Shader = vs;
//            pipeline.PixelShader.Shader = ps;
            mesh.Apply(pipeline);
            pipeline.ApplyShaderResources();
            pipeline.Draw(mesh.VertexCount, 0);
        }
    }
}
