#nullable disable

namespace GExtract
{
    partial class MainForm
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
            components = new System.ComponentModel.Container();
            tableLayoutPanel1 = new TableLayoutPanel();
            introLabel = new Label();
            romLabel = new Label();
            romPathTextBox = new TextBox();
            browseRomButton = new Button();
            detailsLabel = new Label();
            outputLabel = new Label();
            outputTextBox = new TextBox();
            browseOutputButton = new Button();
            modeGroupBox = new GroupBox();
            chrBanksRadioButton = new RadioButton();
            allChrRadioButton = new RadioButton();
            entireFileRadioButton = new RadioButton();
            extractButton = new Button();
            progressBar = new ProgressBar();
            statusLabel = new Label();
            creditLabel = new Label();
            tableLayoutPanel1.SuspendLayout();
            modeGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            tableLayoutPanel1.Controls.Add(introLabel, 0, 0);
            tableLayoutPanel1.Controls.Add(romLabel, 0, 1);
            tableLayoutPanel1.Controls.Add(romPathTextBox, 1, 1);
            tableLayoutPanel1.Controls.Add(browseRomButton, 2, 1);
            tableLayoutPanel1.Controls.Add(detailsLabel, 0, 2);
            tableLayoutPanel1.Controls.Add(outputLabel, 0, 3);
            tableLayoutPanel1.Controls.Add(outputTextBox, 1, 3);
            tableLayoutPanel1.Controls.Add(browseOutputButton, 2, 3);
            tableLayoutPanel1.Controls.Add(modeGroupBox, 0, 4);
            tableLayoutPanel1.Controls.Add(extractButton, 0, 5);
            tableLayoutPanel1.Controls.Add(progressBar, 0, 6);
            tableLayoutPanel1.Controls.Add(statusLabel, 0, 7);
            tableLayoutPanel1.Controls.Add(creditLabel, 0, 8);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.Padding = new Padding(16);
            tableLayoutPanel1.RowCount = 9;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 68F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 132F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.Size = new Size(720, 490);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // introLabel
            // 
            introLabel.Anchor = AnchorStyles.Left;
            introLabel.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(introLabel, 3);
            introLabel.Location = new Point(19, 16);
            introLabel.Margin = new Padding(3, 0, 3, 10);
            introLabel.Name = "introLabel";
            introLabel.Size = new Size(394, 15);
            introLabel.TabIndex = 0;
            introLabel.Text = "Read 8x8 graphics tiles from a NES ROM and save them as bitmaps.";
            // 
            // romLabel
            // 
            romLabel.Anchor = AnchorStyles.Left;
            romLabel.AutoSize = true;
            romLabel.Location = new Point(19, 47);
            romLabel.Margin = new Padding(3, 8, 3, 3);
            romLabel.Name = "romLabel";
            romLabel.Size = new Size(55, 15);
            romLabel.TabIndex = 1;
            romLabel.Text = "NES ROM";
            // 
            // romPathTextBox
            // 
            romPathTextBox.AccessibleName = "NES ROM file";
            romPathTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            romPathTextBox.Location = new Point(129, 43);
            romPathTextBox.Margin = new Padding(3, 6, 3, 3);
            romPathTextBox.Name = "romPathTextBox";
            romPathTextBox.PlaceholderText = "Select a .nes file";
            romPathTextBox.Size = new Size(472, 23);
            romPathTextBox.TabIndex = 2;
            romPathTextBox.TextChanged += romPathTextBox_TextChanged;
            romPathTextBox.Leave += romPathTextBox_Leave;
            // 
            // browseRomButton
            // 
            browseRomButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            browseRomButton.Location = new Point(607, 42);
            browseRomButton.Margin = new Padding(3, 4, 3, 3);
            browseRomButton.Name = "browseRomButton";
            browseRomButton.Size = new Size(94, 25);
            browseRomButton.TabIndex = 3;
            browseRomButton.Text = "Browse...";
            browseRomButton.UseVisualStyleBackColor = true;
            browseRomButton.Click += browseRomButton_Click;
            // 
            // detailsLabel
            // 
            detailsLabel.Dock = DockStyle.Fill;
            tableLayoutPanel1.SetColumnSpan(detailsLabel, 3);
            detailsLabel.Location = new Point(19, 71);
            detailsLabel.Margin = new Padding(3, 4, 3, 8);
            detailsLabel.Name = "detailsLabel";
            detailsLabel.Size = new Size(682, 56);
            detailsLabel.TabIndex = 4;
            detailsLabel.Text = "No ROM loaded.";
            // 
            // outputLabel
            // 
            outputLabel.Anchor = AnchorStyles.Left;
            outputLabel.AutoSize = true;
            outputLabel.Location = new Point(19, 143);
            outputLabel.Margin = new Padding(3, 8, 3, 3);
            outputLabel.Name = "outputLabel";
            outputLabel.Size = new Size(84, 15);
            outputLabel.TabIndex = 5;
            outputLabel.Text = "Output folder";
            // 
            // outputTextBox
            // 
            outputTextBox.AccessibleName = "Output folder";
            outputTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            outputTextBox.Location = new Point(129, 139);
            outputTextBox.Margin = new Padding(3, 6, 3, 3);
            outputTextBox.Name = "outputTextBox";
            outputTextBox.PlaceholderText = "Select a folder";
            outputTextBox.Size = new Size(472, 23);
            outputTextBox.TabIndex = 6;
            outputTextBox.TextChanged += outputTextBox_TextChanged;
            // 
            // browseOutputButton
            // 
            browseOutputButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            browseOutputButton.Location = new Point(607, 138);
            browseOutputButton.Margin = new Padding(3, 4, 3, 3);
            browseOutputButton.Name = "browseOutputButton";
            browseOutputButton.Size = new Size(94, 25);
            browseOutputButton.TabIndex = 7;
            browseOutputButton.Text = "Browse...";
            browseOutputButton.UseVisualStyleBackColor = true;
            browseOutputButton.Click += browseOutputButton_Click;
            // 
            // modeGroupBox
            // 
            modeGroupBox.Controls.Add(chrBanksRadioButton);
            modeGroupBox.Controls.Add(allChrRadioButton);
            modeGroupBox.Controls.Add(entireFileRadioButton);
            tableLayoutPanel1.SetColumnSpan(modeGroupBox, 3);
            modeGroupBox.Dock = DockStyle.Fill;
            modeGroupBox.Location = new Point(19, 168);
            modeGroupBox.Margin = new Padding(3, 4, 3, 8);
            modeGroupBox.Name = "modeGroupBox";
            modeGroupBox.Padding = new Padding(12);
            modeGroupBox.Size = new Size(682, 120);
            modeGroupBox.TabIndex = 8;
            modeGroupBox.TabStop = false;
            modeGroupBox.Text = "Extract";
            // 
            // chrBanksRadioButton
            // 
            chrBanksRadioButton.AutoSize = true;
            chrBanksRadioButton.Enabled = false;
            chrBanksRadioButton.Location = new Point(16, 28);
            chrBanksRadioButton.Name = "chrBanksRadioButton";
            chrBanksRadioButton.Size = new Size(85, 19);
            chrBanksRadioButton.TabIndex = 0;
            chrBanksRadioButton.Text = "CHR banks";
            chrBanksRadioButton.UseVisualStyleBackColor = true;
            chrBanksRadioButton.CheckedChanged += extractionMode_CheckedChanged;
            // 
            // allChrRadioButton
            // 
            allChrRadioButton.AutoSize = true;
            allChrRadioButton.Enabled = false;
            allChrRadioButton.Location = new Point(16, 54);
            allChrRadioButton.Name = "allChrRadioButton";
            allChrRadioButton.Size = new Size(114, 19);
            allChrRadioButton.TabIndex = 1;
            allChrRadioButton.Text = "All CHR graphics";
            allChrRadioButton.UseVisualStyleBackColor = true;
            allChrRadioButton.CheckedChanged += extractionMode_CheckedChanged;
            // 
            // entireFileRadioButton
            // 
            entireFileRadioButton.AutoSize = true;
            entireFileRadioButton.Enabled = false;
            entireFileRadioButton.Location = new Point(16, 80);
            entireFileRadioButton.Name = "entireFileRadioButton";
            entireFileRadioButton.Size = new Size(75, 19);
            entireFileRadioButton.TabIndex = 2;
            entireFileRadioButton.Text = "Entire file";
            entireFileRadioButton.UseVisualStyleBackColor = true;
            entireFileRadioButton.CheckedChanged += extractionMode_CheckedChanged;
            // 
            // extractButton
            // 
            extractButton.Anchor = AnchorStyles.Left;
            extractButton.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(extractButton, 3);
            extractButton.Enabled = false;
            extractButton.Location = new Point(19, 300);
            extractButton.Margin = new Padding(3, 4, 3, 3);
            extractButton.Name = "extractButton";
            extractButton.Padding = new Padding(16, 6, 16, 6);
            extractButton.Size = new Size(88, 35);
            extractButton.TabIndex = 9;
            extractButton.Text = "&Extract";
            extractButton.UseVisualStyleBackColor = true;
            extractButton.Click += extractButton_Click;
            // 
            // progressBar
            // 
            tableLayoutPanel1.SetColumnSpan(progressBar, 3);
            progressBar.Dock = DockStyle.Fill;
            progressBar.Location = new Point(19, 341);
            progressBar.Margin = new Padding(3, 12, 3, 6);
            progressBar.Maximum = 1;
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(682, 22);
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.TabIndex = 10;
            // 
            // statusLabel
            // 
            statusLabel.AutoEllipsis = true;
            tableLayoutPanel1.SetColumnSpan(statusLabel, 3);
            statusLabel.Dock = DockStyle.Fill;
            statusLabel.Location = new Point(19, 372);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(682, 80);
            statusLabel.TabIndex = 11;
            statusLabel.Text = "Open a NES ROM.";
            statusLabel.TextAlign = ContentAlignment.TopLeft;
            // 
            // creditLabel
            // 
            creditLabel.Anchor = AnchorStyles.Left;
            creditLabel.AutoSize = true;
            tableLayoutPanel1.SetColumnSpan(creditLabel, 3);
            creditLabel.ForeColor = SystemColors.GrayText;
            creditLabel.Location = new Point(19, 460);
            creditLabel.Margin = new Padding(3, 8, 3, 0);
            creditLabel.Name = "creditLabel";
            creditLabel.Size = new Size(424, 15);
            creditLabel.TabIndex = 12;
            creditLabel.Text = "Tile decoding follows Graphics Extractor v1.1 by Griffin Knodle, 1998.";
            // 
            // MainForm
            // 
            AcceptButton = extractButton;
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(720, 490);
            Controls.Add(tableLayoutPanel1);
            MinimumSize = new Size(680, 560);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "GExtract";
            DragDrop += MainForm_DragDrop;
            DragEnter += MainForm_DragEnter;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            modeGroupBox.ResumeLayout(false);
            modeGroupBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Label introLabel;
        private Label romLabel;
        private TextBox romPathTextBox;
        private Button browseRomButton;
        private Label detailsLabel;
        private Label outputLabel;
        private TextBox outputTextBox;
        private Button browseOutputButton;
        private GroupBox modeGroupBox;
        private RadioButton chrBanksRadioButton;
        private RadioButton allChrRadioButton;
        private RadioButton entireFileRadioButton;
        private Button extractButton;
        private ProgressBar progressBar;
        private Label statusLabel;
        private Label creditLabel;
    }
}
