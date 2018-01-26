using System;
using System.Windows.Input;
using TestFixtureProject.Common;
using TestFixtureProject.Model;
using System.Security;
using System.Runtime.InteropServices;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace TestFixtureProject.ViewModel
{
    class TestFixtureLoginViewModel : TestFixtureLoginBaseVM
    {

        //singleton design pattern
        private static TestFixtureLoginViewModel _instance;

        public static TestFixtureLoginViewModel Instance
        {
            get
            {
                return _instance;
            }
        }

        private TestFixtureLoginModel _mloginModel;
        private SecureString _password;
        private bool _passwordIncorrect;
        private bool _accessGranted;
        private bool _cancelled;

        public TestFixtureLoginViewModel()
        {
            _mloginModel = new TestFixtureLoginModel();

            //_password = new SecureString();
            //_password.AppendChar('p');
            //_password.AppendChar('a');
            //_password.AppendChar('s');
            //_password.AppendChar('s');
            //_password.AppendChar('w');
            //_password.AppendChar('o');
            //_password.AppendChar('r');
            //_password.AppendChar('d');

            _instance = this;
        }

        public TestFixtureLoginModel LoginModel
        {
            get { return _mloginModel; }
            set
            {
                _mloginModel = value;
                OnPropertyChanged("LoginModel");
            }
        }
        public SecureString LoginPassword
        {
            get { return LoginModel.LoginPassword; }
            set
            {
                LoginModel.LoginPassword = value;
                OnPropertyChanged("LoginPassword");
            }
        }

        #region tabcontrolSelection

        private bool _mPartialAccess;
        public bool PartialAccessGranted
        {
            get { return _mPartialAccess; }
            set
            {
                _mPartialAccess = value;
                OnPropertyChanged("PartialAccessGranted");
            }
        }
        #endregion
        private int _mSelectedIndex;
        public int SelectedIndex
        {
            get { return _mSelectedIndex; }
            set
            {
                _mSelectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }
        public bool PasswordIncorrect
        {
            get { return _passwordIncorrect; }
            set
            {
                _passwordIncorrect = value;
                OnPropertyChanged("PasswordIncorrect");
            }
        }

        public bool AccessGranted
        {
            get { return _accessGranted; }
            set
            {
                _accessGranted = value;
                OnPropertyChanged("AccessGranted");
            }
        }

        public bool Cancelled
        {
            get { return _cancelled; }
            set
            {
                _cancelled = value;
                OnPropertyChanged("Cancelled");
            }
        }



        public ICommand ValidateAndNavigate
        {
            get
            {
                return new RelayCommand(CanExecute, ValidateToNavigate);
            }
        }


        public ICommand Cancel
        {
            get
            {
                return new RelayCommand(CanExecute, CancelledSelected);
            }
        }


        internal void ValidateToNavigate(Object p)
        {
            if (LoginPassword == null)
            {
                PasswordIncorrect = true;
                return;
            }

            //  string pass = LoginPassword.ToString();
            // int ret_val= pass.CompareTo("PASS");

            //should check the  password with the json file
            JObject passobject;
            JObject passobj;
            string loginFileName = TestFixtureConstants.GetLoginInfoFilePath();

            using (StreamReader file = File.OpenText(loginFileName))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                passobject = (JObject)JToken.ReadFrom(reader);
            }
            //JToken rootname = passobject.Root;
            //JToken lastelename = passobject.Last;

            //string rtname = rootname.First.ToString();
            //string ltname = lastelename.Last.ToString();

            JToken tokenvalue = passobject.First;
            string value = tokenvalue.First.ToString();

            JToken jvalue = passobject.Last;
            string lastvalue = jvalue.Last.ToString();


            _password = new SecureString();
            foreach (char c in value)
            {
                _password.AppendChar(c);
            }

            if (SecureStringEqual(LoginPassword, _password))
            {
                AccessGranted = true;
            }
            else if (!AccessGranted)
            {
                if (SelectedIndex >= 1 && SelectedIndex <= 2)
                {
                    _password = new SecureString();
                    foreach (char c in lastvalue)
                    {
                        _password.AppendChar(c);
                    }
                    if (SecureStringEqual(LoginPassword, _password))
                    {
                        PartialAccessGranted = true;
                    }
                }
                else if (SelectedIndex >= 3 && SelectedIndex <= 4)
                {
                    PasswordIncorrect = true;
                }
            }
            else
            {
                PasswordIncorrect = true;
            }
        }


        private void CancelledSelected(Object p)
        {
            Cancelled = true;
        }


        private bool CanExecute()
        {
            return true;
        }

        Boolean SecureStringEqual(SecureString secureString1, SecureString secureString2)
        {
            if (secureString1 == null)
            {
                throw new ArgumentNullException("s1");
            }
            if (secureString2 == null)
            {
                throw new ArgumentNullException("s2");
            }

            if (secureString1.Length != secureString2.Length)
            {
                return false;
            }

            IntPtr ss_bstr1_ptr = IntPtr.Zero;
            IntPtr ss_bstr2_ptr = IntPtr.Zero;

            try
            {
                ss_bstr1_ptr = Marshal.SecureStringToBSTR(secureString1);
                ss_bstr2_ptr = Marshal.SecureStringToBSTR(secureString2);

                String str1 = Marshal.PtrToStringBSTR(ss_bstr1_ptr);
                String str2 = Marshal.PtrToStringBSTR(ss_bstr2_ptr);

                return str1.Equals(str2);
            }
            finally
            {
                if (ss_bstr1_ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(ss_bstr1_ptr);
                }

                if (ss_bstr2_ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(ss_bstr2_ptr);
                }
            }
        }

        public override void ButtonOKClick()
        {
            throw new NotImplementedException();
        }
    }
}
