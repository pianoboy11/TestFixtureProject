using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.IO;

namespace OmniDriverCSharpDemo
{
    public partial class Form1 : Form
    {
        OmniDriver.NETWrapper wrapper = null;
        OOI.OEM.Logger logger = new OOI.OEM.Logger();

        int numberOfSpectrometers = 0;
        List<int> specIndices = new List<int>();

        Dictionary<int, int> integrationTimesMillisec = null;
        Dictionary<int, int> pixels = null;
        Dictionary<int, bool> lampEnabled = null;
        Dictionary<int, double> collectionAreas = null;
        Dictionary<int, double[]> wavelengths = null;
        Dictionary<int, double[]> newestSpectrum = null;
        Dictionary<int, double[]> graphableSpectrum = null;
        Dictionary<int, string> serialNumbers = null;
        Dictionary<int, string> modelNames = null;
        Dictionary<int, Series> graphSeries = null;
        Dictionary<int, int> saveCounts = null;
        Dictionary<int, BackgroundWorker> backgroundWorkers = null;
        Dictionary<int, DateTime> lastScanTimes = null;
        Dictionary<int, OmniDriver.NETGPIO> gpios = null;
        Dictionary<int, List<Tuple<DateTime, double[]>>> bufferedSpectra = null;

        HashSet<int> scanning = null;
        bool closePending = false;

        string saveBaseFilename = null;
        int maxScans = 0;
        bool savingAll = false;
        int scanCount = 0;

        System.Windows.Forms.Timer graphTimer = null;
        System.Windows.Forms.Timer saveTimer = null;
        Mutex mut = new Mutex();

        ////////////////////////////////////////////////////////////////////////
        // Initialization
        ////////////////////////////////////////////////////////////////////////

        public Form1()
        {
            InitializeComponent();

            // initialize logger
            logger.setTextBox(textBoxEventLog);

            // get handle to OmniDriver and SPAM
            wrapper = new OmniDriver.NETWrapper();

            // tooltips
            toolTip1.SetToolTip(checkBoxEnableDataCollection, "Save acquisitions to a CSV file");
            toolTip1.SetToolTip(numericUpDownMaxScans, "Stop acquisition after this many scans (0 = run forever)");
            toolTip1.SetToolTip(checkBoxSaveAll, "Save EVERY acquisition (buffers in memory first, will crash if run too long)");

            // timer to drive the graph (often at a lower rate than acquisitions)
            graphTimer = new System.Windows.Forms.Timer();
            graphTimer.Interval = 100; // update graph at 10Hz
            graphTimer.Tick += graphTimer_Tick;

            saveTimer = new System.Windows.Forms.Timer();
            saveTimer.Interval = 1000; // save 1Hz
            saveTimer.Tick += saveTimer_Tick;
            numericUpDownSaveEverySec.Value = saveTimer.Interval / 1000;

            initializeSpectrometers();
            updateSpectrometersFromGUI();
        }

