namespace TestFixtureProject
{
    partial class frmTestFixtureMetroUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mPanel = new MetroFramework.Controls.MetroPanel();
            this.SuspendLayout();
            // 
            // mPanel
            // 
            this.mPanel.AutoSize = true;
            this.mPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mPanel.HorizontalScrollbarBarColor = true;
            this.mPanel.HorizontalScrollbarHighlightOnWheel = false;
            this.mPanel.HorizontalScrollbarSize = 10;
            this.mPanel.Location = new System.Drawing.Point(20, 60);
            this.mPanel.Name = "mPanel";
            this.mPanel.Size = new System.Drawing.Size(750, 720);
            this.mPanel.TabIndex = 0;
            this.mPanel.VerticalScrollbarBarColor = true;
            this.mPanel.VerticalScrollbarHighlightOnWheel = false;
            this.mPanel.VerticalScrollbarSize = 10;
            // 
            // frmTestFixtureMetroUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 800);
            this.Controls.Add(this.mPanel);
            this.Name = "frmTestFixtureMetroUI";
            this.Text = " TEST FIXTURE CLIENT";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTestFixtureMetroUI_FormClosing);
            this.Load += new System.EventHandler(this.frmTestFixtureMetroUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroPanel mPanel;
    }
}