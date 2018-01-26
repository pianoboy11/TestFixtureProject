using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFixtureProject.ViewModel;

namespace TestFixtureProject.Model
{
   public class TestFixtureTestMainModel : TestFixtureLoginBaseVM
   {

       #region constructor
       public TestFixtureTestMainModel()
        {
        

        }

        #endregion

        #region set/get properties


        private string _mgetVersionNumber;
        public string VersionNumber
        {
            get
            {
                return _mgetVersionNumber;
            }
            set
            {
                _mgetVersionNumber = value;
                OnPropertyChanged("VersionNumber");
            }
        }
        private string _mBarCodeVoltage;
        public string UpdateVoltage
        {
            get
            {
                return _mBarCodeVoltage;
            }
            set
            {
                _mBarCodeVoltage = value;
                OnPropertyChanged("UpdateVoltage");
            }
        }

        private string _mSerialNumber = null;
        public string UpdateSerialNumber
        {
            get
            {
                return _mSerialNumber;
            }
            set
            {
                _mSerialNumber = value;
                OnPropertyChanged("UpdateSerialNumber");
            }
        }
        private string _mModelNumber = null;

        public string ReadModelNumber
        {
            get
            {
                return _mModelNumber;
            }
            set
            {
                _mModelNumber = value;
                OnPropertyChanged("ReadModelNumber");
            }
        }
        private bool _mIsvoltageok = false;
        public bool IsVoltageOK
        {
            get { return _mIsvoltageok; }
            set
            {
                _mIsvoltageok = value;
                OnPropertyChanged("IsVoltageOK");
            }
        }
        private bool _mIsWhiteColorOk = false;
        public bool IsWhiteColorOk
        {
            get { return _mIsWhiteColorOk; }
            set
            {
                _mIsWhiteColorOk = value;
                OnPropertyChanged("IsWhiteColorOk");
            }

        }
        private bool _mIsGreenColorOk = false;
        public bool IsGreenColorOk
        {
            get { return _mIsGreenColorOk; }
            set
            {
                _mIsGreenColorOk = value;
                OnPropertyChanged("IsGreenColorOk");
            }
        }

        private bool _mIsBlueColorOk = false;
        public bool IsBlueColorOk
        {
            get { return _mIsBlueColorOk; }
            set
            {
                _mIsBlueColorOk = value;
                OnPropertyChanged("IsBlueColorOk");
            }
        }

        private bool _mIsRedColorOk = false;
        public bool IsRedColorOk
        {
            get { return _mIsRedColorOk; }
            set
            {
                _mIsRedColorOk = value;
                OnPropertyChanged("IsRedColorOk");
            }
        }

        private string _mVersionNumber = null;
        public string GetVersionNumber
        {
            get { return _mVersionNumber; }
            set
            {
                _mVersionNumber = value;
                OnPropertyChanged("GetVersionNumber");
            }
        }

        private string _mTestResult = null;
        public string UpdatePassFailResult
        {
            get { return _mTestResult; }
            set
            {
                _mTestResult = value;
                OnPropertyChanged("UpdatePassFailResult");
            }
        }

        private Brush _mTestResultBackColor = null;
        public Brush UpdateBackgroundColor
        {
            get { return _mTestResultBackColor; }
            set
            {
                _mTestResultBackColor = value;
                OnPropertyChanged("UpdateBackgroundColor");
            }
        }
        #endregion
        public override void ButtonOKClick()
        {
            throw new NotImplementedException();
        }

    }
}
