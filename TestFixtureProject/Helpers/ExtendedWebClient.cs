﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace TestFixtureProject.Helpers
{
    public class ExtendedWebClient: WebClient
    {
        private int timeout;
        public int Timeout
        {
            get
            {
                return timeout;
            }
            set
            {
                timeout = value;
            }
        }
        public ExtendedWebClient(Uri address)
        {
            this.timeout = 5000;//In Milli seconds
            var objWebClient = GetWebRequest(address);
        }
        protected override WebRequest GetWebRequest(Uri address)
        {
            var objWebRequest = base.GetWebRequest(address);
            objWebRequest.Timeout = this.timeout;
            return objWebRequest;
        }
    }
}
