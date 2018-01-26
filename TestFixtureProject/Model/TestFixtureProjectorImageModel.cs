using System;
using System.Windows.Media;
using TestFixtureProject.ViewModel;
using System.Drawing;
using AForge.Imaging.Filters;
using AForge.Imaging;
using System.Drawing.Imaging;
using AForge.Math;
using System.IO;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using TestFixtureProject.Common;
using Microsoft.Win32;
using System.Windows.Forms;

namespace TestFixtureProject.Model
{
    public class TestFixtureProjectorImageModel : TestFixtureLoginBaseVM
    {

        #region private variables
        private UnmanagedImage _medgeimg = null;
        private UnmanagedImage _mgrayImg = null;
        private string destgrayimage = null;
        Bitmap _mimg = null;
        BitmapData _mimageData = null;
        Bitmap _mconvgrayimg = null;
        string _mfolderPath = null;


        float _mmeanValue = 0;
        private float _mmaxvalue = 0;
        private float _mmedianvalue = 0;
        private float _mstddev = 0;
        private float _mminvalue = 0;
        private bool _mcollapsesettingstabitem;
        float _mintent = 0;

        private float _mmeanValue1 = 0;
        private float _mmaxvalue1 = 0;
        private float _mmedianvalue1 = 0;
        private float _mstddev1 = 0;
        private float _mminvalue1 = 0;

        #endregion

        #region constructor
        public TestFixtureProjectorImageModel()
        {
            LoadSettingDetailsFromFile();
            _mfolderPath = TestFixtureConstants.CreateDirectoryIfNotExists("Images");
            //LoadConvertedImages();
        }
        #endregion

        #region Image loading
        private ImageSource _mimagesource;
        public ImageSource LoadImageSource
        {
            get
            {
                return _mimagesource;
            }
            set
            {
                _mimagesource = value;
                OnPropertyChanged("LoadImageSource");
            }
        }

        private string _mfileName;
        public string OriginalImgName
        {
            get { return _mfileName; }
            set
            {
                _mfileName = value;
                OnPropertyChanged("OriginalImgName");
            }
        }

        private ImageSource _mgrayimg;
        public ImageSource LoadGrayImgSource
        {
            get { return _mgrayimg; }
            set
            {
                _mgrayimg = value;
                OnPropertyChanged("LoadGrayImgSource");
            }

        }
        #endregion

        #region Image Methods

