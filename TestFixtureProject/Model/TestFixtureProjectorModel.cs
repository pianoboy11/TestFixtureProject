using AForge.Video;
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
using System.Collections.Generic;
using AForge.Math.Geometry;
using AForge;
using TestFixtureProject.Common;
using System.Windows.Forms;

namespace TestFixtureProject.Model
{
    public class TestFixtureProjectorModel : TestFixtureLoginBaseVM
    {
        #region

        private UnmanagedImage _medgeimg = null;
        private UnmanagedImage _moedgeimg = null;
        private string destgrayimage = null;
        Bitmap _mimg = null;
        BitmapData _mimageData = null;
        BitmapData _moimgdata = null;
        Bitmap _mconvgrayimg = null;
        string _mfolderPath = null;
        Bitmap _mOriginalImg = null;


        double _mmeanValue = 0;
        private int _mmaxvalue = 0;
        private int _mmedianvalue = 0;
        private double _mstddev = 0;
        private int _mminvalue = 0;
        private bool _mcollapsesettingstabitem;
        public TestFixtureProjectorModel(bool canAccessallPages)
        {
            _mfolderPath = TestFixtureConstants.CreateDirectoryIfNotExists("Images");
        }
        #endregion

        #region Imagesource
        private ImageSource _mimagesource;
        public ImageSource ImageSource
        {
            get
            {
                return _mimagesource;
            }
            set
            {
                _mimagesource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        private ImageSource _mgrayimagesource;
        public ImageSource GrayImageSource
        {
            get
            {
                return _mgrayimagesource;
            }
            set
            {
                _mgrayimagesource = value;
                OnPropertyChanged("GrayImageSource");
            }
        }

        #endregion

        #region
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
                //string path1 = @_mfolderPath + "\\" + "CapturedImgFVO.png";
                //_mimagesource = new BitmapImage(new Uri(path1));

                _mimg = ConvertBitmapImgToBitMap();
                ////process original image for comparision
                //string path = @_mfolderPath + "\\" + "OriginalImg.png";
                //ImageSource original = new BitmapImage(new Uri(path));
                //_mOriginalImg = ConvertBitmapImgToBitMap(original);

                //create image file in the specified path
                //pathString = System.IO.Path.Combine(folderPath, "CapturedImg.png");
                File.Delete(@_mfolderPath + "\\" + "PCapturedImg.png");
                _mimg.Save(_mfolderPath + "\\" + "PCapturedImg.png", ImageFormat.Png);
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
                }
                finally
                {
                   // _mimg.UnlockBits(_mimageData);
                }
            }
        }

        private Bitmap ConvertBitmapImgToBitMap()
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
            ImageStatistics statistics = new ImageStatistics(_medgeimg);

