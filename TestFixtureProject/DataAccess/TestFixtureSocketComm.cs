using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using Newtonsoft.Json.Linq;
using TestFixtureProject.Helpers;
using System.Net.Http;
using TestFixtureProject.Common;

namespace TestFixtureProject.DataAccess
{

    public static class TestFixtureSocketComm
    {
        public static bool _IsDelayCheck = false;
        public static string _RetrivedIpAddress = string.Empty;
        public static bool _IsIpAddressFound = false;

        #region private variables
        public static ViewModel.TestFixtureViewModel vm = null;
        public static int _numberOfRetries = 0;
        public static int _numberOfRetry = 0;
        public static bool _isOuterloop = false;
        public static bool _isInnerloop = false;
        private static bool serverconnected = false;
        private static bool _retvalue = false;
        private static IPAddress _mServerIPAddress;
        private static string version_number = null;
        private static string _mResponseFromServer = null;
        private static string _mexceptionMessage = null;
        public static string serverStatus = "NOTAVAILABLE";
        private static string notConnected = "NOTAVAILABLE";
        internal static string connected = "AVAILABLE";
        private static string interrupted = "INTERRUPT";
        public static string serverUnavailable = " SERVER UNAVAILABLE";
        public static string firmwareVersion = string.Empty;
        #endregion

        #region function to test the connection and device response

        public static string SendCommandToServerToProcess(string commnd)
        {
            if (_mServerIPAddress == null)
                return notConnected;

            _numberOfRetries = 0;
            while (_numberOfRetries <= 3)
            {
                if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                    return "Cancelled SendCommandToServerToProcess operation";

                try
                {
                    _mResponseFromServer = null;
                    HttpWebRequest request;
                    if (_mServerIPAddress == null)
                    {
                        serverStatus = notConnected;
                        return serverStatus;
                    }


                    string ipaddr = _mServerIPAddress.ToString();
                    string portnumber = ":8080";
                    ipaddr = "http://" + ipaddr + portnumber;
                    string addiparams = "/api/";
                    ipaddr = ipaddr + addiparams;

                    string post_data = commnd;
                    //string post_data = "cmd=getVersion";
                    string uri = ipaddr;
                    request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Timeout = 1000;

                    try
                    {
                        request.KeepAlive = false;
                        request.ProtocolVersion = HttpVersion.Version10;
                        request.ServicePoint.Expect100Continue = false;
                        request.Method = @"POST";
                        //// turn our request string into a byte stream
                        byte[] postBytes = Encoding.ASCII.GetBytes(post_data);
                        request.ContentLength = postBytes.Length;
                        //// this is important - make sure you specify type this way
                        request.ContentType = @"application/x-www-form-urlencoded";
                        // request.ContentType = "text/json";

                        // Thread.Sleep(1000);

                        Stream requestStream = request.GetRequestStream();
                        //// now send it
                        requestStream.Write(postBytes, 0, postBytes.Length);
                        requestStream.Close();

                        //// grab te response and print it out to the console along with the status code
                        //HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                        using (WebResponse response = (HttpWebResponse)request.GetResponse())
                        {

                            HttpWebResponse httpResponse = response as HttpWebResponse;
                            using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                _mResponseFromServer = reader.ReadToEnd();

                                _isOuterloop = true;
                                _isInnerloop = true;
                                _numberOfRetries = 4;
                                _retvalue = true;
                            }

                            //requestStream = response.GetResponseStream();
                            //// Open the stream using a StreamReader for easy access.
                            //StreamReader reader = new StreamReader(requestStream);
                            //// Read the content.
                            //_mResponseFromServer = reader.ReadToEnd();

                            //reader.Close();
                            //requestStream.Close();
                            //response.Close();
                            //request.Abort();

                            //_isOuterloop = true;
                            //_isInnerloop = true;
                            //_numberOfRetries = 4;
                            //_retvalue = true;
                        }
                    }
                    catch (WebException e)
                    {
                        Thread.Sleep(1500);
                        _isOuterloop = false;
                        _isInnerloop = false;
                        _numberOfRetries++;
                        if (_numberOfRetries > 3)
                        {
                            _isOuterloop = true;
                            _isInnerloop = true;
                            _retvalue = false;
                        }
                        _mexceptionMessage = e.Message.ToString();

                        if (frmTestFixture.Instance != null)
                            frmTestFixture.Instance.WriteToLog(_numberOfRetries.ToString() + ":" + _mexceptionMessage, ApplicationConstants.TraceLogType.Error);
                    }
                    finally
                    {
                        // _retvalue = false;
                        request.Abort();
                        //MessageBox.Show("request.Abort()")
                    }
                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                    string msg = e.ToString();
                    _isOuterloop = false;
                    _isInnerloop = false;
                    _numberOfRetries++;
                    if (_numberOfRetries > 3)
                    {
                        _isOuterloop = true;
                        _isInnerloop = true;
                        _retvalue = false;
                    }
                    _mexceptionMessage = e.Message.ToString();
                }
            }
            if (_retvalue)
            {
                serverStatus = connected;
                return serverStatus;
            }
            else
            {

                serverStatus = notConnected;
                return serverStatus;
            }

        }
        // Just for local tests


