using System.Security;

namespace TestFixtureProject.Model
{
    class TestFixtureLoginModel : TestFixtureModelBase
    {
        private SecureString _mloginpassowrd;
        public TestFixtureLoginModel()
        {


        }

        public SecureString LoginPassword
        {
            get { return _mloginpassowrd; }
            set
            {
                _mloginpassowrd = value;
                OnPropertyChanged("LoginPassword");
            }
        }
    }
}