        public void ImageTaken1()
        {
            //      BitmapData imageData = null;
            string pathString = null;




            if (_mimagesource != null)
            {
                //string path1 = @_mfolderPath + "\\" + "CapturedImgFVO.png";
                //_mimagesource = new BitmapImage(new Uri(path1));

                _mimg = CovertBitmapImgToBitMap();
                ////process original image for comparision
                //string path = @_mfolderPath + "\\" + "OriginalImg.png";
                //ImageSource original = new BitmapImage(new Uri(path));
                //_mOriginalImg = ConvertBitmapImgToBitMap(original);

                //create image file in the specified path
                //pathString = System.IO.Path.Combine(folderPath, "CapturedImg.png");
                File.Delete(@_mfolderPath + "\\" + "OriginalImg.png");

                _mimg.Save(_mfolderPath + "\\" + "OriginalImg.png", ImageFormat.Png);
                pathString = null;



                // img = AForge.Imaging.Image.FromFile(destgrayimage);

                _mimageData = _mimg.LockBits(new Rectangle(0, 0, _mimg.Width, _mimg.Height), ImageLockMode.ReadOnly, _mimg.PixelFormat);
                //_moimgdata = _mOriginalImg.LockBits(new Rectangle(0, 0, _mOriginalImg.Width, _mOriginalImg.Height), ImageLockMode.ReadOnly, _mOriginalImg.PixelFormat);
                try
                {
                    UnmanagedImage _unmanagedimage = new UnmanagedImage(_mimageData);
                    UnmanagedImage unmanagedgrayimg = UnmanagedImage.Create(_mimg.Width, _mimg.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                    Grayscale.CommonAlgorithms.BT709.Apply(_unmanagedimage, unmanagedgrayimg);
                    SobelEdgeDetector edgedetector = new SobelEdgeDetector();
                    _medgeimg = edgedetector.Apply(unmanagedgrayimg);

                    ////original image processing
                    //UnmanagedImage _unmanagedimage1 = new UnmanagedImage(_moimgdata);
                    //UnmanagedImage unmanagedgrayimg1 = UnmanagedImage.Create(_mOriginalImg.Width, _mOriginalImg.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                    //Grayscale.CommonAlgorithms.BT709.Apply(_unmanagedimage1, unmanagedgrayimg1);
                    //SobelEdgeDetector edgedetector1 = new SobelEdgeDetector();
                    //_moedgeimg = edgedetector1.Apply(unmanagedgrayimg1);
                    ////end of original image processing


                    Threshold threshholdfilter = new Threshold(58);

                    threshholdfilter.ApplyInPlace(_medgeimg);
                    _mconvgrayimg = _medgeimg.ToManagedImage();



                    //create image file in the specified path
                    //pathString = System.IO.Path.Combine(folderPath, "ProcessedImg.png");
                    File.Delete(@_mfolderPath + "\\" + "CProcessedImg.png");
                    _mconvgrayimg.Save(_mfolderPath + "\\" + "CProcessedImg.png", ImageFormat.Png);
                    pathString = null;

                    GetStatistics();
                    FindImageSlope();
                    pathString = null;
                    //  LoadConvertedImages();
                    // CompareBothImages();
                    //FindSquareFiducials();
                }
                catch (SystemException e)
                {
                    String exception = e.ToString();
                    frmTestFixture.Instance.WriteToLog("SetConfigurationSettings0: " + exception, ApplicationConstants.TraceLogType.Error);
                }
                catch (Exception ex)
                {
                    frmTestFixture.Instance.WriteToLog("SetConfigurationSettings1: " + ex.Message, ApplicationConstants.TraceLogType.Error);
                }
                finally
                {
                    // _mimg.UnlockBits(_mimageData);
                }
            }
        }


        /*
         1.Read the output from USB Camera
         * 2. Call the Image processing library to compare with original pic
         */
        public void ImageTaken()
        {
            //      BitmapData imageData = null;
            string pathString = null;


            if (_mimagesource != null)
            {
                try
                {
                    _mimg = CovertBitmapImgToBitMap();
                //GetStatistics();
                //save converted bitmap for testing purpose..later can be commented out 

                //create image file in the specified path
                //pathString = System.IO.Path.Combine(folderPath, "CapturedImg.png");
                File.Delete(@_mfolderPath + "\\" + "OriginalImg.png");
              
                _mimg.Save(_mfolderPath + "\\" + "OriginalImg.png",ImageFormat.Png);
                pathString = null;

                    
                    //// img = AForge.Imaging.Image.FromFile(destgrayimage);

                    _mimageData = _mimg.LockBits(new Rectangle(0, 0, _mimg.Width, _mimg.Height), ImageLockMode.ReadOnly, _mimg.PixelFormat);

                    _mimg.UnlockBits(_mimageData);
                    UnmanagedImage _unmanagedimage = new UnmanagedImage(_mimageData);
                    UnmanagedImage unmanagedgrayimg = UnmanagedImage.Create(_mimg.Width, _mimg.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                    Grayscale.CommonAlgorithms.BT709.Apply(_unmanagedimage, unmanagedgrayimg);
                    SobelEdgeDetector edgedetector = new SobelEdgeDetector();
                    _medgeimg = edgedetector.Apply(unmanagedgrayimg);

                    Threshold threshholdfilter = new Threshold(58);

                    threshholdfilter.ApplyInPlace(_medgeimg);
                    _mconvgrayimg = _medgeimg.ToManagedImage();
                    DateTime cdatetime = System.DateTime.Now;
                    string dtimestr = cdatetime.ToString("yyyyMMddhmmtt");


                    //         File.Delete(@_mfolderPath + "\\" + "OProcessedImg.png");
                    string savedFileName = dtimestr + OriginalImgName + ".png";
                    _mconvgrayimg.Save(_mfolderPath + "\\" + savedFileName, ImageFormat.Png);

                    pathString = null;

                    GetStatistics();
                    //FindImageSlope();
                    //GetStatistics();

                    LoadConvertedImages(savedFileName);
                }
                catch (SystemException e)
                {
                    String exception = e.ToString();
                    frmTestFixture.Instance.WriteToLog("ImageTaken0: " + exception, ApplicationConstants.TraceLogType.Error);
                }
                catch (Exception ex)
                {
                    frmTestFixture.Instance.WriteToLog("ImageTaken1: " + ex.Message, ApplicationConstants.TraceLogType.Error);
                }
                finally
                {
                   // _mimg.UnlockBits(_mimageData);
                }
            }
        }
        public void LoadConvertedImages(string aSavedFileName)
        {
            string fullpath = _mfolderPath + "\\" + aSavedFileName;
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.FileName = fullpath;
            LoadGrayImgSource = new BitmapImage(new Uri(dlg.FileName));
            OnPropertyChanged("LoadGrayImgSource");
        }

        private Bitmap CovertBitmapImgToBitMap()
        {
            var imgage = (BitmapSource)_mimagesource;

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(imgage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
                return new Bitmap(bitmap);
            }


        }
        private void GetStatistics()
        {
            //ImageStatistics statistics = new ImageStatistics(_mconvgrayimg);
            //statistics.

            //Histogram grayhisto = statistics.Gray;
            //GetOrginalImgMaxValue = grayhisto.Max;
            //OnPropertyChanged("GetOrginalImgMaxValue");
            //GetOrginalImgMinValue = grayhisto.Min;
            //OnPropertyChanged("GetOrginalImgMinValue");
            //GetOrginalImgMedianValue = grayhisto.Median;
            //OnPropertyChanged("GetOrginalImgMedianValue");
            //GetOrginalImgStdDevValue = grayhisto.StdDev;
            //OnPropertyChanged("GetOrginalImgStdDevValue");
            //GetOrginalImgMeanValue = grayhisto.Mean;
            //OnPropertyChanged("GetOrginalImgMeanValue");

            //  ImageStatisticsHSL statistics = new ImageStatisticsHSL(_medgeimg);
            // ContinuousHistogram hist = statistics.LuminanceWithoutBlack;
            ImageStatistics statistics = new ImageStatistics(_medgeimg);

            Histogram hist = statistics.Gray;

            GetOrginalImgMaxValue = hist.Max;
            OnPropertyChanged("GetOrginalImgMaxValue");

            GetOrginalImgMinValue = hist.Min;
            OnPropertyChanged("GetOrginalImgMinValue");

            GetOrginalImgMedianValue = hist.Median;
            OnPropertyChanged("GetOrginalImgMedianValue");

            GetOrginalImgStdDevValue = (float)hist.StdDev;
            OnPropertyChanged("GetOrginalImgStdDevValue");
            GetOrginalImgMeanValue = (float)hist.Mean;
            OnPropertyChanged("GetOrginalImgMeanValue");

            //ImageStatisticsHSL statistics1 = new ImageStatisticsHSL(_mimg);
            //ContinuousHistogram hist1 = statistics1.Luminance;
            ImageStatistics statistics1 = new ImageStatistics(_medgeimg);

            Histogram hist1 = statistics.Gray;
            GetOrginalImgMaxValue1 = hist1.Max;
            OnPropertyChanged("GetOrginalImgMaxValue1");

            GetOrginalImgMinValue1 = hist1.Min;
            OnPropertyChanged("GetOrginalImgMinValue1");

            GetOrginalImgMedianValue1 = hist1.Median;
            OnPropertyChanged("GetOrginalImgMedianValue1");

            GetOrginalImgStdDevValue1 = (float)hist1.StdDev;
            OnPropertyChanged("GetOrginalImgStdDevValue1");

            GetOrginalImgMeanValue1 = (float)hist1.Mean;
            OnPropertyChanged("GetOrginalImgMeanValue1");
        }
        #endregion

        #region create the folder if doesnt exist
        private void GetOtherStatistics()
        {
        //Im

        }

        #endregion

        [JsonProperty("_mmeanValue")]
        public float GetOrginalImgMeanValue
        {
            get
            {
                return _mmeanValue;
            }
            set
            {
                _mmeanValue = value;

                if (frmTestFixture.Instance != null)
                {
                    if(frmTestFixture.Instance.txtGetOrginalImgMeanValue.IsHandleCreated)
                    frmTestFixture.Instance.txtGetOrginalImgMeanValue.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.txtGetOrginalImgMeanValue.Text = _mmeanValue.ToString(); });
                }
                OnPropertyChanged("GetOrginalImgMeanValue");
            }
        }

