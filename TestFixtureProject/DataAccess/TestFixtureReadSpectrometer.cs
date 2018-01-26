using SPAM;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using TestFixtureProject.Common;
using System.Windows.Forms;

using TestFixtureProject.Helpers;

namespace TestFixtureProject.DataAccess
{

    public class TestFixtureReadSpectrometer
    {
        #region private member variables 
            internal OmniDriver.CCoWrapper  omniwraper = null;
            SPAM.CCoSpectralMath omnispecmath = null;
            internal bool IsCalibrationValuesDirty = false;

            double[] darkArray = null;
            int numberOfSpectrometers = 0;
            internal int spectrometerIndex = 0;
            double  numIntegrationTime = 0;
            internal int numBoxcarWidth = 0;
            internal int numScansToAverage = 0;

            OmniDriver.NETWrapper wrapper = new OmniDriver.NETWrapper();
            SPAM.NETXYZColor xyzColor = new SPAM.NETXYZColor();
            SPAM.NETSpectralMath spectralMath = new SPAM.NETSpectralMath();
            double[] spectrumArray = new double[1024];
            int pixel = 0;
            double[] wavelengths = new double[1024];
            int baseline = 0;
            float[] calwavelength = new float[1024];
            //was 660
            double[] calvalue = new double[1024];
            double[] calibrationvalue = new double[1024];
            int[] adjustedvalue = new int[1024];
            bool SpectrometerReadError = false;
            bool spectrometerfound = false;
            bool calfileloaded = false;
            int lineCount = 0;
            float calintegrationtime = 0;

            int spectromenterIndex = 0;
            int numberOfPixels = 0;
            internal double[] spectrum;
            //internal  double[] darkArray = null;
            double[] wavelengthArray;
            double[] darkSPectrun;
            internal double[] pCalibrationValues;
            public  int integrationtimeInMs = 1;
            double[] irradianceValues;
            double[] absoluteirradianceValues;
            int _mnumberOfSpectrometers = 0;
            double[] _mcalibrationdata = null;

            public  double collectionArea = 0.0;

            //SPAM.CCoAdvancedColor
            SPAM.CCoAdvancedAbsoluteIrradiance _madvancedAbsoluteIrradiance ;
            SPAM.CCoCIEObserver _mobserver = null;
            SPAM.CCoAdvancedPhotometrics _mphotometrics = null;
            SPAM.CCoIntegrationMethod method;
            SPAM.CCoCIEConstants _mcieConstants = null;
            SPAM.CCoIlluminant _milluminant = null;
            SPAM.CCoAdvancedColor _madvcolor = null;
            SPAM.CCoCIEColor _miecolor = null;
            SPAM.CCoSpectrumPeak _mspecpeak = null;
            SPAM.CCoCorrelatedColorTemperature _mcolortemp = null;
            
            double startingWavelength;
            double endingWavelength;
            double microWPerCm2;
            string strY;
            string strX;
            string strlittleX;
            string strlittleY;
            string strlittleZ;

            int _IntegrationTime { get; set; }
        #endregion

        #region constructor
        public TestFixtureReadSpectrometer()
            {
            omniwraper = new OmniDriver.CCoWrapper();//.CCoWrapper();//.NETWrapper();

            omnispecmath = new SPAM.CCoSpectralMath();//.CCoSpectralMath();
        }
        #endregion

        #region  open all spectrometer
        //this has to be first call
        //returns number of spectrometer found
        public int OpenAllSpectrometers()
        {
           
            _mnumberOfSpectrometers =  omniwraper.openAllSpectrometers();
            if (_mnumberOfSpectrometers < 1)
                return _mnumberOfSpectrometers;
            return _mnumberOfSpectrometers;
        } 
        #endregion

        #region Discovers the spectrometer
        //second call in the sequence
        public  void DiscoverSpetroMeter()
        {
            try
            {
               //assume only one spectrometer available 
                for (int i = 0; i < _mnumberOfSpectrometers; i++)
                {
                    spectromenterIndex = i;
                }
            }
            catch(Exception e)
            {
                string msg = e.Message;
            }
        }
        #endregion

        #region Acquire Sepctrum
        //third call in the sequence
      public string  SetAquisitionParameter()
        {
            string msg = "NoErrors";
            try
            {
                // Set some acquisition parameters and then acquire a spectrum
                integrationtimeInMs = integrationtimeInMs * 1200;
                omniwraper.setIntegrationTime(spectromenterIndex, integrationtimeInMs); // .5 seconds
                numberOfPixels = omniwraper.getNumberOfPixels(spectromenterIndex);
            }
            catch(Exception e)
            {
                msg = e.Message;
            }
            return msg;
        }
        #endregion

        #region aquire dark spectrum and call before turning LED on
        //fourth call in the sequence
        public string AcquireDarkSpectrum(ref double[] darkSpectrum)
        {
            string msg = "NoErrors";
            try
            {
                omniwraper.openAllSpectrometers();
                darkSPectrun = (double[])omniwraper.getSpectrum(spectromenterIndex);
                darkSpectrum = darkSPectrun;
            }
            catch(Exception e)
            {
                msg = e.Message;
            }

            return msg;
        }

        #endregion

        #region aquire sample spectrum
        //fifth call in the sequence
      public string AcquireSampleSpectrum()
        {
            string msg = "NoErrors";
            try
            {
                spectrum = (double[])omniwraper.getSpectrum(spectromenterIndex);
                wavelengthArray = (double[])omniwraper.getWavelengths(spectromenterIndex);
                numberOfPixels = spectrum.GetLength(0);
            }
            catch(Exception e)
            {
                msg = e.Message;
            }
            return msg;
        }
        #endregion

        #region Read calibrated file 
        //sixth call in the sequence
        public string ReadCalibratedFile()
        {
            string isFileExisting = "NoErrors";
            try
            {
                string calibratedFile = TestFixtureConstants.GetCalibratedFile();
                if (!string.IsNullOrEmpty(calibratedFile) && (File.Exists(calibratedFile)))
                {
                    ReadFileContaints(calibratedFile);
                }
                else
                {
                    isFileExisting = "Calibrated File doesn't exist...";
                }
            }
            catch (Exception exp)
            {
                string msg = exp.ToString();
                frmTestFixture.Instance.WriteToLog("ReadCalibratedFile: " + msg, ApplicationConstants.TraceLogType.Error);
            }
            return isFileExisting;
        }

