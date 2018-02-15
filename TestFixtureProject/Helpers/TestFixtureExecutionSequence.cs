using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using TestFixtureProject.Common;

namespace TestFixtureProject.Helpers
{
    public class TestFixtureExecutionSequence
    {

        public TestFixtureExecutionSequence()
        {

        }

        public string GetTestSequnceName()
        {
            JObject passobject;

            string testsequencefile = TestFixtureConstants.GetTestSequenceFromJson();

            using (StreamReader file = File.OpenText(testsequencefile))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                passobject = (JObject)JToken.ReadFrom(reader);
            }
            JToken tokenvalue = passobject.First;
            string seqname = tokenvalue.First.ToString();
            return seqname;
        }
    }
}
