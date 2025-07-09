namespace FileDownloaderWinForms
{
    partial class Form1
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

        private void InitializeComponent()
        {
            txtSavePath = new TextBox();
            progressBar1 = new ProgressBar();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // txtSavePath
            // 
            txtSavePath.BackColor = Color.White;
            txtSavePath.Location = new Point(488, 219);
            txtSavePath.Name = "txtSavePath";
            txtSavePath.Size = new Size(300, 23);
            txtSavePath.TabIndex = 0;
            txtSavePath.TextChanged += textBox1_TextChanged;
            // 
            // progressBar1
            // 
            progressBar1.ForeColor = Color.FromArgb(192, 0, 192);
            progressBar1.Location = new Point(382, 296);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(509, 32);
            progressBar1.TabIndex = 1;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            lblStatus.BackColor = Color.Gray;
            lblStatus.Location = new Point(511, 401);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(255, 39);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Перетащите ссылку на файл сюда";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            lblStatus.Click += lblStatus_Click;
            // 
            // Form1
            // 
            AllowDrop = true;
            BackColor = Color.MediumTurquoise;
            ClientSize = new Size(1269, 669);
            Controls.Add(lblStatus);
            Controls.Add(progressBar1);
            Controls.Add(txtSavePath);
            ForeColor = Color.Black;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ProgressBar progressBar1;
        private Label lblStatus;
        public TextBox txtSavePath;
    }
}
       
    