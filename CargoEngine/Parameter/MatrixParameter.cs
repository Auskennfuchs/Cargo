using System;
using SharpDX;

namespace CargoEngine.Parameter {
    class MatrixParameter : ConstantBufferParameter {
        public MatrixParameter(int offset = 0) : base(RenderParameterType.Matrix, sizeof(float) * 4 * 4, offset) {
        }

        public MatrixParameter(Matrix mat, int offset = 0) : base(RenderParameterType.Matrix, sizeof(float) * 4 * 4, offset, mat) {
        }

        protected override Array GetValArray() {
            return ((Matrix)Value).ToArray();
        }
    }
}
