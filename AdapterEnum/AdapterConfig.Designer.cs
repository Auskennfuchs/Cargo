namespace AdapterEnum {
    partial class AdapterConfig {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.monitors = new AdapterEnum.Monitor();
            this.SuspendLayout();
            //
            // monitors
            //
            this.monitors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.monitors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.monitors.Location = new System.Drawing.Point(12, 12);
            this.monitors.Name = "monitors";
            this.monitors.Size = new System.Drawing.Size(608, 162);
            this.monitors.TabIndex = 1;
            //
            // AdapterConfig
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 414);
            this.Controls.Add(this.monitors);
            this.Name = "AdapterConfig";
            this.Text = "AdapterConfig";
            this.Shown += new System.EventHandler(this.OnShow);
            this.ResumeLayout(false);

        }

        #endregion

        private Monitor monitors;
    }
}