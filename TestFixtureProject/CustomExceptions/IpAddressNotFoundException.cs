using System;

namespace TestFixtureProject
{
    public class IpAddressNotFoundException: Exception
    {
        public IpAddressNotFoundException()
        {
        }

        public IpAddressNotFoundException(string message)
        : base(message)
    {
        }

        public IpAddressNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
        }
    }
}
