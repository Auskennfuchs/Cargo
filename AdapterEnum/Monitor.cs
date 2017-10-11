using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdapterEnum {
    class Monitor : FlowLayoutPanel {

        List<PictureBox> monitors = new List<PictureBox>();

        public Monitor() {
        }

        public void AddMonitor() {
            var size = Math.Min(this.ClientSize.Width / (monitors.Count + 1), this.ClientSize.Height);
            var mon = new PictureBox {
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(size, size),
                Image = Resources.monitor as Image,
                SizeMode = PictureBoxSizeMode.CenterImage,
                Margin = Padding.Empty,
                Anchor = AnchorStyles.Left|AnchorStyles.Right
            };
            mon.Parent = this;
            this.Controls.Add(mon);
            monitors.ForEach(monitor => {
                monitor.Width = size;
                monitor.Height = size;
            });
            monitors.Add(mon);
        }
    }
}
