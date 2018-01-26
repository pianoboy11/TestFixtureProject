using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFixtureProject.Common;
using TestFixtureProject.Model;

namespace TestFixtureProject.DataAccess
{
    class TestFixtureFileOperation
    {

        #region public functions to write setting data to JSon file
        public void WriteSettingDataIntoJSon(TestFixtureSettingModel modelObject)
        {
            string jsonstring = JsonConvert.SerializeObject(modelObject);
            string json_settingdata_file_path = TestFixtureConstants.GetSettingsInfoFilePath();
            System.IO.File.WriteAllText(json_settingdata_file_path, jsonstring);
        }
        #endregion

        #region save image details in Json
        public void WriteImgDetailsToJSon(TestFixtureProjectorImageModel aImgModel)
        {
            string jsonstring = JsonConvert.SerializeObject(aImgModel);
            string json_settingdata_file_path = TestFixtureConstants.ReadSaveImageSettingDetails();
            System.IO.File.WriteAllText(json_settingdata_file_path, jsonstring);
        }

        #endregion

        #region save test sequencce details in Json
        public void WriteEolTestSequenceDetailsToJSon(TestFixtureEolTestSequenceModel eolModel)
        {
            string jsonstring = JsonConvert.SerializeObject(eolModel);
            string json_eol_file_path = TestFixtureConstants.GetEolSequenceFilePath();
            System.IO.File.WriteAllText(json_eol_file_path, jsonstring);
        }

        public void WriteLightEngineTestSequenceDetailsToJSon(TestFixtureLightEngineTestSequenceModel lightEngineModel)
        {
            string jsonstring = JsonConvert.SerializeObject(lightEngineModel);
            string json_lightEngine_file_path = TestFixtureConstants.GetLightEngineSequenceFilePath();
            System.IO.File.WriteAllText(json_lightEngine_file_path, jsonstring);
        }

        public void WriteLineTesterTestSequenceDetailsToJSon(TestFixtureLineTesterConfigModel lightTesterModel)
        {
            string jsonstring = JsonConvert.SerializeObject(lightTesterModel);
            string json_lineTester_file_path = TestFixtureConstants.GetLineTesterSettingsFilePath();
            System.IO.File.WriteAllText(json_lineTester_file_path, jsonstring);
        }
        #endregion
    }

}