        private void initializeSpectrometers()
        {
            // assume initialization will fail
            btnScan.Enabled = false;
            checkBoxEnableIrradiance.Enabled = false;

            // support re-entry
            if (numberOfSpectrometers > 0)
                wrapper.closeAllSpectrometers();

            // initialize everything
            specIndices = null;
            pixels = null;
            wavelengths = null; 
            newestSpectrum = null;
            graphableSpectrum = null;
            serialNumbers = null;
            modelNames = null; 
            backgroundWorkers = null;
            graphSeries = null;
            chartSpectrum.Series.Clear();
            saveCounts = null;
            scanning = null;
            gpios = null;
            bufferedSpectra = null;

            wrapper.openAllSpectrometers();
            numberOfSpectrometers = wrapper.getNumberOfSpectrometersFound();
            logger.display("Found {0} spectrometers", numberOfSpectrometers);
            if (numberOfSpectrometers < 1)
            {
                logger.display("ERROR: no spectrometers found");
                return;
            }

            specIndices       = new List<int>();
            scanning          = new HashSet<int>();
            pixels            = new Dictionary<int, int>();
            lampEnabled       = new Dictionary<int, bool>();
            saveCounts        = new Dictionary<int, int>();
            collectionAreas   = new Dictionary<int, double>();
            wavelengths       = new Dictionary<int, double[]>();
            newestSpectrum    = new Dictionary<int, double[]>();
            graphableSpectrum = new Dictionary<int, double[]>();
            serialNumbers     = new Dictionary<int, string>();
            modelNames        = new Dictionary<int, string>();
            graphSeries       = new Dictionary<int, Series>();
            backgroundWorkers = new Dictionary<int, BackgroundWorker>();
            integrationTimesMillisec = new Dictionary<int, int>();
            lastScanTimes = new Dictionary<int, DateTime>();
            gpios = new Dictionary<int, OmniDriver.NETGPIO>();
            bufferedSpectra = new Dictionary<int, List<Tuple<DateTime, double[]>>>();

            for (int specIndex = 0; specIndex < numberOfSpectrometers; specIndex++)
            {
                specIndices.Add(specIndex);

                // initialize metadata for each spectrometer
                integrationTimesMillisec.Add(specIndex, 100);
                serialNumbers       .Add(specIndex, wrapper.getSerialNumber(specIndex));
                modelNames          .Add(specIndex, wrapper.getName(specIndex));
                pixels              .Add(specIndex, wrapper.getNumberOfPixels(specIndex));
                newestSpectrum      .Add(specIndex, new double[pixels[specIndex]]);
                graphableSpectrum   .Add(specIndex, new double[pixels[specIndex]]);
                backgroundWorkers   .Add(specIndex, new BackgroundWorker());
                lampEnabled         .Add(specIndex, false);
                wavelengths         .Add(specIndex, wrapper.getWavelengths(specIndex));
                bufferedSpectra.Add(specIndex, new List<Tuple<DateTime,double[]>>());
                saveCounts[specIndex] = 0;

                // apply reasonable defaults
                logger.log("setting defaults for spectrometer {0}", specIndex);
                wrapper.setCorrectForElectricalDark(specIndex, 1);
                wrapper.setCorrectForDetectorNonlinearity(specIndex, 1);
                wrapper.setStrobeEnable(specIndex, 0);

                if (wrapper.isFeatureSupportedGPIO(specIndex) != 0)
                {
                    gpios.Add(specIndex, wrapper.getFeatureControllerGPIO(specIndex));

                    // initialize everything to zero (easiest way to match starting checkbox state)
                    OmniDriver.NETBitSet allBits = new OmniDriver.NETBitSet();
                    allBits.set(0);
                    gpios[specIndex].setDirectionAllBits(allBits);
                    gpios[specIndex].setMuxAllBits(allBits);
                    gpios[specIndex].setValueAllBits(allBits);
                }
                else
                {
                    gpios.Add(specIndex, null);
                }

                // create graph series
                logger.log("configuring graph for spectrometer {0}", specIndex);
                graphSeries.Add(specIndex, new Series());
                graphSeries[specIndex].ChartType = SeriesChartType.Line;
                graphSeries[specIndex].Name = String.Format("{0}: {1} {2}", specIndex, modelNames[specIndex], serialNumbers[specIndex]);
                chartSpectrum.Series.Add(graphSeries[specIndex]);

                // create a background thread for each spectrometer (allows each to run at their own integration time, etc)
                logger.log("configuring BackgroundWorker for spectrometer {0}", specIndex);
                backgroundWorkers[specIndex].WorkerReportsProgress = true;
                backgroundWorkers[specIndex].WorkerSupportsCancellation = true;
                backgroundWorkers[specIndex].DoWork += backgroundWorker_DoWork;
                backgroundWorkers[specIndex].ProgressChanged += backgroundWorker_ProgressChanged;
                backgroundWorkers[specIndex].RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

                string firmwareVersion = wrapper.getFirmwareVersion(specIndex);

                double startWavelength = -1;
                double lastWavelength = -1;
                if (wavelengths[specIndex] != null)
                {
                    startWavelength = wavelengths[specIndex][0];
                    lastWavelength = wavelengths[specIndex][pixels[specIndex] - 1];
                }

                logger.display("Spectrometer at index {0} is a/n {1} with serial {2}, firmware {3} and {4} pixels from {5:0.00} to {6:0.00}nm",
                    specIndex, modelNames[specIndex], serialNumbers[specIndex], firmwareVersion,
                    pixels[specIndex], startWavelength, lastWavelength);
            }

            btnScan.Enabled = true;
            Refresh();
        }

        ////////////////////////////////////////////////////////////////////////
        // Methods
        ////////////////////////////////////////////////////////////////////////

        private void updateSpectrometersFromGUI()
        {
            if (specIndices == null)
                return;

            foreach (int specIndex in specIndices)
            {
                wrapper.setScansToAverage(specIndex, (int)numericUpDownScansToAverage.Value);
                wrapper.setBoxcarWidth(specIndex, (int)numericUpDownSmoothingBoxcar.Value);

                wrapper.setCorrectForElectricalDark(specIndex, checkBoxEnableEDC.Checked ? 1 : 0);
                wrapper.setCorrectForDetectorNonlinearity(specIndex, checkBoxEnableNLC.Checked ? 1 : 0);

                wrapper.setStrobeEnable(specIndex, checkBoxStrobeEnable.Checked ? 1 : 0);

                int integrationTimeMillisec = (int)numericUpDownIntegrationTimeMillisec.Value;
                if (integrationTimeMillisec <= 0)
                    integrationTimeMillisec = integrationTimesMillisec[specIndex];
                wrapper.setIntegrationTime(specIndex, integrationTimeMillisec * 1000);
            }
        }

