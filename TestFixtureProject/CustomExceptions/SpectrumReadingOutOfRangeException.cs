using System;

namespace TestFixtureProject
{
    public class SpectrumReadingOutOfRangeException : Exception
    {
        public SpectrumReadingOutOfRangeException()
        {
        }

        public SpectrumReadingOutOfRangeException(string message)
        : base(message)
        {
        }

        public SpectrumReadingOutOfRangeException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
