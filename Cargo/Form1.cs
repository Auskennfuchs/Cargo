using System.Windows.Forms;
using CargoEngine;
using CargoEngine.Shader;

namespace Cargo {
    public partial class Form1 : Form {

        private Renderer renderer;
        private SwapChain swapChain;
        private RenderPipeline deferredPipeline;

        private SimpleRenderTask renderTask;

        public Form1() {
            InitializeComponent();
            renderer = new Renderer();
            swapChain = new SwapChain(this, renderer);

            deferredPipeline = new RenderPipeline(renderer.Device);

            renderTask = new SimpleRenderTask(swapChain.RenderTarget);
        }

        public void MainLoop() {
            renderTask.Render(deferredPipeline);
            deferredPipeline.FinishCommandList();
            renderer.ImmPipeline.ExecuteCommandList(deferredPipeline.CommandList);
            swapChain.Present();
            deferredPipeline.ReleaseCommandList();
        }
    }
}
