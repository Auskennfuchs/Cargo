using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.Generic;

namespace CargoEngine
{
    internal class InputElementList
    {
        public List<InputElement> InputElements {
            get; private set;
        } = new List<InputElement>();

        public int HashCode {
            get; private set;
        }

        public void AddElement(string inputName,Format format,int slot) {
            int index = 0;
            InputElements.ForEach(element => {
                if (element.SemanticName.Equals(inputName)) {
                    index++;
                }
            });
            var el = new InputElement(inputName, index, format, slot);
            InputElements.Add(el);
            HashCode ^= el.GetHashCode();
        }

        public void Clear() {
            InputElements.Clear();
            HashCode = 0;
        }

        public override bool Equals(object obj) {
            if(obj==null) {
                return false;
            }
            var compObj = obj as InputElementList;
            return HashCode == compObj.HashCode;
        }

        public override int GetHashCode() {
            return HashCode;
        }
    }
}