            Histogram grayhisto = statistics.Gray;
            GetMaxValue = grayhisto.Max;
            OnPropertyChanged("GetMaxValue");
            GetMinValue = grayhisto.Min;
            OnPropertyChanged("GetMinValue");
            GetMedianValue = grayhisto.Median;
            OnPropertyChanged("GetMedianValue");
            GetStdDevValu = grayhisto.StdDev;
            OnPropertyChanged("GetStdDevValu");
            GetMeanValue = grayhisto.Mean;
            OnPropertyChanged("GetMeanValue");
        }

        private void CompareBothImages()
        {
            //var tempmatch = new ExhaustiveTemplateMatching(ThreshHoldLimit);
            //var results = tempmatch.ProcessImage(_moedgeimg,_medgeimg);
            //float value = results[0].Similarity;
        }
        public void LoadConvertedImages()
        {
            string fullpath = _mfolderPath + "\\" + "CProcessedImg.png";
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = fullpath;
            ImageSource = new BitmapImage(new Uri(dlg.FileName));
            OnPropertyChanged("GrayImageSource");
        }
        private float _mThreshHoldLimit = 0f;
        public float ThreshHoldLimit
        {
            get { return _mThreshHoldLimit; }
            set
            {
                _mThreshHoldLimit = value;
                OnPropertyChanged("ThreshHoldLimit");
            }
        }
        private void FindImageSlope()
        {
            string con = null;
            int count = 0;
            HoughLineTransformation hlinetransform = new HoughLineTransformation();
            hlinetransform.ProcessImage(_medgeimg);
            Bitmap bmap = hlinetransform.ToBitmap();
            HoughLine[] lines = hlinetransform.GetLinesByRelativeIntensity(0.5);
            double theta = 0;
            int length = 0;
            length = hlinetransform.LinesCount;
            foreach (HoughLine hline in lines)
            {
                int radius = hline.Radius;
                double th = hline.Theta;
                float intent = hline.Intensity;
                con = con + "Slope:" + th + ":" + intent + "\n";
                theta += th;
                count++;
            }
            if (con != null)
                  Imgslope = con.ToString();
                  Imgslope = (theta / count).ToString();
            OnPropertyChanged("Imgslope");
        }
        //private void FindIntensityofImage()
        //{

        //}
        //public void video_new_fram(object sender, NewFrameEventArgs args)
        //{
        //    Bitmap bitimag = (Bitmap)args.Frame.Clone();
        //    //do processing here
        //}
        //#region

        string _mimgslope = null;
        public string Imgslope
        {
            get
            {
                return _mimgslope;
            }
            set
            {
                _mimgslope = value;

                if (frmTestFixture.Instance != null)
                {
                    if (frmTestFixture.Instance.txtImageSlope.IsHandleCreated)
                    {
                        frmTestFixture.Instance.txtImageSlope.Text = _mimgslope.ToString();
                    }
                }

                OnPropertyChanged("Imgslope");
            }
        }
        ////called to set value for Min in Histogram section
        public double GetMeanValue
        {
            get
            {
                return System.Math.Round(_mmeanValue, 2);
            }
            set
            {
                _mmeanValue = value;

                if (frmTestFixture.Instance != null)
                {
                    if (frmTestFixture.Instance.txtGetMeanValue.IsHandleCreated)
                    {
                        frmTestFixture.Instance.txtGetMeanValue.Text = _mmeanValue.ToString();
                    }
                }

                OnPropertyChanged("GetMeanValue");
            }
        }

        //called to set value for Std Dev in Histogram section
        public double GetStdDevValu
        {
            get
            {
                return System.Math.Round(_mstddev, 2);
            }
            set
            {
                _mstddev = value;

                if (frmTestFixture.Instance != null)
                {
                    if (frmTestFixture.Instance.txtGetStdDevValue.IsHandleCreated)
                    {
                        frmTestFixture.Instance.txtGetStdDevValue.Text = _mstddev.ToString();
                    }
                }

                OnPropertyChanged("GetStdDevValu");
            }
        }

        //called to set value for Median in Histogram section
        public int GetMedianValue
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
                    if (frmTestFixture.Instance.txtGetMedianValue.IsHandleCreated)
                    {
                        frmTestFixture.Instance.txtGetMedianValue.Text = _mmedianvalue.ToString();
                    }
                }
                OnPropertyChanged("GetMedianValue");
            }
        }

        //called to set value of Min in Histogram section
        public int GetMinValue
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
                    if (frmTestFixture.Instance.txtGetMinValue.IsHandleCreated)
                    {
                        frmTestFixture.Instance.txtGetMinValue.Text = _mminvalue.ToString();
                    }
                }

                OnPropertyChanged("GetMinValue");
            }
        }

        //called to set value of Min in Histogram section
        public int GetMaxValue
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
                    if (frmTestFixture.Instance.txtGetMaxValue.IsHandleCreated)
                    {
                        frmTestFixture.Instance.txtGetMaxValue.Text = _mmaxvalue.ToString();
                    }
                }

                OnPropertyChanged("GetMaxValue");
            }
        }

        private UnmanagedImage _mUnmanagedImg;
        public UnmanagedImage SetOriginalImg
        {
            get { return _mUnmanagedImg; }
            set
            {
                _mUnmanagedImg = value;
                OnPropertyChanged("SetOriginalImg");
            }
        }

        //#endregion

        //public void FindSquareFiducials()
        //{
        //    try
        //    {
        //        BlobCounter blobcounter = new BlobCounter();
        //        blobcounter.ProcessImage(_mconvgrayimg);

        //        Blob[] blobs = blobcounter.GetObjectsInformation();

        //        GrahamConvexHull hullFinde = new GrahamConvexHull();
        //        BitmapData bData = _mconvgrayimg.LockBits(new Rectangle(0, 0, _mconvgrayimg.Width, _mconvgrayimg.Height), ImageLockMode.ReadWrite, _mconvgrayimg.PixelFormat);
        //        foreach (Blob blob in blobs)
        //        {
        //            List<AForge.IntPoint> leftPoints = new List<IntPoint>();
        //            List<AForge.IntPoint> rightPoints = new List<IntPoint>();
        //            List<AForge.IntPoint> edgePoints = new List<IntPoint>();

        //            List<AForge.IntPoint> topPoints = new List<IntPoint>();
        //            List<AForge.IntPoint> bottomPoints = new List<IntPoint>();

        //            // get blob's edge points
        //            blobcounter.GetBlobsLeftAndRightEdges(blob, out leftPoints, out rightPoints);
        //            blobcounter.GetBlobsTopAndBottomEdges(blob, out topPoints, out bottomPoints);

        //            edgePoints.AddRange(leftPoints);
        //            edgePoints.AddRange(rightPoints);

        //            // blob's convex hull
        //            List<IntPoint> hull = hullFinde.FindHull(edgePoints);

        //            AForge.Imaging.Drawing.Polygon(bData, hull, System.Drawing.Color.Red);
        //        }

        //        //pass the bitmap image
        //    }
        //    catch (Exception e)
        //    {

        //        string exp = e.ToString();
        //    }
        //}

        //#endregion

        #region create the folder if doesnt exist
        private string CreateDirectoryIfNotExists(string folderName)
        {
            string new_path = string.Empty;

            try
            {
                if (!Directory.Exists(folderName))
                {
                    //new_path = Environment.CurrentDirectory + "\\" + folderName;
                    new_path = Environment.CurrentDirectory + "\\" + folderName;
                    Directory.CreateDirectory(new_path);
                    Directory.SetCurrentDirectory(new_path);
                }
                else
                {
                    frmTestFixture.Instance.WriteToLog("Directory Exists: '" + folderName + "'", ApplicationConstants.TraceLogType.Information);
                }
            }
            catch(Exception e)
            {
                frmTestFixture.Instance.WriteToLog("CreateDirectoryIfNotExists Error: " + e.Message, ApplicationConstants.TraceLogType.Information);
                frmTestFixture.Instance.WriteToLog("CreateDirectoryIfNotExists Error: '" + folderName + "'", ApplicationConstants.TraceLogType.Information);
                return string.Empty;
            }

            return new_path;
        }
        #endregion

        #region Dummy implementation to satisfy compiler
        public override void ButtonOKClick()
        {
            throw new NotImplementedException();
        }
        #endregion


        //#region Access Visibility
        //public bool CanAccessProjectorPage
        //{
        //    get { return _mcollapsesettingstabitem; }
        //}
        //#endregion

        //   #region
        //private float _mtoprightvoltage = 0;
        //public float ReadTopRightVoltage
        //{
        //    get { return _mtoprightvoltage; }
        //    set
        //    {
        //        _mtoprightvoltage = value;
        //        OnPropertyChanged("ReadTopRightVoltage");
        //    }
        //}

        //private float _mtopleftvoltage = 0;
        //public float ReadTopLeftVoltage
        //{
        //    get { return _mtoprightvoltage; }
        //    set
        //    {
        //        _mtopleftvoltage = value;
        //        OnPropertyChanged("ReadTopLeftVoltage");
        //    }
        //}

        //private float _mbottomrightvoltage = 0;
        //public float ReadBottomRightVoltage
        //{
        //    get { return _mtoprightvoltage; }
        //    set
        //    {
        //        _mbottomrightvoltage = value;
        //        OnPropertyChanged("ReadBottomRightVoltage");
        //    }
        //}

        //private float _mbottomleftvoltage = 0;
        //public float ReadBottomLeftVoltage
        //{
        //    get { return _mbottomleftvoltage; }
        //    set
        //    {
        //        _mbottomleftvoltage = value;
        //        OnPropertyChanged("ReadBottomLeftVoltage");
        //    }
        //}
        #endregion
    }
}