        // Perform absolute irradiance calibration for spectrometer.
        //
        // S:  Sample spectrum (counts per nanometer)
        // D:  Dark spectrum (counts per nanometer, with the same integration time, 
        //     corrections, and smoothing as sample)
        // C:  Calibration (represented in micro-Joules per count)
        // T:  Integration time (represented here in seconds)
        // A:  Collection area (represented in square centimeters) unless the light 
        //     source is entirely inside an integrating sphere
        // dL: The wavelength spread (how many nanometers a given pixel represents)
        //
        // Absolute irradiance (I) is computed as follows.  Below, the subscript P 
        // will indicate a particular pixel for I, dL, S, D, and C.  Thus, SP refers 
        // to pixel P of the sample spectrum.
        //
        //      IP = (SP - DP) * CP / (T * A * dLP)
        //
        // Note that if the lamp is entirely enclosed within an integrating sphere, 
        // then the A term is omitted.
        //
        // dL is typically computed as follows, where L(P) refers to the wavelength 
        // represented by the center of pixel index P.
        //
        //      dL = [L(P + 1) - L(P - 1)] / 2
        //
        private double[] computeAbsoluteIrradiance(
            double[] wl, double[] spectrum, double[] cal, double collectionArea, int integrationTimeMillisec)
        {
            int pixelCount = spectrum.Length;

            double[] irradiance = new double[pixelCount];
            double[] dLp = new double[pixelCount];

            // Calculate dLp
            dLp[0] = wl[1] - wl[0];
            for (int i = 1; i < pixelCount - 1; i++)
                dLp[i] = (wl[i + 1] - wl[i - 1]) / 2.0;
            dLp[pixelCount - 1] = wl[pixelCount - 1] - wl[pixelCount - 2];

            // compute irradiance
            for (int i = 0; i < pixelCount; i++)
            {
                irradiance[i] = (spectrum[i] * cal[i])
                              / (integrationTimeMillisec * 1000.0 * collectionArea * dLp[i]);
                if (Double.IsNaN(irradiance[i]) || Double.IsInfinity(irradiance[i]))
                    irradiance[i] = 0;
            }

            return irradiance;
        }

        ////////////////////////////////////////////////////////////////////////
        // Graphing
        ////////////////////////////////////////////////////////////////////////

        private void updateGraph()
        {
            chartSpectrum.ChartAreas[0].AxisX.LabelStyle.Format = "F2";
            try
            {
                foreach (int specIndex in specIndices)
                {
                    Series series = graphSeries[specIndex];
                    if (series == null || 
                        wavelengths[specIndex] == null || 
                        graphableSpectrum[specIndex] == null || 
                        graphableSpectrum[specIndex].Length != pixels[specIndex])
                    {
                        logger.display("ERROR: Can't graph index {0}", specIndex);
                        continue;
                    }

                    series.Points.Clear();
                    for (int i = 0; i < pixels[specIndex]; i++)
                        series.Points.AddXY(wavelengths[specIndex][i], graphableSpectrum[specIndex][i]);
                }
            }
            catch (Exception e)
            {
                logger.display("ERROR: caught exception during graphing: {0}", e);
            }

            if (computeNoise)
                updateNoiseComputation();

            Refresh();
        }

        ////////////////////////////////////////////////////////////////////////
        // GUI Events
        ////////////////////////////////////////////////////////////////////////

        private void btnScan_Click(object sender, EventArgs e)
        {
            if (scanning.Count == 0)
            {
                logger.display("Start scanning");
                btnScan.Text = "Stop";
                buttonOptimizeIntegrationTimes.Enabled = false;

                scanning.Clear();
                foreach (int specIndex in specIndices)
                {
                    scanning.Add(specIndex);
                    backgroundWorkers[specIndex].RunWorkerAsync(specIndex);
                }
                graphTimer.Start();
            }
            else
            {
                logger.display("Stop scanning");
                graphTimer.Stop();
                logger.log("Cancelling workers");
                foreach (KeyValuePair<int, BackgroundWorker> entry in backgroundWorkers)
                    entry.Value.CancelAsync();
            }
        }

        private void graphTimer_Tick(object sender, EventArgs e)
        {
            if (scanning.Count == 0)
                return;

            mut.WaitOne();
            foreach (int specIndex in specIndices)
                Array.Copy(newestSpectrum[specIndex], graphableSpectrum[specIndex], pixels[specIndex]);
            mut.ReleaseMutex();

            updateGraph();
        }

        private void saveTimer_Tick(object sender, EventArgs e)
        {
            if (!checkBoxEnableDataCollection.Checked)
                return;

            if (scanning.Count == 0)
                return;

            if (!checkBoxSaveAll.Checked)
            {
                mut.WaitOne();
                foreach (int specIndex in specIndices)
                    saveSpectrum(specIndex);
                mut.ReleaseMutex();
                labelSaveCount.Text = String.Format("{0}", saveCounts[0]);
            }
            else
            {
                int max = 0;
                foreach (int specIndex in specIndices)
                {
                    int count = bufferedSpectra[specIndex].Count;
                    if (max < count)
                        max = count;
                }
                labelSaveCount.Text = String.Format("{0}", max);
            }

            updateGraph();
        }

