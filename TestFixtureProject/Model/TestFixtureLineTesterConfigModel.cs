using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using TestFixtureProject.Common;
using TestFixtureProject.ViewModel;

namespace TestFixtureProject.Model
{
    public class TestFixtureLineTesterConfigModel
    {
        #region constructor
        public TestFixtureLineTesterConfigModel()
        {
            LoadSettingDetailsFromFile();
        }
        #endregion

        #region Private Methods
        private void LoadSettingDetailsFromFile()
        {
            string file_path = TestFixtureConstants.GetLineTesterSettingsFilePath();
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
                frmTestFixture.Instance.WriteToLog("TestFixtureLineTesterConfigModel ERROR: " + e.Message, ApplicationConstants.TraceLogType.Error);
            }

        }
        #endregion

        #region Line Tester variables
        [JsonProperty("_eol")]
        private bool _eol = false;
        [JsonIgnore]
        public bool EOL
        {
            get { return _eol; }
            set
            {
                _eol = value;
            }
        }

        [JsonProperty("_lightEngine")]
        private bool _lightEngine = false;
        [JsonIgnore]
        public bool LightEngine
        {
            get { return _lightEngine; }
            set
            {
                _lightEngine = value;
            }

        }

        private void PopulateDataIntoSettingPage(dynamic file)
        {
            if ("_eol" == file.Name)
            { EOL = file.Value; }
            else if ("_lightEngine" == file.Name)
            { LightEngine = file.Value; }
        }
        #endregion
    }
}
