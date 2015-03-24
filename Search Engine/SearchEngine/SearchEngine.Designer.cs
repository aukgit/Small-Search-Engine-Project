namespace SearchEngine {
    partial class SearchEngine {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchEngine));
            this.SearchEngineInterfaceFlash = new AxShockwaveFlashObjects.AxShockwaveFlash();
            ((System.ComponentModel.ISupportInitialize)(this.SearchEngineInterfaceFlash)).BeginInit();
            this.SuspendLayout();
            // 
            // SearchEngineInterfaceFlash
            // 
            this.SearchEngineInterfaceFlash.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchEngineInterfaceFlash.Enabled = true;
            this.SearchEngineInterfaceFlash.Location = new System.Drawing.Point(0, 0);
            this.SearchEngineInterfaceFlash.Name = "SearchEngineInterfaceFlash";
            this.SearchEngineInterfaceFlash.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("SearchEngineInterfaceFlash.OcxState")));
            this.SearchEngineInterfaceFlash.Size = new System.Drawing.Size(608, 565);
            this.SearchEngineInterfaceFlash.TabIndex = 0;
            this.SearchEngineInterfaceFlash.FSCommand += new AxShockwaveFlashObjects._IShockwaveFlashEvents_FSCommandEventHandler(this.SearchEngineInterfaceFlash_FSCommand);
            this.SearchEngineInterfaceFlash.Enter += new System.EventHandler(this.SearchEngineInterfaceFlash_Enter);
            // 
            // SearchEngine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 565);
            this.Controls.Add(this.SearchEngineInterfaceFlash);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchEngine";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search Engine";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.SearchEngine_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SearchEngineInterfaceFlash)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public AxShockwaveFlashObjects.AxShockwaveFlash SearchEngineInterfaceFlash;




    }
}

