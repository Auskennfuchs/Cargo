﻿using System;
using System.Collections.Generic;
using CargoEngine.Parameter;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace CargoEngine
{
    public class ConstantBuffer : IDisposable
    {
        public Buffer Buffer {
            get;
            private set;
        }

        private byte[] cpuBuffer;
        private int bufferSize;
        private Dictionary<string, ConstantBufferParameter> members = new Dictionary<string, ConstantBufferParameter>();

        public List<string> Members {
            get; private set;
        }

        public int BindPoint {
            get; private set;
        }

        private DataStream dataStream;

        public ConstantBuffer(Buffer buf, int bindpoint) {
            Buffer = buf;
            BindPoint = bindpoint;
            if (buf != null) {
                bufferSize = buf.Description.SizeInBytes;
                cpuBuffer = new byte[bufferSize];
                dataStream = new DataStream(bufferSize, true, true);
            }
            Members = new List<string>();
        }

        ~ConstantBuffer() {
            Dispose();
        }

        public void Dispose() {
            if (Buffer != null) {
                Buffer.Dispose();
            }
            if (dataStream != null) {
                dataStream.Dispose();
            }
        }

        public void AddParameter(string name, ConstantBufferParameter param) {
            members.Add(name, param);
            Members.Add(name);
        }

        public void SetParameterMatrix(string name, Matrix m) {
            SetParameterValue(name, m);
        }

        private void SetParameterValue(string name, object obj) {
            if (members.ContainsKey(name)) {
                members[name].Value = obj;
            }
        }

        public void UpdateBuffer(DeviceContext context, ParameterManager paramManager) {
            UpdateCpuBuffer(paramManager);
            DataStream ds;
            context.MapSubresource(Buffer, 0, MapMode.WriteDiscard, MapFlags.None, out ds);
            ds.Write(cpuBuffer, 0, bufferSize);
            context.UnmapSubresource(Buffer, 0);
        }

        private void UpdateCpuBuffer(ParameterManager paramManager) {
            foreach (string key in members.Keys) {
                ConstantBufferParameter param = members[key];
                var managerParam = paramManager.GetParameter(key);
                if (managerParam == null) {
                    continue;
                }
                param.Value = paramManager.GetParameter(key).Value;
                param.UpdateBuffer();

                System.Buffer.BlockCopy(param.Bytebuffer, 0, cpuBuffer, param.Offset, param.Size);
            }
        }
    }
}
