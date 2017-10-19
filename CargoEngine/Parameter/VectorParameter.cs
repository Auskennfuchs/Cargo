using System;
using SharpDX;

namespace CargoEngine.Parameter
{
    class Vector2Parameter : ConstantBufferParameter
    {
        public Vector2Parameter(int offset = 0) : base(RenderParameterType.Vector2, sizeof(float) * 2, offset) {
        }

        public Vector2Parameter(Vector2 vec, int offset = 0) : base(RenderParameterType.Vector2, sizeof(float) * 2, offset, vec) {
        }

        protected override Array GetValArray() {
            return ((Vector2)Value).ToArray();
        }
    }

    class Vector3Parameter : ConstantBufferParameter
    {
        public Vector3Parameter(int offset = 0) : base(RenderParameterType.Vector3, sizeof(float) * 3, offset) {
        }

        public Vector3Parameter(Vector3 vec, int offset = 0) : base(RenderParameterType.Vector3, sizeof(float) * 3, offset, vec) {
        }

        protected override Array GetValArray() {
            return ((Vector3)Value).ToArray();
        }
    }

    class Vector4Parameter : ConstantBufferParameter
    {
        public Vector4Parameter(int offset = 0) : base(RenderParameterType.Vector3, sizeof(float) * 4, offset) {
        }

        public Vector4Parameter(Vector4 vec, int offset = 0) : base(RenderParameterType.Vector4, sizeof(float) * 4, offset, vec) {
        }

        protected override Array GetValArray() {
            return ((Vector4)Value).ToArray();
        }
    }
}