        ////////////////////////////////////////////////////////////////////////
        // Background Worker
        ////////////////////////////////////////////////////////////////////////

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int specIndex = (int) e.Argument;
            logger.log("[DoWork:{0}] starting...", specIndex);
            if (specIndex < 0 || wrapper == null)
            {
                logger.push("[DoWork:{0}] ERROR: can't start acquisition (missing spectrometer or OmniDriver)", specIndex);
                return;
            }

            BackgroundWorker worker = sender as BackgroundWorker;
            scanCount = 0;

            while (true)
            {
                scanCount++;
                double[] newRaw = null;

                try
                {
                    lastScanTimes[specIndex] = DateTime.Now;
                    newRaw = wrapper.getSpectrum(specIndex);

                    // logger.log("  spectrum {0} valid", 0 == wrapper.isSpectrumValid(specIndex) ? "is" : "is NOT");

                    if (wavelengths[specIndex] == null)
                        newRaw = null;

                    if (savingAll)
                    {
                        Tuple<DateTime, double[]> entry = new Tuple<DateTime, double[]>(DateTime.Now, newRaw);
                        bufferedSpectra[specIndex].Add(entry);
                    }
                    else if (newRaw != null)
                    {
                        // only log this if we're not in high-speed mode
                        double min = 999999;
                        double max = -99999;
                        for (int i = 0; i < newRaw.Length; i++ )
                        {
                            min = newRaw[i] < min ? newRaw[i] : min;
                            max = newRaw[i] > max ? newRaw[i] : max;
                        }
                        logger.log("  spectrum {0} has {1} pixels ({2}, {3})", scanCount, newRaw.Length, min, max);
                    }
                    else
                    {
                        logger.log("  spectrum is NULL!");
                    }

                }
                catch (Exception ex)
                {
                    logger.push("[DoWork:{0}] Caught exception during acquisition: {1}", specIndex, ex);
                    break;
                }

                if (newRaw == null)
                {
                    logger.push("[DoWork:{0}] Error taking acquisition (newRaw null)", specIndex);
                    break;
                }

                if (newRaw.Length != pixels[specIndex])
                {
                    logger.push("[DoWork:{0}] Error taking acquisition (length {1} != pixels {2})", specIndex, newRaw.Length, pixels[specIndex]);
                    break;
                }

                // copy to graphable buffer

                mut.WaitOne();
                Array.Copy(newRaw, newestSpectrum[specIndex], pixels[specIndex]);
                mut.ReleaseMutex();

                // report progress
                worker.ReportProgress(scanCount, specIndex);

                // quit if we've completed our scan count
                if (maxScans > 0 && scanCount >= maxScans)
                {
                    logger.log("[DoWork:{0}] Stopping after {0} scans", specIndex, scanCount);
                    break;
                }

                // end thread if we've been asked to cancel
                if (worker.CancellationPending)
                {
                    logger.log("[DoWork:{0}] closing", specIndex);
                    e.Cancel = true;
                    break;
                }
            }

            // this worker is done
            if (scanning.Contains(specIndex))
                scanning.Remove(specIndex);

            logger.log("[DoWork:{0}] done", specIndex);
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int specIndex = (int) e.UserState;
            logger.flush();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // can't seem to obtain specIndex here from e.Result

            logger.flush();
            logger.log("[RunWorkerComplete] worker cleanup");

