﻿using CargoEngine.Parameter;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace CargoEngine.Stages {
    public class InputAssemblerStage : Stage<InputAssemblerStageState> {

        public InputLayout InputLayout {
            get {
                return DesiredState.InputLayout.State;
            }
            set {
                DesiredState.InputLayout.State = value;
            }
        }

        public PrimitiveTopology PrimitiveTopology {
            get {
                return DesiredState.PrimitiveTopology.State;
            }
            set {
                DesiredState.PrimitiveTopology.State = value;
            }
        }

        public TStateArrayMonitorStruct<VertexBufferBinding> VertexBuffer {
            get {
                return DesiredState.VertexBuffers;
            }
        }

        public Buffer IndexBuffer {
            get {
                return DesiredState.IndexBuffer.State;
            }
            set {
                DesiredState.IndexBuffer.State = value;
            }
        }

        public Format IndexBufferFormat {
            get {
                return DesiredState.IndexBufferFormat.State;
            }
            set {
                DesiredState.IndexBufferFormat.State = value;
            }
        }

        public override void OnApplyDesiredState(DeviceContext dc, ParameterManager paramManager) {
            if (DesiredState.InputLayout.NeedUpdate) {
                dc.InputAssembler.InputLayout = DesiredState.InputLayout.State;
            }
            if (DesiredState.PrimitiveTopology.NeedUpdate) {
                dc.InputAssembler.PrimitiveTopology = DesiredState.PrimitiveTopology.State;
            }
            if (DesiredState.VertexBuffers.NeedUpdate) {
                dc.InputAssembler.SetVertexBuffers(DesiredState.VertexBuffers.StartSlot, DesiredState.VertexBuffers.ChangedStates);
            }

            if(DesiredState.IndexBuffer.NeedUpdate) {
                dc.InputAssembler.SetIndexBuffer(DesiredState.IndexBuffer.State, DesiredState.IndexBufferFormat.State, 0);
            }
        }
    }
}