        //called to set value for Std Dev in Histogram section
        [JsonProperty("_mstddev")]
        public float GetOrginalImgStdDevValue
        {
            get
            {
                return _mstddev;
            }
            set
            {
                _mstddev = value;

                if (frmTestFixture.Instance != null)
                    if (frmTestFixture.Instance.txtGetOrginalImgStdDevValue.IsHandleCreated)
                    {
                        frmTestFixture.Instance.txtGetOrginalImgStdDevValue.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.txtGetOrginalImgStdDevValue.Text = _mstddev.ToString(); });
                    }
                OnPropertyChanged("GetOrginalImgStdDevValue");
            }
        }

        //called to set value for Median in Histogram section
        [JsonProperty("_mmedianvalue")]
        public float GetOrginalImgMedianValue
        {
            get
            {
                return _mmedianvalue;
            }
            set
            {
                _mmedianvalue = value;

                if (frmTestFixture.Instance != null)
                {
                    if (frmTestFixture.Instance.txtGetOrginalImgMedianValue.IsHandleCreated)
                    {
                        frmTestFixture.Instance.txtGetOrginalImgMedianValue.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.txtGetOrginalImgMedianValue.Text = _mmedianvalue.ToString(); });
                    }
                }
                OnPropertyChanged("GetOrginalImgMedianValue");

            }
        }

        //called to set value of Min in Histogram section
        [JsonProperty("_mminvalue")]
        public float GetOrginalImgMinValue
        {
            get
            {
                return _mminvalue;
            }
            set
            {
                _mminvalue = value;

                if (frmTestFixture.Instance != null)
                {
                    if (frmTestFixture.Instance.txtGetOrginalImgMinValue.IsHandleCreated)
                    {
                        frmTestFixture.Instance.txtGetOrginalImgMinValue.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.txtGetOrginalImgMinValue.Text = _mminvalue.ToString(); });
                    }
                }
                OnPropertyChanged("GetOrginalImgMinValue");
            }
        }

        //called to set value of Min in Histogram section
        [JsonProperty("_mmaxvalue")]
        public float GetOrginalImgMaxValue
        {
            get
            {
                return _mmaxvalue;
            }
            set
            {
                _mmaxvalue = value;

                if (frmTestFixture.Instance != null)
                {
                    if (frmTestFixture.Instance.txtGetOrginalImgMaxValue.IsHandleCreated)
                    {
                        frmTestFixture.Instance.txtGetOrginalImgMaxValue.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.txtGetOrginalImgMaxValue.Text = _mmaxvalue.ToString(); });
                    }
                }
                OnPropertyChanged("GetOrginalImgMaxValue");
            }
        }
        private float _mtoprightvoltage = 0;
        [JsonProperty("_mtoprightvoltage")]
        public float ReadTopRightVoltage
        {
            get { return _mtoprightvoltage; }
            set
            {
                _mtoprightvoltage = value;
                OnPropertyChanged("ReadTopRightVoltage");
            }
        }

        private float _mtopleftvoltage = 0;
        [JsonProperty("_mtopleftvoltage")]
        public float ReadTopLeftVoltage
        {
            get { return _mtoprightvoltage; }
            set
            {
                _mtopleftvoltage = value;
                OnPropertyChanged("ReadTopLeftVoltage");
            }
        }

        private float _mbottomrightvoltage = 0;
        [JsonProperty("_mbottomrightvoltage")]
        public float ReadBottomRightVoltage
        {
            get { return _mtoprightvoltage; }
            set
            {
                _mbottomrightvoltage = value;
                OnPropertyChanged("ReadBottomRightVoltage");
            }
        }

        private float _mbottomleftvoltage = 0;
        [JsonProperty("_mbottomleftvoltage")]
        public float ReadBottomLeftVoltage
        {
            get { return _mbottomleftvoltage; }
            set
            {
                _mbottomleftvoltage = value;
                OnPropertyChanged("ReadBottomLeftVoltage");
            }
        }
        private float _mthreshholdsim=0f;
        [JsonProperty("_mthreshholdsim")]
        public float ThreshHoldSimilarity
        {
            get { return _mthreshholdsim; }
            set
            {
                _mthreshholdsim = value;

                if (frmTestFixture.Instance != null)
                {
                    if (frmTestFixture.Instance.txtThreshHoldSimilarity.IsHandleCreated)
                    {
                        frmTestFixture.Instance.txtThreshHoldSimilarity.Invoke((MethodInvoker)delegate { frmTestFixture.Instance.txtThreshHoldSimilarity.Text = _mthreshholdsim.ToString(); });
                    }
                }
                OnPropertyChanged("ThreshHoldSimilarity");
            }
        }
        #region
        private void CompareImages()
        {
            var tempmatch = new ExhaustiveTemplateMatching(_mthreshholdsim);
            var results =  tempmatch.ProcessImage(_medgeimg, _medgeimg);
            //float value = results[0].Similarity;
        }

        #endregion
        public UnmanagedImage GetOriginalImage()
        {
            return _medgeimg;
        }

        #region file operation to load 
        public void LoadSettingDetailsFromFile()
        {
            string file_path = TestFixtureConstants.ReadSaveImageSettingDetails();
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
            catch (Exception e)
            {
                //don't do anything... proceed further without halting page
                frmTestFixture.Instance.WriteToLog("DeserializeAndSetProperties: " + e.Message, ApplicationConstants.TraceLogType.Error);
            }
        }
        private string _mrmirroryvalues = null;
        [JsonProperty("_mrmirroryvalues")]
        public string UpdatetrmirrorYvalues
        {
            get { return _mrmirroryvalues; }
            set
            {
                _mrmirroryvalues = value;
                OnPropertyChanged("UpdatetrmirrorYvalues");
            }
        }


        private string _mmirrorlxvalues = null;
        [JsonProperty("_mmirrorlxvalues")]
        public string UpdatetLmirrorXvalues
        {
            get { return _mmirrorlxvalues; }
            set
            {
                _mmirrorlxvalues = value;
                OnPropertyChanged("UpdatetLmirrorXvalues");
            }
        }

        private string _mmirrorlyvalues = null;
        [JsonProperty("_mmirrorlyvalues")]
        public string UpdatetLmirroryvalues
        {
            get { return _mmirrorlyvalues; }
            set
            {
                _mmirrorlyvalues = value;
                OnPropertyChanged("UpdatetLmirroryvalues");
            }
        }

        private string _mbrmirrorvalue = null;
        [JsonProperty("_mbrmirrorvalue")]
        public string UpdateBRMirrorXValues
        {
            get { return _mbrmirrorvalue; }
            set
            {
                _mbrmirrorvalue = value;
                OnPropertyChanged("UpdateBRMirrorXValues");
            }
        }

        private string _mbrmirroryvalue = null;
        [JsonProperty("_mbrmirroryvalue")]
        public string UpdateBRMirrorYValues
        {
            get { return _mbrmirroryvalue; }
            set
            {
                _mbrmirroryvalue = value;
                OnPropertyChanged("UpdateBRMirrorYValues");
            }
        }

        private string _mblmirrorxvalues = null;
        [JsonProperty("_mblmirrorxvalues")]
        public string UpdateBLMirrorXValues
        {
            get { return _mblmirrorxvalues; }
            set
            {
                _mblmirrorxvalues = value;
                OnPropertyChanged("UpdateBLMirrorXValues");
            }
        }

        private string _mblmirroryvalues = null;
        [JsonProperty("_mblmirroryvalues")]
        public string UpdateBLMirrorYValues
        {
            get { return _mblmirroryvalues; }
            set
            {
                _mblmirroryvalues = value;
                OnPropertyChanged("UpdateBLMirrorXValues");
            }
        }

        private string _mrmirrorxvalues = null;
        [JsonProperty("_mrmirrorxvalues")]
        public string UpdatetrmirrorXvalues
        {
            get { return _mrmirrorxvalues; }
            set
            {
                _mrmirrorxvalues = value;
                OnPropertyChanged("UpdatetrmirrorXvalues");
            }
        }

        private string _mhomescreenxvalue = null;
        [JsonProperty("_mhomescreenxvalue")]
        public string UpdateHomeScreenXValues
        {
            get { return _mhomescreenxvalue; }
            set
            {
                _mhomescreenxvalue = value;
                OnPropertyChanged("UpdateHomeScreenXValues");
            }
        }

        private string _mhomescreenyvalue = null;
        [JsonProperty("_mhomescreenyvalue")]
        public string UpdateHomeScreenYValues
        {
            get { return _mhomescreenyvalue; }
            set
            {
                _mhomescreenyvalue = value;
                OnPropertyChanged("UpdateHomeScreenYValues");
            }
        }
        string _mOriginalimgslope = null;
        public string OriginalImgslope
        {
            get
            {
                return _mOriginalimgslope;
            }
            set
            {
                _mOriginalimgslope = value;
                OnPropertyChanged("OriginalImgslope");
            }
        }

        private float _mintensityCImg = 0f;
        public float AverageIntensityOfCImg
        {
            get { return _mintensityCImg; }
            set { _mintensityCImg = value; }
        }
        private void FindImageSlope()
        {
            string con = null;
            _mintent = 0;
            int count = 0;

            HoughLineTransformation hlinetransform = new HoughLineTransformation();
            hlinetransform.ProcessImage(_mgrayImg);
            Bitmap bmap = hlinetransform.ToBitmap();
            HoughLine[] lines = hlinetransform.GetLinesByRelativeIntensity(1);

            foreach (HoughLine hline in lines)
            {
                int radius = hline.Radius;
                double th = hline.Theta;
                float intensities = hline.Intensity;
                con = con + "Slope:" + th + ":"+ intensities + "\n";
                _mintent += intensities;
                count++;
            }

            if (_mintent != 0)
                AverageIntensityOfCImg = _mintent / count;

            if (con != null)
                OriginalImgslope = con.ToString();
            OnPropertyChanged("Imgslope");
        }

 
        private void PopulateDataIntoSettingPage(dynamic file)
        {
            //if (frmTestFixture.Instance.tabControl2.SelectedTab.Text == "Main")
            //{
                if ("_mmeanValue" == file.Name)
                {
                GetOrginalImgMeanValue = file.Value; }
                else if ("_mmaxvalue" == file.Name)
                {
                GetOrginalImgMaxValue = file.Value; }
                else if ("_mmedianvalue" == file.Name)
                {
                GetOrginalImgMedianValue = file.Value; }
                else if ("_mstddev" == file.Name)
                {
                GetOrginalImgStdDevValue = file.Value; }
                else if ("_mminvalue" == file.Name)
                {
                GetOrginalImgMinValue = file.Value; }
                else if ("_mbottomleftvoltage" == file.Name)
                {
                ReadBottomLeftVoltage = file.Value; }
                else if ("_mbottomrightvoltage" == file.Name)
                {
                ReadBottomRightVoltage = file.Value; }
                else if ("_mtoprightvoltage" == file.Name)
                {
                ReadTopRightVoltage = file.Value; }
                else if ("_mtopleftvoltage" == file.Name)
                {
                ReadTopLeftVoltage = file.Value; }
                else if ("_mthreshholdsim" == file.Name)
                {
                ThreshHoldSimilarity = file.Value; }
                else if ("_mrmirrorxvalues" == file.Name)
                {
                    UpdatetrmirrorXvalues = file.Value;
                }
                else if ("_mrmirroryvalues" == file.Name)
                {
                    UpdatetrmirrorYvalues = file.Value;
                }
                else if ("_mmirrorlxvalues" == file.Name)
                {
                    UpdatetLmirrorXvalues = file.Value;
                }
                else if ("_mmirrorlyvalues" == file.Name)
                {
                    UpdatetLmirroryvalues = file.Value;
                }
                else if ("_mbrmirrorvalue" == file.Name)
                {
                    UpdateBRMirrorXValues = file.Value;
                }
                else if ("_mbrmirroryvalue" == file.Name)
                {
                    UpdateBRMirrorYValues = file.Value;
                }
                else if ("_mblmirrorxvalues" == file.Name)
                {
                    UpdateBLMirrorXValues = file.Value;
                }
                else if ("_mblmirroryvalues" == file.Name)
                {
                    UpdateBLMirrorYValues = file.Value;
                }
                else if ("_mhomescreenxvalue" == file.Name)
                {
                    UpdateHomeScreenXValues = file.Value;
                }
                else if ("_mhomescreenyvalue" == file.Name)
                {
                    UpdateHomeScreenYValues = file.Value;
                }
            //}
            //else
            //{
            //    frmTestFixture.Instance.WriteToLog("Settings tabs have not been created...", ApplicationConstants.TraceLogType.Warning);
            //}
        }

        private BitmapImage DisplayGrayImage()
        {
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.UriSource = new Uri(@_mfolderPath + "\\" + "OProcessedImg.png",UriKind.Absolute);
                bitmapimage.EndInit();

                return bitmapimage;
            //BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap
            //(
            //    _mconvgrayimg.GetHbitmap(),
            //    IntPtr.Zero,
            //    Int32Rect.Empty,
            //    BitmapSizeOptions.FromEmptyOptions()
            //);
            //return bitmapSource;
            System.Windows.Media.Imaging.BitmapSource b = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(_mconvgrayimg.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(_mconvgrayimg.Width, _mconvgrayimg.Height));
            //return b;
        }
        #endregion
        #region Dummy implementation to satisfy compiler
        public override void ButtonOKClick()
        {
            throw new NotImplementedException();
        }
        #endregion

        [JsonProperty("_mmeanValue1")]
        public float GetOrginalImgMeanValue1
        {
            get
            {
                return _mmeanValue1;
            }
            set
            {
                _mmeanValue1 = value;
                OnPropertyChanged("GetOrginalImgMeanValue1");
            }
        }

        //called to set value for Std Dev in Histogram section
        [JsonProperty("_mstddev1")]
        public float GetOrginalImgStdDevValue1
        {
            get
            {
                return _mstddev1;
            }
            set
            {
                _mstddev1 = value;
                OnPropertyChanged("GetOrginalImgStdDevValue1");
            }
        }

        //called to set value for Median in Histogram section
        [JsonProperty("_mmedianvalue1")]
        public float GetOrginalImgMedianValue1
        {
            get
            {
                return _mmedianvalue1;
            }
            set
            {
                _mmedianvalue1 = value;
                OnPropertyChanged("GetOrginalImgMedianValue1");
            }
        }

        //called to set value of Min in Histogram section
        [JsonProperty("_mminvalue1")]
        public float GetOrginalImgMinValue1
        {
            get
            {
                return _mminvalue1;
            }
            set
            {
                _mminvalue1 = value;
                OnPropertyChanged("GetOrginalImgMinValue1");
            }
        }

        //called to set value of Min in Histogram section
        [JsonProperty("_mmaxvalue1")]
        public float GetOrginalImgMaxValue1
        {
            get
            {
                return _mmaxvalue1;
            }
            set
            {
                _mmaxvalue1 = value;
                OnPropertyChanged("GetOrginalImgMaxValue1");
            }
        }

        public string GetOriginalImageFolderPath()
        {
            return _mfolderPath;
        }


    }
}

