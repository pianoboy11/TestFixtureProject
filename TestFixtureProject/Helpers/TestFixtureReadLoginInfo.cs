using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestFixtureProject.Helpers
{
    class TestFixtureReadLoginInfo
    {
        const string _fileName = "Login_JSon.json";
        #region constructor 
        public TestFixtureReadLoginInfo()
        {

        }
        #endregion

        #region
        public void LoadLoginJson()
        {
             string _system_json_path = Path.Combine(Environment.CurrentDirectory, _fileName);

        }
        #endregion

        #region properties

        private string _mpassword = "Pentair";

        public string Password
        {
            get
            {
                return _mpassword;
            }
            set
            {
                _mpassword = value;
                //OnPr
            }
       }
        private void CreateJsonFile()
        {
        //    string jsonloginfpath = Path.
        //    JsonWriter.Write();
        }

        #endregion
    }
}
