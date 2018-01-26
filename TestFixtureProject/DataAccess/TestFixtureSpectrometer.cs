using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFixtureProject.DataAccess
{
    class TestFixtureSpectrometer
    {
        #region private variables
        private TestFixtureReadSpectrometer _mTestFixturereadSpectrum = null;


        #endregion


        #region constructor

        public TestFixtureSpectrometer()
        {
           _mTestFixturereadSpectrum = new TestFixtureReadSpectrometer();
        }

        public bool ScannerForSpectrometer()
        {
          int count =  _mTestFixturereadSpectrum.OpenAllSpectrometers();
            if (count >= 1)
                return true;
            else
                return false;
        }
        public void DiscoverSpetroMeter()
        {
           _mTestFixturereadSpectrum.DiscoverSpetroMeter();
        }

        public string SetAquisitionParameter()
        {
          string err=  _mTestFixturereadSpectrum.SetAquisitionParameter();
          return err;
        }
        public string AcquireDarkSpectrum()
        {
          double [] darkSpectrum = null;
          string msg =   _mTestFixturereadSpectrum.AcquireDarkSpectrum(ref darkSpectrum);
          return msg;
        }

        public double[] GetAcquireDarkSpectrum()
        {
            double[] darkSpectrum = null;
            string msg = _mTestFixturereadSpectrum.AcquireDarkSpectrum(ref darkSpectrum);
            return darkSpectrum;
        }

        public string AcquireSampleSpectrum()
        {
            string msg;
          msg=  _mTestFixturereadSpectrum.AcquireSampleSpectrum();
            return msg;
        }
        public string ReadCalibrationFile()
        {
            string msg = "NoErrors";
            try
            {
                msg =_mTestFixturereadSpectrum.ReadCalibratedFile();
                //_mTestFixturereadSpectrum.GetCalibrationFile();
            }
            catch (Exception exp)
            {
                msg = exp.ToString();
            }
            return msg;
        }
        public void ComputeEnergySpectrum()
        {
            _mTestFixturereadSpectrum.ComputeEngerySpectrum();
        }
        public string ReadLowerXValue()
        {
           string lowerxvalue =  _mTestFixturereadSpectrum.getlowerXValue();
            return lowerxvalue;
        }
        public string ReadLowerYValue()
        {
            string lowerYValue = _mTestFixturereadSpectrum.getloweryValue();
            return lowerYValue;
        }
        public string ReadUpperXValue()
        {
            string upperXValue = _mTestFixturereadSpectrum.getUpperXValue();
            return upperXValue;
        }
        public string ReadUpperYValue()
        {
            string upperYValue = _mTestFixturereadSpectrum.getUpperYValue();
            return upperYValue;
        }
        public double ReadWattValue()
        {

            return _mTestFixturereadSpectrum.getWattvalue();
        }
        public void CloseSpectrometer()
        {
            _mTestFixturereadSpectrum.CloseSpectrometer();
        }
        #endregion
    }
}
