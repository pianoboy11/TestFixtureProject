using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media.Imaging;


//using MagicSquareComp;
using MathWorks.MATLAB.NET.Arrays;

namespace TestFixtureProject
{
    using AForge.Video;
    using AForge.Video.DirectShow;
    using TestFixtureProject.Common;
    using TestFixtureProject.DataAccess;
    using TestFixtureProject.Model;
    using TestFixtureProject.ViewModel;

    public partial class frmTestFixture : Form
    {

        #region private

        private DataSet _LineTesterDataset = null;
        private DataTable _eolTable = null;
        private DataTable _lightEngineTable = null;


        internal frmTestFixtureLogin _frmTestFixtureLogin { get; set; }
        TestFixtureMainWindowVM vm;
        internal TestFixtureViewModel pagevm;
        internal ApplicationConstants.UserMode userMode { get; set; }

        int mycounter = 0;
        int keeprunning = 0;
        string mycommand = null;
        int turnoffled = 0;

        System.Threading.Thread ftpthread = default(System.Threading.Thread);

        internal int errorMessageCounter = 0;
        internal int warningMessageCounter = 0;
        internal int informationMessageCounter = 0;

        //singleton design pattern
        private static frmTestFixture _instance;

        public static frmTestFixture Instance
        {
            get
            {
                //if (Instance == null)
                //    _instance = new frmTestFixtureMetroUI();
                return _instance;
            }
        }

        double totalspeed = 0;
        bool showspeed = false;
        int counter = 0;
        bool turnoffleds = false;
        BitmapImage bitimg;
        VideoCaptureDevice captdevice;
        string images = "captured.png";
        int i = 0;
        internal bool _accessGranted;
        internal bool _partialAccess;
        OmniDriver.NETWrapper wrapper;
        internal bool IsCalibrationFileFound;

        internal TestFixtureLoginViewModel pagelvm;
        internal MetroFramework.Controls.MetroTabControl tc;
        internal string CalibrationFilePath;
        internal TestFixtureReadSpectrometer testFixtureReadSpectrometer;
        #endregion

        #region Constructor
        public frmTestFixture()
        {
            InitializeComponent();

            //try
            //{
            //    //WindowsFormsHost formhost = new WindowsFormsHost();
            //    //Form formhost = new Form();
            //    OmniDriverCSharpDemo.Form1 omnidemo = new OmniDriverCSharpDemo.Form1();
            //    omnidemo.TopLevel = false;
            //    omnidemo.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //    omnidemo.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            //    omnidemo.AutoSize = false;
            //    omnidemo.Dock = DockStyle.Fill;
            //    omnidemo.Visible = true;

            //    omnidemo.BringToFront();
            //    //formhost.Child = omnidemo;
            //    //formhost.chi = omnidemo;
            //    mPanelCalibration.Controls.Add(omnidemo);
            //    //this.specgrid.Children.Add(formhost);
            //}
            //catch (Exception ex)
            //{

            //}

            // Load 
            vm = new TestFixtureMainWindowVM();

            if (pagevm == null)
                pagevm = new TestFixtureViewModel();

            pagevm.ResetDaqBoardPort();

            _instance = this;
        }
        #endregion

        #region Background Worker
        public enum BW_OPERATIONS
        {
            OPERATION_PAIRING_SEQUENCE,
            OPERATION_FIXED_RED,
            OPERATION_FIXED_GREEN,
            OPERATION_FIXED_WHITE,
            OPERATION_FIXED_BLUE,
            OPERATION_FIND_IP_ADDRESS,
            OPERATION_DIRECT_IP_ADDRESS_PING,
            OPERATION_START_TEST_SEQUENCE
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            ProcessBackgroundWorkerOperations(sender, e);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //SetErrorMessageDisplayTextBox(e.ProgressPercentage.ToString() + "%");
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void ProcessBackgroundWorkerOperations(object sender, DoWorkEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            switch (e.Argument)
            {
                case BW_OPERATIONS.OPERATION_PAIRING_SEQUENCE:

                    if (TestFixtureViewModel.Instance != null)
                        TestFixtureViewModel.Instance.StartPairingSequenceDiagnostic();
                    break;

                case BW_OPERATIONS.OPERATION_FIXED_RED:
                    break;

                case BW_OPERATIONS.OPERATION_FIXED_GREEN:
                    break;

                case BW_OPERATIONS.OPERATION_FIXED_WHITE:
                    break;

                case BW_OPERATIONS.OPERATION_FIXED_BLUE:
                    break;

                case BW_OPERATIONS.OPERATION_FIND_IP_ADDRESS:

                    //Keith Dudley
                    //tFixedRed.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.tFixedRed.Enabled = TestFixtureSocketComm._IsIpAddressFound; });
                    //tFixedGreen.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.tFixedGreen.Enabled = TestFixtureSocketComm._IsIpAddressFound; });
                    //tFixedBlue.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.tFixedBlue.Enabled = TestFixtureSocketComm._IsIpAddressFound; });
                    //tFixedWhite.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.tFixedWhite.Enabled = TestFixtureSocketComm._IsIpAddressFound; });

                    try
                    {
                        TestFixtureSocketComm.DiscoverPentairServer(false);

                        if (TestFixtureSocketComm.serverStatus.Equals(TestFixtureSocketComm.serverUnavailable))
                        {
                            SetStatusIndicatorTextBoxBackColor(Color.Red);
                            return;
                        }

                        if (btnGetRedSpectrum.Created)
                            btnGetRedSpectrum.Invoke((MethodInvoker)delegate { btnGetRedSpectrum.Enabled = TestFixtureSocketComm._IsIpAddressFound; });
                        if (btnGetGreenSpectrum.Created)
                            btnGetGreenSpectrum.Invoke((MethodInvoker)delegate { btnGetGreenSpectrum.Enabled = TestFixtureSocketComm._IsIpAddressFound; });
                        if (btnGetBlueSpectrum.Created)
                            btnGetBlueSpectrum.Invoke((MethodInvoker)delegate { btnGetBlueSpectrum.Enabled = TestFixtureSocketComm._IsIpAddressFound; });
                        if (btnGetWhiteSpectrum.Created)
                            btnGetWhiteSpectrum.Invoke((MethodInvoker)delegate { btnGetWhiteSpectrum.Enabled = TestFixtureSocketComm._IsIpAddressFound; });
                        if (btnReadAllColors.Created)
                            btnReadAllColors.Invoke((MethodInvoker)delegate { btnReadAllColors.Enabled = TestFixtureSocketComm._IsIpAddressFound; });

                        SetStatusIndicatorTextBoxBackColor(Color.Green);

                    }
                    catch (Exception ex)
                    {
                        SetStatusIndicatorTextBoxBackColor(Color.Red);
                        WriteToLog("ProcessBackgroundWorkerOperations: " + ex.Message, ApplicationConstants.TraceLogType.Error);
                    }
                    break;

                case BW_OPERATIONS.OPERATION_DIRECT_IP_ADDRESS_PING:

                    if (!string.IsNullOrEmpty(txtIpAddress.Text))
                    {
                        try
                        {
                            IPAddress address = IPAddress.Parse(txtIpAddress.Text);

                            string serverStatus = TestFixtureSocketComm.ConnectToServer(address);

                            serverStatus.Equals(TestFixtureSocketComm.connected);

                            SetIpAddressLabel(serverStatus);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "IP Address Ping", MessageBoxButtons.OK);
                            WriteToLog("BW_OPERATIONS.OPERATION_DIRECT_IP_ADDRESS_PING: " + ex.Message, ApplicationConstants.TraceLogType.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("txtIpAddress is null", "IP Address Ping", MessageBoxButtons.OK);
                        WriteToLog("BW_OPERATIONS.OPERATION_DIRECT_IP_ADDRESS_PING: txtIpAddress is null", ApplicationConstants.TraceLogType.Warning);
                    }

                    break;

                case BW_OPERATIONS.OPERATION_START_TEST_SEQUENCE:
                    //TestFixtureViewModel.Instance.StartTesting(e);

                    try
                    {

                        if (pagevm != null)
                        {
                            ResetEolTreeView();

                            ResetLightEngineTreeView();

                            pagevm.StartExecutionEvents();
                            //if (pagevm._mtestfixturedaq != null)
                            //{
                            //    pagevm._mtestfixturedaq.PressStartButonOnFixtureSimulator();

                            //   // pagevm._mtestfixturedaq.PressStartButonOnFixture();
                            //}
                            //else
                            //   WriteToLog("BW_OPERATIONS.OPERATION_START_TEST_SEQUENCE: TestFixtureDAQ is null", ApplicationConstants.TraceLogType.Warning);
                        }
                        else
                            WriteToLog("BW_OPERATIONS.OPERATION_START_TEST_SEQUENCE: TestFixtureViewModel is null", ApplicationConstants.TraceLogType.Warning);
                    }
                    catch (Exception ex)
                    {
                        WriteToLog("BW_OPERATIONS.OPERATION_START_TEST_SEQUENCE ERROR: " + ex.Message, ApplicationConstants.TraceLogType.Error);
                    }
                    break;

                default:
                    break;
            }

            Cursor.Current = Cursors.Default;
        }
        #endregion

        #region Event Handlers
        private void frmTestFixture_Load(object sender, EventArgs e)
        {
            if (TestFixtureConstants.CreateIllumaVisionRootDirectories())
            {
                SetStatusIndicatorVisibleProperty(false);

                timer1.Start();

                tsTime.Text = DateTime.Now.ToString("F");

                RemoveEngineeringPortal();

                SetErrorMessageDisplayTextBox("Press Green Button to Start...");

                if (FindSpectrometers())
                    FindCalibrationFile();

                progressBar1.Value = 0;
                this.tabControl1.SelectedIndex = 0;

                GetConfigurationSettings();

                GetMirrorSettings();

                CreateTestSequenceDataset();

                dgvEol.AllowUserToAddRows = false;
                dgvLightEngine.AllowUserToAddRows = false;

                if (pagevm._lineTestermodel.EOL)
                {
                    cbLineTesterType.SelectedIndex = 0;
                    lblLineTester.Text = "EOL LINE TESTER";
                    WriteToLog("EOL Line Tester selected...", ApplicationConstants.TraceLogType.Information);

                    tvEol.ExpandAll();
                    tvEol.CheckBoxes = true;
                }
                else if (pagevm._lineTestermodel.LightEngine)
                {
                    cbLineTesterType.SelectedIndex = 1;
                    lblLineTester.Text = "LIGHT ENGINE LINE TESTER";
                    WriteToLog("LIGHT ENGINE Line Tester selected...", ApplicationConstants.TraceLogType.Information);

                    tvLightEngine.ExpandAll();
                    tvLightEngine.CheckBoxes = true;
                }

                SetLineTesterTabPages();

                ResetEolTreeView();
                ResetLightEngineTreeView();
            }
            else
            {
                MessageBox.Show("Error occured creating IllumaVision config files. " + Environment.NewLine + 
                                "Please contact illumaVision support team. ", 
                                "IllumaVision CONFIG FILES", 
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Error);
            }
        }

        private void CreateTestSequenceDataset()
        {
            _eolTable = new DataTable("EOL");
            _eolTable.Columns.Add("Test", typeof(string));
            _eolTable.Columns.Add("RunTest", typeof(bool));

            _lightEngineTable = new DataTable("LightEngine");
            _lightEngineTable.Columns.Add("Test", typeof(string));
            _lightEngineTable.Columns.Add("RunTest", typeof(bool));

            // Create a DataSet and put both tables in it.
            _LineTesterDataset = new DataSet("LineTester"); ;
            _LineTesterDataset.Tables.Add(_eolTable);
            _LineTesterDataset.Tables.Add(_lightEngineTable);

            GetEolTestSequenceSettings();
            dgvEol.DataSource = _LineTesterDataset.Tables["EOL"];

            GetLightEngineTestSequenceSettings();
            dgvLightEngine.DataSource = _LineTesterDataset.Tables["LightEngine"];
        }

        private void GetEolTestSequenceSettings()
        {

            dynamic eolResult = JsonConvert.SerializeObject(pagevm._eolmodel);
            JObject eolObject = JObject.Parse(eolResult);
            TestFixtureEolTestSequenceModel eolData = JsonConvert.DeserializeObject<TestFixtureEolTestSequenceModel>(eolResult);

            _eolTable.Clear();

            _eolTable.Rows.Add("ModelNumber", eolData.ModelNumber);
            _eolTable.Rows.Add("SerialNumber", eolData.SerialNumber);
            _eolTable.Rows.Add("PowerOn", eolData.PowerOn);
            _eolTable.Rows.Add("Pairing", eolData.Pairing);
            _eolTable.Rows.Add("PentairServer", eolData.PentairServer);
            _eolTable.Rows.Add("FirmwareVersion", eolData.FirmwareVersion);
            _eolTable.Rows.Add("ProjectorMirrorCheck", eolData.ProjectorMirrorCheck);
            _eolTable.Rows.Add("LedBrightnessColor", eolData.LedBrightnessColor);
            _eolTable.Rows.Add("Bandwidth", eolData.Bandwidth);
            _eolTable.Rows.Add("ProjectorFocus", eolData.ProjectorFocus);
            _eolTable.Rows.Add("ProjectorBrightness", eolData.ProjectorBrightness);
            _eolTable.Rows.Add("TestCompletion", eolData.TestCompletion);
            //_eolTable.Rows.Add("PartUnload", eolData.PartUnload);
        }

        private void GetLightEngineTestSequenceSettings()
        {
            var lightEngineResult = JsonConvert.SerializeObject(pagevm._lightenginemodel);
            JObject lightEngineObject = JObject.Parse(lightEngineResult);
            TestFixtureLightEngineTestSequenceModel lightEngineData = JsonConvert.DeserializeObject<TestFixtureLightEngineTestSequenceModel>(lightEngineResult);

            _lightEngineTable.Clear();

            _lightEngineTable.Rows.Add("SerialNumber", lightEngineData.SerialNumber);
            _lightEngineTable.Rows.Add("PowerOn", lightEngineData.PowerOn);
            _lightEngineTable.Rows.Add("PentairServer", lightEngineData.PentairServer);
            _lightEngineTable.Rows.Add("FirmwareVersion", lightEngineData.FirmwareVersion);
            _lightEngineTable.Rows.Add("ProjectorMirrorCheck", lightEngineData.ProjectorMirrorCheck);
            _lightEngineTable.Rows.Add("LedBrightnessColor", lightEngineData.LedBrightnessColor);
            _lightEngineTable.Rows.Add("ProjectorFocus", lightEngineData.ProjectorFocus);
            _lightEngineTable.Rows.Add("ProjectorBrightness", lightEngineData.ProjectorBrightness);
            _lightEngineTable.Rows.Add("TestCompletion", lightEngineData.TestCompletion);
        }

        private void SetLightEngineTestSequenceSettings()
        {
            var lightEngineResult = JsonConvert.SerializeObject(pagevm._eolmodel);
            JObject lightEngineObject = JObject.Parse(lightEngineResult);
            TestFixtureLightEngineTestSequenceModel lightEngineData = JsonConvert.DeserializeObject<TestFixtureLightEngineTestSequenceModel>(lightEngineResult);

            lightEngineData.SerialNumber = true;
            lightEngineData.PowerOn = true;
            lightEngineData.PentairServer = true;
            lightEngineData.FirmwareVersion = true;
            lightEngineData.ProjectorMirrorCheck = true;
            lightEngineData.LedBrightnessColor = true;
            lightEngineData.ProjectorFocus = true;
            lightEngineData.ProjectorBrightness = true;
            lightEngineData.TestCompletion = true;
        }


        private void LoadLineTester()
        {
            //GetEolTestSequenceSettings();
            //dataGridView1.DataSource = _LineTesterDataset.Tables["EOL"];

            //GetLightEngineTestSequenceSettings();
            //dataGridView2.DataSource = _LineTesterDataset.Tables["LightEngine"];       
        }


        public TreeNode[] Find(TreeNode motherNode, string findNodeText)
        {
            List<TreeNode> nodeList = new List<TreeNode>();
            foreach (TreeNode childNode in motherNode.Nodes)
                if (childNode.Text.Equals(findNodeText, StringComparison.CurrentCulture))
                    nodeList.Add(childNode);
            return nodeList.ToArray<TreeNode>();
        }

        private void tpDiagnostics_Click(object sender, EventArgs e)
        {
            ShowTabPages(tpDiagnostics);
        }

        private void mainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTabPages(tpMain);
        }

        private void projectDiagnosticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTabPages(tpProjectDiagnostics);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTabPages(tpSettings);
        }

        private void imageSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTabPages(tpImageSettings);
        }

