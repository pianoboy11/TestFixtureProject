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
        const string _ConfigDirectory = "C:\\IllumaVision\\Config\\";
        internal const string _Logger = "C:\\IllumaVision\\Logger\\";
        const string _LoginfileName = "\\IllumaVision\\Config\\TestFixtureLogin.json";
        const string _TestFixtureImageUpload = "C:\\IllumaVision\\Images\\ImageShow\\";
        const string _ThirdPartyDrivers = "C:\\IllumaVision\\ThirdPartyDrivers\\";
        const string _DotNetFramework = "C:\\IllumaVision\\DotNetFramework\\";
        const string _IllumaVisionRootDirectory = "C:\\IllumaVision\\";

        public static string GetDotNetFrameworkFilePath()
        {
            string filePath = string.Format("C:\\{0}{1}", "\\IllumaVision\\DotNetFramework\\", _DotNetFramework);
            return filePath;
        }

        public static string GetThirdPartyDriversFilePath()
        {
            string filePath = string.Format("C:\\{0}{1}", "\\IllumaVision\\ThirdPartyDrivers\\", _ThirdPartyDrivers);
            return filePath;
        }

        public static string GetLineTesterSettingsFilePath()
        {
            string filePath = string.Format("C:\\{0}{1}", "\\IllumaVision\\Config\\", _lineTesterDirectory);
            return filePath;
        }

        public static string GetEolSequenceFilePath()
        {
            string filePath = string.Format("C:{0}{1}", "\\IllumaVision\\Config\\", _eolDirectory);
            return filePath;
        }

        public static string GetLightEngineSequenceFilePath()
        {
            string filePath = string.Format("C:{0}{1}", "\\IllumaVision\\Config\\", _lightEngineDirectory);
            return filePath;
        }

        public static string GetSettingsInfoFilePath()
        {
            string filePath = string.Format("C:{0}{1}", "\\IllumaVision\\Config\\", _fileName);
            return filePath;
        }

        public static string GetCalibratedFile()
        {
            string filePath = string.Format("C:{0}{1}", "\\IllumaVision\\Config\\", _textfixturecalfile);
            return filePath;
        }

        public static string GetSpectrometerCalibratedFile()
        {
            string filePath = string.Format("C:{0}{1}", "\\IllumaVision\\Config\\", _textspectrometercalfile);
            return filePath;
        }

        public static string ReadSaveImageSettingDetails()
        {
            string filePath = string.Format("C:{0}{1}", "\\IllumaVision\\Config\\", _textfixtureImgeSettings);
            return filePath;
        }

        public static string GetIllumaVisionFile()
        {
            string filePath = string.Format("C:{0}{1}", "\\IllumaVision\\Config\\", _illumaVisionFile);
            return filePath;
        }

        public static string GetLoginInfoFilePath()
        {
            string filePath = string.Format("C:{0}{1}", "\\IllumaVision\\Config\\", _LoginfileName);
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
            string filePath = string.Format("C:\\{0}{1}", "\\IllumaVision\\Config\\", _testexecutionseq);

            if (!Directory.Exists(filePath))
            {
                if (!File.Exists(filePath))
                {
                    JObject loginInfo = new JObject(new JProperty("ExecutionEvents", "1"));

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

                new_path = string.Format("{0}", _TestFixtureImageUpload);

                if (!Directory.Exists(new_path))
                {
                    Directory.CreateDirectory(new_path);
                }
            }
            catch (IOException exp)
            {
                MessageBox.Show("CreateDirectoryIfNotExists Error: " + exp.Message, "CreateDirectoryIfNotExists", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return new_path;
        }

        public static string getImageUploadDirPath()
        {
            return Path.Combine(Environment.CurrentDirectory, _TestFixtureImageUpload);
            // return Path.Combine(Environment.CurrentDirectory, _testFixtureImageUpload);
        }

        public static string getBandwidthUploadDirPath()
        {
            return Path.Combine(Environment.CurrentDirectory, _testFixtureBandwidthUpload);
        }

        //string strUri2 = String.Format(@_mfolderPath + "\\" + "OProcessedImg.png");

        #region CreateIllumaVisionRootDirectories
        public static bool CreateIllumaVisionRootDirectories()
        {
            string new_path = string.Empty;
            bool isCreated = false;

            try
            {

                if (!Directory.Exists(_ConfigDirectory))
                {
                    Directory.CreateDirectory(_ConfigDirectory);
                }

                if (!Directory.Exists(_Logger))
                {
                    Directory.CreateDirectory(_Logger);
                }

                if (!Directory.Exists(_TestFixtureImageUpload))
                {
                    Directory.CreateDirectory(_TestFixtureImageUpload);
                }

                //if (!Directory.Exists(_ThirdPartyDrivers))
                //{
                //    Directory.CreateDirectory(_ThirdPartyDrivers);
                //}

                CopyIllumaVisionConfigFilesToConfigDirectory();

                CopyIllumaVisionImageShowFilesToImageDirectory();

                //CopyDllsToThirdPartyDriversDirectory();

                isCreated = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("CreateIllumaVisionRootDirectories Error: " + e.Message, "ROOT DIRECTORIES", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return isCreated;
        }

        private static void CopyIllumaVisionConfigFilesToConfigDirectory()
        {
            try
            {

                string fileName = string.Empty;
                string destFile = string.Empty;

                string filePath = string.Format("{0}{1}", Environment.CurrentDirectory, "\\IllumaVision\\Config\\");

                // To copy all the files in one directory to another directory. 
                // Get the files in the source folder. (To recursively iterate through 
                // all subfolders under the current directory, see 
                // "How to: Iterate Through a Directory Tree.")
                // Note: Check for target path was performed previously 
                //       in this code example. 
                if (System.IO.Directory.Exists(filePath))
                {
                    string[] files = System.IO.Directory.GetFiles(filePath);

                    // Copy the files and overwrite destination files if they already exist. 
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        fileName = System.IO.Path.GetFileName(s);
                        destFile = System.IO.Path.Combine(_ConfigDirectory, fileName);
                        System.IO.File.Copy(s, destFile, true);
                    }
                }
                else
                {
                    MessageBox.Show(filePath, "CopyIllumaVisionConfigFilesToConfigDirectory");

                    //frmTestFixture.Instance.WriteToLog("CopyIllumaVisionConfigFilesToConfigDirectory Error: Source path does not exist!", ApplicationConstants.TraceLogType.Warning);
                    return;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("CopyIllumaVisionConfigFilesToConfigDirectory Error: " + e.Message, "CONFIG DIRECTORIES", MessageBoxButton.OK, MessageBoxImage.Error);

                //frmTestFixture.Instance.WriteToLog("CopyIllumaVisionConfigFilesToConfigDirectory Error: " + e.Message, ApplicationConstants.TraceLogType.Information);
                return;
            }
        }

        private static void CopyIllumaVisionImageShowFilesToImageDirectory()
        {
            try
            {

                string fileName = string.Empty;
                string destFile = string.Empty;

                string filePath = string.Format("{0}{1}", Environment.CurrentDirectory, "\\Images\\ImageShow");

                //To copy all the files in one directory to another directory.
                // Get the files in the source folder. (To recursively iterate through
                // all subfolders under the current directory, see
                // "How to: Iterate Through a Directory Tree.")
                // Note: Check for target path was performed previously 
                //       in this code example.
                if (System.IO.Directory.Exists(filePath))
                {
                    string[] files = System.IO.Directory.GetFiles(filePath);

                    // Copy the files and overwrite destination files if they already exist. 
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        fileName = System.IO.Path.GetFileName(s);
                        destFile = System.IO.Path.Combine(_TestFixtureImageUpload, fileName);
                        System.IO.File.Copy(s, destFile, true);
                    }
                }
            }

            catch (Exception e)
            {
                MessageBox.Show("CopyIllumaVisionImageShowFilesToImageDirectory Error: " + e.Message, "CONFIG DIRECTORIES", MessageBoxButton.OK, MessageBoxImage.Error);

                //frmTestFixture.Instance.WriteToLog("CopyIllumaVisionImageShowFilesToImageDirectory Error: " + e.Message, ApplicationConstants.TraceLogType.Information);
                return;
            }
        }

        internal static void CopyDllsToThirdPartyDriversDirectory()
        {
            try
            {

                string fileName = string.Empty;
                string destFile = string.Empty;

                string filePath = string.Format("{0}{1}", Environment.CurrentDirectory, "\\ThirdPartyDrivers\\");

                //To copy all the files in one directory to another directory.
                // Get the files in the source folder. (To recursively iterate through
                // all subfolders under the current directory, see
                // "How to: Iterate Through a Directory Tree.")
                // Note: Check for target path was performed previously 
                //       in this code example.
                if (System.IO.Directory.Exists(filePath))
                {
                    string[] files = System.IO.Directory.GetFiles(filePath);

                    // Copy the files and overwrite destination files if they already exist. 
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        fileName = System.IO.Path.GetFileName(s);
                        destFile = System.IO.Path.Combine(_ThirdPartyDrivers, fileName);
                        System.IO.File.Copy(s, destFile, true);
                    }
                }
                else
                {
                    MessageBox.Show(filePath,"CopyDllsToThirdPartyDriversDirectory");

                    //frmTestFixture.Instance.WriteToLog("CopyDllsToThirdPartyDriversDirectory Error: Source path does not exist!", ApplicationConstants.TraceLogType.Warning);
                    return;
                }
            }
            catch (Exception e)
            {
                frmTestFixture.Instance.WriteToLog("CopyDllsToThirdPartyDriversDirectory Error: " + e.Message, ApplicationConstants.TraceLogType.Information);
                return;
            }
        }
        #endregion
    }
}
