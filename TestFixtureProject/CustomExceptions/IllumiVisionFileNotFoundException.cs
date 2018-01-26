using System;

namespace TestFixtureProject
{
    public class IllumiVisionFileNotFoundException : Exception
    {
        public IllumiVisionFileNotFoundException()
        {
        }

        public IllumiVisionFileNotFoundException(string message)
        : base(message)
    {
        }

        public IllumiVisionFileNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
        }
    }
}