            // was that the last worker?
            if (scanning.Count == 0)
            {
                // that was the last worker
                logger.log("All workers complete");
                if (closePending)
                {
                    logger.log("Triggering shutdown");
                    cleanShutdown();
                }
                else
                {
                    btnScan.Text = "Start";
                    buttonOptimizeIntegrationTimes.Enabled = true;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Safe Shutdown (ensure "[x] Close box" releases spectrometers)
        ////////////////////////////////////////////////////////////////////////

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if(scanning != null && scanning.Count > 0)
            {
                // if any workers are running, tell them to quit, then retrigger shutdown
                e.Cancel = true;
                closePending = true;
                logger.log("[OnFormClosing] signalling acquisition worker to quit");
                foreach (int specIndex in specIndices)
                    backgroundWorkers[specIndex].CancelAsync();
            }
            else if (!closePending)
            {
                // otherwise, since nothing is running, shutdown now
                logger.log("[OnFormClosing] shutting down directly");
                cleanShutdown();
            }
        }

        private void cleanShutdown()
        {
            logger.log("[CleanShutdown] shutdown initiated");

            if (scanning != null && scanning.Count > 0)
            {
                logger.log("[CleanShutdown] signalling all workers to exit");
                foreach (KeyValuePair<int, BackgroundWorker> entry in backgroundWorkers)
                    entry.Value.CancelAsync();

                while (scanning.Count != 0)
                {
                    logger.log("Waiting for all workers to complete");
                    Thread.Sleep(500);
                }
            }

            // block further user input
            this.Enabled = false;

            // shutdown spectrometer
            wrapper.closeAllSpectrometers();

            // shutdown app
            if (closePending)
                this.Close();
        }

        ////////////////////////////////////////////////////////////////////////
        // GUI Events
        ////////////////////////////////////////////////////////////////////////

        private void numericUpDownIntegrationTimeMillisec_ValueChanged(object sender, EventArgs e) { updateSpectrometersFromGUI(); } 
        private void numericUpDownScansToAverage_ValueChanged(object sender, EventArgs e) { updateSpectrometersFromGUI(); } 
        private void numericUpDownTimeoutMillisec_ValueChanged(object sender, EventArgs e) { updateSpectrometersFromGUI(); } 
        private void numericUpDownSmoothingBoxcar_ValueChanged(object sender, EventArgs e) { updateSpectrometersFromGUI(); } 
        private void checkBoxEnableEDC_CheckedChanged(object sender, EventArgs e) { updateSpectrometersFromGUI(); }
        private void checkBoxEnableNLC_CheckedChanged(object sender, EventArgs e) { updateSpectrometersFromGUI(); }
        private void checkBoxStrobeEnable_CheckedChanged(object sender, EventArgs e) { updateSpectrometersFromGUI(); }

        private void numericUpDownMaxScans_ValueChanged(object sender, EventArgs e)
        {
            maxScans = (int) numericUpDownMaxScans.Value;
        }

        ////////////////////////////////////////////////////////////////////////
        // Save spectra
        ////////////////////////////////////////////////////////////////////////

        private void checkBoxEnableDataCollection_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEnableDataCollection.Checked)
            {
                if (saveFileDialogSaveAll.ShowDialog() == DialogResult.OK)
                {
                    saveBaseFilename = saveFileDialogSaveAll.FileName;
                    string filename = Path.GetFileName(saveBaseFilename);
                    if (filename.LastIndexOf('.') == -1)
                        saveBaseFilename += ".csv";
                    saveTimer.Start();
                }
                else
                {
                    saveBaseFilename = null;
                    checkBoxEnableDataCollection.Checked = false;
                }
            }
            else
            {
                saveTimer.Stop();
                saveBaseFilename = null;
            }
        }

