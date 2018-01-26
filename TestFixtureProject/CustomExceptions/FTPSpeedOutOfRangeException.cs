using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFixtureProject
{
    public class FTPSpeedOutOfRangeException : Exception
    {
        public FTPSpeedOutOfRangeException()
        {
        }

        public FTPSpeedOutOfRangeException(string message)
        : base(message)
        {
        }

        public FTPSpeedOutOfRangeException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
