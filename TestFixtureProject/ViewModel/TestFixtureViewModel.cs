using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TestFixtureProject.Common;
using TestFixtureProject.DataAccess;
using TestFixtureProject.Helpers;
using TestFixtureProject.Model;
using Newtonsoft.Json.Linq;
using System.Threading;
using AForge.Video.DirectShow;
using AForge.Video;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

using System.Net;
using System.Diagnostics;

namespace TestFixtureProject.ViewModel
{
    public delegate void ExternalStartEvent();
    public delegate void ExternalStopEvent();

    public class TestFixtureViewModel : TestFixtureObserverBase
    {
        //singleton design pattern
        private static TestFixtureViewModel _instance;

        public static TestFixtureViewModel Instance
        {
            get
            {
                return _instance;
            }
        }

        #region  variables
        internal bool mainTabSelected = false;
        internal TestFixtureSettingModel _model = null;
        internal TestFixtureEolTestSequenceModel _eolmodel = null;
        internal TestFixtureLightEngineTestSequenceModel _lightenginemodel = null;
        internal TestFixtureLineTesterConfigModel _lineTestermodel = null;
        private TestFixtureFileOperation _fileoperation = null;
        private TestFixtureDLUValueSetModel _dluvaluesetmodel = null;
        private TestFixtureMainModel _mainmodel;
        // private TestFixtureSocketComm _mtestfixturesocketcomm;
        internal TestFixtureDAQ _mtestfixturedaq;
        private TestFixtureProjectorModel _mtestfixtureProjector;
        private TestFixtureTestMainModel _mTestFixtureTestMainModel = null;
        private TestFixtureSerialPortComm _mTestFixtureSerialComm = null;
        private TestFixtureSpectrometer _mTestFixtureSpectrum = null;
        internal TestFixtureProjectorImageModel _mTestProjectImgModel = null;
        //TestFixtureEventLogger _mlogger = null;
        internal int PositionValidationCounter = 0;


        private static int firstTime = 0; //NP -old
        private string commPortNumber = null; // PK

        private MccDaq.MccBoard DaqBoard;
        private MccDaq.ErrorInfo ULStat;
        private string _mErrMsg = null;
        internal string serverIpAddess = null; //KD: Change to internal
        VideoCaptureDevice _mCaptdevice;
        internal bool _m120flag = false;
        private bool _mdhcpflag = false;
        private bool _mpclsflag = false;
        private bool fileRemove = false;

        internal static bool isStopbtnPressed = false;
        internal static bool isStartPressed = false;
        internal static bool isTestExecutionFailure = false;

        private bool _mgrantAccess = true;
        private const string PASSWORD = "PASSWORD";
        private TestFixtureErrorMessage _errmessage = null;
        private bool _mprogress;
        private int _mtotalTests = 22;
        private int _mMinTestSteps = 0;
        private Double _mValue;
        private bool _mRedFlag = false;
        private bool _mGreenFlag = false;
        private bool _mBlueFlag = false;
        private bool _mWhiteFlag = false;
        private bool spectroFile = false;
        private string comPortNumber = string.Empty;
        public bool triggerFlag = false;
        public System.Threading.Timer timer;
        private string serverStatus = "NOTAVAILABLE";
        private string notConnected = "NOTAVAILABLE";
        private string connected = "AVAILABLE";
        private string interrupted = "INTERRUPT";
        string logTestFormat = string.Empty;
        public bool openFile = false;

        private float readTRvoltage = 0;
        private float readTLvoltage = 0;
        private float readBRvoltage = 0;
        private float readBLvoltage = 0;

        //message log counter
        private int counter = 0;
        #endregion

        #region event
        public event ExternalStartEvent OnChangeExternalTrigger;
        public event ExternalStopEvent OnExternalStopTrigger;

        internal Thread stopthread = null;

        internal Thread Startthread = null;

        #endregion

        #region constructor
        public TestFixtureViewModel()
        {
            try
            {
                // 12-12-2016
                /*
                 * Get all the ports and find available port 
                */

                string[] portNames = System.IO.Ports.SerialPort.GetPortNames();
                commPortNumber = portNames[0];
                // commPortNumber
                DaqBoard = new MccDaq.MccBoard();
                //We have now tied the Model to the ViewModel.
                _mainmodel = new TestFixtureMainModel();
                _dluvaluesetmodel = new TestFixtureDLUValueSetModel(_mgrantAccess);
                _model = new TestFixtureSettingModel(_mgrantAccess);
                //_mtestfixturesocketcomm = new TestFixtureSocketComm(); // Made class as static
                _mtestfixturedaq = new TestFixtureDAQ(ref DaqBoard);
                _mTestProjectImgModel = new TestFixtureProjectorImageModel();
                //_mlogger = new TestFixtureEventLogger();

                _mtestfixtureProjector = new TestFixtureProjectorModel(_mgrantAccess);

                _lineTestermodel = new TestFixtureLineTesterConfigModel();
                _eolmodel = new TestFixtureEolTestSequenceModel();
                _lightenginemodel = new TestFixtureLightEngineTestSequenceModel();

                _mTestFixtureTestMainModel = new TestFixtureTestMainModel();
                _errmessage = new TestFixtureErrorMessage();

                //Startthread = new Thread(this.ListenToStartEvent); 
                Startthread = new Thread(this.PushButton);
                Startthread.Name = "StartButtonThread";
                _mTestFixtureSpectrum = new TestFixtureSpectrometer();
                _mCaptdevice = new VideoCaptureDevice();
                stopthread = new Thread(this.ListenToStopEvent);
                stopthread.Name = "StopButtonThread";

                _mtestfixturedaq.ConfigureDaqToSingleEnded();

                Startthread.Start();
                //isTestExecutionFailure = true;

                this.OnChangeExternalTrigger += new ExternalStartEvent(StartExecutionEvents);

                this.OnExternalStopTrigger += new ExternalStopEvent(StopListeningToEvent);

                ResetDaqBoardPort();

                _instance = this;
            }
            catch (Exception e)
            {
                if (e.Source == "NETOmniDriver-NET40")
                {
                    // Here we are not handling Accessviolation Exception 
                }
                else
                {
                    if (ErrorMsgDisplay != null)
                    {
                        ErrorMsgDisplay = "NETOmniDriver issue. Please Restart Application";
                        System.Windows.Forms.MessageBox.Show(ErrorMsgDisplay + ": " + e.Message, "NETOmniDriver", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //frmTestFixture.Instance.WriteToLog("TestFixtureViewModel: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
                    }
                }

                //if (firstTime == 0)
                //{
                //    Startthread.Start();
                //    firstTime = 1;
                //}

                //UpdateTestResultPassFailStatus(true, "/*Press Green Button to Start*/");
                // Startthread.Start();
            }
        }
        #endregion
           
        #region for error display

        public TestFixtureErrorMessage ErrorMsgDispModel
        {
            get { return _errmessage; }
            set
            {
                _errmessage = value;
                OnPropertyChanged("ErroMsgModel");
            }
        }
        public string ErrorMsgDisplay
        {
            get
            {
                if (ErrorMsgDispModel != null)
                    return ErrorMsgDispModel.TestMessage;
                else
                    return string.Empty;
            }
            set
            {
                if (ErrorMsgDispModel == null)
                    ErrorMsgDispModel = new TestFixtureErrorMessage();

                ErrorMsgDispModel.TestMessage = value;

                if (!string.IsNullOrEmpty(value))
                {
                    if (frmTestFixture.Instance != null)
                    {
                        //Keith Dudley
                        //frmTestFixture.Instance.SetUpdatePassFailResultTextBox(value);
                        frmTestFixture.Instance.SetErrorMessageDisplayTextBox(value);
                    }
                }

                OnPropertyChanged("ErrorMsgDisplay");
            }
        }
        #endregion

        public void CheckVisibility()
        {
            //ButtonOK.IsVisible = false;
            //ButtonOK.IsEnabled = false;

            JObject passobject;
            //SecureString password;
            var password = string.Empty;
            TestFixtureDataStores.Information.TryGetValue(PASSWORD, out password);

            string loginFileName = TestFixtureConstants.GetLoginInfoFilePath();

            using (StreamReader file = File.OpenText(loginFileName))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                passobject = (JObject)JToken.ReadFrom(reader);
            }
            JToken tokenvalue = passobject.First;
            var value = tokenvalue.First.ToString();

            _mgrantAccess = true;
        }