        internal static string SendCommandToServerToProcess2(string command)
        {
            try
            {
                if (_mServerIPAddress == null)
                    return notConnected;

                string ipaddr = _mServerIPAddress.ToString();
                string portnumber = ":8080";
                ipaddr = "http://" + ipaddr + portnumber;
                string addiparams = "/api?";
                ipaddr = ipaddr + addiparams;

                string post_data = command;

                //string post_data = "cmd=getVersion";
                string uri = ipaddr + command;

                string str = "";

                using (WebClient client = new WebClient())
                {         
                    Stream data = client.OpenRead(uri);

                    data.ReadTimeout = 5000;

                    using (StreamReader reader = new StreamReader(data))
                    {
                        str = reader.ReadToEnd();

                        if (str.Contains("success"))
                        {
                            try
                            {
                                if (string.IsNullOrEmpty(firmwareVersion))
                                {
                                    string[] array = str.Split('"');

                                    if (array != null)
                                    {
                                        if (array.Count() > 7)
                                        {
                                            firmwareVersion = array[7].ToString();

                                            if (firmwareVersion != "success")
                                            {
                                                if (firmwareVersion == "1.1.106")
                                                {
                                                    LogStructureNew.FirmwareVersion = firmwareVersion;
                                                    frmTestFixture.Instance.SetVersionNumberTextbox(firmwareVersion);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch(Exception e)
                            {
                                frmTestFixture.Instance.WriteToLog("FIRMWARE VERSION: " + e.Message, ApplicationConstants.TraceLogType.Error);
                            }

                            _retvalue = true;
                        }
                        else
                            _retvalue = false;

                        _isOuterloop = true;
                        _isInnerloop = true;
                        _numberOfRetries = 4;
                    }
                }

                if (_retvalue)
                {
                    serverStatus = connected;
                    return serverStatus;
                }
                else
                {

                    serverStatus = notConnected;
                    return serverStatus;
                }

            }
            catch (Exception e)
            {
                frmTestFixture.Instance.WriteToLog(e.Message, ApplicationConstants.TraceLogType.Error);
                serverStatus = notConnected;
                return serverStatus;
            }
        }

        public static bool PingTest(string commnd)
        {
            try
            {
                _mResponseFromServer = null;
                HttpWebRequest request;
                if (_mServerIPAddress == null)
                    return false;

                string ipaddr = _mServerIPAddress.ToString();
                string portnumber = ":8080";
                string ipPort = ipaddr + portnumber;
                ipaddr = "http://" + ipaddr + portnumber;
                string addiparams = "/api/";
                ipaddr = ipaddr + addiparams;

                string post_data = commnd;
                string uri = ipaddr;
                try
                {
                    Ping ping = new Ping();
                    PingReply reply = ping.Send("192.168.1.61:8080", 1000);
                    if (reply != null)
                    {
                        Console.WriteLine("Status :  " + reply.Status + " \n Time : " + reply.RoundtripTime.ToString() + " \n Address : " + reply.Address);
                        //Console.WriteLine(reply.ToString());
                    }
                }
                catch (Exception e)
                {

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("PING Error:" + e);
            }
            return _retvalue;
        }

        #endregion
          
        #region reset number of retries
        public static void ResetRetryCount()
        {
            _numberOfRetries = 0;
        }
        #endregion

        #region Version Number

        public static string GetVersionNumber()
        {
            var versionnum = string.Empty;
            if (_mResponseFromServer != null)
            {
                dynamic resp = JObject.Parse(_mResponseFromServer);
                versionnum = resp.ver;
                if(version_number == null || version_number.Equals(""))
                version_number = resp.ver;
            }
            return version_number;
        }

        #endregion

        #region function discover pentair devices
        public static string DiscoverPentairServer()
        {
            vm = new ViewModel.TestFixtureViewModel();
            vm.openFile = true;
            
            serverStatus = "NOTAVAILABLE";
            try
            {
                var nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var nic in nics)
                {
                    if (serverStatus.Equals(connected))
                        break;
                    if (serverStatus.Equals(interrupted))
                        break;
                    if (serverStatus.Equals(serverUnavailable))
                        break;
                    //  _mServerIPAddress = null;
                    var ippropoerties = nic.GetIPProperties();
                    //we are only interested in IPv4 address
                    var ipv4add = ippropoerties.UnicastAddresses.Where(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork);
                    foreach (var addr in ipv4add)
                    {
                        var network = CalculateNetwork(addr);
                        Console.WriteLine("Addr: {0}   Mask: {1}  Network: {2}", addr.Address, addr.IPv4Mask, network);

                        string retrivedip = addr.Address.ToString();

                        string local_host = Dns.GetHostName(); // returns local hostname

                        IPHostEntry host_entry = Dns.GetHostEntry(Dns.GetHostName());

                        foreach (var ip in host_entry.AddressList)
                        {
                            int delay = 1;
                            LogStructureNew.DeviceDiscover = "FAIL";

                            if (_IsDelayCheck)
                            {
                                bool flag = vm.CheckTriggerDelayOnce(delay);
                                if (!flag)
                                {
                                    serverStatus = interrupted;
                                    return serverStatus;
                                }
                            }

                            TryToConnect(ip, ref retrivedip);
                            if (serverStatus.Equals(connected))
                                break;
                        }

                    }
                }

            }
            catch (Exception e)
            {
                _mexceptionMessage = e.Message.ToString();
            }

            return serverStatus;
        }

        public static string DiscoverPentairServer(bool IsDelayCheck)
        {

            _IsDelayCheck = IsDelayCheck;

            serverStatus = "NOTAVAILABLE";
            string addressInfo = string.Empty;
            
            //serverStatus = CheckForExistanceofIPAddress();
            if (_IsIpAddressFound)
                return "AVAILABLE";

            //if (vm == null)
            //{
            //    vm = new ViewModel.TestFixtureViewModel();
            //    vm.openFile = true;
            //}

            try
            {
                var nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var nic in nics)
                {
                    if (serverStatus.Equals(connected))
                        break;
                    if (serverStatus.Equals(interrupted))
                        break;
                    if (serverStatus.Equals(serverUnavailable))
                        break;

                    //  _mServerIPAddress = null;
                    var ippropoerties = nic.GetIPProperties();
                    //we are only interested in IPv4 address
                    var ipv4add = ippropoerties.UnicastAddresses.Where(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork);
                    foreach (var addr in ipv4add)
                    {
                        var network = CalculateNetwork(addr);
                        Console.WriteLine("Addr: {0}   Mask: {1}  Network: {2}", addr.Address, addr.IPv4Mask, network);

                        string retrivedip = addr.Address.ToString();

                        //Saved ip address
                        addressInfo = retrivedip;

                        string local_host = Dns.GetHostName(); // returns local hostname

                        IPHostEntry host_entry = Dns.GetHostEntry(Dns.GetHostName());

                        foreach (var ip in host_entry.AddressList)
                        {
                            int delay = 1;
                            LogStructureNew.DeviceDiscover = "FAIL";

                            if (_IsDelayCheck)
                            {
                                bool flag = vm.CheckTriggerDelayOnce(delay);
                                if (!flag)
                                {
                                    serverStatus = interrupted;
                                    return serverStatus;
                                }
                            }

                            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                            {
                                serverStatus = "Cancelled SendCommandToServerToProcess operation";
                                frmTestFixture.Instance.WriteToLog(serverStatus, ApplicationConstants.TraceLogType.Information);
                                return "Cancelled SendCommandToServerToProcess operation";
                            }

                            TryToConnect(ip, ref retrivedip);
                            if (serverStatus.Equals(connected))
                                break;
                        }

                        //Saved ip address
                        addressInfo = retrivedip;
                    }
                }

                string serverStatusMessage = String.Format("IP Address '{0}': {1} ", _RetrivedIpAddress, serverStatus);

                if (frmTestFixture.Instance != null)
                    frmTestFixture.Instance.SetIpAddressLabel(serverStatusMessage);

                if (frmTestFixture.Instance != null)
                {
                    if (serverStatus.Equals(connected))
                        frmTestFixture.Instance.SetIpAddressTextBox(addressInfo);
                }
            }
            catch (Exception e)
            {
                _mexceptionMessage = e.Message.ToString();
            }

            return serverStatus;
        }

        public static string DiscoverPentairServer(string ipAddress)
        {

            if (!TestFixtureProject.ViewModel.TestFixtureViewModel.Instance.Start120ACVRelayOn())
            {
                string voltageStatus = "Start120ACVRelayOn - Error occured.";

                frmTestFixture.Instance.SetIpAddressLabel(voltageStatus);
                return voltageStatus;
            }

            vm = new ViewModel.TestFixtureViewModel();
            vm.openFile = true;

            serverStatus = "NOTAVAILABLE";
            string addressInfo = string.Empty;

            try
            {
                IPAddress address = IPAddress.Parse(_mServerIPAddress.ToString());

                Console.WriteLine("Addr: {0}  ", _mServerIPAddress.ToString());

                //Saved ip address
                addressInfo = _mServerIPAddress.ToString();

                ConnectToServer(address);
                if (serverStatus.Equals(connected))
                {
                    LogStructureNew.DeviceDiscover = "PASS";
                    _RetrivedIpAddress = address.ToString();
                    TestFixtureSocketComm._IsIpAddressFound = true;
                }
                else
                {
                    LogStructureNew.DeviceDiscover = "FAIL";
                    _RetrivedIpAddress = string.Empty;
                    TestFixtureSocketComm._IsIpAddressFound = false;
                }

                string serverStatusMessage = String.Format("IP Address '{0}': {1} ", _RetrivedIpAddress, serverStatus);

                if (frmTestFixture.Instance != null)
                {
                    frmTestFixture.Instance.SetIpAddressLabel(serverStatusMessage);

                    frmTestFixture.Instance.SetErrorMessageDisplayTextBox(serverStatusMessage);
                    frmTestFixture.Instance.WriteToLog(serverStatusMessage, ApplicationConstants.TraceLogType.Information);

                    if (serverStatus.Equals(connected))
                        frmTestFixture.Instance.SetIpAddressTextBox(addressInfo);

                    frmTestFixture.Instance.SetErrorMessageDisplayTextBox(addressInfo);
                    frmTestFixture.Instance.WriteToLog(addressInfo, ApplicationConstants.TraceLogType.Information);
                }

                return serverStatus;
            }
            catch (Exception e)
            {
                _mexceptionMessage = e.Message.ToString();
                 return serverStatus;
            }
        }
        #endregion

        #region find available IP add ranges 
        private static IPAddress CalculateNetwork(UnicastIPAddressInformation addrs)
        {
            if (addrs.IPv4Mask == null)
                return null;

            var ip = addrs.Address.GetAddressBytes();
            var mask = addrs.IPv4Mask.GetAddressBytes();
            var result = new Byte[4];
            for (int i = 0; i < 4; ++i)
            {
                result[i] = (Byte)(ip[i] & mask[i]);
            }

            return new IPAddress(result);
        }
        #endregion

        #region with each api, it will try to connect
        private static string TryToConnect(IPAddress ipaddress, ref string retrivedIp)
        {
            _RetrivedIpAddress = string.Empty;

            if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
            {

                string localip = ipaddress.ToString();
                int ret_val = string.Compare(retrivedIp, localip);
                if (ret_val == 0)
                {
                    int len = retrivedIp.Length;
                    int pos = retrivedIp.LastIndexOf(".");
                    string subip = retrivedIp.Substring(0, pos + 1);

                    //now from this string.. start sending request to server by appending numbers 
                    for (int i = 0; i < 255; i++)
                    {
                        string newsubip = subip + i.ToString();
                        try
                        {
                            IPAddress address = IPAddress.Parse(newsubip);

                            if (frmTestFixture.Instance != null)
                            {
                                frmTestFixture.Instance.SetIpAddressLabel("Checking IP Address: " + address.ToString());
                                frmTestFixture.Instance.WriteToLog("Checking IP Address: " + address.ToString(), ApplicationConstants.TraceLogType.Information);
                            }

                            int delay = 1;
                            LogStructureNew.DeviceDiscover = "FAIL";

                            if (_IsDelayCheck)
                            {
                                bool flag = vm.CheckTriggerDelayOnce(delay);
                                if (!flag)
                                {
                                    serverStatus = interrupted;
                                    return serverStatus;
                                }
                            }

                            if (frmTestFixture.Instance.backgroundWorker1.CancellationPending)
                                return "Cancelled SendCommandToServerToProcess operation";

                            ConnectToServer(address);
                            if (serverStatus.Equals(connected))
                            {
                                LogStructureNew.DeviceDiscover = "PASS";
                                _RetrivedIpAddress = retrivedIp = address.ToString();
                               _IsIpAddressFound = true;
                                frmTestFixture.Instance.WriteToLog("IP Address: " + address.ToString(), ApplicationConstants.TraceLogType.Information);
                                break;
                            }
                            else
                            {
                                LogStructureNew.DeviceDiscover = "FAIL";
                                _RetrivedIpAddress = retrivedIp = string.Empty;
                                _IsIpAddressFound = false;
                                frmTestFixture.Instance.WriteToLog("IP Address Not Found...", ApplicationConstants.TraceLogType.Error);

                                if (i == Convert.ToInt32(frmTestFixture.Instance.pagevm._model.IpAddressRange))
                                {
                                    string msg = string.Format("IP Address '{0}' Not Found. IllumiVision Server is unavailable. Please see illumavision support team...", address.ToString());
                                    frmTestFixture.Instance.WriteToLog(msg, ApplicationConstants.TraceLogType.Warning);
                                    serverStatus = serverUnavailable;
                                    //throw new IpAddressNotFoundException(msg);
                                    //break;
                                    //throw new IpAddressNotFoundException(msg);
                                }
                            }
                        }
                        catch (IpAddressNotFoundException e)
                        {
                            _mexceptionMessage = e.Message.ToString();
                            frmTestFixture.Instance.WriteToLog("TryToConnect IpAddressNotFoundException: " + e.Message, ApplicationConstants.TraceLogType.Error);

                            return serverStatus = serverUnavailable;
                        }
                        catch (SocketException e)
                        {
                            _mexceptionMessage = e.Message.ToString();
                            frmTestFixture.Instance.WriteToLog("TryToConnect SocketException ERROR: " + e.Message, ApplicationConstants.TraceLogType.Error);
                        }
                        catch(Exception ex)
                        {
                            _mexceptionMessage = ex.Message.ToString();
                            frmTestFixture.Instance.WriteToLog("TryToConnect Exception ERROR: " + ex.Message, ApplicationConstants.TraceLogType.Error);
                        }
                    }
                }
            }
            return serverStatus;
        }
        #endregion

        #region connect to server
        internal static string ConnectToServer(IPAddress connAddress)
        {
            try
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress address = IPAddress.Parse(connAddress.ToString());
                IPEndPoint hostep = new IPEndPoint(connAddress, 8080);

                IAsyncResult result = sock.BeginConnect(hostep, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(500, true);
                if (success)
                {
                    int delay = 1;
                    LogStructureNew.DeviceDiscover = "FAIL";

                        _numberOfRetries = 1;
                        _mServerIPAddress = address;
                        string getVersionComd = TestFixtureCommands._mgetVersionCommd;
                        string serverCheck = SendCommandToServerToProcess2(getVersionComd);
                        serverStatus = serverCheck;
                        if (serverStatus.Equals(connected))
                        {
                            dynamic resp = JObject.Parse(_mResponseFromServer);
                            if (version_number == null || version_number.Equals(""))
                                version_number = resp.ver;
                        }

                    //bool triggerFlag = vm.CheckTriggerDelayOnce(delay);
                    //if (!triggerFlag)
                    //{
                    //    serverStatus = interrupted;
                    //    return serverStatus;
                    //}
                    //else
                    //{
                    //    _numberOfRetries = 1;
                    //    _mServerIPAddress = address;
                    //    string getVersionComd = TestFixtureCommands._mgetVersionCommd;
                    //    string serverCheck = SendCommandToServerToProcess2(getVersionComd);
                    //    serverStatus = serverCheck;
                    //    if (serverStatus.Equals(connected))
                    //    {
                    //        dynamic resp = JObject.Parse(_mResponseFromServer);
                    //        if (version_number == null || version_number.Equals(""))
                    //            version_number = resp.ver;
                    //    }
                    //}
                }
            }
            catch (SocketException e)
            {
                _mexceptionMessage = e.Message.ToString();

            }
            catch (Exception e)
            {
                _mexceptionMessage = e.Message.ToString();
            }
            finally
            {

            }
            return serverStatus;
        }
        public static string GetServerIpAddress()
        {
            return _mServerIPAddress.ToString();
        }
        public static void SetServerIpAddress(string address)
        {
            _mServerIPAddress = IPAddress.Parse(address);
        }
        #endregion

        #region server response
        public static string GetServerResponse()
        {
            return _mResponseFromServer;
        }
        #endregion

        #region exception or messages
        public static string ExceptionHandling()
        {
            return _mexceptionMessage;

        }

        public static double[] checkBandwidth()
        {
            double[] data = null;
            _numberOfRetry = 0;
            while (_numberOfRetry <= 3)
            {
                data = new double[2];
                data[0] = 0;
                data[1] = 0;
                string bandwidthDir = TestFixtureConstants.getBandwidthUploadDirPath();
                //string[] fileNames = GetFileNames(bandwidthDir);
                int i = 0;
                string ipaddr = GetServerIpAddress();
                string portnumber = ":8080";
                ipaddr = "http://" + ipaddr + portnumber;
                string addiparams = "/media/update/";
                ipaddr = ipaddr + addiparams;
                double bw = 0;
                WebClient client = new WebClient();

               
                string[] fileArray = Directory.GetFileSystemEntries(bandwidthDir);

                try
                {
                    for (i = 0; i < fileArray.Length; i++)
                    {
                        client.Credentials = CredentialCache.DefaultCredentials;
                        //client.Encoding = Encoding.UTF8;
                        //request.ContentType = @"application/x-www-form-urlencoded";
                        // client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                        //client.Headers.Add();
                        //MessageBox.Show(DateTime.Now.ToShortTimeString().ToString());
                        DateTime start = DateTime.Now;
                        client.UploadFile(ipaddr, @"POST", fileArray[i]);

                        DateTime end = DateTime.Now;
                        TimeSpan timeDiff = end - start;
                        //MessageBox.Show (timeDiff.TotalMilliseconds.ToString());
                        bw = (fileArray[i].Length * 8) / (timeDiff.TotalSeconds * 1000);
                        data[0] = timeDiff.TotalSeconds;
                        data[1] = bw;
                        _isOuterloop = true;
                        _isInnerloop = true;
                        _numberOfRetry = 4;
                        _retvalue = true;
                        client.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Thread.Sleep(1500);
                    string msg = e.ToString();
                    _isOuterloop = false;
                    _isInnerloop = false;
                    _numberOfRetry++;
                    if (_numberOfRetry > 3)
                    {
                        _isOuterloop = true;
                        _isInnerloop = true;
                        _retvalue = false;
                    }
                    _mexceptionMessage = e.Message.ToString();
                }
            }

            return data;
        }

        #endregion

        #region to load  the imgages to webserver
        public static bool fileUpload()
        {
            try
            {

            string imageDir = TestFixtureConstants.getImageUploadDirPath();
            int i = 0;
            string ipaddr = GetServerIpAddress();
            string portnumber = ":8080";
            ipaddr = "http://" + ipaddr + portnumber;
            string addiparams = "/media/image/";
            ipaddr = ipaddr + addiparams;
            string[] fileArray = Directory.GetFileSystemEntries(imageDir);
            //try
            //{
                for (i = 0; i < fileArray.Length; i++)
                {
                    //create WebClient object
                    WebClient client = new WebClient();
                    client.Proxy = null;
                    client.Credentials = CredentialCache.DefaultCredentials;
                    //client.UploadFile(ipaddr, "POST", fileArray[i]);
                    client.UploadFile(ipaddr, "POST", @"C:\Builds\TestFixtureProject\TestFixtureProject\bin\Debug\Images\ImageShow\White--Dot.png" );
                    client.Dispose();
                }
            }
            catch (Exception err)
            {
                frmTestFixture.Instance.WriteToLog(err.Message, ApplicationConstants.TraceLogType.Error);
                return false;
            }

            return true;
        }

        private static string[] GetFileNames(string path)
        {
            string[] files = Directory.GetFiles(path);
            for (int i = 0; i < files.Length; i++)
                files[i] = Path.GetFileName(files[i]);
            return files;
        }

        public static bool DeleteUploadedFile(string fileType)
        {
            string[] allDir = null;
            string imageDir = null;
            string bandwidthDir = null;
            int i = 0;
            string[] fileNames = null;
            string ipaddr = null;
            string portnumber = null;
            string addiparams;
            if (fileType.Equals("image"))
            {
                imageDir = TestFixtureConstants.getImageUploadDirPath();
                fileNames = GetFileNames(imageDir);
                i = 0;
                for (i = 0; i < fileNames.Length; i++)
                {
                    WebRequest request = null;
                    HttpClient client = null;

                    HttpWebResponse response = null;
                    ipaddr = GetServerIpAddress();
                    portnumber = ":8080";
                    ipaddr = "http://" + ipaddr + portnumber;
                    addiparams = "/media/image";
                    ipaddr = ipaddr + addiparams;
                        try
                        {
                        //client.DeleteAsync("");
                            request = WebRequest.Create(ipaddr + "/" + fileNames[i]);
                            //request.ContentType = @"application/x-www-form-urlencoded";
                            request.Method = @"DELETE";
                            response = (HttpWebResponse)request.GetResponse();
                        }
                        catch (Exception err)
                        {
                        // if(err.)
                            return true;
                        }
                }
                return true;
            }
            else if (fileType.Equals("bandwidth"))
            {
                bandwidthDir = TestFixtureConstants.getBandwidthUploadDirPath();
                fileNames = GetFileNames(bandwidthDir);
                i = 0;
                for (i = 0; i < fileNames.Length; i++)
                {
                    ipaddr = GetServerIpAddress();
                    portnumber = ":8080";
                    ipaddr = "http://" + ipaddr + portnumber;
                    addiparams = "/media/update";
                    ipaddr = ipaddr + addiparams;
                        try
                        {
                            WebRequest request = WebRequest.Create(ipaddr + "/" + fileNames[i]);
                            request.ContentType = @"application/x-www-form-urlencoded";
                            request.Method = @"DELETE";

                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                        }
                        catch (Exception err)
                        {
                            return false;
                        }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }

}
