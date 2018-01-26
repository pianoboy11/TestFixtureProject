using System;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace TestFixtureProject.DataAccess
{
    public class TestFixtureSerialPortComm: IDisposable
    {
        #region
        private string _mbarcodeserial = null;
        public  SerialPort _mSerialPort = null;
        public bool _misValueRead = false;
        public bool _mdatanotread = false;
        public bool _mdataEventtriggered = false;
        public int flag = 0;
        private string _mexceptionMessage = null;

       
        #endregion
        #region constructor
        public TestFixtureSerialPortComm()
        {

        }
        #endregion
        public void ListenToSerialPort(string commPortNumber)
        {
            try
            {
                 Dispose();
                //_mSerialPort = null;
                _mSerialPort = new SerialPort(commPortNumber);
                _mSerialPort.BaudRate = 9600;
                _mSerialPort.Parity = Parity.None;
                _mSerialPort.StopBits = StopBits.One;
                _mSerialPort.DataBits = 8;
                _mSerialPort.Handshake = Handshake.None;
                _mSerialPort.DtrEnable = true;

                try
                {                 
                    if (!_mSerialPort.IsOpen)
                        _mSerialPort.Open();
                }
                catch(System.IO.InvalidDataException exp)
                {
                    Dispose();
                    _mexceptionMessage = exp.ToString();
                }
                _mSerialPort.DiscardOutBuffer();
            }
            catch (Exception e)
            {
                _mdatanotread = true;
                Dispose();
                _mexceptionMessage = e.ToString();
            }
        }
        public bool OpenAndRead()
        {
            try
            {
                _mbarcodeserial = null;
                _mSerialPort.DataReceived += new SerialDataReceivedEventHandler(GetDataHandleFromSerialPort);

                _mSerialPort.RtsEnable = true;
             //   _mSerialPort.ReadTimeout = 5000;
            }
            catch (Exception e)
            {
                _mdatanotread = true;
                Dispose();
                _mexceptionMessage = e.ToString();
                return false;
            }
            return true;
        }
        private void GetDataHandleFromSerialPort(object sender, SerialDataReceivedEventArgs e)
        {

            try
            {
 
                Thread.Sleep(1000);
                _mSerialPort.ReadTimeout = 3000;
                _mbarcodeserial = _mSerialPort.ReadExisting();
                DisposeAndClose();
            }
            catch (TimeoutException texp)
            {
                _mdatanotread = true;
                Dispose();
                _mexceptionMessage = texp.ToString();
            }
            catch (Exception exp)
            {
                _mdatanotread = true;
                Dispose();
                _mexceptionMessage = exp.ToString();
            }
        }
        public string GetBarcodeScanInfo()
        {

            return _mbarcodeserial;
        }

        public void DisposeAndClose()
        {
            try
            {
                if (_mSerialPort.IsOpen)
                {

                    _mSerialPort.DiscardInBuffer();
                    Dispose();
                  //  _mSerialPort.Close();
                    _misValueRead = true;
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                MessageBox.Show(msg);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (_mSerialPort != null)
                {
                    _mSerialPort.Dispose();
                    _mSerialPort = null;
                }
            }
            // free native resources if there are any.
        }
        public string GetExceptionMessagesIfAny()
        {
            return _mexceptionMessage;
        }
    }
}
