using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DXGI;

namespace AdapterEnum {
    class Program {
        static void Main(string[] args) {
            var factory = new Factory1();
            for(var i =0; i<factory.GetAdapterCount();i++) {
                var adapter = factory.GetAdapter(i);
                Console.WriteLine(adapter.Description.Description.Trim('\0')+":");
                for (var j = 0; j< adapter.Outputs.Length; j++) {
                    var output = adapter.Outputs[j];
                    Console.WriteLine(output.Description.DeviceName);
                    Console.WriteLine("Desktop: " + output.Description.IsAttachedToDesktop);
                    Console.WriteLine("Bounds: " + output.Description.DesktopBounds.Right);
                }
                Console.WriteLine();
            }

            var wnd = new AdapterConfig();
            wnd.ShowDialog();
        }
    }
}
