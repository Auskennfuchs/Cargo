using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CargoEngine;

namespace Cargo {
    class SimpleRenderTask : RenderTask {

        private RenderTarget renderTarget;

        public SimpleRenderTask(RenderTarget rt) {
            renderTarget = rt;
        }

        public override void Render(RenderPipeline pipeline) {
            pipeline.OutputMergerStage.DesiredState.RenderTarget.SetState(0, renderTarget.View);
            pipeline.OutputMergerStage.ApplyRenderTargets(pipeline.DevContext);
            pipeline.ClearBuffer(new SharpDX.Color4(1.0f, 0.0f, 0.0f, 1.0f), 1.0f, 0);
        }
    }
}
