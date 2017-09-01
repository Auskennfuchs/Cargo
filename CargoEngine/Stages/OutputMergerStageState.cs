﻿using SharpDX.Direct3D11;

namespace CargoEngine.Stages {
    public class OutputMergerStageState : IStageState {
        public static int NUM_RENDERTARGETS = 8;

        public TStateMonitor<BlendState> BlendState {
            get; private set;
        }

        public TStateArrayMonitor<RenderTargetView> RenderTarget {
            get; private set;
        }

        public TStateMonitor<DepthStencilView> DepthStencilView {
            get; private set;
        }

        public OutputMergerStageState() {
            BlendState = new TStateMonitor<SharpDX.Direct3D11.BlendState>(null);
            RenderTarget = new TStateArrayMonitor<RenderTargetView>(NUM_RENDERTARGETS, null);
            DepthStencilView = new TStateMonitor<SharpDX.Direct3D11.DepthStencilView>(null);
        }

        public void ClearState() {
            BlendState.InitializeState();
            RenderTarget.InitializeState();
            DepthStencilView.InitializeState();
        }

        public void Clone(IStageState src) {
            BlendState.State = ((OutputMergerStageState)src).BlendState.State;
            for (var i = 0; i < NUM_RENDERTARGETS; i++) {
                RenderTarget.States[i] = ((OutputMergerStageState)src).RenderTarget.States[i];
            }
            DepthStencilView.State = ((OutputMergerStageState)src).DepthStencilView.State;
        }

        public void ResetTracking() {
            BlendState.ResetTracking();
            RenderTarget.ResetTracking();
            DepthStencilView.ResetTracking();
        }

        public int GetRenderTargetCount() {
            int count = 0;
            for(int i=0;i<NUM_RENDERTARGETS;i++) {
                if(RenderTarget.States[i]!=null) {
                    count=i+1;
                }
            }
            return count;
        }
    }
}