        private void calibrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTabPages(tpCalibration);
        }

        private void spectrumDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //ShowTabPages(tpSpectrumData);
        }

        private void showEngineeringPortalToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //ShowEngineeringPortal();

            ShowLoginForm();
        }

        private void removeEngineeringPortalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveEngineeringPortal();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        #region private methods
        void ShowTabPages(TabPage tabPage)
        {
            if (tabPage.Name != "tpMain")
            {
                if (!tabControl2.TabPages.Contains(tabPage))
                {
                    tabControl2.SuspendLayout();
                    tabControl2.TabPages.Remove(tabPage);
                    tabControl2.TabPages.Add(tabPage);
                    tabControl2.ResumeLayout();
                    tabControl2.TabPages[tabPage.Name].Show();
                }
                else
                {
                    tabControl2.SelectedTab = tabPage;
                }
            }
        }

        void ShowLineTesterTabPages(TabPage tabPage)
        {
            if (!tcLineTester.TabPages.Contains(tabPage))
            {
                tcLineTester.SuspendLayout();
                tcLineTester.TabPages.Remove(tabPage);
                tcLineTester.TabPages.Add(tabPage);
                tcLineTester.ResumeLayout();
                tcLineTester.TabPages[tabPage.Name].Show();
            }
            else
            {
                tcLineTester.SelectedTab = tabPage;
            }
        }

        void ShowLoginForm()
        {
            pagelvm = new TestFixtureLoginViewModel();

            _frmTestFixtureLogin = new frmTestFixtureLogin();

            DialogResult dialogResult = _frmTestFixtureLogin.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                ShowEngineeringPortal();
                tsApplicationUserMode.Text = "ENGINEERING";
            }
            else
            {
                RemoveEngineeringPortal();
                tsApplicationUserMode.Text = "OPERATOR";
            }
        }

        void ShowEngineeringPortal()
        {
            ShowTabPages(tpDiagnostics);
            ShowTabPages(tpProjectDiagnostics);
            ShowTabPages(tpSettings);
            ShowTabPages(tpImageSettings);
            ShowTabPages(tpCalibration);
            ShowTabPages(tpTestSequence);
            //ShowTabPages(tpSpectrumData);

            tabControl2.SelectedTab = tpMain;

            tsApplicationUserMode.Text = "ENGINEERING";
        }

        void RemoveEngineeringPortal()
        {
            tpDiagnostics.Hide();
            tpProjectDiagnostics.Hide();
            tpSettings.Hide();
            tpImageSettings.Hide();
            tpCalibration.Hide();
            tpTestSequence.Hide();
            //tpSpectrumData.Hide();

            tabControl2.TabPages.Remove(tpDiagnostics);
            tabControl2.TabPages.Remove(tpProjectDiagnostics);
            tabControl2.TabPages.Remove(tpSettings);
            tabControl2.TabPages.Remove(tpImageSettings);
            tabControl2.TabPages.Remove(tpCalibration);
            tabControl2.TabPages.Remove(tpTestSequence);
            //tabControl2.TabPages.Remove(tpSpectrumData);

            tabControl2.SelectedTab = tpMain;

            tsApplicationUserMode.Text = "OPERATOR";
        }
        #endregion

        #region Crossthread Methods
        internal void SetUpperXValueTextbox(string message)
        {
            try
            {
                txtSetUpperXValue.Invoke((MethodInvoker)delegate { txtSetUpperXValue.Text = message; txtSetUpperXValue.Refresh(); });
            }
            catch { }
        }

        internal void SetUpperYValueTextbox(string message)
        {
            try
            {
                txtSetUpperYValue.Invoke((MethodInvoker)delegate { txtSetUpperYValue.Text = message; txtSetUpperYValue.Refresh(); });
            }
            catch { }
        }

        internal void SetWattTextbox(string message)
        {
            try
            {
                txtSetWattValue.Invoke((MethodInvoker)delegate { txtSetWattValue.Text = message; txtSetWattValue.Refresh(); });
            }
            catch { }
        }

        internal void SetEolTreeViewTestSequenceCheckProperty(string testSequenceName, bool isChecked)
        {
            try
            {


                //KD Winform: Set textbox backcolor
                tvEol.Invoke((MethodInvoker)delegate

                {

                    if (testSequenceName == "nodeEolTestSequence")
                    {
                        tvEol.Nodes[testSequenceName].Checked = isChecked;
                        tvEol.Nodes[testSequenceName].ForeColor = Color.Green;
                        return;
                    }

                    TreeNode[] nodes = Find(tvEol.Nodes[0], testSequenceName);

                    foreach (var node in nodes)
                    {
                        if (node.Text == testSequenceName)
                        {
                            if (isChecked)
                            {
                                node.Checked = true;
                                node.ForeColor = Color.Green;
                            }
                            else
                            {
                                node.Checked = false;
                                node.ForeColor = Color.Red;
                            }
                        }
                    }

                    //tvTestSequence.Nodes[testSequenceName].Checked = isChecked;
                    //tvEol.Refresh();

                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        internal void SetLightEngineTreeViewTestSequenceCheckProperty(string testSequenceName, bool isChecked)
        {
            try
            {


                //KD Winform: Set textbox backcolor
                tvEol.Invoke((MethodInvoker)delegate

                {

                    if (testSequenceName == "nodeLightEngineTestSequence")
                    {
                        tvLightEngine.Nodes[testSequenceName].Checked = isChecked;
                        tvLightEngine.Nodes[testSequenceName].ForeColor = Color.Green;
                        return;
                    }

                    TreeNode[] nodes = Find(tvLightEngine.Nodes[0], testSequenceName);

                    foreach (var node in nodes)
                    {
                        if (node.Text == testSequenceName)
                        {
                            if (isChecked)
                            {
                                node.Checked = true;
                                node.ForeColor = Color.Green;
                            }
                            else
                            {
                                node.Checked = false;
                                node.ForeColor = Color.Red;
                            }
                        }
                    }

                    //tvLightEngine.Nodes[testSequenceName].Checked = isChecked;
                    //tvLightEngine.Refresh();

                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        internal void SetModelNumberTextbox(string message)
        {
            try
            {
                //KD Winform: Set model number textbox
                txtModelNumber.Invoke((MethodInvoker)delegate { txtModelNumber.Text = message; txtModelNumber.Refresh(); });
            }
            catch { }
        }

        internal void SetSerialNumberTextbox(string message)
        {
            try
            {
                //KD Winform: Set serial number textbox
                txtSerialNumber.Invoke((MethodInvoker)delegate { txtSerialNumber.Text = message; txtSerialNumber.Refresh(); });
            }
            catch { }
        }

        internal void SetVersionNumberTextbox(string message)
        {
            try
            {
                TestFixtureSocketComm.firmwareVersion = string.Empty;

                //KD Winform: Set version number textbox
                txtVersionNumber.Invoke((MethodInvoker)delegate { txtVersionNumber.Text = message; });
            }
            catch { }
        }

        internal void SetVoltageTextbox(string message)
        {
            try
            {
                //KD Winform: Set voltage textbox
                txtVoltage.Invoke((MethodInvoker)delegate { txtVoltage.Text = message; });
            }
            catch { }
        }

        internal void SetErrorMessageDisplayTextBox(string message)
        {
            try
            {

                //KD Winform: Set error message display textbox 
                txtErrorMsgDisplay.Invoke((MethodInvoker)delegate { txtErrorMsgDisplay.Text = message; txtErrorMsgDisplay.Refresh(); /*txtErrorMsgDisplay.BackColor = Color.Red;*/ });
            }
            catch { }
        }

        internal void SetStatusIndicatorTextBox(string message)
        {
            try
            {

                //KD Winform: Set status indicator message textbox 
                txtStatusIndicator.Invoke((MethodInvoker)delegate { txtStatusIndicator.Text = message; txtStatusIndicator.Refresh(); /*txtErrorMsgDisplay.BackColor = Color.Red;*/ });
            }
            catch { }
        }

        internal void SetStatusIndicatorTextBoxBackColor(Color color)
        {
            try
            {
                if (color == Color.Red)
                {
                    SetStatusIndicatorTextBox("FAIL");
                }
                else
                {
                    SetStatusIndicatorTextBox("PASS");
                }

                //KD Winform: Set textbox backcolor
                txtStatusIndicator.Invoke((MethodInvoker)delegate { txtStatusIndicator.BackColor = color; txtStatusIndicator.Refresh(); });
            }
            catch { }
        }

        internal void SetStatusIndicatorVisibleProperty(bool isTest)
        {
            try
            {


                //KD Winform: Set textbox backcolor
                txtStatusIndicator.Invoke((MethodInvoker)delegate { txtStatusIndicator.Visible = isTest; txtStatusIndicator.Refresh(); });
            }
            catch { }
        }

        //Keith Dudley Remove
        //internal void SetUpdatePassFailResultTextBox(string message)
        //{
        //    //KD Winform: Set UpdatePassFailResult textbox 
        //    txtErrorMsgDisplay.Invoke((MethodInvoker)delegate { txtErrorMsgDisplay.Text = message; });
        //}

        internal void SetTraceLogTextBox(string message, ApplicationConstants.TraceLogType traceLogType)
        {
            string msg = string.Empty;

            switch (traceLogType)
            {
                case ApplicationConstants.TraceLogType.Information:
                    //KD Winform: Set trace log txtInformation 
                    informationMessageCounter++;

                    tabControl1.TabPages["tpInformation"].Invoke((MethodInvoker)delegate { tabControl1.TabPages["tpInformation"].Text = string.Format("Information ({0})", informationMessageCounter); });
                    msg = String.Format("{0}. {1} - {2}", informationMessageCounter, DateTime.Now, message);
                    pagevm.InformationStatusMessage += msg.ToString() + Environment.NewLine;
                    txtInformation.Invoke((MethodInvoker)delegate { txtInformation.Text = pagevm.InformationStatusMessage; });
                    break;

                case ApplicationConstants.TraceLogType.Warning:
                    //KD Winform: Set trace log txtInformation 
                    warningMessageCounter++;
                    tabControl1.TabPages["tpWarnings"].Invoke((MethodInvoker)delegate { tabControl1.TabPages["tpWarnings"].Text = string.Format("Warnings ({0})", warningMessageCounter); });
                    msg = String.Format("{0}. {1} - {2}", warningMessageCounter, DateTime.Now, message);
                    pagevm.WarningStatusMessage += msg.ToString() + Environment.NewLine;
                    txtWarnings.Invoke((MethodInvoker)delegate { txtWarnings.Text = pagevm.WarningStatusMessage; });
                    break;

                case ApplicationConstants.TraceLogType.Error:
                    //KD Winform: Set trace log txtInformation 
                    errorMessageCounter++;
                    tabControl1.TabPages["tpErrors"].Invoke((MethodInvoker)delegate { tabControl1.TabPages["tpErrors"].Text = string.Format("Errors ({0})", errorMessageCounter); });
                    msg = String.Format("{0}. {1} - {2}", errorMessageCounter, DateTime.Now, message);
                    pagevm.ErrorStatusMessage += msg.ToString() + Environment.NewLine;
                    txtErrors.Invoke((MethodInvoker)delegate { txtErrors.Text = pagevm.ErrorStatusMessage; });
                    break;

                default:
                    MessageBox.Show("Undefined tracelog type: " + traceLogType.ToString(), "ApplicationConstants.TraceLogType");
                    break;
            }
        }

        internal void SetImpedanceVoltageTextbox(string message)
        {
            try
            {
                //KD Winform: Set impedance voltage textbox 
                txtImpedanceVoltage.Invoke((MethodInvoker)delegate { txtImpedanceVoltage.Text = message; });
            }
            catch { }
        }

        internal void SetAmbientTempTextbox(string message)
        {
            try
            {
                //if (txtReadAmbientTemp.InvokeRequired)
                //KD Winform: Set ambient temperature textbox 
                txtReadAmbientTemp.Invoke((MethodInvoker)delegate { txtReadAmbientTemp.Text = message; });
            }
            catch { }
        }

        internal void Set120VACRadioButton(bool isCheck)
        {
            try
            {
                //KD Winform: Set rb120VAC 
                rb120VAC.Invoke((MethodInvoker)delegate { rb120VAC.Checked = isCheck; });
            }
            catch { }
        }

        internal void Set15VACRadioButton(bool isCheck)
        {
            try
            {
                //KD Winform: Set cb15VAC 
                rb15VAC.Invoke((MethodInvoker)delegate { rb15VAC.Checked = isCheck; });
            }
            catch { }
        }

        internal void SetTraceLogListView(string message)
        {
            //KD Winform: Set trace log listview
            //if (lvTraceLog.InvokeRequired)
            //{
            //    lvTraceLog.Invoke((MethodInvoker)delegate
            //    {
            //        lvTraceLog.Items.Add(message);

            //        lvTraceLog.Items[lvTraceLog.Items.Count - 1].EnsureVisible();

            //    });
            //}
            //else
            //{
            //    lvTraceLog.Items.Add(message);
            //    lvTraceLog.Items[lvTraceLog.Items.Count - 1].EnsureVisible();
            //}
        }

        internal void SetProgressStatusBar(double value)
        {
            //KD Winform: Set progress status bar
            progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = (int)value; });
        }

        internal void SetProgressStatusBarMin(double value)
        {
            //KD Winform: Set progress status bar min
            progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Minimum = (int)value; });
        }

        internal void SetProgressStatusBarMax(double value)
        {
            //KD Winform: Set progress status bar max
            progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Maximum = (int)value; });
        }

        internal void ResetProgressStatusBar(double value)
        {
            //KD Winform: reset progress status bar
            progressBar1.Invoke((MethodInvoker)delegate { progressBar1.Value = (int)value; });
        }

        internal void SetFTPProgressStatusBar(int value)
        {
            //KD Winform: FTP progress status bar
            pbFTPDataThroughProgress.Invoke((MethodInvoker)delegate { pbFTPDataThroughProgress.Value = value; });
        }

        internal void SetVoltageCheckbox(bool value)
        {
            try
            {
                //KD Winform: Set voltage checkbox
                cbStem.Invoke((MethodInvoker)delegate { cbStem.Checked = value; });
            }
            catch { }
        }

        internal void SetWhiteCheckbox(bool value)
        {
            try
            {
                //KD Winform: Set white checkbox
                cbWhite.Invoke((MethodInvoker)delegate { cbWhite.Checked = value; });
            }
            catch { }
        }

        internal void SetGreenCheckbox(bool value)
        {
            try
            {
                //KD Winform: Set green checkbox
                cbGreen.Invoke((MethodInvoker)delegate { cbGreen.Checked = value; });
            }
            catch { }
        }

        internal void SetBlueCheckbox(bool value)
        {
            try
            {
                //KD Winform: Set blue checkbox
                cbBlue.Invoke((MethodInvoker)delegate { cbBlue.Checked = value; });
            }
            catch { }
        }

        internal void SetRedCheckbox(bool value)
        {
            try
            {
                //KD Winform: Set red checkbox
                cbRed.Invoke((MethodInvoker)delegate { cbRed.Checked = value; });
            }
            catch { }
        }

        internal void SetPictureBox(Image value)
        {
            try
            {
                //KD Winform: Set picture box
                pbImage.Invoke((MethodInvoker)delegate { pbImage.Image = value; });
            }
            catch { }
        }

        internal void SetIpAddressLabel(string value)
        {
            try
            {
                //KD Winform: Set IP address label
                lbIpAddress.Invoke((MethodInvoker)delegate { lbIpAddress.Text = value; });
            }
            catch { }
        }

        internal void SetIpAddressTextBox(string value)
        {
            try
            {
                //KD Winform: Set IP address textbox
                txtIpAddress.Invoke((MethodInvoker)delegate { txtIpAddress.Text = value; });
            }
            catch { }
        }

        internal void SetSpeedOffTextbox(string message)
        {
            try
            {
                //KD Winform:Set SpeedOff textbox
                txtSpeedOff.Invoke((MethodInvoker)delegate { txtSpeedOff.Text = message; });
            }
            catch { }
        }

        internal void SetSpeedOnTextbox(string message)
        {
            try
            {
                //KD Winform:Set SpeedOn textbox
                txtSpeedOn.Invoke((MethodInvoker)delegate { txtSpeedOn.Text = message; });
            }
            catch { }
        }

        internal void SetFilesizeTextbox(string message)
        {
            try
            {
                //KD Winform:Set Filesize textbox
                txtFilesize.Invoke((MethodInvoker)delegate { txtFilesize.Text = message; });
            }
            catch { }
        }
        #endregion

        #region Log Information

        internal void WriteToLog(string message, ApplicationConstants.TraceLogType traceLogType)
        {
            if (pagevm == null)
                pagevm = new TestFixtureViewModel();

            string msg = string.Empty;
            //counter = counter + 1;
            // msg = String.Format("{0}. {1} - {2}", counter, DateTime.Now, message);
            //pagevm.StatusMessage += msg.ToString() + Environment.NewLine;
            SetTraceLogTextBox(message, traceLogType);
            //SetTraceLogListView(msg);
        }

        #endregion

        #region Methods
        void GetConfigurationSettings()
        {
            try
            {
                if (pagevm == null)
                    pagevm = new TestFixtureViewModel();

                //Magenta Limits
                txtMagendaXMinLimit.Text = pagevm.MagentaXValue1;
                txtMagendaXMaxLimit.Text = pagevm.MAGENTAMAXVALUE;
                txtMagendaYMinLimit.Text = pagevm.MAGENTAYMINVALUE;
                txtMagendaYMaxLimit.Text = pagevm.MAGENTAYMAXVALUE;
                txtMagendaWattMinLimit.Text = pagevm.MagentaMicroWattValue;
                txtMagendaWattMaxLimit.Text = pagevm.MagentaMicroWattValueUpper;

                //Blue Limits
                txtBLUELValue.Text = pagevm.BLRGBBLUELValue;
                txtBLUEUValue.Text = pagevm.BLRGBBLUEUValue;
                txtBlueYMinValue.Text = pagevm.BlueYMinValue;
                txtBlueYMaxValue.Text = pagevm.BlueYMaxValue;
                txtBlueMicroWattValueLower.Text = pagevm.BlueMicroWattValue;
                txtBlueMicroWattValueUpper.Text = pagevm.BlueMicroWattValueUpper;

                //Red Limits
                txtRedXMinValues.Text = pagevm.RLRGBREDLValue;
                txtRedXMaxValues.Text = pagevm.RLRGBREDUValue;
                txtRedYMinValues.Text = pagevm.RedYMinValues;
                txtRedYMaxValues.Text = pagevm.RedYMaxValue;
                txtRedMicroWattValueLower.Text = pagevm.RedMicroWattValue;
                txtRedMicroWattValueUpper.Text = pagevm.RedMicroWattValueUpper;

                //White Limits
                txtWhiteLimitXMinValue.Text = pagevm.WLRGBXMINVALUE;
                txtWhiteLimitXMaxValue.Text = pagevm.WLRGBXMAXVALUE;
                txtWhiteLimitYMinValue.Text = pagevm.WLRYMINVALUE;
                txtWhiteLimitYMaxValue.Text = pagevm.WLRGBXMAXVALUE;
                txtWhiteMicroWattValueLower.Text = pagevm.WhiteMicroWattValue;
                txtWhiteMicroWattValueUpper.Text = pagevm.WhiteMicroWattValueUpper;

                //Blended White Limits
                txtBlendedWhiteRedLLimits.Text = pagevm.BlendedWhiteRedLimits;
                txtBlendedWhiteRedULimits.Text = pagevm.BlendedWhiteRedULimits;
                txtBlendedWhiteGreenLLimits.Text = pagevm.BlendedWhiteGreenLLimits;
                txtBlendedWhiteGreenULimits.Text = pagevm.BlendedWhiteGreenULimits;
                txtBlendedWhiteBlueLLimits.Text = pagevm.BlendedWhiteBlueLLimits;
                txtBlendedWhiteBlueULimits.Text = pagevm.BlendedWhiteBlueULimits;

                //White Limits
                txtGREENLValue.Text = pagevm.GLRGBGREENLValue;
                txtGREENUValue.Text = pagevm.GLRGBGREENUValue;
                txtGreenYMinValues.Text = pagevm.GreenYMinValues;
                txtGreenYMaxValues.Text = pagevm.GreenYMaxValue;
                txtGreenMicroWattValueLower.Text = pagevm.GreenMicroWattValue;
                txtGreenMicroWattValueUpper.Text = pagevm.GreenMicroWattValueUpper;

                //LED Temperature Derating 
                txtLedTempDeratingWhiteValue.Text = pagevm.LedTempDeratingWhiteValue;
                txtLedTempDeratingRedValue.Text = pagevm.LedTempDeratingRedValue;
                txtLedTempDeratingGreenValue.Text = pagevm.LedTempDeratingGreenValue;
                txtLedTempDeratingBlueValue.Text = pagevm.LedTempDeratingBlueValue;

                //Fixture info
                txtFixtureId.Text = pagevm.FixtureId;
                txtCommPortNumber.Text = pagevm.CommPortNUmber;

                //Intergration Time
                txtIntegrationTime.Text = pagevm.IntegrationTime;
                txtBlueIntegrationTime.Text = pagevm.BlueIntegrationTime;
                txtRedIntegrationTime.Text = pagevm.RedIntegrationTime;
                txtGreenIntegrationTime.Text = pagevm.GreenIntegrationTime;
                txtWhiteIntegrationTime.Text = pagevm.WhiteIntegrationTime;
                txtMagendaIntegrationTime.Text = pagevm.MagendaIntegrationTime;
                txtBlendedWhiteIntegrationTime.Text = pagevm.BlendedWhiteIntegrationTime;

                //FTP Settings
                txtFtpOffMin.Text = pagevm.FTPOffMin;
                txtFtpOffMax.Text = pagevm.FTPOffMax;
                txtFtpOnMin.Text = pagevm.FTPOnMin;
                txtFtpOnMax.Text = pagevm.FTPOnMax;

                //Ip Address Range
                txtIPAddressRange.Text = pagevm.IPAddressRange;

                //BootSequenceTime
                txtBootSequenceTime.Text = pagevm.BootSequenceTime;
            }
            catch (Exception e)
            {
                WriteToLog("GetConfigurationSettings: " + e.Message, ApplicationConstants.TraceLogType.Error);
                SetStatusIndicatorTextBoxBackColor(Color.Red);
            }

        }

        void GetMirrorSettings()
        {
            try
            {
                if (pagevm != null)
                {
                    txtTopLeftX.Text = pagevm.UpdatetLmirrorXvalues;
                    txtTopLeftY.Text = pagevm.UpdatetLmirrorYvalues;

                    txtTopRightX.Text = pagevm.UpdatetrmirrorXvalues;
                    txtTopRightY.Text = pagevm.UpdatetrmirrorYvalues;

                    txtBottomLeftX.Text = pagevm.UpdateBLMirrorXValues;
                    txtBottomLeftY.Text = pagevm.UpdateBLMirrorYValues;

                    txtBottomRightX.Text = pagevm.UpdateBRMirrorXValues;
                    txtBottomRightY.Text = pagevm.UpdateBRMirrorYValues;

                    txtHomeScreenX.Text = pagevm.UpdateHomeScreenXValues;
                    txtHomeScreenY.Text = pagevm.UpdateHomeScreenYValues;
                }
                else
                {
                    WriteToLog("GetMirrorSettings: pagevm is null...", ApplicationConstants.TraceLogType.Warning);
                }
            }
            catch (Exception e)
            {
                WriteToLog("GetMirrorSettings: " + e.Message, ApplicationConstants.TraceLogType.Error);
            }

        }

        void SetConfigurationSettings()
        {
            try
            {
                if (TestFixtureViewModel.Instance != null)
                {
                    //Magenta Limits
                    TestFixtureViewModel.Instance.Model.MagentaXValue1 = txtMagendaXMinLimit.Text;
                    TestFixtureViewModel.Instance.Model.MAGENTAMAXVALUE = txtMagendaXMaxLimit.Text;
                    TestFixtureViewModel.Instance.Model.MAGENTAYMINVALUE = txtMagendaYMinLimit.Text;
                    TestFixtureViewModel.Instance.Model.MAGENTAYMAXVALUE = txtMagendaYMaxLimit.Text;
                    TestFixtureViewModel.Instance.Model.MagentaMicroWattValue = txtMagendaWattMinLimit.Text;
                    TestFixtureViewModel.Instance.Model.MagentaMicroWattValueUpper = txtMagendaWattMaxLimit.Text;

                    //Blue Limits
                    TestFixtureViewModel.Instance.Model.BLRGBBLUELValue = txtBLUELValue.Text;
                    TestFixtureViewModel.Instance.Model.BLRGBBLUEUValue = txtBLUEUValue.Text;
                    TestFixtureViewModel.Instance.Model.BlueYMinValue = txtBlueYMinValue.Text;
                    TestFixtureViewModel.Instance.Model.BlueYMaxValue = txtBlueYMaxValue.Text;
                    TestFixtureViewModel.Instance.Model.BlueMicroWattValue = txtBlueMicroWattValueLower.Text;
                    TestFixtureViewModel.Instance.Model.BlueMicroWattValueUpper = txtBlueMicroWattValueUpper.Text;

                    //Red Limits
                    TestFixtureViewModel.Instance.Model.RLRGBREDLValue = txtRedXMinValues.Text;
                    TestFixtureViewModel.Instance.Model.RLRGBREDUValue = txtRedXMaxValues.Text;
                    TestFixtureViewModel.Instance.Model.RedYMinValues = txtRedYMinValues.Text;
                    TestFixtureViewModel.Instance.Model.RedYMaxValue = txtRedYMaxValues.Text;
                    TestFixtureViewModel.Instance.Model.RedMicroWattValue = txtRedMicroWattValueLower.Text;
                    TestFixtureViewModel.Instance.Model.RedMicroWattValueUpper = txtRedMicroWattValueUpper.Text;

                    //White Limits
                    TestFixtureViewModel.Instance.Model.WLRGBXMINVALUE = txtWhiteLimitXMinValue.Text;
                    TestFixtureViewModel.Instance.Model.WLRGBXMAXVALUE = txtWhiteLimitXMaxValue.Text;
                    TestFixtureViewModel.Instance.Model.WLRYMINVALUE = txtWhiteLimitYMinValue.Text;
                    TestFixtureViewModel.Instance.Model.WLRGBXMAXVALUE = txtWhiteLimitYMaxValue.Text;
                    TestFixtureViewModel.Instance.Model.WhiteMicroWattValue = txtWhiteMicroWattValueLower.Text;
                    TestFixtureViewModel.Instance.Model.WhiteMicroWattValueUpper = txtWhiteMicroWattValueUpper.Text;

                    //Blended White Limits
                    TestFixtureViewModel.Instance.Model.BlendedWhiteRedLimits = txtBlendedWhiteRedLLimits.Text;
                    TestFixtureViewModel.Instance.Model.BlendedWhiteRedULimits = txtBlendedWhiteRedULimits.Text;
                    TestFixtureViewModel.Instance.Model.BlendedWhiteGreenLLimits = txtBlendedWhiteGreenLLimits.Text;
                    TestFixtureViewModel.Instance.Model.BlendedWhiteGreenULimits = txtBlendedWhiteGreenULimits.Text;
                    TestFixtureViewModel.Instance.Model.BlendedWhiteBlueLLimits = txtBlendedWhiteBlueLLimits.Text;
                    TestFixtureViewModel.Instance.Model.BlendedWhiteBlueULimits = txtBlendedWhiteBlueULimits.Text;

                    //White Limits
                    TestFixtureViewModel.Instance.Model.GLRGBGREENLValue = txtGREENLValue.Text;
                    TestFixtureViewModel.Instance.Model.GLRGBGREENUValue = txtGREENUValue.Text;
                    TestFixtureViewModel.Instance.Model.GreenYMinValues = txtGreenYMinValues.Text;
                    TestFixtureViewModel.Instance.Model.GreenYMaxValue = txtGreenYMaxValues.Text;
                    TestFixtureViewModel.Instance.Model.GreenMicroWattValue = txtGreenMicroWattValueLower.Text;
                    TestFixtureViewModel.Instance.Model.GreenMicroWattValueUpper = txtGreenMicroWattValueUpper.Text;

                    //LED Temperature Derating                                                                        
                    TestFixtureViewModel.Instance.Model.LedTempDeratingWhiteValue = txtLedTempDeratingWhiteValue.Text;
                    TestFixtureViewModel.Instance.Model.LedTempDeratingRedValue = txtLedTempDeratingRedValue.Text;
                    TestFixtureViewModel.Instance.Model.LedTempDeratingGreenValue = txtLedTempDeratingGreenValue.Text;
                    TestFixtureViewModel.Instance.Model.LedTempDeratingBlueValue = txtLedTempDeratingBlueValue.Text;

                    //Fixture info
                    TestFixtureViewModel.Instance.Model.FixtureId = txtFixtureId.Text;
                    TestFixtureViewModel.Instance.Model.CommPortNUmber = txtCommPortNumber.Text;

                    //Intergration Time
                    TestFixtureViewModel.Instance.Model.IntegrationTime = txtIntegrationTime.Text;
                    TestFixtureViewModel.Instance.Model.BlueIntegrationTime = txtBlueIntegrationTime.Text;
                    TestFixtureViewModel.Instance.Model.RedIntegrationTime = txtRedIntegrationTime.Text;
                    TestFixtureViewModel.Instance.Model.GreenIntegrationTime = txtGreenIntegrationTime.Text;
                    TestFixtureViewModel.Instance.Model.WhiteIntegrationTime = txtWhiteIntegrationTime.Text;
                    TestFixtureViewModel.Instance.Model.BlendedWhiteIntegrationTime = txtBlendedWhiteIntegrationTime.Text;
                    TestFixtureViewModel.Instance.Model.MagendaIntegrationTime = txtMagendaIntegrationTime.Text;

                    //FTP Settings
                    TestFixtureViewModel.Instance.Model.FtpOffMin = pagevm.FTPOffMin = txtFtpOffMin.Text;
                    TestFixtureViewModel.Instance.Model.FtpOffMax = pagevm.FTPOffMax = txtFtpOffMax.Text;
                    TestFixtureViewModel.Instance.Model.FtpOnMin = pagevm.FTPOnMin = txtFtpOnMin.Text;
                    TestFixtureViewModel.Instance.Model.FtpOnMax = pagevm.FTPOnMax = txtFtpOnMax.Text;

                    //IP Address Range
                    TestFixtureViewModel.Instance.Model.IpAddressRange = txtIPAddressRange.Text;

                    //BootSequenceTime
                    TestFixtureViewModel.Instance.Model.BootSequenceTime = txtBootSequenceTime.Text;
                }
                else
                {
                    WriteToLog("SetConfigurationSettings: pagevm is null...", ApplicationConstants.TraceLogType.Warning);
                }
            }
            catch (Exception e)
            {
                WriteToLog("SetConfigurationSettings: " + e.Message, ApplicationConstants.TraceLogType.Error);
            }
        }

        private void ReadAllColors()
        {
            foreach (TestFixtureProject.Common.ApplicationConstants.SpectrumColors spectrumColor in Enum.GetValues(typeof(TestFixtureProject.Common.ApplicationConstants.SpectrumColors)))
            {
                if (spectrumColor != Common.ApplicationConstants.SpectrumColors.Dark)
                {
                    ExecuteSpectrumReading(spectrumColor);
                }
            }
        }

        private void ExecuteSpectrumReading(TestFixtureProject.Common.ApplicationConstants.SpectrumColors spectrumColor)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (TestFixtureSocketComm._IsIpAddressFound)
            {
                if (TestFixtureViewModel.Instance != null)
                {
                    TestFixtureViewModel.Instance.CommandToTurnOffLeds();

                    if (testFixtureReadSpectrometer == null)
                        testFixtureReadSpectrometer = new TestFixtureReadSpectrometer();

                    string msg = testFixtureReadSpectrometer.ReadFileContaints(CalibrationFilePath);

                    if (msg.Equals("NoErrors"))
                    {
                        if (testFixtureReadSpectrometer.GetSpectrum(spectrumColor))
                        {
                            testFixtureReadSpectrometer.CalculateColor(spectrumColor);
                        }
                        else
                            WriteToLog("GetSpectrum execution failaure: " + spectrumColor.ToString(), ApplicationConstants.TraceLogType.Error);
                    }
                    else
                        WriteToLog(msg, ApplicationConstants.TraceLogType.Warning);
                }
            }
            else
            {
                MessageBox.Show("txtIpAddress is null. Please enter an valid IP address...", "IP Address Ping", MessageBoxButtons.OK);
                WriteToLog("txtIpAddress is null. Please enter an valid IP address...", ApplicationConstants.TraceLogType.Warning);
            }

            Cursor.Current = Cursors.Default;
        }

        internal void TurnSpectrumLedOn(Common.ApplicationConstants.SpectrumColors spectrumColor)
        {
            if (pagevm == null)
                pagevm = new TestFixtureViewModel();

            //Keith Dudley Added Fixed Light Switch Control
            if (spectrumColor == Common.ApplicationConstants.SpectrumColors.Red)
            {
                pagevm.TurnOnRedLightCommand();

                //tFixedRed.CheckState = CheckState.Checked;
                //tFixedGreen.CheckState = CheckState.Unchecked;
                //tFixedBlue.CheckState = CheckState.Unchecked;
                //tFixedWhite.CheckState = CheckState.Unchecked;

            }
            else if (spectrumColor == Common.ApplicationConstants.SpectrumColors.Green)
            {
                pagevm.TurnOnGreenLightCommand();

                //tFixedRed.CheckState = CheckState.Unchecked;
                //tFixedGreen.CheckState = CheckState.Checked;
                //tFixedBlue.CheckState = CheckState.Unchecked;
                //tFixedWhite.CheckState = CheckState.Unchecked;

            }
            else if (spectrumColor == Common.ApplicationConstants.SpectrumColors.Blue)
            {
                pagevm.TurnOnBlueLightCommand();

                //tFixedRed.CheckState = CheckState.Unchecked;
                //tFixedGreen.CheckState = CheckState.Unchecked;
                //tFixedBlue.CheckState = CheckState.Checked;
                //tFixedWhite.CheckState = CheckState.Unchecked;

            }
            else if (spectrumColor == Common.ApplicationConstants.SpectrumColors.White)
            {
                pagevm.TurnOnWhiteLightCommand();

                //tFixedRed.CheckState = CheckState.Unchecked;
                //tFixedGreen.CheckState = CheckState.Unchecked;
                //tFixedBlue.CheckState = CheckState.Unchecked;
                //tFixedWhite.CheckState = CheckState.Checked;
            }

            else if (spectrumColor == Common.ApplicationConstants.SpectrumColors.BlendedWhite)
            {
                pagevm.TurnOnBlendedWhiteLightCommand();

                //tFixedRed.CheckState = CheckState.Unchecked;
                //tFixedGreen.CheckState = CheckState.Unchecked;
                //tFixedBlue.CheckState = CheckState.Unchecked;
                //tFixedWhite.CheckState = CheckState.Checked;
            }

            else if (spectrumColor == Common.ApplicationConstants.SpectrumColors.Magenda)
            {
                pagevm.TurnOnMagendaLightCommand();

                //tFixedRed.CheckState = CheckState.Unchecked;
                //tFixedGreen.CheckState = CheckState.Unchecked;
                //tFixedBlue.CheckState = CheckState.Unchecked;
                //tFixedWhite.CheckState = CheckState.Checked;
            }



            //tFixedRed.CheckState = CheckState.Unchecked;
            //tFixedGreen.CheckState = CheckState.Unchecked;
            //tFixedBlue.CheckState = CheckState.Unchecked;
            //tFixedWhite.CheckState = CheckState.Unchecked;
        }

        public bool FindSpectrometers()
        {
            bool IsSpectrometer = false;

            try
            {
                if (wrapper == null)
                    wrapper = new OmniDriver.NETWrapper();

                wrapper.openAllSpectrometers();
                int numberOfSpectrometers = wrapper.getNumberOfSpectrometersFound();
                WriteToLog(string.Format("Found {0} spectrometers: ", numberOfSpectrometers), ApplicationConstants.TraceLogType.Information);
                lblSpectrometer.Text = string.Format("Found {0} spectrometers", numberOfSpectrometers);

                if (numberOfSpectrometers < 1)
                {
                    IsSpectrometer = false;
                    WriteToLog("WARNING: no spectrometers found", ApplicationConstants.TraceLogType.Warning);
                    lblSpectrometer.Text = "Not Available";
                }
                else
                    IsSpectrometer = true;

                return IsSpectrometer;
            }
            catch (Exception ex)
            {
                WriteToLog("FindSpectrometers: " + ex.Message, ApplicationConstants.TraceLogType.Error);

                return IsSpectrometer;
            }
        }

        public void ReStartThread()
        {
            pagevm = new TestFixtureViewModel();

            //pagevm.Startthread.Start();

            //backgroundWorker1.Dispose();

            //if (TestFixtureViewModel.Instance != null)
            //TestFixtureViewModel.Instance.PushButton();

            TestFixtureViewModel.isTestExecutionFailure = false;

            //SetSerialNumberTextbox("update serial number");
            //SetModelNumberTextbox("update model number");
            //SetVersionNumberTextbox("update version number");
            //SetVoltageTextbox("update voltage number");
        }

        void CameraMainWindowLoaded()
        {
            FilterInfoCollection infocoll = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            try
            {
                captdevice = new VideoCaptureDevice(infocoll[0].MonikerString);

                //set camera properties
                captdevice.SetCameraProperty(CameraControlProperty.Exposure, 100, CameraControlFlags.Manual);
                captdevice.SetCameraProperty(CameraControlProperty.Focus, 100, CameraControlFlags.Manual);
                captdevice.SetCameraProperty(CameraControlProperty.Zoom, 50, CameraControlFlags.Manual);

                captdevice.NewFrame += new AForge.Video.NewFrameEventHandler(TakeNewImage);

                bool flag = captdevice.ProvideSnapshots;
                captdevice.Start();

            }
            catch (Exception e)
            {
                MessageBox.Show("TestFixtureUI:CameraMainWindowLoadedMessage:" + "No Camera's,Please connect/Switch On Camera");
                WriteToLog("CameraMainWindowLoaded: " + e.Message, ApplicationConstants.TraceLogType.Error);
                WriteToLog("TestFixtureUI:CameraMainWindowLoadedMessage:" + "No Camera's,Please connect/Switch On Camera", ApplicationConstants.TraceLogType.Error);
            }


        }

        private bool imageconvertcallback()
        {
            return false;
        }

        private bool FindCalibrationFile()
        {
            IsCalibrationFileFound = false;

            try
            {
                if (wrapper == null)
                    wrapper = new OmniDriver.NETWrapper();

                if (wrapper.openAllSpectrometers() > 0)
                {
                    CalibrationFilePath = TestFixtureConstants.GetSpectrometerCalibratedFile()
                                            + wrapper.getSerialNumber(0)
                                            + ".csv";

                    //CalibrationFilePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                    //                       + "\\"
                    //                       + "Spectrometer-Calibration_"
                    //                       + wrapper.getSerialNumber(0)
                    //                       + ".csv";

                    WriteToLog(CalibrationFilePath, ApplicationConstants.TraceLogType.Information);

                    if (File.Exists(CalibrationFilePath))
                        IsCalibrationFileFound = true;
                    else
                        IsCalibrationFileFound = false;
                }

                return IsCalibrationFileFound;
            }
            catch (Exception ex)
            {
                WriteToLog("FindCalibrationFile: " + ex.Message, ApplicationConstants.TraceLogType.Error);

                return IsCalibrationFileFound;
            }
        }

        public void LoadImageFromCamera(object sender, EventArgs args)
        {
            CameraMainWindowLoaded();

        }

        private void TakeNewImage(object sender, NewFrameEventArgs args)
        {
            try
            {
                bitimg = new BitmapImage();
                bitimg.BeginInit();

                System.Drawing.Image captimg = (Bitmap)args.Frame.Clone();
                MemoryStream ms = new MemoryStream();

                captimg.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                bitimg.StreamSource = ms;
                bitimg.EndInit();
                bitimg.Freeze();
                //Dispatcher.BeginInvoke(new ThreadStart(delegate
                //{
                //    //Console.WriteLine("");
                //    imgPhoto1.Source = bitimg;
                //}), DispatcherPriority.Render, null);
            }
            catch (Exception ex)
            {
                WriteToLog("TakeNewImage: " + ex.Message, ApplicationConstants.TraceLogType.Error);
            }
        }

        public void CaptureImageFromCamera(object sender, EventArgs args)
        {
            try
            {
                if (captdevice != null)
                {
                    captdevice.SignalToStop();
                    captdevice.WaitForStop();
                    captdevice = null;
                    //  imgPhoto1.Source = imgPhoto1.Source;
                }
            }
            catch (Exception ex)
            {
                WriteToLog("CaptureImageFromCamera: " + ex.Message, ApplicationConstants.TraceLogType.Error);
            }
        }
        #endregion


        #region Event Handlers
        internal void tcHostTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {

            //uc.Dock = DockStyle.Fill;
            //frmTestFixtureMetroUI.Instance.MetroContainer.Controls.Add(uc);

            tc = (MetroFramework.Controls.MetroTabControl)sender;

            //System.Windows.Controls.TabItem item = tc.ite as System.Windows.Controls.TabItem;
            //MetroFramework.Controls.TabItem item = tc.SelectedItem as TabItem;
            int index = tc.SelectedIndex;

            pagelvm = new TestFixtureLoginViewModel();

            //ucLogin p1 = new ucLogin();
            pagelvm.SelectedIndex = index;
            tc.SelectedIndex = index;

            if (sender is MetroFramework.Controls.MetroTabControl && !_accessGranted)
            {
                if (tc.SelectedIndex != 0)
                {
                    if (!_accessGranted && !_partialAccess)
                    {
                        //if (frmTestFixtureMetroUI.Instance != null)
                        //    frmTestFixtureMetroUI.Instance.MetroContainer.Controls["ucLogin"].BringToFront();

                        //if (pagelvm.AccessGranted)
                        //{
                        //    _accessGranted = true;
                        //    _partialAccess = false;
                        //}
                        //else if (pagelvm.PartialAccessGranted)
                        //{
                        //    _partialAccess = true;
                        //    _accessGranted = false;
                        //}
                        //else
                        //{
                        //    tc.SelectedIndexChanged -= tcHostTabControl_SelectedIndexChanged;
                        //    tc.SelectedIndex = 0;
                        //    tc.SelectedIndexChanged += tcHostTabControl_SelectedIndexChanged;
                        //}
                    }
                }
            }

            if (_partialAccess)
            {
                string tabname = tc.SelectedTab.Name;

                //if (tabname.Equals("Diagnostics"))
                //    item.IsSelected = true;
                //if (tabname.Equals("Projector Diagnostics"))
                //    item.IsSelected = true;
                if (tabname.Equals("Settings"))
                {
                    //if (frmTestFixtureMetroUI.Instance != null)
                    //    frmTestFixtureMetroUI.Instance.MetroContainer.Controls["ucLogin"].BringToFront();

                    //if (frmTestFixtureMetroUI.Instance != null)
                    //    frmTestFixtureMetroUI.Instance.MetroContainer.Controls["ucLogin"].BringToFront();

                    //if (pagelvm.AccessGranted)
                    //{
                    //    _accessGranted = true;
                    //    _partialAccess = false;
                    //    tc.SelectedIndex = index;
                    //}
                    //else
                    //{
                    //    tc.SelectedIndexChanged -= tcHostTabControl_SelectedIndexChanged;
                    //    tc.SelectedIndex = 0;
                    //    tc.SelectedIndexChanged += tcHostTabControl_SelectedIndexChanged;
                    //}
                }
                if (tabname.Equals("Calibration"))
                {
                    //if (frmTestFixtureMetroUI.Instance != null)
                    //    frmTestFixtureMetroUI.Instance.MetroContainer.Controls["ucLogin"].BringToFront();

                    //if (pagelvm.AccessGranted)
                    //{
                    //    _accessGranted = true;
                    //    _partialAccess = false;
                    //    tc.SelectedIndex = index;
                    //}
                    //else
                    //{
                    //    tc.SelectedIndexChanged -= tcHostTabControl_SelectedIndexChanged;
                    //    tc.SelectedIndex = 0;
                    //    tc.SelectedIndexChanged += tcHostTabControl_SelectedIndexChanged;
                    //}
                }
            }
        }

        private void btnStartCamera_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            TestFixtureViewModel.Instance.StartCameraToCaptureImage(e);

            Cursor.Current = Cursors.Default;
        }

        private void btCaptureImage_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            TestFixtureViewModel.Instance.CaptureImageFromCamera(e);

            Cursor.Current = Cursors.Default;
        }

        private void btnProcessImage_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            TestFixtureViewModel.Instance.DummyTesting(e);

            Cursor.Current = Cursors.Default;
        }

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            TestFixtureViewModel.Instance.BrowseImageToLoad(e);

            Cursor.Current = Cursors.Default;
        }

        private void btnProcessImg_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            TestFixtureViewModel.Instance.ProcessImageForData(e);

            Cursor.Current = Cursors.Default;
        }

        private void btnSaveImgDetails_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                TestFixtureViewModel.Instance.SaveImgDetailsToFile(e);

                WriteToLog("Save image details successfully...", ApplicationConstants.TraceLogType.Information);
            }
            catch (Exception ex)
            {
                WriteToLog("btnSaveImgDetails_Click: " + ex.Message, ApplicationConstants.TraceLogType.Error);
            }

            Cursor.Current = Cursors.Default;
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                SetConfigurationSettings();

                TestFixtureViewModel.Instance.SaveDetailsToFile(e);

                WriteToLog("Save configuration settings successfully...", ApplicationConstants.TraceLogType.Information);
            }
            catch (Exception ex)
            {
                WriteToLog("btnSaveSettings_Click: " + ex.Message, ApplicationConstants.TraceLogType.Error);
            }
            Cursor.Current = Cursors.Default;
        }

        private void btn120VAC_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            TestFixtureViewModel.Instance.Start120ACVRelayOn();

            Cursor.Current = Cursors.Default;
        }

        private void btn15VAC_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            TestFixtureViewModel.Instance.TurnOn15ACVRelay();

            Cursor.Current = Cursors.Default;
        }

        private void btnHallEffectSolenoid_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            TestFixtureViewModel.Instance.PairSolenoidOn(e);

            Cursor.Current = Cursors.Default;
        }

        private void btnWallAdapterSolenoid_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            TestFixtureViewModel.Instance.PCBLoadSelOn();

            Cursor.Current = Cursors.Default;
        }

        private void btnResetBoard_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            TestFixtureViewModel.Instance.ResetDaqBoardPort();

            Cursor.Current = Cursors.Default;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            SetStatusIndicatorVisibleProperty(false);

            //if (!pagevm.Startthread.IsAlive)
            //     pagevm.Startthread.Start();
            //if (!backgroundWorker1.IsBusy)
            //    backgroundWorker1.RunWorkerAsync(BW_OPERATIONS.OPERATION_START_TEST_SEQUENCE);

            if (!pagevm.Startthread.IsAlive)
            {
                pagevm = new TestFixtureViewModel();

                //SetErrorMessageDisplayTextBox("Press Green Button to Start...");
            }

            Cursor.Current = Cursors.Default;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            //TestFixtureViewModel.Instance.StopExecuting();
            if (pagevm == null)
                pagevm = new TestFixtureViewModel();

            pagevm.StopExecuting();

            pagevm.ResetDaqBoardPort();

            Cursor.Current = Cursors.Default;
        }

        private void btnTopRight_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            //rbTopRight.Checked = TestFixtureViewModel.Instance.MoveMirrorToTopRightPosition(e);
            ResetRadioButtons();

            rbTopRight.Checked = TestFixtureViewModel.Instance.MoveMirrorPosition(Common.ApplicationConstants.MirrorPosition.TopRight);

            //rbTopRight.Checked = true;

            Cursor.Current = Cursors.Default;
        }

        private void btnTopLeft_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            ResetRadioButtons();

            //rbTopLeft.Checked = TestFixtureViewModel.Instance.MoveMirrorToTopLeftPosition(e);
            rbTopLeft.Checked = TestFixtureViewModel.Instance.MoveMirrorPosition(Common.ApplicationConstants.MirrorPosition.TopLeft);

            //rbTopLeft.Checked = true;

            Cursor.Current = Cursors.Default;
        }

        private void btnBottomRight_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            ResetRadioButtons();

            //rbBottomRight.Checked = TestFixtureViewModel.Instance.MoveMirrorToBottomRightPosition(e);

            rbBottomRight.Checked = TestFixtureViewModel.Instance.MoveMirrorPosition(Common.ApplicationConstants.MirrorPosition.BottomRight);

            //rbBottomRight.Checked = true;

            Cursor.Current = Cursors.Default;
        }

        private void btnBottomLeft_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            ResetRadioButtons();
            //rbBottomLeft.Checked = TestFixtureViewModel.Instance.MoveMirrorToBottomLeftPosition(e);

            rbBottomLeft.Checked = TestFixtureViewModel.Instance.MoveMirrorPosition(Common.ApplicationConstants.MirrorPosition.BottomLeft);

            //rbBottomLeft.Checked = true;

            Cursor.Current = Cursors.Default;
        }

        private void btnHomeScreen_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            rbHomeScreen.Checked = TestFixtureViewModel.Instance.MoveMirrorToHomeScreenPosition(e);

            rbHomeScreen.Checked = TestFixtureViewModel.Instance.MoveMirrorPosition(Common.ApplicationConstants.MirrorPosition.Home);

            Cursor.Current = Cursors.Default;
        }

        private void btnIsStartThreadAlive_Click(object sender, EventArgs e)
        {
            if (TestFixtureViewModel.Instance.Startthread.IsAlive)
                MessageBox.Show("I'm Alive...", "Start Thread", MessageBoxButtons.OK);
        }

        private void btnIsStopThreadAlive_Click(object sender, EventArgs e)
        {
            if (TestFixtureViewModel.Instance.stopthread.IsAlive)
                MessageBox.Show("I'm Alive...", "Stop Thread", MessageBoxButtons.OK);
        }

        private void txtTraceLog_TextChanged(object sender, EventArgs e)
        {
            // txtTraceLog.SelectionStart = txtTraceLog.Text.Length;
            //txtTraceLog.ScrollToCaret();
        }

        private void btnReadVoltage_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (TestFixtureViewModel.Instance != null)
                TestFixtureViewModel.Instance.ReadImpedanceVolt(e);

            Cursor.Current = Cursors.Default;

            //backgroundWorker1.RunWorkerAsync(BW_OPERATIONS.START_READ_VOLTAGE);
        }

        private void btnReadTemperature_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (TestFixtureViewModel.Instance != null)
                TestFixtureViewModel.Instance.ReadTempSensorValue(e);

            Cursor.Current = Cursors.Default;
            //backgroundWorker1.RunWorkerAsync(BW_OPERATIONS.START_READ_TEMPERATURE);
        }

        private void btnFixedRed_Click(object sender, EventArgs e)
        {
            if (TestFixtureViewModel.Instance != null)
                TestFixtureViewModel.Instance.GetRedValueDetails(e);
        }

        private void btnFixedGreen_Click(object sender, EventArgs e)
        {
            if (TestFixtureViewModel.Instance != null)
                TestFixtureViewModel.Instance.ReadGreenValuesDetailsFromSpectrum(e);
        }

        private void btnFixedBlue_Click(object sender, EventArgs e)
        {
            if (TestFixtureViewModel.Instance != null)
                TestFixtureViewModel.Instance.ReadBlueXYDetailsFromSpectrum(e);
        }

        private void btnFixedWhite_Click(object sender, EventArgs e)
        {
            if (TestFixtureViewModel.Instance != null)
                TestFixtureViewModel.Instance.ReadWhiteDetailsFromSpectrum(e);
        }

        private void btnPairingSequence_Click(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync(BW_OPERATIONS.OPERATION_PAIRING_SEQUENCE);
        }

        private void btnHallEffectSolenoidOff_Click(object sender, EventArgs e)
        {
            TestFixtureViewModel.Instance.PairSolenoidOff(e);
        }

        private void btnGetIpAddress_Click(object sender, EventArgs e)
        {
            TestFixtureSocketComm._IsIpAddressFound = false;

            if (!backgroundWorker1.IsBusy)
            {
                if (cbDirectIpAdrressPing.Checked)
                {
                    if (!string.IsNullOrEmpty(txtIpAddress.Text))
                    {
                        backgroundWorker1.RunWorkerAsync(BW_OPERATIONS.OPERATION_DIRECT_IP_ADDRESS_PING);
                    }
                    else
                    {
                        MessageBox.Show("txtIpAddress is null. Enter ip address!", "IP Address Ping", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    backgroundWorker1.RunWorkerAsync(BW_OPERATIONS.OPERATION_FIND_IP_ADDRESS);
                }
            }
        }

        private void txtIpAddress_TextChanged(object sender, EventArgs e)
        {
            if (TestFixtureViewModel.Instance != null)
            {
                if (!string.IsNullOrEmpty(txtIpAddress.Text))
                    TestFixtureViewModel.Instance.serverIpAddess = txtIpAddress.Text;
                else
                    TestFixtureViewModel.Instance.serverIpAddess = string.Empty;
            }
        }

        private void cbIsCheckDelay_CheckedChanged(object sender, EventArgs e)
        {
            TestFixtureProject.DataAccess.TestFixtureSocketComm._IsDelayCheck = cbIsCheckDelay.Checked;
        }

        private void btnTurnLedsOff_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (TestFixtureViewModel.Instance != null)
            {
                TestFixtureViewModel.Instance.CommandToTurnOffLeds();

                //tFixedRed.CheckState = CheckState.Unchecked;
                //tFixedGreen.CheckState = CheckState.Unchecked;
                //tFixedBlue.CheckState = CheckState.Unchecked;
                //tFixedWhite.CheckState = CheckState.Unchecked;
            }

            Cursor.Current = Cursors.Default;
        }

        private void rbFixedRed_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (!string.IsNullOrEmpty(txtIpAddress.Text))
            {
                if (TestFixtureViewModel.Instance != null)
                {
                    //if (rbFixedRed.Checked)
                    //{
                    TestFixtureViewModel.Instance.CommandToTurnOffLeds();

                    TestFixtureViewModel.Instance.GetRedValueDetails(e);
                    // }
                }
            }
            else
                MessageBox.Show("IP Address is null. Please enter valid a IP Address.", "IP ADDRESS", MessageBoxButtons.OK);

            Cursor.Current = Cursors.Default;
        }

        private void rbFixedGreen_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (!string.IsNullOrEmpty(txtIpAddress.Text))
            {
                if (TestFixtureViewModel.Instance != null)
                {
                    TestFixtureViewModel.Instance.CommandToTurnOffLeds();

                    TestFixtureViewModel.Instance.ReadGreenValuesDetailsFromSpectrum(e);
                }
            }
            else
                MessageBox.Show("IP Address is null. Please enter valid a IP Address.", "IP ADDRESS", MessageBoxButtons.OK);


            Cursor.Current = Cursors.Default;
        }

        private void rbFixedBlue_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (!string.IsNullOrEmpty(txtIpAddress.Text))
            {
                if (TestFixtureViewModel.Instance != null)
                {
                    TestFixtureViewModel.Instance.CommandToTurnOffLeds();

                    TestFixtureViewModel.Instance.ReadBlueXYDetailsFromSpectrum(e);
                }
            }
            else
                MessageBox.Show("IP Address is null. Please enter valid a IP Address.", "IP ADDRESS", MessageBoxButtons.OK);


            Cursor.Current = Cursors.Default;
        }

        private void rbFixedWhite_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (!string.IsNullOrEmpty(txtIpAddress.Text))
            {
                if (TestFixtureViewModel.Instance != null)
                {
                    TestFixtureViewModel.Instance.CommandToTurnOffLeds();

                    TestFixtureViewModel.Instance.ReadWhiteDetailsFromSpectrum(e);
                }
            }
            else
                MessageBox.Show("IP Address is null. Please enter valid a IP Address.", "IP ADDRESS", MessageBoxButtons.OK);


            Cursor.Current = Cursors.Default;
        }

        private void rbLedOff_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (!string.IsNullOrEmpty(txtIpAddress.Text))
            {
                if (TestFixtureViewModel.Instance != null)
                {
                    TestFixtureViewModel.Instance.CommandToTurnOffLeds();
                }
            }
            else
                MessageBox.Show("IP Address is null. Please enter valid a IP Address.", "IP ADDRESS", MessageBoxButtons.OK);


            Cursor.Current = Cursors.Default;
        }

        private void btnWallAdapterSolenoidOff_Click(object sender, EventArgs e)
        {
            TestFixtureViewModel.Instance.PCBLoadSelOff(e);
        }

        private void tFixedRed_Click(object sender, EventArgs e)
        {

            //tFixedRed.CheckState = CheckState.Unchecked;

            //tFixedGreen.CheckState = CheckState.Unchecked;
            //tFixedBlue.CheckState = CheckState.Unchecked;
            //tFixedWhite.CheckState = CheckState.Unchecked;

            Cursor.Current = Cursors.WaitCursor;

            if (!string.IsNullOrEmpty(txtIpAddress.Text))
            {
                if (TestFixtureViewModel.Instance != null)
                {
                    TestFixtureViewModel.Instance.CommandToTurnOffLeds();

                    //if (tFixedRed.CheckState == CheckState.Checked)
                    //{
                    //    TestFixtureViewModel.Instance.GetRedValueDetails(e);
                    //}
                }
            }
            else
            {
                //tFixedRed.CheckState = CheckState.Unchecked;
                MessageBox.Show("txtIpAddress is null. Please enter an valid IP address...", "IP Address Ping", MessageBoxButtons.OK);
            }

            Cursor.Current = Cursors.Default;
        }

        private void tFixedGreen_Click(object sender, EventArgs e)
        {
            //tFixedRed.CheckState = CheckState.Unchecked;
            ////tFixedGreen.CheckState = CheckState.Unchecked;
            //tFixedBlue.CheckState = CheckState.Unchecked;
            //tFixedWhite.CheckState = CheckState.Unchecked;

            Cursor.Current = Cursors.WaitCursor;

            if (!string.IsNullOrEmpty(txtIpAddress.Text))
            {
                if (TestFixtureViewModel.Instance != null)
                {
                    TestFixtureViewModel.Instance.CommandToTurnOffLeds();

                    //if (tFixedGreen.CheckState == CheckState.Checked)
                    //{
                    //    TestFixtureViewModel.Instance.ReadGreenValuesDetailsFromSpectrum(e);
                    //}
                }
            }
            else
            {
                //tFixedGreen.CheckState = CheckState.Unchecked;
                MessageBox.Show("txtIpAddress is null. Please enter an valid IP address...", "IP Address Ping", MessageBoxButtons.OK);
            }

            Cursor.Current = Cursors.Default;
        }

        private void tFixedBlue_Click(object sender, EventArgs e)
        {
            //tFixedRed.CheckState = CheckState.Unchecked;
            //tFixedGreen.CheckState = CheckState.Unchecked;
            ////tFixedBlue.CheckState = CheckState.Unchecked;
            //tFixedWhite.CheckState = CheckState.Unchecked;

            Cursor.Current = Cursors.WaitCursor;

            if (!string.IsNullOrEmpty(txtIpAddress.Text))
            {
                if (TestFixtureViewModel.Instance != null)
                {
                    TestFixtureViewModel.Instance.CommandToTurnOffLeds();

                    //if (tFixedBlue.CheckState == CheckState.Checked)
                    //{
                    //    TestFixtureViewModel.Instance.ReadBlueXYDetailsFromSpectrum(e);
                    //}
                }
            }
            else
            {
                //tFixedBlue.CheckState = CheckState.Unchecked;
                MessageBox.Show("txtIpAddress is null. Please enter an valid IP address...", "IP Address Ping", MessageBoxButtons.OK);
            }

            Cursor.Current = Cursors.Default;
        }

        private void tFixedWhite_Click(object sender, EventArgs e)
        {
            //tFixedRed.CheckState = CheckState.Unchecked;
            //tFixedGreen.CheckState = CheckState.Unchecked;
            //tFixedBlue.CheckState = CheckState.Unchecked;
            ////tFixedWhite.CheckState = CheckState.Unchecked;

            Cursor.Current = Cursors.WaitCursor;

            if (!string.IsNullOrEmpty(txtIpAddress.Text))
            {
                if (TestFixtureViewModel.Instance != null)
                {
                    TestFixtureViewModel.Instance.CommandToTurnOffLeds();

                    //if (tFixedWhite.CheckState == CheckState.Checked)
                    //{
                    //    TestFixtureViewModel.Instance.ReadWhiteDetailsFromSpectrum(e);
                    //}
                }
            }
            else
            {
                //tFixedWhite.CheckState = CheckState.Unchecked;
                MessageBox.Show("txtIpAddress is null. Please enter an valid IP address...", "IP Address Direct", MessageBoxButtons.OK);
            }

            Cursor.Current = Cursors.Default;
        }

        private void btnStopIpSearch_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
                backgroundWorker1.CancelAsync();
        }

        private void btnStartImageShow_Click(object sender, EventArgs e)
        {
            //Cursor.Current = Cursors.WaitCursor;

            bool flag = false;

            try
            {
                if (pagevm != null)
                {
                    WriteToLog("Starting fileUpload...", ApplicationConstants.TraceLogType.Information);

                    bool upload = TestFixtureSocketComm.fileUpload();

                    pagevm.SetScokectFlagsToFalse();

                    WriteToLog("Starting image show for mirror...", ApplicationConstants.TraceLogType.Information);

                    flag = pagevm.StartImageShowForMirror();
                    if (!flag)
                    {
                        pagevm.UpdateTestResultPassFailStatus(false, "Start Image Show Failed");
                        pagevm.ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "btnStartImageShow_Click", MessageBoxButtons.OK);
                WriteToLog("btnStartImageShow_Click: " + ex.Message, ApplicationConstants.TraceLogType.Error);
            }
            finally
            {
                //  Cursor.Current = Cursors.Default;
            }

            return;
        }

        private void btnDeleteImageUpload_Click(object sender, EventArgs e)
        {
            //Cursor.Current = Cursors.WaitCursor;

            bool flag = false;

            try
            {
                if (pagevm != null)
                {
                    WriteToLog("Deleting upload image file...", ApplicationConstants.TraceLogType.Information);

                    bool upload = TestFixtureSocketComm.DeleteUploadedFile("image");

                    //pagevm.SetScokectFlagsToFalse();
                    //flag = pagevm.StartImageShowForMirror();
                    //if (!flag)
                    //{
                    //    pagevm.UpdateTestResultPassFailStatus(false, "Start Image Show Failed");
                    //    pagevm.ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    //    return;
                    //}
                }
            }
            catch (Exception ex)
            {
                WriteToLog("btnDeleteImageUpload_Click: " + ex.Message, ApplicationConstants.TraceLogType.Error);
            }
            finally
            {
                //  Cursor.Current = Cursors.Default;
            }

            return;
        }

        private void btnStopImageShow_Click(object sender, EventArgs e)
        {
            //Cursor.Current = Cursors.WaitCursor;

            bool flag = false;

            try
            {
                if (pagevm != null)
                {

                    WriteToLog("Sending command to stop image show for mirror...", ApplicationConstants.TraceLogType.Information);

                    bool upload = pagevm.SendCommandToStopImageShow();

                    //pagevm.SetScokectFlagsToFalse();
                    //flag = pagevm.StartImageShowForMirror();
                    //if (!flag)
                    //{
                    //    pagevm.UpdateTestResultPassFailStatus(false, "Start Image Show Failed");
                    //    pagevm.ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    //    return;
                    //}
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "btnStopImageShow_Click", MessageBoxButtons.OK);
                WriteToLog("btnStopImageShow_Click: " + ex.Message, ApplicationConstants.TraceLogType.Error);
            }
            finally
            {
                //  Cursor.Current = Cursors.Default;
            }

            return;
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            ClearLogs();
        }

        internal void ClearLogs()
        {
            TestFixtureViewModel.Instance.InformationStatusMessage = string.Empty;
            TestFixtureViewModel.Instance.WarningStatusMessage = string.Empty;
            TestFixtureViewModel.Instance.ErrorStatusMessage = string.Empty;

            //reset counter
            informationMessageCounter = 0;
            warningMessageCounter = 0;
            errorMessageCounter = 0;

            // txtTraceLog.Text = string.Empty;
            counter = 0;
            //lvTraceLog.Items.Clear();

            txtInformation.Clear();
            txtWarnings.Clear();
            txtErrors.Clear();

            tabControl1.TabPages["tpInformation"].Invoke((MethodInvoker)delegate { tabControl1.TabPages["tpInformation"].Text = "Information (0)"; });
            tabControl1.TabPages["tpWarnings"].Invoke((MethodInvoker)delegate { tabControl1.TabPages["tpWarnings"].Text = "Warnings (0)"; });
            tabControl1.TabPages["tpErrors"].Invoke((MethodInvoker)delegate { tabControl1.TabPages["tpErrors"].Text = "Errors (0)"; });
        }

        private void btnReadCalibrationFile_Click(object sender, EventArgs e)
        {

            Cursor.Current = Cursors.WaitCursor;

            if (TestFixtureViewModel.Instance.BrowseToCalibrationFile(e))
            {
                TestFixtureReadSpectrometer calibration = new TestFixtureReadSpectrometer();

                if (pagevm != null)
                {
                    calibration.ReadCalibratedFile(pagevm.CalibrationFileName);
                    //calibration.Get
                }
                else
                    WriteToLog("TestFixtureViewModel object is null", ApplicationConstants.TraceLogType.Warning);

            }

            Cursor.Current = Cursors.Default;
        }

        private void tFixedRed_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnAcquireSpectrum_Click(object sender, EventArgs e)
        {
            if (pagevm != null)
                pagevm.AcquireDarkSpectrum();
        }

        private void btnGetRedSpectrum_Click(object sender, EventArgs e)
        {
            if (IsCalibrationFileFound)
            {
                ExecuteSpectrumReading(TestFixtureProject.Common.ApplicationConstants.SpectrumColors.Red);
            }
            else
            {
                if (TestFixtureViewModel.Instance.BrowseToCalibrationFile(e))
                {
                    ExecuteSpectrumReading(TestFixtureProject.Common.ApplicationConstants.SpectrumColors.Red);
                }
            }
        }

        private void btnGetGrrenSpectrum_Click(object sender, EventArgs e)
        {
            if (IsCalibrationFileFound)
            {
                ExecuteSpectrumReading(TestFixtureProject.Common.ApplicationConstants.SpectrumColors.Green);
            }
            else
            {
                if (TestFixtureViewModel.Instance.BrowseToCalibrationFile(e))
                {
                    ExecuteSpectrumReading(TestFixtureProject.Common.ApplicationConstants.SpectrumColors.Green);
                }
            }
        }

        private void btnGetBlueSpectrum_Click(object sender, EventArgs e)
        {
            if (IsCalibrationFileFound)
            {
                ExecuteSpectrumReading(TestFixtureProject.Common.ApplicationConstants.SpectrumColors.Blue);
            }
            else
            {
                if (TestFixtureViewModel.Instance.BrowseToCalibrationFile(e))
                {
                    ExecuteSpectrumReading(TestFixtureProject.Common.ApplicationConstants.SpectrumColors.Blue);
                }
            }
        }

        private void btnGetWhiteSpectrum_Click(object sender, EventArgs e)
        {
            if (IsCalibrationFileFound)
            {
                ExecuteSpectrumReading(TestFixtureProject.Common.ApplicationConstants.SpectrumColors.White);
            }
            else
            {
                if (TestFixtureViewModel.Instance.BrowseToCalibrationFile(e))
                {
                    ExecuteSpectrumReading(TestFixtureProject.Common.ApplicationConstants.SpectrumColors.White);
                }
            }
        }

        private void btnExportSpectrumListView_Click(object sender, EventArgs e)
        {
            //declare new SaveFileDialog + set it's initial properties 
            //try
            //{
            //    SaveFileDialog sfd = new SaveFileDialog
            //    {
            //        Title = "Choose file to save to",
            //        FileName = "example.csv",
            //        Filter = "CSV (*.csv)|*.csv",
            //        FilterIndex = 0,
            //        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            //    };

            //    //show the dialog + display the results in a msgbox unless cancelled 
            //    if (sfd.ShowDialog() == DialogResult.OK)
            //    {

            //        string[] headers = lvSpectrumData.Columns
            //                   .OfType<ColumnHeader>()
            //                   .Select(header => header.Text.Trim())
            //                   .ToArray();

            //        string[][] items = lvSpectrumData.Items
            //                    .OfType<ListViewItem>()
            //                    .Select(lvi => lvi.SubItems
            //                        .OfType<ListViewItem.ListViewSubItem>()
            //                        .Select(si => si.Text).ToArray()).ToArray();

            //        string table = string.Join(",", headers) + Environment.NewLine;
            //        foreach (string[] a in items)
            //        {
            //            //a = a_loopVariable; 
            //            table += string.Join(",", a) + Environment.NewLine;
            //        }
            //        table = table.TrimEnd('\r', '\n');
            //        System.IO.File.WriteAllText(sfd.FileName, table);

            //        if (!string.IsNullOrEmpty(sfd.FileName))
            //        {
            //            Process.Start(sfd.FileName);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "btnExportSpectrumListView_Click PRESS...");
            //    WriteToLog(ex.Message, ApplicationConstants.TraceLogType.Error);
            //}
        }

        private void btnExportTraceLog_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    //declare new SaveFileDialog + set it's initial properties 
            //    SaveFileDialog sfd = new SaveFileDialog
            //    {
            //        Title = "Choose file to save to",
            //        FileName = "example.csv",
            //        Filter = "CSV (*.csv)|*.csv",
            //        FilterIndex = 0,
            //        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            //    };

            //    //show the dialog + display the results in a msgbox unless cancelled 
            //    if (sfd.ShowDialog() == DialogResult.OK)
            //    {

            //        string[] headers = lvTraceLog.Columns
            //                   .OfType<ColumnHeader>()
            //                   .Select(header => header.Text.Trim())
            //                   .ToArray();

            //        string[][] items = lvTraceLog.Items
            //                    .OfType<ListViewItem>()
            //                    .Select(lvi => lvi.SubItems
            //                        .OfType<ListViewItem.ListViewSubItem>()
            //                        .Select(si => si.Text).ToArray()).ToArray();

            //        string table = string.Join(",", headers) + Environment.NewLine;
            //        foreach (string[] a in items)
            //        {
            //            //a = a_loopVariable; 
            //            table += string.Join(",", a) + Environment.NewLine;
            //        }
            //        table = table.TrimEnd('\r', '\n');
            //        System.IO.File.WriteAllText(sfd.FileName, table);


            //        if (!string.IsNullOrEmpty(sfd.FileName))
            //        {
            //            Process.Start(sfd.FileName);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, "btnExportTraceLog_Click PRESS...");
            //    WriteToLog(ex.Message, ApplicationConstants.TraceLogType.Error);
            //}
        }

        private void btnFindSpectrometers_Click(object sender, EventArgs e)
        {
            FindSpectrometers();
        }

        private void btnReadAllColors_Click(object sender, EventArgs e)
        {
            ReadAllColors();
        }

        private void txtSetUpperXValue_TextChanged(object sender, EventArgs e)
        {
            txtSetUpperXValue.Refresh();
        }

        private void txtSetUpperYValue_TextChanged(object sender, EventArgs e)
        {
            txtSetUpperYValue.Refresh();
        }

        private void txtSetWattValue_TextChanged(object sender, EventArgs e)
        {
            txtSetWattValue.Refresh();
        }

        private void lvTraceLog_ItemAdded(MetroFramework.Controls.MetroListView obj)
        {
            //lvTraceLog.Refresh();
        }

        private void btnClearVoltageReadings_Click(object sender, EventArgs e)
        {
            WriteToLog("Clearing voltage readings...", ApplicationConstants.TraceLogType.Information);

            tbTopRightVoltageReading.Text = "0";
            tbTopLeftVoltageReading.Text = "0";
            tbBottomRightVoltageReading.Text = "0";
            tbBottomLeftVoltageReading.Text = "0";

            rbTopRight.Checked = false;
            rbTopLeft.Checked = false;
            rbBottomRight.Checked = false;
            rbBottomLeft.Checked = false;
            rbHomeScreen.Checked = false;

            cbTopRightStatus.Checked = false;
            cbTopLeftStatus.Checked = false;
            cbBottomRightStatus.Checked = false;
            cbBottomLeftStatus.Checked = false;
        }

        private void ResetRadioButtons()
        {
            rbTopRight.Checked = false;
            rbTopLeft.Checked = false;
            rbBottomRight.Checked = false;
            rbBottomLeft.Checked = false;
            rbHomeScreen.Checked = false;

            cbTopRightStatus.Checked = false;
            cbTopLeftStatus.Checked = false;
            cbBottomRightStatus.Checked = false;
            cbBottomLeftStatus.Checked = false;
        }

        private void btnReadSensors_Click(object sender, EventArgs e)
        {
            if (pagevm == null)
                pagevm = new TestFixtureViewModel();

            WriteToLog("Reading sensor voltages...", ApplicationConstants.TraceLogType.Information);

            pagevm.ReadSensorsVoltages();
        }

        private void tbBottomRightVoltageReading_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtBottomRightY_TextChanged(object sender, EventArgs e)
        {
            TestFixtureViewModel.Instance.UpdateBRMirrorYValues = txtBottomRightY.Text;
        }

        private void txtBottomRightX_TextChanged(object sender, EventArgs e)
        {
            TestFixtureViewModel.Instance.UpdateBRMirrorXValues = txtBottomRightX.Text;
        }

        private void txtBottomLeftX_TextChanged(object sender, EventArgs e)
        {
            TestFixtureViewModel.Instance.UpdateBLMirrorXValues = txtBottomLeftX.Text;
        }

        private void txtBottomLeftY_TextChanged(object sender, EventArgs e)
        {
            TestFixtureViewModel.Instance.UpdateBLMirrorYValues = txtBottomLeftY.Text;
        }

        private void txtTopLeftY_TextChanged(object sender, EventArgs e)
        {
            TestFixtureViewModel.Instance.UpdatetLmirrorYvalues = txtTopLeftY.Text;
        }

        private void txtTopLeftX_TextChanged(object sender, EventArgs e)
        {
            TestFixtureViewModel.Instance.UpdatetLmirrorXvalues = txtTopLeftX.Text;
        }

        private void txtTopRightX_TextChanged(object sender, EventArgs e)
        {
            TestFixtureViewModel.Instance.UpdatetrmirrorXvalues = txtTopRightX.Text;
        }

        private void txtTopRightY_TextChanged(object sender, EventArgs e)
        {
            TestFixtureViewModel.Instance.UpdatetrmirrorYvalues = txtTopRightY.Text;
        }

        private void frmTestFixture_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (TestFixtureViewModel.Instance != null)
                {
                    TestFixtureViewModel.Instance.ResetDaqBoardPort();
                    //TestFixtureViewModel.Instance.SaveImgDetailsToFile(e);
                    //TestFixtureViewModel.Instance.SaveDetailsToFile(e);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Environment.Exit(1);
        }

        private void clearLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearLogs();
        }

        private void btnFTPAbort_Click(object sender, EventArgs e)
        {
            keeprunning = 0;

            if (bgwFTPDataThroughput.IsBusy)
                bgwFTPDataThroughput.CancelAsync();
        }

        private void btnFTPDataThroughput_Click(object sender, EventArgs e)
        {
            if (!bgwFTPDataThroughput.IsBusy)
                bgwFTPDataThroughput.RunWorkerAsync();
            else
                bgwFTPDataThroughput.CancelAsync();
        }
        #endregion


        #region bvgFTPDataThroughput Events
        private void bgwFTPDataThroughput_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //txtIpAddress.Text = "192.168.1.2:8080"; //Just Debugingg Purposes only
                ExecuteFTPDataThroughputTest(ApplicationConstants.AmbientLedState.Off);
                ExecuteFTPDataThroughputTest(ApplicationConstants.AmbientLedState.On);

                SetStatusIndicatorTextBoxBackColor(Color.Green);
            }
            catch (FTPSpeedOutOfRangeException ftp)
            {
                WriteToLog("Error: " + ftp.Message, ApplicationConstants.TraceLogType.Error);
                //SetErrorMessageDisplayTextBox("Error: " + ftp.Message);
                SetStatusIndicatorTextBoxBackColor(Color.Red);
            }
            catch (IpAddressNotFoundException ip)
            {
                WriteToLog("Error: " + ip.Message, ApplicationConstants.TraceLogType.Error);
                //SetErrorMessageDisplayTextBox("Error: " + ip.Message);
                SetStatusIndicatorTextBoxBackColor(Color.Red);
            }
            catch (IllumiVisionFileNotFoundException ip)
            {
                WriteToLog("Error: " + ip.Message, ApplicationConstants.TraceLogType.Error);
                //SetErrorMessageDisplayTextBox("Error: " + ip.Message);
                SetStatusIndicatorTextBoxBackColor(Color.Red);
            }
            catch (Exception ex)
            {
                WriteToLog("bgwFTPDataThroughput_DoWork: " + ex.Message, ApplicationConstants.TraceLogType.Error);
                SetErrorMessageDisplayTextBox("bgwFTPDataThroughput_DoWork: " + ex.Message);
                SetStatusIndicatorTextBoxBackColor(Color.Red);
            }
        }

        private void bgwFTPDataThroughput_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        #endregion

        //private void ExecuteFTPDataThroughputTest()
        //{
        //    System.IO.Stream clsStream = default(System.IO.Stream);
        //    System.Net.FtpWebRequest clsRequest = default(System.Net.FtpWebRequest);
        //    int chunkSize = 0;
        //    double myspeed = 0;
        //    long myTimeStarts = 0;

        //    double tempTickCount = 0;

        //    long myStarttime = 0;
        //    int i = 0;
        //    double[] myx = new double[25001];

        //    long StartTime = 0;

        //    Stopwatch sw = new Stopwatch();
        //    Stopwatch swFast = new Stopwatch();
        //    Stopwatch mystopwatch = new Stopwatch();
        //    int timerloops = 0;
        //    double elapsedtotal = 0;
        //    double myspeed1 = 0;

        //    string myfilename = string.Empty;

        //    int buffersize = 65535;

        //    try
        //    {
        //        SetErrorMessageDisplayTextBox("Connecting...");

        //        keeprunning = 1;

        //        mycounter = 0;
        //        //myfilename = Path.GetFileName(TB_filename.Text);
        //        //clsRequest = (System.Net.FtpWebRequest)System.Net.WebRequest.Create("ftp://" + TB_ipaddress.Text + "/" + myfilename);


        //        clsRequest.Credentials = new System.Net.NetworkCredential("pentair", "pentair");
        //        clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
        //        clsRequest.KeepAlive = true;
        //        clsRequest.ConnectionGroupName = "MyGroupName";
        //        clsRequest.ServicePoint.ConnectionLimit = 2;
        //        clsRequest.UseBinary = true;

        //        DateTime Date1 = default(DateTime);
        //        DateTime Date2 = default(DateTime);
        //        //byte[] File = System.IO.File.ReadAllBytes(TB_filename.Text);
        //        //lbl_size.Text = Math.Round(File.Length * 7.62939453125E-06, 1) + " Mb   (" + Math.Round(File.Length * 9.5367431640625E-07, 2) + "MB)";

        //        //if (Math.Round(File.Length / 1024 / 1000) < 5)
        //        //{
        //        //    MessageBox.Show("File is too small for testing.  Please select at least a 5MB file.");
        //        //    WriteToLog("ExecuteFTPDataThroughputTest: File is too small for testing.  Please select at least a 5MB file.", ApplicationConstants.TraceLogType.Information);
        //        //    return;
        //        //}


        //        clsStream = clsRequest.GetRequestStream();
        //        clsStream.WriteTimeout = 5000;
        //        Date1 = System.DateTime.Now;
        //        //Label1.Text = Date1.ToString();
        //        this.Refresh();

        //        myTimeStarts = System.DateTime.Now.Ticks;
        //        myStarttime = System.DateTime.Now.Ticks;
        //        tempTickCount = 0;
        //        i = 0;

        //        StartTime = System.DateTime.Now.Ticks;
        //        int mybuffer = buffersize;
        //        sw.Start();
        //        mystopwatch.Start();
        //        elapsedtotal = 0;
        //        timerloops = 0;
        //        //lbl_file.Text = "Uploading:   " + myfilename;
        //        //lbl_file2.Text = "Uploading:   " + myfilename;

        //    }
        //    catch (Exception e)
        //    {
        //        WriteToLog("ExecuteFTPDataThroughputTest: " + e.Message, ApplicationConstants.TraceLogType.Error);
        //    }
        //}
        internal void ExecuteFTPDataThroughputMainTest(ApplicationConstants.AmbientLedState ambientLedState)
        {
            Stream clsStream = default(Stream);
            FtpWebRequest clsRequest = default(FtpWebRequest);
            int chunkSize = 0;
            double myspeed = 0;
            long myTimeStarts = 0;

            double tempTickCount = 0;

            long myStarttime = 0;
            int i = 0;
            double[] myx = new double[25001];

            long StartTime = 0;

            Stopwatch sw = new Stopwatch();
            int timerloops = 0;
            double elapsedtotal = 0;
            double myspeed1 = 0;

            string myfilename = string.Empty;

            int buffersize = 65535;

            double[] myy = new double[25001];

            try
            {
                turnoffled = 1;

                SetStatusIndicatorTextBoxBackColor(Color.White);
                SetErrorMessageDisplayTextBox(String.Empty);
                SetErrorMessageDisplayTextBox("Connecting...");

                keeprunning = 1;

                mycounter = 0;

                myfilename = TestFixtureConstants.GetIllumaVisionFile();
                myfilename = Path.GetFileName(myfilename);

                if (string.IsNullOrEmpty(myfilename))
                {
                    SetStatusIndicatorTextBoxBackColor(Color.Red);
                    throw new IllumiVisionFileNotFoundException(string.Format("ExecuteFTPDataThroughputTest: Filename ('{0}') is empty...", myfilename));
                }
                //if (!TestFixtureSocketComm._IsIpAddressFound)
                //{
                //    WriteToLog(string.Format("ExecuteFTPDataThroughputTest: Ipaddress ('{0}') is not valid...", txtIpAddress.Text), ApplicationConstants.TraceLogType.Information);
                //    return;
                //}

                if (bgwFTPDataThroughput.CancellationPending)
                {
                    WriteToLog(string.Format("ExecuteFTPDataThroughputTest: FPTAbort..."), ApplicationConstants.TraceLogType.Information);
                    return;
                }

                //192.169.1.2:8080
                if (string.IsNullOrEmpty(TestFixtureSocketComm._RetrivedIpAddress))
                {
                    throw new IpAddressNotFoundException(string.Format("ExecuteFTPDataThroughputTest:  Ipaddress ('{0}') is null..", TestFixtureSocketComm._RetrivedIpAddress));
                }

                SetAmbientLedLight(ambientLedState);

                Thread.Sleep(500);

                clsRequest = (System.Net.FtpWebRequest)System.Net.WebRequest.Create("ftp://" + TestFixtureSocketComm._RetrivedIpAddress + "/" + myfilename);
                clsRequest.Credentials = new System.Net.NetworkCredential("pentair", "pentair");
                clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                clsRequest.KeepAlive = true;
                clsRequest.ConnectionGroupName = "MyGroupName";
                clsRequest.ServicePoint.ConnectionLimit = 2;
                clsRequest.UseBinary = true;

                byte[] File = System.IO.File.ReadAllBytes(myfilename);

                string filesize = Math.Round(File.Length * 7.62939453125E-06, 1) + " Mb   (" + Math.Round(File.Length * 9.5367431640625E-07, 2) + "MB)";

                SetFilesizeTextbox(filesize);

                var fileSize = Math.Round(Convert.ToDecimal(File.Length / 1024 / 1000));

                if (fileSize < 5)
                {
                    throw new ArgumentOutOfRangeException("ExecuteFTPDataThroughputTest: File is too small for testing.  Please select at least a 5MB file.");
                }

                clsStream = clsRequest.GetRequestStream();
                clsStream.WriteTimeout = 5000;
                myTimeStarts = DateTime.Now.Ticks;
                myStarttime = DateTime.Now.Ticks;
                tempTickCount = 0;
                i = 0;

                StartTime = DateTime.Now.Ticks;
                int mybuffer = buffersize;
                sw.Start();
                elapsedtotal = 0;
                timerloops = 0;

                SetErrorMessageDisplayTextBox("Uploading: " + myfilename);
                WriteToLog("Uploading:   " + myfilename, ApplicationConstants.TraceLogType.Information);

                if (bgwFTPDataThroughput.CancellationPending)
                {
                    WriteToLog("ExecuteFTPDataThroughputTest: FPTAbort...", ApplicationConstants.TraceLogType.Information);
                    return;
                }

                //Uploading file to illumivision light
                for (int offset = 0; offset <= File.Length; offset += buffersize) //Change back to int
                {
                    if (bgwFTPDataThroughput.CancellationPending)
                    {
                        WriteToLog("ExecuteFTPDataThroughputTest: FPTAbort...", ApplicationConstants.TraceLogType.Information);
                        return;
                    }

                    chunkSize = Convert.ToInt32(File.Length - offset - 1);

                    if (chunkSize > buffersize)
                        chunkSize = buffersize;
                    if (clsStream.CanWrite == false)
                    {
                        throw new NotSupportedException("ExecuteFTPDataThroughputTest: Cannot write to stream...");
                    }

                    //sends chunk to FTP server in illumivision
                    clsStream.Write(File, offset, chunkSize);

                    if (ambientLedState == ApplicationConstants.AmbientLedState.Off)
                    {
                        //Ambient Led Off
                        SetSpeedOffTextbox(totalspeed + " Mb/s (avg)");
                    }
                    else if (ambientLedState == ApplicationConstants.AmbientLedState.On)
                    {
                        //Ambient Led On
                        SetSpeedOnTextbox(totalspeed + " Mb/s (avg)");
                    }

                    decimal progressBarValue = ((decimal)offset / (decimal)File.Length) * pbFTPDataThroughProgress.Maximum;

                    SetFTPProgressStatusBar((int)progressBarValue);

                    if (bgwFTPDataThroughput.CancellationPending)
                    {
                        clsStream.Close();
                        clsStream.Dispose();
                        WriteToLog("Stop button pressed (cancel)", ApplicationConstants.TraceLogType.Information);
                        return;
                    }

                    if (bgwFTPDataThroughput.CancellationPending)
                    {
                        WriteToLog("ExecuteFTPDataThroughputTest: FPTAbort...", ApplicationConstants.TraceLogType.Information);
                        return;
                    }
                }

                sw.Stop();

                if (ambientLedState == ApplicationConstants.AmbientLedState.Off)
                {
                    //Ambient Led Off
                    totalspeed = Math.Round((File.Length / (Convert.ToSingle(sw.ElapsedMilliseconds) / 1000)) * 0.00000762939453125, 1);
                    SetSpeedOffTextbox(Math.Round((File.Length / (Convert.ToSingle(sw.ElapsedMilliseconds) / 1000)) * 0.00000762939453125, 1) + " Mb/s (avg)");

                }
                else if (ambientLedState == ApplicationConstants.AmbientLedState.On)
                {
                    //Ambient Led On
                    totalspeed = Math.Round((File.Length / (Convert.ToSingle(sw.ElapsedMilliseconds) / 1000)) * 0.00000762939453125, 1);
                    SetSpeedOnTextbox(Math.Round((File.Length / (Convert.ToSingle(sw.ElapsedMilliseconds) / 1000)) * 0.00000762939453125, 1) + " Mb/s (avg)");
                }

                SetErrorMessageDisplayTextBox("Ambient State: " + ambientLedState.ToString());
                WriteToLog("Ambient State: " + ambientLedState.ToString(), ApplicationConstants.TraceLogType.Information);

                //reset parameters
                sw.Reset();

                if (turnoffleds == true)
                {
                    mycommand = "http://" + TestFixtureSocketComm._RetrivedIpAddress + ":8080/api?cmd=setlightcolor&id=&idType=d&zone=&color=00000000";
                    sendrequest(mycommand);
                }

                turnoffleds = true;

                if (i > 2)
                {
                    Array.Resize(ref myx, i);
                    Array.Resize(ref myy, i);
                }

                myy[0] = 0;
                myx[0] = 0;

                turnoffled = 0;

                if (keeprunning == 0)
                {
                    SetFTPProgressStatusBar(0);
                }

                if (bgwFTPDataThroughput.CancellationPending)
                {
                    WriteToLog("ExecuteFTPDataThroughputTest: FPTAbort...", ApplicationConstants.TraceLogType.Information);
                    return;
                }

                //Delete the uploaded file
                //SetErrorMessageDisplayTextBox("Deleting: " + myfilename);
                WriteToLog("Deleting: " + myfilename, ApplicationConstants.TraceLogType.Information);

                System.Net.FtpWebRequest FTPRequest = (System.Net.FtpWebRequest)System.Net.WebRequest.Create("ftp://" + TestFixtureSocketComm._RetrivedIpAddress + "/" + myfilename);
                FTPRequest.Credentials = new System.Net.NetworkCredential("pentair", "pentair");
                FTPRequest.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;
                System.Net.FtpWebResponse FTPDelResp = (FtpWebResponse)FTPRequest.GetResponse();

                SetErrorMessageDisplayTextBox("Finished uploading and deleting: " + myfilename);
                WriteToLog("Finished uploading and deleting: " + myfilename, ApplicationConstants.TraceLogType.Information);

                if (bgwFTPDataThroughput.CancellationPending)
                {
                    WriteToLog("ExecuteFTPDataThroughputTest: FPTAbort...", ApplicationConstants.TraceLogType.Information);
                    return;
                }

                if (ambientLedState == ApplicationConstants.AmbientLedState.Off)
                {
                    if (totalspeed < Convert.ToDouble(pagevm.FTPOffMin) || totalspeed > Convert.ToDouble(pagevm.FTPOffMax))
                    {
                        throw new FTPSpeedOutOfRangeException("FTP Off Speed out of range");
                    }
                }

                if (ambientLedState == ApplicationConstants.AmbientLedState.On)
                {
                    if (totalspeed < Convert.ToDouble(pagevm.FTPOnMin) || totalspeed > Convert.ToDouble(pagevm.FTPOnMax))
                    {
                        throw new FTPSpeedOutOfRangeException("FTP ON Speed out of range");
                    }
                }

            }
            //catch (Exception e)
            //{
            //    SetErrorMessageDisplayTextBox("ExecuteFTPDataThroughputTest: " + e.Message);
            //    WriteToLog("ExecuteFTPDataThroughputTest: " + e.Message, ApplicationConstants.TraceLogType.Error);
            //}
            finally
            {
                if (clsStream != null)
                {
                    clsStream.Close();
                    clsStream.Dispose();
                }
                SetFTPProgressStatusBar(0);
            }
        }

        internal void ExecuteFTPDataThroughputTest(ApplicationConstants.AmbientLedState ambientLedState)
        {
            Stream clsStream = default(Stream);
            FtpWebRequest clsRequest = default(FtpWebRequest);
            int chunkSize = 0;
            double myspeed = 0;
            long myTimeStarts = 0;

            double tempTickCount = 0;

            long myStarttime = 0;
            int i = 0;
            double[] myx = new double[25001];

            long StartTime = 0;

            Stopwatch sw = new Stopwatch();
            int timerloops = 0;
            double elapsedtotal = 0;
            double myspeed1 = 0;

            string myfilename = string.Empty;
            string uploadfilename = string.Empty;

            int buffersize = 65535;

            double[] myy = new double[25001];

            try
            {
                turnoffled = 1;

                SetStatusIndicatorTextBoxBackColor(Color.White);
                SetErrorMessageDisplayTextBox(String.Empty);
                SetErrorMessageDisplayTextBox("Connecting...");

                keeprunning = 1;

                mycounter = 0;

                myfilename = TestFixtureConstants.GetIllumaVisionFile();
                uploadfilename = Path.GetFileName(myfilename);

                if (string.IsNullOrEmpty(myfilename))
                {
                    SetStatusIndicatorTextBoxBackColor(Color.Red);
                    throw new IllumiVisionFileNotFoundException(string.Format("ExecuteFTPDataThroughputTest: Filename ('{0}') is empty...", myfilename));
                }
                //if (!TestFixtureSocketComm._IsIpAddressFound)
                //{
                //    WriteToLog(string.Format("ExecuteFTPDataThroughputTest: Ipaddress ('{0}') is not valid...", txtIpAddress.Text), ApplicationConstants.TraceLogType.Information);
                //    return;
                //}

                if (bgwFTPDataThroughput.CancellationPending)
                {
                    WriteToLog(string.Format("ExecuteFTPDataThroughputTest: FPTAbort..."), ApplicationConstants.TraceLogType.Information);
                    return;
                }

                //192.169.1.2:8080
                if (string.IsNullOrEmpty(TestFixtureSocketComm._RetrivedIpAddress))
                {
                    throw new IpAddressNotFoundException(string.Format("ExecuteFTPDataThroughputTest:  Ipaddress ('{0}') is null..", TestFixtureSocketComm._RetrivedIpAddress));
                }

                SetAmbientLedLight(ambientLedState);

                Thread.Sleep(500);

                clsRequest = (System.Net.FtpWebRequest)System.Net.WebRequest.Create("ftp://" + TestFixtureSocketComm._RetrivedIpAddress + "/" + uploadfilename);
                clsRequest.Credentials = new System.Net.NetworkCredential("pentair", "pentair");
                clsRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                clsRequest.KeepAlive = true;
                clsRequest.ConnectionGroupName = "MyGroupName";
                clsRequest.ServicePoint.ConnectionLimit = 2;
                clsRequest.UseBinary = true;

                byte[] File = System.IO.File.ReadAllBytes(myfilename);

                string filesize = Math.Round(File.Length * 7.62939453125E-06, 1) + " Mb   (" + Math.Round(File.Length * 9.5367431640625E-07, 2) + "MB)";

                SetFilesizeTextbox(filesize);

                var fileSize = Math.Round(Convert.ToDecimal(File.Length / 1024 / 1000));

                if (fileSize < 5)
                {
                    throw new ArgumentOutOfRangeException("ExecuteFTPDataThroughputTest: File is too small for testing.  Please select at least a 5MB file.");
                }

                clsStream = clsRequest.GetRequestStream();
                clsStream.WriteTimeout = 5000;
                myTimeStarts = DateTime.Now.Ticks;
                myStarttime = DateTime.Now.Ticks;
                tempTickCount = 0;
                i = 0;

                StartTime = DateTime.Now.Ticks;
                int mybuffer = buffersize;
                sw.Start();
                elapsedtotal = 0;
                timerloops = 0;

                SetErrorMessageDisplayTextBox("Uploading: " + myfilename);
                WriteToLog("Uploading:   " + myfilename, ApplicationConstants.TraceLogType.Information);

                if (bgwFTPDataThroughput.CancellationPending)
                {
                    WriteToLog("ExecuteFTPDataThroughputTest: FPTAbort...", ApplicationConstants.TraceLogType.Information);
                    return;
                }

                //Uploading file to illumivision light
                for (int offset = 0; offset <= File.Length; offset += buffersize) //Change back to int
                {
                    if (bgwFTPDataThroughput.CancellationPending)
                    {
                        WriteToLog("ExecuteFTPDataThroughputTest: FPTAbort...", ApplicationConstants.TraceLogType.Information);
                        return;
                    }

                    chunkSize = Convert.ToInt32(File.Length - offset - 1);

                    if (chunkSize > buffersize)
                        chunkSize = buffersize;
                    if (clsStream.CanWrite == false)
                    {
                        throw new NotSupportedException("ExecuteFTPDataThroughputTest: Cannot write to stream...");
                    }

                    //sends chunk to FTP server in illumivision
                    clsStream.Write(File, offset, chunkSize);

                    if (ambientLedState == ApplicationConstants.AmbientLedState.Off)
                    {
                        //Ambient Led Off
                        SetSpeedOffTextbox(totalspeed + " Mb/s (avg)");
                    }
                    else if (ambientLedState == ApplicationConstants.AmbientLedState.On)
                    {
                        //Ambient Led On
                        SetSpeedOnTextbox(totalspeed + " Mb/s (avg)");
                    }

                    decimal progressBarValue = ((decimal)offset / (decimal)File.Length) * pbFTPDataThroughProgress.Maximum;

                    SetFTPProgressStatusBar((int)progressBarValue);

                    if (bgwFTPDataThroughput.CancellationPending)
                    {
                        clsStream.Close();
                        clsStream.Dispose();
                        WriteToLog("Stop button pressed (cancel)", ApplicationConstants.TraceLogType.Information);
                        return;
                    }

                    if (bgwFTPDataThroughput.CancellationPending)
                    {
                        WriteToLog("ExecuteFTPDataThroughputTest: FPTAbort...", ApplicationConstants.TraceLogType.Information);
                        return;
                    }
                }

                sw.Stop();

                if (ambientLedState == ApplicationConstants.AmbientLedState.Off)
                {
                    //Ambient Led Off
                    totalspeed = Math.Round((File.Length / (Convert.ToSingle(sw.ElapsedMilliseconds) / 1000)) * 0.00000762939453125, 1);
                    SetSpeedOffTextbox(Math.Round((File.Length / (Convert.ToSingle(sw.ElapsedMilliseconds) / 1000)) * 0.00000762939453125, 1) + " Mb/s (avg)");

                }
                else if (ambientLedState == ApplicationConstants.AmbientLedState.On)
                {
                    //Ambient Led On
                    totalspeed = Math.Round((File.Length / (Convert.ToSingle(sw.ElapsedMilliseconds) / 1000)) * 0.00000762939453125, 1);
                    SetSpeedOnTextbox(Math.Round((File.Length / (Convert.ToSingle(sw.ElapsedMilliseconds) / 1000)) * 0.00000762939453125, 1) + " Mb/s (avg)");
                }

                SetErrorMessageDisplayTextBox("Ambient State: " + ambientLedState.ToString());
                WriteToLog("Ambient State: " + ambientLedState.ToString(), ApplicationConstants.TraceLogType.Information);

                //reset parameters
                sw.Reset();

                if (turnoffleds == true)
                {
                    mycommand = "http://" + TestFixtureSocketComm._RetrivedIpAddress + ":8080/api?cmd=setlightcolor&id=&idType=d&zone=&color=00000000";
                    sendrequest(mycommand);
                }

                turnoffleds = true;

                if (i > 2)
                {
                    Array.Resize(ref myx, i);
                    Array.Resize(ref myy, i);
                }

                myy[0] = 0;
                myx[0] = 0;

                turnoffled = 0;

                if (keeprunning == 0)
                {
                    SetFTPProgressStatusBar(0);
                }

                if (bgwFTPDataThroughput.CancellationPending)
                {
                    WriteToLog("ExecuteFTPDataThroughputTest: FPTAbort...", ApplicationConstants.TraceLogType.Information);
                    return;
                }

                //Delete the uploaded file
                //SetErrorMessageDisplayTextBox("Deleting: " + myfilename);
                WriteToLog("Deleting: " + uploadfilename, ApplicationConstants.TraceLogType.Information);

                System.Net.FtpWebRequest FTPRequest = (System.Net.FtpWebRequest)System.Net.WebRequest.Create("ftp://" + TestFixtureSocketComm._RetrivedIpAddress + "/" + uploadfilename);
                FTPRequest.Credentials = new System.Net.NetworkCredential("pentair", "pentair");
                FTPRequest.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;
                System.Net.FtpWebResponse FTPDelResp = (FtpWebResponse)FTPRequest.GetResponse();

                SetErrorMessageDisplayTextBox("Finished uploading and deleting: " + uploadfilename);
                WriteToLog("Finished uploading and deleting: " + uploadfilename, ApplicationConstants.TraceLogType.Information);

                if (bgwFTPDataThroughput.CancellationPending)
                {
                    WriteToLog("ExecuteFTPDataThroughputTest: FPTAbort...", ApplicationConstants.TraceLogType.Information);
                    return;
                }

                if (ambientLedState == ApplicationConstants.AmbientLedState.Off)
                {
                    if (totalspeed < Convert.ToDouble(pagevm.FTPOffMin) || totalspeed > Convert.ToDouble(pagevm.FTPOffMax))
                    {
                        throw new FTPSpeedOutOfRangeException("FTP Off Speed out of range");
                    }
                }

                if (ambientLedState == ApplicationConstants.AmbientLedState.On)
                {
                    if (totalspeed < Convert.ToDouble(pagevm.FTPOnMin) || totalspeed > Convert.ToDouble(pagevm.FTPOnMax))
                    {
                        throw new FTPSpeedOutOfRangeException("FTP ON Speed out of range");
                    }
                }

            }
            //catch (Exception e)
            //{
            //    SetErrorMessageDisplayTextBox("ExecuteFTPDataThroughputTest: " + e.Message);
            //    WriteToLog("ExecuteFTPDataThroughputTest: " + e.Message, ApplicationConstants.TraceLogType.Error);
            //}
            finally
            {
                if (clsStream != null)
                {
                    clsStream.Close();
                    clsStream.Dispose();
                }
                SetFTPProgressStatusBar(0);
            }
        }

        private void SetAmbientLedLight(ApplicationConstants.AmbientLedState ambientLedState)
        {
            try
            {
                //Set zones to all on
                mycommand = "http://" + TestFixtureSocketComm._RetrivedIpAddress + ":8080/api?cmd=setlightZoneDefault&id=&idType=d&zone=FFFFFF&color=&featureName=zone&featureCmd=zone&value=FFFFFF";
                sendrequest(mycommand);

                if (ambientLedState == ApplicationConstants.AmbientLedState.Off)
                {
                    //Set ambient leds to off
                    mycommand = "http://" + TestFixtureSocketComm._RetrivedIpAddress + ":8080/api?cmd=setlightcolor&id=&idType=d&zone=&color=00000000";
                    sendrequest(mycommand);
                }
                else if (ambientLedState == ApplicationConstants.AmbientLedState.On)
                {
                    //Set ambient leds to on
                    mycommand = "http://" + TestFixtureSocketComm._RetrivedIpAddress + ":8080/api?cmd=setlightcolor&id=&idType=d&zone=&color=FFFFFFFF";
                    sendrequest(mycommand);
                }
                else
                {
                    WriteToLog("SetAbmientLedLight: Invalid ambient led type...", ApplicationConstants.TraceLogType.Warning);
                    return;
                }
            }
            catch (Exception e)
            {
                WriteToLog("SetAbmientLedLight: " + e.Message, ApplicationConstants.TraceLogType.Error);
                return;
            }
        }

        private string sendrequest(string myrequest)
        {
            string str = string.Empty;
            Stream data = null;
            StreamReader reader = null;

            try
            {
                WriteToLog("Sending: " + myrequest, ApplicationConstants.TraceLogType.Information);

                string URL = myrequest;
                // Get HTML data
                WebClient client = new WebClient();
                data = client.OpenRead(URL);
                reader = new StreamReader(data);
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                str = reader.ReadToEnd();

                WriteToLog(str, ApplicationConstants.TraceLogType.Information);
                data.Close();
                reader.Close();
            }
            catch (Exception e)
            {
                //SetErrorMessageDisplayTextBox("sendrequest: " + e.Message);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(Color.Red);
                WriteToLog("sendrequest: " + e.Message, ApplicationConstants.TraceLogType.Error);
            }
            finally
            {
                if (data != null)
                    data.Close();

                if (reader != null)
                    reader.Close();
            }

            return (str);
        }

        private void statusStrip1_Click(object sender, EventArgs e)
        {
            // If the user clicks the status bar, update current datetime
            tsTime.Text = DateTime.Now.ToString("F");
        }

        private void tsTime_Click(object sender, EventArgs e)
        {
            // If the user clicks the status bar, update current datetime
            tsTime.Text = DateTime.Now.ToString("F");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                // If the user clicks the status bar, update current datetime
                tsTime.Text = DateTime.Now.ToString("F");
            }
            catch (Exception ex)
            {
                WriteToLog("timer1_Tick: " + ex.Message, ApplicationConstants.TraceLogType.Error);
                timer1.Stop();
            }
        }

        private void tvEol_AfterCheck(object sender, TreeViewEventArgs e)
        {
            //if (e.Action != TreeViewAction.ByMouse)
            //    return;
            //SetParentNode(e.Node);
            //SetChildNode(e.Node);
        }

        private void SetParentNode(TreeNode node)
        {
            if (node.Level > 0)
            {
                node.Parent.Checked = true;
                SetParentNode(node.Parent);
            }
        }

        private void SetChildNode(TreeNode node)
        {
            foreach (TreeNode childNode in node.Nodes)
            {
                childNode.Checked = node.Checked;
                if (node.Nodes.Count > 0)
                    SetChildNode(childNode);
            }
        }

        public void ResetEolTreeView()
        {
            if (!tvEol.IsHandleCreated)
                return;

            tvEol.Invoke((MethodInvoker)delegate

            {
                tvEol.Nodes[0].ForeColor = Color.Black;
                tvEol.Nodes[0].Checked = false;

                foreach (TreeNode myNode in tvEol.Nodes[0].Nodes)
                {
                    // Set the parent node's foreColor.
                    myNode.Checked = false;

                    switch (myNode.Text)
                    {
                        case "MODEL NO":
                            if (pagevm._eolmodel.ModelNumber)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "SERIAL NO":
                            if (pagevm._eolmodel.SerialNumber)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "POWER":
                            if (pagevm._eolmodel.PowerOn)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "PAIRING":
                            if (pagevm._eolmodel.Pairing)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "PENTAIR SERVER":
                            if (pagevm._eolmodel.PentairServer)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "FIRMWARE VERSION":
                            if (pagevm._eolmodel.FirmwareVersion)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "PROJECTOR MIRROR CHECK":
                            if (pagevm._eolmodel.ProjectorMirrorCheck)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "LED BRIGHTNESS / COLOR":
                            if (pagevm._eolmodel.LedBrightnessColor)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "BANDWIDTH":
                            if (pagevm._eolmodel.Bandwidth)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "PROJECTOR BRIGHTNESS":
                            if (pagevm._eolmodel.ProjectorBrightness)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "PROJECTOR FOCUS":
                            if (pagevm._eolmodel.ProjectorFocus)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "TEST COMPLETION":
                            if (pagevm._eolmodel.TestCompletion)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        default:
                            break;
                    }

                    foreach (TreeNode childNode in myNode.Nodes)
                    {
                        // Set the child node's foreColor.
                        childNode.ForeColor = Color.Black;
                        childNode.Checked = false;
                    }
                }
            });
        }

        public void ResetLightEngineTreeView()
        {
            if (!tvLightEngine.IsHandleCreated)
                return;

            tvLightEngine.Invoke((MethodInvoker)delegate

            {
                tvLightEngine.Nodes[0].ForeColor = Color.Black;
                tvLightEngine.Nodes[0].Checked = false;

                foreach (TreeNode myNode in tvLightEngine.Nodes[0].Nodes)
                {

                    myNode.Checked = false;

                    // Set the parent node's foreColor.
                    myNode.Checked = false;

                    switch (myNode.Text)
                    {

                        case "SERIAL NO":
                            if (pagevm._lightenginemodel.SerialNumber)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "POWER":
                            if (pagevm._lightenginemodel.PowerOn)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;


                        case "PENTAIR SERVER":
                            if (pagevm._lightenginemodel.PentairServer)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "FIRMWARE VERSION":
                            if (pagevm._lightenginemodel.FirmwareVersion)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "PROJECTOR MIRROR CHECK":
                            if (pagevm._lightenginemodel.ProjectorMirrorCheck)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "LED BRIGHTNESS / COLOR":
                            if (pagevm._lightenginemodel.LedBrightnessColor)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "PROJECTOR BRIGHTNESS":
                            if (pagevm._lightenginemodel.ProjectorBrightness)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "PROJECTOR FOCUS":
                            if (pagevm._lightenginemodel.ProjectorFocus)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        case "TEST COMPLETION":
                            if (pagevm._lightenginemodel.TestCompletion)
                                myNode.ForeColor = Color.Black;
                            else
                                myNode.ForeColor = Color.DarkGray;
                            break;

                        default:
                            break;
                    }

                    foreach (TreeNode childNode in myNode.Nodes)
                    {
                        // Set the child node's foreColor.
                        childNode.ForeColor = Color.Black;
                        childNode.Checked = false;
                    }
                }
            });
        }

        private void btnClearStatusIndicatorColor_Click(object sender, EventArgs e)
        {
            SetStatusIndicatorTextBoxBackColor(Color.White);
            SetStatusIndicatorTextBox(string.Empty);
            SetProgressStatusBar(0);
            SetErrorMessageDisplayTextBox(string.Empty);
        }

        private void txtInformation_TextChanged(object sender, EventArgs e)
        {
            txtInformation.SelectionStart = txtInformation.Text.Length;
            txtInformation.ScrollToCaret();
        }

        private void txtWarnings_TextChanged(object sender, EventArgs e)
        {
            txtWarnings.SelectionStart = txtWarnings.Text.Length;
            txtWarnings.ScrollToCaret();
        }

        private void txtErrors_TextChanged(object sender, EventArgs e)
        {
            txtErrors.SelectionStart = txtErrors.Text.Length;
            txtErrors.ScrollToCaret();
        }

        private void btnFixedBlendedWhite_Click(object sender, EventArgs e)
        {
            if (TestFixtureViewModel.Instance != null)
                TestFixtureViewModel.Instance.TurnOnBlendedWhiteLightCommand();
        }

        private void btnFixedMagenda_Click(object sender, EventArgs e)
        {
            if (TestFixtureViewModel.Instance != null)
                TestFixtureViewModel.Instance.TurnOnMagendaLightCommand();
        }

        private void btnBlendedWhite_Click(object sender, EventArgs e)
        {
            if (IsCalibrationFileFound)
            {
                ExecuteSpectrumReading(TestFixtureProject.Common.ApplicationConstants.SpectrumColors.BlendedWhite);
            }
            else
            {
                if (TestFixtureViewModel.Instance.BrowseToCalibrationFile(e))
                {
                    ExecuteSpectrumReading(TestFixtureProject.Common.ApplicationConstants.SpectrumColors.BlendedWhite);
                }
            }
        }

        private void btnMagenda_Click(object sender, EventArgs e)
        {
            if (IsCalibrationFileFound)
            {
                ExecuteSpectrumReading(TestFixtureProject.Common.ApplicationConstants.SpectrumColors.Magenda);
            }
            else
            {
                if (TestFixtureViewModel.Instance.BrowseToCalibrationFile(e))
                {
                    ExecuteSpectrumReading(TestFixtureProject.Common.ApplicationConstants.SpectrumColors.Magenda);
                }
            }
        }

        private void tabControl2_TabIndexChanged(object sender, EventArgs e)
        {
            tabControl2.Invoke((MethodInvoker)delegate

            {
                if (tabControl2.SelectedTab.Text == "Diagnostics")
                {
                    if (TestFixtureSocketComm._IsIpAddressFound)
                    {
                        SetIpAddressTextBox(TestFixtureSocketComm._RetrivedIpAddress);
                    }
                    else
                    {
                        SetIpAddressTextBox(string.Empty);
                    }
                }
            });
        }

        private void tabControl2_Selecting(object sender, TabControlCancelEventArgs e)
        {
            TabPage current = (sender as TabControl).SelectedTab;

            if (current.Text == "Diagnostics")
            {
                if (TestFixtureSocketComm._IsIpAddressFound)
                {
                    string ip = TestFixtureSocketComm._RetrivedIpAddress;

                    SetIpAddressLabel("IP Address: " + ip.ToString());
                    SetIpAddressTextBox("IP Address: " + ip.ToString());
                }
                else
                {
                    SetIpAddressTextBox(string.Empty);
                }
            }

            // Validate the current page. To cancel the select, use:
            //e.Cancel = true;
        }

        private void btnLoadImageSettings_Click(object sender, EventArgs e)
        {
            if (pagevm != null)
            {
                pagevm._mTestProjectImgModel.LoadSettingDetailsFromFile();
                GetMirrorSettings();
            }
        }

        private void startTestSeqThreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!pagevm.Startthread.IsAlive)
            {
                pagevm = new TestFixtureViewModel();

                //SetErrorMessageDisplayTextBox("Press Green Button to Start...");
            }
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedTab.Name == "tpMain")
            {
                RemoveEngineeringPortal();
            }
        }

        private void btn120VacOff_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            TestFixtureViewModel.Instance.Start120ACVRelayOff();

            Cursor.Current = Cursors.Default;
        }

        private void btn15VacOff_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            TestFixtureViewModel.Instance.TurnOff15ACVRelay();

            Cursor.Current = Cursors.Default;
        }

        private void resetTestSequenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetEolTreeView();

            ResetLightEngineTreeView();
        }

        private void cbLineTesterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLineTesterType.SelectedItem.ToString() == "EOL")
            {
                pagevm._lineTestermodel.EOL = true;
                pagevm._lineTestermodel.LightEngine = false;
            }
            else if (cbLineTesterType.SelectedItem.ToString() == "LightEngine")
            {
                pagevm._lineTestermodel.EOL = false;
                pagevm._lineTestermodel.LightEngine = true;
            }
            else
            {
                WriteToLog("Unknown Line Tester Type...", ApplicationConstants.TraceLogType.Warning);
                MessageBox.Show("Unknown Line Tester Type...", "LINE TESTER TYPE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //LoadLineTester();
        }

        private void btnSaveEolTestSequence_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                TestFixtureViewModel.Instance.SaveEolDetailsToFile();

                WriteToLog("Save 'EOL' test sequence successfully...", ApplicationConstants.TraceLogType.Information);
            }
            catch (Exception ex)
            {
                WriteToLog("btnSaveEolTestSequence_Click: " + ex.Message, ApplicationConstants.TraceLogType.Error);
            }
            Cursor.Current = Cursors.Default;
        }

        private void dgvEol_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            senderGrid.EndEdit();

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dgvEol.Rows[e.RowIndex];

                if (row != null)
                {
                    var cbxCell = (DataGridViewCheckBoxCell)senderGrid.Rows[e.RowIndex].Cells["RunTest"];

                    if (row.Cells["Test"].Value.ToString() == "ModelNumber")
                        pagevm._eolmodel.ModelNumber = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "SerialNumber")
                        pagevm._eolmodel.SerialNumber = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "PowerOn")
                        pagevm._eolmodel.PowerOn = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "Pairing")
                        pagevm._eolmodel.Pairing = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "PentairServer")
                        pagevm._eolmodel.PentairServer = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "FirmwareVersion")
                        pagevm._eolmodel.FirmwareVersion = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "ProjectorMirrorCheck")
                        pagevm._eolmodel.ProjectorMirrorCheck = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "LedBrightnessColor")
                        pagevm._eolmodel.LedBrightnessColor = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "Bandwidth")
                        pagevm._eolmodel.Bandwidth = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "ProjectorFocus")
                        pagevm._eolmodel.ProjectorFocus = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "ProjectorBrightness")
                        pagevm._eolmodel.ProjectorBrightness = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "TestCompletion")
                        pagevm._eolmodel.TestCompletion = (bool)cbxCell.Value;
                }
            }
        }

        private void dgvLightEngine_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            var senderGrid = (DataGridView)sender;
            senderGrid.EndEdit();

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dgvLightEngine.Rows[e.RowIndex];

                if (row != null)
                {

                    var cbxCell = (DataGridViewCheckBoxCell)senderGrid.Rows[e.RowIndex].Cells["RunTest"];

                    if (row.Cells["Test"].Value.ToString() == "SerialNumber")
                        pagevm._lightenginemodel.SerialNumber = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "PowerOn")
                        pagevm._lightenginemodel.PowerOn = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "PentairServer")
                        pagevm._lightenginemodel.PentairServer = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "FirmwareVersion")
                        pagevm._lightenginemodel.FirmwareVersion = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "ProjectorMirrorCheck")
                        pagevm._lightenginemodel.ProjectorMirrorCheck = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "LedBrightnessColor")
                        pagevm._lightenginemodel.LedBrightnessColor = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "ProjectorFocus")
                        pagevm._lightenginemodel.ProjectorFocus = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "ProjectorBrightness")
                        pagevm._lightenginemodel.ProjectorBrightness = (bool)cbxCell.Value;

                    if (row.Cells["Test"].Value.ToString() == "TestCompletion")
                        pagevm._lightenginemodel.TestCompletion = (bool)cbxCell.Value;

                }
            }
        }

        private void btnSaveLightEngineTEstSequence_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                TestFixtureViewModel.Instance.SaveLightEngineDetailsToFile();

                WriteToLog("Save 'LIGHT ENGINE' test sequence successfully...", ApplicationConstants.TraceLogType.Information);
            }
            catch (Exception ex)
            {
                WriteToLog("btnSaveLightEngineTEstSequence_Click: " + ex.Message, ApplicationConstants.TraceLogType.Error);
            }
            Cursor.Current = Cursors.Default;
        }

        private void btnSaveLineTester_Click(object sender, EventArgs e)
        {
            SetLineTesterTabPages();
        }

        private void SetLineTesterTabPages()
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (cbLineTesterType.SelectedItem != null)
                {
                    if (pagevm._lineTestermodel.EOL)
                    {
                        cbLineTesterType.SelectedIndex = 0;
                        lblLineTester.Text = "EOL LINE TESTER";
                        WriteToLog("EOL Line Tester selected...", ApplicationConstants.TraceLogType.Information);

                        ShowLineTesterTabPages(tpEol);

                        tvEol.ExpandAll();
                        tvEol.CheckBoxes = true;

                        //Since we are testing EOL Test Sequence, remove / hide EMLINQ tab control
                        tpLightEngine.Hide();
                        tcLineTester.TabPages.Remove(tpLightEngine);

                        ResetEolTreeView();

                    }
                    else if (pagevm._lineTestermodel.LightEngine)
                    {
                        cbLineTesterType.SelectedIndex = 1;
                        lblLineTester.Text = "LIGHT ENGINE TESTER";
                        WriteToLog("LIGHT ENGINE Line Tester selected...", ApplicationConstants.TraceLogType.Information);

                        ShowLineTesterTabPages(tpLightEngine);

                        tvLightEngine.ExpandAll();
                        tvLightEngine.CheckBoxes = true;

                        //Since we are testing EOL Test Sequence, remove / hide EMLINQ tab control
                        tpEol.Hide();
                        tcLineTester.TabPages.Remove(tpEol);

                        ResetLightEngineTreeView();
                    }

                    TestFixtureViewModel.Instance.SaveLineTesterDetailsToFile();

                    WriteToLog("Save 'LINE TESTER' settings successfully...", ApplicationConstants.TraceLogType.Information);

                    TestFixtureViewModel.Instance.SaveEolDetailsToFile();

                    WriteToLog("Save 'EOL' test sequence successfully...", ApplicationConstants.TraceLogType.Information);

                    TestFixtureViewModel.Instance.SaveLightEngineDetailsToFile();

                    WriteToLog("Save 'LINE ENGINE' test sequence successfully...", ApplicationConstants.TraceLogType.Information);
                }
                else
                {
                    WriteToLog("Please select line tester from drop down menu...", ApplicationConstants.TraceLogType.Warning);
                    MessageBox.Show("Please select line tester from drop down menu...", "LINE TESTER", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                WriteToLog("btnSaveLineTester_Click ERROR: " + ex.Message, ApplicationConstants.TraceLogType.Error);
                MessageBox.Show("btnSaveLineTester_Click ERROR: " + ex.Message, "LINE TESTER", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Cursor.Current = Cursors.Default;
        }

        private void dgvEOL_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvEol.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvEol.AutoResizeColumns();
            dgvEol.AllowUserToResizeColumns = true;
            dgvEol.AllowUserToOrderColumns = true;
        }

        private void dgvLightEngine_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgvLightEngine.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvLightEngine.AutoResizeColumns();
            dgvLightEngine.AllowUserToResizeColumns = true;
            dgvLightEngine.AllowUserToOrderColumns = true;
        }
    }
}
