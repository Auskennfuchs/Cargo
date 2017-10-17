using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoEngine.Parameter
{
    class SamplerStateParameter : RenderParameter
    {
        public SamplerStateParameter(object value = null) : base(RenderParameterType.SamplerState, value) {
        }
    }
}
