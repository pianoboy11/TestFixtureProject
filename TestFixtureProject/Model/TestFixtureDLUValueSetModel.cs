using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFixtureProject.Common;
using TestFixtureProject.ViewModel;

namespace TestFixtureProject.Model
{
    public class TestFixtureDLUValueSetModel : TestFixtureLoginBaseVM
    {
        #region private members for Fixed Red button click

        private string _misadmin;
        private string _mrdiagnosticslowervalue1 = null;
        private string _mrdiagnosticslowervalue2 = null;
        private string _mrdiagnosticslowervalue3 = null;

        private string _mrdiagnosticsuppervalue1 = null;
        private string _mrdiagnosticsuppervalue2 = null;
        private string _mrdiagnosticsuppervalue3 = null;
        private double _mrdiagnosticwattvalue = 0.0;
        #endregion

        #region constructor
        private bool _mcollapsesettingstabitem;
        public TestFixtureDLUValueSetModel(bool canAccessallPages)
        {   //default sensor is not saturated and hence not visible. for testing purpose ...put bool as true
            IsSensorSaturated = true;
            _mcollapsesettingstabitem = canAccessallPages;
        }
        #endregion

        #region public members to set/get property values for Fixed Red Button click
        public string SetLowerLimitValues1
        {
            get { return _mrdiagnosticslowervalue1; }
            set
            {
                _mrdiagnosticslowervalue1 = value;
                OnPropertyChanged("SetLowerLimitValues1");
            }
        }
        public string SetUpperXValue
        {
            get { return _mrdiagnosticslowervalue2; }
            set
            {
                _mrdiagnosticslowervalue2 = value;
                OnPropertyChanged("SetUpperXValue");
            }
        }

        public string SetLowerLimitValue3
        {
            get { return _mrdiagnosticslowervalue3; }
            set
            {
                _mrdiagnosticslowervalue3 = value;
                OnPropertyChanged("SetLowerLimitValue3");
            }
        }
 
        public string SetLowerUpperValue1
        {
            get { return _mrdiagnosticsuppervalue1; }

            set
            {
                _mrdiagnosticsuppervalue1 = value;
                OnPropertyChanged("SetLowerUpperValue1");
            }
        }
        public string SetUpperYValue
        {
            get { return _mrdiagnosticsuppervalue2; }
            set
            {
                _mrdiagnosticsuppervalue2 = value;
                OnPropertyChanged("SetUpperYValue");
            }
        }

        public string SetLowerUpperValue3
        {
            get { return _mrdiagnosticsuppervalue3; }
            set
            {
                _mrdiagnosticsuppervalue3 = value;
                OnPropertyChanged("SetLowerUpperValue3");
            }

        }
        public double SetWattValue
        {
            get { return _mrdiagnosticwattvalue; }
            set
            {
                _mrdiagnosticwattvalue = value;
                OnPropertyChanged("SetWattValue");
            }

        }
        #endregion

        #region calls 'off' button click event
        private string _moffevent;
        #endregion
        #region 
        public string ClickOffButton
        {
            get
            {
                return _moffevent;
            }
            set
            {
                _moffevent = value;
                OnPropertyChanged("ClickOffButton");
            }
        }
        #endregion

        #region 12v Radio Button checked and 120v Radio Button Checked
        private bool _mIsTwelveRadioCheked = false;
        private bool _misonetwentyRadioChecked = false;
        #endregion

        #region methods to set/get properties related to radio button

        public bool IsTweleveRadioChecked
        {
            get { return _mIsTwelveRadioCheked; }
            set
            {
                _mIsTwelveRadioCheked = value;
                OnPropertyChanged("IsTweleveRadioChecked");
            }
        }

        public bool IsOneTwentyvRadioChecked
        {
            get { return _misonetwentyRadioChecked; }
            set
            {
                _misonetwentyRadioChecked = value;
                OnPropertyChanged("IsOneTwentyvRadioChecked");
            }
        }
        #endregion
        #region  to set readvoltage value
        private float _mreadvoltage = 0;
        public float ImpedanceVoltage
        {
            get { return _mreadvoltage; }
            set
            {
                _mreadvoltage = value;
                OnPropertyChanged("ImpedanceVoltage");
            }
        }
        #endregion
        #region
        private double _mAmbientTempValue = 0.0;
        public double ReadAmbientTemp
        {
            get { return _mAmbientTempValue; }
            set
            {
                _mAmbientTempValue = value;
                OnPropertyChanged("ReadAmbientTemp");
            }
        }
        #endregion

        #region - depending upon the property enable or disable diagnostic page
        public string IsAdmin
        {
            get { return _misadmin; }
            set
            {
                _misadmin = value;
                OnPropertyChanged("IsAdmin");
            }
        }
        #endregion

        #region

        private string _displayredcallibration;
        public string DisplayRedCallibration
        {
            get
            {
                return _displayredcallibration;
            }
            set
            {
                _displayredcallibration = value;
                OnPropertyChanged("DisplayRedCallibration");
            }
        }

        private string _displagreencallibration;

        public string DisplayGreenCallibration
        {
            get
            {
                return _displagreencallibration;
            }
            set
            {
                _displagreencallibration = value;
                OnPropertyChanged("DisplayGreenCallibration");
            }
        }

        private string _displaybluecallibration;

        public string DisplayBlueCallibration
        {
            get
            {
                return _displaybluecallibration;
            }
            set
            {
                _displaybluecallibration = value;
                OnPropertyChanged("DisplayBlueCallibration");
            }
        }

        #endregion
        #region read sensor values, temperature, red, green,blue sensor values from DAQ
        public void ReadColorSensorValues()
        {
            ReadTemperatureSensor();
            ReadRedSensor();
            ReadGreenSensor();
            ReadBlueSensor();
        }
        private void ReadTemperatureSensor()
        {


        }
        private void ReadRedSensor()
        {

        }
        private void ReadGreenSensor()
        {


        }
        private void ReadBlueSensor()
        {

        }
        #endregion

        #region sets the visibility of SensorSatureted label
        private bool _missensorsaturated;
        public bool IsSensorSaturated
        {
            get { return _missensorsaturated; }
            set
            {
                _missensorsaturated = value;
                OnPropertyChanged("IsSensorSaturated");
            }
        }
        #endregion
        #region sets the balastvoltage and temperature value
        private string _mstemvoltage;
        public string Setstemvoltage
        {
            get { return _mstemvoltage; }
            set
            {
                _mstemvoltage = value;
                OnPropertyChanged("Setstemvoltage");
            }
        }
        private string _mtemperaturevalue;

        public string SetTemperatureValue
        {
            get { return _mtemperaturevalue; }
            set
            {
                _mtemperaturevalue = value;
                OnPropertyChanged("SetTemperatureValue");
            }
        }
        #endregion
        #region dummy implementation to satisfy compiler
        public override void ButtonOKClick()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Access Visibility
        public bool CanAccessDaignosticPage
        {
            get { return _mcollapsesettingstabitem; }
        }
        #endregion
    }
}
