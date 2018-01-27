using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace TestFixtureProject.Helpers
{
    using TestFixtureProject.Common;

    public static class TestFixtureEventLogger
    {
        private static string _folderPath = null;
        private static string _newFolderPath = null;
        static StreamWriter log;
        static FileStream fs = null;
        static string strLogText;

        static int _lineCount;

        public static int GetFileLineCount()
        {
            _lineCount = 0;

            // Create a writer and open the file:
            if (File.Exists(_newFolderPath + "\\TestFixture_log.csv"))
            {
                using (StreamReader r = new StreamReader(_newFolderPath + "\\TestFixture_log.csv"))
                {
                    string line;
                    while ((line = r.ReadLine()) != null)
                    {
                        if (_lineCount == 500)
                            break;

                        _lineCount++;
                    }
                }
            }

            if(_lineCount >= 500)
            {
                FileInfo file = new FileInfo(_newFolderPath + "\\TestFixture_log.csv");

                string newName = string.Format("TestFixture_log_{0}.csv", DateTime.Now.ToString("MMddyyyy_HHmmss_tt"));

                file.Rename(newName);
            }

            return _lineCount;
        }

        public static void createLogFile()
        {

            _newFolderPath = CreateDirectoryIfNotExistsInC(TestFixtureConstants._Logger);

            int lineCount = GetFileLineCount();

            // Create a writer and open the file:
            if (!File.Exists(_newFolderPath + "\\TestFixture_log.csv"))
            {

                string createText = "PentAir Illumavision Testfixture" +
                                    "\nThe Test Fixture to show the test results.";

                log = new StreamWriter(_newFolderPath + "\\TestFixture_log.csv");

                log.WriteLine(createText);


                //log.WriteLine(DateTime.Now);
                strLogText = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30}",
                                            "Date/Time",
                                            "ModelNo",
                                            "SerialNo",
                                            "Varient",
                                            "FirmwareVersion",
                                            "PlcLightPair",
                                            "DeviceDiscover",
                                            "BandwidthOff",
                                            "BandwidthOn",
                                            "BandwidthStatus",
                                            "MirrorVerify",
                                            "ProjectorFocus",
                                            "LedTest",
                                            "RedWatts",
                                            "GreenWatts",
                                            "BlueWatts",
                                            "WhiteWatts",
                                            "BlendedWhiteWatts",
                                            "MagentaWatts",
                                            "RedX",
                                            "RedY",
                                            "GreenX",
                                            "GreenY",
                                            "BlueX",
                                            "BlueY",
                                            "WhiteX",
                                            "WhiteY",
                                            "BlendedWhiteX",
                                            "BlendedWhiteY",
                                            "MagentaX",
                                            "MagentaY");

                log.WriteLine(strLogText);
            }
            else
            {
  
                log = File.AppendText(_newFolderPath + "\\TestFixture_log.csv");
                //log.WriteLine(log.NewLine);
                //log.WriteLine(log.NewLine);
            }

            //log.Close();
        }

        public static void appendLogTest(bool lineStatus, List<object> logTestFormat)
        {
            if (lineStatus)
            {
                log.WriteLine(strLogText);
            }
            else
            {
                log.Write(strLogText);
            }

        }

        public static void appendLog(bool lineStatus)
        {
            // log.Close();

            if (log != null)
            {

                if (log.BaseStream == null)
                {

                }
                else
                {
                    if (lineStatus)
                    {
                        //strLogText = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", DateTime.Now, LogStructureNew.SerialNo, LogStructureNew.Varient, LogStructureNew.PlcLightPair, LogStructureNew.DeviceDiscover, LogStructureNew.BandwidthOn, LogStructureNew.BandwidthOff, LogStructureNew.BandwidthStatus, LogStructureNew.MirrorVerify, LogStructureNew.ProjectorFocus, LogStructureNew.LedTest);
                        strLogText = String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30}", 
                            DateTime.Now,
                            LogStructureNew.ModelNo,
                            LogStructureNew.SerialNo, 
                            LogStructureNew.Varient,
                            LogStructureNew.FirmwareVersion,
                            LogStructureNew.PlcLightPair, 
                            LogStructureNew.DeviceDiscover, 
                            LogStructureNew.BandwidthOff, 
                            LogStructureNew.BandwidthOn, 
                            LogStructureNew.BandwidthStatus, 
                            LogStructureNew.MirrorVerify,
                            LogStructureNew.ProjectorFocus, 
                            LogStructureNew.LedTest,
                            LogStructureNew.RedWatts,
                            LogStructureNew.GreenWatts,
                            LogStructureNew.BlueWatts,
                            LogStructureNew.WhiteWatts,
                            LogStructureNew.BlendedWhiteWatts,
                            LogStructureNew.MagentaWatts,
                            LogStructureNew.RedX,
                            LogStructureNew.RedY,
                            LogStructureNew.GreenX,
                            LogStructureNew.GreenY,
                            LogStructureNew.BlueX,
                            LogStructureNew.BlueY,
                            LogStructureNew.WhiteX,
                            LogStructureNew.WhiteY,
                            LogStructureNew.BlendedWhiteX,
                            LogStructureNew.BlendedWhiteY,
                            LogStructureNew.MagentaX,
                            LogStructureNew.MagentaY);

                  
                        log.WriteLine(strLogText);
                    }
                    else
                    {
                        log.Write(strLogText);
                    }
                }
            }
        }

        public static void closeLog()
        {
            if (log != null)
                log.Close();
        }

        public static void Log()
        {
            _folderPath = CreateDirectoryIfNotExists("Logger");
        }
        public static void LogDetails(ArrayList aList)
        {
            System.Text.StringBuilder theBuilder = new System.Text.StringBuilder();
            foreach (ViewModel.TestFixtureViewModel.LogStructure detail in aList)
            {
                theBuilder.Append(detail.SetDate);
                theBuilder.Append(",");
                theBuilder.Append(detail.TestName);
                theBuilder.Append(",");
                theBuilder.Append(detail.TestData);
                theBuilder.Append("\n");
            }

            using (StreamWriter theWriter = new StreamWriter(_folderPath))
            {
                theWriter.Write(theBuilder.ToString());
            }

        }
        public static void WrtiteStringToFile()
        {
            try
            {
                System.IO.FileStream fs;
                string path = System.IO.Path.Combine(_folderPath, "TestFixtureLog.txt");
                if (!System.IO.File.Exists(path))
                {
                    fs = System.IO.File.Create(path);
                    fs.Close();
                    System.IO.File.WriteAllText(path, _mstrtext);
                    System.IO.File.WriteAllText(path, Environment.NewLine);

                }
                System.IO.File.AppendAllText(path, _mstrtext);
                System.IO.File.AppendAllText(path, Environment.NewLine);
            }
            catch (Exception exp)
            {

            }
        }

        public static void WrtiteStringToFile(string aMessage)
        {
            try
            {
                System.IO.FileStream fs;
                string path = System.IO.Path.Combine(_folderPath, "TestFixtureLog.txt");
                if (!System.IO.File.Exists(path))
                {
                    fs = System.IO.File.Create(path);
                    fs.Close();
                    System.IO.File.WriteAllText(path, aMessage);
                    System.IO.File.WriteAllText(path, Environment.NewLine);

                }
                System.IO.File.AppendAllText(path, aMessage);
                System.IO.File.AppendAllText(path, Environment.NewLine);
            }
            catch (Exception exp)
            {

            }
        }

        public static void WriteToPath(bool aFlag, string aMessage)
        {
            using (StreamWriter streamWriter = new StreamWriter(aMessage, aFlag))
            {
                streamWriter.WriteLine(aMessage);
                streamWriter.Close();
            }
        }
        private static string CreateDirectoryIfNotExists(string folderName)
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
            catch (IOException exp)
            {

            }
            return new_path;
        }

        private static string CreateDirectoryIfNotExistsInC(string folderName)
        {
            string new_path = null;
            try
            {
                new_path = folderName;
                if (!Directory.Exists(new_path))
                {
                    Directory.CreateDirectory(new_path);
                }
            }
            catch (IOException exp)
            {

            }
            return new_path;
        }


        private static string _mstrtext = null;
        public static void CreateStringAToWrite(string aText)
        {
            if (aText.Length <= 0)
                return;

            _mstrtext += aText + "  ";
        }
        public static string GetStringToWrite()
        {
            return _mstrtext;
        }

        public static string GetLogFilePath()
        {
            return _folderPath;
        }
    }
}

public static class ExtendedMethod
{
    public static void Rename(this FileInfo fileInfo, string newName)
    {
        fileInfo.MoveTo(fileInfo.Directory.FullName + "\\" + newName);
    }
}