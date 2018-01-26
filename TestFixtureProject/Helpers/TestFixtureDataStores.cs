using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFixtureProject.Helpers
{
    class TestFixtureDataStores
    {
        private static readonly TestFixtureDataStores storeinstance = new TestFixtureDataStores();

        private TestFixtureDataStores() { }

        public static Dictionary<String, String> Information
        {
            get
            {
                if (_Information == null)
                    _Information = new Dictionary<string, string>();
                return _Information;
            }
            set { _Information = value; }
        } static Dictionary<String, String> _Information;

        public static String ExeDir { get; set; }

        public static String ExeFile { get; set; }
        
        public static String ExePath { get; set; }

    }
}
