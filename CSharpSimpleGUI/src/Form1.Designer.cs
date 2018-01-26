namespace OmniDriverCSharpDemo
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chartSpectrum = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnScan = new System.Windows.Forms.Button();
            this.checkBoxEnableIrradiance = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.checkBoxStrobeEnable = new System.Windows.Forms.CheckBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.checkBoxSaveAll = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownSaveEverySec = new System.Windows.Forms.NumericUpDown();
            this.labelSaveCount = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.numericUpDownMaxScans = new System.Windows.Forms.NumericUpDown();
            this.checkBoxEnableDataCollection = new System.Windows.Forms.CheckBox();
            this.buttonMergeSaved = new System.Windows.Forms.Button();
            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonOptimizeIntegrationTimes = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabControlSettings = new System.Windows.Forms.TabControl();
            this.tabPageAcquisition = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.checkBoxEnableNLC = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.checkBoxEnableEDC = new System.Windows.Forms.CheckBox();
            this.numericUpDownIntegrationTimeMillisec = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownScansToAverage = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownSmoothingBoxcar = new System.Windows.Forms.NumericUpDown();
            this.tabPageGPIO = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.checkBoxGPIOAlt3 = new System.Windows.Forms.CheckBox();
            this.checkBoxGPIODir3 = new System.Windows.Forms.CheckBox();
            this.checkBoxGPIOVal3 = new System.Windows.Forms.CheckBox();
            this.checkBoxGPIOAlt2 = new System.Windows.Forms.CheckBox();
            this.checkBoxGPIODir2 = new System.Windows.Forms.CheckBox();
            this.checkBoxGPIOVal2 = new System.Windows.Forms.CheckBox();
            this.checkBoxGPIOAlt1 = new System.Windows.Forms.CheckBox();
            this.checkBoxGPIODir1 = new System.Windows.Forms.CheckBox();
            this.checkBoxGPIOVal1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxGPIOAlt0 = new System.Windows.Forms.CheckBox();
            this.checkBoxGPIODir0 = new System.Windows.Forms.CheckBox();
            this.checkBoxGPIOVal0 = new System.Windows.Forms.CheckBox();
            this.tabPageNoise = new System.Windows.Forms.TabPage();
            this.checkBoxComputeNoise = new System.Windows.Forms.CheckBox();
            this.groupBoxNoiseSettings = new System.Windows.Forms.GroupBox();
            this.labelNoiseResult = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownRMSMeasurements = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBoxEventLog = new System.Windows.Forms.TextBox();
            this.saveFileDialogSaveAll = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.folderBrowserDialogMergeFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.splitContainerAppVsEventLog = new System.Windows.Forms.SplitContainer();
            this.splitContainerGraphSettingsVsControls = new System.Windows.Forms.SplitContainer();
            this.splitContainerGraphVsSettings = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.chartSpectrum)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSaveEverySec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxScans)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tabControlSettings.SuspendLayout();
            this.tabPageAcquisition.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIntegrationTimeMillisec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScansToAverage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSmoothingBoxcar)).BeginInit();
            this.tabPageGPIO.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabPageNoise.SuspendLayout();
            this.groupBoxNoiseSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRMSMeasurements)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAppVsEventLog)).BeginInit();
            this.splitContainerAppVsEventLog.Panel1.SuspendLayout();
            this.splitContainerAppVsEventLog.Panel2.SuspendLayout();
            this.splitContainerAppVsEventLog.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerGraphSettingsVsControls)).BeginInit();
            this.splitContainerGraphSettingsVsControls.Panel1.SuspendLayout();
            this.splitContainerGraphSettingsVsControls.Panel2.SuspendLayout();
            this.splitContainerGraphSettingsVsControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerGraphVsSettings)).BeginInit();
            this.splitContainerGraphVsSettings.Panel1.SuspendLayout();
            this.splitContainerGraphVsSettings.Panel2.SuspendLayout();
            this.splitContainerGraphVsSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // chartSpectrum
            // 
            chartArea1.CursorX.IsUserEnabled = true;
            chartArea1.CursorX.IsUserSelectionEnabled = true;
            chartArea1.CursorY.IsUserEnabled = true;
            chartArea1.CursorY.IsUserSelectionEnabled = true;
            chartArea1.Name = "ChartArea1";
            this.chartSpectrum.ChartAreas.Add(chartArea1);
            this.chartSpectrum.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Alignment = System.Drawing.StringAlignment.Center;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.Name = "Legend1";
            this.chartSpectrum.Legends.Add(legend1);
            this.chartSpectrum.Location = new System.Drawing.Point(0, 0);
            this.chartSpectrum.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chartSpectrum.Name = "chartSpectrum";
            this.chartSpectrum.Size = new System.Drawing.Size(772, 435);
            this.chartSpectrum.TabIndex = 0;
            this.chartSpectrum.Text = "Spectrum";
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(8, 23);
            this.btnScan.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(100, 28);
            this.btnScan.TabIndex = 1;
            this.btnScan.Text = "Start";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // checkBoxEnableIrradiance
            // 
            this.checkBoxEnableIrradiance.AutoSize = true;
            this.checkBoxEnableIrradiance.Location = new System.Drawing.Point(8, 21);
            this.checkBoxEnableIrradiance.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxEnableIrradiance.Name = "checkBoxEnableIrradiance";
            this.checkBoxEnableIrradiance.Size = new System.Drawing.Size(132, 21);
            this.checkBoxEnableIrradiance.TabIndex = 2;
            this.checkBoxEnableIrradiance.Text = "Apply Irradiance";
            this.checkBoxEnableIrradiance.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox9);
            this.groupBox1.Controls.Add(this.groupBox8);
            this.groupBox1.Controls.Add(this.buttonReset);
            this.groupBox1.Controls.Add(this.btnScan);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(1099, 117);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Controls";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.checkBoxStrobeEnable);
            this.groupBox9.Controls.Add(this.checkBoxEnableIrradiance);
            this.groupBox9.Location = new System.Drawing.Point(136, 17);
            this.groupBox9.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox9.Size = new System.Drawing.Size(172, 87);
            this.groupBox9.TabIndex = 8;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Advanced Features";
            // 
            // checkBoxStrobeEnable
            // 
            this.checkBoxStrobeEnable.AutoSize = true;
            this.checkBoxStrobeEnable.Location = new System.Drawing.Point(8, 42);
            this.checkBoxStrobeEnable.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxStrobeEnable.Name = "checkBoxStrobeEnable";
            this.checkBoxStrobeEnable.Size = new System.Drawing.Size(120, 21);
            this.checkBoxStrobeEnable.TabIndex = 3;
            this.checkBoxStrobeEnable.Text = "Strobe Enable";
            this.checkBoxStrobeEnable.UseVisualStyleBackColor = true;
            this.checkBoxStrobeEnable.CheckedChanged += new System.EventHandler(this.checkBoxStrobeEnable_CheckedChanged);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.checkBoxSaveAll);
            this.groupBox8.Controls.Add(this.label1);
            this.groupBox8.Controls.Add(this.numericUpDownSaveEverySec);
            this.groupBox8.Controls.Add(this.labelSaveCount);
            this.groupBox8.Controls.Add(this.label10);
            this.groupBox8.Controls.Add(this.numericUpDownMaxScans);
            this.groupBox8.Controls.Add(this.checkBoxEnableDataCollection);
            this.groupBox8.Controls.Add(this.buttonMergeSaved);
            this.groupBox8.Location = new System.Drawing.Point(316, 17);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox8.Size = new System.Drawing.Size(445, 87);
            this.groupBox8.TabIndex = 7;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Data Collection";
            // 
            // checkBoxSaveAll
            // 
            this.checkBoxSaveAll.AutoSize = true;
            this.checkBoxSaveAll.Location = new System.Drawing.Point(8, 48);
            this.checkBoxSaveAll.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxSaveAll.Name = "checkBoxSaveAll";
            this.checkBoxSaveAll.Size = new System.Drawing.Size(81, 21);
            this.checkBoxSaveAll.TabIndex = 14;
            this.checkBoxSaveAll.Text = "Save All";
            this.checkBoxSaveAll.UseVisualStyleBackColor = true;
            this.checkBoxSaveAll.CheckedChanged += new System.EventHandler(this.checkBoxSaveAll_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(192, 55);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "Stop After (cnt)";
            // 
            // numericUpDownSaveEverySec
            // 
            this.numericUpDownSaveEverySec.Location = new System.Drawing.Point(95, 21);
            this.numericUpDownSaveEverySec.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numericUpDownSaveEverySec.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numericUpDownSaveEverySec.Name = "numericUpDownSaveEverySec";
            this.numericUpDownSaveEverySec.Size = new System.Drawing.Size(91, 22);
            this.numericUpDownSaveEverySec.TabIndex = 12;
            this.numericUpDownSaveEverySec.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownSaveEverySec.ValueChanged += new System.EventHandler(this.numericUpDownSaveEverySec_ValueChanged);
            // 
            // labelSaveCount
            // 
            this.labelSaveCount.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelSaveCount.Font = new System.Drawing.Font("Lucida Console", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSaveCount.ForeColor = System.Drawing.Color.Green;
            this.labelSaveCount.Location = new System.Drawing.Point(325, 55);
            this.labelSaveCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSaveCount.Name = "labelSaveCount";
            this.labelSaveCount.Size = new System.Drawing.Size(104, 22);
            this.labelSaveCount.TabIndex = 11;
            this.labelSaveCount.Text = "0";
            this.labelSaveCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(192, 23);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(116, 17);
            this.label10.TabIndex = 10;
            this.label10.Text = "Save Every (sec)";
            // 
            // numericUpDownMaxScans
            // 
            this.numericUpDownMaxScans.Location = new System.Drawing.Point(95, 53);
            this.numericUpDownMaxScans.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numericUpDownMaxScans.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.numericUpDownMaxScans.Name = "numericUpDownMaxScans";
            this.numericUpDownMaxScans.Size = new System.Drawing.Size(91, 22);
            this.numericUpDownMaxScans.TabIndex = 9;
            this.numericUpDownMaxScans.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownMaxScans.ValueChanged += new System.EventHandler(this.numericUpDownMaxScans_ValueChanged);
            // 
            // checkBoxEnableDataCollection
            // 
            this.checkBoxEnableDataCollection.AutoSize = true;
            this.checkBoxEnableDataCollection.Location = new System.Drawing.Point(8, 23);
            this.checkBoxEnableDataCollection.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxEnableDataCollection.Name = "checkBoxEnableDataCollection";
            this.checkBoxEnableDataCollection.Size = new System.Drawing.Size(74, 21);
            this.checkBoxEnableDataCollection.TabIndex = 3;
            this.checkBoxEnableDataCollection.Text = "Enable";
            this.checkBoxEnableDataCollection.UseVisualStyleBackColor = true;
            this.checkBoxEnableDataCollection.CheckedChanged += new System.EventHandler(this.checkBoxEnableDataCollection_CheckedChanged);
            // 
            // buttonMergeSaved
            // 
            this.buttonMergeSaved.Location = new System.Drawing.Point(315, 18);
            this.buttonMergeSaved.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonMergeSaved.Name = "buttonMergeSaved";
            this.buttonMergeSaved.Size = new System.Drawing.Size(115, 28);
            this.buttonMergeSaved.TabIndex = 6;
            this.buttonMergeSaved.Text = "Merge Saved";
            this.buttonMergeSaved.UseVisualStyleBackColor = true;
            this.buttonMergeSaved.Click += new System.EventHandler(this.buttonMergeSaved_Click);
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(8, 54);
            this.buttonReset.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(100, 28);
            this.buttonReset.TabIndex = 5;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // buttonOptimizeIntegrationTimes
            // 
            this.buttonOptimizeIntegrationTimes.Location = new System.Drawing.Point(8, 148);
            this.buttonOptimizeIntegrationTimes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonOptimizeIntegrationTimes.Name = "buttonOptimizeIntegrationTimes";
            this.buttonOptimizeIntegrationTimes.Size = new System.Drawing.Size(100, 32);
            this.buttonOptimizeIntegrationTimes.TabIndex = 3;
            this.buttonOptimizeIntegrationTimes.Text = "Optimize";
            this.buttonOptimizeIntegrationTimes.UseVisualStyleBackColor = true;
            this.buttonOptimizeIntegrationTimes.Click += new System.EventHandler(this.buttonOptimizeIntegrationTimes_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.tabControlSettings);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(322, 435);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // tabControlSettings
            // 
            this.tabControlSettings.Controls.Add(this.tabPageAcquisition);
            this.tabControlSettings.Controls.Add(this.tabPageGPIO);
            this.tabControlSettings.Controls.Add(this.tabPageNoise);
            this.tabControlSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlSettings.Location = new System.Drawing.Point(4, 19);
            this.tabControlSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControlSettings.Name = "tabControlSettings";
            this.tabControlSettings.SelectedIndex = 0;
            this.tabControlSettings.Size = new System.Drawing.Size(314, 412);
            this.tabControlSettings.TabIndex = 21;
            // 
            // tabPageAcquisition
            // 
            this.tabPageAcquisition.Controls.Add(this.groupBox5);
            this.tabPageAcquisition.Location = new System.Drawing.Point(4, 25);
            this.tabPageAcquisition.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageAcquisition.Name = "tabPageAcquisition";
            this.tabPageAcquisition.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageAcquisition.Size = new System.Drawing.Size(306, 383);
            this.tabPageAcquisition.TabIndex = 1;
            this.tabPageAcquisition.Text = "Acquisition";
            this.tabPageAcquisition.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBoxEnableNLC);
            this.groupBox5.Controls.Add(this.buttonOptimizeIntegrationTimes);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.checkBoxEnableEDC);
            this.groupBox5.Controls.Add(this.numericUpDownIntegrationTimeMillisec);
            this.groupBox5.Controls.Add(this.numericUpDownScansToAverage);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.numericUpDownSmoothingBoxcar);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(4, 4);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox5.Size = new System.Drawing.Size(298, 375);
            this.groupBox5.TabIndex = 19;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Acquisition";
            // 
            // checkBoxEnableNLC
            // 
            this.checkBoxEnableNLC.AutoSize = true;
            this.checkBoxEnableNLC.Checked = true;
            this.checkBoxEnableNLC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEnableNLC.Location = new System.Drawing.Point(81, 119);
            this.checkBoxEnableNLC.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxEnableNLC.Name = "checkBoxEnableNLC";
            this.checkBoxEnableNLC.Size = new System.Drawing.Size(57, 21);
            this.checkBoxEnableNLC.TabIndex = 21;
            this.checkBoxEnableNLC.Text = "NLC";
            this.checkBoxEnableNLC.UseVisualStyleBackColor = true;
            this.checkBoxEnableNLC.CheckedChanged += new System.EventHandler(this.checkBoxEnableNLC_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(105, 20);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 34);
            this.label9.TabIndex = 14;
            this.label9.Text = "Integration\r\nTime (ms)";
            // 
            // checkBoxEnableEDC
            // 
            this.checkBoxEnableEDC.AutoSize = true;
            this.checkBoxEnableEDC.Checked = true;
            this.checkBoxEnableEDC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxEnableEDC.Location = new System.Drawing.Point(9, 119);
            this.checkBoxEnableEDC.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxEnableEDC.Name = "checkBoxEnableEDC";
            this.checkBoxEnableEDC.Size = new System.Drawing.Size(58, 21);
            this.checkBoxEnableEDC.TabIndex = 20;
            this.checkBoxEnableEDC.Text = "EDC";
            this.checkBoxEnableEDC.UseVisualStyleBackColor = true;
            this.checkBoxEnableEDC.CheckedChanged += new System.EventHandler(this.checkBoxEnableEDC_CheckedChanged);
            // 
            // numericUpDownIntegrationTimeMillisec
            // 
            this.numericUpDownIntegrationTimeMillisec.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownIntegrationTimeMillisec.Location = new System.Drawing.Point(8, 23);
            this.numericUpDownIntegrationTimeMillisec.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numericUpDownIntegrationTimeMillisec.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownIntegrationTimeMillisec.Name = "numericUpDownIntegrationTimeMillisec";
            this.numericUpDownIntegrationTimeMillisec.Size = new System.Drawing.Size(93, 22);
            this.numericUpDownIntegrationTimeMillisec.TabIndex = 6;
            this.numericUpDownIntegrationTimeMillisec.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownIntegrationTimeMillisec.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownIntegrationTimeMillisec.ValueChanged += new System.EventHandler(this.numericUpDownIntegrationTimeMillisec_ValueChanged);
            // 
            // numericUpDownScansToAverage
            // 
            this.numericUpDownScansToAverage.Location = new System.Drawing.Point(8, 55);
            this.numericUpDownScansToAverage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numericUpDownScansToAverage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownScansToAverage.Name = "numericUpDownScansToAverage";
            this.numericUpDownScansToAverage.Size = new System.Drawing.Size(93, 22);
            this.numericUpDownScansToAverage.TabIndex = 18;
            this.numericUpDownScansToAverage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownScansToAverage.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownScansToAverage.ValueChanged += new System.EventHandler(this.numericUpDownScansToAverage_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(109, 52);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 34);
            this.label3.TabIndex = 19;
            this.label3.Text = "Scan\r\nAveraging";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(111, 90);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 17);
            this.label7.TabIndex = 15;
            this.label7.Text = "Boxcar";
            // 
            // numericUpDownSmoothingBoxcar
            // 
            this.numericUpDownSmoothingBoxcar.Location = new System.Drawing.Point(9, 87);
            this.numericUpDownSmoothingBoxcar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numericUpDownSmoothingBoxcar.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numericUpDownSmoothingBoxcar.Name = "numericUpDownSmoothingBoxcar";
            this.numericUpDownSmoothingBoxcar.Size = new System.Drawing.Size(93, 22);
            this.numericUpDownSmoothingBoxcar.TabIndex = 14;
            this.numericUpDownSmoothingBoxcar.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownSmoothingBoxcar.ValueChanged += new System.EventHandler(this.numericUpDownSmoothingBoxcar_ValueChanged);
            // 
            // tabPageGPIO
            // 
            this.tabPageGPIO.Controls.Add(this.groupBox4);
            this.tabPageGPIO.Location = new System.Drawing.Point(4, 25);
            this.tabPageGPIO.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageGPIO.Name = "tabPageGPIO";
            this.tabPageGPIO.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageGPIO.Size = new System.Drawing.Size(306, 383);
            this.tabPageGPIO.TabIndex = 0;
            this.tabPageGPIO.Text = "GPIO";
            this.tabPageGPIO.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.checkBoxGPIOAlt3);
            this.groupBox4.Controls.Add(this.checkBoxGPIODir3);
            this.groupBox4.Controls.Add(this.checkBoxGPIOVal3);
            this.groupBox4.Controls.Add(this.checkBoxGPIOAlt2);
            this.groupBox4.Controls.Add(this.checkBoxGPIODir2);
            this.groupBox4.Controls.Add(this.checkBoxGPIOVal2);
            this.groupBox4.Controls.Add(this.checkBoxGPIOAlt1);
            this.groupBox4.Controls.Add(this.checkBoxGPIODir1);
            this.groupBox4.Controls.Add(this.checkBoxGPIOVal1);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.checkBoxGPIOAlt0);
            this.groupBox4.Controls.Add(this.checkBoxGPIODir0);
            this.groupBox4.Controls.Add(this.checkBoxGPIOVal0);
            this.groupBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox4.Location = new System.Drawing.Point(4, 4);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox4.Size = new System.Drawing.Size(298, 375);
            this.groupBox4.TabIndex = 20;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "GPIO";
            // 
            // checkBoxGPIOAlt3
            // 
            this.checkBoxGPIOAlt3.AutoSize = true;
            this.checkBoxGPIOAlt3.Location = new System.Drawing.Point(69, 114);
            this.checkBoxGPIOAlt3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxGPIOAlt3.Name = "checkBoxGPIOAlt3";
            this.checkBoxGPIOAlt3.Size = new System.Drawing.Size(38, 21);
            this.checkBoxGPIOAlt3.TabIndex = 12;
            this.checkBoxGPIOAlt3.Text = "3";
            this.checkBoxGPIOAlt3.UseVisualStyleBackColor = true;
            this.checkBoxGPIOAlt3.CheckedChanged += new System.EventHandler(this.checkBoxGPIOAlt3_CheckedChanged);
            // 
            // checkBoxGPIODir3
            // 
            this.checkBoxGPIODir3.AutoSize = true;
            this.checkBoxGPIODir3.Location = new System.Drawing.Point(41, 116);
            this.checkBoxGPIODir3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxGPIODir3.Name = "checkBoxGPIODir3";
            this.checkBoxGPIODir3.Size = new System.Drawing.Size(18, 17);
            this.checkBoxGPIODir3.TabIndex = 11;
            this.checkBoxGPIODir3.UseVisualStyleBackColor = true;
            this.checkBoxGPIODir3.CheckedChanged += new System.EventHandler(this.checkBoxGPIODir3_CheckedChanged);
            // 
            // checkBoxGPIOVal3
            // 
            this.checkBoxGPIOVal3.AutoSize = true;
            this.checkBoxGPIOVal3.Location = new System.Drawing.Point(13, 116);
            this.checkBoxGPIOVal3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxGPIOVal3.Name = "checkBoxGPIOVal3";
            this.checkBoxGPIOVal3.Size = new System.Drawing.Size(18, 17);
            this.checkBoxGPIOVal3.TabIndex = 10;
            this.checkBoxGPIOVal3.UseVisualStyleBackColor = true;
            this.checkBoxGPIOVal3.CheckedChanged += new System.EventHandler(this.checkBoxGPIOVal3_CheckedChanged);
            // 
            // checkBoxGPIOAlt2
            // 
            this.checkBoxGPIOAlt2.AutoSize = true;
            this.checkBoxGPIOAlt2.Location = new System.Drawing.Point(69, 90);
            this.checkBoxGPIOAlt2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxGPIOAlt2.Name = "checkBoxGPIOAlt2";
            this.checkBoxGPIOAlt2.Size = new System.Drawing.Size(38, 21);
            this.checkBoxGPIOAlt2.TabIndex = 9;
            this.checkBoxGPIOAlt2.Text = "2";
            this.checkBoxGPIOAlt2.UseVisualStyleBackColor = true;
            this.checkBoxGPIOAlt2.CheckedChanged += new System.EventHandler(this.checkBoxGPIOAlt2_CheckedChanged);
            // 
            // checkBoxGPIODir2
            // 
            this.checkBoxGPIODir2.AutoSize = true;
            this.checkBoxGPIODir2.Location = new System.Drawing.Point(41, 91);
            this.checkBoxGPIODir2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxGPIODir2.Name = "checkBoxGPIODir2";
            this.checkBoxGPIODir2.Size = new System.Drawing.Size(18, 17);
            this.checkBoxGPIODir2.TabIndex = 8;
            this.checkBoxGPIODir2.UseVisualStyleBackColor = true;
            this.checkBoxGPIODir2.CheckedChanged += new System.EventHandler(this.checkBoxGPIODir2_CheckedChanged);
            // 
            // checkBoxGPIOVal2
            // 
            this.checkBoxGPIOVal2.AutoSize = true;
            this.checkBoxGPIOVal2.Location = new System.Drawing.Point(13, 91);
            this.checkBoxGPIOVal2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxGPIOVal2.Name = "checkBoxGPIOVal2";
            this.checkBoxGPIOVal2.Size = new System.Drawing.Size(18, 17);
            this.checkBoxGPIOVal2.TabIndex = 7;
            this.checkBoxGPIOVal2.UseVisualStyleBackColor = true;
            this.checkBoxGPIOVal2.CheckedChanged += new System.EventHandler(this.checkBoxGPIOVal2_CheckedChanged);
            // 
            // checkBoxGPIOAlt1
            // 
            this.checkBoxGPIOAlt1.AutoSize = true;
            this.checkBoxGPIOAlt1.Location = new System.Drawing.Point(69, 65);
            this.checkBoxGPIOAlt1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxGPIOAlt1.Name = "checkBoxGPIOAlt1";
            this.checkBoxGPIOAlt1.Size = new System.Drawing.Size(38, 21);
            this.checkBoxGPIOAlt1.TabIndex = 6;
            this.checkBoxGPIOAlt1.Text = "1";
            this.checkBoxGPIOAlt1.UseVisualStyleBackColor = true;
            this.checkBoxGPIOAlt1.CheckedChanged += new System.EventHandler(this.checkBoxGPIOAlt1_CheckedChanged);
            // 
            // checkBoxGPIODir1
            // 
            this.checkBoxGPIODir1.AutoSize = true;
            this.checkBoxGPIODir1.Location = new System.Drawing.Point(41, 66);
            this.checkBoxGPIODir1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxGPIODir1.Name = "checkBoxGPIODir1";
            this.checkBoxGPIODir1.Size = new System.Drawing.Size(18, 17);
            this.checkBoxGPIODir1.TabIndex = 5;
            this.checkBoxGPIODir1.UseVisualStyleBackColor = true;
            this.checkBoxGPIODir1.CheckedChanged += new System.EventHandler(this.checkBoxGPIODir1_CheckedChanged);
            // 
            // checkBoxGPIOVal1
            // 
            this.checkBoxGPIOVal1.AutoSize = true;
            this.checkBoxGPIOVal1.Location = new System.Drawing.Point(13, 66);
            this.checkBoxGPIOVal1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxGPIOVal1.Name = "checkBoxGPIOVal1";
            this.checkBoxGPIOVal1.Size = new System.Drawing.Size(18, 17);
            this.checkBoxGPIOVal1.TabIndex = 4;
            this.checkBoxGPIOVal1.UseVisualStyleBackColor = true;
            this.checkBoxGPIOVal1.CheckedChanged += new System.EventHandler(this.checkBoxGPIOVal1_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 21);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Val  Dir   Alt  Pin";
            // 
            // checkBoxGPIOAlt0
            // 
            this.checkBoxGPIOAlt0.AutoSize = true;
            this.checkBoxGPIOAlt0.Location = new System.Drawing.Point(69, 41);
            this.checkBoxGPIOAlt0.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxGPIOAlt0.Name = "checkBoxGPIOAlt0";
            this.checkBoxGPIOAlt0.Size = new System.Drawing.Size(38, 21);
            this.checkBoxGPIOAlt0.TabIndex = 2;
            this.checkBoxGPIOAlt0.Text = "0";
            this.checkBoxGPIOAlt0.UseVisualStyleBackColor = true;
            this.checkBoxGPIOAlt0.CheckedChanged += new System.EventHandler(this.checkBoxGPIOAlt0_CheckedChanged);
            // 
            // checkBoxGPIODir0
            // 
            this.checkBoxGPIODir0.AutoSize = true;
            this.checkBoxGPIODir0.Location = new System.Drawing.Point(41, 42);
            this.checkBoxGPIODir0.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxGPIODir0.Name = "checkBoxGPIODir0";
            this.checkBoxGPIODir0.Size = new System.Drawing.Size(18, 17);
            this.checkBoxGPIODir0.TabIndex = 1;
            this.checkBoxGPIODir0.UseVisualStyleBackColor = true;
            this.checkBoxGPIODir0.CheckedChanged += new System.EventHandler(this.checkBoxGPIODir0_CheckedChanged);
            // 
            // checkBoxGPIOVal0
            // 
            this.checkBoxGPIOVal0.AutoSize = true;
            this.checkBoxGPIOVal0.Location = new System.Drawing.Point(13, 42);
            this.checkBoxGPIOVal0.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxGPIOVal0.Name = "checkBoxGPIOVal0";
            this.checkBoxGPIOVal0.Size = new System.Drawing.Size(18, 17);
            this.checkBoxGPIOVal0.TabIndex = 0;
            this.checkBoxGPIOVal0.UseVisualStyleBackColor = true;
            this.checkBoxGPIOVal0.CheckedChanged += new System.EventHandler(this.checkBoxGPIOVal0_CheckedChanged);
            // 
            // tabPageNoise
            // 
            this.tabPageNoise.Controls.Add(this.checkBoxComputeNoise);
            this.tabPageNoise.Controls.Add(this.groupBoxNoiseSettings);
            this.tabPageNoise.Location = new System.Drawing.Point(4, 25);
            this.tabPageNoise.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageNoise.Name = "tabPageNoise";
            this.tabPageNoise.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageNoise.Size = new System.Drawing.Size(305, 381);
            this.tabPageNoise.TabIndex = 2;
            this.tabPageNoise.Text = "Noise";
            this.tabPageNoise.UseVisualStyleBackColor = true;
            // 
            // checkBoxComputeNoise
            // 
            this.checkBoxComputeNoise.AutoSize = true;
            this.checkBoxComputeNoise.Location = new System.Drawing.Point(8, 7);
            this.checkBoxComputeNoise.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxComputeNoise.Name = "checkBoxComputeNoise";
            this.checkBoxComputeNoise.Size = new System.Drawing.Size(126, 21);
            this.checkBoxComputeNoise.TabIndex = 1;
            this.checkBoxComputeNoise.Text = "Compute Noise";
            this.checkBoxComputeNoise.UseVisualStyleBackColor = true;
            this.checkBoxComputeNoise.CheckedChanged += new System.EventHandler(this.checkBoxComputeNoise_CheckedChanged);
            // 
            // groupBoxNoiseSettings
            // 
            this.groupBoxNoiseSettings.Controls.Add(this.labelNoiseResult);
            this.groupBoxNoiseSettings.Controls.Add(this.label4);
            this.groupBoxNoiseSettings.Controls.Add(this.numericUpDownRMSMeasurements);
            this.groupBoxNoiseSettings.Enabled = false;
            this.groupBoxNoiseSettings.Location = new System.Drawing.Point(8, 36);
            this.groupBoxNoiseSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxNoiseSettings.Name = "groupBoxNoiseSettings";
            this.groupBoxNoiseSettings.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBoxNoiseSettings.Size = new System.Drawing.Size(267, 79);
            this.groupBoxNoiseSettings.TabIndex = 0;
            this.groupBoxNoiseSettings.TabStop = false;
            this.groupBoxNoiseSettings.Text = "Noise Settings";
            // 
            // labelNoiseResult
            // 
            this.labelNoiseResult.AutoSize = true;
            this.labelNoiseResult.Location = new System.Drawing.Point(8, 52);
            this.labelNoiseResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelNoiseResult.Name = "labelNoiseResult";
            this.labelNoiseResult.Size = new System.Drawing.Size(42, 17);
            this.labelNoiseResult.TabIndex = 2;
            this.labelNoiseResult.Text = "RMS:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(84, 26);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(135, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "RMS Measurements";
            // 
            // numericUpDownRMSMeasurements
            // 
            this.numericUpDownRMSMeasurements.Location = new System.Drawing.Point(8, 23);
            this.numericUpDownRMSMeasurements.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numericUpDownRMSMeasurements.Name = "numericUpDownRMSMeasurements";
            this.numericUpDownRMSMeasurements.Size = new System.Drawing.Size(68, 22);
            this.numericUpDownRMSMeasurements.TabIndex = 0;
            this.numericUpDownRMSMeasurements.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownRMSMeasurements.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownRMSMeasurements.ValueChanged += new System.EventHandler(this.numericUpDownRMSMeasurements_ValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.textBoxEventLog);
            this.groupBox3.Location = new System.Drawing.Point(8, 4);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox3.Size = new System.Drawing.Size(1087, 108);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Event Log";
            // 
            // textBoxEventLog
            // 
            this.textBoxEventLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEventLog.Location = new System.Drawing.Point(8, 23);
            this.textBoxEventLog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxEventLog.Multiline = true;
            this.textBoxEventLog.Name = "textBoxEventLog";
            this.textBoxEventLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxEventLog.Size = new System.Drawing.Size(1069, 77);
            this.textBoxEventLog.TabIndex = 0;
            // 
            // saveFileDialogSaveAll
            // 
            this.saveFileDialogSaveAll.Title = "Save All Acquisitions";
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            // 
            // folderBrowserDialogMergeFolder
            // 
            this.folderBrowserDialogMergeFolder.ShowNewFolderButton = false;
            // 
            // splitContainerAppVsEventLog
            // 
            this.splitContainerAppVsEventLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerAppVsEventLog.Location = new System.Drawing.Point(16, 15);
            this.splitContainerAppVsEventLog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainerAppVsEventLog.Name = "splitContainerAppVsEventLog";
            this.splitContainerAppVsEventLog.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerAppVsEventLog.Panel1
            // 
            this.splitContainerAppVsEventLog.Panel1.Controls.Add(this.splitContainerGraphSettingsVsControls);
            // 
            // splitContainerAppVsEventLog.Panel2
            // 
            this.splitContainerAppVsEventLog.Panel2.Controls.Add(this.groupBox3);
            this.splitContainerAppVsEventLog.Size = new System.Drawing.Size(1099, 678);
            this.splitContainerAppVsEventLog.SplitterDistance = 557;
            this.splitContainerAppVsEventLog.SplitterWidth = 5;
            this.splitContainerAppVsEventLog.TabIndex = 6;
            // 
            // splitContainerGraphSettingsVsControls
            // 
            this.splitContainerGraphSettingsVsControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerGraphSettingsVsControls.Location = new System.Drawing.Point(0, 0);
            this.splitContainerGraphSettingsVsControls.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainerGraphSettingsVsControls.Name = "splitContainerGraphSettingsVsControls";
            this.splitContainerGraphSettingsVsControls.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerGraphSettingsVsControls.Panel1
            // 
            this.splitContainerGraphSettingsVsControls.Panel1.Controls.Add(this.splitContainerGraphVsSettings);
            // 
            // splitContainerGraphSettingsVsControls.Panel2
            // 
            this.splitContainerGraphSettingsVsControls.Panel2.Controls.Add(this.groupBox1);
            this.splitContainerGraphSettingsVsControls.Size = new System.Drawing.Size(1099, 557);
            this.splitContainerGraphSettingsVsControls.SplitterDistance = 435;
            this.splitContainerGraphSettingsVsControls.SplitterWidth = 5;
            this.splitContainerGraphSettingsVsControls.TabIndex = 6;
            // 
            // splitContainerGraphVsSettings
            // 
            this.splitContainerGraphVsSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerGraphVsSettings.Location = new System.Drawing.Point(0, 0);
            this.splitContainerGraphVsSettings.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainerGraphVsSettings.Name = "splitContainerGraphVsSettings";
            // 
            // splitContainerGraphVsSettings.Panel1
            // 
            this.splitContainerGraphVsSettings.Panel1.Controls.Add(this.chartSpectrum);
            // 
            // splitContainerGraphVsSettings.Panel2
            // 
            this.splitContainerGraphVsSettings.Panel2.Controls.Add(this.groupBox2);
            this.splitContainerGraphVsSettings.Size = new System.Drawing.Size(1099, 435);
            this.splitContainerGraphVsSettings.SplitterDistance = 772;
            this.splitContainerGraphVsSettings.SplitterWidth = 5;
            this.splitContainerGraphVsSettings.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1131, 708);
            this.Controls.Add(this.splitContainerAppVsEventLog);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form1";
            this.Text = "OmniDriver C# Demo";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.chartSpectrum)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSaveEverySec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxScans)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.tabControlSettings.ResumeLayout(false);
            this.tabPageAcquisition.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIntegrationTimeMillisec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScansToAverage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSmoothingBoxcar)).EndInit();
            this.tabPageGPIO.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabPageNoise.ResumeLayout(false);
            this.tabPageNoise.PerformLayout();
            this.groupBoxNoiseSettings.ResumeLayout(false);
            this.groupBoxNoiseSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRMSMeasurements)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.splitContainerAppVsEventLog.Panel1.ResumeLayout(false);
            this.splitContainerAppVsEventLog.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerAppVsEventLog)).EndInit();
            this.splitContainerAppVsEventLog.ResumeLayout(false);
            this.splitContainerGraphSettingsVsControls.Panel1.ResumeLayout(false);
            this.splitContainerGraphSettingsVsControls.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerGraphSettingsVsControls)).EndInit();
            this.splitContainerGraphSettingsVsControls.ResumeLayout(false);
            this.splitContainerGraphVsSettings.Panel1.ResumeLayout(false);
            this.splitContainerGraphVsSettings.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerGraphVsSettings)).EndInit();
            this.splitContainerGraphVsSettings.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartSpectrum;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.CheckBox checkBoxEnableIrradiance;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numericUpDownIntegrationTimeMillisec;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBoxEventLog;
        private System.Windows.Forms.CheckBox checkBoxEnableDataCollection;
        private System.Windows.Forms.SaveFileDialog saveFileDialogSaveAll;
        private System.Windows.Forms.NumericUpDown numericUpDownSmoothingBoxcar;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonMergeSaved;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogMergeFolder;
        private System.Windows.Forms.NumericUpDown numericUpDownScansToAverage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxScans;
        private System.Windows.Forms.Label labelSaveCount;
        private System.Windows.Forms.CheckBox checkBoxEnableNLC;
        private System.Windows.Forms.CheckBox checkBoxEnableEDC;
        private System.Windows.Forms.Button buttonOptimizeIntegrationTimes;
        private System.Windows.Forms.SplitContainer splitContainerAppVsEventLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownSaveEverySec;
        private System.Windows.Forms.CheckBox checkBoxStrobeEnable;
        private System.Windows.Forms.CheckBox checkBoxSaveAll;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox checkBoxGPIOAlt3;
        private System.Windows.Forms.CheckBox checkBoxGPIODir3;
        private System.Windows.Forms.CheckBox checkBoxGPIOVal3;
        private System.Windows.Forms.CheckBox checkBoxGPIOAlt2;
        private System.Windows.Forms.CheckBox checkBoxGPIODir2;
        private System.Windows.Forms.CheckBox checkBoxGPIOVal2;
        private System.Windows.Forms.CheckBox checkBoxGPIOAlt1;
        private System.Windows.Forms.CheckBox checkBoxGPIODir1;
        private System.Windows.Forms.CheckBox checkBoxGPIOVal1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxGPIOAlt0;
        private System.Windows.Forms.CheckBox checkBoxGPIODir0;
        private System.Windows.Forms.CheckBox checkBoxGPIOVal0;
        private System.Windows.Forms.SplitContainer splitContainerGraphVsSettings;
        private System.Windows.Forms.SplitContainer splitContainerGraphSettingsVsControls;
        private System.Windows.Forms.TabControl tabControlSettings;
        private System.Windows.Forms.TabPage tabPageAcquisition;
        private System.Windows.Forms.TabPage tabPageGPIO;
        private System.Windows.Forms.TabPage tabPageNoise;
        private System.Windows.Forms.CheckBox checkBoxComputeNoise;
        private System.Windows.Forms.GroupBox groupBoxNoiseSettings;
        private System.Windows.Forms.Label labelNoiseResult;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDownRMSMeasurements;
    }
}