        private void saveSpectrum(int specIndex)
        {
            if (!checkBoxEnableDataCollection.Checked 
                || saveBaseFilename == null 
                || newestSpectrum.Count < specIndex + 1
                || newestSpectrum[specIndex] == null
                || wavelengths.Count < specIndex + 1 
                || wavelengths[specIndex] == null)
                return;

            saveCounts[specIndex]++;

            string directory = Path.GetDirectoryName(saveBaseFilename);
            string filename = Path.GetFileName(saveBaseFilename);
            int pos = filename.LastIndexOf('.');
            filename = filename.Insert(pos, String.Format("-{0}-{1:00000000}", specIndex, saveCounts[specIndex]));
            string pathname = Path.Combine(directory, filename);
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathname))
            {
                file.WriteLine("Model,{0}", modelNames[specIndex]);
                file.WriteLine("Serial,{0}", serialNumbers[specIndex]);
                file.WriteLine("Integration Time,{0}", integrationTimesMillisec[specIndex]);
                file.WriteLine("Time,{0}", lastScanTimes[specIndex].ToString("yyyyMMdd-HHmmss"));
                file.WriteLine();
                file.WriteLine("{0},{1}", "Wavelength", "Count");
                for (int i = 0; i < pixels[specIndex]; i++)
                    file.WriteLine("{0:0.00},{1:0.00000}", wavelengths[specIndex][i], newestSpectrum[specIndex][i]);
            }
            logger.display("saved {0}", pathname);
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            initializeSpectrometers();
            updateSpectrometersFromGUI();
        }

        private void buttonMergeSaved_Click(object sender, EventArgs e)
        {
            if (checkBoxSaveAll.Checked)
                flushToDisk();
            else
                mergeSaved();
        }

        private void flushToDisk()
        {
            foreach (int specIndex in specIndices)
            {
                if (bufferedSpectra[specIndex] == null)
                    continue;

                string directory = Path.GetDirectoryName(saveBaseFilename);
                string filename = Path.GetFileName(saveBaseFilename);
                int pos = filename.LastIndexOf('.');
                filename = filename.Insert(pos, String.Format("-{0}", specIndex));
                string pathname = Path.Combine(directory, filename);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(pathname))
                {
                    foreach (double wl in wavelengths[specIndex])
                        file.Write(",{0:f2}", wl);
                    file.WriteLine();
                    foreach (Tuple<DateTime, double[]> pair in bufferedSpectra[specIndex])
                    {
                        file.Write("{0}", pair.Item1.ToString("yyyyMMdd-HHmmss.fff"));
                        foreach (double intensity in pair.Item2)
                            file.Write(",{0:f2}", intensity);
                        file.WriteLine();
                    }
                }
                bufferedSpectra[specIndex] = new List<Tuple<DateTime, double[]>>();
            }
        }
                
        private void mergeSaved()
        {
            folderBrowserDialogMergeFolder.ShowDialog();
            string folder = folderBrowserDialogMergeFolder.SelectedPath;
            List<string> infileNames = new List<string>(System.IO.Directory.EnumerateFiles(folder, "*.csv"));
            if (infileNames.Count < 1)
                return;

            string outfileName = folder + "\\" + "merged.csv";
            logger.display("Merging {0} CSV files in {1} to {2}", infileNames.Count, folder, outfileName);
            
            // allocate storage to temporarily load all files in selected folder
            List<double> mergeWavelengths = new List<double>();
            List<List<double>> mergeIntensities = new List<List<double>>();
            int outPixels = 0;
            int fileCount = 0;
            char[] delim = { ',' };

            // load data
            foreach (string infileName in infileNames)
            {
                logger.display("  Loading {0}", infileName);
                List<double> intensities = new List<double>();
                TextReader tr = new StreamReader(infileName);
                string line = tr.ReadLine();
                while (line != null)
                {
                    if (line.Length > 0 && Char.IsDigit(line[0]))
                    {
                        string[] values = line.Split(delim);
                        if (fileCount == 0)
                        {
                            mergeWavelengths.Add(Convert.ToDouble(values[0]));
                            outPixels++;
                        }
                        intensities.Add(Convert.ToDouble(values[1]));
                    }
                    line = tr.ReadLine();
                }
                tr.Close();
                mergeIntensities.Add(intensities);
                fileCount++;
            }

            // write data
            using (System.IO.StreamWriter outfile = new System.IO.StreamWriter(outfileName))
            {
                // header row
                outfile.Write("{0}", "Wavelength");
                foreach (string infileName in infileNames)
                {
                    string filename = Path.GetFileName(infileName);
                    outfile.Write(",{0}", filename);
                }
                outfile.WriteLine();

                // data rows
                for (int pixel = 0; pixel < outPixels; pixel++)
                {
                    outfile.Write("{0:0.00}", mergeWavelengths[pixel]);
                    for (int i = 0; i < fileCount; i++)
                        outfile.Write(",{0:0.00}", mergeIntensities[i][pixel]);
                    outfile.WriteLine();
                }
            }

            logger.display("Merge complete ({0} spectra merged to {1})", infileNames.Count, outfileName);
        }

        private void buttonOptimizeIntegrationTimes_Click(object sender, EventArgs e)
        {
            mut.WaitOne();

            updateGraph();
            this.Enabled = false;
            Cursor = Cursors.WaitCursor;
            foreach (int specIndex in specIndices)
                integrationTimesMillisec[specIndex] = determineOptimalIntegrationTime(specIndex);
            mut.ReleaseMutex();
            numericUpDownIntegrationTimeMillisec.Value = 0;
            Cursor = Cursors.Default;
            this.Enabled = true;
        }

        private int determineOptimalIntegrationTime(int specIndex)
        {
            logger.log("optimizing integration time({0})", specIndex);

            const int MAX_TRIES = 30;
            const int MAX_SANE_INTEGRATION_TIME_SEC = 10;
            const int TARGET_COUNTS = 55000;
            const double TARGET_COUNT_ERROR_MARGIN = 0.05;
            const int TARGET_COUNT_TOLERANCE = (int)(TARGET_COUNTS * TARGET_COUNT_ERROR_MARGIN);
            const int DEFAULT_INTEGRATION_TIME_MILLISEC = 100;

            // start search at 100 milliseconds
            int currentIntegrationTimeMillisec = DEFAULT_INTEGRATION_TIME_MILLISEC;
            logger.display("optimizing integration time for spectrometer {0} ({1}) (goal {2} counts +/- {3}%, max {4} iterations)",
                specIndex,
                String.Format("{0}:{1}", modelNames[specIndex], serialNumbers[specIndex]),
                TARGET_COUNTS,
                TARGET_COUNT_ERROR_MARGIN * 100,
                MAX_TRIES);
            wrapper.setIntegrationTime(specIndex, currentIntegrationTimeMillisec * 1000);

            double maxCounts = 0;
            bool found = false;
            int prevIntegrationTime = currentIntegrationTimeMillisec;
            int repeatCount = 0;

            for (int pass = 0; !found && pass < MAX_TRIES; pass++)
            {
                double[] spectrum = wrapper.getSpectrum(specIndex);
                Array.Copy(spectrum, graphableSpectrum[specIndex], pixels[specIndex]);
                updateGraph();

                maxCounts = 5;
                for (int i = 0; i < spectrum.Length; i++)
                    maxCounts = spectrum[i] > maxCounts ? spectrum[i] : maxCounts;
                logger.display("  spec {0} pass {1}: integration time {2}ms = max {3} counts",
                    specIndex, pass + 1, currentIntegrationTimeMillisec, (int)maxCounts);

                if (maxCounts >= (TARGET_COUNTS - TARGET_COUNT_TOLERANCE) &&
                    maxCounts <= (TARGET_COUNTS + TARGET_COUNT_TOLERANCE))
                {
                    found = true;
                    break;
                }

                int prevIntegrationTimeMillisec = currentIntegrationTimeMillisec;

                if (maxCounts > TARGET_COUNTS + TARGET_COUNT_TOLERANCE)
                {
                    currentIntegrationTimeMillisec = (int)(currentIntegrationTimeMillisec * 0.5);
                }
                else
                {
                    double ratio = 1.0 * TARGET_COUNTS / maxCounts;
                    currentIntegrationTimeMillisec = (int)(currentIntegrationTimeMillisec * ratio);
                }

                // sanity-check 
                if (currentIntegrationTimeMillisec > MAX_SANE_INTEGRATION_TIME_SEC * 1000)
                {
                    logger.log("  (rounding down from {0}ms to {1} sec)",
                        currentIntegrationTimeMillisec, MAX_SANE_INTEGRATION_TIME_SEC);
                    currentIntegrationTimeMillisec = MAX_SANE_INTEGRATION_TIME_SEC * 1000;
                }
                else if (currentIntegrationTimeMillisec < 1)
                {
                    logger.log("  (rounding up from {0}ms to {1} sec)",
                        currentIntegrationTimeMillisec, 1);
                    currentIntegrationTimeMillisec = 1;
                }

                if (currentIntegrationTimeMillisec == prevIntegrationTime)
                {
                    repeatCount++;
                    if (repeatCount >= 3)
                    {
                        logger.display("ERROR: integration time is not converging after {0} duplicate values", repeatCount);
                        break;
                    }
                }
                else
                {
                    repeatCount = 0;
                }
                prevIntegrationTime = currentIntegrationTimeMillisec;
                wrapper.setIntegrationTime(specIndex, currentIntegrationTimeMillisec * 1000);

                // throwaway
                wrapper.getSpectrum(specIndex);
            }

            if (!found)
            {
                logger.display("ERROR: unable to find good integration time for spectrometer {0} after {1} tries",
                    specIndex, MAX_TRIES);
                return DEFAULT_INTEGRATION_TIME_MILLISEC;
            }
            else
            {
                logger.display("  Optimal integration time for spectrometer {0} = {1} millisec ",
                    specIndex, currentIntegrationTimeMillisec);
                return currentIntegrationTimeMillisec;
            }
        }

        private void numericUpDownSaveEverySec_ValueChanged(object sender, EventArgs e)
        {
            saveTimer.Interval = (int)(numericUpDownSaveEverySec.Value * 1000);
        }

        ////////////////////////////////////////////////////////////////////////
        // GPIO
        ////////////////////////////////////////////////////////////////////////

        void updateGPIOValues()
        {
            // yes there are better ways but I'm in a hurry
            for (int i = 0; i < numberOfSpectrometers; i++)
            {
                if (gpios[i] != null)
                {
                    gpios[i].setValueBit(0, (short)(checkBoxGPIOVal0.Checked ? 1 : 0));
                    //logger.display("set GPIO[{0}] value to {1} on index {2}", 0, checkBoxGPIOVal0.Checked ? 1 : 0, i);
                    //gpios[i].setValueBit(1, (short)(checkBoxGPIOVal1.Checked ? 1 : 0));
                    //gpios[i].setValueBit(2, (short)(checkBoxGPIOVal2.Checked ? 1 : 0));
                    //gpios[i].setValueBit(3, (short)(checkBoxGPIOVal3.Checked ? 1 : 0));
                }
            }
        }

        void updateGPIODirections()
        {
            // yes there are better ways but I'm in a hurry
            for (int i = 0; i < numberOfSpectrometers; i++)
            {
                if (gpios[i] != null)
                {
                    gpios[i].setDirectionBit(0, (short)(checkBoxGPIODir0.Checked ? 1 : 0));
                    logger.display("set GPIO[{0}] direction to {1} on index {2}", 0, checkBoxGPIODir0.Checked ? 1 : 0, i);
                    gpios[i].setDirectionBit(1, (short)(checkBoxGPIODir1.Checked ? 1 : 0));
                    gpios[i].setDirectionBit(2, (short)(checkBoxGPIODir2.Checked ? 1 : 0));
                    gpios[i].setDirectionBit(3, (short)(checkBoxGPIODir3.Checked ? 1 : 0));
                }
            }
        }

        void updateGPIOAlternates()
        {
            // yes there are better ways but I'm in a hurry
            for (int i = 0; i < numberOfSpectrometers; i++)
            {
                if (gpios[i] != null)
                {
                    gpios[i].setMuxBit(0, (short)(checkBoxGPIOAlt0.Checked ? 1 : 0));
                    logger.display("set GPIO[{0}] alt to {1} on index {2}", 0, checkBoxGPIOAlt0.Checked ? 1 : 0, i);
                    gpios[i].setMuxBit(1, (short)(checkBoxGPIOAlt1.Checked ? 1 : 0));
                    gpios[i].setMuxBit(2, (short)(checkBoxGPIOAlt2.Checked ? 1 : 0));
                    gpios[i].setMuxBit(3, (short)(checkBoxGPIOAlt3.Checked ? 1 : 0));
                }
            }
        }

        private void checkBoxGPIOVal0_CheckedChanged(object sender, EventArgs e) { updateGPIOValues(); }
        private void checkBoxGPIOVal1_CheckedChanged(object sender, EventArgs e) { updateGPIOValues(); }
        private void checkBoxGPIOVal2_CheckedChanged(object sender, EventArgs e) { updateGPIOValues(); }
        private void checkBoxGPIOVal3_CheckedChanged(object sender, EventArgs e) { updateGPIOValues(); }

        private void checkBoxGPIODir0_CheckedChanged(object sender, EventArgs e) { updateGPIODirections();  }
        private void checkBoxGPIODir1_CheckedChanged(object sender, EventArgs e) { updateGPIODirections();  }
        private void checkBoxGPIODir2_CheckedChanged(object sender, EventArgs e) { updateGPIODirections();  }
        private void checkBoxGPIODir3_CheckedChanged(object sender, EventArgs e) { updateGPIODirections(); }

        private void checkBoxGPIOAlt0_CheckedChanged(object sender, EventArgs e) { updateGPIOAlternates();  } 
        private void checkBoxGPIOAlt1_CheckedChanged(object sender, EventArgs e) { updateGPIOAlternates();  } 
        private void checkBoxGPIOAlt2_CheckedChanged(object sender, EventArgs e) { updateGPIOAlternates();  }
        private void checkBoxGPIOAlt3_CheckedChanged(object sender, EventArgs e) { updateGPIOAlternates();  }

        private void checkBoxSaveAll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSaveAll.Checked)
            {
                numericUpDownSaveEverySec.Enabled = false;
                savingAll = true;
                buttonMergeSaved.Text = "Write Disk";
            }
            else
            {
                numericUpDownSaveEverySec.Enabled = true;
                savingAll = false;
                buttonMergeSaved.Text = "Merge Saved";
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Noise Analysis
        ////////////////////////////////////////////////////////////////////////

        bool computeNoise = false;
        uint rmsMeasurements = 5;
        List<double[]> noiseHistory = null;

        private void checkBoxComputeNoise_CheckedChanged(object sender, EventArgs e)
        {
            computeNoise = checkBoxComputeNoise.Checked;
            numericUpDownRMSMeasurements_ValueChanged(null, null);
            groupBoxNoiseSettings.Enabled = computeNoise;
            if (computeNoise)
                noiseHistory = new List<double[]>();
            else
                noiseHistory = null;
        }

        private void numericUpDownRMSMeasurements_ValueChanged(object sender, EventArgs e)
        {
            rmsMeasurements = (uint) numericUpDownRMSMeasurements.Value;
        }

        int lastNoiseScanCount = 0;
        void updateNoiseComputation()
        {
            if (!computeNoise || noiseHistory == null)
                return;

            // updateNoiseComputation() is called by updateGraph(). The graph() may be updated faster or slower than the actual scan.
            // If we don't do this, we may add the same scan multiple times to the history, driving down RMS and creating a sawtooth.
            if (scanCount == lastNoiseScanCount)
                return;

            // only do this for the first connected spectrometer
            const int specIndex = 0;
            int pixelCount = pixels[specIndex];

            // add current spectrum to end of noiseHistory
            double[] newSpec = new double[pixels[specIndex]];
            Array.Copy(graphableSpectrum[specIndex], newSpec, pixelCount);
            noiseHistory.Add(newSpec);

            // drop any dated history
            while (noiseHistory.Count > rmsMeasurements)
                noiseHistory.RemoveAt(0);
            
            // compute noise for each pixel
            double sumOfRMS = 0;
            for (uint pixel = 0; pixel < pixelCount;  pixel++)
            {
                // sum each pixel over time
                double sumOfIntensities = 0;
                foreach (double[] spec in noiseHistory)
                    sumOfIntensities += spec[pixel];

                // take mean of each pixel
                double avgIntensity = sumOfIntensities / noiseHistory.Count;

                // sum squared deltas over history
                double sumOfSquares = 0;
                foreach (double[] spec in noiseHistory)
                {
                    double delta = spec[pixel] - avgIntensity;
                    sumOfSquares += delta * delta;
                    // logger.log("  pixel {0} intensity {1:f4} delta {2:f4} sqr {3:f4}", pixel, spec[pixel], delta, delta * delta);
                }

                double rms = Math.Sqrt(sumOfSquares / noiseHistory.Count);
                // logger.log("  pixel {0} rms {1:f4} sumOfSquares {2:f4} sumOfIntensities {3:f4} avgIntensity {4:f4} history {5}", 
                //     pixel, rms, sumOfSquares, sumOfIntensities, avgIntensity, noiseHistory.Count);
                sumOfRMS += rms;
            }

            double avgRMS = sumOfRMS / pixelCount;
            labelNoiseResult.Text = String.Format("RMS: {0:f4}", avgRMS);
            logger.log("RMS for scan {0}: {1:f4}", scanCount, avgRMS);

            lastNoiseScanCount = scanCount;
        }
    }
}