        public string ReadCalibratedFile(string calibrationFile)
        {
            string isFileExisting = "NoErrors";

            try
            {
                if (!string.IsNullOrEmpty(calibrationFile) && (File.Exists(calibrationFile)))
                {
                    ReadFileContaints(calibrationFile);
                }
                else
                {
                    isFileExisting = "Calibrated File doesn't exist...";
                }
            }
            catch (Exception exp)
            {
                string msg = exp.ToString();
                frmTestFixture.Instance.WriteToLog("ReadCalibratedFile: " + msg, ApplicationConstants.TraceLogType.Error);
            }
            return isFileExisting;
        }

        private double [] ProcessSpectrumColorArray(double[] spectrumColorArray, double[] darkSpectrumColorArray)
        {
            double[] specArray = null;

            if (spectrumColorArray == null)
                return null;

            if (spectrumColorArray.Length != 1024)
                return null;

            if (darkSpectrumColorArray == null)
                return null;

            if (darkSpectrumColorArray.Length != 1024)
                return null;

            for (int i = 0; i < spectrumColorArray.Length; i++)
            {
               

            }

            return specArray;
        }


        internal bool GetSpectrum(Common.ApplicationConstants.SpectrumColors spectrumColor)
        {
            bool spectrumStatus = false;

            decimal MaxPercent = 0;
            double maxcount = 0;
            //darkArray = null;

            try
            {
                omniwraper.openAllSpectrometers();

                //Settings for Dark Reading
                numIntegrationTime = 500;
                numScansToAverage = 3;
                numBoxcarWidth = 3;

                //Set acquisition parameters
                omniwraper.setIntegrationTime(spectrometerIndex, (int)numIntegrationTime * 1000);
                omniwraper.setScansToAverage(spectrometerIndex, numScansToAverage);
                omniwraper.setBoxcarWidth(spectrometerIndex, numBoxcarWidth);

                if (darkArray == null)
                {
                    spectrumArray = omniwraper.getSpectrum(spectrometerIndex);
                    darkArray = spectrumArray;
                }

                //Get wavelengths
                wavelengthArray = (double[])omniwraper.getWavelengths(spectrometerIndex);

                maxcount = 0;
                MaxPercent = 0;

                //Settings for Red, Green, Blue, and White Reading
                //"_mIntegrationTime": "1000",
                //"_mBlueIntegrationTime": "1125",
                //"_mRedIntegrationTime": "500",
                //"_mGreenIntegrationTime": "2063",
                //"_mWhiteIntegrationTime": "8250",
                //"_mBlendedWhiteIntegrationTime": "8250",
                //if (spectrumColor == ApplicationConstants.SpectrumColors.Red)
                //    numIntegrationTime = 500;
                //else if(spectrumColor == ApplicationConstants.SpectrumColors.Green)
                //    numIntegrationTime = 2063;
                //else if (spectrumColor == ApplicationConstants.SpectrumColors.Blue)
                //    numIntegrationTime = 1125;
                //else if (spectrumColor == ApplicationConstants.SpectrumColors.White)
                //    numIntegrationTime = 8250;


                if (spectrumColor == ApplicationConstants.SpectrumColors.Red)
                    numIntegrationTime = Convert.ToDouble(frmTestFixture.Instance.txtRedIntegrationTime.Text);
                else if (spectrumColor == ApplicationConstants.SpectrumColors.Green)
                    numIntegrationTime = Convert.ToDouble(frmTestFixture.Instance.txtGreenIntegrationTime.Text);
                else if (spectrumColor == ApplicationConstants.SpectrumColors.Blue)
                    numIntegrationTime = Convert.ToDouble(frmTestFixture.Instance.txtBlueIntegrationTime.Text);
                else if (spectrumColor == ApplicationConstants.SpectrumColors.White)
                    numIntegrationTime = Convert.ToDouble(frmTestFixture.Instance.txtWhiteIntegrationTime.Text);
                else if (spectrumColor == ApplicationConstants.SpectrumColors.BlendedWhite)
                    numIntegrationTime = Convert.ToDouble(frmTestFixture.Instance.txtBlendedWhiteIntegrationTime.Text);
                else if (spectrumColor == ApplicationConstants.SpectrumColors.Magenda)
                    numIntegrationTime = Convert.ToDouble(frmTestFixture.Instance.txtMagendaIntegrationTime.Text);
                else
                    numIntegrationTime = Convert.ToDouble(frmTestFixture.Instance.txtIntegrationTime.Text);

                numScansToAverage = 1;
                numBoxcarWidth = 3;

                omniwraper.setIntegrationTime(spectrometerIndex, (int)numIntegrationTime * 1000);
                omniwraper.setScansToAverage(spectrometerIndex, numScansToAverage);
                omniwraper.setBoxcarWidth(spectrometerIndex, numBoxcarWidth);

                frmTestFixture.Instance.TurnSpectrumLedOn(spectrumColor);

                while (MaxPercent <= 80)
                {
                    frmTestFixture.Instance.WriteToLog("Int Time: " + numIntegrationTime.ToString(), ApplicationConstants.TraceLogType.Information);
                    omniwraper.setIntegrationTime(spectrometerIndex, (int)numIntegrationTime * 1000);
                    spectrumArray = omniwraper.getSpectrum(spectrometerIndex);

                    //Get a spectrum
                    for (pixel = 0; pixel <= omniwraper.getNumberOfPixels(spectrometerIndex) - 1; this.pixel++)
                    {
                        if (maxcount < Math.Round(spectrumArray[pixel], 2))
                            maxcount = Math.Round(spectrumArray[pixel], 2);
                    }

                    MaxPercent = Math.Round(Convert.ToDecimal(maxcount / 14500 * 100), 0);

                    if (MaxPercent <= 10)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime * 10, 0);
                    }
                    else if (MaxPercent <= 20)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime * 5, 0);
                    }
                    else if (MaxPercent <= 40)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime * 3, 0);
                    }
                    else if (MaxPercent <= 50)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime * 2, 0);
                    }
                    else if (MaxPercent <= 60)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime * 1.5, 0);
                    }
                    else if (MaxPercent <= 70)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime * 1.2, 0);
                    }
                    else if (MaxPercent <= 80)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime * 1.1, 0);
                    }
                    if (MaxPercent > 100)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime / 2, 0);
                        MaxPercent = MaxPercent / 2;
                    }

                    //Integration Time limiter
                    if (numIntegrationTime > 10000)
                    {
                        frmTestFixture.Instance.WriteToLog("STOPPED: INTEGRATION TIME EXCEDED 10 SECONDS.  IS THE LIGHT ON?", ApplicationConstants.TraceLogType.Error);
                        frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(Color.Red);
                        SpectrometerReadError = true;
                        spectrumStatus = false;
                        frmTestFixture.Instance.pagevm.CommandToTurnOffLeds();
                        return spectrumStatus;
                    }

                    frmTestFixture.Instance.WriteToLog(MaxPercent + "% of Max", ApplicationConstants.TraceLogType.Information);
                    frmTestFixture.Instance.WriteToLog("Maxcount: + " + maxcount, ApplicationConstants.TraceLogType.Information);
                    maxcount = 0;
                }

    

                if (MaxPercent > 80)
                {
                    numScansToAverage = 3;
                    numBoxcarWidth = 3;
                    omniwraper.setIntegrationTime(spectrometerIndex, (int)numIntegrationTime * 1000);
                    omniwraper.setScansToAverage(spectrometerIndex, numScansToAverage);
                    omniwraper.setBoxcarWidth(spectrometerIndex, numBoxcarWidth);
                    spectrumArray = omniwraper.getSpectrum(spectrometerIndex);

                    for (int x = 0; x <= 1023; x++)
                    {
                        spectrumArray[x] = (spectrumArray[x] - darkArray[x]) * pCalibrationValues[x] * (_IntegrationTime / numIntegrationTime);

                        if (spectrumArray[x] < 0)
                        {
                            spectrumArray[x] = 0;
                        }
                    }

                }

                if (ViewModel.TestFixtureViewModel.Instance != null)
                {
                    ViewModel.TestFixtureViewModel.Instance.CommandToTurnOffLeds();
                }

                spectrumStatus = true;

                SpectrometerReadError = false;

                ListViewItem lvi = null;
                frmTestFixture.Instance.lvSpectrumData.Items.Clear();

                //no errors in sub
                for (pixel = 0; pixel <= omniwraper.getNumberOfPixels(spectrometerIndex) - 1; pixel++)
                {
                    //lvi = new ListViewItem(pixel.ToString());
                    //lvi.SubItems.Add(darkArray[pixel].ToString());
                    //lvi.SubItems.Add(wavelengths[pixel].ToString());
                    //lvi.SubItems.Add(spectrumArray[pixel].ToString());
                    //lvi.SubItems.Add(pCalibrationValues[pixel].ToString());

                    //frmTestFixture.Instance.lvSpectrumData.Items.Add(lvi);
                }
            }
            catch (Exception ex)
            {
                frmTestFixture.Instance.WriteToLog("GetSpectrum: " + ex.Message, ApplicationConstants.TraceLogType.Error);
                spectrumStatus = false;
            }
            finally
            {
                frmTestFixture.Instance.pagevm.CommandToTurnOffLeds();
            }

            return spectrumStatus;
        }

        internal bool GetSpectrumMain(Common.ApplicationConstants.SpectrumColors spectrumColor)
        {
            bool spectrumStatus = false;

            decimal MaxPercent = 0;
            double maxcount = 0;
            //darkArray = null;

            try
            {
                omniwraper.openAllSpectrometers();

                //Settings for Dark Reading
                numIntegrationTime = 500;
                numScansToAverage = 3;
                numBoxcarWidth = 3;

                //Set acquisition parameters
                omniwraper.setIntegrationTime(spectrometerIndex, (int)numIntegrationTime * 1000);
                omniwraper.setScansToAverage(spectrometerIndex, numScansToAverage);
                omniwraper.setBoxcarWidth(spectrometerIndex, numBoxcarWidth);

                if (darkArray == null)
                {
                    spectrumArray = omniwraper.getSpectrum(spectrometerIndex);
                    darkArray = spectrumArray;
                }

                //Get wavelengths
                wavelengthArray = (double[])omniwraper.getWavelengths(spectrometerIndex);

                maxcount = 0;
                MaxPercent = 0;

                //Settings for Red, Green, Blue, and White Reading
                //"_mIntegrationTime": "1000",
                //"_mBlueIntegrationTime": "1125",
                //"_mRedIntegrationTime": "500",
                //"_mGreenIntegrationTime": "2063",
                //"_mWhiteIntegrationTime": "8250",
                //"_mBlendedWhiteIntegrationTime": "8250",
                //if (spectrumColor == ApplicationConstants.SpectrumColors.Red)
                //    numIntegrationTime = 500;
                //else if(spectrumColor == ApplicationConstants.SpectrumColors.Green)
                //    numIntegrationTime = 2063;
                //else if (spectrumColor == ApplicationConstants.SpectrumColors.Blue)
                //    numIntegrationTime = 1125;
                //else if (spectrumColor == ApplicationConstants.SpectrumColors.White)
                //    numIntegrationTime = 8250;


                if (spectrumColor == ApplicationConstants.SpectrumColors.Red)
                    numIntegrationTime = Convert.ToDouble(frmTestFixture.Instance.txtRedIntegrationTime.Text);
                else if (spectrumColor == ApplicationConstants.SpectrumColors.Green)
                    numIntegrationTime = Convert.ToDouble(frmTestFixture.Instance.txtGreenIntegrationTime.Text);
                else if (spectrumColor == ApplicationConstants.SpectrumColors.Blue)
                    numIntegrationTime = Convert.ToDouble(frmTestFixture.Instance.txtBlueIntegrationTime.Text);
                else if (spectrumColor == ApplicationConstants.SpectrumColors.White)
                    numIntegrationTime = Convert.ToDouble(frmTestFixture.Instance.txtWhiteIntegrationTime.Text);
                else if (spectrumColor == ApplicationConstants.SpectrumColors.BlendedWhite)
                    numIntegrationTime = Convert.ToDouble(frmTestFixture.Instance.txtBlendedWhiteIntegrationTime.Text);
                else if (spectrumColor == ApplicationConstants.SpectrumColors.Magenda)
                    numIntegrationTime = Convert.ToDouble(frmTestFixture.Instance.txtMagendaIntegrationTime.Text);
                else
                    numIntegrationTime = Convert.ToDouble(frmTestFixture.Instance.txtIntegrationTime.Text);

                numScansToAverage = 1;
                numBoxcarWidth = 3;

                omniwraper.setIntegrationTime(spectrometerIndex, (int)numIntegrationTime * 1000);
                omniwraper.setScansToAverage(spectrometerIndex, numScansToAverage);
                omniwraper.setBoxcarWidth(spectrometerIndex, numBoxcarWidth);

                frmTestFixture.Instance.TurnSpectrumLedOn(spectrumColor);

                while (MaxPercent <= 80)
                {
                    frmTestFixture.Instance.WriteToLog("Int Time: " + numIntegrationTime.ToString(), ApplicationConstants.TraceLogType.Information);
                    omniwraper.setIntegrationTime(spectrometerIndex, (int)numIntegrationTime * 1000);
                    spectrumArray = omniwraper.getSpectrum(spectrometerIndex);

                    //Get a spectrum
                    for (pixel = 0; pixel <= omniwraper.getNumberOfPixels(spectrometerIndex) - 1; this.pixel++)
                    {
                        if (maxcount < Math.Round(spectrumArray[pixel], 2))
                            maxcount = Math.Round(spectrumArray[pixel], 2);
                    }

                    MaxPercent = Math.Round(Convert.ToDecimal(maxcount / 14500 * 100), 0);

                    if (MaxPercent <= 10)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime * 10, 0);
                    }
                    else if (MaxPercent <= 20)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime * 5, 0);
                    }
                    else if (MaxPercent <= 40)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime * 3, 0);
                    }
                    else if (MaxPercent <= 50)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime * 2, 0);
                    }
                    else if (MaxPercent <= 60)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime * 1.5, 0);
                    }
                    else if (MaxPercent <= 70)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime * 1.2, 0);
                    }
                    else if (MaxPercent <= 80)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime * 1.1, 0);
                    }
                    if (MaxPercent > 100)
                    {
                        numIntegrationTime = Math.Round(numIntegrationTime / 2, 0);
                        MaxPercent = MaxPercent / 2;
                    }

                    //Integration Time limiter
                    if (numIntegrationTime > 10000)
                    {
                        frmTestFixture.Instance.WriteToLog("STOPPED: INTEGRATION TIME EXCEDED 10 SECONDS.  IS THE LIGHT ON?", ApplicationConstants.TraceLogType.Error);
                        frmTestFixture.Instance.SetStatusIndicatorTextBoxBackColor(Color.Red);
                        SpectrometerReadError = true;
                        spectrumStatus = false;
                        frmTestFixture.Instance.pagevm.CommandToTurnOffLeds();
                        return spectrumStatus;
                    }

                    frmTestFixture.Instance.WriteToLog(MaxPercent + "% of Max", ApplicationConstants.TraceLogType.Information);
                    frmTestFixture.Instance.WriteToLog("Maxcount: + " + maxcount, ApplicationConstants.TraceLogType.Information);
                    maxcount = 0;
                }

                if (MaxPercent > 80)
                {
                    numScansToAverage = 3;
                    numBoxcarWidth = 3;
                    omniwraper.setIntegrationTime(spectrometerIndex, (int)numIntegrationTime * 1000);
                    omniwraper.setScansToAverage(spectrometerIndex, numScansToAverage);
                    omniwraper.setBoxcarWidth(spectrometerIndex, numBoxcarWidth);
                    spectrumArray = omniwraper.getSpectrum(spectrometerIndex);

                    for (int x = 0; x <= 1023; x++)
                    {
                        spectrumArray[x] = (spectrumArray[x] - darkArray[x]) * pCalibrationValues[x] * (_IntegrationTime / numIntegrationTime);

                        if (spectrumArray[x] < 0)
                        {
                            spectrumArray[x] = 0;
                        }
                    }
                }

                if (ViewModel.TestFixtureViewModel.Instance != null)
                {
                    ViewModel.TestFixtureViewModel.Instance.CommandToTurnOffLeds();
                }

                spectrumStatus = true;

                SpectrometerReadError = false;

                //ListViewItem lvi = null;
                //frmTestFixture.Instance.lvSpectrumData.Items.Clear();

                ////no errors in sub
                //for (pixel = 0; pixel <= omniwraper.getNumberOfPixels(spectrometerIndex) - 1; pixel++)
                //{
                //    //lvi = new ListViewItem(pixel.ToString());
                //    //lvi.SubItems.Add(darkArray[pixel].ToString());
                //    //lvi.SubItems.Add(wavelengths[pixel].ToString());
                //    //lvi.SubItems.Add(spectrumArray[pixel].ToString());
                //    //lvi.SubItems.Add(pCalibrationValues[pixel].ToString());

                //    //frmTestFixture.Instance.lvSpectrumData.Items.Add(lvi);
                //}
            }
            catch (Exception ex)
            {
                frmTestFixture.Instance.WriteToLog("GetSpectrum: " + ex.Message, ApplicationConstants.TraceLogType.Error);
                spectrumStatus = false;
            }
            finally
            {
                frmTestFixture.Instance.pagevm.CommandToTurnOffLeds();
            }

            return spectrumStatus;
        }


        internal void CalculateColor(Common.ApplicationConstants.SpectrumColors spectrumColor)
        {
            SPAM.NETAdvancedColor advancedColor = default(SPAM.NETAdvancedColor);
            SPAM.NETCIEColor cieColor = default(SPAM.NETCIEColor);
            SPAM.NETXYZColor xyzColor = default(SPAM.NETXYZColor);

            SPAM.NETCIEObserver observer = new SPAM.NETCIEObserver();
            SPAM.NETCIEConstants constants = default(SPAM.NETCIEConstants);
            SPAM.NETIlluminant illuminant = new SPAM.NETIlluminant();

            try
            {
                double totalwatts = 0;
                double wavediff = 0;
                

                spectrometerIndex = 0;
                constants = spectralMath.createCIEConstantsObject();

                // Get combobox selection for observer
                //0 means 2° observer (photopic, daylight)
                //1 means 10° observer (scotopic, dark adjusted)
                int selectedIndexObserver = 0;
                selectedIndexObserver = 0;
                observer = constants.getCIEObserverByIndex(selectedIndexObserver);


                // Get combobox selection for illuminant
                //https://en.wikipedia.org/wiki/Standard_illuminant
                //0 means Illuminant A which is tungsten lighting
                //5 means Illuminant D65 which is daylight lighting
                int selectedIndexIlluminant = 4;
                //selectedIndexIlluminant = tb_illuminant.Text;

                
                illuminant = constants.getIlluminantByIndex(selectedIndexIlluminant);

                advancedColor = spectralMath.createAdvancedColorObject();

                //debug only
                //ListBox2.Items.Clear()
                //ListBox2.Items.Add("right before calculation")
                //MsgBox("wave: " & wavelengths.Length)
                //MsgBox("calibrationvalue: " & calibrationvalue.Length)
                //For x = 0 To wavelengths.Length - 1
                //    ListBox2.Items.Add(x & vbTab & Math.Round(wavelengths(x), 2) & vbTab & calibrationvalue(x))
                //Next

                ListViewItem lvi = null;

                frmTestFixture.Instance.lvSpectrumData.Items.Clear();

                //no errors in sub
                for (pixel = 0; pixel <= omniwraper.getNumberOfPixels(spectrometerIndex) - 1; pixel++)
                {
                    lvi = new ListViewItem(pixel.ToString());
                    lvi.SubItems.Add(darkArray[pixel].ToString());
                    lvi.SubItems.Add(wavelengthArray[pixel].ToString());
                    lvi.SubItems.Add(spectrumArray[pixel].ToString());
                    lvi.SubItems.Add(pCalibrationValues[pixel].ToString());

                    frmTestFixture.Instance.lvSpectrumData.Items.Add(lvi);
                }

                cieColor = advancedColor.computeReflectiveChromaticity(wavelengthArray, spectrumArray, observer, illuminant);

                //Always call Dispose() when we are finished using an object to prevent a memory leak
                advancedColor.Dispose();

                // L* a* b*
                //cieLab = spectralMath.createCIELABObject(cieColor)
                // lstColor.Items.Add("L* = " & Format(Math.Round(cieLab.getLStar(), 2), "#0.00"))
                //lstColor.Items.Add("a* = " & Format(Math.Round(cieLab.get_aStar(), 2), "#0.00"))
                // lstColor.Items.Add("b* = " & Format(Math.Round(cieLab.get_bStar(), 2), "#0.00"))

                //lstColor.Items.Add("")

                // XYZ
                xyzColor = spectralMath.createXYZColorObject(cieColor);
                //lstColor.Items.Add("X = " & Format(Math.Round(xyzColor.getX(), 5), "#0.0000"))
                // lstColor.Items.Add("Y = " & Format(Math.Round(xyzColor.getY(), 5), "#0.0000"))
                // lstColor.Items.Add("Z = " & Format(Math.Round(xyzColor.getZ(), 5), "#0.0000"))

                //lstColor.Items.Add(observer)

                frmTestFixture.Instance.pagevm.SetUpperXValue = Math.Round(xyzColor.getLittleX(), 3).ToString();
                frmTestFixture.Instance.pagevm.SetUpperYvalue = Math.Round(xyzColor.getLittleY(), 3).ToString();

                frmTestFixture.Instance.txtSetUpperXValue.Text = frmTestFixture.Instance.pagevm.SetUpperXValue.ToString();
                frmTestFixture.Instance.txtSetUpperYValue.Text = frmTestFixture.Instance.pagevm.SetUpperYvalue.ToString();

                //Always call Dispose() when we are finished using an object to prevent a memory leak
                //cieLab.Dispose()
                xyzColor.Dispose();

                //integrate spectrum to total watts
                totalwatts = 0;

                //cannot integrate the last value
                for (int x = 0; x <= wavelengthArray.Length - 2; x++)
                {
                    wavediff = (wavelengthArray[x + 1] - wavelengthArray[x]);
                    totalwatts = totalwatts + (wavediff * spectrumArray[x]);
                    // lst_Calibrated_Spectrum.Items.Add(Math.Round(wavelengths(x), 2) & vbTab & vbTab & calibrationvalue(x) & vbTab & vbTab & Math.Round(totalwatts, 2))
                }

                // totalwatts = totalwatts + (wavediff * spectrumArray[spectrumArray.Length - 1]);

                //Temperature sensor correction value. 
                //Get temp sensor from greg when temp sensor is available
                //totalwatts = totalwatts * tempSensorCorrection;

                frmTestFixture.Instance.pagevm.SetWattValue = totalwatts;

                frmTestFixture.Instance.pagevm.SpectrumValidation(spectrumColor);

                // last value added
                frmTestFixture.Instance.txtSetWattValue.Text = totalwatts.ToString();

                frmTestFixture.Instance.WriteToLog("Watts: " + totalwatts, ApplicationConstants.TraceLogType.Information);
            }
            catch (Exception ex)
            {
                frmTestFixture.Instance.WriteToLog("CalculateColor: " + ex.Message, ApplicationConstants.TraceLogType.Error);
            }
        }

        internal bool CalculateColorMain(Common.ApplicationConstants.SpectrumColors spectrumColor)
        {
            SPAM.NETAdvancedColor advancedColor = default(SPAM.NETAdvancedColor);
            SPAM.NETCIEColor cieColor = default(SPAM.NETCIEColor);
            SPAM.NETXYZColor xyzColor = default(SPAM.NETXYZColor);

            SPAM.NETCIEObserver observer = new SPAM.NETCIEObserver();
            SPAM.NETCIEConstants constants = default(SPAM.NETCIEConstants);
            SPAM.NETIlluminant illuminant = new SPAM.NETIlluminant();

            try
            {
                double totalwatts = 0;
                double wavediff = 0;

                spectrometerIndex = 0;
                constants = spectralMath.createCIEConstantsObject();

                // Get combobox selection for observer
                //0 means 2° observer (photopic, daylight)
                //1 means 10° observer (scotopic, dark adjusted)
                int selectedIndexObserver = 0;
                selectedIndexObserver = 0;
                observer = constants.getCIEObserverByIndex(selectedIndexObserver);


                // Get combobox selection for illuminant
                //https://en.wikipedia.org/wiki/Standard_illuminant
                //0 means Illuminant A which is tungsten lighting
                //5 means Illuminant D65 which is daylight lighting
                int selectedIndexIlluminant = 4;
                //selectedIndexIlluminant = tb_illuminant.Text;


                illuminant = constants.getIlluminantByIndex(selectedIndexIlluminant);

                advancedColor = spectralMath.createAdvancedColorObject();

                //ListViewItem lvi = null;

                //frmTestFixture.Instance.lvSpectrumData.Items.Clear();

                //no errors in sub
                //for (pixel = 0; pixel <= omniwraper.getNumberOfPixels(spectrometerIndex) - 1; pixel++)
                //{
                //    lvi = new ListViewItem(pixel.ToString());
                //    lvi.SubItems.Add(darkArray[pixel].ToString());
                //    lvi.SubItems.Add(wavelengthArray[pixel].ToString());
                //    lvi.SubItems.Add(spectrumArray[pixel].ToString());
                //    lvi.SubItems.Add(pCalibrationValues[pixel].ToString());

                //    frmTestFixture.Instance.lvSpectrumData.Items.Add(lvi);
                //}

                cieColor = advancedColor.computeReflectiveChromaticity(wavelengthArray, spectrumArray, observer, illuminant);

                //Always call Dispose() when we are finished using an object to prevent a memory leak
                advancedColor.Dispose();

                // XYZ
                xyzColor = spectralMath.createXYZColorObject(cieColor);

                frmTestFixture.Instance.pagevm.SetUpperXValue = Math.Round(xyzColor.getLittleX(), 3).ToString();
                frmTestFixture.Instance.pagevm.SetUpperYvalue = Math.Round(xyzColor.getLittleY(), 3).ToString();

                //frmTestFixture.Instance.txtSetUpperXValue.Text = frmTestFixture.Instance.pagevm.SetUpperXValue.ToString();
                //frmTestFixture.Instance.txtSetUpperYValue.Text = frmTestFixture.Instance.pagevm.SetUpperYvalue.ToString();

                //Always call Dispose() when we are finished using an object to prevent a memory leak
                //cieLab.Dispose()
                xyzColor.Dispose();

                //integrate spectrum to total watts
                totalwatts = 0;

                //cannot integrate the last value
                for (int x = 0; x <= wavelengthArray.Length - 2; x++)
                {
                    wavediff = (wavelengthArray[x + 1] - wavelengthArray[x]);
                    totalwatts = totalwatts + (wavediff * spectrumArray[x]);
                    // lst_Calibrated_Spectrum.Items.Add(Math.Round(wavelengths(x), 2) & vbTab & vbTab & calibrationvalue(x) & vbTab & vbTab & Math.Round(totalwatts, 2))
                }

                // totalwatts = totalwatts + (wavediff * spectrumArray[spectrumArray.Length - 1]);

                //Temperature sensor correction value. 
                //Get temp sensor from greg when temp sensor is available
                //totalwatts = totalwatts * tempSensorCorrection;

                frmTestFixture.Instance.pagevm.SetWattValue = totalwatts;

                if(spectrumColor == ApplicationConstants.SpectrumColors.Red)
                {
                    LogStructureNew.RedX = string.Format("{0}", frmTestFixture.Instance.pagevm.SetUpperXValue);
                    LogStructureNew.RedY = string.Format("{0}", frmTestFixture.Instance.pagevm.SetUpperYvalue);
                    LogStructureNew.RedWatts = totalwatts.ToString();
                }
                else if (spectrumColor == ApplicationConstants.SpectrumColors.Green)
                {
                    LogStructureNew.GreenX = string.Format("{0}", frmTestFixture.Instance.pagevm.SetUpperXValue);
                    LogStructureNew.GreenY = string.Format("{0}", frmTestFixture.Instance.pagevm.SetUpperYvalue);
                    LogStructureNew.GreenWatts = totalwatts.ToString();
                }
                else if(spectrumColor == ApplicationConstants.SpectrumColors.Blue)
                {
                    LogStructureNew.BlueX = string.Format("{0}", frmTestFixture.Instance.pagevm.SetUpperXValue);
                    LogStructureNew.BlueY = string.Format("{0}", frmTestFixture.Instance.pagevm.SetUpperYvalue);
                    LogStructureNew.BlueWatts = totalwatts.ToString();
                }
                else if(spectrumColor == ApplicationConstants.SpectrumColors.White)
                {
                    LogStructureNew.WhiteX = string.Format("{0}", frmTestFixture.Instance.pagevm.SetUpperXValue);
                    LogStructureNew.WhiteY = string.Format("{0}", frmTestFixture.Instance.pagevm.SetUpperYvalue);
                    LogStructureNew.WhiteWatts = totalwatts.ToString();
                }
                else if(spectrumColor == ApplicationConstants.SpectrumColors.BlendedWhite)
                {
                    LogStructureNew.BlendedWhiteX = string.Format("{0}", frmTestFixture.Instance.pagevm.SetUpperXValue);
                    LogStructureNew.BlendedWhiteY = string.Format("{0}", frmTestFixture.Instance.pagevm.SetUpperYvalue);
                    LogStructureNew.BlendedWhiteWatts = totalwatts.ToString();
                }
                else if(spectrumColor == ApplicationConstants.SpectrumColors.Magenda)
                {
                    LogStructureNew.MagentaX = string.Format("{0}", frmTestFixture.Instance.pagevm.SetUpperXValue);
                    LogStructureNew.MagentaY = string.Format("{0}", frmTestFixture.Instance.pagevm.SetUpperYvalue);
                    LogStructureNew.MagentaWatts = totalwatts.ToString();
                }

                bool flag = frmTestFixture.Instance.pagevm.SpectrumValidation(spectrumColor);

                // last value added
                //frmTestFixture.Instance.txtSetWattValue.Text = totalwatts.ToString();

                frmTestFixture.Instance.WriteToLog("Watts: " + totalwatts, ApplicationConstants.TraceLogType.Information);

                return flag;
            }
            catch (Exception ex)
            {
                frmTestFixture.Instance.WriteToLog("CalculateColor: " + ex.Message, ApplicationConstants.TraceLogType.Error);
                return false;
            }
        }

        public string ReadFileContaints(string aFile)
        {
            string msg = "NoErrors";
            int counter = 0;
            string col = null;
            string line;

            if (!IsCalibrationValuesDirty)
            {
                frmTestFixture.Instance.WriteToLog("Reading calibration file...", ApplicationConstants.TraceLogType.Information);

                try
                {
                    StreamReader reader = new StreamReader(aFile);
                    pCalibrationValues = new double[1024];

                    int integrationTime = -1;

                    while ((line = reader.ReadLine()) != null)
                    {
                        //Capturing Integration Time
                        if (integrationTime == -1)
                        {
                            if (Int32.TryParse(line.Split(',')[1], out integrationTime))
                                _IntegrationTime = integrationTime;
                        }

                        if (counter <= 1)
                        {
                            counter++;
                            continue;
                        }
                        col = line.Split(',')[1];
                        pCalibrationValues[counter - 2] = Double.Parse(col);
                        counter++;
                    }

                    reader.Close();
                    reader.Dispose();

                    IsCalibrationValuesDirty = true;

                    frmTestFixture.Instance.WriteToLog("Read calibration file operation completed...", ApplicationConstants.TraceLogType.Information);
                }
                catch (Exception exp)
                {
                    IsCalibrationValuesDirty = false;

                    msg = exp.ToString();

                    frmTestFixture.Instance.WriteToLog("ReadFileContaints: " + msg, ApplicationConstants.TraceLogType.Error);
                }
            }
            else
            {
                msg = "NoErrors";

                frmTestFixture.Instance.WriteToLog("Read calibration file array has data...", ApplicationConstants.TraceLogType.Information);
            }

            return msg;
        }
        #endregion

        #region calibration and copied from examples... later can be changed or callibration values can be read from file
        //void CreateDummyCalibrationData()
        //{
        //    double[][] pCallibrationArray;
        //    pCalibrationValues = new double[numberOfPixels];
        //    for (int i = 0; i < numberOfPixels; i++)
        //    {
        //          pCalibrationValues[i] = 3.10228421E-5;
        //        //pCalibrationValues[i] = 0;
        //    }
        //   // pCallibrationArray = new double[pCalibrationValues][numberOfPixels];
        //}
        #endregion 

        #region returns dominant wavelength
        double GetDominantWaveLength()
        {
            SPAM.CCoDominantWavelengthPurity dominantWavelenght = null;
            double dwavelength = 0;
            try
            {
               dominantWavelenght = omnispecmath.createDominantWavelengthPurityObjectd(_miecolor);
              
                dwavelength = dominantWavelenght.getDominantWavelength();

                Console.WriteLine("Dominant Wavelength:" + dwavelength.ToString());
            }
            catch(Exception e)
            {
                string msg = e.Message;
            }
            dominantWavelenght.Dispose();//check this might create some problem ..test it properly

            return dwavelength;
        }
        #endregion

        #region returns center wavelength
        double GetCenterWaveLength()
        {
            double centwave = 0.0;
            try
            {
                centwave = _mspecpeak.getCenterWavelength();
                Console.WriteLine("Center Wavelength:" + centwave.ToString());
            }
            catch(Exception e)
            {
                string msg = e.Message;
            }
            return centwave;
        }
        #endregion

        #region(returns pixel full width half maximum)
        double GetFixelFullWidthAtHalfMax()
        {
            double fwidthhalfmax = 0.0;
            try
            {
                fwidthhalfmax = _mspecpeak.getPixelFullWidthAtHalfMaximum();
                //Console.WriteLine("Full Width Half Max bandwidth:" + fwidthhalfmax.ToString());
            }
            catch (Exception e)
            {
                string msg = e.Message;
                frmTestFixture.Instance.WriteToLog("GetFixelFullWidthAtHalfMax: " + msg, ApplicationConstants.TraceLogType.Error);
            }
           return fwidthhalfmax;
        }
        #endregion        

        #region(returns centroid wavelength)
        double GetCentroidWavelength()
        {
            double centroidwave = 0.0;
            try
            {
                centroidwave = _mspecpeak.getCentroid();
                //Console.WriteLine("Centroid Wavelength:" + centroidwave.ToString());
            }
            catch (Exception e)
            {
                string msg = e.Message;
                frmTestFixture.Instance.WriteToLog("GetCentroidWavelength: " + msg, ApplicationConstants.TraceLogType.Error);
            }
            return centroidwave;
        }
        #endregion

        #region(returns peak wavelength)
        double[] GetPeakWavelengths()
        {
            double[] array = { 0, 0 };
            SPAM.CCoAdvancedPeakFinding peakfind = null;
            try
            {
                peakfind = omnispecmath.createAdvancedPeakFindingObject();
                double[] extracted_wavelengths = _miecolor.getWavelengths();

                array = peakfind.getPeakWavelengths(extracted_wavelengths, spectrum, 100, 100);

                for (int index = 0; index < array.Length; index++)
                {
                    Console.WriteLine("Wavelength: {0}   Intensity: {1}", array[index], array[index]);
                    //Display the raw pixel values of this spectrum
                }
            }
            catch (Exception e)
            {
                string msg = e.Message;
                frmTestFixture.Instance.WriteToLog("GetPeakWavelengths: " + msg, ApplicationConstants.TraceLogType.Error);
            }
            peakfind.Dispose();  //check does it create problem in 
            return array;
        }
        #endregion

        #region(Energy spectrum computation)

        public void ComputeEngerySpectrum()
        {
            try
            {
                double fiberDiameter = 1;//get from FirstView Fiber diameter
                collectionArea = Math.Pow((fiberDiameter / 20000), 2) * Math.PI;

                _madvancedAbsoluteIrradiance = omnispecmath.createAdvancedAbsoluteIrradianceObject();
               
                irradianceValues = _madvancedAbsoluteIrradiance.processSpectrum(darkSPectrun, spectrum, wavelengthArray, pCalibrationValues, integrationtimeInMs, 1, 1);

                int numberOfObservers;

                _mcieConstants = omnispecmath.createCIEConstantsObject();

                numberOfObservers = _mcieConstants.getNumberOfObservers(); // number of available observers

                _mobserver = _mcieConstants.getCIEObserverByIndex(0);

                _milluminant = _mcieConstants.getIlluminantByIndex(0);

                _mphotometrics = omnispecmath.createAdvancedPhotometricsObject();

                _madvcolor = omnispecmath.createAdvancedColorObject();
                
                double[] V_wavelengths = _mobserver.getWavelengths();
                double[] V = _mobserver.getV();
                double K_m = _mobserver.getKm();

                // Calculate energy spectrum in Watts/nm
                double[] energyWattsPerNm = new double[irradianceValues.Count()];
                string[] strEnergyWattsPerNm = new string[irradianceValues.Count()];

                for (int i = 0; i < irradianceValues.Count(); i++)
                {
                    energyWattsPerNm[i] = irradianceValues[i] * collectionArea / 1000000;
                    strEnergyWattsPerNm[i] = energyWattsPerNm[i].ToString("0.00000000e0");
                }

                // Compute Luminous Flux in Lumen (Does not use Steradians)
                double lumen = _mphotometrics.computeLuminousFluxLumen(wavelengthArray, energyWattsPerNm, V_wavelengths, V, K_m);
                double lumenRounded = Math.Round(lumen, 10);

                // Compute Luminous Intensity in Candela (Does use Steradians)                       
                //double candela = _mphotometrics.computeLuminousIntensityCandela(wavelengthArray, energyWattsPerNm, V_wavelengths, V, K_m, steradians);
                //double candelaRounded = Math.Round(candela, 10);

                // Use computeEmissiveChromaticity() to get CIEColor object
                _miecolor = _madvcolor.computeEmissiveChromaticity(wavelengthArray, irradianceValues, _mobserver, _milluminant);

                // _mcolortemp = omnispecmath.createCorrelatedColorTemperatureObject(_miecolor);
                // double data = _mcolortemp.computeCorrelatedColorTemperature(_miecolor);
                //double getColourTemp = _mcolortemp.getCorrelatedColorTemperature();

                double[] colorWavelen = _miecolor.getWavelengths();
                double[] colorEnergySpec = _miecolor.getEnergySpectrum();

               // double[] data = _miecolor.

                strlittleX = _miecolor.getLittleX().ToString("0.##");
                strlittleY = _miecolor.getLittleY().ToString("0.##");
                strlittleZ = _miecolor.getLittleZ().ToString("0.##");
                                
                strY = _miecolor.getY().ToString("0.##");
                strX = _miecolor.getX().ToString("0.##");
                string strZ = _miecolor.getZ().ToString("0.##");

                double chromoCoEfficientX = _miecolor.getLittleX() / (_miecolor.getLittleX()+ _miecolor.getLittleY()+ _miecolor.getLittleZ());
                double chromoCoEfficientY = _miecolor.getLittleY() / (_miecolor.getLittleX() + _miecolor.getLittleY() + _miecolor.getLittleZ());

                strX = chromoCoEfficientX.ToString("0.##");
                strY = chromoCoEfficientY.ToString("0.##");

                double areaSquareMeters = collectionArea / 10000;
                double lux = Math.Round(_mphotometrics.computeIlluminanceLux(wavelengthArray, energyWattsPerNm, V_wavelengths, V, K_m, areaSquareMeters), 2);

               decimal numStartingWavelength = Math.Round((Convert.ToDecimal(omniwraper.getWavelength(spectromenterIndex, 0))), 2);
               decimal numEndingWavelength = Math.Round(Convert.ToDecimal(omniwraper.getWavelength(spectromenterIndex, omniwraper.getNumberOfPixels(spectromenterIndex) - 1)), 2);

                // Compute µW/cm2
                 method = omnispecmath.createIntegrationMethodObject(1);

                startingWavelength = Convert.ToDouble(numStartingWavelength);
                endingWavelength = Convert.ToDouble(numEndingWavelength);
                microWPerCm2 = _mphotometrics.compute_uWattPerCmSquared(wavelengthArray, irradianceValues, startingWavelength, endingWavelength, method);
                
            }
            catch (Exception e)
            {
                string msg = e.Message;
                frmTestFixture.Instance.WriteToLog("ComputeEngerySpectrum: " + msg, ApplicationConstants.TraceLogType.Error);
            }

        }
        #endregion

        #region  get X and Y value
        public string getUpperXValue()
        {
            string xvalue = strX;
            return xvalue;
        }
        public string getUpperYValue()
        {
            string yvalue = strY;
            return yvalue;

        }
        public string getlowerXValue()
        {
           string lowerxvalue = strlittleX;
            return lowerxvalue;
        }

        public string getloweryValue()
        {
            string loweryvalue = strlittleY;
            return loweryvalue;
        }
        public double getWattvalue()
        {
            return microWPerCm2;
        }
        #endregion

        #region dispose all created data or memory objects
        void CalculateColorTemperature()
        {
            double color_temp = _mcolortemp.getCorrelatedColorTemperature();
        }
        public void CloseSpectrometer()
        {
            //omniwraper.closeSpectrometer(spectromenterIndex);
            _mcieConstants = null;
            _milluminant = null;
            _madvcolor = null;
            _miecolor = null;
            _mspecpeak = null;
            _mcolortemp = null;
            spectromenterIndex = -1;
            numberOfPixels = 0;
            integrationtimeInMs = 1;
        }
        #endregion

        #region AdvancedIrradianceCalibration

        SPAM.CCoAdvancedIrradianceCalibration _madvancedCalibration = null;

        public void ComputeCalibration()
        {
            double fiberDiameter = 0.04;//get from FirstView Fiber diameter
            collectionArea = Math.Pow((fiberDiameter / 20000), 2) * Math.PI;
            _madvancedCalibration =  omnispecmath.createAdvancedIrradianceCalibrationObject();

            GetCalibrationFile();
            absoluteirradianceValues = _madvancedCalibration.processSpectrum(darkSPectrun, spectrum, wavelengthArray, _mcalibrationdata, integrationtimeInMs, collectionArea, 1);

            _mobserver = _mcieConstants.getCIEObserverByIndex(0);

            _milluminant = _mcieConstants.getIlluminantByIndex(0);

           double[] V_wavelengths = _mobserver.getWavelengths();
           double[] V = _mobserver.getV();
           double K_m = _mobserver.getKm();

            // Calculate energy spectrum in Watts/nm
           double[] energyWattsPerNm = new double[absoluteirradianceValues.Count()];
           string[] strEnergyWattsPerNm = new string[absoluteirradianceValues.Count()];

            for (int i = 0; i < absoluteirradianceValues.Count(); i++)
            {
                energyWattsPerNm[i] = absoluteirradianceValues[i] * collectionArea / 1000000;
                strEnergyWattsPerNm[i] = energyWattsPerNm[i].ToString("0.00000000e0");
            }

            // Compute Luminous Flux in Lumen (Does not use Steradians)
           double lumen = _mphotometrics.computeLuminousFluxLumen(wavelengthArray, energyWattsPerNm, V_wavelengths, V, K_m);
            double lumenRounded = Math.Round(lumen, 10);

            // Compute Luminous Intensity in Candela (Does use Steradians)                       
            //double candela = _mphotometrics.computeLuminousIntensityCandela(wavelengthArray, energyWattsPerNm, V_wavelengths, V, K_m, steradians);
            //double candelaRounded = Math.Round(candela, 10);

            // Use computeEmissiveChromaticity() to get CIEColor object
            _miecolor = _madvcolor.computeEmissiveChromaticity(wavelengthArray, absoluteirradianceValues, _mobserver, _milluminant);



            strlittleX = _miecolor.getLittleX().ToString("0.##");
            strlittleY = _miecolor.getLittleY().ToString("0.##");
            strlittleZ = _miecolor.getLittleZ().ToString("0.##");
            strY = _miecolor.getY().ToString("0.##");
            strX = _miecolor.getX().ToString("0.##");

            double areaSquareMeters = collectionArea / 10000;
           double lux = Math.Round(_mphotometrics.computeIlluminanceLux(wavelengthArray, energyWattsPerNm, V_wavelengths, V, K_m, areaSquareMeters), 2);

    
        }

        #endregion

        public void GetCalibrationFile()
        {
            string calibratedFile = TestFixtureConstants.GetCalibratedFile();
            string[] filecontent = System.IO.File.ReadAllLines(calibratedFile);
            double[] arrDouble = new double[filecontent.Length];
            for (int i = 0; i < filecontent.Length; i++)
            {
                if (i <= 43)
                    continue;
                arrDouble[i] = Convert.ToDouble(filecontent[i]);
            }
            _mcalibrationdata = arrDouble;
        }
    }
}
  