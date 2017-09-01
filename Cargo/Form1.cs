using System.Windows.Forms;
using CargoEngine;
using CargoEngine.Shader;

namespace Cargo {
    public partial class Form1 : Form {

        private Renderer renderer;
        private SwapChain swapChain;

        private SimpleRenderTask renderTask;

        public Form1() {
            InitializeComponent();
            renderer = new Renderer();
            swapChain = new SwapChain(this, renderer);

            renderTask = new SimpleRenderTask(swapChain.RenderTarget);

            var vertexShader = new VertexShader(renderer, "assets/shader/shaders.hlsl","VSMain");
            var pixelhader = new PixelShader(renderer, "assets/shader/shaders.hlsl", "PSMain");
        }

        public void MainLoop() {
            renderTask.Render(renderer.ImmPipeline);
            swapChain.Present();
        }
    }
}
