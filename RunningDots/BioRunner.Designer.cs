namespace RunningDots
{
    partial class BioRunner
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            simRunnerWorker = new System.ComponentModel.BackgroundWorker();
            SuspendLayout();
            // 
            // simRunnerWorker
            // 
            simRunnerWorker.DoWork += SimRunnerWorker_DoWork;
            simRunnerWorker.ProgressChanged += SimRunnerWorker_ProgressChanged;
            // 
            // BioRunner
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1400, 1000);
            Name = "BioRunner";
            Text = "BioRunner";
            Load += BioRunner_Load;
            ResumeLayout(false);
        }

        private void SimRunnerWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BioRunner_Load(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void SimRunnerWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private System.ComponentModel.BackgroundWorker simRunnerWorker;
    }
}