        #region public methods to get/set values of setting tab control
        //This property allows us to change and receive changes made to the Model.
        [JsonProperty("_model")]
        public TestFixtureSettingModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged("Model");
            }
        }
        #endregion

        #region White Limits RGB Red Lower Value
        [JsonProperty("Model.WLRGBXMINVALUE")]
        public string WLRGBXMINVALUE
        {
            get { return Model.WLRGBXMINVALUE; }
            set
            {
                if (value != null)
                {
                    Model.WLRGBXMINVALUE = value;
                }
                else
                {
                    //read default value later after getting it... 
                }
                OnPropertyChanged("WLRGBXMINVALUE");
            }
        }
        ////   white limits RGB Red Upper Values
        public string WLRYMINVALUE
        {
            get { return Model.WLRYMINVALUE; }
            set
            {
                if (value != null)
                {
                    Model.WLRYMINVALUE = value;
                }
                else //assign default value
                {
                    //later read default values from file
                    // Model.WLRGBLimitsUValue = "1234";
                }
                OnPropertyChanged("WLRYMINVALUE");
            }
        }

        ////White Limits RGB Flux Lower Value
        public string WLRGBXMAXVALUE
        {
            get { return Model.WLRGBXMAXVALUE; }
            set
            {
                if (value != null)
                {
                    Model.WLRGBXMAXVALUE = value;
                }
                else
                {
                    //read default value 
                }
                OnPropertyChanged("WLRGBXMAXVALUE");
            }
        }

        public string WhiteMicroWattValue
        {
            get { return Model.WhiteMicroWattValue; }

            set
            {
                if (value != null)
                {
                    Model.WhiteMicroWattValue = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("WhiteMicroWattValue");
            }
        }

        public string WhiteMicroWattValueUpper
        {
            get { return Model.WhiteMicroWattValueUpper; }

            set
            {
                if (value != null)
                {
                    Model.WhiteMicroWattValueUpper = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("WhiteMicroWattValueUpper");
            }
        }

        public string BootSequenceTime
        {
            get { return Model.BootSequenceTime; }
            set
            {
                if (value != null)
                {
                    Model.BootSequenceTime = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("BootSequenceTime");
            }
        }

        public string IPAddressRange
        {
            get { return Model.IpAddressRange; }
            set
            {
                if (value != null)
                {
                    Model.IpAddressRange = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("IPAddressRange");
            }
        }

        public string IntegrationTime
        {
            get { return Model.IntegrationTime; }
            set
            {
                if (value != null)
                {
                    Model.IntegrationTime = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("IntegrationTime");
            }
        }

        public string BlueIntegrationTime
        {
            get { return Model.BlueIntegrationTime; }
            set
            {
                if (value != null)
                {
                    Model.BlueIntegrationTime = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("BlueIntegrationTime");
            }
        }

        public string RedIntegrationTime
        {
            get { return Model.RedIntegrationTime; }
            set
            {
                if (value != null)
                {
                    Model.RedIntegrationTime = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("RedIntegrationTime");
            }
        }

        public string GreenIntegrationTime
        {
            get { return Model.GreenIntegrationTime; }
            set
            {
                if (value != null)
                {
                    Model.GreenIntegrationTime = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("GreenIntegrationTime");
            }
        }

        public string WhiteIntegrationTime
        {
            get { return Model.WhiteIntegrationTime; }
            set
            {
                if (value != null)
                {
                    Model.WhiteIntegrationTime = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("WhiteIntegrationTime");
            }
        }

        public string BlendedWhiteIntegrationTime
        {
            get { return Model.BlendedWhiteIntegrationTime; }
            set
            {
                if (value != null)
                {
                    Model.BlendedWhiteIntegrationTime = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("BlendedWhiteIntegrationTime");
            }
        }

        public string MagendaIntegrationTime
        {
            get { return Model.MagendaIntegrationTime; }
            set
            {
                if (value != null)
                {
                    Model.MagendaIntegrationTime = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("MagendaIntegrationTime");
            }
        }

        public string FTPOnMin
        {
            get { return Model.FtpOnMin; }
            set
            {
                if (value != null)
                {
                    Model.FtpOnMin = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("FTPOnMin");
            }
        }

        public string FTPOnMax
        {
            get { return Model.FtpOnMax; }
            set
            {
                if (value != null)
                {
                    Model.FtpOnMax = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("FTPOnMax");
            }
        }

        public string FTPOffMin
        {
            get { return Model.FtpOffMin; }
            set
            {
                if (value != null)
                {
                    Model.FtpOffMin = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("FTPOffMin");
            }
        }

        public string FTPOffMax
        {
            get { return Model.FtpOffMax; }
            set
            {
                if (value != null)
                {
                    Model.FtpOffMax = value;
                }
                else
                {
                    //read default value
                }
                OnPropertyChanged("FTPOffMax");
            }
        }
        #endregion

        [JsonProperty("_comportnumb")]
        private string _comportnumb = null;
        public string CommPortNUmber
        {
            get { return Model.CommPortNUmber; }
            set
            {
                Model.CommPortNUmber = value;
                OnPropertyChanged("CommPortNUmber");
            }

        }

        #region Green Limits Lower and upper Value
        public string GLRGBGREENLValue  // Green Limits RGB Green Lower Value
        {
            get { return Model.GLRGBGREENLValue; }

            set
            {
                if (value != null)
                {
                    Model.GLRGBGREENLValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("GLRGBGREENLValue");
            }
        }

        public string GreenYMinValues  // Green Limits RGB Green Lower Value
        {
            get { return Model.GreenYMinValues; }

            set
            {
                if (value != null)
                {
                    Model.GreenYMinValues = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("GreenYMinValues");
            }
        }
        //GreenYMaxValue

        public string GreenYMaxValue  // Green Limits RGB Green Lower Value
        {
            get { return Model.GreenYMaxValue; }

            set
            {
                if (value != null)
                {
                    Model.GreenYMaxValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("GreenYMaxValue");
            }
        }



        public string GLRGBGREENUValue  // Green Limits RGB Green Upper Value
        {
            get { return Model.GLRGBGREENUValue; }

            set
            {
                if (value != null)
                {
                    Model.GLRGBGREENUValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("GLRGBGREENUValue");
            }
        }


        public string GreenMicroWattValue
        {
            get { return Model.GreenMicroWattValue; }

            set
            {
                if (value != null)
                {
                    Model.GreenMicroWattValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("GreenMicroWattValue");
            }
        }

        public string GreenMicroWattValueUpper
        {
            get { return Model.GreenMicroWattValueUpper; }

            set
            {
                if (value != null)
                {
                    Model.GreenMicroWattValueUpper = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("GreenMicroWattValueUpper");
            }
        }
        #endregion

        #region Blended white Lower and Upper Limit set
        public string BlendedWhiteRedLimits
        {
            get { return Model.BlendedWhiteRedLimits; }
            set
            {
                if (value != null)
                {
                    Model.BlendedWhiteRedLimits = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("BlendedWhiteRedLimits");
            }
        }

        //Blended Limits Red Upper Value
        public string BlendedWhiteRedULimits
        {
            get { return Model.BlendedWhiteRedULimits; }
            set
            {
                if (value != null)
                {
                    Model.BlendedWhiteRedULimits = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("BlendedWhiteRedULimits");
            }
        }

        //Blended Limits Green Lower Value
        public string BlendedWhiteGreenLLimits
        {
            get { return Model.BlendedWhiteGreenLLimits; }
            set
            {
                if (value != null)
                {
                    Model.BlendedWhiteGreenLLimits = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("BlendedWhiteGreenLLimits");
            }
        }

        //Blended Limits Green Upper Value
        public string BlendedWhiteGreenULimits
        {
            get { return Model.BlendedWhiteGreenULimits; }
            set
            {
                if (value != null)
                {
                    Model.BlendedWhiteGreenULimits = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("BlendedWhiteGreenULimits");
            }
        }

        //Blended Limits Blue Lower Value
        public string BlendedWhiteBlueLLimits
        {
            get { return Model.BlendedWhiteBlueLLimits; }
            set
            {
                if (value != null)
                {
                    Model.BlendedWhiteBlueLLimits = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("BlendedWhiteBlueLLimits");
            }
        }

        //Blended Limits Blue Upper Value
        public string BlendedWhiteBlueULimits
        {
            get { return Model.BlendedWhiteBlueULimits; }
            set
            {
                if (value != null)
                {
                    Model.BlendedWhiteBlueULimits = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("BlendedWhiteBlueULimits");
            }
        }
        #endregion

        #region Blue Limits Lower and upper Value
        public string BLRGBBLUELValue
        {
            get { return Model.BLRGBBLUELValue; }
            set
            {
                if (value != null)
                {
                    Model.BLRGBBLUELValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("BLRGBBLUELValue");
            }
        }
        public string BlueYMinValue
        {
            get { return Model.BlueYMinValue; }
            set
            {
                if (value != null)
                {
                    Model.BlueYMinValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("BlueYMinValue");
            }
        }

        public string BlueYMaxValue
        {
            get { return Model.BlueYMaxValue; }
            set
            {
                if (value != null)
                {
                    Model.BlueYMaxValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("BlueYMaxValue");
            }


        }
        //Blue Limits RGB Blue Upper Value
        public string BLRGBBLUEUValue
        {

            get { return Model.BLRGBBLUEUValue; }
            set
            {
                if (value != null)
                {
                    Model.BLRGBBLUEUValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("BLRGBBLUEUValue");
            }
        }
        #endregion

        #region Red Limit Lower and upper Value
        public string RLRGBREDLValue
        {
            get { return Model.RLRGBREDLValue; }
            set
            {
                if (value != null)
                {
                    Model.RLRGBREDLValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("RLRGBREDLValue");
            }
        }

        public string RedYMinValues
        {
            get { return Model.RedYMinValues; }
            set
            {
                if (value != null)
                {
                    Model.RedYMinValues = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("RedYMinValues");
            }
        }

        public string RedYMaxValue
        {
            get { return Model.RedYMaxValue; }
            set
            {
                if (value != null)
                {
                    Model.RedYMaxValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("RedYMaxValue");
            }
        }
        //Red Limit RGB red Upper Value

        public string RLRGBREDUValue
        {
            get { return Model.RLRGBREDUValue; }
            set
            {
                if (value != null)
                {
                    Model.RLRGBREDUValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("RLRGBREDUValue");
            }
        }

        public string RedMicroWattValue
        {
            get { return Model.RedMicroWattValue; }
            set
            {
                if (value != null)
                {
                    Model.RedMicroWattValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("RedMicroWattValue");
            }
        }

        public string RedMicroWattValueUpper
        {
            get { return Model.RedMicroWattValueUpper; }
            set
            {
                if (value != null)
                {
                    Model.RedMicroWattValueUpper = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("RedMicroWattValueUpper");
            }
        }
        #endregion

        #region Magenta Lower and upper Value
        public string MAGENTAMAXVALUE
        {
            get { return Model.MAGENTAMAXVALUE; }
            set
            {
                if (value != null)
                {
                    Model.MAGENTAMAXVALUE = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("MAGENTAMAXVALUE");
            }
        }

        // Magenta Red upper Value
        public string MagentaXValue1
        {
            get { return Model.MagentaXValue1; }
            set
            {
                if (value != null)
                {
                    Model.MagentaXValue1 = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("MagentaXValue1");
            }
        }

        // Magenta Green Lower Value
        public string MAGENTAYMINVALUE
        {
            get { return Model.MAGENTAYMINVALUE; }
            set
            {
                if (value != null)
                {
                    Model.MAGENTAYMINVALUE = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("MAGENTAYMINVALUE");
            }
        }

        // Magenta Green upper Value
        public string MAGENTAYMAXVALUE
        {
            get { return Model.MAGENTAYMAXVALUE; }
            set
            {
                if (value != null)
                {
                    Model.MAGENTAYMAXVALUE = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("MAGENTAYMAXVALUE");
            }
        }

        // Magenta Blue Lower Value
        public string MAGENTALBLUELValue
        {
            get { return Model.MAGENTALBLUELValue; }
            set
            {
                if (value != null)
                {
                    Model.MAGENTALBLUELValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("MAGENTALBLUELValue");
            }
        }

        // Magenta Blue Upper Value
        public string MAGENTALBLUEUValue
        {
            get { return Model.MAGENTALBLUEUValue; }
            set
            {
                if (value != null)
                {
                    Model.MAGENTALBLUEUValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("MAGENTALBLUEUValue");
            }
        }

        public string MagentaMicroWattValue
        {
            get { return Model.MagentaMicroWattValue; }
            set
            {
                if (value != null)
                {
                    Model.MagentaMicroWattValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("MagentaMicroWattValue");
            }
        }

        public string MagentaMicroWattValueUpper
        {
            get { return Model.MagentaMicroWattValueUpper; }
            set
            {
                if (value != null)
                {
                    Model.MagentaMicroWattValueUpper = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("MagentaMicroWattValueUpper");
            }
        }
        #endregion

        #region LED Temperation derating values
        // LED Temperature Derating White Value
        public string LedTempDeratingWhiteValue
        {
            get { return Model.LedTempDeratingWhiteValue; }
            set
            {
                if (value != null)
                {
                    Model.LedTempDeratingWhiteValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("LedTempDeratingWhiteValue");
            }
        }

        // LED Temperature Derating Red Value
        public string LedTempDeratingRedValue
        {
            get { return Model.LedTempDeratingRedValue; }
            set
            {
                if (value != null)
                {
                    Model.LedTempDeratingRedValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("LedTempDeratingRedValue");
            }
        }

        // LED Temperature Derating Green Value
        public string LedTempDeratingGreenValue
        {
            get { return Model.LedTempDeratingGreenValue; }
            set
            {
                if (value != null)
                {
                    Model.LedTempDeratingGreenValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("LedTempDeratingGreenValue");
            }
        }

        // LED Temperature Derating Blue Value
        public string LedTempDeratingBlueValue
        {
            get { return Model.LedTempDeratingBlueValue; }
            set
            {
                if (value != null)
                {
                    Model.LedTempDeratingBlueValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("LedTempDeratingBlueValue");
            }
        }

        public string BlueMicroWattValue
        {
            get { return Model.BlueMicroWattValue; }
            set
            {
                if (value != null)
                {
                    Model.BlueMicroWattValue = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("BlueMicroWattValue");
            }
        }

        public string BlueMicroWattValueUpper
        {
            get { return Model.BlueMicroWattValueUpper; }
            set
            {
                if (value != null)
                {
                    Model.BlueMicroWattValueUpper = value;
                }
                else
                {
                    //read default value and set it
                }

                OnPropertyChanged("BlueMicroWattValueUpper");
            }
        }
        #endregion

        #region Fixture Id
        //Fixture Id
        public string FixtureId
        {
            get { return Model.FixtureId; }
            set
            {
                if (value != null)
                {
                    Model.FixtureId = value;
                }
                else
                {
                    //read default value and set it
                }
                OnPropertyChanged("FixtureId");
            }
        }
        #endregion

        #region TestLevel- assembly or board level or Image
        [JsonProperty("_isimagetestchecked")]
        public bool IsImageTestingChecked
        {
            get { return Model.IsImageTestingChecked; }
            set
            {
                Model.IsImageTestingChecked = value;
                OnPropertyChanged("IsImageTestingChecked");
            }
        }
        #endregion

        #region save setting file- save button click
        //This method will launch a messagebox for us.
        internal void SaveDetailsToFile(Object p)
        {
            if (Model != null)
            {

                //no need to create  fileoperation object in the begining, create it when we need it
                if (_fileoperation == null)
                {
                    _fileoperation = new TestFixtureFileOperation();
                    _fileoperation.WriteSettingDataIntoJSon(Model);
                }
                else
                {
                    //what can be done in else :(..think and revisit later
                    _fileoperation.WriteSettingDataIntoJSon(Model);
                }
            }
            else
            {
                //MessageBox.Show("Please Entere  Data");
            }
        }
        #endregion
       
        
        #region save test sequence file- save button click

        internal void SaveEolDetailsToFile()
        {
            if (_eolmodel != null)
            {
                if (_fileoperation == null)
                {
                    _fileoperation = new TestFixtureFileOperation();
                    _fileoperation.WriteEolTestSequenceDetailsToJSon(_eolmodel);
                }
                else
                {
                    _fileoperation.WriteEolTestSequenceDetailsToJSon(_eolmodel);
                }
            }
            else
            {
                frmTestFixture.Instance.WriteToLog("'EOL' Model is null. Please contact IllumaVision support team to resolve issue...", ApplicationConstants.TraceLogType.Information);
            }
        }

        internal void SaveLightEngineDetailsToFile()
        {
            if (_lightenginemodel != null)
            {
                if (_fileoperation == null)
                {
                    _fileoperation = new TestFixtureFileOperation();
                    _fileoperation.WriteLightEngineTestSequenceDetailsToJSon(_lightenginemodel);
                }
                else
                {
                    _fileoperation.WriteLightEngineTestSequenceDetailsToJSon(_lightenginemodel);
                }
            }
            else
            {
                frmTestFixture.Instance.WriteToLog("'LIGHTENGINE' Model is null. Please contact IllumaVision support team to resolve issue...", ApplicationConstants.TraceLogType.Information);
            }
        }

        internal void SaveLineTesterDetailsToFile()
        {
            if (_lineTestermodel != null)
            {
                if (_fileoperation == null)
                {
                    _fileoperation = new TestFixtureFileOperation();
                    _fileoperation.WriteLineTesterTestSequenceDetailsToJSon(_lineTestermodel);
                }
                else
                {
                    _fileoperation.WriteLineTesterTestSequenceDetailsToJSon(_lineTestermodel);
                }
            }
            else
            {
                frmTestFixture.Instance.WriteToLog("'LINETESTER' Model is null. Please contact IllumaVision support team to resolve issue...", ApplicationConstants.TraceLogType.Information);
            }
        }
        #endregion


        #region topright mirror x values
        public string UpdatetrmirrorXvalues
        {
            get { return ImageModel.UpdatetrmirrorXvalues; }
            set
            {
                ImageModel.UpdatetrmirrorXvalues = value;
                OnPropertyChanged("UpdatetrmirrorXvalues");
            }
        }
        public string UpdatetrmirrorYvalues
        {
            get { return ImageModel.UpdatetrmirrorYvalues; }
            set
            {
                ImageModel.UpdatetrmirrorYvalues = value;
                OnPropertyChanged("UpdatetrmirrorYvalues");
            }

        }

        public string UpdatetLmirrorXvalues
        {
            get { return ImageModel.UpdatetLmirrorXvalues; }
            set
            {
                ImageModel.UpdatetLmirrorXvalues = value;
                OnPropertyChanged("UpdatetLmirrorXvalues");
            }
        }

        public string UpdatetLmirrorYvalues
        {
            get { return ImageModel.UpdatetLmirroryvalues; }
            set
            {
                ImageModel.UpdatetLmirroryvalues = value;
                OnPropertyChanged("UpdatetLmirroryvalues");
            }
        }

        public string UpdateBRMirrorXValues
        {
            get { return ImageModel.UpdateBRMirrorXValues; }
            set
            {
                ImageModel.UpdateBRMirrorXValues = value;
                OnPropertyChanged("UpdateBRMirrorXValues");
            }
        }

        public string UpdateBRMirrorYValues
        {
            get { return ImageModel.UpdateBRMirrorYValues; }
            set
            {
                ImageModel.UpdateBRMirrorYValues = value;
                OnPropertyChanged("UpdateBRMirrorYValues");
            }
        }

        public string UpdateBLMirrorXValues
        {
            get { return ImageModel.UpdateBLMirrorXValues; }
            set
            {
                ImageModel.UpdateBLMirrorXValues = value;
                OnPropertyChanged("UpdateBLMirrorXValues");
            }
        }

        public string UpdateBLMirrorYValues
        {
            get { return ImageModel.UpdateBLMirrorYValues; }
            set
            {
                ImageModel.UpdateBLMirrorYValues = value;
                OnPropertyChanged("UpdateBLMirrorYValues");
            }
        }
        public string UpdateHomeScreenXValues
        {
            get { return ImageModel.UpdateHomeScreenXValues; }
            set
            {
                ImageModel.UpdateHomeScreenXValues = value;
                OnPropertyChanged("UpdateHomeScreenXValues");
            }
        }

        public string UpdateHomeScreenYValues
        {
            get { return ImageModel.UpdateHomeScreenYValues; }
            set
            {
                ImageModel.UpdateHomeScreenYValues = value;
                OnPropertyChanged("UpdateHomeScreenYValues");
            }
        }
        #region save setting file- save button click

        //This property is how we link it to the View.
        public ICommand SaveImgDetails
        {
            get
            {
                return new RelayCommand(CanExecute, SaveImgDetailsToFile);
            }
        }
        //This method will launch a messagebox for us.
        internal void SaveImgDetailsToFile(Object p)
        {
            if (ImageModel != null)
            {
                //no need to create  fileoperation object in the begining, create it when we need it
                if (_fileoperation == null)
                {
                    _fileoperation = new TestFixtureFileOperation();
                    _fileoperation.WriteImgDetailsToJSon(ImageModel);
                }
                else
                {
                    //what can be done in else :(..think and revisit later
                    _fileoperation.WriteImgDetailsToJSon(ImageModel);
                }

            }
            else
            {
                //MessageBox.Show("Please Entere  Data");

            }
        }
        #endregion

        #region Button "Fixed Red" click - diagnostic model
        public ICommand TurnOnnReadRedValues
        {
            get
            {
                return new RelayCommand(CanExecute, GetRedValueDetails);
            }

        }

        //// on click of "Fixed Red" button, sets the values
        //internal void GetRedValueDetails(Object p)
        //{
        //    bool flag = false;
        //    string rtmsg;

        //    if (!_mRedFlag)
        //    {

        //        // YYYY
        //        flag = CheckForSpectrometer();
        //        if (!flag)
        //        {
        //            //UpdateTestResultPassFailStatus(false, "Acquiring Dark Spectrum Failed");
        //            ErrorMsgDisplay = "Spectrometer not detected";
        //            return;
        //        }
        //        rtmsg = AcquireDarkSpectrum();
        //        if (!rtmsg.Equals("NoErrors"))
        //        {
        //            //UpdateTestResultPassFailStatus(false, "Acquiring Dark Spectrum Failed");
        //            return;
        //        }
        //        rtmsg = _mTestFixtureSpectrum.ReadCalibrationFile();
        //        if (!rtmsg.Equals("NoErrors"))
        //        {
        //            ErrorMsgDisplay = rtmsg.ToString();
        //            return;
        //        }

        //        //CommandToTurnOnRedLedAndRedValues();
        //        CommandToTurnOnRedLedAndRedValuesFix();
        //        _mRedFlag = true;
        //    }
        //    else
        //    {
        //        //CommandToTurnOffLeds();
        //        _mRedFlag = false;
        //    }
        //}

        // on click of "Fixed Red" button, sets the values
        internal void GetRedValueDetails(Object p)
        {
            bool flag = false;
            string rtmsg;

            // YYYY
            flag = CheckForSpectrometer();
            if (!flag)
            {
                //UpdateTestResultPassFailStatus(false, "Acquiring Dark Spectrum Failed");
                ErrorMsgDisplay = "Spectrometer not detected";
                return;
            }
            //rtmsg = AcquireDarkSpectrum();
            //if (!rtmsg.Equals("NoErrors"))
            //{
            //    //UpdateTestResultPassFailStatus(false, "Acquiring Dark Spectrum Failed");
            //    return;
            //}

            ////rtmsg = _mTestFixtureSpectrum.ReadCalibrationFile();
            //TestFixtureReadSpectrometer calibration = new TestFixtureReadSpectrometer();

            //rtmsg = calibration.ReadCalibratedFile(CalibrationFileName);
            //if (!rtmsg.Equals("NoErrors"))
            //{
            //    ErrorMsgDisplay = rtmsg.ToString();
            //    return;
            //}

            //CommandToTurnOnRedLedAndRedValues();
            CommandToTurnOnRedLedAndRedValuesFix();
        }

        public bool TurnOnRedLightCommand()
        {
            bool flag = true;

            this.ErrorMsgDisplay = "Turning on Red LED to read values...";
            frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);

            try
            {
                if (serverStatus.Equals(interrupted))
                {
                    return false;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    return false;
                }
                else
                {
                    string redledcomm = TestFixtureCommands._mredledcommd;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm.ResetRetryCount();
                    serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(redledcomm);
                    if (serverStatus.Equals(connected))
                    {
                        this.ErrorMsgDisplay = "Sucessfully Turned on Red Led and Reading values...";
                        frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);

                        Thread.Sleep(2000);//because ocean optics to take some time 

                        return flag;
                    }
                    else if (serverStatus.Equals(notConnected))
                    {
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("TurnOnRedLightCommand: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }

        public bool TurnOnGreenLightCommand()
        {
            bool flag = true;

            this.ErrorMsgDisplay = "Turning on Green LED to read values...";
            frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);

            try
            {
                if (serverStatus.Equals(interrupted))
                {
                    return false;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    return false;
                }
                else
                {
                    string greenledcomm = TestFixtureCommands._mgreenledcomm;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm.ResetRetryCount();
                    serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(greenledcomm);
                    if (serverStatus.Equals(connected))
                    {
                        this.ErrorMsgDisplay = "Sucessfully Turned on Green Led and Reading values...";
                        frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);

                        Thread.Sleep(2000);//because ocean optics to take some time 

                        return flag;
                    }
                    else if (serverStatus.Equals(notConnected))
                    {
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("TurnOnGreenLightCommand: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }

        public bool TurnOnBlueLightCommand()
        {
            bool flag = true;

            this.ErrorMsgDisplay = "Turning on Blue LED to read values...";
            frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);

            try
            {
                if (serverStatus.Equals(interrupted))
                {
                    return false;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    return false;
                }
                else
                {
                    string blueledcomm = TestFixtureCommands._mblueledcomm;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm.ResetRetryCount();
                    serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(blueledcomm);
                    if (serverStatus.Equals(connected))
                    {
                        this.ErrorMsgDisplay = "Sucessfully Turned on Blue Led and Reading values...";
                        frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);

                        Thread.Sleep(2000);//because ocean optics to take some time 

                        return flag;
                    }
                    else if (serverStatus.Equals(notConnected))
                    {
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("TurnOnBlueLightCommand: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }

        public bool TurnOnWhiteLightCommand()
        {
            bool flag = true;

            this.ErrorMsgDisplay = "Turning on White LED to read values...";
            frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);

            try
            {
                if (serverStatus.Equals(interrupted))
                {
                    return false;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    return false;
                }
                else
                {
                    string whiteledcomm = TestFixtureCommands._mwhiteledcomm;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm.ResetRetryCount();
                    serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(whiteledcomm);
                    if (serverStatus.Equals(connected))
                    {
                        this.ErrorMsgDisplay = "Sucessfully Turned on White Led and Reading values...";
                        frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);
                        Thread.Sleep(2000);//because ocean optics to take some time 

                        return flag;
                    }
                    else if (serverStatus.Equals(notConnected))
                    {
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("TurnOnWhiteLightCommand: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }

        public bool TurnOnMagendaLightCommand()
        {
            bool flag = true;

            this.ErrorMsgDisplay = "Turning on Magenda LED to read values...";
            frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);

            try
            {
                if (serverStatus.Equals(interrupted))
                {
                    return false;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    return false;
                }
                else
                {
                    string magendaledcomm = TestFixtureCommands._mmagendaledcomm;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm.ResetRetryCount();
                    serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(magendaledcomm);
                    if (serverStatus.Equals(connected))
                    {
                        this.ErrorMsgDisplay = "Sucessfully Turned on Magenda Led and Reading values...";
                        frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);

                        Thread.Sleep(2000);//because ocean optics to take some time 

                        return flag;
                    }
                    else if (serverStatus.Equals(notConnected))
                    {
                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("TurnOnMagendaLightCommand: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }

        public bool TurnOnBlendedWhiteLightCommand()
        {
            bool flag = true;

            this.ErrorMsgDisplay = "Turning on Blended White LED to read values...";
            frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);

            try
            {
                if (serverStatus.Equals(interrupted))
                {
                    return false;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    return false;
                }
                else
                {
                    string blendedWhiteledcomm = TestFixtureCommands._mblendedwhiteledcomm;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm.ResetRetryCount();
                    serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(blendedWhiteledcomm);
                    if (serverStatus.Equals(connected))
                    {
                        this.ErrorMsgDisplay = "Sucessfully Turned on Blended White Led and Reading values...";
                        frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);

                        Thread.Sleep(2000);//because ocean optics to take some time 

                        return flag;
                    }
                    else if (serverStatus.Equals(notConnected))
                    {
                        this.ErrorMsgDisplay = "Blended White: " + serverStatus;
                        frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Warning);

                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("TurnOnBlendedWhiteLightCommand: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }

        private bool CommandToTurnOnRedLedAndRedValuesFix()
        {
            bool flag = true;

            this.ErrorMsgDisplay = "Turning on Red LED to read values...";
            
            try
            {
                
                //serverStatus = CheckForExistanceofIPAddress();
                if (TestFixtureSocketComm.serverStatus.Equals(interrupted))
                {
                    //SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                    //OnPropertyChanged("SetLowerLimitValues1");

                    //SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                    //OnPropertyChanged("SetLowerUpperValues1");

                    //SetUpperXValue = "0";
                    //OnPropertyChanged("SetUpperXValue");

                    //SetUpperYvalue = "0";
                    //OnPropertyChanged("SetUpperYValue");

                    //SetWattValue = 0;
                    //OnPropertyChanged("SetWattValue");
                    //// write test data in log file
                    //// WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(true, "Execution Stopped");
                    //ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else if (TestFixtureSocketComm.serverStatus.Equals(notConnected))
                {
                    //SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                    //OnPropertyChanged("SetLowerLimitValues1");

                    //SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                    //OnPropertyChanged("SetLowerUpperValues1");

                    //SetUpperXValue = "0";
                    //OnPropertyChanged("SetUpperXValue");

                    //SetUpperYvalue = "0";
                    //OnPropertyChanged("SetUpperYValue");

                    //SetWattValue = 0;
                    //OnPropertyChanged("SetWattValue");
                    // write test data in log file
                    //if (openFile)
                    //    WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(false, "Failed to Obtain Server");
                    //ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else
                {
                    // server available
                    //string rtmsg = _mTestFixtureSpectrum.ReadCalibrationFile();
                    //if (!rtmsg.Equals("NoErrors"))
                    //{
                    //    ErrorMsgDisplay = rtmsg.ToString();
                    //    return false;
                    //}

                    string redledcomm = TestFixtureCommands._mredledcommd;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm.ResetRetryCount();
                    serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(redledcomm);
                    if (serverStatus.Equals(connected))
                    {
                        this.ErrorMsgDisplay = "Sucessfuly Turned on Red Led and Reading values...";
     
                        Thread.Sleep(2000);//because ocean optics to take some time 

                        //_mTestFixtureSpectrum.AcquireSampleSpectrum();
                        //_mTestFixtureSpectrum.ComputeEnergySpectrum();

                        //SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                        //OnPropertyChanged("SetLowerLimitValues1");

                        //SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                        //OnPropertyChanged("SetLowerUpperValues1");

                        //SetUpperXValue = _mTestFixtureSpectrum.ReadUpperXValue();
                        //OnPropertyChanged("SetUpperXValue");

                        //SetUpperYvalue = _mTestFixtureSpectrum.ReadUpperYValue();
                        //OnPropertyChanged("SetUpperYValue");

                        //SetWattValue = _mTestFixtureSpectrum.ReadWattValue();
                        //OnPropertyChanged("SetWattValue");
                        //TurnOffSpectrometer();
                        return flag;
                    }
                    else if (TestFixtureSocketComm.serverStatus.Equals(notConnected))
                    {
                        //SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                        //OnPropertyChanged("SetLowerLimitValues1");

                        //SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                        //OnPropertyChanged("SetLowerUpperValues1");

                        //SetUpperXValue = "0";
                        //OnPropertyChanged("SetUpperXValue");

                        //SetUpperYvalue = "0";
                        //OnPropertyChanged("SetUpperYValue");

                        //SetWattValue = 0;
                        //OnPropertyChanged("SetWattValue");
                        return false;
                    }
                    else
                    {
                        //SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                        //OnPropertyChanged("SetLowerLimitValues1");

                        //SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                        //OnPropertyChanged("SetLowerUpperValues1");

                        //SetUpperXValue = "0";
                        //OnPropertyChanged("SetUpperXValue");

                        //SetUpperYvalue = "0";
                        //OnPropertyChanged("SetUpperYValue");

                        //SetWattValue = 0;
                        //OnPropertyChanged("SetWattValue");
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("CommandToTurnOnRedLedAndRedValuesFix: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }

        private bool CommandToTurnOnRedLedAndRedValues()
        {
            bool flag = true;

            this.ErrorMsgDisplay = "Turning on Red LED to read values...";

            try
            {
                //serverStatus = CheckForExistanceofIPAddress();
                if (TestFixtureSocketComm.serverStatus.Equals(interrupted))
                {
                    // write test data in log file
                    // WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(true, "Execution Stopped");
                    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
      
                    return false;
                }
                else if (TestFixtureSocketComm.serverStatus.Equals(notConnected))
                {
                    // write test data in log file
                    if (openFile)
                        WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(false, "Failed to Obtain Server");
                    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
 
                    return false;
                }
                else
                {
                    // server available
                }

                string rtmsg = _mTestFixtureSpectrum.ReadCalibrationFile();
                if (!rtmsg.Equals("NoErrors"))
                {
                    ErrorMsgDisplay = rtmsg.ToString();

                    return false;
                }

                string redledcomm = TestFixtureCommands._mredledcommd;
                TestFixtureSocketComm._isInnerloop = false;
                TestFixtureSocketComm._isInnerloop = false;
                TestFixtureSocketComm.ResetRetryCount();
                serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(redledcomm);
                if (serverStatus.Equals(connected))
                {
                    _mRedFlag = true;
                    this.ErrorMsgDisplay = "Sucessfuly Turned on Red Led and Reading values...";

                    // Commented for Demo
                    Thread.Sleep(2000);//because ocean optics to take some time 

                    _mTestFixtureSpectrum.AcquireSampleSpectrum();
                    _mTestFixtureSpectrum.ComputeEnergySpectrum();

                    SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                    OnPropertyChanged("SetLowerLimitValues1");

                    SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                    OnPropertyChanged("SetLowerUpperValues1");

                    SetUpperXValue = _mTestFixtureSpectrum.ReadUpperXValue();
                    OnPropertyChanged("SetUpperXValue");

                    SetUpperYvalue = _mTestFixtureSpectrum.ReadUpperYValue();
                    OnPropertyChanged("SetUpperYValue");

                    SetWattValue = _mTestFixtureSpectrum.ReadWattValue();
                    OnPropertyChanged("SetWattValue");

                    RedSpectrumValidation();

                    return flag;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("CommandToTurnOnRedLedAndRedValues: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }

        public bool SpectrumValidation(Common.ApplicationConstants.SpectrumColors spectrumColor)
        {
            bool flag = true;

            if (spectrumColor == ApplicationConstants.SpectrumColors.Red)
            {
                flag = RedSpectrumValidation();

                if(!flag)
                 return flag;
            }

            if (spectrumColor == ApplicationConstants.SpectrumColors.Blue)
            {
                flag = BlueSpectrumValidation();
                if (!flag)
                    return flag;
            }

            if (spectrumColor == ApplicationConstants.SpectrumColors.Green)
            {
                flag = GreenSpectrumValidation();
                if (!flag)
                    return flag;
            }

            if (spectrumColor == ApplicationConstants.SpectrumColors.White)
            {
                flag = WhiteSpectrumValidation();
                if (!flag)
                    return flag;
            }

            if (spectrumColor == ApplicationConstants.SpectrumColors.BlendedWhite)
            {
                flag = BlendedWhiteSpectrumValidation();
                if (!flag)
                    return flag;
            }

            if (spectrumColor == ApplicationConstants.SpectrumColors.Magenda)
            {
                flag = MagendaSpectrumValidation();
                if (!flag)
                    return flag;
            }

            return flag;
        }

        public bool RedSpectrumValidation()
        {
            double redXValue = Convert.ToDouble(SetUpperXValue);
            double redYValue = Convert.ToDouble(SetUpperYvalue);
            double wattValue = Convert.ToDouble(SetWattValue);
            double redXMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtRedXMinValues.Text);
            double redXMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtRedXMaxValues.Text);
            double redYMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtRedYMinValues.Text);
            double redYMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtRedYMaxValues.Text);
            double redWattMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtRedMicroWattValueLower.Text);
            double redWattMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtRedMicroWattValueUpper.Text);

            string msg = string.Empty;

            if (redXValue < redXMinLimit || redXValue > redXMaxLimit)
            {
                msg = string.Format("Red Spectrum reading 'X:{0}' out of range", redXValue);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }

            if (redYValue < redYMinLimit || redYValue > redYMaxLimit)
            {
                msg = string.Format("Red Spectrum reading 'Y:{0}' out of range", redYValue);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }

            if (wattValue < redWattMinLimit || wattValue > redWattMaxLimit)
            {
                msg = string.Format("Red Spectrum reading 'Watt:{0}' out of range", wattValue);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }


            msg = "Red Spectrum reading in range...";
            frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Green);
            frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
            frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Information);
            return true;
        }

        public bool BlueSpectrumValidation()
        {
            double blueXValue = Convert.ToDouble(SetUpperXValue);
            double blueYValue = Convert.ToDouble(SetUpperYvalue);
            double wattValue = Convert.ToDouble(SetWattValue);
            double blueXMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtBLUELValue.Text);
            double blueXMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtBLUEUValue.Text);
            double blueYMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtBlueYMinValue.Text);
            double blueYMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtBlueYMaxValue.Text);
            double WattMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtBlueMicroWattValueLower.Text);
            double WattMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtBlueMicroWattValueUpper.Text);

            string msg = string.Empty;

            if (blueXValue < blueXMinLimit || blueXValue > blueXMaxLimit)
            {
                msg = string.Format("Blue Spectrum reading 'X:{0}' out of range", blueXValue);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }

            if (blueYValue < blueYMinLimit || blueYValue > blueYMaxLimit)
            {
                msg = string.Format("Blue Spectrum reading 'Y:{0}' out of range", blueYValue);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }

            if (wattValue < WattMinLimit || wattValue > WattMaxLimit)
            {
                msg = string.Format("Blue Spectrum reading 'Watt:{0}' out of range", wattValue);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }


            msg = "Blue Spectrum reading in range...";
            frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Green);
            frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
            frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Information);
            return true;
        }

        public bool GreenSpectrumValidation()
        {
            double greenXValue = Convert.ToDouble(SetUpperXValue);
            double greenYValue = Convert.ToDouble(SetUpperYvalue);
            double wattValue = Convert.ToDouble(SetWattValue);
            double greenXMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtGREENLValue.Text);
            double greenXMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtGREENUValue.Text);
            double greenYMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtGreenYMinValues.Text);
            double greenYMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtGreenYMaxValues.Text);
            double greenWattMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtGreenMicroWattValueLower.Text);
            double greenWattMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtGreenMicroWattValueUpper.Text);

            string msg = string.Empty;

            if (greenXValue < greenXMinLimit || greenXValue > greenXMaxLimit)
            {
                msg = string.Format("Green Spectrum reading 'X:{0}' out of range", greenXValue);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }

            if (greenYValue < greenYMinLimit || greenYValue > greenYMaxLimit)
            {
                msg = string.Format("Green Spectrum reading 'Y:{0}' out of range", greenYValue);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }

            if (wattValue < greenWattMinLimit || wattValue > greenWattMaxLimit)
            {
                msg = string.Format("Green Spectrum reading 'Watt:{0}' out of range", wattValue);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }

            msg = "Green Spectrum reading in range...";
            frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Green);
            frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
            frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Information);
            return true;
        }

        public bool WhiteSpectrumValidation()
        {
            double whiteXValue = Convert.ToDouble(SetUpperXValue);
            double whiteYValue = Convert.ToDouble(SetUpperYvalue);
            double wattValue = Convert.ToDouble(SetWattValue);
            double whiteXMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtWhiteLimitXMinValue.Text);
            double whiteXMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtWhiteLimitXMaxValue.Text);
            double whiteYMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtWhiteLimitYMinValue.Text);
            double whiteYMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtWhiteLimitYMaxValue.Text);
            double whiteWattMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtWhiteMicroWattValueLower.Text);
            double whiteWattMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtWhiteMicroWattValueUpper.Text);

            string msg = string.Empty;

            if (whiteXValue < whiteXMinLimit || whiteXValue > whiteXMaxLimit)
            {
                msg = string.Format("White Spectrum reading 'X:{0}' out of range", whiteXValue);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }

            if (whiteYValue < whiteYMinLimit || whiteYValue > whiteYMaxLimit)
            {
                msg = string.Format("White Spectrum reading 'Y:{0}' out of range", whiteYValue);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }

            if (wattValue < whiteWattMinLimit || wattValue > whiteWattMaxLimit)
            {
                msg = string.Format("White Spectrum reading 'Watt:{0}' out of range", wattValue);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }


            msg = "White Spectrum reading in range...";
            frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Green);
            frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
            frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Information);
            return true;
        }


        public bool BlendedWhiteSpectrumValidation()
        {
            double BlendedWhiteXValue = Convert.ToDouble(SetUpperXValue);
            double BlendedWhiteYValue = Convert.ToDouble(SetUpperYvalue);
            double wattValue = Convert.ToDouble(SetWattValue);
            double BlendedWhiteXMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtBlendedWhiteRedLLimits.Text);
            double BlendedWhiteXMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtBlendedWhiteRedULimits.Text);
            double BlendedWhiteYMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtBlendedWhiteGreenLLimits.Text);
            double BlendedWhiteYMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtBlendedWhiteGreenULimits.Text);
            double BlendedWhiteWattMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtBlendedWhiteBlueLLimits.Text);
            double BlendedWhiteWattMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtBlendedWhiteBlueULimits.Text);

            string msg = string.Empty;

            if (BlendedWhiteXValue < BlendedWhiteXMinLimit || BlendedWhiteXValue > BlendedWhiteXMaxLimit)
            {
                msg = string.Format("Blended White Spectrum reading 'X:{0}' out of range", BlendedWhiteXValue);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }

            if (BlendedWhiteYValue < BlendedWhiteYMinLimit || BlendedWhiteYValue > BlendedWhiteYMaxLimit)
            {
                msg = string.Format("Blended White Spectrum reading 'Y:{0}' out of range", BlendedWhiteYValue);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }

            if (wattValue < BlendedWhiteWattMinLimit || wattValue > BlendedWhiteWattMaxLimit)
            {
                msg = string.Format("Blended White Spectrum reading 'Watt:{0}' out of range", wattValue);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }


            msg = "Blended White Spectrum reading in range...";
            frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Green);
            frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
            frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Information);
            return true;
        }


        public bool MagendaSpectrumValidation()
        {
            double magendaXValue = Convert.ToDouble(SetUpperXValue);
            double magendaYValue = Convert.ToDouble(SetUpperYvalue);
            double wattValue = Convert.ToDouble(SetWattValue);
            double magendaXMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtMagendaXMinLimit.Text);
            double magendaXMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtMagendaXMaxLimit.Text);
            double magendaYMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtMagendaYMinLimit.Text);
            double magendaYMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtMagendaYMaxLimit.Text);
            double magendaWattMinLimit = Convert.ToDouble(frmTestFixture.Instance.txtMagendaWattMinLimit.Text);
            double magendaWattMaxLimit = Convert.ToDouble(frmTestFixture.Instance.txtMagendaWattMaxLimit.Text);

            string msg = string.Empty;

            if (magendaXValue < magendaXMinLimit || magendaXValue > magendaXMaxLimit)
            {
                msg = string.Format("Magenda Spectrum reading 'X:{0}' out of range", magendaXValue);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }

            if (magendaYValue < magendaYMinLimit || magendaYValue > magendaYMaxLimit)
            {
                msg = string.Format("Magenda Spectrum reading 'Y:{0}' out of range", magendaYValue);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }

            if (wattValue < magendaWattMinLimit || wattValue > magendaWattMaxLimit)
            {
                msg = string.Format("Magenda Spectrum reading 'Watt:{0}' out of range", wattValue);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
                frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);
                //throw new SpectrumReadingOutOfRangeException(msg);
                return false;
            }


            msg = "Magenda Spectrum reading in range...";
            frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Green);
            frmTestFixture.Instance.SetErrorMessageDisplayTextBox(msg);
            frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Information);
            return true;
        }
        #endregion

        #region sets the lower and upper values in diagnostic screen
        public string SetLowerLimitValues1
        {
            get { return DiagnosticsModel.SetLowerLimitValues1; }
            set
            {
                DiagnosticsModel.SetLowerLimitValues1 = value;
                OnPropertyChanged("SetLowerLimitValues1");
            }
        }

        public string SetUpperXValue//SetLowerLimitValues2
        {
            get { return DiagnosticsModel.SetUpperXValue; }

            set
            {
                DiagnosticsModel.SetUpperXValue = value;

                if (frmTestFixture.Instance != null)
                    frmTestFixture.Instance.SetUpperXValueTextbox(DiagnosticsModel.SetUpperXValue);

                OnPropertyChanged("SetUpperXValue");
            }
        }

        public string SetLowerUpperValues1
        {
            get { return DiagnosticsModel.SetLowerUpperValue1; }
            set
            {
                DiagnosticsModel.SetLowerUpperValue1 = value;
                OnPropertyChanged("SetLowerUpperValues1");
            }
        }
        public string SetUpperYvalue//SetLowerUpperValues2
        {
            get { return DiagnosticsModel.SetUpperYValue; }
            set
            {
                DiagnosticsModel.SetUpperYValue = value;

                if (frmTestFixture.Instance != null)
                    frmTestFixture.Instance.SetUpperYValueTextbox(DiagnosticsModel.SetUpperYValue);

                OnPropertyChanged("SetUpperYValue");
            }
        }

        public double SetWattValue
        {
            get { return DiagnosticsModel.SetWattValue; }
            set
            {
                DiagnosticsModel.SetWattValue = value;

                if (frmTestFixture.Instance != null)
                    frmTestFixture.Instance.SetWattTextbox(DiagnosticsModel.SetWattValue.ToString());

                OnPropertyChanged("SetWattValue");
            }
        }

        #endregion

        #region Button "Fixed Green" click - diagnostic model
        public ICommand TurnOnnReadGreenValues
        {
            get
            {
                return new RelayCommand(CanExecute, ReadGreenValuesDetailsFromSpectrum);
            }

        }
        // on click of "Fixed Red" button, sets the values
        internal void ReadGreenValuesDetailsFromSpectrum(Object p)
        {
            bool flag = false;
            string rtmsg;

            flag = CheckForSpectrometer();
            if (!flag)
            {
                //UpdateTestResultPassFailStatus(false, "Acquiring Dark Spectrum Failed");
                ErrorMsgDisplay = "Spectrometer not detected";
                return;
            }
            rtmsg = AcquireDarkSpectrum();
            if (!rtmsg.Equals("NoErrors"))
            {
                //UpdateTestResultPassFailStatus(false, "Acquiring Dark Spectrum Failed");
                ErrorMsgDisplay = "Acquiring Dark Spectrum Failed";
                return;
            }
            //_mTestFixtureSpectrum.ReadCalibrationFile();

            //TestFixtureReadSpectrometer calibration = new TestFixtureReadSpectrometer();

            //rtmsg = calibration.ReadCalibratedFile(CalibrationFileName);
            //if (!rtmsg.Equals("NoErrors"))
            //{
            //    ErrorMsgDisplay = rtmsg.ToString();
            //    return;
            //}


            // CommandGreenLedToTurnOnAndReadValues();
            CommandGreenLedToTurnOnAndReadValuesFix();
            //_mGreenFlag = true;
        }

        private bool CommandGreenLedToTurnOnAndReadValuesFix()
        {
            bool flag = true;
            this.ErrorMsgDisplay = "Turning on Green LED to read values...";
            frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);

            try
            {
                //serverStatus = CheckForExistanceofIPAddress();
                if (serverStatus.Equals(interrupted))
                {
                    SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                    OnPropertyChanged("SetLowerLimitValues1");

                    SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                    OnPropertyChanged("SetLowerUpperValues1");

                    SetUpperXValue = "0";
                    OnPropertyChanged("SetUpperXValue");

                    SetUpperYvalue = "0";
                    OnPropertyChanged("SetUpperYValue");

                    SetWattValue = 0;
                    OnPropertyChanged("SetWattValue");
                    return false;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                    OnPropertyChanged("SetLowerLimitValues1");

                    SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                    OnPropertyChanged("SetLowerUpperValues1");

                    SetUpperXValue = "0";
                    OnPropertyChanged("SetUpperXValue");

                    SetUpperYvalue = "0";
                    OnPropertyChanged("SetUpperYValue");

                    SetWattValue = 0;
                    OnPropertyChanged("SetWattValue");
                    return false;
                }
                else
                {
                    string grnledcomm = TestFixtureCommands._mgreenledcomm;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm.ResetRetryCount();

                    serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess(grnledcomm);
                    if (serverStatus.Equals(connected))
                    {
                        //_mGreenFlag = true;
                        this.ErrorMsgDisplay = "Sucessfuly Turned on Green Led and Reading values...";
                        // Commented For Demo 
                        /*
                        string rtmsg = _mTestFixtureSpectrum.ReadCalibrationFile();
                        if (!rtmsg.Equals("NoErrors"))
                        {
                            ErrorMsgDisplay = rtmsg.ToString();
                            return false;
                        }
                        */

                        Thread.Sleep(3000);//because ocean optics to take some time 

                        _mTestFixtureSpectrum.AcquireSampleSpectrum();
                        _mTestFixtureSpectrum.ComputeEnergySpectrum();
                        /*
                        double lowerxvalue = _mTestFixtureSpectrum.ReadLowerXValue();
                        double loweryvalue = _mTestFixtureSpectrum.ReadLowerYValue();
                        double upperxvalue = _mTestFixtureSpectrum.ReadUpperXValue();
                        double upperyvalue = _mTestFixtureSpectrum.ReadUpperYValue();

                        SetLowerLimitValues1 = lowerxvalue;
                        OnPropertyChanged("SetLowerLimitValues1");
                        SetLowerUpperValues1 = upperxvalue;
                        OnPropertyChanged("SetLowerUpperValues1");
                        SetLowerLimitValues2 = loweryvalue;
                        OnPropertyChanged("SetLowerLimitValues2");
                        SetLowerUpperValues2 = upperyvalue;
                        OnPropertyChanged("SetLowerUpperValues2");
                        */
                        SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                        OnPropertyChanged("SetLowerLimitValues1");

                        SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                        OnPropertyChanged("SetLowerUpperValues1");

                        SetUpperXValue = _mTestFixtureSpectrum.ReadUpperXValue();
                        OnPropertyChanged("SetUpperXValue");

                        SetUpperYvalue = _mTestFixtureSpectrum.ReadUpperYValue();
                        OnPropertyChanged("SetUpperYValue");

                        SetWattValue = _mTestFixtureSpectrum.ReadWattValue();
                        OnPropertyChanged("SetWattValue");
                        TurnOffSpectrometer();
                        return flag;
                    }
                    else if (serverStatus.Equals(notConnected))
                    {
                        SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                        OnPropertyChanged("SetLowerLimitValues1");

                        SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                        OnPropertyChanged("SetLowerUpperValues1");

                        SetUpperXValue = "0";
                        OnPropertyChanged("SetUpperXValue");

                        SetUpperYvalue = "0";
                        OnPropertyChanged("SetUpperYValue");

                        SetWattValue = 0;
                        OnPropertyChanged("SetWattValue");
                        return false;
                    }
                    else
                    {
                        SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                        OnPropertyChanged("SetLowerLimitValues1");

                        SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                        OnPropertyChanged("SetLowerUpperValues1");

                        SetUpperXValue = "0";
                        OnPropertyChanged("SetUpperXValue");

                        SetUpperYvalue = "0";
                        OnPropertyChanged("SetUpperYValue");

                        SetWattValue = 0;
                        OnPropertyChanged("SetWattValue");
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                // return false;
                flag = false;

                frmTestFixture.Instance.WriteToLog("CommandGreenLedToTurnOnAndReadValuesFix: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }


        private bool CommandGreenLedToTurnOnAndReadValues()
        {
            bool flag = true;
            this.ErrorMsgDisplay = "Turning on Green LED to read values...";

            try
            {
                serverStatus = CheckForExistanceofIPAddress();
                if (serverStatus.Equals(interrupted))
                {
                    // write test data in log file
                    // WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(true, "Execution Stopped");
                    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    // write test data in log file
                    if (openFile)
                        WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(false, "Failed to Obtain Server");
                    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else
                {
                    // server available
                }

                string grnledcomm = TestFixtureCommands._mgreenledcomm;
                TestFixtureSocketComm._isInnerloop = false;
                TestFixtureSocketComm._isInnerloop = false;
                TestFixtureSocketComm.ResetRetryCount();

                serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess(grnledcomm);
                if (serverStatus.Equals(connected))
                {
                    //_mGreenFlag = true;
                    this.ErrorMsgDisplay = "Sucessfuly Turned on Green Led and Reading values...";
                    // Commented For Demo 
                    /*
                    string rtmsg = _mTestFixtureSpectrum.ReadCalibrationFile();
                    if (!rtmsg.Equals("NoErrors"))
                    {
                        ErrorMsgDisplay = rtmsg.ToString();
                        return false;
                    }
                    */
                    Thread.Sleep(3000);//because ocean optics to take some time 

                    _mTestFixtureSpectrum.AcquireSampleSpectrum();
                    _mTestFixtureSpectrum.ComputeEnergySpectrum();
                    /*
                    double lowerxvalue = _mTestFixtureSpectrum.ReadLowerXValue();
                    double loweryvalue = _mTestFixtureSpectrum.ReadLowerYValue();
                    double upperxvalue = _mTestFixtureSpectrum.ReadUpperXValue();
                    double upperyvalue = _mTestFixtureSpectrum.ReadUpperYValue();

                    SetLowerLimitValues1 = lowerxvalue;
                    OnPropertyChanged("SetLowerLimitValues1");
                    SetLowerUpperValues1 = upperxvalue;
                    OnPropertyChanged("SetLowerUpperValues1");
                    SetLowerLimitValues2 = loweryvalue;
                    OnPropertyChanged("SetLowerLimitValues2");
                    SetLowerUpperValues2 = upperyvalue;
                    OnPropertyChanged("SetLowerUpperValues2");
                    */
                    SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                    OnPropertyChanged("SetLowerLimitValues1");

                    SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                    OnPropertyChanged("SetLowerUpperValues1");

                    SetUpperXValue = _mTestFixtureSpectrum.ReadUpperXValue();
                    OnPropertyChanged("SetUpperXValue");

                    SetUpperYvalue = _mTestFixtureSpectrum.ReadUpperYValue();
                    OnPropertyChanged("SetUpperYValue");

                    SetWattValue = _mTestFixtureSpectrum.ReadWattValue();
                    OnPropertyChanged("SetWattValue");

                    return flag;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("CommandGreenLedToTurnOnAndReadValues: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }
        #endregion

        #region button 'Fixed Blue' click event - diagnostic model
        public ICommand TurnOnnReadBlueValues
        {
            get
            {
                return new RelayCommand(CanExecute, ReadBlueXYDetailsFromSpectrum);
            }
        }
        internal void ReadBlueXYDetailsFromSpectrum(Object p)
        {
            bool flag = false;
            string rtmsg;


            flag = CheckForSpectrometer();
            if (!flag)
            {

                // UpdateTestResultPassFailStatus(false, "Acquiring Dark Spectrum Failed");
                return;
            }
            rtmsg = AcquireDarkSpectrum();
            if (!rtmsg.Equals("NoErrors"))
            {
                // UpdateTestResultPassFailStatus(false, "Acquiring Dark Spectrum Failed");
                return;
            }
            //_mTestFixtureSpectrum.ReadCalibrationFile();

            //TestFixtureReadSpectrometer calibration = new TestFixtureReadSpectrometer();

            //rtmsg = calibration.ReadCalibratedFile(CalibrationFileName);
            //if (!rtmsg.Equals("NoErrors"))
            //{
            //    ErrorMsgDisplay = rtmsg.ToString();
            //    return;
            //}

            // CommandBlueLedToTurnOnAndReadValues();
            CommandBlueLedToTurnOnAndReadValuesFix();
        }

        private bool CommandBlueLedToTurnOnAndReadValuesFix()
        {
            bool flag = true;
            this.ErrorMsgDisplay = "Turning on Blue LED to read values...";
            frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);

            try
            {
                //serverStatus = CheckForExistanceofIPAddress();
                if (serverStatus.Equals(interrupted))
                {
                    SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                    OnPropertyChanged("SetLowerLimitValues1");

                    SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                    OnPropertyChanged("SetLowerUpperValues1");

                    SetUpperXValue = "0";
                    OnPropertyChanged("SetUpperXValue");

                    SetUpperYvalue = "0";
                    OnPropertyChanged("SetUpperYValue");

                    SetWattValue = 0;
                    OnPropertyChanged("SetWattValue");
                    //// write test data in log file
                    //// WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(true, "Execution Stopped");
                    //ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                    OnPropertyChanged("SetLowerLimitValues1");

                    SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                    OnPropertyChanged("SetLowerUpperValues1");

                    SetUpperXValue = "0";
                    OnPropertyChanged("SetUpperXValue");

                    SetUpperYvalue = "0";
                    OnPropertyChanged("SetUpperYValue");

                    SetWattValue = 0;
                    OnPropertyChanged("SetWattValue");
                    return false;
                }
                else
                {
                    string blueledcomm = TestFixtureCommands._mblueledcomm;

                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm.ResetRetryCount();

                    serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess(blueledcomm);
                    if (serverStatus.Equals(connected))
                    {
                        // _mGreenFlag = true;
                        this.ErrorMsgDisplay = "Sucessfuly Turned on Blue Led and Reading values...";

                        Thread.Sleep(3000);//because ocean optics to take some time 

                        _mTestFixtureSpectrum.AcquireSampleSpectrum();
                        _mTestFixtureSpectrum.ComputeEnergySpectrum();
                        /*
                        double lowerxvalue = _mTestFixtureSpectrum.ReadLowerXValue();
                        double loweryvalue = _mTestFixtureSpectrum.ReadLowerYValue();
                        double upperxvalue = _mTestFixtureSpectrum.ReadUpperXValue();
                        double upperyvalue = _mTestFixtureSpectrum.ReadUpperYValue();

                        SetLowerLimitValues1 = lowerxvalue;
                        OnPropertyChanged("SetLowerLimitValues1");
                        SetLowerUpperValues1 = upperxvalue;
                        OnPropertyChanged("SetLowerUpperValues1");
                        SetLowerLimitValues2 = loweryvalue;
                        OnPropertyChanged("SetLowerLimitValues2");
                        SetLowerUpperValues2 = upperyvalue;
                        OnPropertyChanged("SetLowerUpperValues2");
                        */
                        SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                        OnPropertyChanged("SetLowerLimitValues1");

                        SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                        OnPropertyChanged("SetLowerUpperValues1");

                        SetUpperXValue = _mTestFixtureSpectrum.ReadUpperXValue();
                        OnPropertyChanged("SetUpperXValue");

                        SetUpperYvalue = _mTestFixtureSpectrum.ReadUpperYValue();
                        OnPropertyChanged("SetUpperYValue");

                        SetWattValue = _mTestFixtureSpectrum.ReadWattValue();
                        OnPropertyChanged("SetWattValue");
                        TurnOffSpectrometer();
                        return flag;
                    }
                    else if (serverStatus.Equals(notConnected))
                    {
                        SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                        OnPropertyChanged("SetLowerLimitValues1");

                        SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                        OnPropertyChanged("SetLowerUpperValues1");

                        SetUpperXValue = "0";
                        OnPropertyChanged("SetUpperXValue");

                        SetUpperYvalue = "0";
                        OnPropertyChanged("SetUpperYValue");

                        SetWattValue = 0;
                        OnPropertyChanged("SetWattValue");
                        return false;
                    }
                    else
                    {
                        SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                        OnPropertyChanged("SetLowerLimitValues1");

                        SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                        OnPropertyChanged("SetLowerUpperValues1");

                        SetUpperXValue = "0";
                        OnPropertyChanged("SetUpperXValue");

                        SetUpperYvalue = "0";
                        OnPropertyChanged("SetUpperYValue");

                        SetWattValue = 0;
                        OnPropertyChanged("SetWattValue");
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                // return false;
                flag = false;
                frmTestFixture.Instance.WriteToLog("CommandBlueLedToTurnOnAndReadValuesFix: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }


        private bool CommandBlueLedToTurnOnAndReadValues()
        {
            bool flag = true;
            this.ErrorMsgDisplay = "Turning on Blue LED to read values...";

            try
            {
                //serverStatus = CheckForExistanceofIPAddress();
                if (serverStatus.Equals(interrupted))
                {
                    // write test data in log file
                    // WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(true, "Execution Stopped");
                    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    // write test data in log file
                    if (openFile)
                        WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(false, "Failed to Discover Server");
                    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else
                {
                    // server available
                }

                string blueledcomm = TestFixtureCommands._mblueledcomm;

                TestFixtureSocketComm._isInnerloop = false;
                TestFixtureSocketComm._isInnerloop = false;
                TestFixtureSocketComm.ResetRetryCount();

                serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess(blueledcomm);
                if (serverStatus.Equals(connected))
                {
                    _mBlueFlag = true;
                    this.ErrorMsgDisplay = "Sucessfuly Turned on Blue Led and Reading values...";

                    // Commented For Demo 
                    /*
                    string rtmsg = _mTestFixtureSpectrum.ReadCalibrationFile();
                    if (!rtmsg.Equals("NoErrors"))
                    {
                        ErrorMsgDisplay = rtmsg.ToString();
                        return false;
                    }
                    */
                    Thread.Sleep(3000);//because ocean optics to take some time 

                    _mTestFixtureSpectrum.AcquireSampleSpectrum();
                    _mTestFixtureSpectrum.ComputeEnergySpectrum();
                    /*
                    double lowerxvalue = _mTestFixtureSpectrum.ReadLowerXValue();
                    double loweryvalue = _mTestFixtureSpectrum.ReadLowerYValue();
                    double upperxvalue = _mTestFixtureSpectrum.ReadUpperXValue();
                    double upperyvalue = _mTestFixtureSpectrum.ReadUpperYValue();

                    SetLowerLimitValues1 = lowerxvalue;
                    OnPropertyChanged("SetLowerLimitValues1");
                    SetLowerUpperValues1 = upperxvalue;
                    OnPropertyChanged("SetLowerUpperValues1");
                    SetLowerLimitValues2 = loweryvalue;
                    OnPropertyChanged("SetLowerLimitValues2");
                    SetLowerUpperValues2 = upperyvalue;
                    OnPropertyChanged("SetLowerUpperValues2");
                    */
                    SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                    OnPropertyChanged("SetLowerLimitValues1");

                    SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                    OnPropertyChanged("SetLowerUpperValues1");

                    SetUpperXValue = _mTestFixtureSpectrum.ReadUpperXValue();
                    OnPropertyChanged("SetUpperXValue");

                    SetUpperYvalue = _mTestFixtureSpectrum.ReadUpperYValue();
                    OnPropertyChanged("SetUpperYValue");

                    SetWattValue = _mTestFixtureSpectrum.ReadWattValue();
                    OnPropertyChanged("SetWattValue");

                    return flag;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("CommandBlueLedToTurnOnAndReadValues: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }
        #endregion

        #region Button  'Magenta RGB' click event - diagnostic model
        public ICommand GetFixedMagentaValues
        {
            get
            {
                return new RelayCommand(CanExecute, SetLowerUpperMagentaLimitsValues);
            }
        }

        private void SetLowerUpperMagentaLimitsValues(Object p)
        {
        }
        #endregion

        #region  Button 'Blended White' button click event - diagnostic model
        public ICommand TurnOnnReadWhiteValues
        {
            get
            {
                return new RelayCommand(CanExecute, ReadWhiteDetailsFromSpectrum);
            }
        }
        internal void ReadWhiteDetailsFromSpectrum(Object p)
        {
            bool flag = false;
            string rtmsg;

            //if (!_mWhiteFlag)
            //{
                flag = CheckForSpectrometer();
                if (!flag)
                {
                    //UpdateTestResultPassFailStatus(false, "Acquiring Dark Spectrum Failed");
                    return;
                }
                rtmsg = AcquireDarkSpectrum();
                if (!rtmsg.Equals("NoErrors"))
                {
                    // UpdateTestResultPassFailStatus(false, "Acquiring Dark Spectrum Failed");
                    return;
                }
               // _mTestFixtureSpectrum.ReadCalibrationFile();

            //TestFixtureReadSpectrometer calibration = new TestFixtureReadSpectrometer();

            //rtmsg = calibration.ReadCalibratedFile(CalibrationFileName);
            //if (!rtmsg.Equals("NoErrors"))
            //{
            //    ErrorMsgDisplay = rtmsg.ToString();
            //    return;
            //}


            //CommandToTurnOnWhiteLedAndReadValues();
            CommandToTurnOnWhiteLedAndReadValuesFix();
                //_mWhiteFlag = true;
            //}
            //else
            //{
            //    //CommandToTurnOffLeds();
            //    _mWhiteFlag = false;
            //}
        }

        private bool CommandToTurnOnWhiteLedAndReadValuesFix()
        {
            bool flag = true;
            this.ErrorMsgDisplay = "Turning on White LED to read values...";
            frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);

            try
            {
                //serverStatus = CheckForExistanceofIPAddress();
                if (serverStatus.Equals(interrupted))
                {
                    SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                    OnPropertyChanged("SetLowerLimitValues1");

                    SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                    OnPropertyChanged("SetLowerUpperValues1");

                    SetUpperXValue = "0";
                    OnPropertyChanged("SetUpperXValue");

                    SetUpperYvalue = "0";
                    OnPropertyChanged("SetUpperYValue");

                    SetWattValue = 0;
                    OnPropertyChanged("SetWattValue");
                    //// write test data in log file
                    //// WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(true, "Execution Stopped");
                    //ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                    OnPropertyChanged("SetLowerLimitValues1");

                    SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                    OnPropertyChanged("SetLowerUpperValues1");

                    SetUpperXValue = "0";
                    OnPropertyChanged("SetUpperXValue");

                    SetUpperYvalue = "0";
                    OnPropertyChanged("SetUpperYValue");

                    SetWattValue = 0;
                    OnPropertyChanged("SetWattValue");
                    //// write test data in log file
                    //if (openFile)
                    //    WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(false, "Failed to Obtain Server");
                    //ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else
                {
                    string whiteledcomm = TestFixtureCommands._mwhiteledcomm;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm._isInnerloop = false;
                    TestFixtureSocketComm.ResetRetryCount();

                    serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess(whiteledcomm);
                    if (serverStatus.Equals(connected))
                    {
                        // _mGreenFlag = true;
                        this.ErrorMsgDisplay = "Sucessfuly Turned on White Led and Reading values...";

                        Thread.Sleep(3000);//because ocean optics to take some time 

                        _mTestFixtureSpectrum.AcquireSampleSpectrum();
                        _mTestFixtureSpectrum.ComputeEnergySpectrum();
                        /*
                        double lowerxvalue = _mTestFixtureSpectrum.ReadLowerXValue();
                        double loweryvalue = _mTestFixtureSpectrum.ReadLowerYValue();
                        double upperxvalue = _mTestFixtureSpectrum.ReadUpperXValue();
                        double upperyvalue = _mTestFixtureSpectrum.ReadUpperYValue();

                        SetLowerLimitValues1 = lowerxvalue;
                        OnPropertyChanged("SetLowerLimitValues1");
                        SetLowerUpperValues1 = upperxvalue;
                        OnPropertyChanged("SetLowerUpperValues1");
                        SetLowerLimitValues2 = loweryvalue;
                        OnPropertyChanged("SetLowerLimitValues2");
                        SetLowerUpperValues2 = upperyvalue;
                        OnPropertyChanged("SetLowerUpperValues2");
                        */
                        SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                        OnPropertyChanged("SetLowerLimitValues1");

                        SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                        OnPropertyChanged("SetLowerUpperValues1");

                        SetUpperXValue = _mTestFixtureSpectrum.ReadUpperXValue();
                        OnPropertyChanged("SetUpperXValue");

                        SetUpperYvalue = _mTestFixtureSpectrum.ReadUpperYValue();
                        OnPropertyChanged("SetUpperYValue");

                        SetWattValue = _mTestFixtureSpectrum.ReadWattValue();
                        OnPropertyChanged("SetWattValue");
                        TurnOffSpectrometer();
                        return flag;
                    }
                    else if (serverStatus.Equals(notConnected))
                    {
                        SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                        OnPropertyChanged("SetLowerLimitValues1");

                        SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                        OnPropertyChanged("SetLowerUpperValues1");

                        SetUpperXValue = "0";
                        OnPropertyChanged("SetUpperXValue");

                        SetUpperYvalue = "0";
                        OnPropertyChanged("SetUpperYValue");

                        SetWattValue = 0;
                        OnPropertyChanged("SetWattValue");
                        return false;
                    }
                    else
                    {
                        SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                        OnPropertyChanged("SetLowerLimitValues1");

                        SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                        OnPropertyChanged("SetLowerUpperValues1");

                        SetUpperXValue = "0";
                        OnPropertyChanged("SetUpperXValue");

                        SetUpperYvalue = "0";
                        OnPropertyChanged("SetUpperYValue");

                        SetWattValue = 0;
                        OnPropertyChanged("SetWattValue");
                        return false;
                    }
                }
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                // return false;
                flag = false;
                frmTestFixture.Instance.WriteToLog("CommandToTurnOnWhiteLedAndReadValuesFix: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }


        private bool CommandToTurnOnWhiteLedAndReadValues()
        {
            bool flag = true;
            try
            {
                serverStatus = CheckForExistanceofIPAddress();
                if (serverStatus.Equals(interrupted))
                {
                    // write test data in log file
                    // WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(true, "Execution Stopped");
                    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    // write test data in log file
                    if (openFile)
                        WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(false, "Failed to Obtain Server");
                    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else
                {
                    // server available
                }

                string whiteledcomm = TestFixtureCommands._mwhiteledcomm;
                TestFixtureSocketComm._isInnerloop = false;
                TestFixtureSocketComm._isInnerloop = false;
                TestFixtureSocketComm.ResetRetryCount();

                serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess(whiteledcomm);
                if (serverStatus.Equals(connected))
                {
                    _mWhiteFlag = true;
                    this.ErrorMsgDisplay = "Sucessfuly Turned on White Led...";
 
                    // Commented For Demo 
                    /*
                    string rtmsg = _mTestFixtureSpectrum.ReadCalibrationFile();
                    if (!rtmsg.Equals("NoErrors"))
                    {
                        ErrorMsgDisplay = rtmsg.ToString();
                        return false;
                    }
                    */
                    Thread.Sleep(3000);//because ocean optics to take some time 

                    _mTestFixtureSpectrum.AcquireSampleSpectrum();
                    _mTestFixtureSpectrum.ComputeEnergySpectrum();
                    /*
                    double lowerxvalue = _mTestFixtureSpectrum.ReadLowerXValue();
                    double loweryvalue = _mTestFixtureSpectrum.ReadLowerYValue();
                    double upperxvalue = _mTestFixtureSpectrum.ReadUpperXValue();
                    double upperyvalue = _mTestFixtureSpectrum.ReadUpperYValue();

                    SetLowerLimitValues1 = lowerxvalue;
                    OnPropertyChanged("SetLowerLimitValues1");
                    SetLowerUpperValues1 = upperxvalue;
                    OnPropertyChanged("SetLowerUpperValues1");
                    SetLowerLimitValues2 = loweryvalue;
                    OnPropertyChanged("SetLowerLimitValues2");
                    SetLowerUpperValues2 = upperyvalue;
                    OnPropertyChanged("SetLowerUpperValues2");
                    */
                    SetLowerLimitValues1 = _mTestFixtureSpectrum.ReadLowerXValue();
                    OnPropertyChanged("SetLowerLimitValues1");

                    SetLowerUpperValues1 = _mTestFixtureSpectrum.ReadLowerYValue();
                    OnPropertyChanged("SetLowerUpperValues1");

                    SetUpperXValue = _mTestFixtureSpectrum.ReadUpperXValue();
                    OnPropertyChanged("SetUpperXValue");

                    SetUpperYvalue = _mTestFixtureSpectrum.ReadUpperYValue();
                    OnPropertyChanged("SetUpperYValue");

                    SetWattValue = _mTestFixtureSpectrum.ReadWattValue();
                    OnPropertyChanged("SetWattValue");

                    return flag;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    // if (openFile)
                    //     WriteTestResultLog();
                    //// UpdateTestResultPassFailStatus(false, "Failed to Discover Server");
                    ErrorMsgDisplay = "Server Not Available";
                    return false;

                }
                else
                {
                    //UpdateTestResultPassFailStatus(true, "Execution Stopped");
                    ErrorMsgDisplay = "Execution Manually Stopped";
                    return false;
                }
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("CommandToTurnOnWhiteLedAndReadValues: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }

        internal bool CommandToTurnOffLeds()
        {
            bool flag = true;
            this.ErrorMsgDisplay = "Turning off LEDs...";
            frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);
            try
            {
                string switchoffcmd = TestFixtureCommands._mswitchofflightcommand;
                TestFixtureSocketComm._isInnerloop = false;
                TestFixtureSocketComm._isInnerloop = false;
                TestFixtureSocketComm.ResetRetryCount();

                serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess(switchoffcmd);
                if (serverStatus.Equals(notConnected))
                {
                    this.ErrorMsgDisplay = "Failed to turn off Led's...";
                    frmTestFixture.Instance.WriteToLog( ErrorMsgDisplay, ApplicationConstants.TraceLogType.Warning);
                    return false;
                }
                else if (serverStatus.Equals(connected))
                {
                    this.ErrorMsgDisplay = "Sucessfully Turn OFF LED's...";
                    frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);
                    //return true;
                }
                else
                {
                    this.ErrorMsgDisplay = "Unknown Server Status...";
                    frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Warning);

                    return false;
                }

            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("CommandToTurnOffLeds: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return flag;
        }

        private void TurnOffSpectrometer()
        {
            _mTestFixtureSpectrum.CloseSpectrometer();
        }
        #endregion

        #region Button 'Off' click event - diagnostic model
        public ICommand ClickOffButton
        {
            get
            {
                return new RelayCommand(CanExecute, OffClickEvent);
            }
        }
        private void OffClickEvent(Object p)
        {
            //perform this action...what?

        }
        #endregion

        #region read stem voltage button click event - diagnostic model
        /*
         * This function reads the voltage sensed from daq and sets the stem  voltage in diagnostic page
         * same function can be or api call is to make to set the 12v or 120v radion button in the same page
         * @param p: not used for any purpose ..right now its dummy
         */
        internal void Start120ACVRealyOn(Object p)
        {
            //if (!_m120flag)
            //{
                MessageBoxResult result = System.Windows.MessageBox.Show("Activating 120VAC Relay on", "Warning!", System.Windows.MessageBoxButton.OKCancel);

                if (result.Equals(MessageBoxResult.OK))
                {

                    float stemvoltage;
                    //all these methods are just place holder... need to call daaq api's
                    _mtestfixturedaq.TurnOn120VRelayOn(1);
                   // stemvoltage = _mtestfixturedaq.ReadOutVoltage();
                    //    _mtestfixturedaq.TurnOffVoltageOff();
                   // DiagnosticsModel.Setstemvoltage = stemvoltage.ToString("F2");
                    _m120flag = true;
                }
            //}
            //else
            //{
            //    _mtestfixturedaq.TurnOn120VRelayOn(0);
            //    _m120flag = false;
            //}

        }

        internal bool Start120ACVRelayOn()
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Activating 120VAC Relay on", "Warning!", System.Windows.MessageBoxButton.OKCancel);

            if (result.Equals(MessageBoxResult.OK))
            {
                //all these methods are just place holder... need to call daaq api's
                _mtestfixturedaq.TurnOn120VRelayOn(1);

                return true;
            }
            else
            {
                return false;
            }


        }

        internal void Start120ACVRelayOff()
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Activating 120VAC Relay Off", "Warning!", System.Windows.MessageBoxButton.OKCancel);

            if (result.Equals(MessageBoxResult.OK))
            {
                _mtestfixturedaq.TurnOn120VRelayOn(0);
            }
        }
        #endregion

        #region start relay controls
        public ICommand ReadImpedanceVoltage
        {
            get { return new RelayCommand(CanExecute, ReadImpedanceVolt); }

        }

        internal void ReadImpedanceVolt(Object p)
        {
            string impedanceStatus = string.Empty;

            frmTestFixture.Instance.pagevm._mtestfixturedaq.ConfigureDaqToDifferential();
            //TurnOn15ACVRelay(1);

            //Thread.Sleep(1000);

            float voltage = _mtestfixturedaq.ReadOutVoltage();
            ImpedanceVoltage = voltage;
            if (voltage >= 0.01 && voltage <= 1.7)
            {
                IsOneTwentyvRadioChecked = true;
                IsTweleveRadioChecked = false;
                IsVoltageOK = true;
                impedanceStatus = _model.ModelNumber120V50Ft;
            }
            else if (voltage >= 2.0 && voltage <= 3.5)
            {
                IsTweleveRadioChecked = true;
                IsOneTwentyvRadioChecked = false;
                IsVoltageOK = true;
                impedanceStatus = _model.ModelNumber12V50Ft;
            }
            else
            {
                IsOneTwentyvRadioChecked = false;
                IsTweleveRadioChecked = false;
                IsVoltageOK = false;
            }
        }


        internal string ReadImpedanceVolt()
        {
            string impedanceStatus = string.Empty;

            //TurnOn15ACVRelay(1);

            //Thread.Sleep(1000);

            float voltage = _mtestfixturedaq.ReadOutVoltage();
            ImpedanceVoltage = voltage;
            if (voltage >= 0.01 && voltage <= 0.5)
            {
                    IsOneTwentyvRadioChecked = true;
                    IsTweleveRadioChecked = false;
                    IsVoltageOK = true;
                    impedanceStatus = _model.ModelNumber120V50Ft;

            }
            else if (voltage >= 0.71 && voltage <= 2.5)
            {

                    IsTweleveRadioChecked = true;
                    IsOneTwentyvRadioChecked = false;
                    IsVoltageOK = true;
                    impedanceStatus = _model.ModelNumber12V50Ft;
            }
            else
            {
                    IsOneTwentyvRadioChecked = false;
                    IsTweleveRadioChecked = false;
                    IsVoltageOK = false;
            }

            return impedanceStatus;
        }


        //internal void ReadImpedanceVolt(Object p)
        //{
        //    //TurnOn15ACVRelay(1);

        //    //Thread.Sleep(1000);

        //    float voltage = _mtestfixturedaq.ReadOutVoltage();
        //    ImpedanceVoltage = voltage;
        //    if (voltage >= 2.5 && voltage <= 3.6)
        //    {
        //        IsOneTwentyvRadioChecked = true;
        //        IsTweleveRadioChecked = false;
        //        IsVoltageOK = true;
        //    }
        //    else if (voltage >= 0.9 && voltage <= 1.2)
        //    {
        //        IsTweleveRadioChecked = true;
        //        IsOneTwentyvRadioChecked = false;
        //        IsVoltageOK = true;
        //    }
        //    else
        //    {
        //        IsOneTwentyvRadioChecked = false;
        //        IsTweleveRadioChecked = false;
        //        IsVoltageOK = false;
        //    }

        //}


        public ICommand ReadTemperatureSensor
        {

            get { return new RelayCommand(CanExecute, ReadTempSensorValue); }
        }
        internal void ReadTempSensorValue(Object p)
        {
            double voltage = _mtestfixturedaq.ReadAmbbientTemperature(); // Commented on 03-02-2017
            ReadAmbientTemp = voltage;
        }
        public ICommand Start120ACRelayOn
        {
            get { return new RelayCommand(CanExecute, Start120ACVRealyOn); }
        }


        internal void TurnOn15ACVRelay()
        {

            MessageBoxResult result = System.Windows.MessageBox.Show("Activating 15VAC Relay on", "Warning!", System.Windows.MessageBoxButton.OKCancel);
            if (result.Equals(MessageBoxResult.OK))
            {
                string returnvalue = _mtestfixturedaq.TurnOn15VRelayOn(1);
                if (returnvalue.Equals("NoErrors"))
                    this.ErrorMsgDisplay = "15ACV Relay On...";
                else
                    this.ErrorMsgDisplay = returnvalue;
            }
        }

        internal void TurnOff15ACVRelay()
        {

            MessageBoxResult result = System.Windows.MessageBox.Show("Activating 15VAC Relay off", "Warning!", System.Windows.MessageBoxButton.OKCancel);
            if (result.Equals(MessageBoxResult.OK))
            {
                string returnvalue = _mtestfixturedaq.TurnOn15VRelayOn(0);
                if (returnvalue.Equals("NoErrors"))
                    this.ErrorMsgDisplay = "15ACV Relay Off...";
                else
                    this.ErrorMsgDisplay = returnvalue;
            }
        }

        internal void ResetDaqBoardPort()
        {
            _mtestfixturedaq.ResetPort();
        }

        internal void PCBLoadSelOn()
        {
            if (!_mpclsflag)
            {
                string returnvalue = _mtestfixturedaq.PLCSolenoidPairing(1);
                if (returnvalue.Equals("NoErrors"))
                {
                    this.ErrorMsgDisplay = "PLC Pairing Sucessfully";
                    _mtestfixturedaq.TurnHighForPLCPairing(1);
                    _mpclsflag = true;
                }
                else
                {
                    this.ErrorMsgDisplay = returnvalue;
                    _mpclsflag = false;
                }
            }
            else
            {
                //_mtestfixturedaq.PLCSolenoidPairing(0);
                //_mtestfixturedaq.TurnLowForPLCPairing();
                //_mpclsflag = false;
            }
        }

        internal void PCBLoadSelOff(object p)
        {
            //if (!_mpclsflag)
            //{
                string returnvalue = _mtestfixturedaq.PLCSolenoidPairing(1);
                if (returnvalue.Equals("NoErrors"))
                {
                    this.ErrorMsgDisplay = "PLC Pairing Sucessfully";
                   // _mtestfixturedaq.PLCSolenoidPairing(0);
                    _mtestfixturedaq.TurnLowForPLCPairing();
                    _mpclsflag = false;
                }
                else
                {
                    this.ErrorMsgDisplay = returnvalue;
                    _mpclsflag = false;
                }
            //}
            //else
            //{
            //    _mtestfixturedaq.PLCSolenoidPairing(0);
            //    _mtestfixturedaq.TurnLowForPLCPairing();
            //    _mpclsflag = false;
            //}
        }

        public ICommand PairSolenoid
        {
            get { return new RelayCommand(CanExecute, PairSolenoidOnDevice); }
        }
        internal void PairSolenoidOnDevice(Object p)
        {
            if (!_mdhcpflag)
            {
                string returnvalue = _mtestfixturedaq.SendOnCommandToSolenoid(1);
                if (returnvalue.Equals("NoErrors"))
                {
                    this.ErrorMsgDisplay = "Solenoid Pairing Sucessfully";
                    _mdhcpflag = true;
                }
                else
                    this.ErrorMsgDisplay = returnvalue;
                
            }
            else
            {
                string returnvalue = _mtestfixturedaq.SendOnCommandToSolenoid(0);
                _mdhcpflag = false;
            }
        }

        internal void PairSolenoidOn(Object p)
        {
            string returnvalue = _mtestfixturedaq.SendOnCommandToSolenoidTest();
            if (returnvalue.Equals("NoErrors"))
            {
                this.ErrorMsgDisplay = "Solenoid Pairing on Sucessfully";
            }
            else
                this.ErrorMsgDisplay = returnvalue;
        }

        internal void PairSolenoidOff(Object p)
        {
            string returnvalue = _mtestfixturedaq.SendOffCommandToSolenoidTest();
            if (returnvalue.Equals("NoErrors"))
            {
                this.ErrorMsgDisplay = "Solenoid Pairing off Sucessfully";
            }
            else
                this.ErrorMsgDisplay = returnvalue;
        }

        #endregion

        public string SetTemperatureValue
        {
            get { return DiagnosticsModel.SetTemperatureValue; }
            set
            {
                DiagnosticsModel.SetTemperatureValue = value;
                OnPropertyChanged("SetTemperatureValue");
            }
        }


        #region 12v and 120v Radio button checked ??? - diagnostic model
        public bool IsTweleveRadioChecked
        {
            get { return DiagnosticsModel.IsTweleveRadioChecked; }
            set
            {
                DiagnosticsModel.IsTweleveRadioChecked = value;

                if (frmTestFixture.Instance != null)
                {
                    frmTestFixture.Instance.Set15VACRadioButton(DiagnosticsModel.IsTweleveRadioChecked);
                }

                OnPropertyChanged("IsTweleveRadioChecked");
            }
        }
        public bool IsOneTwentyvRadioChecked
        {
            get { return DiagnosticsModel.IsOneTwentyvRadioChecked; }

            set
            {
                DiagnosticsModel.IsOneTwentyvRadioChecked = value;

                if (frmTestFixture.Instance != null)
                {
                    frmTestFixture.Instance.Set120VACRadioButton(DiagnosticsModel.IsOneTwentyvRadioChecked);
                }
                OnPropertyChanged("IsOneTwentyvRadioChecked");
            }
        }

        #endregion

        #region DiagnosticsModel - Object Creation to OnpropertyChanged Event
        public TestFixtureDLUValueSetModel DiagnosticsModel
        {
            get { return _dluvaluesetmodel; }
            set
            {
                _dluvaluesetmodel = value;

                //if (ucHostContainer.Instance != null)

                OnPropertyChanged("DiagnosticsModel");
            }
        }
        public TestFixtureProjectorModel ProjectModel
        {
            get { return _mtestfixtureProjector; }
            set
            {
                _mtestfixtureProjector = value;

                //if (ucHostContainer.Instance != null)

                OnPropertyChanged("ProjectModel");
            }
        }

        public TestFixtureProjectorImageModel ImageModel
        {
            get { return _mTestProjectImgModel; }
            set
            {
                _mTestProjectImgModel = value;

                //if (ucHostContainer.Instance != null)

                    OnPropertyChanged("ImageModel");
            }
        }

        public float ImpedanceVoltage
        {
            get { return DiagnosticsModel.ImpedanceVoltage; }
            set
            {
                DiagnosticsModel.ImpedanceVoltage = value;

                if (frmTestFixture.Instance != null)
                {
                    frmTestFixture.Instance.SetImpedanceVoltageTextbox(DiagnosticsModel.ImpedanceVoltage.ToString());
                }
                OnPropertyChanged("ImpedanceVoltage");
            }
        }
        public double ReadAmbientTemp
        {
            get { return DiagnosticsModel.ReadAmbientTemp; }
            set
            {
                DiagnosticsModel.ReadAmbientTemp = value;

                if (frmTestFixture.Instance != null)
                {
                    frmTestFixture.Instance.SetAmbientTempTextbox(DiagnosticsModel.ReadAmbientTemp.ToString());
                }

                OnPropertyChanged("ReadAmbientTemp");
            }


        }


        #endregion

        #region Main Screen
        public TestFixtureMainModel MainModel
        {
            get { return _mainmodel; }
            set
            {
                _mainmodel = value;
                OnPropertyChanged("MainModel");
            }
        }

        public string TestDriveMain
        {
            get { return MainModel.Testdatbind; }
            set
            {
                MainModel.Testdatbind = value;
                OnPropertyChanged("TestDriveMain");
            }
        }

        #endregion


        #region loadimage button click event -Projector  Screen

        //This property allows us to change and receive changes made to the Model.
        [JsonProperty("_mtestfixtureProjector")]
        public TestFixtureProjectorModel ProjectorModel
        {
            get { return _mtestfixtureProjector; }
            set
            {
                _mtestfixtureProjector = value;
                OnPropertyChanged("ProjectorModel");
            }
        }

        #region Access Visibility
        //public bool CanAccessProjectorPage
        //{
        //    get { return ProjectorModel.CanAccessProjectorPage; }
        //}
        #endregion
        public ICommand LoadOriginalImage
        {
            get
            {
                return new RelayCommand(CanExecute, BrowseImageToLoad);
            }
        }

        internal void BrowseImageToLoad(Object p)
        {

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = "c:\\";
            dlg.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                         "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                         "Portable Network Graphic (*.png)|*.png";

            if (dlg.ShowDialog() == true)
            {
                LoadImageSource = new BitmapImage(new Uri(dlg.FileName));
                string strfileName = dlg.FileName;
                int index = strfileName.LastIndexOf("\\") + 1;
                int len = strfileName.Length;
                string extfilename = strfileName.Substring(index, len - index);
                int dotlen = extfilename.LastIndexOf(".");
                int totallen = extfilename.Length;

                OriginalImgName = extfilename.Substring(0, dotlen);
            }
        }

        internal bool BrowseToCalibrationFile(Object p)
        {
            OmniDriver.NETWrapper wrapper = new OmniDriver.NETWrapper();

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.InitialDirectory = "c:\\";
            dlg.Filter = "Calibration File|*.csv";

            wrapper.openAllSpectrometers();
            int numberOfSpectrometers = wrapper.getNumberOfSpectrometersFound();
            frmTestFixture.Instance.WriteToLog(string.Format("Found {0} spectrometers: ", numberOfSpectrometers),  ApplicationConstants.TraceLogType.Information);
            frmTestFixture.Instance.lblSpectrometer.Text = string.Format("Found {0} spectrometers: ", numberOfSpectrometers);

            if (numberOfSpectrometers < 1)
            {
                frmTestFixture.Instance.WriteToLog("WARNING: no spectrometers found", ApplicationConstants.TraceLogType.Warning);
                frmTestFixture.Instance.lblSpectrometer.Text = "no spectrometers found";
                return false;
            }

            string serialNo = "Spectrometer-Calibration_" + wrapper.getSerialNumber(0);

            if (dlg.ShowDialog() == true)
            {
                CalibrationFileName = string.Empty;

                string strfileName = CalibrationFileName = dlg.FileName;

                int index = strfileName.LastIndexOf("\\") + 1;
                int len = strfileName.Length;
                string extfilename = strfileName.Substring(index, len - index);
                int dotlen = extfilename.LastIndexOf(".");
                int totallen = extfilename.Length;

                OriginalImgName = extfilename.Substring(0, dotlen);

                if (!serialNo.Equals(OriginalImgName))
                    return false;

                frmTestFixture.Instance.WriteToLog(string.Format("Spectrometer found: '{0}'", serialNo), ApplicationConstants.TraceLogType.Information);

                return true;
            }
            else
            {
                frmTestFixture.Instance.WriteToLog("User cancelled OpenFileDialog...", ApplicationConstants.TraceLogType.Information);
            }

            return false;
        }


        internal bool GetCalibrationFile(string filename)
        {
            OmniDriver.NETWrapper wrapper = new OmniDriver.NETWrapper();

            wrapper.openAllSpectrometers();
            int numberOfSpectrometers = wrapper.getNumberOfSpectrometersFound();
            frmTestFixture.Instance.WriteToLog(string.Format("Found {0} spectrometers: ", numberOfSpectrometers), ApplicationConstants.TraceLogType.Information);
            frmTestFixture.Instance.lblSpectrometer.Text = string.Format("Found {0} spectrometers: {1} ", numberOfSpectrometers);

            if (numberOfSpectrometers < 1)
            {
                frmTestFixture.Instance.WriteToLog("WARNING: no spectrometers found", ApplicationConstants.TraceLogType.Warning);
                frmTestFixture.Instance.lblSpectrometer.Text = "WARNING: no spectrometers found";
                return false;
            }

            CalibrationFileName =  "Spectrometer-Calibration_" + wrapper.getSerialNumber(0); 

            string strfileName = CalibrationFileName;

            //string CalibrationFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + CalibrationFileName;

            int index = strfileName.LastIndexOf("\\") + 1;
            int len = strfileName.Length;
            string extfilename = strfileName.Substring(index, len - index);
            int dotlen = extfilename.LastIndexOf(".");
            int totallen = extfilename.Length;

            OriginalImgName = extfilename.Substring(0, dotlen);

            string serialNo = "Spectrometer-Calibration_" + wrapper.getSerialNumber(0) + ".csv";

            if (!serialNo.Equals(OriginalImgName))
                return false;

            frmTestFixture.Instance.WriteToLog(string.Format("Spectrometer found: '{0}'", serialNo), ApplicationConstants.TraceLogType.Information);

            return true;
        }

        public string OriginalImgName
        {
            get { return ImageModel.OriginalImgName; }
            set
            {

                ImageModel.OriginalImgName = value;
                OnPropertyChanged("OriginalImgName");
            }
        }

        public string CalibrationFileName { get; set; }


        public ICommand ProcessOriginalImage
        {
            get { return new RelayCommand(CanExecute, ProcessImageForData); }
        }

        internal void ProcessImageForData(object p)
        {
            ImageModel.ImageTaken();
            //string foldername = TestFixtureConstants.CreateDirectoryIfNotExists("Images");
            //string fullpath = foldername + "\\" + "OProcessedImg.png";
            //OpenFileDialog dlg = new OpenFileDialog();
            //dlg.FileName = fullpath;
            //ImageModel.LoadGrayImg = new BitmapImage(new Uri(dlg.FileName));
            // ImageModel.LoadConvertedImages();
        }

        public ImageSource ImageSourceSetting
        {
            get
            {
                return Model.ImageSource;
            }
            set
            {
                Model.ImageSource = value;
                OnPropertyChanged("ImageSourceSetting");
            }
        }
        public ImageSource LoadImageSource
        {
            get
            {
                return ImageModel.LoadImageSource;
            }
            set
            {
                ImageModel.LoadImageSource = value;
                OnPropertyChanged("LoadImageSource");
            }
        }


        public ImageSource ImageSource
        {
            get
            {
                return ProjectorModel.ImageSource;
            }
            set
            {
                ProjectorModel.ImageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        public ICommand ProjectorTest
        {
            get
            {
                return new RelayCommand(CanExecute, DummyTesting);
            }
        }

        internal void DummyTesting(Object p)
        {
            /*
             1. Get the loaded image
             * 2.Send it to the devie through device command or query the image from the device
             * 3.project it
             * 4.Take the photo of project image from USB camera
             * 5. Using Aforge.net , read the image to compare it with original image 
             */

            ProjectorModel.SetOriginalImg = _mTestProjectImgModel.GetOriginalImage();
            ProjectModel.ThreshHoldLimit = _mTestProjectImgModel.ThreshHoldSimilarity;
            ProjectorModel.ImageTaken();
        }
        //called to set value for Min in Histogram section
        #region ICommand implements - CanExecute Method
        private bool CanExecute()
        {
            bool canSavedata = Model.CanSaveData();
            if (canSavedata)
                return true;
            return false;
        }
        #endregion
        #endregion

        #region click on Start button on the application
        internal void StartTesting(Object p)
        {
            _mTestFixtureSpectrum.ReadCalibrationFile();
            //  StartExecutionOfMirrorCommands();
        }

        public void ResetStatusIndicator()
        {
            frmTestFixture.Instance.SetStatusIndicatorVisibleProperty(false);
            frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.White);
            frmTestFixture.Instance.SetStatusIndicatorTextBox(string.Empty);
        }

        //public void StartExecutionEvents()
        //{
        //    try
        //    {
        //        TestFixtureExecutionSequence seq = new TestFixtureExecutionSequence();
        //        string testseq = seq.GetTestSequnceName();
        //        bool flag = false;
        //        MinTestSteps = 0;
        //        ResetControlsandOthers();

        //        switch (testseq)
        //        {
        //            case "1": // execute all test starting from daq till led
        //                MaxTestSteps = 30;

        //                frmTestFixture.Instance.SetProgressStatusBarMax(MaxTestSteps);

        //                if (_lineTestermodel != null)
        //                {
        //                    if (_lineTestermodel.EOL)
        //                        flag = EolStartExecution();
        //                    else if (_lineTestermodel.EOL)
        //                        flag = EolStartExecution();
        //                    else
        //                    {
        //                        frmTestFixture.Instance.WriteToLog("_lineTestermodel is unknown. Please contact IllumaVision support team to resolve issue...", ApplicationConstants.TraceLogType.Error);
        //                    }
        //                }
        //                else
        //                {
        //                    frmTestFixture.Instance.WriteToLog("_lineTestermodel is null. Please contact IllumaVision support team to resolve issue...", ApplicationConstants.TraceLogType.Error);
        //                }

        //                break;
        //            case "2": // execute only daq
        //                MaxTestSteps = 12;

        //                frmTestFixture.Instance.SetProgressStatusBarMax(MaxTestSteps);

        //                flag = StartExecutionOfDaq();
        //                if (flag)
        //                    OverallTestResultPassFailStatus(true, "Test Completed Successfull");
        //                else
        //                    OverallTestResultPassFailStatus(false, "Test Completed failed");
        //                break;
        //            case "3": // execute only Mirror
        //                MaxTestSteps = 1;

        //                frmTestFixture.Instance.SetProgressStatusBarMax(MaxTestSteps);

        //                flag = StartExecutionOfMirrorCommands();
        //                if (flag)
        //                {
        //                    if (openFile)
        //                        WriteTestResultLog();
        //                    OverallTestResultPassFailStatus(true, "Test Completed Successfull");
        //                }
        //                else
        //                    OverallTestResultPassFailStatus(false, "Test Completed failed");
        //                break;
        //            case "4": // execute only Image... prerequesite mirror position has to be set
        //                flag = StartImageProcessingTest();
        //                if (flag)
        //                {
        //                    if (openFile)
        //                        WriteTestResultLog();
        //                    OverallTestResultPassFailStatus(true, "Test Completed Successfull");
        //                }
        //                else
        //                    OverallTestResultPassFailStatus(false, "Test Completed failed");
        //                break;
        //            case "5": //Led execution
        //                MaxTestSteps = 7;

        //                frmTestFixture.Instance.SetProgressStatusBarMax(MaxTestSteps);

        //                flag = StartLedLighTest();
        //                if (flag)
        //                {
        //                    if (openFile)
        //                        WriteTestResultLog();
        //                    OverallTestResultPassFailStatus(true, "Test Completed Successfull");
        //                }
        //                else
        //                    OverallTestResultPassFailStatus(false, "Test Completed failed");
        //                break;
        //            case "6":
        //                flag = FindServerIPAndHandShake();
        //                if (flag)
        //                {
        //                    if (openFile)
        //                        WriteTestResultLog();
        //                    OverallTestResultPassFailStatus(true, "Test Completed Successfull");
        //                }
        //                else
        //                    OverallTestResultPassFailStatus(false, "Test Completed failed");
        //                break;
        //            default:
        //                ErrorMsgDisplay = "Not valid TestSequnce.1-AllTest,2-Daq,3-Mirror,4-Image and 5-Led";
        //                break;
        //        }

        //        if (flag)
        //        {
        //            frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Green);
        //            frmTestFixture.Instance.SetStatusIndicatorVisibleProperty(true);
        //        }
        //        else
        //        {
        //            frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
        //            frmTestFixture.Instance.SetStatusIndicatorVisibleProperty(true);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
        //        ErrorMsgDisplay = e.Message.ToString();
        //        frmTestFixture.Instance.WriteToLog("StartExecutionEvents: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
        //    }
        //}

        public void StartExecutionEvents()
        {
            try
            {
                TestFixtureExecutionSequence seq = new TestFixtureExecutionSequence();
                string testseq = seq.GetTestSequnceName();
                bool flag = false;
                MinTestSteps = 0;
                ResetControlsandOthers();

                switch (testseq)
                {
                    case "1": // execute all test starting from daq till led
                        MaxTestSteps = 14;

                        frmTestFixture.Instance.SetProgressStatusBarMax(MaxTestSteps);

                        if (_lineTestermodel != null)
                        {
                            if (_lineTestermodel.EOL)
                                flag = EolStartExecution();
                            else if (_lineTestermodel.LightEngine)
                                flag = LightEngineStartExecution();
                            else
                            {
                                frmTestFixture.Instance.WriteToLog("_lineTestermodel is unknown. Please contact IllumaVision support team to resolve issue...", ApplicationConstants.TraceLogType.Error);
                            }
                        }
                        else
                        {
                            frmTestFixture.Instance.WriteToLog("_lineTestermodel is null. Please contact IllumaVision support team to resolve issue...", ApplicationConstants.TraceLogType.Error);
                        }

                        break;

                    default:
                        ErrorMsgDisplay = "Not valid TestSequnce.1-AllTest,2-Daq,3-Mirror,4-Image and 5-Led";
                        break;
                }

                if (flag)
                {
                    frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Green);
                    frmTestFixture.Instance.SetStatusIndicatorVisibleProperty(true);
                }
                else
                {
                    frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                    frmTestFixture.Instance.SetStatusIndicatorVisibleProperty(true);
                }
            }
            catch (Exception e)
            {
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                ErrorMsgDisplay = e.Message.ToString();
                frmTestFixture.Instance.WriteToLog("StartExecutionEvents: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            finally
            {
                
            }
        }

        private void ResetControlsandOthers()
        {
            ErrorMsgDisplay = String.Empty;
            UpdateSerialNumber = String.Empty;
            VersionNumber = String.Empty;
            UpdatePassFailResult = String.Empty;
            UpdateBackgroundColor = System.Windows.Media.Brushes.White;
            ReadModelNumber = String.Empty;
            IsVoltageOK = false;
            IsWhiteColorOk = false;
            IsGreenColorOk = false;
            IsBlueColorOk = false;
            IsRedColorOk = false;
            Reset();
        }

        private bool EolStartExecution()
        {
            bool flag = false;

            try
            {
                //frmTestFixture.Instance.ResetTreeView();

                frmTestFixture.Instance.SetSerialNumberTextbox("update serial number");
                frmTestFixture.Instance.SetModelNumberTextbox("update model number");
                frmTestFixture.Instance.SetVersionNumberTextbox("update version number");
                frmTestFixture.Instance.SetVoltageTextbox("update voltage number");

                frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("nodeEolTestSequence", true);

                #region Read Impedance Voltage Check
                if (_eolmodel.PowerOn)
                {
                    flag = StartEolReadImpedanceCheck();
                    if (!flag)
                    {
                        //WriteTestResultLog();
                        OverallTestResultPassFailStatus(false, "StartReadImpedanceCheck FAILED");
                        return flag;
                    }

                    Increment();
                }
                #endregion

                if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                    return false;

                #region Start Pairing Sequence   
                if (_eolmodel.Pairing)
                {
                    flag = StartPairingSequence();
                    if (!flag)
                    {
                        //WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PAIRING", false);
                        OverallTestResultPassFailStatus(false, "StartPairingSequence FAILED");
                        return flag;
                    }

                    Increment();

                    frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PAIRING", true);
                }
                #endregion

                if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                    return false;

                #region server Ip and HandShake

                if (_eolmodel.PentairServer)
                {
                    //flag = FindServerIPAndHandShake();
                    string sflag = TestFixtureSocketComm.DiscoverPentairServer(false);

                    if (sflag != "AVAILABLE")
                    {
                        LogStructureNew.DeviceDiscover = "FAIL";
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PENTAIR SERVER", false);
                        OverallTestResultPassFailStatus(false, "DiscoverPentairServer FAILED");
                        flag = false;
                        return flag;
                    }

                    LogStructureNew.DeviceDiscover = "PASS";
                    flag = true;
                    frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PENTAIR SERVER", true);

                    Increment();
                }
                #endregion

                if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                    return false;

                #region Firmware Version

                if (_eolmodel.FirmwareVersion)
                {
                    flag = GetFirmwareVersion();
                    if (!flag)
                    {
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("FIRMWARE VERSION", false);
                        OverallTestResultPassFailStatus(false, "GetFirmwareVersion FAILED");
                        return flag;
                    }

                    frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("FIRMWARE VERSION", true);

                    Increment();
                }
                #endregion

                if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                    return false;

                #region Mirror commands
                if (_eolmodel.ProjectorMirrorCheck)
                {
                    flag = StartMirror();
                    if (!flag)
                    {
                        LogStructureNew.MirrorVerify = "FAIL";
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PROJECTOR MIRROR CHECK", false);
                        OverallTestResultPassFailStatus(false, "StartExecutionOfMirrorCommands FAILED");
                        return flag;
                    }

                    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.TopRight);
                    if (!flag)
                    {
                        LogStructureNew.MirrorVerify = "FAIL";
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PROJECTOR MIRROR CHECK", false);
                        OverallTestResultPassFailStatus(false, "MirrorPosition.TopRight FAILED");
                        return flag;
                    }

                    ErrorMsgDisplay = "Mirror position to 'top right' completed, wait for 3 seconds...";
                    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

                    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.TopLeft);
                    if (!flag)
                    {
                        LogStructureNew.MirrorVerify = "FAIL";
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PROJECTOR MIRROR CHECK", false);
                        OverallTestResultPassFailStatus(false, "MirrorPosition.TopLeft FAILED");
                        return flag;
                    }

                    ErrorMsgDisplay = "Mirror position to 'top left' completed, wait for 3 seconds...";
                    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

                    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.BottomRight);
                    if (!flag)
                    {
                        LogStructureNew.MirrorVerify = "FAIL";
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PROJECTOR MIRROR CHECK", false);
                        OverallTestResultPassFailStatus(false, "MirrorPosition.BottomRight FAILED");
                        return flag;
                    }

                    ErrorMsgDisplay = "Mirror position to 'bottom right' completed, wait for 3 seconds...";
                    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

                    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.BottomLeft);
                    if (!flag)
                    {
                        LogStructureNew.MirrorVerify = "FAIL";
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PROJECTOR MIRROR CHECK", false);
                        OverallTestResultPassFailStatus(false, "MirrorPosition.BottomLeft FAILED");
                        return flag;
                    }

                    ErrorMsgDisplay = "Mirror position to 'bottom left' completed, wait for 3 seconds...";
                    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

                    flag = SendCommandToStopImageShow();
                    if (!flag)
                    {
                        LogStructureNew.MirrorVerify = "FAIL";
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PROJECTOR MIRROR CHECK", false);
                        ErrorMsgDisplay = "SendCommandToStopImageShow FAILED...";
                        OverallTestResultPassFailStatus(false, ErrorMsgDisplay);
                        return flag;
                    }

                    LogStructureNew.MirrorVerify = "PASS";
                    frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PROJECTOR MIRROR CHECK", true);

                    Increment();
                }
                #endregion

                if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                    return false;

                #region Spectrometer Test

                if (_eolmodel.LedBrightnessColor)
                {
                    flag = ExecuteSpectrumReadingMain(ApplicationConstants.SpectrumColors.Red);
                    if (!flag)
                    {
                        WriteTestResultLog();
                        LogStructureNew.LedTest = "FAIL";
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("LED BRIGHTNESS / COLOR", false);
                        OverallTestResultPassFailStatus(false, "Spectrum Color Red FAILED");
                        return flag;
                    }

                    flag = ExecuteSpectrumReadingMain(ApplicationConstants.SpectrumColors.Green);
                    if (!flag)
                    {
                        WriteTestResultLog();
                        LogStructureNew.LedTest = "FAIL";
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("LED BRIGHTNESS / COLOR", false);
                        OverallTestResultPassFailStatus(false, "Spectrum Color Green FAILED");
                        return flag;
                    }

                    flag = ExecuteSpectrumReadingMain(ApplicationConstants.SpectrumColors.Blue);
                    if (!flag)
                    {
                        WriteTestResultLog();
                        LogStructureNew.LedTest = "FAIL";
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("LED BRIGHTNESS / COLOR", false);
                        OverallTestResultPassFailStatus(false, "Spectrum Color Blue FAILED");
                        return flag;
                    }

                    flag = ExecuteSpectrumReadingMain(ApplicationConstants.SpectrumColors.White);
                    if (!flag)
                    {
                        WriteTestResultLog();
                        LogStructureNew.LedTest = "FAIL";
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("LED BRIGHTNESS / COLOR", false);
                        OverallTestResultPassFailStatus(false, "Spectrum Color White FAILED");
                        return flag;
                    }

                    flag = ExecuteSpectrumReadingMain(ApplicationConstants.SpectrumColors.Magenda);
                    if (!flag)
                    {
                        WriteTestResultLog();
                        LogStructureNew.LedTest = "FAIL";
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("LED BRIGHTNESS / COLOR", false);
                        OverallTestResultPassFailStatus(false, "Spectrum Color Magenda FAILED");
                        return flag;
                    }

                    flag = ExecuteSpectrumReadingMain(ApplicationConstants.SpectrumColors.BlendedWhite);
                    if (!flag)
                    {
                        WriteTestResultLog();
                        LogStructureNew.LedTest = "FAIL";
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("LED BRIGHTNESS / COLOR", false);
                        OverallTestResultPassFailStatus(false, "Spectrum Color BlendedWhite FAILED");
                        return flag;
                    }

                    LogStructureNew.LedTest = "PASS";
                    frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("LED BRIGHTNESS / COLOR", true);

                    Increment();
                }
                #endregion

                #region BANDWIDTH: FTP Data Throughput

                if (_eolmodel.Bandwidth)
                {
                    flag = ExecuteFTPDataThroughputMainTest(ApplicationConstants.AmbientLedState.Off);
                    if (!flag)
                    {
                        WriteTestResultLog();
                        LogStructureNew.BandwidthStatus = "FAIL";
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("BANDWIDTH", false);
                        OverallTestResultPassFailStatus(false, "BANDWIDTH: Executed AmbientLedState.Off FAILED");
                        return flag;
                    }

                    frmTestFixture.Instance.SetErrorMessageDisplayTextBox("BANDWIDTH: Executed AmbientLedState.Off");

                    flag = ExecuteFTPDataThroughputMainTest(ApplicationConstants.AmbientLedState.On);
                    if (!flag)
                    {
                        WriteTestResultLog();
                        LogStructureNew.BandwidthStatus = "FAIL";
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("BANDWIDTH", false);
                        OverallTestResultPassFailStatus(false, "BANDWIDTH: Executed AmbientLedState.On FAILED");
                        return flag;
                    }

                    LogStructureNew.BandwidthStatus = "PASS";
                    frmTestFixture.Instance.SetErrorMessageDisplayTextBox("BANDWIDTH: Executed AmbientLedState.On");
                    frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("BANDWIDTH", true);
                    Increment();

                }
                #endregion


                #region TEST COMPLETION
                //Test Sequence is complete. Set progress to max steps to reset progress bar
                ResetDaqBoardPort();

                ProgressStatus = MaxTestSteps;

                WriteTestResultLog();

                Increment();

                frmTestFixture.Instance.SetErrorMessageDisplayTextBox("TEST SEQUENCE PASS");
                frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("TEST COMPLETION", true);

                //Test sequence pass. Re-intializing test sequence model
                frmTestFixture.Instance.pagevm = new TestFixtureViewModel();
                #endregion

                return flag;
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("StartExecutionOfAllTestCases: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
                return false;
            }
            finally
            {
                TestFixtureEventLogger.closeLog();
            }
        }

        private bool LightEngineStartExecution()
        {
            bool flag = false;

            try
            {
                //frmTestFixture.Instance.ResetTreeView();

                frmTestFixture.Instance.SetSerialNumberTextbox("update serial number");
                frmTestFixture.Instance.SetModelNumberTextbox("update model number");
                frmTestFixture.Instance.SetVersionNumberTextbox("update version number");
                frmTestFixture.Instance.SetVoltageTextbox("update voltage number");

                frmTestFixture.Instance.SetLightEngineTreeViewTestSequenceCheckProperty("nodeLightEngineTestSequence", true);

                #region Read Impedance Voltage Check
                if (_lightenginemodel.PowerOn)
                {
                    flag = StartLightEngineReadImpedanceCheck();
                    if (!flag)
                    {
                        //WriteTestResultLog();
                        OverallTestResultPassFailStatus(false, "StartLightEngineReadImpedanceCheck FAILED");
                        return flag;
                    }

                    Increment();
                }
                #endregion

                if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                    return false;

                #region Discover Pentair Server

                if (_lightenginemodel.PentairServer)
                {
                    //flag = FindServerIPAndHandShake();
                    string sflag = TestFixtureSocketComm.DiscoverPentairServer(false);

                    if (sflag != "AVAILABLE")
                    {
                        LogStructureNew.DeviceDiscover = "FAIL";
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetLightEngineTreeViewTestSequenceCheckProperty("PENTAIR SERVER", false);
                        OverallTestResultPassFailStatus(false, "DiscoverPentairServer FAILED");
                        flag = false;
                        return flag;
                    }

                    LogStructureNew.DeviceDiscover = "PASS";
                    flag = true;
                    frmTestFixture.Instance.SetLightEngineTreeViewTestSequenceCheckProperty("PENTAIR SERVER", true);

                    Increment();
                }
                #endregion

                if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                    return false;

                #region Firmware Version

                if (_lightenginemodel.FirmwareVersion)
                {
                    flag = GetFirmwareVersion();
                    if (!flag)
                    {
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("FIRMWARE VERSION", false);
                        OverallTestResultPassFailStatus(false, "GetFirmwareVersion FAILED");
                        return flag;
                    }

                    frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("FIRMWARE VERSION", true);

                    Increment();
                }
                #endregion

                if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                    return false;

                #region Mirror commands
                if (_lightenginemodel.ProjectorMirrorCheck)
                {
                    flag = StartMirror();
                    if (!flag)
                    {
                        LogStructureNew.MirrorVerify = "FAIL";
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PROJECTOR MIRROR CHECK", false);
                        OverallTestResultPassFailStatus(false, "StartExecutionOfMirrorCommands FAILED");
                        return flag;
                    }

                    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.TopRight);
                    if (!flag)
                    {
                        LogStructureNew.MirrorVerify = "FAIL";
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PROJECTOR MIRROR CHECK", false);
                        OverallTestResultPassFailStatus(false, "MirrorPosition.TopRight FAILED");
                        return flag;
                    }

                    ErrorMsgDisplay = "Mirror position to 'top right' completed, wait for 3 seconds...";
                    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

                    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.TopLeft);
                    if (!flag)
                    {
                        LogStructureNew.MirrorVerify = "FAIL";
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PROJECTOR MIRROR CHECK", false);
                        OverallTestResultPassFailStatus(false, "MirrorPosition.TopLeft FAILED");
                        return flag;
                    }

                    ErrorMsgDisplay = "Mirror position to 'top left' completed, wait for 3 seconds...";
                    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

                    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.BottomRight);
                    if (!flag)
                    {
                        LogStructureNew.MirrorVerify = "FAIL";
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PROJECTOR MIRROR CHECK", false);
                        OverallTestResultPassFailStatus(false, "MirrorPosition.BottomRight FAILED");
                        return flag;
                    }

                    ErrorMsgDisplay = "Mirror position to 'bottom right' completed, wait for 3 seconds...";
                    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

                    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.BottomLeft);
                    if (!flag)
                    {
                        LogStructureNew.MirrorVerify = "FAIL";
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PROJECTOR MIRROR CHECK", false);
                        OverallTestResultPassFailStatus(false, "MirrorPosition.BottomLeft FAILED");
                        return flag;
                    }

                    ErrorMsgDisplay = "Mirror position to 'bottom left' completed, wait for 3 seconds...";
                    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

                    flag = SendCommandToStopImageShow();
                    if (!flag)
                    {
                        LogStructureNew.MirrorVerify = "FAIL";
                        WriteTestResultLog();
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PROJECTOR MIRROR CHECK", false);
                        ErrorMsgDisplay = "SendCommandToStopImageShow FAILED...";
                        OverallTestResultPassFailStatus(false, ErrorMsgDisplay);
                        return flag;
                    }

                    LogStructureNew.MirrorVerify = "PASS";
                    frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("PROJECTOR MIRROR CHECK", true);

                    Increment();
                }
                #endregion

                if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                    return false;

                #region Spectrometer Test

                if (_lightenginemodel.LedBrightnessColor)
                {
                    flag = ExecuteSpectrumReadingMain(ApplicationConstants.SpectrumColors.Red);
                    if (!flag)
                    {
                        WriteTestResultLog();
                        LogStructureNew.LedTest = "FAIL";
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("LED BRIGHTNESS / COLOR", false);
                        OverallTestResultPassFailStatus(false, "Spectrum Color Red FAILED");
                        return flag;
                    }

                    flag = ExecuteSpectrumReadingMain(ApplicationConstants.SpectrumColors.Green);
                    if (!flag)
                    {
                        WriteTestResultLog();
                        LogStructureNew.LedTest = "FAIL";
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("LED BRIGHTNESS / COLOR", false);
                        OverallTestResultPassFailStatus(false, "Spectrum Color Green FAILED");
                        return flag;
                    }

                    flag = ExecuteSpectrumReadingMain(ApplicationConstants.SpectrumColors.Blue);
                    if (!flag)
                    {
                        WriteTestResultLog();
                        LogStructureNew.LedTest = "FAIL";
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("LED BRIGHTNESS / COLOR", false);
                        OverallTestResultPassFailStatus(false, "Spectrum Color Blue FAILED");
                        return flag;
                    }

                    flag = ExecuteSpectrumReadingMain(ApplicationConstants.SpectrumColors.White);
                    if (!flag)
                    {
                        WriteTestResultLog();
                        LogStructureNew.LedTest = "FAIL";
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("LED BRIGHTNESS / COLOR", false);
                        OverallTestResultPassFailStatus(false, "Spectrum Color White FAILED");
                        return flag;
                    }

                    flag = ExecuteSpectrumReadingMain(ApplicationConstants.SpectrumColors.Magenda);
                    if (!flag)
                    {
                        WriteTestResultLog();
                        LogStructureNew.LedTest = "FAIL";
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("LED BRIGHTNESS / COLOR", false);
                        OverallTestResultPassFailStatus(false, "Spectrum Color Magenda FAILED");
                        return flag;
                    }

                    flag = ExecuteSpectrumReadingMain(ApplicationConstants.SpectrumColors.BlendedWhite);
                    if (!flag)
                    {
                        WriteTestResultLog();
                        LogStructureNew.LedTest = "FAIL";
                        frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("LED BRIGHTNESS / COLOR", false);
                        OverallTestResultPassFailStatus(false, "Spectrum Color BlendedWhite FAILED");
                        return flag;
                    }

                    LogStructureNew.LedTest = "PASS";
                    frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("LED BRIGHTNESS / COLOR", true);

                    Increment();
                }
                #endregion

                if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                    return false;

                #region TEST COMPLETION
                //Test Sequence is complete. Set progress to max steps to reset progress bar
                ProgressStatus = MaxTestSteps;

                WriteTestResultLog();

                ResetDaqBoardPort();

                frmTestFixture.Instance.SetErrorMessageDisplayTextBox("TEST SEQUENCE PASS");
                frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("TEST COMPLETION", true);

                //Test sequence pass. Re-intializing test sequence model
                frmTestFixture.Instance.pagevm = new TestFixtureViewModel();

                Increment();
                #endregion

                return flag;
            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("StartExecutionOfAllTestCases: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
                return false;
            }
            finally
            {
                TestFixtureEventLogger.closeLog();
            }
        }

        private bool GetFirmwareVersion()
        {
            bool flag = false;

            string version = TestFixtureCommands._mgetVersionCommd;
            TestFixtureSocketComm._isInnerloop = false;
            TestFixtureSocketComm._isInnerloop = false;
            TestFixtureSocketComm.ResetRetryCount();
            serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(version);
            if (serverStatus.Equals(connected))
            {
                this.ErrorMsgDisplay = "Sucessfuly Turned on Red Led and Reading values...";
                flag = true;
                Thread.Sleep(2000);//because ocean optics to take some time ;
                return flag;
            }
            else if (TestFixtureSocketComm.serverStatus.Equals(notConnected))
            {
                return false;
            }
            else
            {
                return false;
            }
        }

        internal bool ExecuteFTPDataThroughputMainTest(ApplicationConstants.AmbientLedState ambientLedState)
        {
            Stream clsStream = default(Stream);
            System.Net.FtpWebRequest clsRequest = default(System.Net.FtpWebRequest);
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

            double totalspeed = 0;

            string myfilename = string.Empty;
            string uploadfilename = string.Empty;

            int buffersize = 65535;

            double[] myy = new double[25001];

            bool turnoffleds = true;

            try
            {
                myfilename = TestFixtureConstants.GetIllumaVisionFile();
                uploadfilename = Path.GetFileName(myfilename);

                if (string.IsNullOrEmpty(uploadfilename))
                {
                    string msg = string.Format("ExecuteFTPDataThroughputTest: Filename ('{0}') is empty...", uploadfilename);

                    frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Error);

                    return false;
                }
             
                //192.169.1.2:8080
                if (string.IsNullOrEmpty(TestFixtureSocketComm._RetrivedIpAddress))
                {
                    string msg = string.Format("ExecuteFTPDataThroughputTest:  Ipaddress ('{0}') is null..", TestFixtureSocketComm._RetrivedIpAddress);

                    frmTestFixture.Instance.WriteToLog( msg, ApplicationConstants.TraceLogType.Error);

                    return false;
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

                var fileSize = Math.Round(Convert.ToDecimal(File.Length / 1024 / 1000));

                if (fileSize < 5)
                {
                    frmTestFixture.Instance.WriteToLog("ExecuteFTPDataThroughputTest: File is too small for testing.  Please select at least a 5MB file.", ApplicationConstants.TraceLogType.Error);
                    return false;
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

                //Uploading file to illumivision light
                for (int offset = 0; offset <= File.Length; offset += buffersize) //Change back to int
                {
                    chunkSize = Convert.ToInt32(File.Length - offset - 1);

                    if (chunkSize > buffersize)
                        chunkSize = buffersize;
                    if (clsStream.CanWrite == false)
                    {
                        frmTestFixture.Instance.WriteToLog("ExecuteFTPDataThroughputTest: Cannot write to stream...", ApplicationConstants.TraceLogType.Error);
                        return false;
                    }

                    //sends chunk to FTP server in illumivision
                    clsStream.Write(File, offset, chunkSize);
                }

                sw.Stop();

                if (ambientLedState == ApplicationConstants.AmbientLedState.Off)
                {
                    //Ambient Led Off
                    totalspeed = Math.Round((File.Length / (Convert.ToSingle(sw.ElapsedMilliseconds) / 1000)) * 0.00000762939453125, 1);
                    LogStructureNew.BandwidthOff = string.Format("{0} Mb / s(avg)", totalspeed);
                }
                else if (ambientLedState == ApplicationConstants.AmbientLedState.On)
                {
                    //Ambient Led On
                    totalspeed = Math.Round((File.Length / (Convert.ToSingle(sw.ElapsedMilliseconds) / 1000)) * 0.00000762939453125, 1);
                    LogStructureNew.BandwidthOn = string.Format("{0} Mb / s(avg)", totalspeed);
                }

                //reset parameters
                sw.Reset();

                if (turnoffleds == true)
                {
                    string  mycommand = "http://" + TestFixtureSocketComm._RetrivedIpAddress + ":8080/api?cmd=setlightcolor&id=&idType=d&zone=&color=00000000";
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

                //Delete the uploaded file
                frmTestFixture.Instance.WriteToLog("Deleting: " + uploadfilename, ApplicationConstants.TraceLogType.Information);

                System.Net.FtpWebRequest FTPRequest = (System.Net.FtpWebRequest)System.Net.WebRequest.Create("ftp://" + TestFixtureSocketComm._RetrivedIpAddress + "/" + uploadfilename);
                FTPRequest.Credentials = new System.Net.NetworkCredential("pentair", "pentair");
                FTPRequest.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;
                System.Net.FtpWebResponse FTPDelResp = (FtpWebResponse)FTPRequest.GetResponse();

                //WriteToLog("Finished uploading and deleting: " + myfilename, ApplicationConstants.TraceLogType.Information);

                if (ambientLedState == ApplicationConstants.AmbientLedState.Off)
                {
                    if (totalspeed < Convert.ToDouble(FTPOffMin) || totalspeed > Convert.ToDouble(FTPOffMax))
                    {
                        frmTestFixture.Instance.WriteToLog("FTP Off Speed out of range", ApplicationConstants.TraceLogType.Error);
                        return false;
                    }
                }

                if (ambientLedState == ApplicationConstants.AmbientLedState.On)
                {
                    if (totalspeed < Convert.ToDouble(FTPOnMin) || totalspeed > Convert.ToDouble(FTPOnMax))
                    {
                       frmTestFixture.Instance.WriteToLog("FTP ON Speed out of range", ApplicationConstants.TraceLogType.Error);
                       return false;
                    }
                }

                return true;
            }
            finally
            {
                if (clsStream != null)
                {
                    clsStream.Close();
                    clsStream.Dispose();
                }
            }
        }

        private void SetAmbientLedLight(ApplicationConstants.AmbientLedState ambientLedState)
        {
            try
            {
                //Set zones to all on
                string mycommand = string.Empty;

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
            }
            catch (Exception e)
            {
                frmTestFixture.Instance.WriteToLog("SetAbmientLedLight: " + e.Message, ApplicationConstants.TraceLogType.Error);
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
                frmTestFixture.Instance.WriteToLog("Sending: " + myrequest, ApplicationConstants.TraceLogType.Information);

                string URL = myrequest;
                // Get HTML data
                WebClient client = new WebClient();
                data = client.OpenRead(URL);
                reader = new StreamReader(data);
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                str = reader.ReadToEnd();

                frmTestFixture.Instance.WriteToLog(str, ApplicationConstants.TraceLogType.Information);
                data.Close();
                reader.Close();
            }
            catch (Exception e)
            {
                //SetErrorMessageDisplayTextBox("sendrequest: " + e.Message);
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                frmTestFixture.Instance.WriteToLog("sendrequest: " + e.Message, ApplicationConstants.TraceLogType.Error);
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

        private bool ReadAllColorsMain()
        {
            try
            {
                foreach (TestFixtureProject.Common.ApplicationConstants.SpectrumColors spectrumColor in Enum.GetValues(typeof(TestFixtureProject.Common.ApplicationConstants.SpectrumColors)))
                {
                    if (spectrumColor != Common.ApplicationConstants.SpectrumColors.Dark)
                    {
                        ExecuteSpectrumReadingMain(spectrumColor);
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }


        private bool ExecuteSpectrumReadingMain(TestFixtureProject.Common.ApplicationConstants.SpectrumColors spectrumColor)
        {
            if (TestFixtureSocketComm._IsIpAddressFound)
            {
                CommandToTurnOffLeds();

                if (frmTestFixture.Instance.testFixtureReadSpectrometer == null)
                    frmTestFixture.Instance.testFixtureReadSpectrometer = new TestFixtureReadSpectrometer();

                string msg = frmTestFixture.Instance.testFixtureReadSpectrometer.ReadFileContaints(frmTestFixture.Instance.CalibrationFilePath);

                if (msg.Equals("NoErrors"))
                {
                    if (frmTestFixture.Instance.testFixtureReadSpectrometer.GetSpectrumMain(spectrumColor))
                    {
                       bool flag = frmTestFixture.Instance.testFixtureReadSpectrometer.CalculateColorMain(spectrumColor);

                       return flag;
                    }
                    else
                    {
                        frmTestFixture.Instance.WriteToLog("GetSpectrum execution failaure...", ApplicationConstants.TraceLogType.Warning);
                        return false;
                    }
                }
                else
                {
                    frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Warning);
                    return false;
                }
            }
            else
            {
                frmTestFixture.Instance.WriteToLog("txtIpAddress is null. Please enter an valid IP address...", ApplicationConstants.TraceLogType.Warning);
                return false;
            }

            frmTestFixture.Instance.WriteToLog("Executed Spectrum Color: '" + spectrumColor.ToString() + "' successfully" , ApplicationConstants.TraceLogType.Information);
            return true;
        }

        internal void TurnSpectrumLedOnMain(Common.ApplicationConstants.SpectrumColors spectrumColor)
        {
            //Keith Dudley Added Fixed Light Switch Control
            if (spectrumColor == Common.ApplicationConstants.SpectrumColors.Red)
            {
                TurnOnRedLightCommand();

                //tFixedRed.CheckState = CheckState.Checked;
                //tFixedGreen.CheckState = CheckState.Unchecked;
                //tFixedBlue.CheckState = CheckState.Unchecked;
                //tFixedWhite.CheckState = CheckState.Unchecked;

            }
            else if (spectrumColor == Common.ApplicationConstants.SpectrumColors.Green)
            {
                TurnOnGreenLightCommand();

                //tFixedRed.CheckState = CheckState.Unchecked;
                //tFixedGreen.CheckState = CheckState.Checked;
                //tFixedBlue.CheckState = CheckState.Unchecked;
                //tFixedWhite.CheckState = CheckState.Unchecked;

            }
            else if (spectrumColor == Common.ApplicationConstants.SpectrumColors.Blue)
            {
                TurnOnBlueLightCommand();

                //tFixedRed.CheckState = CheckState.Unchecked;
                //tFixedGreen.CheckState = CheckState.Unchecked;
                //tFixedBlue.CheckState = CheckState.Checked;
                //tFixedWhite.CheckState = CheckState.Unchecked;

            }
            else if (spectrumColor == Common.ApplicationConstants.SpectrumColors.White)
            {
                TurnOnWhiteLightCommand();

                //tFixedRed.CheckState = CheckState.Unchecked;
                //tFixedGreen.CheckState = CheckState.Unchecked;
                //tFixedBlue.CheckState = CheckState.Unchecked;
                //tFixedWhite.CheckState = CheckState.Checked;
            }

            else if (spectrumColor == Common.ApplicationConstants.SpectrumColors.BlendedWhite)
            {
                TurnOnBlendedWhiteLightCommand();

                //tFixedRed.CheckState = CheckState.Unchecked;
                //tFixedGreen.CheckState = CheckState.Unchecked;
                //tFixedBlue.CheckState = CheckState.Unchecked;
                //tFixedWhite.CheckState = CheckState.Checked;
            }

            else if (spectrumColor == Common.ApplicationConstants.SpectrumColors.Magenda)
            {
                TurnOnMagendaLightCommand();

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

        public bool CheckTriggerDelayOnce(int delay)
        {
            if (TestFixtureSocketComm._IsDelayCheck)
            {
                int i = 0;
                for (i = 1; i <= delay; i++)
                {

                    if (isStopbtnPressed)
                    {
                        _mErrMsg = _mtestfixturedaq.TurnOn120VRelayOn(0);
                        _mErrMsg = _mtestfixturedaq.TurnHighForPLCPairing(0);
                        // write test data in log file
                        if (openFile)
                            WriteTestResultLog();
                        UpdateTestResultPassFailStatus(false, "Execution Stopped");
                        ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                        i = delay + 1;
                        return false;
                    }
                    else
                    {
                        i++;
                    }
                }
                if (i == delay && (!isStopbtnPressed))
                {
                    return true;
                }
            }

            return true;
        }

        public bool CheckTrigger(int delay)
        {

            int i = 0;
            for (i = 1; i <= delay; i++)
            {

                if (isStopbtnPressed)
                {
                    _mErrMsg = _mtestfixturedaq.TurnOn120VRelayOn(0);
                    _mErrMsg = _mtestfixturedaq.TurnHighForPLCPairing(0);
                    // write test data in log file
                    if (openFile)
                        WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(false, "Execution Stopped");
                    //ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    i = delay + 1;
                    return false;
                }
                else
                {
                    System.Threading.Thread.Sleep(1000);
                    i++;
                }
            }
            if (i == delay && (!isStopbtnPressed))
            {
                return true;
            }
            return true;
        }

        public bool CalibrationDelayCounter(int delay)
        {
            int i = 0;
            for (i = 1; i <= delay; i++)
            {
                System.Threading.Thread.Sleep(1000);
                //i++;

                if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                    return false;

                ErrorMsgDisplay = string.Format("Waiting for booting to complete, wait {0} seconds...", i );
                UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);
            }

            return true;
        }

        private bool StartEolReadImpedanceCheck()
        {
            if (!openFile)
            {
                LogStructureNew.Varient = string.Empty;
                LogStructureNew.ModelNo = string.Empty;
                LogStructureNew.SerialNo = string.Empty;
                LogStructureNew.FirmwareVersion = string.Empty;
                LogStructureNew.PlcLightPair = "NOT EXECUTED";
                LogStructureNew.DeviceDiscover = "NOT EXECUTED";
                LogStructureNew.BandwidthOn = "NOT EXECUTED";
                LogStructureNew.BandwidthOff = "NOT EXECUTED";
                LogStructureNew.BandwidthStatus = "NOT EXECUTED";
                LogStructureNew.MirrorVerify = "NOT EXECUTED";
                LogStructureNew.ProjectorFocus = "NOT EXECUTED";
                LogStructureNew.LedTest = "NOT EXECUTED";
                LogStructureNew.RedWatts = "NOT EXECUTED";
                LogStructureNew.GreenWatts = "NOT EXECUTED";
                LogStructureNew.BlueWatts = "NOT EXECUTED";
                LogStructureNew.WhiteWatts = "NOT EXECUTED";
                LogStructureNew.BlendedWhiteWatts = "NOT EXECUTED";
                LogStructureNew.MagentaWatts = "NOT EXECUTED";
                LogStructureNew.RedX = "NOT EXECUTED";
                LogStructureNew.RedY = "NOT EXECUTED";
                LogStructureNew.GreenX = "NOT EXECUTED";
                LogStructureNew.GreenY = "NOT EXECUTED";
                LogStructureNew.BlueX = "NOT EXECUTED";
                LogStructureNew.BlueY = "NOT EXECUTED";
                LogStructureNew.WhiteX = "NOT EXECUTED";
                LogStructureNew.WhiteY = "NOT EXECUTED";
                LogStructureNew.BlendedWhiteX = "NOT EXECUTED";
                LogStructureNew.BlendedWhiteY = "NOT EXECUTED";
                LogStructureNew.MagentaX = "NOT EXECUTED";
                LogStructureNew.MagentaY = "NOT EXECUTED";

                openFile = true;
                TestFixtureEventLogger.createLogFile();
            }
            else
            {
                LogStructureNew.Varient = string.Empty;
                LogStructureNew.ModelNo = string.Empty;
                LogStructureNew.SerialNo = string.Empty;
                LogStructureNew.FirmwareVersion = string.Empty;
                LogStructureNew.PlcLightPair = "NOT EXECUTED";
                LogStructureNew.DeviceDiscover = "NOT EXECUTED";
                LogStructureNew.BandwidthOn = "NOT EXECUTED";
                LogStructureNew.BandwidthOff = "NOT EXECUTED";
                LogStructureNew.BandwidthStatus = "NOT EXECUTED";
                LogStructureNew.MirrorVerify = "NOT EXECUTED";
                LogStructureNew.ProjectorFocus = "NOT EXECUTED";
                LogStructureNew.LedTest = "NOT EXECUTED";
                LogStructureNew.RedWatts = "NOT EXECUTED";
                LogStructureNew.GreenWatts = "NOT EXECUTED";
                LogStructureNew.BlueWatts = "NOT EXECUTED";
                LogStructureNew.WhiteWatts = "NOT EXECUTED";
                LogStructureNew.BlendedWhiteWatts = "NOT EXECUTED";
                LogStructureNew.MagentaWatts = "NOT EXECUTED";
                LogStructureNew.RedX = "NOT EXECUTED";
                LogStructureNew.RedY = "NOT EXECUTED";
                LogStructureNew.GreenX = "NOT EXECUTED";
                LogStructureNew.GreenY = "NOT EXECUTED";
                LogStructureNew.BlueX = "NOT EXECUTED";
                LogStructureNew.BlueY = "NOT EXECUTED";
                LogStructureNew.WhiteX = "NOT EXECUTED";
                LogStructureNew.WhiteY = "NOT EXECUTED";
                LogStructureNew.BlendedWhiteX = "NOT EXECUTED";
                LogStructureNew.BlendedWhiteY = "NOT EXECUTED";
                LogStructureNew.MagentaX = "NOT EXECUTED";
                LogStructureNew.MagentaY = "NOT EXECUTED";
            }

            bool flag = false;
            _mtestfixturedaq.ConfigureDaqToSingleEnded();

            ErrorMsgDisplay = "Please Scan for Model Number...!";
            flag = ReadModelNumberAfterScan();
            if (!flag)
            {
                frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("MODEL NO", true);

                isTestExecutionFailure = true;
                UpdateTestResultPassFailStatus(false, "Model Number Scanning Failed...");
                ErrorMsgDisplay = _mTestFixtureSerialComm.GetExceptionMessagesIfAny();
                return flag;
            }

            Increment(); //KD Added

            if (ReadModelNumber != null)
            {
                frmTestFixture.Instance.SetModelNumberTextbox(ReadModelNumber);

                frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("MODEL NO", true);

                Increment();
                flag = MapModelNumber();
                if (!flag)
                {
                    // write test data in log file
                    // WriteTestResultLog();
                    frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("MODEL NO", false);
                    isTestExecutionFailure = true;
                    UpdateTestResultPassFailStatus(false, "Mapping Model Number and Voltage Failed...");
                    TestFixtureEventLogger.CreateStringAToWrite("Mapping of Model and Serial Number Failed");
                    return flag;
                }

                UpdateTestResultPassFailStatus(true, "MapModelAndSerialNumber Completed...");


                Increment(); //KD Added
                UpdateTestResultPassFailStatus(true, "Model Number Scanning Completed...");
               
                //Do Model Voltage lookup immediately after retrieving model number
                //Do the impedance check
                ErrorMsgDisplay = "Please Scan for Serial Number...";
                flag = ReadSerialNumberFromScanner();
                if (!flag)
                {
                    // write test data in log file
                    // WriteTestResultLog();
                    frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("SERIAL NO", false);

                    isTestExecutionFailure = true;
                    UpdateTestResultPassFailStatus(false, "Serial Number Scanning Failed...");
                    ErrorMsgDisplay = _mTestFixtureSerialComm.GetExceptionMessagesIfAny();
                    return flag;
                }

                LogStructureNew.SerialNo = UpdateSerialNumber;
                frmTestFixture.Instance.SetSerialNumberTextbox(UpdateSerialNumber);
                frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("SERIAL NO", true);

                UpdateTestResultPassFailStatus(true, "ReadSerialNumberFromScanner Completed...");
                //ErrorMsgDisplay = string.Empty;

   
                TestFixtureEventLogger.CreateStringAToWrite("Mapping of Model and Serial Number Passed");
                //UpdateTestResultPassFailStatus(true, "");
            }
            else
            {
                frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("MODEL NO", false);
                isTestExecutionFailure = true;
                UpdateTestResultPassFailStatus(false, "Error reading model number...");
                ErrorMsgDisplay = "Error reading model number...";
                return false;
            }
            Increment();
            UpdateTestResultPassFailStatus(true, "Mapping Model Number and Voltage Passed...");

            ErrorMsgDisplay = "Turning on 120 VAC Relay";
            _mErrMsg = _mtestfixturedaq.TurnOn120VRelayOn(1);
            if (!_mErrMsg.Equals("NoErrors"))
            {
                if (openFile)
                    WriteTestResultLog();


                frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("POWER", false);
                isTestExecutionFailure = true;
                UpdateTestResultPassFailStatus(false, "Failed to Turn on 120VAC Relay");
                ErrorMsgDisplay = _mErrMsg;
                TestFixtureEventLogger.CreateStringAToWrite("Failed to turned on 120VAC Relay");
                return flag;
            }

            frmTestFixture.Instance.SetEolTreeViewTestSequenceCheckProperty("POWER", true);
            //_mlogger.CreateStringAToWrite("Turned on 120AC Voltage");
            UpdateTestResultPassFailStatus(true, "Turned on 120VAC Relay");
            IsVoltageOK = true;
            Increment();

            return flag;
        }

        private bool StartLightEngineReadImpedanceCheck()
        {
            if (!openFile)
            {
                LogStructureNew.Varient = string.Empty;
                LogStructureNew.ModelNo = string.Empty;
                LogStructureNew.SerialNo = string.Empty;
                LogStructureNew.FirmwareVersion = string.Empty;
                LogStructureNew.PlcLightPair = "NOT EXECUTED";
                LogStructureNew.DeviceDiscover = "NOT EXECUTED";
                LogStructureNew.BandwidthOn = "NOT EXECUTED";
                LogStructureNew.BandwidthOff = "NOT EXECUTED";
                LogStructureNew.BandwidthStatus = "NOT EXECUTED";
                LogStructureNew.MirrorVerify = "NOT EXECUTED";
                LogStructureNew.ProjectorFocus = "NOT EXECUTED";
                LogStructureNew.LedTest = "NOT EXECUTED";
                LogStructureNew.RedWatts = "NOT EXECUTED";
                LogStructureNew.GreenWatts = "NOT EXECUTED";
                LogStructureNew.BlueWatts = "NOT EXECUTED";
                LogStructureNew.WhiteWatts = "NOT EXECUTED";
                LogStructureNew.BlendedWhiteWatts = "NOT EXECUTED";
                LogStructureNew.MagentaWatts = "NOT EXECUTED";
                LogStructureNew.RedX = "NOT EXECUTED";
                LogStructureNew.RedY = "NOT EXECUTED";
                LogStructureNew.GreenX = "NOT EXECUTED";
                LogStructureNew.GreenY = "NOT EXECUTED";
                LogStructureNew.BlueX = "NOT EXECUTED";
                LogStructureNew.BlueY = "NOT EXECUTED";
                LogStructureNew.WhiteX = "NOT EXECUTED";
                LogStructureNew.WhiteY = "NOT EXECUTED";
                LogStructureNew.BlendedWhiteX = "NOT EXECUTED";
                LogStructureNew.BlendedWhiteY = "NOT EXECUTED";
                LogStructureNew.MagentaX = "NOT EXECUTED";
                LogStructureNew.MagentaY = "NOT EXECUTED";

                openFile = true;
                TestFixtureEventLogger.createLogFile();
            }
            else
            {
                LogStructureNew.Varient = string.Empty;
                LogStructureNew.ModelNo = string.Empty;
                LogStructureNew.SerialNo = string.Empty;
                LogStructureNew.FirmwareVersion = string.Empty;
                LogStructureNew.PlcLightPair = "NOT EXECUTED";
                LogStructureNew.DeviceDiscover = "NOT EXECUTED";
                LogStructureNew.BandwidthOn = "NOT EXECUTED";
                LogStructureNew.BandwidthOff = "NOT EXECUTED";
                LogStructureNew.BandwidthStatus = "NOT EXECUTED";
                LogStructureNew.MirrorVerify = "NOT EXECUTED";
                LogStructureNew.ProjectorFocus = "NOT EXECUTED";
                LogStructureNew.LedTest = "NOT EXECUTED";
                LogStructureNew.RedWatts = "NOT EXECUTED";
                LogStructureNew.GreenWatts = "NOT EXECUTED";
                LogStructureNew.BlueWatts = "NOT EXECUTED";
                LogStructureNew.WhiteWatts = "NOT EXECUTED";
                LogStructureNew.BlendedWhiteWatts = "NOT EXECUTED";
                LogStructureNew.MagentaWatts = "NOT EXECUTED";
                LogStructureNew.RedX = "NOT EXECUTED";
                LogStructureNew.RedY = "NOT EXECUTED";
                LogStructureNew.GreenX = "NOT EXECUTED";
                LogStructureNew.GreenY = "NOT EXECUTED";
                LogStructureNew.BlueX = "NOT EXECUTED";
                LogStructureNew.BlueY = "NOT EXECUTED";
                LogStructureNew.WhiteX = "NOT EXECUTED";
                LogStructureNew.WhiteY = "NOT EXECUTED";
                LogStructureNew.BlendedWhiteX = "NOT EXECUTED";
                LogStructureNew.BlendedWhiteY = "NOT EXECUTED";
                LogStructureNew.MagentaX = "NOT EXECUTED";
                LogStructureNew.MagentaY = "NOT EXECUTED";
            }

            bool flag = false;
            _mtestfixturedaq.ConfigureDaqToSingleEnded();

            //Do Model Voltage lookup immediately after retrieving model number
            //Do the impedance check
            ErrorMsgDisplay = "Please Scan for Serial Number...";
            flag = ReadSerialNumberFromScanner();
            if (!flag)
            {
                // write test data in log file
                // WriteTestResultLog();
                frmTestFixture.Instance.SetLightEngineTreeViewTestSequenceCheckProperty("SERIAL NO", false);

                isTestExecutionFailure = true;
                OverallTestResultPassFailStatus(false, "Serial Number Scanning Failed...");
                ErrorMsgDisplay = _mTestFixtureSerialComm.GetExceptionMessagesIfAny();
                return flag;
            }

            LogStructureNew.SerialNo = UpdateSerialNumber;
            frmTestFixture.Instance.SetSerialNumberTextbox(UpdateSerialNumber);
            frmTestFixture.Instance.SetLightEngineTreeViewTestSequenceCheckProperty("SERIAL NO", true);

            UpdateTestResultPassFailStatus(true, "ReadSerialNumberFromScanner Completed...");

            Increment();

            ErrorMsgDisplay = "Turning on 120 VAC Relay";
            _mErrMsg = _mtestfixturedaq.TurnOn120VRelayOn(1);
            if (!_mErrMsg.Equals("NoErrors"))
            {
                if (openFile)
                    WriteTestResultLog();

                frmTestFixture.Instance.SetLightEngineTreeViewTestSequenceCheckProperty("POWER", false);
                isTestExecutionFailure = true;
                OverallTestResultPassFailStatus(false, "Failed to Turn on 120VAC Relay");
                ErrorMsgDisplay = _mErrMsg;
                TestFixtureEventLogger.CreateStringAToWrite("Failed to turned on 120VAC Relay");
                return flag;
            }

            frmTestFixture.Instance.SetLightEngineTreeViewTestSequenceCheckProperty("POWER", true);
            //_mlogger.CreateStringAToWrite("Turned on 120AC Voltage");
            UpdateTestResultPassFailStatus(true, "Turned on 120VAC Relay");
            IsVoltageOK = true;
            Increment();

            return flag;
        }

        private bool StartExecutionOfDaq()
        {
            if (!openFile)
            {
                LogStructureNew.Varient = string.Empty;
                LogStructureNew.SerialNo = string.Empty;
                LogStructureNew.DeviceDiscover = "NOT EXECUTED";
                LogStructureNew.BandwidthOn = "NOT EXECUTED";
                LogStructureNew.BandwidthOff = "NOT EXECUTED";
                LogStructureNew.BandwidthStatus = "NOT EXECUTED";
                LogStructureNew.MirrorVerify = "NOT EXECUTED";
                LogStructureNew.ProjectorFocus = "NOT EXECUTED";
                LogStructureNew.LedTest = "NOT EXECUTED";
                LogStructureNew.PlcLightPair = "NOT EXECUTED";
                openFile = true;
                TestFixtureEventLogger.createLogFile();
            }
            else
            {
                LogStructureNew.Varient = string.Empty;
                LogStructureNew.SerialNo = string.Empty;
                LogStructureNew.DeviceDiscover = "NOT EXECUTED";
                LogStructureNew.BandwidthOn = "NOT EXECUTED";
                LogStructureNew.BandwidthOff = "NOT EXECUTED";
                LogStructureNew.BandwidthStatus = "NOT EXECUTED";
                LogStructureNew.MirrorVerify = "NOT EXECUTED";
                LogStructureNew.ProjectorFocus = "NOT EXECUTED";
                LogStructureNew.LedTest = "NOT EXECUTED";
                LogStructureNew.PlcLightPair = "NOT EXECUTED";
            }

            bool flag = false;
            _mtestfixturedaq.ConfigureDaqToSingleEnded();

            ErrorMsgDisplay = "Please Scan for Model Number...!";
            flag = ReadModelNumberAfterScan();
            if (!flag)
            {
                isTestExecutionFailure = true;
                UpdateTestResultPassFailStatus(false, "Model Number Scanning Failed...");
                ErrorMsgDisplay = _mTestFixtureSerialComm.GetExceptionMessagesIfAny();
                return flag;
            }

            Increment(); //KD Added

            if (ReadModelNumber != null)
            {
                frmTestFixture.Instance.SetModelNumberTextbox(ReadModelNumber);
               
                Increment(); //KD Added
                UpdateTestResultPassFailStatus(true, "Model Number Scanning Completed...");

                ErrorMsgDisplay = "Please Scan for Serial Number...!";
                flag = ReadSerialNumberFromScanner();
                if (!flag)
                {
                    // write test data in log file
                    // WriteTestResultLog();
                    isTestExecutionFailure = true;
                    UpdateTestResultPassFailStatus(false, "Serial Number Scanning Failed...");
                    ErrorMsgDisplay = _mTestFixtureSerialComm.GetExceptionMessagesIfAny();
                    return flag;
                }

                frmTestFixture.Instance.SetSerialNumberTextbox(UpdateSerialNumber);

                UpdateTestResultPassFailStatus(true, "ReadSerialNumberFromScanner Completed...");
                //ErrorMsgDisplay = string.Empty;

                Increment();
                flag = MapModelNumber();
                if (!flag)
                {
                    // write test data in log file
                    // WriteTestResultLog();
                    isTestExecutionFailure = true;
                    UpdateTestResultPassFailStatus(false, "Mapping Model Number and Voltage Failed...");
                    TestFixtureEventLogger.CreateStringAToWrite("Mapping of Model and Serial Number Failed");
                    return flag;
                }

                UpdateTestResultPassFailStatus(true, "MapModelAndSerialNumber Completed...");

                TestFixtureEventLogger.CreateStringAToWrite("Mapping of Model and Serial Number Passed");
                UpdateTestResultPassFailStatus(true, "");
            }
            else
            {
                isTestExecutionFailure = true;
                UpdateTestResultPassFailStatus(true, "Error reading model number...");
                ErrorMsgDisplay = "Error reading model number...";
                return false;
            }
            Increment();
            UpdateTestResultPassFailStatus(true, "Mapping Model Number and Voltage Passed...");

            ErrorMsgDisplay = "Turning on 120 VAC Relay";
            _mErrMsg = _mtestfixturedaq.TurnOn120VRelayOn(1);
            if (!_mErrMsg.Equals("NoErrors"))
            {
                if (openFile)
                    WriteTestResultLog();

                isTestExecutionFailure = true;
                UpdateTestResultPassFailStatus(false, "Failed to Turn on 120VAC Relay");
                ErrorMsgDisplay = _mErrMsg;
                TestFixtureEventLogger.CreateStringAToWrite("Failed to turned on 120VAC Relay");
                return flag;
            }
            //_mlogger.CreateStringAToWrite("Turned on 120AC Voltage");
            UpdateTestResultPassFailStatus(true, "Turned on 120VAC Relay");
            IsVoltageOK = true;
            Increment();

            ErrorMsgDisplay = "Solenoid Sequence in progress...";

            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            //Wall Adapter turining on for 14 seconds after that it should go low for 5 seconds
            _mErrMsg = _mtestfixturedaq.TurnHighForPLCPairing(1); //make it high for 15sec
            LogStructureNew.PlcLightPair = "FAIL";
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                // write test data in log file
                //  WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            UpdateTestResultPassFailStatus(true, "TurnHighForPLCPairing(1) completed" );

            Increment();
            int delay = 14;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                isTestExecutionFailure = true;
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }


            _mErrMsg = _mtestfixturedaq.TurnHighForPLCPairing(0); //make it high for 15sec
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                // write test data in log file
                //  WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            UpdateTestResultPassFailStatus(true, "TurnHighForPLCPairing(0) completed");

            Increment();
            delay = 5;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                isTestExecutionFailure = true;
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            //Hall Effect turned on for 14seconds and go low for 5 seconds
            _mErrMsg = SendCommandToSolenoid();
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Hall Effect Solenoid Error:");
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            UpdateTestResultPassFailStatus(true, "SendCommandToSolenoid completed");

            Increment();
            delay = 14;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                isTestExecutionFailure = true;
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            _mErrMsg = StopDHCPSolenoid();
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Hall Effect Solenoid Error:");
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            UpdateTestResultPassFailStatus(true, "StopDHCPSolenoid completed");

            Increment();
            delay = 5;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                isTestExecutionFailure = true;
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            _mErrMsg = _mtestfixturedaq.TurnHighForPLCPairing(1); //make it high for 15sec
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                // write test data in log file
                // WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            UpdateTestResultPassFailStatus(true, "TurnHighForPLCPairing(1) completed");

            Increment();
            delay = 2;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                isTestExecutionFailure = true;
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            _mErrMsg = _mtestfixturedaq.TurnHighForPLCPairing(0); //make it high for 15sec
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                // write test data in log file
                // WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            UpdateTestResultPassFailStatus(true, "TurnHighForPLCPairing(0) completed");

            Increment();
            delay = 5;
            flag = CheckTrigger(delay);
            if (!flag)
                return flag;

            //Hall Effect turned on for 14seconds and go low for 5 seconds
            _mErrMsg = SendCommandToSolenoid();
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                // write test data in log file
                //WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Hall Effect Solenoid Error:");
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            UpdateTestResultPassFailStatus(true, "SendCommandToSolenoid completed");

            Increment();
            delay = 2;
            flag = CheckTrigger(delay);
            if (!flag)
                return flag;

            _mErrMsg = StopDHCPSolenoid();
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Hall Effect Solenoid Error:");
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            UpdateTestResultPassFailStatus(true, "StopDHCPSolenoid completed");

            Increment();
            LogStructureNew.PlcLightPair = "PASS";
            UpdateTestResultPassFailStatus(true, "Solenoid Sequence Success");

            ErrorMsgDisplay = "Calibration in Progress..";

            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            // Increased delay to 45 sec for calibration complete and then
            // Device discovery will start
            delay = 150;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                isTestExecutionFailure = true;
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            UpdateTestResultPassFailStatus(true, "Start Execution of DAQ completed completed");

            return flag;
        }

        private bool FindServerIPAndHandShake()
        {
            if (!openFile)
            {
                LogStructureNew.DeviceDiscover = "NOT EXECUTED";
                openFile = true;
                TestFixtureEventLogger.createLogFile();
            }
            else
            {
                LogStructureNew.DeviceDiscover = "NOT EXECUTED";
            }

            LogStructureNew.DeviceDiscover = "FAIL";
            bool flag = false;
            SetScokectFlagsToFalse();
            ErrorMsgDisplay = "Discovering Server IP address...";
            if (serverIpAddess == null)
            {

                serverStatus = EstablishEtherNetInterfaceWithDevice();
            }
            else
            {
                serverStatus = connected;
            }

            if (serverStatus.Equals(notConnected))
            {
                LogStructureNew.DeviceDiscover = "FAIL";
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Discovering Server IP Address Failed");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else if (serverStatus.Equals(interrupted))
            {
                LogStructureNew.DeviceDiscover = "FAIL";
                // write test data in log file
                //WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            Increment();
            LogStructureNew.DeviceDiscover = "PASS";
            flag = true;
            UpdateTestResultPassFailStatus(true, "Server IP address discovered");

            //Bandwidth Check
            Thread.Sleep(1000);
            UpdateTestResultPassFailStatus(true, "PLC and Light pairing sequence success");
            Thread.Sleep(1000);
            ErrorMsgDisplay = "Obtaining version number..";

            string serverresponse = TestFixtureSocketComm.GetVersionNumber();
            if (serverresponse.Length > 0)
            {
                VersionNumber = serverresponse;
                //frmTestFixture.Instance.SetVersionNumberTextbox(VersionNumber);

                OnPropertyChanged("VersionNumber");
            }
            else
            {
                VersionNumber = string.Empty;
                frmTestFixture.Instance.SetVersionNumberTextbox(VersionNumber);

                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Failed to Obtain Version Number");
                return false;
            }
            Increment();
            UpdateTestResultPassFailStatus(true, "Sucessfully Obtained Version Number");
            //ErrorMsgDisplay = "Obtaining Product Type..";

            Thread.Sleep(1000);
            ErrorMsgDisplay = "Bandwidth test in progress..";

            //LogStructureNew.BandwidthOff = "NIL";
            //LogStructureNew.BandwidthOn = "NIL";
            LogStructureNew.BandwidthStatus = "FAIL";

            double[] bandwidthData = TestFixtureSocketComm.checkBandwidth();

            double timeTaken = bandwidthData[0];
            double bw = bandwidthData[1];

            if (bw <= 0)
            {
                //LogStructureNew.Bandwidth = "NIL";
                //LogStructureNew.BandwidthDuration = "NIL";
                //LogStructureNew.BandwidthStatus = "FAIL";
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Bandwidth: Upload File Failed");
                Thread.Sleep(1000);
                ErrorMsgDisplay = "Network issue"; // Print
                Thread.Sleep(1000);
                return false;
            }
            else if (bw >= 0 && timeTaken < _model.BWidthToleranceSecond)
            {
                //LogStructureNew.BandwidthOff = bw.ToString();
                //LogStructureNew.BandwidthOn = timeTaken.ToString();
                LogStructureNew.BandwidthStatus = "FAIL";
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Bandwidth: Upload file Time Exceeded tolerance level..");
                Thread.Sleep(1000);
                ErrorMsgDisplay = "Upload file Time Exceeded tolerance level";
                Thread.Sleep(1000);
                return false;
            }

            Increment();
            //LogStructureNew.BandwidthOff = bw.ToString();
            //LogStructureNew.BandwidthOn = timeTaken.ToString();
            LogStructureNew.BandwidthStatus = "PASS";

            UpdateTestResultPassFailStatus(true, "Bandwidth: Test Successfull");
            ErrorMsgDisplay = "Bandwidth: Test Successfulls";
            Thread.Sleep(1000);
            bool deleteFile = TestFixtureSocketComm.DeleteUploadedFile("bandwidth");
            Thread.Sleep(1000);
            ////log the return value + version number
            string producttype = TestFixtureCommands._mgetProductType;

            SetScokectFlagsToFalse();

            ////get the product Type and log it ..not working..check
            serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(producttype);
            if (serverStatus.Equals(interrupted))
            {
                // write test data in log file
                // WriteTestResultLog();
                UpdateTestResultPassFailStatus(true, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else if (serverStatus.Equals(notConnected))
            {
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Failed to Obtain Product Type");
                ErrorMsgDisplay = "Server not available..";
                return false;
            }
            else
            {
                Increment();
                UpdateTestResultPassFailStatus(true, "Product type obtained");
                ErrorMsgDisplay = "Uploading test files in progress..";
            }

            // This below line is for image upload
            int delay = 2;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            Increment();
            // UpdateTestResultPassFailStatus(true, "Uploading test files..");

            bool upload = TestFixtureSocketComm.fileUpload();

            Increment();
            UpdateTestResultPassFailStatus(true, "Uploading test files successfull");

            Thread.Sleep(1000);
            return flag;
        }

        private bool StartMirror()
        {
            if (!openFile)
            {
                LogStructureNew.MirrorVerify = "NOT EXECUTED";
                openFile = true;
                TestFixtureEventLogger.createLogFile();
            }
            else
            {
                LogStructureNew.MirrorVerify = "NOT EXECUTED";
            }

            bool flag = false;
            ErrorMsgDisplay = "Mirror Verification in Progress...";
            LogStructureNew.MirrorVerify = "FAIL";

            //flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.Home);

            ////if (!flag)
            ////    return flag;

            //ErrorMsgDisplay = "Mirror position to home completed, wait for 3 seconds...";
            //UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            //Thread.Sleep(3000);
  
            flag = TestFixtureSocketComm.fileUpload();
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "File Upload Failed");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            ErrorMsgDisplay = "fileUpload completed";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            SetScokectFlagsToFalse();
            flag = StartImageShowForMirror();
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "Start Image Show Failed");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            return true;
        }

        //private bool StartMirror()
        //{
        //    if (!openFile)
        //    {
        //        LogStructureNew.MirrorVerify = "NOT EXECUTED";
        //        openFile = true;
        //        TestFixtureEventLogger.createLogFile();
        //    }
        //    else
        //    {
        //        LogStructureNew.MirrorVerify = "NOT EXECUTED";
        //    }

        //    bool flag = false;
        //    ErrorMsgDisplay = "Mirror Verification in Progress...";
        //    LogStructureNew.MirrorVerify = "FAIL";

        //    //flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.Home);

        //    ////if (!flag)
        //    ////    return flag;

        //    //ErrorMsgDisplay = "Mirror position to home completed, wait for 3 seconds...";
        //    //UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

        //    //Thread.Sleep(3000);

        //    flag = TestFixtureSocketComm.fileUpload();
        //    if (!flag)
        //    {
        //        UpdateTestResultPassFailStatus(false, "File Upload Failed");
        //        ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
        //        return false;
        //    }

        //    ErrorMsgDisplay = "fileUpload completed";
        //    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);


        //    SetScokectFlagsToFalse();
        //    flag = StartImageShowForMirror();
        //    if (!flag)
        //    {
        //        UpdateTestResultPassFailStatus(false, "Start Image Show Failed");
        //        ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
        //        return false;
        //    }

        //    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.TopRight);
        //    if (!flag)
        //        return flag;

        //    ErrorMsgDisplay = "Mirror position to 'top right' completed, wait for 3 seconds...";
        //    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

        //    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.TopLeft);
        //    if (!flag)
        //        return flag;

        //    ErrorMsgDisplay = "Mirror position to 'top left' completed, wait for 3 seconds...";
        //    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

        //    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.BottomRight);
        //    if (!flag)
        //        return flag;

        //    ErrorMsgDisplay = "Mirror position to 'bottom right' completed, wait for 3 seconds...";
        //    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

        //    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.BottomLeft);
        //    if (!flag)
        //        return flag;

        //    ErrorMsgDisplay = "Mirror position to 'bottom left' completed, wait for 3 seconds...";
        //    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

        //    Thread.Sleep(3000);

        //    Increment();
        //    LogStructureNew.MirrorVerify = "PASS";
        //    UpdateTestResultPassFailStatus(true, "Mirror Verification is Sucessfull");

        //    flag = SendCommandToStopImageShow();
        //    if (!flag)
        //    {
        //        LogStructureNew.MirrorVerify = "FAIL";
        //        if (openFile)
        //            WriteTestResultLog();
        //        UpdateTestResultPassFailStatus(false, "Stop Image Show Failed");
        //        ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
        //        return false;
        //    }

        //    ErrorMsgDisplay = "SendCommandToStopImageShow completed, wait for 1 second";
        //    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

        //    return true;
        //}

        //private bool StartMirror()
        //{

        //    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.TopRight);
        //    if (!flag)
        //        return flag;

        //    ErrorMsgDisplay = "Mirror position to 'top right' completed, wait for 3 seconds...";
        //    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

        //    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.TopLeft);
        //    if (!flag)
        //        return flag;

        //    ErrorMsgDisplay = "Mirror position to 'top left' completed, wait for 3 seconds...";
        //    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

        //    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.BottomRight);
        //    if (!flag)
        //        return flag;

        //    ErrorMsgDisplay = "Mirror position to 'bottom right' completed, wait for 3 seconds...";
        //    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

        //    flag = MoveMirrorPosition(ApplicationConstants.MirrorPosition.BottomLeft);
        //    if (!flag)
        //        return flag;

        //    ErrorMsgDisplay = "Mirror position to 'bottom left' completed, wait for 3 seconds...";
        //    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

        //    Thread.Sleep(3000);

        //    Increment();
        //    LogStructureNew.MirrorVerify = "PASS";
        //    UpdateTestResultPassFailStatus(true, "Mirror Verification is Sucessfull");

        //    flag = SendCommandToStopImageShow();
        //    if (!flag)
        //    {
        //        LogStructureNew.MirrorVerify = "FAIL";
        //        if (openFile)
        //            WriteTestResultLog();
        //        UpdateTestResultPassFailStatus(false, "Stop Image Show Failed");
        //        ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
        //        return false;
        //    }

        //    ErrorMsgDisplay = "SendCommandToStopImageShow completed, wait for 1 second";
        //    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

        //    return true;
        //}

        private bool StartExecutionOfMirrorCommands()
        {
            if (!openFile)
            {
                LogStructureNew.MirrorVerify = "NOT EXECUTED";
                openFile = true;
                TestFixtureEventLogger.createLogFile();
            }
            else
            {
                LogStructureNew.MirrorVerify = "NOT EXECUTED";
            }

            bool flag = false;
            ErrorMsgDisplay = "Mirror Verification in Progress...";
            LogStructureNew.MirrorVerify = "FAIL";
            SetScokectFlagsToFalse();
            flag = SendCommandToBringMirrorToHome();
            if (!flag)
                return flag;

            ErrorMsgDisplay = "SendCommandToBringMirrorToHome completed, wait for 2 seconds...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            Thread.Sleep(2000);
            int delay = 2;
            //flag = CheckTrigger(delay);
            //if (!flag)
            //{
            //    UpdateTestResultPassFailStatus(false, "Execution Stopped");
            //    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
            //    return false;
            //}
            flag = TestFixtureSocketComm.fileUpload();
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "File Upload Failed");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            ErrorMsgDisplay = "fileUpload completed";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            SetScokectFlagsToFalse();
            flag = StartImageShowForMirror();
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "Start Image Show Failed");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            SetScokectFlagsToFalse();
            TopRightMirrorCommand();

            if (readTRvoltage >= 1.5)
            {
                if (readTLvoltage <= 1 && readBLvoltage <= 1 && readBRvoltage <= 1)
                {
                    ErrorMsgDisplay = "Top Right Mirror Calibration Pass";
                    UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);
                    // ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
                    //return flag;
                }
                else
                {
                    if (openFile)
                        WriteTestResultLog();

                    //ErrorMsgDisplay = "Top Right Mirror Calibration Failed";
                    //UpdateTestResultPassFailStatus(false, ErrorMsgDisplay);
                    //return false;
                }
            }
            else
            {
                if (openFile)
                    WriteTestResultLog();
                //UpdateTestResultPassFailStatus(false, "Top Right Mirror Calibration Failed");
                //ErrorMsgDisplay = "readTRvoltage is out of range: " + readTRvoltage.ToString();
                //return false;
            }

            ErrorMsgDisplay = "TopRightMirrorCommand completed, wait for 1 second";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            delay = 1;

            Thread.Sleep(1000);
            //flag = CheckTrigger(delay);
            //if (!flag)
            //{
            //    UpdateTestResultPassFailStatus(false, "Execution Stopped");
            //    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
            //    //return false;
            //}
            SetScokectFlagsToFalse();

            TopLeftMirrorCOmmand();
            if (readTLvoltage >= 1.5)
            {
                if (readTRvoltage <= 1 && readBLvoltage <= 1 && readBRvoltage <= 1)
                {
                    UpdateTestResultPassFailStatus(true, "Top Left Mirror Calibration Pass");
                     ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
                    //return flag;
                }
                else
                {
                    //if (openFile)
                    //    WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(false, "Top Left Mirror Calibration Failed");
                    //ErrorMsgDisplay = "Test image is not falling on photodiode.";
                    //return false;
                }
            }
            else
            {
                if (openFile)
                    WriteTestResultLog();
                //UpdateTestResultPassFailStatus(false, "Top Left Mirror Calibration Failed");
                //ErrorMsgDisplay = "Test image is not falling on photodiode.";
                //return false;
            }

            ErrorMsgDisplay = "TopLeftMirrorCommand completed, wait for 1 second";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            delay = 1;

            Thread.Sleep(1000);
            //flag = CheckTrigger(delay);
            //if (!flag)
            //{
            //    UpdateTestResultPassFailStatus(false, "Execution Stopped");
            //    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
            //    return false;
            //}

            SetScokectFlagsToFalse();

            BottomRightMirrorCommand();
            if (readBRvoltage >= 1.5)
            {

                if (readTRvoltage <= 1 && readBLvoltage <= 1 && readTLvoltage <= 1)
                {
                    UpdateTestResultPassFailStatus(true, "Bottom Right Mirror Calibration Pass");
                    ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
                    //return flag;
                }
                else
                {
                    //if (openFile)
                    //    WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(false, "Bottom Right Mirror Calibration Failed");
                    //ErrorMsgDisplay = "Test image is not falling on photodiode.";
                    //return false;
                }
            }
            else
            {
                if (openFile)
                    WriteTestResultLog();
                //UpdateTestResultPassFailStatus(false, "Bottom Right Mirror Calibration Failed");
                //ErrorMsgDisplay = "Test image is not falling on photodiode.";
                //return false;
            }
            delay = 1;

            Thread.Sleep(1000);
            //flag = CheckTrigger(delay);
            //if (!flag)
            //{
            //    UpdateTestResultPassFailStatus(false, "Execution Stopped");
            //    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
            //    return false;
            //}
            SetScokectFlagsToFalse();

            BottomLeftCommand();
            if (readBLvoltage >= 1.5)
            {
                if (readTRvoltage <= 1 && readBRvoltage <= 1 && readTLvoltage <= 1)
                {
                    UpdateTestResultPassFailStatus(true, "Bottom Left Mirror Calibration Pass");
                    // ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
                    //return flag;
                }
                else
                {
                    //if (openFile)
                    //    WriteTestResultLog();
                    //UpdateTestResultPassFailStatus(false, "Bottom Left Mirror Calibration Failed");
                    //ErrorMsgDisplay = "Test image is not falling on photodiode.";
                    //return true;
                }
            }
            else
            {
                if (openFile)
                    WriteTestResultLog();
                //UpdateTestResultPassFailStatus(false, "Bottom Left Mirror Calibration Failed");
                //ErrorMsgDisplay = "Test image is not falling on photodiode.";
                ////return false;
            }
            Thread.Sleep(1000);
            Increment();
            LogStructureNew.MirrorVerify = "PASS";
            UpdateTestResultPassFailStatus(true, "Mirror Verification is Sucessfull");

            ErrorMsgDisplay = "BottomLeftCommand completed";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            flag = SendCommandToStopImageShow();
            if (!flag)
            {
                LogStructureNew.MirrorVerify = "PASS";
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Stop Image Show Failed");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            ErrorMsgDisplay = "SendCommandToStopImageShow completed, wait for 1 second";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            Thread.Sleep(1000);

            //delay = 1;
            //flag = CheckTrigger(delay);
            //if (!flag)
            //{
            //    UpdateTestResultPassFailStatus(false, "Execution Stopped");
            //    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
            //    return false;
            //}

            SetScokectFlagsToFalse();
            flag = SendCommandToBringMirrorToHome();
            if (!flag)
            {
                LogStructureNew.MirrorVerify = "PASS";
                if (openFile)
                    WriteTestResultLog();
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            ErrorMsgDisplay = "SendCommandToBringMirrorToHome completed, wait for 1 second";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            delay = 1;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            ErrorMsgDisplay = "Mirror of command operations completed";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

           
            return flag;
        }

        private bool StartExecutionOfMirrorCommands2()
        {
            if (!openFile)
            {
                LogStructureNew.MirrorVerify = "NOT EXECUTED";
                openFile = true;
                TestFixtureEventLogger.createLogFile();
            }
            else
            {
                LogStructureNew.MirrorVerify = "NOT EXECUTED";
            }

            bool flag = false;
            ErrorMsgDisplay = "Mirror Verification in Progress...";
            LogStructureNew.MirrorVerify = "FAIL";
            SetScokectFlagsToFalse();
            flag = SendCommandToBringMirrorToHome();
            if (!flag)
                return flag;

            Thread.Sleep(2000);
            int delay = 2;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            SetScokectFlagsToFalse();
            flag = StartImageShowForMirror();
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "Start Image Show Failed");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            SetScokectFlagsToFalse();
            TopRightMirrorCommand();

            if (readTRvoltage >= 1)
            {
                if (readTLvoltage <= 1 && readBLvoltage <= 1 && readBRvoltage <= 1)
                {
                    UpdateTestResultPassFailStatus(true, "Top Right Mirror Calibration Pass");
                    // ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
                    //return flag;
                }
                else
                {
                    if (openFile)
                        WriteTestResultLog();
                    UpdateTestResultPassFailStatus(false, "Top Right Mirror Calibration Failed");
                    ErrorMsgDisplay = "Top Right Mirror Calibration Failed";
                    return false;
                }
            }
            else
            {
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Top Right Mirror Calibration Failed");
                ErrorMsgDisplay = "readTRvoltage is out of range: " + readTRvoltage.ToString();
                return false;
            }
            delay = 1;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                //return false;
            }
            SetScokectFlagsToFalse();

            TopLeftMirrorCOmmand();
            if (readTLvoltage >= 1)
            {
                if (readTRvoltage <= 1 && readBLvoltage <= 1 && readBRvoltage <= 1)
                {
                    UpdateTestResultPassFailStatus(true, "Top Left Mirror Calibration Pass");
                    // ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
                    //return flag;
                }
                else
                {
                    if (openFile)
                        WriteTestResultLog();
                    UpdateTestResultPassFailStatus(false, "Top Left Mirror Calibration Failed");
                    ErrorMsgDisplay = "Test image is not falling on photodiode.";
                    return false;
                }
            }
            else
            {
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Top Left Mirror Calibration Failed");
                ErrorMsgDisplay = "Test image is not falling on photodiode.";
                return false;
            }
            delay = 1;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            SetScokectFlagsToFalse();

            BottomRightMirrorCommand();
            if (readBRvoltage >= 1)
            {

                if (readTRvoltage <= 1 && readBLvoltage <= 1 && readTLvoltage <= 1)
                {
                    UpdateTestResultPassFailStatus(true, "Bottom Right Mirror Calibration Pass");
                    // ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
                    //return flag;
                }
                else
                {
                    if (openFile)
                        WriteTestResultLog();
                    UpdateTestResultPassFailStatus(false, "Bottom Right Mirror Calibration Failed");
                    ErrorMsgDisplay = "Test image is not falling on photodiode.";
                    return false;
                }
            }
            else
            {
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Bottom Right Mirror Calibration Failed");
                ErrorMsgDisplay = "Test image is not falling on photodiode.";
                return false;
            }
            delay = 1;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            SetScokectFlagsToFalse();

            BottomLeftCommand();
            if (readBLvoltage >= 1)
            {
                if (readTRvoltage <= 1 && readBRvoltage <= 1 && readTLvoltage <= 1)
                {
                    UpdateTestResultPassFailStatus(true, "Bottom Left Mirror Calibration Pass");
                    // ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
                    //return flag;
                }
                else
                {
                    if (openFile)
                        WriteTestResultLog();
                    UpdateTestResultPassFailStatus(false, "Bottom Left Mirror Calibration Failed");
                    ErrorMsgDisplay = "Test image is not falling on photodiode.";
                    //return true;
                }
            }
            else
            {
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Bottom Left Mirror Calibration Failed");
                ErrorMsgDisplay = "Test image is not falling on photodiode.";
                //return false;
            }
            Thread.Sleep(1000);
            Increment();
            LogStructureNew.MirrorVerify = "PASS";
            UpdateTestResultPassFailStatus(true, "Mirror Verification is Sucessfull");

            flag = SendCommandToStopImageShow();
            if (!flag)
            {
                LogStructureNew.MirrorVerify = "PASS";
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Stop Image Show Failed");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            delay = 1;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            SetScokectFlagsToFalse();
            flag = SendCommandToBringMirrorToHome();
            if (!flag)
            {
                LogStructureNew.MirrorVerify = "PASS";
                if (openFile)
                    WriteTestResultLog();
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            delay = 1;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            return flag;
        }

        private bool StartImageProcessingTest()
        {
            if (!openFile)
            {
                LogStructureNew.ProjectorFocus = "NOT EXECUTED";
                openFile = true;
                TestFixtureEventLogger.createLogFile();
            }
            else
            {
                LogStructureNew.ProjectorFocus = "NOT EXECUTED";
            }

            LogStructureNew.ProjectorFocus = "FAIL";
            SetScokectFlagsToFalse();
            bool flag = true;

            flag = SendCommandToPorcessHomeScreen();
            if (!flag)
            {
                if (openFile)
                    WriteTestResultLog();
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            #region image processing commands
            ErrorMsgDisplay = "Starting Camera to Capture Image...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            StartiCubeCamera();
            //System.Threading.Thread.Sleep(3000);
            int delay = 4;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            ErrorMsgDisplay = "Starting image show for camera";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            flag = StartImageShowForCamera();
            if (!flag)
                return flag;


            //System.Threading.Thread.Sleep(3000);
            delay = 5;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            ErrorMsgDisplay = "Taking Snapshot of Projected Image, wait 3 seconds";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            TakeSnapofProjectedImage();

            //System.Threading.Thread.Sleep(2000);
            delay = 3;
            flag = CheckTrigger(delay);
            if (!flag)
            {
                UpdateTestResultPassFailStatus(false, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            // Processing Image
            ErrorMsgDisplay = "Processing Imgage";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            ProjectorModel.SetOriginalImg = _mTestProjectImgModel.GetOriginalImage();
            ProjectModel.ThreshHoldLimit = _mTestProjectImgModel.ThreshHoldSimilarity;
            ProjectorModel.ImageTaken();

            Thread.Sleep(1000);

            ErrorMsgDisplay = "Stop image show for camera.";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            flag = SendCommandToStopImageShowForCamera();
            if (!flag)
            {
                if (openFile)
                    WriteTestResultLog();
                return false;
            }

            //compare the values with stad deviation to determine test 
            //  if (ProjectModel.GetMeanValue == _mTestProjectImgModel.GetOrginalImgMeanValue && ProjectModel.GetStdDevValu == _mTestProjectImgModel.GetOrginalImgStdDevValue)
            if (ProjectModel.GetStdDevValu >= 64 && ProjectModel.GetStdDevValu <= 74)
            {
                LogStructureNew.ProjectorFocus = "PASS";
                UpdateTestResultPassFailStatus(true, "Projector in Focus");
                ErrorMsgDisplay = "Projector in Focus";
                //return true;
            }
            else
            {
                LogStructureNew.ProjectorFocus = "FAIL";
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Projector not in Focus");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }

            Increment();
            return flag;
            #endregion
        }

        private bool SendCommandToPorcessHomeScreen()
        {
            serverStatus = CheckForExistanceofIPAddress();
            if (serverStatus.Equals(interrupted))
            {
                // write test data in log file
                // WriteTestResultLog();
                UpdateTestResultPassFailStatus(true, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else if (serverStatus.Equals(notConnected))
            {
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Discovering IP address failed");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else
            {
                // server available
            }
            string homescreencmd = TestFixtureCommands._mhomescreenmirrorcommand;
            homescreencmd = homescreencmd + UpdateHomeScreenXValues + "," + UpdateHomeScreenYValues + "," + "0" + "";

            serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(homescreencmd);
            if (serverStatus.Equals(interrupted))
            {
                // write test data in log file
                // WriteTestResultLog();
                UpdateTestResultPassFailStatus(true, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else if (serverStatus.Equals(notConnected))
            {
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Discovering IP address failed");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else
            {
                return true;
            }

        }

        private bool SendCommandToBringMirrorToHome()
        {
            if (!TestFixtureSocketComm._IsIpAddressFound)
            {
                serverStatus = CheckForExistanceofIPAddress();
                if (serverStatus.Equals(interrupted))
                {
                    // write test data in log file
                    // WriteTestResultLog();
                    UpdateTestResultPassFailStatus(true, "Execution Stopped");
                    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    // write test data in log file
                    if (openFile)
                        WriteTestResultLog();
                    UpdateTestResultPassFailStatus(false, "Discovering IP address failed");
                    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else
                {
                    // server available
                }
            }

            string homescreencmd = TestFixtureCommands._mhomecommand;

            serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(homescreencmd);
            if (serverStatus.Equals(interrupted))
            {
                // write test data in log file
                // WriteTestResultLog();
                UpdateTestResultPassFailStatus(true, "Execution Stopped..");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else if (serverStatus.Equals(notConnected))
            {
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Discovering IP address failed");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool StartLedLighTest()
        {
            if (!openFile)
            {
                LogStructureNew.LedTest = "NOT EXECUTED";
                openFile = true;
                TestFixtureEventLogger.createLogFile();
            }
            else
            {
                LogStructureNew.LedTest = "NOT EXECUTED";
            }
            bool flag;
            LogStructureNew.LedTest = "FAIL";
            ErrorMsgDisplay = "Checking Spectrometer in Progress...";

            string returnmsg;
            SetScokectFlagsToFalse();

            serverStatus = CheckForExistanceofIPAddress();
            if (serverStatus.Equals(interrupted))
            {
                // write test data in log file
                // WriteTestResultLog();
                UpdateTestResultPassFailStatus(true, "Execution Stopped..");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else if (serverStatus.Equals(notConnected))
            {
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Discovering IP address failed");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else
            {
                // server available
            }
            SetScokectFlagsToFalse();

            flag = CheckForSpectrometer();
            if (!flag)
            {
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Spectrometer Discovery Failed");
                this.ErrorMsgDisplay = "Spectrometer Discovery Failed...";
                return false;
            }
            Increment();
            UpdateTestResultPassFailStatus(true, "Spectrometer Discovery Sucessfull");

            // Commented on 01-05-2017

            ErrorMsgDisplay = "Acquiring Dark Spectrum in Progress...";
            returnmsg = AcquireDarkSpectrum();
            if (!returnmsg.Equals("NoErrors"))
            {
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Acquiring Dark Spectrum Failed");
                return false;
            }

            Increment();
            UpdateTestResultPassFailStatus(true, "Acquired Dark Spectrum");

            _mTestFixtureSpectrum.ReadCalibrationFile();
            ErrorMsgDisplay = "Calibration File reading completed";

            flag = CommandToTurnOnRedLedAndRedValues();
            if (!flag)
            {
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "RED LED Failed to turn ON");
                ErrorMsgDisplay = "RED LED Failed to turn ON";
                return false;
            }
            IsRedColorOk = true;
            Increment();
            UpdateTestResultPassFailStatus(true, "RED LED Turned ON");
            SetScokectFlagsToFalse();

            flag = CommandGreenLedToTurnOnAndReadValues();
            if (!flag)
            {
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(true, "GREEN LED Failed to turn ON");
                //                this.ErrorMsgDisplay = "Failed to Turn on Green LED....";
                return false;
            }
            IsGreenColorOk = true;
            Increment();
            UpdateTestResultPassFailStatus(true, "GREEN LED turned ON");

            SetScokectFlagsToFalse();
            flag = CommandBlueLedToTurnOnAndReadValues();
            if (!flag)
            {
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "BLUE LED Failed to turn ON");
                return false;
            }
            IsBlueColorOk = true;
            Increment();
            UpdateTestResultPassFailStatus(true, "BLUE LED turned ON");

            SetScokectFlagsToFalse();
            flag = CommandToTurnOnWhiteLedAndReadValues();
            if (!flag)
            {
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "WHITE LED Failed to turn ON");
                return false;
            }
            IsWhiteColorOk = true;
            Increment();
            UpdateTestResultPassFailStatus(true, "WHITE LED turned ON");
            Thread.Sleep(1000);
            TurnOffSpectrometer();
            ErrorMsgDisplay = "Turning OFF LED's";
            SetScokectFlagsToFalse();
            LogStructureNew.LedTest = "PASS";
            flag = CommandToTurnOffLeds();
            if (!flag)
            {
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(true, "Turning OFF LED Failed");
                return false;
            }

            //Increment();
            _mRedFlag = false;
            _mGreenFlag = false;
            _mBlueFlag = false;
            _mWhiteFlag = false;
            UpdateTestResultPassFailStatus(true, "Turning OFF LED Passed");

            System.Threading.Thread.Sleep(2000);


            // Increment();
            ErrorMsgDisplay = "Erasing the test files...";

            if (fileRemove)
            {
                bool deleteFile = TestFixtureSocketComm.DeleteUploadedFile("image");
            }
            // Delete uploaded test files..
            System.Threading.Thread.Sleep(2000);
            // bool deleteFile = TestFixtureSocketComm.DeleteUploadedFile("bandwidth"); // Already removed
            System.Threading.Thread.Sleep(2000);
            UpdateTestResultPassFailStatus(true, "Test Files Erased");
            ErrorMsgDisplay = "Test files Erased";
            Thread.Sleep(1000);
            //ErrorMsgDisplay = "";
            //Increment();

            return flag;
        }

        private void WriteTestResultLog()
        {
            TestFixtureEventLogger.appendLog(true);
            TestFixtureEventLogger.closeLog();
        }

        private void CheckUsbConnection(bool aFlag, string aMsg)
        {
            UpdateBackgroundColor = System.Windows.Media.Brushes.Yellow;
            UpdatePassFailResult = aMsg;
        }

        internal void UpdateTestResultPassFailStatus(bool aFlag, string aMsg)
        {
            if (!aFlag)
            {
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);

                openFile = false;
                //isStopbtnPressed = true;
                //logDataArray.Add(logStructure);      
                UpdatePassFailResult = aMsg;
                frmTestFixture.Instance.WriteToLog(UpdatePassFailResult, ApplicationConstants.TraceLogType.Error);
                UpdateBackgroundColor = System.Windows.Media.Brushes.Red;

                Thread.Sleep(1000);
                //this.OnExternalStopTrigger(); TODO: Keith uncomment out 
                //Thread.Sleep(1000);
                //this.OnChangeExternalTrigger();
                Reset();
                frmTestFixture.Instance.ResetProgressStatusBar(0);

                //SetVersionNumberTextbox("read version number"); //KD Added
                //SetVoltageTextbox("read voltage"); //KD Added
                //SetModelNumberTextbox("read model number"); //KD Added
                //SetSerialNumberTextbox("read serial number"); //KD Added

                UpdateBackgroundColor = System.Windows.Media.Brushes.GreenYellow;
                //UpdatePassFailResult= "Press Green Button to Restart";
                //ErrorMsgDisplay = UpdatePassFailResult;
                //WriteToLog(UpdatePassFailResult);
                //frmTestFixture.Instance.informationMessageCounter = 0;
                //frmTestFixture.Instance.warningMessageCounter = 0;
                //frmTestFixture.Instance.errorMessageCounter = 0;
            }
            else
            {
                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Green);

                UpdateBackgroundColor = System.Windows.Media.Brushes.GreenYellow;
                UpdatePassFailResult = aMsg;
                if (frmTestFixture.Instance != null)
                    frmTestFixture.Instance.WriteToLog(UpdatePassFailResult, ApplicationConstants.TraceLogType.Information);

            }
        }

        private void OverallTestResultPassFailStatus(bool aFlag, string aMsg)
        {
            frmTestFixture.Instance.SetStatusIndicatorVisibleProperty(true);

            if (!aFlag)
            {

                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);

                openFile = false;
                //isStopbtnPressed = true;
                //logDataArray.Add(logStructure);      
                UpdatePassFailResult = aMsg;
                frmTestFixture.Instance.WriteToLog(UpdatePassFailResult, ApplicationConstants.TraceLogType.Error);
                UpdateBackgroundColor = System.Windows.Media.Brushes.Red;

                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Red);
                
                //this.OnExternalStopTrigger();
                Thread.Sleep(1000);
                Reset();
                //this.OnChangeExternalTrigger();
                UpdateBackgroundColor = System.Windows.Media.Brushes.GreenYellow;
                UpdatePassFailResult = "Press Green Button to Restart";
                frmTestFixture.Instance.WriteToLog(UpdatePassFailResult, ApplicationConstants.TraceLogType.Information);

                frmTestFixture.Instance.ReStartThread();
                //StopExecuting();

                ResetDaqBoardPort();

                frmTestFixture.Instance.informationMessageCounter = 0;
                frmTestFixture.Instance.warningMessageCounter = 0;
                frmTestFixture.Instance.errorMessageCounter = 0;

                frmTestFixture.Instance.ResetProgressStatusBar(0);
            }
            else
            {

                frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(System.Drawing.Color.Green);

                openFile = false;
                //isStopbtnPressed = true;
                UpdateBackgroundColor = System.Windows.Media.Brushes.GreenYellow;
                UpdatePassFailResult = aMsg;
                frmTestFixture.Instance.WriteToLog(UpdatePassFailResult, ApplicationConstants.TraceLogType.Information);
      
                Thread.Sleep(1000);
                //this.OnExternalStopTrigger();
                Reset();
                //this.OnChangeExternalTrigger();
                UpdateBackgroundColor = System.Windows.Media.Brushes.GreenYellow;
                UpdatePassFailResult = "Press Green Button to Restart";
                frmTestFixture.Instance.WriteToLog(UpdatePassFailResult, ApplicationConstants.TraceLogType.Information);
            }
        }

        private bool CheckForSpectrometer()
        {

            bool isavailable = _mTestFixtureSpectrum.ScannerForSpectrometer();
            if (isavailable)
            {
                this.ErrorMsgDisplay = "Spectrometer discovery successful..";
                frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);
                return isavailable;
            }
            else
                return isavailable;
        }
        public string AcquireDarkSpectrum()
        {
            _mTestFixtureSpectrum.DiscoverSpetroMeter();//.SpectrunBeforeSwicthOn();
            string msg = _mTestFixtureSpectrum.SetAquisitionParameter();
            frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Information);
            if (!msg.Equals("NoErrors"))
                return msg;
            msg = _mTestFixtureSpectrum.AcquireDarkSpectrum();
            frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Information);
            return msg;
        }
        private string AcquireSampleSpectrun()
        {
            string msg = _mTestFixtureSpectrum.AcquireSampleSpectrum();
            frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Information);
            return msg;
        }
        private void StartreadingSerialNumber()
        {
            bool retvalue = ReadSerialNumberFromScanner();
            if (retvalue)
            {
                ErrorMsgDisplay = "Please Scan for Model Number...";
                frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);
                StartReadingModelNumber();

            }
            else
            {
                ErrorMsgDisplay = "Problem in reading Serial Number, Please check COM port...!";
                frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Warning);
            }
        }
        private bool StartReadingModelNumber()
        {

            this.ReadModelNumber = "";
            bool retvalu = ReadModelNumberAfterScan();
            return retvalu;
        }
        public TestFixtureTestMainModel TestMainModel
        {
            get
            {
                return _mTestFixtureTestMainModel;
            }
            set
            {
                _mTestFixtureTestMainModel = value;
                OnPropertyChanged("TestMainModel");
            }
        }

        public string UpdateSerialNumber
        {
            get { return TestMainModel.UpdateSerialNumber; }
            set
            {
                TestMainModel.UpdateSerialNumber = value;
                OnPropertyChanged("UpdateSerialNumber");
            }

        }

        public string UpdatePassFailResult
        {
            get { return TestMainModel.UpdatePassFailResult; }
            set
            {
                TestMainModel.UpdatePassFailResult = value;

                if (!string.IsNullOrEmpty(value))
                {
                    if (frmTestFixture.Instance != null)
                    {
                        //Keith Dudley
                        //frmTestFixture.Instance.SetUpdatePassFailResultTextBox(value);
                        frmTestFixture.Instance.SetErrorMessageDisplayTextBox(value);
                    }
                }

                OnPropertyChanged("UpdatePassFailResult");
            }
        }
        public System.Windows.Media.Brush UpdateBackgroundColor
        {
            get { return TestMainModel.UpdateBackgroundColor; }
            set
            {
                TestMainModel.UpdateBackgroundColor = value;
                OnPropertyChanged("UpdateBackgroundColor");
            }
        }
        public string ReadModelNumber
        {
            get { return TestMainModel.ReadModelNumber; }
            set
            {
                TestMainModel.ReadModelNumber = value;
                OnPropertyChanged("ReadModelNumber");
            }
        }

        public string ReadSerialNumber
        {
            get { return TestMainModel.UpdateSerialNumber; }
            set
            {
                TestMainModel.UpdateSerialNumber = value;
                OnPropertyChanged("UpdateSerialNumber");
            }
        }


        public string VersionNumber
        {
            get { return TestMainModel.VersionNumber; }
            set
            {
                TestMainModel.VersionNumber = value;
                OnPropertyChanged("VersionNumber");
            }

        }

        public string UpdateVoltage
        {
            get { return TestMainModel.UpdateVoltage; }
            set
            {
                TestMainModel.UpdateVoltage = value;
                OnPropertyChanged("UpdateVoltage");
            }

        }
        public bool IsWhiteColorOk
        {
            get { return TestMainModel.IsWhiteColorOk; }
            set
            {
                TestMainModel.IsWhiteColorOk = value;

                frmTestFixture.Instance.SetWhiteCheckbox(TestMainModel.IsWhiteColorOk);

                OnPropertyChanged("IsWhiteColorOk");
            }

        }
        public bool IsGreenColorOk
        {
            get { return TestMainModel.IsGreenColorOk; }
            set
            {
                TestMainModel.IsGreenColorOk = value;

                frmTestFixture.Instance.SetGreenCheckbox(TestMainModel.IsGreenColorOk);

                OnPropertyChanged("IsGreenColorOk");
            }
        }

        public bool IsBlueColorOk
        {
            get { return TestMainModel.IsBlueColorOk; }
            set
            {
                TestMainModel.IsBlueColorOk = value;

                frmTestFixture.Instance.SetBlueCheckbox(TestMainModel.IsBlueColorOk);

                OnPropertyChanged("IsBlueColorOk");
            }
        }

        public bool IsRedColorOk
        {
            get { return TestMainModel.IsRedColorOk; }
            set
            {
                TestMainModel.IsRedColorOk = value;

                frmTestFixture.Instance.SetRedCheckbox(TestMainModel.IsRedColorOk);

                OnPropertyChanged("IsRedColorOk");
            }
        }

        public string GetVersionNumber
        {
            get { return TestMainModel.GetVersionNumber; }
            set
            {
                TestMainModel.GetVersionNumber = value;
                OnPropertyChanged("GetVersionNumber");
            }
        }

        public bool IsVoltageOK
        {
            get { return TestMainModel.IsVoltageOK; }
            set
            {
                TestMainModel.IsVoltageOK = value;

                frmTestFixture.Instance.SetVoltageCheckbox(TestMainModel.IsVoltageOK);

                OnPropertyChanged("IsVoltageOK");
            }
        }
        //reads the serial number from barcode scanner
        private bool ReadSerialNumberFromScanner()
        {
            try
            {
                //string comPortNumber = CommPortNUmber;
                string comPortNumber = commPortNumber;
                if (string.IsNullOrEmpty(comPortNumber) != true)
                {
                    CheckObjectForNull();
                    _mTestFixtureSerialComm.ListenToSerialPort(comPortNumber);
                    bool flag = _mTestFixtureSerialComm.OpenAndRead();
                    do
                    {
                        _mTestFixtureSerialComm.flag = 0;

                        if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                            return false;

                        if (isStopbtnPressed)
                            break;

                    } while ((_mTestFixtureSerialComm._misValueRead != true) && (_mTestFixtureSerialComm._mdatanotread != true));

                    _mTestFixtureSerialComm._misValueRead = false;
                    _mTestFixtureSerialComm._mdatanotread = false;
                    UpdateSerialNumber = _mTestFixtureSerialComm.GetBarcodeScanInfo();
                }
                else
                {
                    this.ErrorMsgDisplay = "Please specify COM port number to continue bar code scanning...";
                    frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                string msg = e.Message; //test will fail
                this.ErrorMsgDisplay = msg.ToString();
                frmTestFixture.Instance.WriteToLog("ReadSerialNumberFromScanner: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
                return false;
            }
            return true;
        }
        //reads the serial number from barcode scanner
        // but at sometime it has to come out from this loop? is it after time out????
        private bool ReadModelNumberAfterScan()
        {
            try
            {
                ErrorMsgDisplay = "Please Scan for Model Number...";
                //string comPortNumber = CommPortNUmber;
                string comPortNumber = commPortNumber;
                if (string.IsNullOrEmpty(comPortNumber) != true)
                {

                    CheckObjectForNull();

                    _mTestFixtureSerialComm.ListenToSerialPort(comPortNumber);
                    bool flag = _mTestFixtureSerialComm.OpenAndRead();
                    do
                    {
                        _mTestFixtureSerialComm.flag = 1;


                        if (isStopbtnPressed)
                            break;

                    } while ((_mTestFixtureSerialComm._misValueRead != true) && (_mTestFixtureSerialComm._mdatanotread != true));
                    _mTestFixtureSerialComm._misValueRead = false;
                    _mTestFixtureSerialComm._mdatanotread = false;
                    ReadModelNumber = _mTestFixtureSerialComm.GetBarcodeScanInfo();

                    if (!string.IsNullOrEmpty(ReadModelNumber))
                        LogStructureNew.ModelNo = ReadModelNumber;
                }
                else
                {
                    ErrorMsgDisplay = "Please specify COM port number to continue bar code scanning...";
                    return false;
                }
            }
            catch (Exception e)
            {
                string msg = e.Message; //test will fail
                ErrorMsgDisplay = msg.ToString();
                frmTestFixture.Instance.WriteToLog("ReadModelNumberAfterScan: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
                return false;
            }
            return true;
        }

        private bool MapModelNumber()
        {
            if (!isStopbtnPressed)
            {
                //Char delimiter = '\\';

                //var parseModel = ReadModelNumber.Split('\\');

                string parseModel = Regex.Replace(ReadModelNumber, @"\t|\n|\r", "");

                string modelnumber = parseModel;

                //string modelnumber = ReadModelNumber.ToString();
                if (modelnumber.Equals(_model.ModelNumber120V50Ft) || modelnumber.Equals(_model.ModelNumber120V150Ft))
                {
                    string impedanceCheck = ReadImpedanceVolt();

                        if (impedanceCheck != string.Empty)
                        {
                            if (impedanceCheck.Equals(_model.ModelNumber120V50Ft)) // Change after light connect
                            {
                                UpdateVoltage = "120VAC";

                                frmTestFixture.Instance.SetVoltageTextbox(UpdateVoltage);

                                UpdateSerialNumber = Regex.Replace(UpdateSerialNumber, @"\t|\n|\r", "");
                                LogStructureNew.SerialNo = UpdateSerialNumber;
                                LogStructureNew.Varient = UpdateVoltage;
                                //WriteTestResultLog();
                                UpdateTestResultPassFailStatus(true, "120 VAC Voltage Matched with Impedance");
                                Thread.Sleep(1000);
                                return true;
                            }

                            UpdateVoltage = "Invalid";
                            frmTestFixture.Instance.SetVoltageTextbox(UpdateVoltage);

                            LogStructureNew.SerialNo = UpdateSerialNumber;
                            LogStructureNew.Varient = UpdateVoltage;
                            if (openFile)
                                WriteTestResultLog();
                            UpdateTestResultPassFailStatus(false, "120 VAC Voltage didn't Match with Impedance");
                            return false;

                        }
                        else
                        {
                            UpdateVoltage = "Impedance empty";
                            frmTestFixture.Instance.SetVoltageTextbox(UpdateVoltage);

                            LogStructureNew.SerialNo = UpdateSerialNumber;
                            LogStructureNew.Varient = UpdateVoltage;
                            if (openFile)
                                WriteTestResultLog();
                            UpdateTestResultPassFailStatus(false, "Impedance empty");
                            return false;
                        }
                }
                else if (modelnumber.Equals(_model.ModelNumber12V50Ft) || modelnumber.Equals(_model.ModelNumber12V150Ft))
                {
                    string impedanceCheck = ReadImpedanceVolt();

                    if (impedanceCheck != string.Empty)
                    {
                        if (impedanceCheck.Equals(_model.ModelNumber120V50Ft)) // Change after light connect
                        {
                            UpdateVoltage = "12VAC";
                            frmTestFixture.Instance.SetVoltageTextbox(UpdateVoltage);

                            LogStructureNew.SerialNo = UpdateSerialNumber;
                            LogStructureNew.Varient = UpdateVoltage;
                            if (openFile)
                                WriteTestResultLog();
                            UpdateTestResultPassFailStatus(true, "12VAC Voltage Matched with Impedance");
                            Thread.Sleep(1000);
                            return false;
                        }

                        UpdateVoltage = "Invalid";
                        frmTestFixture.Instance.SetVoltageTextbox(UpdateVoltage);

                        LogStructureNew.SerialNo = UpdateSerialNumber;
                        LogStructureNew.Varient = UpdateVoltage;
                        if (openFile)
                            WriteTestResultLog();
                        //UpdateTestResultPassFailStatus(false, "12 VAC Voltage didn't Match with Impedance");
                        return false;                 
                    }
                    else
                    {
                        UpdateVoltage = "Impedance Empty";
                        frmTestFixture.Instance.SetVoltageTextbox(UpdateVoltage);

                        LogStructureNew.SerialNo = UpdateSerialNumber;
                        LogStructureNew.Varient = UpdateVoltage;
                        if (openFile)
                            WriteTestResultLog();
                        //UpdateTestResultPassFailStatus(false, "12 VAC Voltage didn't Match with Impedance");
                        return false;
                    }

                }
                else
                {
                    UpdateVoltage = "Invalid Model Number";
                    frmTestFixture.Instance.SetVoltageTextbox(UpdateVoltage);

                    LogStructureNew.SerialNo = UpdateSerialNumber;
                    LogStructureNew.Varient = UpdateVoltage;
                    if (openFile)
                        WriteTestResultLog();
                    // UpdateTestResultPassFailStatus(false, "Model Number Match Failed");
                    return false;
                }
            }

            return true;
        }

        private string SendCommandToSolenoid()
        {
            ErrorMsgDisplay = "PLC Solenoid Turning ON...";

            string errmsg = _mtestfixturedaq.SendOnCommandToSolenoid(1);
            if (errmsg.Equals("NoErrors"))
                return errmsg;
            else
                return errmsg;
        }
        private string DoPlcSolenoidPairing()
        {

            string errmsg = _mtestfixturedaq.PLCSolenoidPairing(1);
            if (errmsg.Equals("NoErrors"))
                return errmsg;
            else
                return errmsg;
        }
        //object obj, EventArgs e
        private string StopDHCPSolenoid()
        {
            string value = _mtestfixturedaq.SendOnCommandToSolenoid(0);
            if (value.Equals("NoErrors"))
                return value;
            else
                return value;
        }

        #endregion
        #region
        private string EstablishEtherNetInterfaceWithDevice()
        {
            // bool discovered = false;
            // string serverCheck = null;
            try
            {
                //serverStatus = TestFixtureSocketComm.DiscoverPentairServer();
                serverStatus = TestFixtureSocketComm.DiscoverPentairServer(false);
                if (serverStatus.Equals(connected))
                {
                    ErrorMsgDisplay = "Discovering Server Completed Sucessfuly...";
                    serverIpAddess = TestFixtureSocketComm.GetServerIpAddress();
                }
            }
            catch (Exception e)
            {
                ErrorMsgDisplay = e.Message.ToString();
                frmTestFixture.Instance.WriteToLog("EstablishEtherNetInterfaceWithDevice: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
            return serverStatus;
        }

        private void ActuateHallSensorsAndWallAdapterButton()
        {
            //probaly through daq..need to determine
        }

        private bool CommandSolenoidToLock()
        {
            string returnvalue = _mtestfixturedaq.SendOnCommandToSolenoid(1);
            if (!returnvalue.Equals("NoErrors"))
            {
                ErrorMsgDisplay = returnvalue.ToString();
                return false;
            }
            else
            {
                ErrorMsgDisplay = "Solenoid Command Executed Successfully";

            }
            return true;
        }
        private void CheckObjectForNull()
        {
            if (_mTestFixtureSerialComm == null)
                _mTestFixtureSerialComm = new TestFixtureSerialPortComm();
            else
                return;
        }
        void HandShakeWithDevice()
        {
            if (_mTestFixtureSerialComm == null)
                _mTestFixtureSerialComm = new TestFixtureSerialPortComm();
        }
        #endregion
        #endregion

        #region get the image command and process it

        private void ExecuteImageCommand()
        {
            try
            {
                string getImageCommand = GetImageCommand();
                //before call getProductType command to complete pairing process
                serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(getImageCommand);
            }
            catch (Exception e)
            {
                ErrorMsgDisplay = e.Message.ToString();
                frmTestFixture.Instance.WriteToLog("ExecuteImageCommand: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
        }
        private string GetImageCommand()
        {
            string imageCommand = "cmd=startShow" + "&" + "id=" + "&" + "idType=" + "&" + "filename=" + "&" + "isloop=" + "&" + "schedule";
            return imageCommand;
        }
        public void StopListeningToEvent()
        {
            _mtestfixturedaq.PressStopButonOnFixture();
            
        }

        #endregion

        #region
        private string CheckForExistanceofIPAddress()
        {
            bool flag = false;
            if (serverIpAddess == null) //changed
            {
                ErrorMsgDisplay = "Acquiring Server IP Addess...";
                serverStatus = EstablishEtherNetInterfaceWithDevice();

                if (serverStatus.Equals(notConnected))
                {
                    LogStructureNew.DeviceDiscover = "FAIL";
                    ErrorMsgDisplay = "Failed to Detect Server...";
                }
                else if (serverStatus.Equals(connected))
                {
                    LogStructureNew.DeviceDiscover = "PASS";
                    flag = true;
                }
                else
                {
                    LogStructureNew.DeviceDiscover = "FAIL";
                    flag = false;
                }
            }
            else
            {
                TestFixtureSocketComm.SetServerIpAddress(serverIpAddess);
                flag = true;
                serverStatus = "AVAILABLE";
            }
            return serverStatus;
        }

        internal void StopExecuting()
        {

           if (frmTestFixture.Instance.backgroundWorker1.IsBusy)
                frmTestFixture.Instance.backgroundWorker1.CancelAsync();

            //_mtestfixturedaq.SendOnCommandToSolenoid(0);
            //_mtestfixturedaq.SendOnCommandToSolenoid(0);
            //_mtestfixturedaq.TurnOn120VRelayOn(0);
            //_mtestfixturedaq.TurnOn15VRelayOn(0);
            //_mtestfixturedaq.PLCSolenoidPairing(0);
        }
        #endregion

        #region child thread for Start and Stop
        public void ListenToStopEvent(object sender)
        {
            EventWaitHandle stopHandle = new AutoResetEvent(true);
            short DataValue;
            firstTime = 0;
            if (isStartPressed && !isStopbtnPressed)
            {
                while (true)
                {
                    ULStat = DaqBoard.DIn(MccDaq.DigitalPortType.FirstPortA, out DataValue);
                    if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                    {

                        if (((DataValue & (1 << 1)) == 0))
                        {
                            DaqBoard.DisableEvent(MccDaq.EventType.AllEventTypes);

                           // isStopbtnPressed = true;
                            isStartPressed = false;
                            if (!Startthread.IsAlive)
                                Startthread.Start();

                            if (!isTestExecutionFailure)
                            {
                                OnExternalStopTrigger();

                                //ResetDaqBoardPort(sender);

                                //Startthread.Abort();

                                //stopthread.Abort();
                                //ErrorMsgDisplay = "Execution Stopped";
                            }
                        }
                        // else
                        //   isStopbtnPressed = false;
                    }
                }
            }
        }

        public void PushButton()
        {
            firstTime = 1;
            MccDaq.ErrorInfo ULStat;
            string return_value = MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();
            MccDaq.DigitalLogicState DataValue;

            mainTabSelected = false;

            try
            {
                while (true)
                {
                    if (frmTestFixture.Instance != null)
                    {

                        if (frmTestFixture.Instance.tabControl2.IsHandleCreated)
                        {
                            // frmTestFixture.Instance.tabControl2.Invoke((MethodInvoker)delegate

                            //{

                            frmTestFixture.Instance.tabControl2.Invoke((MethodInvoker)delegate
                            {
                                if(frmTestFixture.Instance.tabControl2.SelectedTab.Text == "Main")
                                {
                                    mainTabSelected = true;
                                }
                                else
                                {
                                    mainTabSelected = false;
                                }

                            });

                            if (!isTestExecutionFailure && mainTabSelected)
                               {

                                   ULStat = DaqBoard.DBitIn(MccDaq.DigitalPortType.FirstPortA, 0, out DataValue);

                                   if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                                   {
                                       if (DataValue == MccDaq.DigitalLogicState.Low)
                                       {
                                       //ResetStatusIndicator();

                                       isStartPressed = true;
                                       //isStopbtnPressed = false;

                                       frmTestFixture.Instance.tabControl2.Invoke((MethodInvoker)delegate

                                       {

                                           frmTestFixture.Instance.pagevm.ResetStatusIndicator();

                                           frmTestFixture.Instance.Refresh();

                                           //ucHostContainer.Instance.txtModelNumber.Text = message;
                                           if (!isTestExecutionFailure && frmTestFixture.Instance.tabControl2.SelectedTab.Text == "Main")
                                                {
                                                   if (!frmTestFixture.Instance.backgroundWorker1.IsBusy)
                                                      frmTestFixture.Instance.backgroundWorker1.RunWorkerAsync(frmTestFixture.BW_OPERATIONS.OPERATION_START_TEST_SEQUENCE);

                                                    //OnChangeExternalTrigger();
                                                }
                                            });

                                           return;

                                       }
                                       else
                                       {
                                           isStartPressed = false;

                                           //if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                                           //    break;

                                          
                                       }
                                   }
                            //       return;
                               }

                           //});
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMsgDisplay = e.Message.ToString();
                frmTestFixture.Instance.WriteToLog("PushButton: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
        }

        public void ListenToStartEvent(object sender)
        {
            firstTime = 1;
            MccDaq.ErrorInfo ULStat;
            string return_value = MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();
           // short DataValue = 0;
            MccDaq.DigitalLogicState DataValue;
            try
            {
                if ((!isStartPressed && !isStopbtnPressed) || (!isStartPressed && isStopbtnPressed))
                {
                    while (true)
                    {    
                        Thread.Sleep(500);
                        //ULStat = DaqBoard.DIn(MccDaq.DigitalPortType.FirstPortA, out DataValue);
                        //ULStat = DaqBoard.DConfigBit(MccDaq.DigitalPortType.FirstPortA, 1, MccDaq.DigitalPortDirection.DigitalIn);
                        ULStat = DaqBoard.DBitIn(MccDaq.DigitalPortType.FirstPortA, 0, out DataValue);
                        // ULStat = DaqBoard.DOut(MccDaq.DigitalPortType.FirstPortA, DataValue);
                        if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                        {
                           if(DataValue == MccDaq.DigitalLogicState.Low)
                            //if (((DataValue & (1 << 0)) == 0))
                            {
                                isStartPressed = true;
                                isStopbtnPressed = false;

                                if (!stopthread.IsAlive)
                                    stopthread.Start();

                                if (!isTestExecutionFailure)
                                    OnChangeExternalTrigger();

                                break;
                            }
                            else
                            {
                                //Startthread.Abort();
                                isStartPressed = false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMsgDisplay = e.Message.ToString();
                frmTestFixture.Instance.WriteToLog("ListenToStartEvent: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
        }

    
        #endregion

        #region Mirror commands

        public ICommand MoveMirrorToTopRight
        {
            get
            {
                return new RelayCommand(CanExecute, MoveToTopRightPosition);
            }
        }

        private void  MoveToTopRightPosition(object p)
        {
            TopRightMirrorCommand();
        }

        internal bool TopRightMirrorCommand()
        {
            bool flag = false;

            //CheckForExistanceofIPAddress();
            if (!TestFixtureSocketComm._IsIpAddressFound)
                return false;

            string command = TestFixtureCommands._mtoprightcommand;
            command = command + UpdatetrmirrorXvalues + "," + UpdatetrmirrorYvalues + "," + "0" + "";

            serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(command);
            if (serverStatus.Equals(notConnected))
            {
                ErrorMsgDisplay = "Failed to Set Top Right Mirror Position";
                flag = false;
            }
            else
            {
                flag = true;
            }

            System.Threading.Thread.Sleep(4000);
            ReadSensorsVoltages();

            if (frmTestFixture.Instance != null)
            {
                frmTestFixture.Instance.txtTopRightX.Text = UpdatetrmirrorXvalues;
                frmTestFixture.Instance.txtTopRightX.Text = UpdatetrmirrorYvalues;
            }

            return flag;
        }

        internal void CheckForTopRightPDiodeVoltage()
        {
            float voltage = _mtestfixturedaq.ReadTopRightPDiodeVoltage();
            readTRvoltage = voltage;
            ReadTopRightVoltage = voltage;
            frmTestFixture.Instance.tbTopRightVoltageReading.Text = readTRvoltage.ToString();

            //MirrorPositionValidation(ApplicationConstants.MirrorPosition.TopRight);
        }
        private float ReadTopRightVoltage
        {
            get { return ImageModel.ReadTopRightVoltage; }
            set
            {
                ImageModel.ReadTopRightVoltage = value;
                OnPropertyChanged("ReadTopRightVoltage");
            }
        }


        public ICommand MoveMirrorToTopLeft
        {
            get
            {
                return new RelayCommand(CanExecute, MoveToTopLeftPosition);
            }
        }

        private void MoveToTopLeftPosition(object p)
        {
            TopLeftMirrorCOmmand();
        }

        internal bool TopLeftMirrorCOmmand()
        {
            bool flag = false;

            //CheckForExistanceofIPAddress();
            if (!TestFixtureSocketComm._IsIpAddressFound)
                return false;

            string topleftcommand = TestFixtureCommands._mtoplefttcommand;
            topleftcommand = topleftcommand + UpdatetLmirrorXvalues + "," + UpdatetLmirrorYvalues + "," + "0" + "";

            TestFixtureSocketComm._isInnerloop = false;
            TestFixtureSocketComm._isOuterloop = false;
            TestFixtureSocketComm.ResetRetryCount();

            serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(topleftcommand);
            if (serverStatus.Equals(notConnected))
            { 
                ErrorMsgDisplay = "Failed to Set Top Left Mirror Position";
                flag = false;
            }
            else
            {
                flag = true;
            }

            System.Threading.Thread.Sleep(4000);
            ReadSensorsVoltages();

            return flag;
        }
        internal void CheckForTopLeftPDiodeVoltage()
        {
            //System.Threading.Thread.Sleep(4000);
            float voltage = _mtestfixturedaq.ReadTopLeftPDiodeVoltage();
            readTLvoltage = voltage;
            ReadTopLeftVoltage = voltage;
            frmTestFixture.Instance.tbTopLeftVoltageReading.Text = readTLvoltage.ToString();

            //MirrorPositionValidation(ApplicationConstants.MirrorPosition.TopLeft);
        }

        internal void CheckForPDiodeVoltage(ApplicationConstants.SensorChannel channel)
        {
            float voltage = -99f;

            voltage = _mtestfixturedaq.ReadSensorChanel(channel);

            if (channel == ApplicationConstants.SensorChannel.TopRightSensor)
            {
                readTRvoltage = voltage;
                ReadTopRightVoltage = voltage;

                if(frmTestFixture.Instance.tbTopRightVoltageReading.IsHandleCreated)
                frmTestFixture.Instance.tbTopRightVoltageReading.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.tbTopRightVoltageReading.Text = readTRvoltage.ToString(); });
            }
            else if (channel == ApplicationConstants.SensorChannel.TopLeftSensor)
            {
                readTLvoltage = voltage;
                ReadTopLeftVoltage = voltage;

                if (frmTestFixture.Instance.tbTopLeftVoltageReading.IsHandleCreated)
                    frmTestFixture.Instance.tbTopLeftVoltageReading.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.tbTopLeftVoltageReading.Text = readTLvoltage.ToString(); });
            }
            else if(channel == ApplicationConstants.SensorChannel.BottomRightSensor)
            {
                readBRvoltage = voltage;
                ReadBottomRightVoltage = voltage;

                if (frmTestFixture.Instance.tbBottomRightVoltageReading.IsHandleCreated)
                    frmTestFixture.Instance.tbBottomRightVoltageReading.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.tbBottomRightVoltageReading.Text = readBRvoltage.ToString(); });
            }
            else if(channel == ApplicationConstants.SensorChannel.BottomLeftSensor)
            {
                readBLvoltage = voltage;
                ReadBottomLeftVoltage = voltage;

                if (frmTestFixture.Instance.tbBottomLeftVoltageReading.IsHandleCreated)
                    frmTestFixture.Instance.tbBottomLeftVoltageReading.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.tbBottomLeftVoltageReading.Text = readBLvoltage.ToString(); });
            }
            else
            { 
                //Invalid Channel
                voltage = -99f;
            }
        }


        private float ReadTopLeftVoltage
        {
            get { return ImageModel.ReadTopLeftVoltage; }
            set
            {
                ImageModel.ReadTopLeftVoltage = value;
                OnPropertyChanged("ReadTopLeftVoltage");
            }
        }
        public ICommand MoveMirrorToBottomRight
        {
            get
            {
                return new RelayCommand(CanExecute, MoveBottomRightPosition);
            }

        }

        private void MoveBottomRightPosition(object p)
        {
            BottomRightMirrorCommand();
        }

        internal bool BottomRightMirrorCommand()
        {
            bool flag = false;

            //CheckForExistanceofIPAddress();
            if (!TestFixtureSocketComm._IsIpAddressFound)
                return false;

            string mirrorcommand = TestFixtureCommands._mbottomrightcommand;
            mirrorcommand = mirrorcommand + UpdateBRMirrorXValues + "," + UpdateBRMirrorYValues + "," + "0" + "";

            TestFixtureSocketComm._isInnerloop = false;
            TestFixtureSocketComm._isOuterloop = false;
            TestFixtureSocketComm.ResetRetryCount();

            serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(mirrorcommand);
            if (serverStatus.Equals(notConnected))
            { 
                ErrorMsgDisplay = "Failed to Set Bottom Right Mirror Position";
                flag = false;
            }
            else
            {
                flag = true;
            }

            System.Threading.Thread.Sleep(4000);
            ReadSensorsVoltages();

            return flag;
        }

        internal void CheckForBottomRightPDiodeVoltage()
        {
            //System.Threading.Thread.Sleep(4000);
            float voltage = _mtestfixturedaq.ReadBottomRightPDiodeVoltage();
            readBRvoltage = voltage;
            ReadBottomRightVoltage = voltage;
            frmTestFixture.Instance.tbBottomRightVoltageReading.Text = readBRvoltage.ToString();

            //MirrorPositionValidation(ApplicationConstants.MirrorPosition.BottomRight);
        }

        internal void ReadSensorsVoltages()
        {
            frmTestFixture.Instance.pagevm._mtestfixturedaq.ConfigureDaqToSingleEnded();

            CheckForPDiodeVoltage(ApplicationConstants.SensorChannel.TopRightSensor);
            CheckForPDiodeVoltage(ApplicationConstants.SensorChannel.TopLeftSensor);
            CheckForPDiodeVoltage(ApplicationConstants.SensorChannel.BottomRightSensor);
            CheckForPDiodeVoltage(ApplicationConstants.SensorChannel.BottomLeftSensor);
        }

        private float ReadBottomRightVoltage
        {
            get { return ImageModel.ReadBottomRightVoltage; }
            set
            {
                ImageModel.ReadBottomRightVoltage = value;
                OnPropertyChanged("ReadBottomRightVoltage");
            }
        }
        public ICommand MoveMirrorToBottomLetf
        {
            get
            {
                return new RelayCommand(CanExecute, MoveBottomLeftPosition);
            }
        }

        private void MoveBottomLeftPosition(object p)
        {
            BottomLeftCommand();
        }

        internal bool BottomLeftCommand()
        {
            bool flag = false;

            if (!TestFixtureSocketComm._IsIpAddressFound)
                return false;

            //CheckForExistanceofIPAddress();

            string bottomleftcommand = TestFixtureCommands._mbottomlefttcommand;
            bottomleftcommand = bottomleftcommand + UpdateBLMirrorXValues + "," + UpdateBLMirrorYValues + "," + "0" + "";

            TestFixtureSocketComm._isInnerloop = false;
            TestFixtureSocketComm._isOuterloop = false;
            TestFixtureSocketComm.ResetRetryCount();

            serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(bottomleftcommand);
            if (serverStatus.Equals(notConnected))
            { 
                ErrorMsgDisplay = "Failed to Set Bottom Left Mirror Position";
                flag = false;
            }
            else
            {
                flag = true;
            }

            System.Threading.Thread.Sleep(4000);
            ReadSensorsVoltages();

            return flag;
        }

        internal void CheckForBottomLeftPDiodeVoltage()
        {
            //System.Threading.Thread.Sleep(4000);
            float voltage = _mtestfixturedaq.ReadBottomLeftPDiodeVoltage();
            readBLvoltage = voltage;
            ReadBottomLeftVoltage = voltage;
            frmTestFixture.Instance.tbBottomLeftVoltageReading.Text = readBLvoltage.ToString();

            //MirrorPositionValidation(ApplicationConstants.MirrorPosition.BottomLeft);
        }

        private float ReadBottomLeftVoltage
        {
            get { return ImageModel.ReadBottomLeftVoltage; }
            set
            {
                ImageModel.ReadBottomLeftVoltage = value;
                OnPropertyChanged("ReadBottomLeftVoltage");
            }
        }
        internal bool StartImageShowForMirror()
        {
            if (!TestFixtureSocketComm._IsIpAddressFound)
            {
                serverStatus = CheckForExistanceofIPAddress();
                if (serverStatus.Equals(interrupted))
                {
                    // write test data in log file
                    // WriteTestResultLog();
                    UpdateTestResultPassFailStatus(true, "Execution Stopped");
                    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else if (serverStatus.Equals(notConnected))
                {
                    // write test data in log file
                    if (openFile)
                        WriteTestResultLog();
                    UpdateTestResultPassFailStatus(false, "Failed to Obtain Server");
                    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    return false;
                }
                else
                {
                    // server available
                }
            }

            string startimageshowcmd = TestFixtureCommands._mstartImgShow;

            TestFixtureSocketComm._isInnerloop = false;
            TestFixtureSocketComm._isOuterloop = false;
            TestFixtureSocketComm.ResetRetryCount();

            serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(startimageshowcmd);
            if (serverStatus.Equals(interrupted))
            {
                // write test data in log file
                // WriteTestResultLog();
                UpdateTestResultPassFailStatus(true, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else if (serverStatus.Equals(notConnected))
            {
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Failed to Obtain Product Type");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else
            {
                return true;
            }

        }

        internal bool SendCommandToStopImageShow()
        {
            //serverStatus = CheckForExistanceofIPAddress();

            if (!TestFixtureSocketComm._IsIpAddressFound)
                return false;

            string stopimgcommand = TestFixtureCommands._mstopimageshow;

            TestFixtureSocketComm._isInnerloop = false;
            TestFixtureSocketComm._isOuterloop = false;
            TestFixtureSocketComm.ResetRetryCount();

            serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(stopimgcommand);
            if (serverStatus.Equals(interrupted))
            {
                // write test data in log file
                // WriteTestResultLog();
                UpdateTestResultPassFailStatus(true, "Execution Stopped");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else if (serverStatus.Equals(notConnected))
            {
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                UpdateTestResultPassFailStatus(false, "Failed to Obtain Server");
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region Image Commands

        public Action CanLoadCamera { get; set; }

        public ICommand StartCamera
        {
            get
            {
                return new RelayCommand(CanExecute, StartCameraToCaptureImage);
            }
        }

        internal void StartCameraToCaptureImage(object p)
        {
            try
            {
                StartiCubeCamera();
            }
            catch
            {


            }
        }
        private void StartiCubeCamera()
        {
            VideoCaptureDevice captdevice;
            FilterInfoCollection infocoll = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            try
            {
                _mCaptdevice = new VideoCaptureDevice(infocoll[0].MonikerString);

                //set camera properties
                //_mCaptdevice.SetCameraProperty(CameraControlProperty.Exposure, 100, CameraControlFlags.Auto);
                //_mCaptdevice.SetCameraProperty(CameraControlProperty.Focus, 90, CameraControlFlags.Auto);
                //_mCaptdevice.SetCameraProperty(CameraControlProperty.Zoom, 50, CameraControlFlags.Auto);

                _mCaptdevice.NewFrame += new AForge.Video.NewFrameEventHandler(TakeNewImage);


                bool flag = _mCaptdevice.ProvideSnapshots;
                _mCaptdevice.Start();

            }
            catch (Exception exp)
            {
                ErrorMsgDisplay = exp.Message.ToString();
                frmTestFixture.Instance.WriteToLog("StartiCubeCamera: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
        }
        private void TakeNewImage(object sender, NewFrameEventArgs args)
        {
            BitmapImage bitimg;

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

                ImageSource = bitimg;

                frmTestFixture.Instance.SetPictureBox(captimg);

            }
            catch (Exception ex)
            {
                ErrorMsgDisplay = ex.ToString();
                frmTestFixture.Instance.WriteToLog("TakeNewImage: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
        }
        public ICommand TakeSnapShot
        {
            get
            {
                return new RelayCommand(CanExecute, CaptureImageFromCamera);
            }
        }

        internal void CaptureImageFromCamera(object p)
        {
            TakeSnapofProjectedImage();
        }

        private void TakeSnapofProjectedImage()
        {
            try
            {
                if (_mCaptdevice != null)
                {
                    _mCaptdevice.SignalToStop();
                    _mCaptdevice.WaitForStop();
                    _mCaptdevice = null;
                }
            }
            catch (Exception ex)
            {
                ErrorMsgDisplay = ex.ToString();
                frmTestFixture.Instance.WriteToLog("TakeSnapofProjectedImage: " + ErrorMsgDisplay, ApplicationConstants.TraceLogType.Error);
            }
        }
        #endregion

        #region progress bar implementation


        /// <summary>
        /// Used to mark if the counter is in progress so the counter can't be started
        /// while it is already running.
        /// </summary>
        public bool IsInProgress
        {
            get { return _mprogress; }
            set
            {
                _mprogress = value;
                OnPropertyChanged("IsInProgress");
                OnPropertyChanged("IsNotInProgress");
            }
        }

        public bool IsNotInProgress
        {
            get { return !IsInProgress; }
            set
            {
                IsInProgress = value;
                OnPropertyChanged("IsNotInProgress");
            }

        }

        public int MaxTotalTestSequence
        {
            get { return _mtotalTests; }
            set { _mtotalTests = value; OnPropertyChanged("MaxTotalTestSequence"); }
        }

        public int MinTotalTestSequence
        {
            get { return _mMinTestSteps; }
            set { _mMinTestSteps = value; OnPropertyChanged("MinTotalTestSequence"); }
        }

        public Double ProgressStatus
        {
            get { return _mValue; }
            set
            {
                if (value <= _mtotalTests)
                {
                    if (value >= _mMinTestSteps)
                    {
                        _mValue = value;
                    }
                    else
                    {
                        _mValue = _mMinTestSteps;
                    }
                }
                else
                {
                    _mValue = _mtotalTests;
                }

                OnPropertyChanged("ProgressStatus");
            }

        }
        public int MinTestSteps
        {
            get { return _mMinTestSteps; }
            set
            {
                _mMinTestSteps = value;
                OnPropertyChanged("MinTestSteps");
            }
        }
        public int MaxTestSteps
        {
            get { return _mtotalTests; }
            set
            {
                _mtotalTests = value;
                OnPropertyChanged("MaxTestSteps");
            }
        }
        internal void SetScokectFlagsToFalse()
        {
            TestFixtureSocketComm._isInnerloop = false;
            TestFixtureSocketComm._isOuterloop = false;
            TestFixtureSocketComm.ResetRetryCount();
        }
        public void Increment()
        {
            // If we are in progress already, don't do anything
            if (IsInProgress)
                return;

            // If the value is already at max steps, start the counting over.
            if (ProgressStatus == MaxTestSteps)
                Reset();
            else
            ProgressStatus++;

            frmTestFixture.Instance.SetProgressStatusBar(ProgressStatus);
        }
        private void Reset()
        {
            ProgressStatus = _mMinTestSteps;

            frmTestFixture.Instance.SetProgressStatusBarMin(_mMinTestSteps);

            //SetProgressStatusBar(_mMinTestSteps);
        }

        #endregion

        #region image
        private bool StartImageShowForCamera()
        {
            CheckForExistanceofIPAddress();
            string startimageshowcmd = TestFixtureCommands._mstartImgShow1; // Focus Test

            TestFixtureSocketComm._isInnerloop = false;
            TestFixtureSocketComm._isOuterloop = false;
            TestFixtureSocketComm.ResetRetryCount();

            serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(startimageshowcmd);
            if (serverStatus.Equals(notConnected))
            {
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else if (serverStatus.Equals(interrupted))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool SendCommandToStopImageShowForCamera()
        {
            CheckForExistanceofIPAddress();
            string stopimgcommandforcamera = TestFixtureCommands._mstopimageshow1;

            TestFixtureSocketComm._isInnerloop = false;
            TestFixtureSocketComm._isOuterloop = false;
            TestFixtureSocketComm.ResetRetryCount();
            serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(stopimgcommandforcamera);
            if (serverStatus.Equals(notConnected))
            {
                ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                return false;
            }
            else if (serverStatus.Equals(interrupted))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region delete all files

        //private void DeleteAllGeneratedFiles()
        //{
        //    try
        //    {
        //        //delete images
        //        string OImgPath = _mTestProjectImgModel.GetOriginalImageFolderPath();
        //        System.IO.DirectoryInfo filestoDelete = new DirectoryInfo(OImgPath);
        //        foreach (FileInfo file in filestoDelete.GetFiles())
        //        {
        //            file.Delete();
        //        }
        //        OImgPath = null;
        //        filestoDelete = null;
        //        OImgPath = _mtestfixtureProjector.GetCapturedImageFolderPath();
        //        filestoDelete = new DirectoryInfo(OImgPath);
        //        foreach (FileInfo file in filestoDelete.GetFiles())
        //        {
        //            file.Delete();
        //        }

        //        OImgPath = _mlogger.GetLogFilePath();
        //        filestoDelete = new DirectoryInfo(OImgPath);
        //        foreach (FileInfo file in filestoDelete.GetFiles())
        //        {
        //            file.Delete();
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        string msg = exp.ToString();
        //        ErrorMsgDisplay = msg;
        //    }
        //}
        #endregion

        #region Move Mirror Position

        bool IsTopRight { get; set; }
        bool IsTopLeft { get; set; }
        bool IsBottomRight { get; set; }
        bool IsBottomLeft { get; set; }
        bool IsHomeScreen { get; set; }

        private string _ErrorStatusMessage;
        public string ErrorStatusMessage
        {
            get
            {
                return _ErrorStatusMessage;
            }
            set
            {
                _ErrorStatusMessage = value;
                OnPropertyChanged("ErrorStatusMessage");
            }
        }

        private string _WarningStatusMessage;
        public string WarningStatusMessage
        {
            get
            {
                return _WarningStatusMessage;
            }
            set
            {
                _WarningStatusMessage = value;
                OnPropertyChanged("WarningStatusMessage");
            }
        }

        private string _InformationStatusMessage;
        public string InformationStatusMessage
        {
            get
            {
                return _InformationStatusMessage;
            }
            set
            {
                _InformationStatusMessage = value;
                OnPropertyChanged("InformationStatusMessage");
            }
        }


        //This property is how we link it to the View.
        //public ICommand MoveMirrorToTopRightPos
        //{
        //    get
        //    {
        //        return new RelayCommand(CanExecute, MoveMirrorToTopRightPosition);
        //    }
        //}


        //This property is how we link it to the View.
        //public ICommand MoveMirrorToTopLeftPos
        //{
        //    get
        //    {
        //        return new RelayCommand(CanExecute, MoveMirrorToTopLeftPosition);
        //    }
        //}



        ////This property is how we link it to the View.
        //public ICommand MoveMirrorToBottomRightPos
        //{
        //    get
        //    {
        //        return new RelayCommand(CanExecute, MoveMirrorToBottomRightPosition);
        //    }
        //}



        //This property is how we link it to the View.
        //public ICommand MoveMirrorToBottomLeftPos
        //{
        //    get
        //    {
        //        return new RelayCommand(CanExecute, MoveMirrorToBottomLeftPosition);
        //    }
        //}



        ////This property is how we link it to the View.
        //public ICommand MoveMirrorToHomeScreenPos
        //{
        //    get
        //    {
        //        return new RelayCommand(CanExecute, MoveMirrorToHomeScreenPosition);
        //    }
        //}


        //This method will move mirror to XY position base upon user input.
        internal bool MoveMirrorToTopRightPosition(Object p)
        {
            bool flag = false;

            if (!string.IsNullOrEmpty(UpdatetrmirrorXvalues) && !string.IsNullOrEmpty(UpdatetrmirrorYvalues))
            {
                int delay = 2;

                //ErrorMsgDisplay = "MoveMirrorToTopRightPosition...processing";

                //WriteToLog(ErrorMsgDisplay);

                //ErrorMsgDisplay = "MoveMirrorToTopRightPosition...processing debugging";

                //WriteToLog(ErrorMsgDisplay);

                SetScokectFlagsToFalse();

                TopRightMirrorCommand();

                if (frmTestFixture.Instance != null)
                {
                    frmTestFixture.Instance.txtTopRightX.Text = UpdatetrmirrorXvalues;
                    frmTestFixture.Instance.txtTopRightY.Text = UpdatetrmirrorYvalues;
                }

                return true;

                //if (readTRvoltage >= 1.5)
                //{
                //    if (readTLvoltage <= 1 && readBLvoltage <= 1 && readBRvoltage <= 1)
                //    {
                //        UpdateTestResultPassFailStatus(true, "Top Right Mirror Calibration Pass");
                //        ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
                //        return true;
                //    }
                //    else
                //    {
                //        if (openFile)
                //            WriteTestResultLog();
                //        UpdateTestResultPassFailStatus(false, "Top Right Mirror Calibration Failed");
                //        ErrorMsgDisplay = "Top Right Mirror Calibration Failed";
                //        return false;
                //    }
                //}
                //else
                //{
                //    if (openFile)
                //        WriteTestResultLog();
                //    UpdateTestResultPassFailStatus(false, "Top Right Mirror Calibration Failed");
                //    ErrorMsgDisplay = "readTRvoltage out of range: " + readTRvoltage.ToString();
                //    return false;
                //}
                //delay = 1;
                //flag = CheckTrigger(delay);
                //if (!flag)
                //{
                //    UpdateTestResultPassFailStatus(false, "Execution Stopped");
                //    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                //    return false;
                //}

                return flag;
            }

            return false;
        }

        //This method will move mirror to XY position base upon user input.
        internal bool MoveMirrorToTopLeftPosition(Object p)
        {
            bool flag = false;

            if (!string.IsNullOrEmpty(UpdatetLmirrorXvalues) && !string.IsNullOrEmpty(UpdatetLmirrorYvalues))
            {
                int delay = 2;

                ErrorMsgDisplay = "MoveMirrorToTopLeftPosition...processing";

                SetScokectFlagsToFalse();

                TopLeftMirrorCOmmand();

                if (frmTestFixture.Instance != null)
                {
                    frmTestFixture.Instance.txtTopLeftX.Text = UpdatetLmirrorXvalues;
                    frmTestFixture.Instance.txtTopLeftY.Text = UpdatetLmirrorYvalues;
                }

                return true;

                //if (readTLvoltage >= 1.5)
                //{
                //    if (readTRvoltage <= 1 && readBLvoltage <= 1 && readBRvoltage <= 1)
                //    {
                //        UpdateTestResultPassFailStatus(true, "Top Left Mirror Calibration Pass");
                //        ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
                //        return true;
                //    }
                //    else
                //    {
                //        if (openFile)
                //            WriteTestResultLog();
                //        UpdateTestResultPassFailStatus(false, "Top Left Mirror Calibration Failed");
                //        ErrorMsgDisplay = "Test image is not falling on photodiode.";
                //        return false;
                //    }
                //}
                //else
                //{
                //    if (openFile)
                //        WriteTestResultLog();
                //    UpdateTestResultPassFailStatus(false, "Top Left Mirror Calibration Failed");
                //    ErrorMsgDisplay = "Test image is not falling on photodiode.";
                //    return false;
                //}
                //delay = 1;
                //flag = CheckTrigger(delay);
                //if (!flag)
                //{
                //    UpdateTestResultPassFailStatus(false, "Execution Stopped");
                //    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                //    return false;
                //}
            }

            return true;
        }

        //This method will move mirror to XY position base upon user input.
        internal bool MoveMirrorToBottomRightPosition(Object p)
        {
            bool flag = false;

            if (!string.IsNullOrEmpty(UpdateBRMirrorXValues) && !string.IsNullOrEmpty(UpdateBRMirrorYValues))
            {
                int delay = 2;

                ErrorMsgDisplay = "MoveMirrorToBottomRightPosition...processing";

                SetScokectFlagsToFalse();

                BottomRightMirrorCommand();

                if (frmTestFixture.Instance != null)
                {
                    frmTestFixture.Instance.txtBottomRightX.Text = UpdateBRMirrorXValues;
                    frmTestFixture.Instance.txtBottomRightY.Text = UpdateBRMirrorYValues;
                }

                return true;

                //if (readBRvoltage >= 1.5)
                //{

                //    if (readTRvoltage <= 1 && readBLvoltage <= 1 && readTLvoltage <= 1)
                //    {
                //        UpdateTestResultPassFailStatus(true, "Bottom Right Mirror Calibration Pass");
                //        ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
                //        return true;
                //    }
                //    else
                //    {
                //        if (openFile)
                //            WriteTestResultLog();
                //        UpdateTestResultPassFailStatus(false, "Bottom Right Mirror Calibration Failed");
                //        ErrorMsgDisplay = "Test image is not falling on photodiode.";
                //        return false;
                //    }
                //}
                //else
                //{
                //    if (openFile)
                //        WriteTestResultLog();
                //    UpdateTestResultPassFailStatus(false, "Bottom Right Mirror Calibration Failed");
                //    ErrorMsgDisplay = "Test image is not falling on photodiode.";
                //    return false;
                //}
                //delay = 1;
                //flag = CheckTrigger(delay);
                //if (!flag)
                //{
                //    UpdateTestResultPassFailStatus(false, "Execution Stopped");
                //    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                //    return false;
                //}
            }

            return true;
        }


        //This method will move mirror to XY position base upon user input.
        internal bool MoveMirrorPosition(ApplicationConstants.MirrorPosition mirrorPosition)
        {
            bool flag = false;

            if (!TestFixtureSocketComm._IsIpAddressFound)
                return false;

            TestFixtureSocketComm._isInnerloop = false;
            TestFixtureSocketComm._isOuterloop = false;
            TestFixtureSocketComm.ResetRetryCount();

            string positionCommand = string.Empty;
            string position = string.Empty;

            SetScokectFlagsToFalse();

            //voltage: check sensor voltage 
            //make a variable call position check... Passing equals 4, else did not pass
            switch (mirrorPosition)
            {
                case ApplicationConstants.MirrorPosition.TopRight:

                    position = "Top Right";
                    positionCommand = TestFixtureCommands._mtoprightcommand;
                    positionCommand = positionCommand + UpdatetrmirrorXvalues + "," + UpdatetrmirrorYvalues + "," + "0" + "";
                    break;

                case ApplicationConstants.MirrorPosition.TopLeft:

                    position = "Top Left";
                    positionCommand = TestFixtureCommands._mtoplefttcommand;
                    positionCommand = positionCommand + UpdatetLmirrorXvalues + "," + UpdatetLmirrorYvalues + "," + "0" + "";
                    break;

                case ApplicationConstants.MirrorPosition.BottomRight:

                    position = "Bottom Right";
                    positionCommand = TestFixtureCommands._mbottomrightcommand;
                    positionCommand = positionCommand + UpdateBRMirrorXValues + "," + UpdateBRMirrorYValues + "," + "0" + "";
                    break;

                case ApplicationConstants.MirrorPosition.BottomLeft:

                    position = "Bottom Left";
                    positionCommand = TestFixtureCommands._mbottomlefttcommand;
                    positionCommand = positionCommand + UpdateBLMirrorXValues + "," + UpdateBLMirrorYValues + "," + "0" + "";
                    break;

                case ApplicationConstants.MirrorPosition.Home:

                    position = "Home Screen";
                    positionCommand = TestFixtureCommands._mhomecommand;
                    positionCommand = positionCommand + UpdateHomeScreenXValues + "," + UpdateHomeScreenYValues + "," + "0" + "";
                    break;
            }

            serverStatus = TestFixtureSocketComm.SendCommandToServerToProcess2(positionCommand);

            if (serverStatus.Equals(notConnected))
            {
                string msg = string.Format("Failed to Set {0} Mirror Position", position);
                ErrorMsgDisplay = msg;
                flag = false;
            }
            else
            {
                flag = true;
            }

            UpdateMirrorPositions(mirrorPosition);

            System.Threading.Thread.Sleep(3000);
            ReadSensorsVoltages();

            flag = MirrorPositionValidation(mirrorPosition);

            return flag;
        }

        internal void UpdateMirrorPositions(ApplicationConstants.MirrorPosition mirrorPosition)
        {

            switch (mirrorPosition)
            {
                case ApplicationConstants.MirrorPosition.TopRight:

                    if (frmTestFixture.Instance != null)
                    {
                        frmTestFixture.Instance.txtTopRightX.Text = UpdatetrmirrorXvalues;
                        frmTestFixture.Instance.txtTopRightY.Text = UpdatetrmirrorYvalues;
                    }

                    break;

                case ApplicationConstants.MirrorPosition.TopLeft:

                    if (frmTestFixture.Instance != null)
                    {
                        frmTestFixture.Instance.txtTopLeftX.Text = UpdatetLmirrorXvalues;
                        frmTestFixture.Instance.txtTopLeftY.Text = UpdatetLmirrorYvalues;
                    }

                    break;

                case ApplicationConstants.MirrorPosition.BottomRight:

                    if (frmTestFixture.Instance != null)
                    {
                        frmTestFixture.Instance.txtBottomRightX.Text = UpdateBRMirrorXValues;
                        frmTestFixture.Instance.txtBottomRightY.Text = UpdateBRMirrorYValues;
                    }

                    break;

                case ApplicationConstants.MirrorPosition.BottomLeft:

                    if (frmTestFixture.Instance != null)
                    {
                        frmTestFixture.Instance.txtBottomLeftX.Text = UpdateBLMirrorXValues;
                        frmTestFixture.Instance.txtBottomLeftY.Text = UpdateBLMirrorYValues;
                    }

                    break;

                case ApplicationConstants.MirrorPosition.Home:

                    if (frmTestFixture.Instance != null)
                    {
                        frmTestFixture.Instance.txtHomeScreenX.Text = UpdateHomeScreenXValues;
                        frmTestFixture.Instance.txtHomeScreenY.Text = UpdateHomeScreenYValues;
                    }

                    break;
            }
        }

        internal bool MirrorPositionValidation(ApplicationConstants.MirrorPosition mirrorPosition)
        {
            bool flag = false;

            if (mirrorPosition == ApplicationConstants.MirrorPosition.TopRight)
            {
                if (readTRvoltage > 1 && readTLvoltage < 1 && readBRvoltage < 1 && readBLvoltage < 1)
                {
                    LogStructureNew.MirrorVerify = "PASS";
                    UpdateTestResultPassFailStatus(true, "Top Right Mirror Calibration Pass");
                    ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
                    //frmTestFixture.Instance.cbTopRightStatus.Checked = flag = true;
                    if(frmTestFixture.Instance.cbTopRightStatus.IsHandleCreated)
                    frmTestFixture.Instance.cbTopRightStatus.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.cbTopRightStatus.Checked = flag = true; });
                    return true;
                }
                else
                {
                    LogStructureNew.MirrorVerify = "FAIL";
                    if (openFile)
                        WriteTestResultLog();
                    UpdateTestResultPassFailStatus(false, "Top Right Mirror Calibration Failed");
                    ErrorMsgDisplay = "Top Right Mirror Calibration Failed.";
                    //frmTestFixture.Instance.cbTopRightStatus.Checked = flag = false;
                    if (frmTestFixture.Instance.cbTopRightStatus.IsHandleCreated)
                        frmTestFixture.Instance.cbTopRightStatus.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.cbTopRightStatus.Checked = flag = false; });
                    return false;
                }
            }

            if (mirrorPosition == ApplicationConstants.MirrorPosition.TopLeft)
            {
                if (readTLvoltage > 1 && readTRvoltage < 1 && readBRvoltage < 1 && readBLvoltage < 1)
                {
                    LogStructureNew.MirrorVerify = "PASS";
                    UpdateTestResultPassFailStatus(true, "Top Left Mirror Calibration Pass");
                    ErrorMsgDisplay = "Top Left Mirror Calibration Pass...";
                    //frmTestFixture.Instance.cbTopLeftStatus.Checked = flag = true;
                    if (frmTestFixture.Instance.cbTopLeftStatus.IsHandleCreated)
                        frmTestFixture.Instance.cbTopLeftStatus.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.cbTopLeftStatus.Checked = flag = true; });
                    return true;
                }
                else
                {
                    LogStructureNew.MirrorVerify = "FAIL";
                    if (openFile)
                        WriteTestResultLog();
                    UpdateTestResultPassFailStatus(false, "Top Left Mirror Calibration Failed");
                    ErrorMsgDisplay = "Top Left Mirror Calibration Failed.";
                    //frmTestFixture.Instance.cbTopLeftStatus.Checked = flag = false;
                    if (frmTestFixture.Instance.cbTopLeftStatus.IsHandleCreated)
                        frmTestFixture.Instance.cbTopLeftStatus.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.cbTopLeftStatus.Checked = flag = false; });
                    return false;
                }
            }

            if (mirrorPosition == ApplicationConstants.MirrorPosition.BottomRight)
            {
                if (readBRvoltage > 1 && readTLvoltage < 1 && readTRvoltage < 1 && readBLvoltage < 1)
                {
                    LogStructureNew.MirrorVerify = "PASS";
                    UpdateTestResultPassFailStatus(true, "Bottom Right Mirror Calibration Pass");
                    ErrorMsgDisplay = "Bottom Right Mirror Calibration Pass...";
                    //frmTestFixture.Instance.cbBottomRightStatus.Checked = flag = true;
                    if (frmTestFixture.Instance.cbBottomRightStatus.IsHandleCreated)
                        frmTestFixture.Instance.cbBottomRightStatus.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.cbBottomRightStatus.Checked = flag = true; });
                    return true;
                }
                else
                {
                    LogStructureNew.MirrorVerify = "FAIL";
                    if (openFile)
                        WriteTestResultLog();
                    UpdateTestResultPassFailStatus(false, "Bottom Right Mirror Calibration Failed");
                    ErrorMsgDisplay = "Bottom Right Mirror Calibration Failed.";
                    //frmTestFixture.Instance.cbBottomRightStatus.Checked = flag = false;
                    if (frmTestFixture.Instance.cbBottomRightStatus.IsHandleCreated)
                        frmTestFixture.Instance.cbBottomRightStatus.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.cbBottomRightStatus.Checked = flag = false; });
                    return false;
                }
            }

            if (mirrorPosition == ApplicationConstants.MirrorPosition.BottomLeft)
            {
                if (readBLvoltage > 1 && readTRvoltage < 1 && readTLvoltage < 1 && readBRvoltage < 1)
                {
                    LogStructureNew.MirrorVerify = "PASS";
                    UpdateTestResultPassFailStatus(true, "Bottom Left Mirror Calibration Pass");
                    ErrorMsgDisplay = "Bottom Left Mirror Calibration Pass...";
                    //frmTestFixture.Instance.cbBottomLeftStatus.Checked = flag = true;
                    if (frmTestFixture.Instance.cbBottomLeftStatus.IsHandleCreated)
                        frmTestFixture.Instance.cbBottomLeftStatus.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.cbBottomLeftStatus.Checked = flag = true; });
                    return true;
                }
                else
                {
                    LogStructureNew.MirrorVerify = "FAIL";
                    if (openFile)
                        WriteTestResultLog();
                    UpdateTestResultPassFailStatus(false, "Bottom Left Mirror Calibration Failed");
                    ErrorMsgDisplay = "Bottom Left Mirror Calibration Failed.";
                    //frmTestFixture.Instance.cbBottomLeftStatus.Checked = flag = false;
                    if (frmTestFixture.Instance.cbBottomLeftStatus.IsHandleCreated)
                        frmTestFixture.Instance.cbBottomLeftStatus.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.cbBottomLeftStatus.Checked = flag = false; });
                    return false;
                }
            }

            return flag;
        }


        //internal bool MirrorPositionValidation(ApplicationConstants.MirrorPosition mirrorPosition)
        //{
        //    bool flag = false;

        //    //voltage: check sensor voltage 
        //    //make a variable call position check... Passing equals 4, else did not pass
        //    switch (mirrorPosition)
        //    {
        //        case ApplicationConstants.MirrorPosition.TopRight:

        //            if (readTRvoltage > 1 && readTLvoltage < 1 && readBRvoltage < 1 && readBLvoltage < 1)
        //            {
        //                UpdateTestResultPassFailStatus(true, "Top Right Mirror Calibration Pass");
        //                ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
        //                frmTestFixture.Instance.cbTopRightStatus.Checked = flag = true;
        //                return true;
        //            }
        //            else
        //            {
        //                if (openFile)
        //                    WriteTestResultLog();
        //                UpdateTestResultPassFailStatus(false, "Top Right Mirror Calibration Failed");
        //                ErrorMsgDisplay = "Test image is not falling on photodiode.";
        //                frmTestFixture.Instance.cbTopRightStatus.Checked = flag = false;
        //            }

        //            break;

        //        case ApplicationConstants.MirrorPosition.TopLeft:

        //            if (readTLvoltage > 1 && readTRvoltage < 1 && readBRvoltage < 1 && readBLvoltage < 1)
        //            {
        //                UpdateTestResultPassFailStatus(true, "Top Left Mirror Calibration Pass");
        //                ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
        //                frmTestFixture.Instance.cbTopLeftStatus.Checked = flag = true;
        //                return true;
        //            }
        //            else
        //            {
        //                if (openFile)
        //                    WriteTestResultLog();
        //                UpdateTestResultPassFailStatus(false, "Top Left Mirror Calibration Failed");
        //                ErrorMsgDisplay = "Test image is not falling on photodiode.";
        //                frmTestFixture.Instance.cbTopLeftStatus.Checked = flag = false;
        //            }

        //            break;

        //        case ApplicationConstants.MirrorPosition.BottomRight:

        //            if (readBRvoltage > 1 && readTLvoltage < 1 && readTRvoltage < 1 && readBLvoltage < 1)
        //            {
        //                UpdateTestResultPassFailStatus(true, "Bottom Right Mirror Calibration Pass");
        //                ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
        //                frmTestFixture.Instance.cbBottomRightStatus.Checked = flag = true;
        //                return true;
        //            }
        //            else
        //            {
        //                if (openFile)
        //                    WriteTestResultLog();
        //                UpdateTestResultPassFailStatus(false, "Bottom Right Mirror Calibration Failed");
        //                ErrorMsgDisplay = "Test image is not falling on photodiode.";
        //                frmTestFixture.Instance.cbBottomRightStatus.Checked = flag = false;
        //            }

        //            break;

        //        case ApplicationConstants.MirrorPosition.BottomLeft:

        //            if (readBLvoltage > 1 && readTRvoltage < 1 && readTLvoltage < 1 && readBRvoltage < 1)
        //            {
        //                UpdateTestResultPassFailStatus(true, "Bottom Left Mirror Calibration Pass");
        //                ErrorMsgDisplay = "Top Right Mirror Calibration Pass...";
        //                frmTestFixture.Instance.cbBottomLeftStatus.Checked = flag = true;
        //                return true;
        //            }
        //            else
        //            {
        //                if (openFile)
        //                    WriteTestResultLog();
        //                UpdateTestResultPassFailStatus(false, "Bottom Left Mirror Calibration Failed");
        //                ErrorMsgDisplay = "Test image is not falling on photodiode.";
        //                frmTestFixture.Instance.cbBottomLeftStatus.Checked = flag = false;
        //                return false;
        //            }

        //            break;

        //        case ApplicationConstants.MirrorPosition.Home:

        //            break;
        //    }

        //    return flag;
        //}

        //This method will move mirror to XY position base upon user input.
        internal bool MoveMirrorToBottomLeftPosition(Object p)
        {
            bool flag = false;

            if (!string.IsNullOrEmpty(UpdateBLMirrorXValues) && !string.IsNullOrEmpty(UpdateBLMirrorYValues))
            {
                int delay = 2;

                ErrorMsgDisplay = "MoveMirrorToBottomLeftPosition...processing";

                SetScokectFlagsToFalse();

                BottomLeftCommand();
                if (frmTestFixture.Instance != null)
                {
                    frmTestFixture.Instance.txtBottomLeftX.Text = UpdateBLMirrorXValues;
                    frmTestFixture.Instance.txtBottomLeftY.Text = UpdateBLMirrorYValues;
                }

                return true;

                //if (readBLvoltage >= 1.5)
                //{
                //    if (readTRvoltage <= 1 && readBRvoltage <= 1 && readTLvoltage <= 1)
                //    {
                //        UpdateTestResultPassFailStatus(true, "Bottom Left Mirror Calibration Pass");
                //        ErrorMsgDisplay = "Bottom Left Mirror Calibration Pass...";
                //        return true;
                //    }
                //    else
                //    {
                //        if (openFile)
                //            WriteTestResultLog();
                //        UpdateTestResultPassFailStatus(false, "Bottom Left Mirror Calibration Failed");
                //        ErrorMsgDisplay = "Test image is not falling on photodiode.";
                //        return false;
                //    }
                //}
                //else
                //{
                //    if (openFile)
                //        WriteTestResultLog();
                //    UpdateTestResultPassFailStatus(false, "Bottom Left Mirror Calibration Failed");
                //    ErrorMsgDisplay = "Test image is not falling on photodiode.";
                //    ucHostContainer.Instance.WriteToLog(ErrorMsgDisplay);
                //    return false;
                //}
            }

            return true;
        }

        //internal void WriteToLog(string message)
        //{
        //    string msg = string.Empty;
        //    counter = counter + 1;
        //    msg = String.Format("{0}. {1} - {2}", counter, DateTime.Now, message);
        //    this.StatusMessage += msg.ToString() + Environment.NewLine;
        //    ucHostContainer.Instance.SetTraceLogTextBox(this.StatusMessage);
        //}

        //This method will move mirror to XY position base upon user input.
        internal bool MoveMirrorToHomeScreenPosition(Object p)
        {
            bool flag = false;

            if (!string.IsNullOrEmpty(UpdateHomeScreenXValues) && !string.IsNullOrEmpty(UpdateHomeScreenYValues))
            {
                int delay = 2;

                SetScokectFlagsToFalse();

                Thread.Sleep(1000);
                Increment();
                LogStructureNew.MirrorVerify = "PASS";
                UpdateTestResultPassFailStatus(true, "Mirror Verification is Sucessfull");

                flag = SendCommandToStopImageShow();

                if (frmTestFixture.Instance != null)
                {
                    frmTestFixture.Instance.txtHomeScreenX.Text = UpdateHomeScreenXValues;
                    frmTestFixture.Instance.txtHomeScreenY.Text = UpdateHomeScreenYValues;
                }

                if (!flag)
                {
                    LogStructureNew.MirrorVerify = "FAIL";
                    if (openFile)
                        WriteTestResultLog();
                    UpdateTestResultPassFailStatus(false, "Stop Image Show Failed");
                    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Information);
                    return false;
                }

                Thread.Sleep(1000);
                //delay = 1;
                //flag = CheckTrigger(delay);
                //if (!flag)
                //{
                //    UpdateTestResultPassFailStatus(false, "Execution Stopped");
                //    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                //    ucHostContainer.Instance.WriteToLog(ErrorMsgDisplay);
                //    return false;
                //}

                SetScokectFlagsToFalse();
                flag = SendCommandToBringMirrorToHome();
                if (!flag)
                {
                    LogStructureNew.MirrorVerify = "FAIL";
                    if (openFile)
                        WriteTestResultLog();
                    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                    frmTestFixture.Instance.WriteToLog(ErrorMsgDisplay, ApplicationConstants.TraceLogType.Warning);
                    return false;
                }

                Thread.Sleep(1000);

                //delay = 1;
                //flag = CheckTrigger(delay);
                //if (!flag)
                //{
                //    UpdateTestResultPassFailStatus(false, "Execution Stopped");
                //    ErrorMsgDisplay = TestFixtureSocketComm.ExceptionHandling();
                //    ucHostContainer.Instance.WriteToLog(ErrorMsgDisplay);
                //    return false ;
                //}
            }

            return true;
        }
        #endregion

        //#region Crossthread Methods
        //void SetModelNumberTextbox(string message)
        //{
        //    //KD Winform: Set model number textbox
        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.txtModelNumber.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.txtModelNumber.Text = message; });
        //}

        //void SetSerialNumberTextbox(string message)
        //{
        //    //KD Winform: Set serial number textbox
        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.txtSerialNumber.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.txtSerialNumber.Text = message; });
        //}

        //void SetVersionNumberTextbox(string message)
        //{
        //    //KD Winform: Set version number textbox
        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.txtVersionNumber.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.txtVersionNumber.Text = message; });
        //}

        //void SetVoltageTextbox(string message)
        //{
        //    //KD Winform: Set voltage textbox
        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.txtVoltage.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.txtVoltage.Text = message; });
        //}


        //internal void SetErrorMessageDisplayTextBox(string message)
        //{
        //    try
        //    {
        //        //KD Winform: Set error message display textbox 
        //        if (ucHostContainer.Instance != null)
        //            ucHostContainer.Instance.txtErrorMsgDisplay.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.txtErrorMsgDisplay.Text = message; });

        //    }
        //    catch { }
        //}
        
        //void SetUpdatePassFailResultTextBox(string message)
        //{
        //    //KD Winform: Set UpdatePassFailResult textbox 
        //    if(ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.txtUpdatePassFailResult.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.txtUpdatePassFailResult.Text = message; });
        //}

        //internal void SetTraceLogTextBox(string message)
        //{
        //    //KD Winform: Set trace log textbox 
        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.txtTraceLog.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.txtTraceLog.Text = message; });
        //}

        //void SetProgressStatusBar(double value)
        //{
        //    //KD Winform: Set progress status bar

        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.circularProgressBar1.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.circularProgressBar1.Value = (int)value; });
        //}

        //void SetProgressStatusBarMin(double value)
        //{
        //    //KD Winform: Set progress status bar min

        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.circularProgressBar1.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.circularProgressBar1.Minimum = (int)value; });
        //}

        //void SetProgressStatusBarMax(double value)
        //{
        //    //KD Winform: Set progress status bar max

        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.circularProgressBar1.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.circularProgressBar1.Maximum = (int)value; });
        //}

        //void ResetProgressStatusBar(double value)
        //{
        //    //KD Winform: reset progress status bar

        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.circularProgressBar1.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.circularProgressBar1.Value = (int)value; });
        //}

        //void SetVoltageCheckbox(bool value)
        //{
        //    //KD Winform: Set voltage checkbox
        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.cbStem.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.cbStem.Checked = value; });
        //}

        //void SetWhiteCheckbox(bool value)
        //{
        //    //KD Winform: Set white checkbox
        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.cbWhite.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.cbWhite.Checked = value; });
        //}

        //void SetGreenCheckbox(bool value)
        //{
        //    //KD Winform: Set green checkbox
        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.cbGreen.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.cbGreen.Checked = value; });
        //}

        //void SetBlueCheckbox(bool value)
        //{
        //    //KD Winform: Set blue checkbox
        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.cbBlue.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.cbBlue.Checked = value; });
        //}

        //void SetRedCheckbox(bool value)
        //{
        //    //KD Winform: Set red checkbox
        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.cbRed.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.cbRed.Checked = value; });
        //}

        //void SetPictureBox(Image value)
        //{
        //    //KD Winform: Set picture box
        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.pbImage.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.pbImage.Image = value; });
        //}

        //internal void SetIpAddressLabel(string value)
        //{
        //    //KD Winform: Set IP address label
        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.lbIpAddress.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.lbIpAddress.Text = value; });
        //}

        //internal void SetIpAddressTextBox(string value)
        //{
        //    //KD Winform: Set IP address textbox
        //    if (ucHostContainer.Instance != null)
        //        ucHostContainer.Instance.txtIpAddress.Invoke((MethodInvoker)delegate { ucHostContainer.Instance.txtIpAddress.Text = value; });
        //}

        //#endregion

        #region Pairing Sequence
        internal bool StartPairingSequence()
        {
            //if (!openFile)
            //{
            //    LogStructureNew.Varient = string.Empty;
            //    LogStructureNew.ModelNo = string.Empty;
            //    LogStructureNew.SerialNo = string.Empty;
            //    LogStructureNew.FirmwareVersion = string.Empty;
            //    LogStructureNew.PlcLightPair = "NOT EXECUTED";
            //    LogStructureNew.DeviceDiscover = "NOT EXECUTED";
            //    LogStructureNew.BandwidthOn = "NOT EXECUTED";
            //    LogStructureNew.BandwidthOff = "NOT EXECUTED";
            //    LogStructureNew.BandwidthStatus = "NOT EXECUTED";
            //    LogStructureNew.MirrorVerify = "NOT EXECUTED";
            //    LogStructureNew.ProjectorFocus = "NOT EXECUTED";
            //    LogStructureNew.LedTest = "NOT EXECUTED";
            //    LogStructureNew.RedWatts = "NOT EXECUTED";
            //    LogStructureNew.GreenWatts = "NOT EXECUTED";
            //    LogStructureNew.BlueWatts = "NOT EXECUTED";
            //    LogStructureNew.WhiteWatts = "NOT EXECUTED";
            //    LogStructureNew.BlendedWhiteWatts = "NOT EXECUTED";
            //    LogStructureNew.MagentaWatts = "NOT EXECUTED";
            //    LogStructureNew.RedXY = "NOT EXECUTED";
            //    LogStructureNew.GreenXY = "NOT EXECUTED";
            //    LogStructureNew.BlueXY = "NOT EXECUTED";
            //    LogStructureNew.WhiteXY = "NOT EXECUTED";
            //    LogStructureNew.BlendedWhiteXY = "NOT EXECUTED";
            //    LogStructureNew.MagentaXY = "NOT EXECUTED";


            //    openFile = true;
            //    TestFixtureEventLogger.createLogFile();
            //}
            //else
            //{
            //    LogStructureNew.Varient = string.Empty;
            //    LogStructureNew.ModelNo = string.Empty;
            //    LogStructureNew.SerialNo = string.Empty;
            //    LogStructureNew.FirmwareVersion = string.Empty;
            //    LogStructureNew.PlcLightPair = "NOT EXECUTED";
            //    LogStructureNew.DeviceDiscover = "NOT EXECUTED";
            //    LogStructureNew.BandwidthOn = "NOT EXECUTED";
            //    LogStructureNew.BandwidthOff = "NOT EXECUTED";
            //    LogStructureNew.BandwidthStatus = "NOT EXECUTED";
            //    LogStructureNew.MirrorVerify = "NOT EXECUTED";
            //    LogStructureNew.ProjectorFocus = "NOT EXECUTED";
            //    LogStructureNew.LedTest = "NOT EXECUTED";
            //    LogStructureNew.RedWatts = "NOT EXECUTED";
            //    LogStructureNew.GreenWatts = "NOT EXECUTED";
            //    LogStructureNew.BlueWatts = "NOT EXECUTED";
            //    LogStructureNew.WhiteWatts = "NOT EXECUTED";
            //    LogStructureNew.BlendedWhiteWatts = "NOT EXECUTED";
            //    LogStructureNew.MagentaWatts = "NOT EXECUTED";
            //    LogStructureNew.RedXY = "NOT EXECUTED";
            //    LogStructureNew.GreenXY = "NOT EXECUTED";
            //    LogStructureNew.BlueXY = "NOT EXECUTED";
            //    LogStructureNew.WhiteXY = "NOT EXECUTED";
            //    LogStructureNew.BlendedWhiteXY = "NOT EXECUTED";
            //    LogStructureNew.MagentaXY = "NOT EXECUTED";
            //}

            bool flag = false;

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            ErrorMsgDisplay = "Solenoid Sequence in progress, wait 7 seconds for ambient green leds to turn on...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            System.Threading.Thread.Sleep(7000);  //wait for ambient green leds to turn on

            //Wall Adapter turining on for 14 seconds after that it should go low for 5 seconds
            _mErrMsg = _mtestfixturedaq.TurnHighForPLCPairing(1); //make it high for 15sec

            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                OverallTestResultPassFailStatus(false, "Execution Stop: TurnHighForPLCPairing(ON): " + _mErrMsg);
                // write test data in log file
                //  WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            UpdateTestResultPassFailStatus(true, "TurnHighForPLCPairing(ON) completed...");

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            _mErrMsg = _mtestfixturedaq.TurnHighForLuminairePairing(1); //make luminaire high for 15sec


            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                OverallTestResultPassFailStatus(false, "Execution Stop: TurnHighForLuminairePairing(ON): " + _mErrMsg);
                // write test data in log file
                //  WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            ErrorMsgDisplay = "TurnHighForLuminairePairing(ON) completed, wait 15 seconds...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            System.Threading.Thread.Sleep(15000);

            _mErrMsg = _mtestfixturedaq.TurnLowForPLCPairing(); //make it low after 15sec

            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                OverallTestResultPassFailStatus(false, "Execution Stop: TurnHighForPLCPairing(OFF): " + _mErrMsg);
                // write test data in log file
                //  WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            ErrorMsgDisplay = "TurnHighForPLCPairing(Off) completed...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);


            _mErrMsg = _mtestfixturedaq.TurnHighForLuminairePairing(0); //make it low after 15sec

            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                OverallTestResultPassFailStatus(false, "Execution Stop: TurnHighForLuminairePairing(OFF): " + _mErrMsg);
                // write test data in log file
                //  WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            UpdateTestResultPassFailStatus(true, "TurnHighForLuminairePairing(OFF) completed...");

            ErrorMsgDisplay = "TurnHighForPLCPairing(On), wait 5 seconds...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            System.Threading.Thread.Sleep(5000);

            _mErrMsg = _mtestfixturedaq.TurnHighForPLCPairing(1); //make it high for 15sec
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                OverallTestResultPassFailStatus(false, "Execution Stop: TurnHighForPLCPairing(On): " + _mErrMsg);
                // write test data in log file
                // WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            ErrorMsgDisplay = "TurnHighForPLCPairing(On) completed, wait 1 seconds...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            System.Threading.Thread.Sleep(1000);

            _mErrMsg = _mtestfixturedaq.TurnLowForPLCPairing(); 
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                OverallTestResultPassFailStatus(false, "Execution Stop: TurnLowForPLCPairing(OFF): " + _mErrMsg);
                // write test data in log file
                // WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            ErrorMsgDisplay = "TurnLowForPLCPairing(On) completed, wait 8 seconds...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            System.Threading.Thread.Sleep(8000);

            //Hall Effect turned on for 14seconds and go low for 5 seconds
            _mErrMsg = SendCommandToSolenoid();
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                // write test data in log file
                //WriteTestResultLog();
                //UpdateTestResultPassFailStatus(false, "Hall Effect Solenoid Error:");
                OverallTestResultPassFailStatus(false, "Execution Stop: SendCommandToSolenoid(ON): " + _mErrMsg);
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            ErrorMsgDisplay = "SendCommandToSolenoid(On) completed, wait 1 seconds...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            System.Threading.Thread.Sleep(1000);

            _mErrMsg = StopDHCPSolenoid();
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                if (openFile)
                    WriteTestResultLog();
                //UpdateTestResultPassFailStatus(false, "Hall Effect Solenoid Error:");
                OverallTestResultPassFailStatus(false, "Execution Stop: StopDHCPSolenoid(OFF): " + _mErrMsg);
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            ErrorMsgDisplay = "StopDHCPSolenoid(On) completed, wait 10 seconds...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            LogStructureNew.PlcLightPair = "PASS";

            ErrorMsgDisplay = "Pairing Sequence completed...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            ErrorMsgDisplay = String.Format("Waiting for booting to complete, wait {0} seconds...", frmTestFixture.Instance.pagevm._model.BootSequenceTime);
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            // Increased delay to 150 sec for booting complete and then
            // Device discovery will start
            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            try
            {
                int delay = Convert.ToInt32(frmTestFixture.Instance.pagevm._model.BootSequenceTime);
                flag = CalibrationDelayCounter(delay);
            }
            catch(Exception e)
            {
                frmTestFixture.Instance.WriteToLog("StartPairingSequence Error: " + e.Message, ApplicationConstants.TraceLogType.Error);
                ErrorMsgDisplay = "StartPairingSequence Error: " + e.Message;
                OverallTestResultPassFailStatus(false, ErrorMsgDisplay);
                flag = false;
            }

            return flag;
        }

        internal bool StartPairingSequenceDiagnostic()
        {
            //if (!openFile)
            //{
            //    LogStructureNew.Varient = string.Empty;
            //    LogStructureNew.ModelNo = string.Empty;
            //    LogStructureNew.SerialNo = string.Empty;
            //    LogStructureNew.FirmwareVersion = string.Empty;
            //    LogStructureNew.PlcLightPair = "NOT EXECUTED";
            //    LogStructureNew.DeviceDiscover = "NOT EXECUTED";
            //    LogStructureNew.BandwidthOn = "NOT EXECUTED";
            //    LogStructureNew.BandwidthOff = "NOT EXECUTED";
            //    LogStructureNew.BandwidthStatus = "NOT EXECUTED";
            //    LogStructureNew.MirrorVerify = "NOT EXECUTED";
            //    LogStructureNew.ProjectorFocus = "NOT EXECUTED";
            //    LogStructureNew.LedTest = "NOT EXECUTED";
            //    LogStructureNew.RedWatts = "NOT EXECUTED";
            //    LogStructureNew.GreenWatts = "NOT EXECUTED";
            //    LogStructureNew.BlueWatts = "NOT EXECUTED";
            //    LogStructureNew.WhiteWatts = "NOT EXECUTED";
            //    LogStructureNew.BlendedWhiteWatts = "NOT EXECUTED";
            //    LogStructureNew.MagentaWatts = "NOT EXECUTED";
            //    LogStructureNew.RedXY = "NOT EXECUTED";
            //    LogStructureNew.GreenXY = "NOT EXECUTED";
            //    LogStructureNew.BlueXY = "NOT EXECUTED";
            //    LogStructureNew.WhiteXY = "NOT EXECUTED";
            //    LogStructureNew.BlendedWhiteXY = "NOT EXECUTED";
            //    LogStructureNew.MagentaXY = "NOT EXECUTED";


            //    openFile = true;
            //    TestFixtureEventLogger.createLogFile();
            //}
            //else
            //{
            //    LogStructureNew.Varient = string.Empty;
            //    LogStructureNew.ModelNo = string.Empty;
            //    LogStructureNew.SerialNo = string.Empty;
            //    LogStructureNew.FirmwareVersion = string.Empty;
            //    LogStructureNew.PlcLightPair = "NOT EXECUTED";
            //    LogStructureNew.DeviceDiscover = "NOT EXECUTED";
            //    LogStructureNew.BandwidthOn = "NOT EXECUTED";
            //    LogStructureNew.BandwidthOff = "NOT EXECUTED";
            //    LogStructureNew.BandwidthStatus = "NOT EXECUTED";
            //    LogStructureNew.MirrorVerify = "NOT EXECUTED";
            //    LogStructureNew.ProjectorFocus = "NOT EXECUTED";
            //    LogStructureNew.LedTest = "NOT EXECUTED";
            //    LogStructureNew.RedWatts = "NOT EXECUTED";
            //    LogStructureNew.GreenWatts = "NOT EXECUTED";
            //    LogStructureNew.BlueWatts = "NOT EXECUTED";
            //    LogStructureNew.WhiteWatts = "NOT EXECUTED";
            //    LogStructureNew.BlendedWhiteWatts = "NOT EXECUTED";
            //    LogStructureNew.MagentaWatts = "NOT EXECUTED";
            //    LogStructureNew.RedXY = "NOT EXECUTED";
            //    LogStructureNew.GreenXY = "NOT EXECUTED";
            //    LogStructureNew.BlueXY = "NOT EXECUTED";
            //    LogStructureNew.WhiteXY = "NOT EXECUTED";
            //    LogStructureNew.BlendedWhiteXY = "NOT EXECUTED";
            //    LogStructureNew.MagentaXY = "NOT EXECUTED";
            //}

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

                bool flag = false;

            ErrorMsgDisplay = "Solenoid Sequence in progress, wait 7 seconds for ambient green leds to turn on...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            System.Threading.Thread.Sleep(7000);  //wait for ambient green leds to turn on

            //Wall Adapter turining on for 14 seconds after that it should go low for 5 seconds
            _mErrMsg = _mtestfixturedaq.TurnHighForPLCPairing(1); //make it high for 15sec

            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                UpdateTestResultPassFailStatus(false, "Execution Stop: TurnHighForPLCPairing(ON): " + _mErrMsg);
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            UpdateTestResultPassFailStatus(true, "TurnHighForPLCPairing(ON) completed...");

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            _mErrMsg = _mtestfixturedaq.TurnHighForLuminairePairing(1); //make luminaire high for 15sec


            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                UpdateTestResultPassFailStatus(false, "Execution Stop: TurnHighForLuminairePairing(ON): " + _mErrMsg);
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            ErrorMsgDisplay = "TurnHighForLuminairePairing(ON) completed, wait 15 seconds...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            System.Threading.Thread.Sleep(15000);

            _mErrMsg = _mtestfixturedaq.TurnLowForPLCPairing(); //make it low after 15sec

            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                UpdateTestResultPassFailStatus(false, "Execution Stop: TurnHighForPLCPairing(OFF): " + _mErrMsg);
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            ErrorMsgDisplay = "TurnHighForPLCPairing(Off) completed...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            _mErrMsg = _mtestfixturedaq.TurnHighForLuminairePairing(0); //make it low after 15sec

            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                UpdateTestResultPassFailStatus(false, "Execution Stop: TurnHighForLuminairePairing(OFF): " + _mErrMsg);
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            UpdateTestResultPassFailStatus(true, "TurnHighForLuminairePairing(OFF) completed...");

            ErrorMsgDisplay = "TurnHighForPLCPairing(On), wait 5 seconds...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            System.Threading.Thread.Sleep(5000);

            _mErrMsg = _mtestfixturedaq.TurnHighForPLCPairing(1); //make it high for 15sec
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                UpdateTestResultPassFailStatus(false, "Execution Stop: TurnHighForPLCPairing(On): " + _mErrMsg);
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            ErrorMsgDisplay = "TurnHighForPLCPairing(On) completed, wait 1 seconds...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            System.Threading.Thread.Sleep(1000);

            _mErrMsg = _mtestfixturedaq.TurnLowForPLCPairing();
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                UpdateTestResultPassFailStatus(false, "Execution Stop: TurnLowForPLCPairing(OFF): " + _mErrMsg);
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            ErrorMsgDisplay = "TurnLowForPLCPairing(On) completed, wait 8 seconds...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            System.Threading.Thread.Sleep(8000);

            //Hall Effect turned on for 14seconds and go low for 5 seconds
            _mErrMsg = SendCommandToSolenoid();
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                // write test data in log file
                if (openFile)
                    WriteTestResultLog();
                //UpdateTestResultPassFailStatus(false, "Hall Effect Solenoid Error:");
                UpdateTestResultPassFailStatus(false, "Execution Stop: SendCommandToSolenoid(ON): " + _mErrMsg);
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            ErrorMsgDisplay = "SendCommandToSolenoid(On) completed, wait 1 seconds...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            System.Threading.Thread.Sleep(1000);

            _mErrMsg = StopDHCPSolenoid();
            if (!_mErrMsg.Equals("NoErrors"))
            {
                isTestExecutionFailure = true;
                LogStructureNew.PlcLightPair = "FAIL";
                if (openFile)
                    WriteTestResultLog();
                //UpdateTestResultPassFailStatus(false, "Hall Effect Solenoid Error:");
                UpdateTestResultPassFailStatus(false, "Execution Stop: StopDHCPSolenoid(OFF): " + _mErrMsg);
                ErrorMsgDisplay = _mErrMsg;
                return flag;
            }

            ErrorMsgDisplay = "StopDHCPSolenoid(On) completed, wait 10 seconds...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            LogStructureNew.PlcLightPair = "PASS";

            ErrorMsgDisplay = "Pairing Sequence completed...";
            UpdateTestResultPassFailStatus(true, ErrorMsgDisplay);

            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                return false;

            flag = true;

            return flag;
        }
        #endregion

        #region log class
        public class LogStructure
        {
            private DateTime nowDate = DateTime.Today;
            private string testname = null;
            private string testresult = null;
            private string resultdata = null;

            public DateTime SetDate
            {
                get { return nowDate; }
                set { nowDate = value; }
            }
            public string TestName
            {
                get { return testname; }
                set { testname = value; }
            }
            public string Result
            {
                get { return testresult; }
                set { testresult = value; }
            }
            public string TestData
            {
                get { return resultdata; }
                set { resultdata = value; }
            }
        }
        #endregion
    }
}

