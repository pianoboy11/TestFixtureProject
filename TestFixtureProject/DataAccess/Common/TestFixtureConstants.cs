using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestFixtureProject.Common
{
    public static class TestFixtureConstants
    {
        const string _fileName = "setting_details_JSon.json";
        const string _loginfileName = "TestFixtureLogin.json";
        const string _testexecutionseq = "TestFixtureExecutionSequence.json";
        const string _textfixtureImgeSettings = "TestFixtureImageSettings.json";
        const string _textfixturecalfile = "lamplamp.csv";
        const string _textspectrometercalfile = "Spectrometer-Calibration_";
        const string _illumaVisionFile = "illumavision.bmp";
        const string _testFixtureImageUpload = "Images\\ImageShow\\";
        const string _testFixtureBandwidthUpload = "Images\\Bandwidth\\";
        const string _eolDirectory = "EOL_Test_Sequence_Config.json";
        const string _lightEngineDirectory = "LightEngine_Test_Sequence_Config.json";
        const string _lineTesterDirectory = "LineTester_Config.json";
        const string _LightEngineDirectory = "\\IllumaVision\\Config\\LightEngine_Test_Sequence_Config.json";
        const string _EOLDirectory = "\\IllumaVision\\Config\\EOL_Test_Sequence_Config.json";
        const string _LineTesterDirectory = "\\IllumaVision\\Config\\LineTester_Config.json";
        const string _ConfigDirectory = "\\IllumaVision\\Config\\";
        const string _Logger = "\\IllumaVision\\Logger\\";
        const string _IllumaVisionRootDirectory = "\\IllumaVision\\";

        public static string GetLineTesterSettingsFilePath()
        {
            DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
            return Path.Combine(info.Parent.FullName, _lineTesterDirectory);
            // return Path.Combine(Environment.CurrentDirectory, _eolDirectory);  //Commented line out for integration purpose. 
        }

        public static string GetEolSequenceFilePath()
        {
            DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
            return Path.Combine(info.Parent.FullName, _eolDirectory);
            // return Path.Combine(Environment.CurrentDirectory, _eolDirectory);  //Commented line out for integration purpose. 
        }

        public static string GetLightEngineSequenceFilePath()
        {
            DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
            return Path.Combine(info.Parent.FullName, _lightEngineDirectory);
            // return Path.Combine(Environment.CurrentDirectory, _lightEngineDirectory);  //Commented line out for integration purpose. 
        }

        public static string GetSettingsInfoFilePath()
        {
            DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
            return Path.Combine(info.Parent.FullName, _fileName);
           // return Path.Combine(Environment.CurrentDirectory, _fileName);  //Commented line out for integration purpose. 
        }

        public static string GetCalibratedFile()
        {
            DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
            return Path.Combine(info.Parent.FullName, _textfixturecalfile);
            //return Path.Combine(Environment.CurrentDirectory, _textfixturecalfile); //Commented line out for integration purpose. 
        }

        public static string GetSpectrometerCalibratedFile()
        {
            DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
            return Path.Combine(info.Parent.FullName, _textspectrometercalfile);
            //return Path.Combine(Environment.CurrentDirectory, _textspectrometercalfile); //Commented line out for integration purpose. 
        }

        public static string ReadSaveImageSettingDetails()
        {
            DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
            return Path.Combine(info.Parent.FullName, _textfixtureImgeSettings);
            // return Path.Combine(Environment.CurrentDirectory, _textfixtureImgeSettings); //Commented line out for integration purpose. 
        }

        public static string GetIllumaVisionFile()
        {
            DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
            return Path.Combine(info.Parent.FullName, _illumaVisionFile);
            //return Path.Combine(Environment.CurrentDirectory, _illumaVisionFile); //Commented line out for integration purpose. 
        }

        public static string GetLoginInfoFilePath()
        {
            DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
            string filePath = Path.Combine(info.Parent.FullName, _loginfileName);
            //string filePath =  Path.Combine(Environment.CurrentDirectory, _loginfileName);
            if (!Directory.Exists(filePath))
            {
                if (!File.Exists(filePath))
                {
                    JObject loginInfo = new JObject(new JProperty("Password", "Pentair"));

                    using (StreamWriter file = File.CreateText(filePath))
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        loginInfo.WriteTo(writer);
                    }
                }
            }
            return filePath;
        }

        public static string GetTestSequenceFromJson()
        {
            DirectoryInfo info = new DirectoryInfo(Environment.CurrentDirectory);
            string filePath = Path.Combine(info.Parent.FullName, _testexecutionseq);
            //string filePath = Path.Combine(Environment.CurrentDirectory, _testexecutionseq);
            if (!Directory.Exists(filePath))
            {
                if (!File.Exists(filePath))
                {
                    JObject loginInfo = new JObject(new JProperty("ExecutionEvents","1"));
                    
                    using (StreamWriter file = File.CreateText(filePath))
                    using (JsonTextWriter writer = new JsonTextWriter(file))
                    {
                        loginInfo.WriteTo(writer);
                    }
                }
            }
            return filePath;
        }

        public static string CreateDirectoryIfNotExists(string folderName)
        {
            string new_path = null;
            try
            {
                new_path = Environment.CurrentDirectory + "\\" + folderName;
                if (!Directory.Exists(new_path))
                {
                    Directory.CreateDirectory(new_path);
                }
            }
            catch(IOException exp)
            {

            }
            return new_path;
        }

        public static string getImageUploadDirPath()
        {
            return Path.Combine(Environment.CurrentDirectory, _testFixtureImageUpload);
        }

        public static string getBandwidthUploadDirPath()
        {
            return Path.Combine(Environment.CurrentDirectory, _testFixtureBandwidthUpload);
        }

        //string strUri2 = String.Format(@_mfolderPath + "\\" + "OProcessedImg.png");

        #region CreateIllumaVisionRootDirectories
        private static void CreateIllumaVisionRootDirectories()
        {
            string new_path = string.Empty;

            try
            {

                if (!Directory.Exists(_IllumaVisionRootDirectory))
                {
                    Directory.CreateDirectory(_IllumaVisionRootDirectory);
                }

                if (!Directory.Exists(_EOLDirectory))
                {
                    Directory.CreateDirectory(_EOLDirectory);
                }

                if (!Directory.Exists(_LightEngineDirectory))
                {
                    Directory.CreateDirectory(_LightEngineDirectory);
                }

                if (!Directory.Exists(_ConfigDirectory))
                {
                    Directory.CreateDirectory(_ConfigDirectory);
                }

                if (!Directory.Exists(_Logger))
                {
                    Directory.CreateDirectory(_Logger);
                }

            }
            catch (Exception e)
            {
                frmTestFixture.Instance.WriteToLog("CreateIllumaVisionRootDirectories Error: " + e.Message, ApplicationConstants.TraceLogType.Information);
                //return string.Empty;
            }
        }
        #endregion
    }
}
