using System.Windows.Forms;
using CargoEngine;
using CargoEngine.Shader;

namespace Cargo {
    public partial class Form1 : Form {

        private SwapChain swapChain;

        public Form1() {
            InitializeComponent();
            Renderer r1 = new Renderer();
            swapChain = new SwapChain(this, r1);
            var vertexShader = new VertexShader(r1, "assets/shader/shaders.hlsl","VSMain");
            var pixelhader = new PixelShader(r1, "assets/shader/shaders.hlsl", "PSMain");
        }

        public void MainLoop() {
            swapChain.Present();
        }
    }
}
