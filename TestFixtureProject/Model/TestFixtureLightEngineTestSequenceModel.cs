using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using TestFixtureProject.Common;
using TestFixtureProject.ViewModel;

namespace TestFixtureProject.Model
{
    public class TestFixtureLightEngineTestSequenceModel
    {
        #region constructor
        public TestFixtureLightEngineTestSequenceModel()
        {
            LoadSettingDetailsFromFile();
        }
        #endregion

        #region Private Methods
        private void LoadSettingDetailsFromFile()
        {
            string file_path = TestFixtureConstants.GetLightEngineSequenceFilePath();
            if (!string.IsNullOrEmpty(file_path) && (File.Exists(file_path)))
            {
                DeserializeAndSetProperties(file_path);
            }
        }

        private void DeserializeAndSetProperties(string filepath)
        {
            //handle file path scenarion
            try
            {

                string jsonsettingdetails = System.IO.File.ReadAllText(filepath);
                dynamic files = JsonConvert.DeserializeObject(jsonsettingdetails);
                foreach (var f in files)
                {
                    PopulateDataIntoSettingPage(f);
                }
            }
            catch (FileNotFoundException e)
            {
                frmTestFixture.Instance.WriteToLog("TestFixtureLightEngineTestSequenceModel ERROR: " + e.Message, ApplicationConstants.TraceLogType.Error);
            }

        }
        #endregion

        #region Line Tester Sequence variables
        [JsonProperty("_serialNumber")]
        private bool _serialNumber = false;
        [JsonIgnore]
        public bool SerialNumber
        {
            get { return _serialNumber; }
            set
            {
                _serialNumber = value;
            }

        }

        [JsonProperty("_powerOn")]
        private bool _powerOn = false;
        [JsonIgnore]
        public bool PowerOn
        {
            get { return _powerOn; }
            set
            {
                _powerOn = value;
            }
        }

         [JsonProperty("_pentairServer")]
        private bool _pentairServer = false;
        [JsonIgnore]
        public bool PentairServer
        {
            get { return _pentairServer; }
            set
            {
                _pentairServer = value;
            }
        }

        [JsonProperty("_firmwareVersion")]
        private bool _firmwareVersion = false;
        [JsonIgnore]
        public bool FirmwareVersion
        {
            get { return _firmwareVersion; }
            set
            {
                _firmwareVersion = value;
            }
        }

        [JsonProperty("_projectorMirrorCheck")]
        private bool _projectorMirrorCheck = false;
        [JsonIgnore]
        public bool ProjectorMirrorCheck
        {
            get { return _projectorMirrorCheck; }
            set
            {
                _projectorMirrorCheck = value;
            }
        }

        [JsonProperty("_ledBrightnessColor")]
        private bool _ledBrightnessColor = false;
        [JsonIgnore]
        public bool LedBrightnessColor
        {
            get { return _ledBrightnessColor; }
            set
            {
                _ledBrightnessColor = value;
            }
        }

        [JsonProperty("_projectorFocus")]
        private bool _projectorFocus = false;
        [JsonIgnore]
        public bool ProjectorFocus
        {
            get { return _projectorFocus; }
            set
            {
                _projectorFocus = value;
            }
        }

        [JsonProperty("_projectorBrightness")]
        private bool _projectorBrightness = false;
        [JsonIgnore]
        public bool ProjectorBrightness
        {
            get { return _projectorBrightness; }
            set
            {
                _projectorBrightness = value;
            }
        }

        [JsonProperty("_testCompletion")]
        private bool _testCompletion = false;
        [JsonIgnore]
        public bool TestCompletion
        {
            get { return _testCompletion; }
            set
            {
                _testCompletion = value;
            }
        }

        private void PopulateDataIntoSettingPage(dynamic file)
        {
            if ("_serialNumber" == file.Name)
            { SerialNumber = file.Value; }
            else if ("_powerOn" == file.Name)
            { PowerOn = file.Value; }
            else if ("_pentairServer" == file.Name)
            { PentairServer = file.Value; }
            else if ("_firmwareVersion" == file.Name)
            { FirmwareVersion = file.Value; }
            else if ("_projectorMirrorCheck" == file.Name)
            { ProjectorMirrorCheck = file.Value; }
            else if ("_ledBrightnessColor" == file.Name)
            { LedBrightnessColor = file.Value; }
            else if ("_projectorFocus" == file.Name)
            { ProjectorFocus = file.Value; }
            else if ("_projectorBrightness" == file.Name)
            { ProjectorBrightness = file.Value; }
            else if ("_testCompletion" == file.Name)
            { TestCompletion = file.Value; }
          }
        #endregion
    }
}
