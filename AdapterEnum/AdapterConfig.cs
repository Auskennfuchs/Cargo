using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdapterEnum {
    public partial class AdapterConfig : Form {
        public AdapterConfig() {
            InitializeComponent();
        }

        private void OnShow(object sender, EventArgs e) {
            monitors.AddMonitor();
            monitors.AddMonitor();
            monitors.AddMonitor();
            monitors.AddMonitor();
        }
    }
}
