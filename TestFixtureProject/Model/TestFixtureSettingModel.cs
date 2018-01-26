using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using TestFixtureProject.Common;
using TestFixtureProject.ViewModel;

namespace TestFixtureProject.Model
{
    public class TestFixtureSettingModel : TestFixtureLoginBaseVM
    {
        #region private variable for white limits RGB Section

        private bool _mcollapsesettingstabitem;

        #endregion

        #region constructor
        public TestFixtureSettingModel(bool canAccessallPages)
        {
            _mcollapsesettingstabitem = canAccessallPages;
            LoadSettingDetailsFromFile();
        }
        #endregion

        #region flag to enable or disable save button

        private bool _canSaveData = true;

        public bool CanSaveData()
        {
            return _canSaveData;
        }

        #endregion

        #region Image loading
        private ImageSource _imagesource;
        public ImageSource ImageSource
        {
            get
            {
                return _imagesource;
            }
            set
            {
                _imagesource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        #endregion

        #region public methods to set/get property for White Limits RGB section

        private string _wlrxminlvalue;  //White Limits RGB Red Lower Values
        private string _wlrxmaxvalues; // white limits RGB Red Upper Values
        private string _wlryminvalues; //White Limits RGB Green Lower Value
        private string _wlrymaxvalues; //White Limits RGB Green Upper Value

        [JsonProperty("_wlrxminlvalue")]
        [JsonIgnore]
        public string WLRGBXMINVALUE    //White Limits RGB Red Lower Value
        {
            get { return _wlrxminlvalue; }
            set
            {
                _wlrxminlvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("WLRGBXMINVALUE");
            }
        }

        [JsonProperty("_wlrxmaxvalues")]
        [JsonIgnore]
        public string WLRGBXMAXVALUE  // white limits RGB Red Upper Values
        {
            get { return _wlrxmaxvalues; }
            set
            {
                _wlrxmaxvalues = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("WLRGBXMAXVALUE");
            }
        }

        ////wlrgbgreenlowertextbox
        [JsonProperty("_wlryminvalues")]
        [JsonIgnore]
        public string WLRYMINVALUE  //White Limits RGB Green Lower Value
        {
            get { return _wlryminvalues; }
            set
            {
                _wlryminvalues = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("WLRYMAXVALUE");
            }
        }

        [JsonProperty("_wlrgbgreenuvalues")]
        [JsonIgnore]
        public string WLRYMAXVALUE       //White Limits RGB Green Upper Value
        {
            get { return _wlrymaxvalues; }
            set
            {
                _wlrymaxvalues = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("WLRYMAXVALUE");
            }
        }
        private string _wlrgbfluxUvalues = null;

        [JsonProperty("_wlrgbfluxUvalues")]
        [JsonIgnore]
        public string WLRGBBLUELValue //white limits RGB Blue Lower Values
        {
            get { return _wlrgbfluxUvalues; }
            set
            {
                _wlrgbfluxUvalues = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("WLRGBBLUELValue");
            }
        }

        private string _mWhiteMicroWattValue = null;
        [JsonProperty("_mWhiteMicroWattValue")]
        [JsonIgnore]
        public string WhiteMicroWattValue
        {
            get { return _mWhiteMicroWattValue; }
            set
            {
                _mWhiteMicroWattValue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("WhiteMicroWattValue");
            }
        }

        private string _mWhiteMicroWattValueUpper = null;
        [JsonProperty("_mWhiteMicroWattValueUpper")]
        [JsonIgnore]
        public string WhiteMicroWattValueUpper
        {
            get { return _mWhiteMicroWattValueUpper; }
            set
            {
                _mWhiteMicroWattValueUpper = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("WhiteMicroWattValueUpper");
            }
        }
        #endregion

        #region public methods to set/get property for Green Limits RGB Section

        private string _glrgbgreenlvalue; // Green Limits RGB Green Lower Value
        private string _glrgbgreenuvalue; // Green Limits RGB Green Upper Value

        [JsonProperty("_glrgbgreenlvalue")]
        [JsonIgnore]
        public string GLRGBGREENLValue  // Green Limits RGB Green Lower Value
        {
            get { return _glrgbgreenlvalue; }

            set
            {
                _glrgbgreenlvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("GLRGBGREENLValue");
            }
        }

        [JsonProperty("_glrgbgreenuvalue")]
        [JsonIgnore]
        public string GLRGBGREENUValue  // Green Limits RGB Green Upper Value
        {
            get { return _glrgbgreenuvalue; }

            set
            {
                _glrgbgreenuvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("GLRGBGREENUValue");
            }
        }
        //GreenYMinValues
        private string _greenyminvalue;

        [JsonProperty("_greenyminvalue")]
        [JsonIgnore]
        public string GreenYMinValues  // Green Limits RGB Green Upper Value
        {
            get { return _greenyminvalue; }

            set
            {
                _greenyminvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("GreenYMinValues");
            }
        }
        //GreenYMaxValue
        private string _greenymaxvalue;

        [JsonProperty("_greenymaxvalue")]
        [JsonIgnore]
        public string GreenYMaxValue  // Green Limits RGB Green Upper Value
        {
            get { return _greenymaxvalue; }

            set
            {
                _greenymaxvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("GreenYMaxValue");
            }
        }

        private string _mGreenMicroWatValue = null;
        [JsonProperty("_mGreenMicroWatValue")]
        [JsonIgnore]
        public string GreenMicroWattValue
        {
            get { return _mGreenMicroWatValue; }

            set
            {
                _mGreenMicroWatValue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("GreenMicroWattValue");
            }
        }


        private string _mGreenMicroWatValueUpper = null;
        [JsonProperty("_mGreenMicroWatValueUpper")]
        [JsonIgnore]
        public string GreenMicroWattValueUpper
        {
            get { return _mGreenMicroWatValueUpper; }

            set
            {
                _mGreenMicroWatValueUpper = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("GreenMicroWattValueUpper");
            }
        }
        #endregion

        #region public methods to set/get property for Blended White section

        private string _mblendedredlvalue; //Magenta Limits Red Lower Value
        private string _mblendedreduvalue; //Magenta Limits  Red Upper Value
        private string _mblendedgreenlvalue; //Magenta Limits  Green Lower Value
        private string _mblendedgreenuvalue; //Magenta Limits  Green Upper Value
        private string _mblendedbluelvalue; //Magenta Limits  Blue Lower Value
        private string _mblendedblueuvalue; //Magenta Limits  Blue Upper Value

        //Blended White Limits Red Lower Value
        [JsonProperty("_mblendedredlvalue")]
        [JsonIgnore]
        public string BlendedWhiteRedLimits
        {
            get { return _mblendedredlvalue; }
            set
            {
                _mblendedredlvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("BlendedWhiteRedLimits");
            }
        }

        //Blended White Red Upper Value
        [JsonProperty("_mblendedreduvalue")]
        [JsonIgnore]
        public string BlendedWhiteRedULimits
        {
            get { return _mblendedreduvalue; }
            set
            {
                _mblendedreduvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("BlendedWhiteRedULimits");
            }
        }

        //Blended White Limits Green Lower Value
        [JsonProperty("_mblendedgreenlvalue")]
        [JsonIgnore]
        public string BlendedWhiteGreenLLimits
        {
            get { return _mblendedgreenlvalue; }
            set
            {
                _mblendedgreenlvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("BlendedWhiteGreenLLimits");
            }
        }

        //Blended White Limits Green Upper Value
        [JsonProperty("_mblendedgreenuvalue")]
        [JsonIgnore]
        public string BlendedWhiteGreenULimits
        {
            get { return _mblendedgreenuvalue; }
            set
            {
                _mblendedgreenuvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("BlendedWhiteGreenULimits");
            }
        }

        //Blended White Limits Blue Lower Value
        [JsonProperty("_mblendedbluelvalue")]
        [JsonIgnore]
        public string BlendedWhiteBlueLLimits
        {
            get { return _mblendedbluelvalue; }
            set
            {
                _mblendedbluelvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("BlendedWhiteBlueLLimits");
            }
        }

        //Blended White Limits Blue Upper Value
        [JsonProperty("_mblendedblueuvalue")]
        [JsonIgnore]
        public string BlendedWhiteBlueULimits
        {
            get { return _mblendedblueuvalue; }
            set
            {
                _mblendedblueuvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("BlendedWhiteBlueULimits");
            }
        }
        #endregion

        #region public methods for set/get property for Blue limits RGB Section

        private string _blrgbbluelvalue; //Blue Limits RGB Blue Lower Value
        private string _bllrgbblueuvalue; //Blue Limits RGB Blue Upper Value 

        //Blue Limits RGB Blue Lower Value
        [JsonProperty("_blrgbbluelvalue")]
        [JsonIgnore]
        public string BLRGBBLUELValue
        {
            get { return _blrgbbluelvalue; }
            set
            {
                _blrgbbluelvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("BLRGBBLUELValue");
            }
        }

        private string _blueyminvalue = null;
        [JsonProperty("_blueyminvalue")]
        [JsonIgnore]
        public string BlueYMinValue
        {
            get { return _blueyminvalue; }
            set
            {
                _blueyminvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("BlueYMinValue");
            }
        }

        private string _blueymaxvalue = null;
        [JsonProperty("_blueymaxvalue")]
        [JsonIgnore]
        public string BlueYMaxValue
        {
            get { return _blueymaxvalue; }
            set
            {
                _blueymaxvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("BlueYMaxValue");
            }
        }

        //Blue Limits RGB Blue Upper Value
        [JsonProperty("_bllrgbblueuvalue")]
        [JsonIgnore]
        public string BLRGBBLUEUValue
        {

            get { return _bllrgbblueuvalue; }
            set
            {
                _bllrgbblueuvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("BLRGBBLUEUValue");
            }
        }

        private string _mBlueMicroWattValue = null;
        [JsonProperty("_mBlueMicroWattValue")]
        [JsonIgnore]
        public string BlueMicroWattValue
        {
            //get { return _rlrgbredlvalue; }
            get { return _mBlueMicroWattValue; }
            set
            {
                _mBlueMicroWattValue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("BlueMicroWattValue");
            }
        }


        private string _mBlueMicroWattValueUpper = null;
        [JsonProperty("_mBlueMicroWattValueUpper")]
        [JsonIgnore]
        public string BlueMicroWattValueUpper
        {
            //get { return _rlrgbreduvalue; } //Todo: Keith 
            get { return _mBlueMicroWattValueUpper; } //Todo: Keith 
            set
            {
                _mBlueMicroWattValueUpper = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("BlueMicroWattValueUpper");
            }
        }
        #endregion

        #region public methods to get/set properties of Red Limits RGB Section

        private string _rlrgbredlvalue; // Red Limit RGB Red Lower Value
        private string _rlrgbreduvalue; //Red Limit RGB red Upper Value

        // Red Limit RGB Red Lower Value
        [JsonProperty("_rlrgbredlvalue")]
        [JsonIgnore]
        public string RLRGBREDLValue
        {
            get { return _rlrgbredlvalue; }
            set
            {
                _rlrgbredlvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("RLRGBREDLValue");
            }
        }
        private string _redminyvalues;

        [JsonProperty("_redminyvalues")]
        [JsonIgnore]
        public string RedYMinValues
        {
            get { return _redminyvalues; }
            set
            {
                _redminyvalues = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("RedYMinValues");
            }
        }

        private string _redmaxyvalues;

        [JsonProperty("_redmaxyvalues")]
        [JsonIgnore]
        public string RedYMaxValue
        {
            get { return _redmaxyvalues; }
            set
            {
                _redmaxyvalues = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("RedYMaxValue");
            }
        }

        //Red Limit RGB red Upper Value
        [JsonProperty("_rlrgbreduvalue")]
        [JsonIgnore]
        public string RLRGBREDUValue
        {
            get { return _rlrgbreduvalue; }
            set
            {
                _rlrgbreduvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("RLRGBREDUValue");
            }
        }

        private string _mRedMicroWattValue = null;
        [JsonProperty("_mRedMicroWattValue")]
        [JsonIgnore]
        public string RedMicroWattValue
        {
            get { return _mRedMicroWattValue; }
            set
            {
                _mRedMicroWattValue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("RedMicroWattValue");
            }
        }

        private string _mRedMicroWattValueUpper = null;
        [JsonProperty("_mRedMicroWattValueUpper")]
        [JsonIgnore]
        public string RedMicroWattValueUpper

        {
            get { return _mRedMicroWattValueUpper; }
            set
            {
                _mRedMicroWattValueUpper = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("RedMicroWattValueUpper");
            }
        }
        #endregion

        #region public methods to set/get properties of Magenta Limits section

        private string mMagentaxmaxLValue; // Magenta Limits Red Lower value
        private string _mMagentaxminUValue; // Magenta Limits Red Upper Value
        private string _mMagentayminValue; // Magenta Limits Green Lower Value
        private string _mMagentaymaxValue; // Magenta Limits Green Upper Value
        private string _mMagentaLimitsBlueLValue; // Magenta Limits Blue Lower Value
        private string _mMagentaLimitsBlueUValue; // Magenta Limits Blue Upper Value
        private string _mMagenteMicroWattValue;
        private string _mMagenteMicroWattValueUpper;
        // Magenta Limits Red Lower value
        [JsonProperty("mMagentaxmaxLValue")]
        [JsonIgnore]
        public string MAGENTAMAXVALUE
        {
            get { return mMagentaxmaxLValue; }
            set
            {
                mMagentaxmaxLValue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("MAGENTAMAXVALUE");
            }
        }

        //Magenta Limits Red Upper Value
        [JsonProperty("_mMagentaxminUValue")]
        [JsonIgnore]
        public string MagentaXValue1
        {
            get { return _mMagentaxminUValue; }
            set
            {
                _mMagentaxminUValue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("MagentaXValue1");
            }
        }

        // Magenta Limits Green Lower Value
        [JsonProperty("_mMagentayminValue")]
        [JsonIgnore]
        public string MAGENTAYMINVALUE
        {
            get { return _mMagentayminValue; }
            set
            {
                _mMagentayminValue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("MAGENTAYMINVALUE");
            }
        }

        // Magenta Limits Green Upper Value
        [JsonProperty("_mMagentaymaxValue")]
        [JsonIgnore]
        public string MAGENTAYMAXVALUE
        {

            get { return _mMagentaymaxValue; }
            set
            {
                _mMagentaymaxValue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }

                OnPropertyChanged("MAGENTAYMAXVALUE");
            }
        }

        // Magenta Limits Blue Lower Value
        [JsonProperty("_mMagentaLimitsBlueLValue")]
        [JsonIgnore]
        public string MAGENTALBLUELValue
        {
            get { return _mMagentaLimitsBlueLValue; }
            set
            {
                _mMagentaLimitsBlueLValue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("MAGENTALBLUELValue");
            }
        }

        // Magenta Limits Blue Upper Value
        [JsonProperty("_mMagentaLimitsBlueUValue")]
        [JsonIgnore]
        public string MAGENTALBLUEUValue
        {
            get { return _mMagentaLimitsBlueUValue; }
            set
            {
                _mMagentaLimitsBlueUValue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("MAGENTALBLUEUValue");
            }
        }

        [JsonProperty("_mMagenteMicroWattValue")]
        [JsonIgnore]
        public string MagentaMicroWattValue
        {
            get { return _mMagenteMicroWattValue; }
            set
            {
                _mMagenteMicroWattValue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("MagentaMicroWattValue");
            }

        }

        [JsonProperty("_mMagenteMicroWattValueUpper")]
        [JsonIgnore]
        public string MagentaMicroWattValueUpper
        {
            get { return _mMagenteMicroWattValueUpper; }
            set
            {
                _mMagenteMicroWattValueUpper = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("MagentaMicroWattValueUpper");
            }

        }

        #endregion

        #region  public methods to set/get sensor gain section

        private string _sensorgainredvalue; // Sensor Gain Red Value
        private string _sensorgaingreenvalue; // Sensor Gain Green Value
        private string _sensorgainbluevalue; // Sensor gain Blue value

        // Sensor Gain Red Value
        [JsonProperty("_sensorgainredvalue")]
        [JsonIgnore]
        public string SensorGainREDValue
        {
            get { return _sensorgainredvalue; }
            set
            {
                _sensorgainredvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("SensorGainREDValue");
            }
        }

        // Sensor Gain Green Value
        [JsonProperty("_sensorgaingreenvalue")]
        [JsonIgnore]
        public string SensorGainGREENValue
        {
            get { return _sensorgaingreenvalue; }

            set
            {
                _sensorgaingreenvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("SensorGainGREENValue");
            }
        }

        //_sensorgainbluevalue
        [JsonProperty("_sensorgainbluevalue")]
        [JsonIgnore]
        public string SensorGainBLUEValue
        {
            get { return _sensorgainbluevalue; }
            set
            {
                _sensorgainbluevalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("SensorGainBLUEValue");
            }
        }
        #endregion

        #region public methods to set/get LED Temperature Derating section

        private string _ledtemderatingwhitevalue; // LED Temperature Derating White Value
        private string _ledtempderatingredvalue; // LED Temperature Derating Red Value
        private string _ledtempderatinggreenvalue; // LED Temperature Derating Green Value
        private string _ledtempderatingbluevalue; // LED Temperature Derating Blue Value

        // LED Temperature Derating White Value
        [JsonProperty("_ledtemderatingwhitevalue")]
        [JsonIgnore]
        public string LedTempDeratingWhiteValue
        {
            get { return _ledtemderatingwhitevalue; }
            set
            {
                _ledtemderatingwhitevalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("LedTempDeratingWhiteValue");
            }
        }

        // LED Temperature Derating Red Value
        [JsonProperty("_ledtempderatingredvalue")]
        [JsonIgnore]
        public string LedTempDeratingRedValue
        {
            get { return _ledtempderatingredvalue; }
            set
            {
                _ledtempderatingredvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("LedTempDeratingRedValue");
            }
        }

        // LED Temperature Derating Green Value
        [JsonProperty("_ledtempderatinggreenvalue")]
        [JsonIgnore]
        public string LedTempDeratingGreenValue
        {
            get { return _ledtempderatinggreenvalue; }
            set
            {
                _ledtempderatinggreenvalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("LedTempDeratingGreenValue");
            }
        }

        // LED Temperature Derating Blue Value
        [JsonProperty("_ledtempderatingbluevalue")]
        [JsonIgnore]
        public string LedTempDeratingBlueValue
        {
            get { return _ledtempderatingbluevalue; }
            set
            {
                _ledtempderatingbluevalue = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("LedTempDeratingBlueValue");
            }
        }
        #endregion

        #region private variables for 500w White Limits section
        private string _5wwlredlvalue; // 500w White Limits Red Lower value
        private string _5wwlreduvalue; // 500w White Limits Red Upper Value
        private string _5wwlgreenlvalue; // 500w White Limits Green Lower Value
        private string _5wwlgreenuvalue; // 500w White Limits Green Upper Value
        private string _5wwlbluelvalue; // 500w White Limits Blue Lower Value
        private string _5wwlblueuvalue; // 500w White Limits Blue Upper Value

        #endregion

        #region private variables for fixture id
        private string _fixtureid; //Fixture Id
        #endregion

        #region public method to set/get properties of Fixture Id section
        //Fixture Id
        [JsonProperty("_fixtureid")]
        [JsonIgnore]
        public string FixtureId
        {
            get { return _fixtureid; }
            set
            {
                _fixtureid = value;
                try
                {
                    bool ret_val = DataValidation.ValidateInputData(value);
                    if (ret_val != true)
                    {
                        _canSaveData = false;
                        throw new ApplicationException("Enter Integer Number");
                    }
                    _canSaveData = true;
                }
                catch (ApplicationException x)
                {
                    MessageBox.Show(x.Message);
                }
                OnPropertyChanged("FixtureId");
            }
        }

        #endregion

        #region private variable for board level and assemble
        private bool _isboardlevelrbchecked;
        private bool _isassemblyrbchecked;
        private bool _isimagetestchecked;

        public bool IsBoardLevelRBChecked
        {
            get { return _isboardlevelrbchecked; }
            set
            {
                _isboardlevelrbchecked = value;
                OnPropertyChanged("IsBoardLevelRBChecked");
            }
        }

        public bool IsAssemblyRBChecked
        {
            get { return _isassemblyrbchecked; }
            set
            {
                _isassemblyrbchecked = value;
                OnPropertyChanged("IsAssemblyRBChecked");
            }
        }
        [JsonProperty("_isimagetestchecked")]
        [JsonIgnore]
        public bool IsImageTestingChecked
        {
            get { return _isimagetestchecked; }
            set
            {
                _isimagetestchecked = value;
                OnPropertyChanged("IsImageTestingChecked");
            }
        }
        #endregion

        #region private variables
        private string _mtestdisp;
        public string TestDisplay
        {
            get { return _mtestdisp; }
            set
            {
                _mtestdisp = value;
                OnPropertyChanged("TestDisplay");
            }
        }

        private string _minput;
        public string Input
        {
            get
            {
                return _minput;
            }
            set
            {
                _minput = value;
            }
        }
        #endregion
        
        
        #region Load saved setting details from file
        private void LoadSettingDetailsFromFile()
        {
            string file_path = TestFixtureConstants.GetSettingsInfoFilePath();
            if (!string.IsNullOrEmpty(file_path) && (File.Exists(file_path)))
            {
                DeserializeAndSetProperties(file_path);
            }
        }

        private void DeserializeAndSetProperties(string filepath)
        {
            //handle file path scenarion
            try
            {
               
                string jsonsettingdetails = System.IO.File.ReadAllText(filepath);
                dynamic files = JsonConvert.DeserializeObject(jsonsettingdetails);
                foreach (var f in files)
                {
                    PopulateDataIntoSettingPage(f);
                }
            }
            catch (FileNotFoundException e)
            {
                //don't do anything... proceed further without halting page
            }

        }
        [JsonProperty("_comportnumb")]
        private string _comportnumb = null;
        [JsonIgnore]
        public string CommPortNUmber
        {
            get { return _comportnumb; }
            set
            {
                _comportnumb = value;
                OnPropertyChanged("CommPortNUmber");
            }

        }

        // Barcode model numbers for saving into setting file
        // Added by prakash
        // 120V - 50Ft
        [JsonProperty("_model_120V_50Ft")]
        private string _model_120V_50Ft = null;
        [JsonIgnore]
        public string ModelNumber120V50Ft
        {
            get { return _model_120V_50Ft; }
            set
            {
                _model_120V_50Ft = value;
                OnPropertyChanged("ModelNumber120V50Ft");
            }

        }
        // Barcode model numbers for saving into setting file
        // Added by prakash
        // 120V - 150Ft
        [JsonProperty("_model_120V_150Ft")]
        private string _model_120V_150Ft = null;
        [JsonIgnore]
        public string ModelNumber120V150Ft
        {
            get { return _model_120V_150Ft; }
            set
            {
                _model_120V_150Ft = value;
                OnPropertyChanged("ModelNumber120V150Ft");
            }

        }
        // Barcode model numbers for saving into setting file
        // Added by prakash
        // 12V - 50Ft
        [JsonProperty("_model_12V_50Ft")]
        private string _model_12V_50Ft = null;
        [JsonIgnore]
        public string ModelNumber12V50Ft
        {
            get { return _model_12V_50Ft; }
            set
            {
                _model_12V_50Ft = value;
                OnPropertyChanged("ModelNumber12V50Ft");
            }

        }
        // Barcode model numbers for saving into setting file
        // Added by prakash
        // 12V - 150Ft
        [JsonProperty("_model_12V_150Ft")]
        private string _model_12V_150Ft = null;
        [JsonIgnore]
        public string ModelNumber12V150Ft
        {
            get { return _model_12V_150Ft; }
            set
            {
                _model_12V_150Ft = value;
                OnPropertyChanged("ModelNumber12V150Ft");
            }

        }

        [JsonProperty("_bandwidth_tolerance_second")]
        private double _bandwidth_tolerance_second = 0;
        [JsonIgnore]
        public double BWidthToleranceSecond
        {
            get { return _bandwidth_tolerance_second; }
            set
            {
                _bandwidth_tolerance_second = value;
                OnPropertyChanged("BWidthToleranceSecond");
            }

        }

        #region IP Address Range, Boot Sequence Time, IsProduction
        private bool _mIsProduction = false;
        [JsonProperty("_mIsProduction")]
        [JsonIgnore]
        public bool IsProduction
        {
            get { return _mIsProduction; }
            set
            {
                _mIsProduction = value;
                OnPropertyChanged("IsProduction");
            }
        }

        private string _mBootSequenceTime = null;
        [JsonProperty("_mBootSequenceTime")]
        [JsonIgnore]
        public string BootSequenceTime
        {
            get { return _mBootSequenceTime; }
            set
            {
                _mBootSequenceTime = value;
                OnPropertyChanged("BootSequenceTime");
            }
        }

        private string _mIpAddressRange = null;
        [JsonProperty("_mIpAddressRange")]
        [JsonIgnore]
        public string IpAddressRange
        {
            get { return _mIpAddressRange; }
            set
            {
                _mIpAddressRange = value;
                OnPropertyChanged("IpAddressRange");
            }
        }
        #endregion

        #region Integration time
        private string _mIntegrationTime = null;
        [JsonProperty("_mIntegrationTime")]
        [JsonIgnore]
        public string IntegrationTime
        {
            get { return _mIntegrationTime; }
            set
            {
                _mIntegrationTime = value;
                OnPropertyChanged("IntegrationTime");
            }
        }

        private string _mBlueIntegrationTime = null;
        [JsonProperty("_mBlueIntegrationTime")]
        [JsonIgnore]
        public string BlueIntegrationTime
        {
            get { return _mBlueIntegrationTime; }
            set
            {
                _mBlueIntegrationTime = value;
                OnPropertyChanged("BlueIntegrationTime");
            }
        }


        private string _mRedIntegrationTime = null;
        [JsonProperty("_mRedIntegrationTime")]
        [JsonIgnore]
        public string RedIntegrationTime
        {
            get { return _mRedIntegrationTime; }
            set
            {
                _mRedIntegrationTime = value;
                OnPropertyChanged("RedIntegrationTime");
            }
        }


        private string _mGreenIntegrationTime = null;
        [JsonProperty("_mGreenIntegrationTime")]
        [JsonIgnore]
        public string GreenIntegrationTime
        {
            get { return _mGreenIntegrationTime; }
            set
            {
                _mGreenIntegrationTime = value;
                OnPropertyChanged("GreenIntegrationTime");
            }
        }


        private string _mWhiteIntegrationTime = null;
        [JsonProperty("_mWhiteIntegrationTime")]
        [JsonIgnore]
        public string WhiteIntegrationTime
        {
            get { return _mWhiteIntegrationTime; }
            set
            {
                _mWhiteIntegrationTime = value;
                OnPropertyChanged("WhiteIntegrationTime");
            }
        }


        private string _mBlendedWhiteIntegrationTime = null;
        [JsonProperty("_mBlendedWhiteIntegrationTime")]
        [JsonIgnore]
        public string BlendedWhiteIntegrationTime
        {
            get { return _mBlendedWhiteIntegrationTime; }
            set
            {
                _mBlendedWhiteIntegrationTime = value;
                OnPropertyChanged("BlendedWhiteIntegrationTime");
            }
        }


        private string _mMagendaIntegrationTime = null;
        [JsonProperty("_mMagendaIntegrationTime")]
        [JsonIgnore]
        public string MagendaIntegrationTime
        {
            get { return _mMagendaIntegrationTime; }
            set
            {
                _mMagendaIntegrationTime = value;
                OnPropertyChanged("MagendaIntegrationTime");
            }
        }
        #endregion

        #region FTP Limits
        [JsonProperty("_FTP_ON_MIN")]
        private string _FTP_ON_MIN = null;
        [JsonIgnore]
        public string FtpOnMin
        {
            get { return _FTP_ON_MIN; }
            set
            {
                _FTP_ON_MIN = value;
                OnPropertyChanged("FtpOnMin");
            }
        }

        [JsonProperty("_FTP_ON_MAX")]
        private string _FTP_ON_MAX = null;
        [JsonIgnore]
        public string FtpOnMax
        {
            get { return _FTP_ON_MAX; }
            set
            {
                _FTP_ON_MAX = value;
                OnPropertyChanged("FtpOnMax");
            }
        }

        [JsonProperty("_FTP_OFF_MIN")]
        private string _FTP_OFF_MIN = null;
        [JsonIgnore]
        public string FtpOffMin
        {
            get { return _FTP_OFF_MIN; }
            set
            {
                _FTP_OFF_MIN = value;
                OnPropertyChanged("FtpOffMin");
            }
        }

        [JsonProperty("_FTP_OFF_MAX")]
        private string _FTP_OFF_MAX = null;
        [JsonIgnore]
        public string FtpOffMax
        {
            get { return _FTP_OFF_MAX; }
            set
            {
                _FTP_OFF_MAX = value;
                OnPropertyChanged("FtpOffMax");
            }
        }
        #endregion

        //though me not happy with this function but Deserialize some how not working... need to find better solution
        //deserialize is some how getting into loop and resulting into stack over loop
        //but this needs to be revisited to find the optimum solution
        private void PopulateDataIntoSettingPage(dynamic file)
        {
            if ("_wlrxminlvalue" == file.Name)
            { WLRGBXMINVALUE = file.Value; }
            else if ("_wlrxmaxvalues" == file.Name)
            { WLRGBXMAXVALUE = file.Value; }
            else if ("_wlryminvalues" == file.Name)
            { WLRYMINVALUE = file.Value; }
            else if ("_wlrymaxvalues" == file.Name)
            { WLRYMAXVALUE = file.Value; }
            else if ("_mWhiteMicroWattValue" == file.Name)
            { WhiteMicroWattValue = file.Value; }
            else if ("_mWhiteMicroWattValueUpper" == file.Name)
            { WhiteMicroWattValueUpper = file.Value; }
            else if ("_glrgbgreenlvalue" == file.Name)
            { GLRGBGREENLValue = file.Value; }
            else if ("_glrgbgreenuvalue" == file.Name)
            { GLRGBGREENUValue = file.Value; }
            else if ("_mblendedredlvalue" == file.Name)
            { BlendedWhiteRedLimits = file.Value; }
            else if ("_greenyminvalue" == file.Name)
            { GreenYMinValues = file.Value; }
            else if ("_greenymaxvalue" == file.Name)
            { GreenYMaxValue = file.Value; }
            else if ("_mGreenMicroWatValue" == file.Name)
            { GreenMicroWattValue = file.Value; }
            else if ("_mGreenMicroWatValueUpper" == file.Name)
            { GreenMicroWattValueUpper = file.Value; }
            else if ("_mblendedreduvalue" == file.Name)
            { BlendedWhiteRedULimits = file.Value; }
            else if ("_mblendedgreenlvalue" == file.Name)
            { BlendedWhiteGreenLLimits = file.Value; }
            else if ("_mblendedgreenuvalue" == file.Name)
            { BlendedWhiteGreenULimits = file.Value; }
            else if ("_mblendedbluelvalue" == file.Name)
            { BlendedWhiteBlueLLimits = file.Value; }
            else if ("_mblendedblueuvalue" == file.Name)
            { BlendedWhiteBlueULimits = file.Value; }
            else if ("_blrgbbluelvalue" == file.Name)
            { BLRGBBLUELValue = file.Value; }
            else if ("_blueyminvalue" == file.Name)
            { BlueYMinValue = file.Value; }
            else if ("_blueymaxvalue" == file.Name)
            { BlueYMaxValue = file.Value; }
            else if ("_bllrgbblueuvalue" == file.Name)
            { BLRGBBLUEUValue = file.Value; }
            else if ("_mBlueMicroWattValue" == file.Name)
            { BlueMicroWattValue = file.Value; }
            else if ("_mBlueMicroWattValueUpper" == file.Name)
            { BlueMicroWattValueUpper = file.Value; }
            else if ("_rlrgbredlvalue" == file.Name)
            { RLRGBREDLValue = file.Value; }
            else if ("_redminyvalues" == file.Name)
            { RedYMinValues = file.Value; }
            else if ("_redmaxyvalues" == file.Name)
            { RedYMaxValue = file.Value; }
            else if ("_rlrgbreduvalue" == file.Name)
            { RLRGBREDUValue = file.Value; }
            else if ("_mRedMicroWattValue" == file.Name)
            { RedMicroWattValue = file.Value; }
            else if ("_mRedMicroWattValueUpper" == file.Name)
            { RedMicroWattValueUpper = file.Value; }
            else if ("mMagentaxmaxLValue" == file.Name)
            { MAGENTAMAXVALUE = file.Value; }
            else if ("_mMagentaxminUValue" == file.Name)
            { MagentaXValue1 = file.Value; }
            else if ("_mMagentayminValue" == file.Name)
            { MAGENTAYMINVALUE = file.Value; }
            else if ("_mMagentaymaxValue" == file.Name)
            { MAGENTAYMAXVALUE = file.Value; }
            else if ("_mMagentaLimitsBlueLValue" == file.Name)
            { MAGENTALBLUELValue = file.Value; }
            else if ("_mMagentaLimitsBlueUValue" == file.Name)
            { MAGENTALBLUEUValue = file.Value; }
            else if ("_mMagenteMicroWattValue" == file.Name)
            { MagentaMicroWattValue = file.Value; }
            else if ("_mMagenteMicroWattValueUpper" == file.Name)
            { MagentaMicroWattValueUpper = file.Value; }
            else if ("_sensorgainredvalue" == file.Name)
            { SensorGainREDValue = file.Value; }
            else if ("_sensorgaingreenvalue" == file.Name)
            { SensorGainGREENValue = file.Value; }
            else if ("_sensorgainbluevalue" == file.Name)
            { SensorGainBLUEValue = file.Value; }
            else if ("_ledtemderatingwhitevalue" == file.Name)
            { LedTempDeratingWhiteValue = file.Value; }
            else if ("_ledtempderatingredvalue" == file.Name)
            { LedTempDeratingRedValue = file.Value; }
            else if ("_ledtempderatinggreenvalue" == file.Name)
            { LedTempDeratingGreenValue = file.Value; }
            else if ("_ledtempderatingbluevalue" == file.Name)
            { LedTempDeratingBlueValue = file.Value; }
            else if ("_fixtureid" == file.Name)
            { FixtureId = file.Value; }
            else if ("IsBoardLevelRBChecked" == file.Name)
            { IsBoardLevelRBChecked = file.Value; }
            else if ("_comportnumb" == file.Name)
            { CommPortNUmber = file.Value; }
            else if ("_isimagetestchecked" == file.Name)
            { IsImageTestingChecked = file.Value; }
            else if ("_mBlueIntegrationTime" == file.Name)
            { BlueIntegrationTime = file.Value; }
            else if ("_mRedIntegrationTime" == file.Name)
            { RedIntegrationTime = file.Value; }
            else if ("_mGreenIntegrationTime" == file.Name)
            { GreenIntegrationTime = file.Value; }
            else if ("_mWhiteIntegrationTime" == file.Name)
            { WhiteIntegrationTime = file.Value; }
            else if ("_mBlendedWhiteIntegrationTime" == file.Name)
            { BlendedWhiteIntegrationTime = file.Value; }
            else if ("_mMagendaIntegrationTime" == file.Name)
            { MagendaIntegrationTime = file.Value; }
            else if ("_mIntegrationTime" == file.Name)
            { IntegrationTime = file.Value; }
            else if ("_model_120V_50Ft" == file.Name)
            { ModelNumber120V50Ft = file.Value; }
            else if ("_model_120V_150Ft" == file.Name)
            { ModelNumber120V150Ft = file.Value; }
            else if ("_model_12V_50Ft" == file.Name)
            { ModelNumber12V50Ft = file.Value; }
            else if ("_model_12V_150Ft" == file.Name)
            { ModelNumber12V150Ft = file.Value; }
            else if ("_FTP_ON_MIN" == file.Name)
            { FtpOnMin = file.Value; }
            else if ("_FTP_ON_MAX" == file.Name)
            { FtpOnMax = file.Value; }
            else if ("_FTP_OFF_MIN" == file.Name)
            { FtpOffMin = file.Value; }
            else if ("_FTP_OFF_MAX" == file.Name)
            { FtpOffMax = file.Value; }
            else if ("_mBootSequenceTime" == file.Name)
            { BootSequenceTime = file.Value; }
            else if ("_mIpAddressRange" == file.Name)
            { IpAddressRange = file.Value; }
            else if ("_mIsProduction" == file.Name)
            { IsProduction = file.Value; } 
        }
        #endregion

        #region to set the visibility factor

        public bool CanAccessSettingPage
        {
            get { return _mcollapsesettingstabitem; }
        }
           #endregion
 
        public override void ButtonOKClick()
        {
            throw new NotImplementedException();
        }
    }
}
