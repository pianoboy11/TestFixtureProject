using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;

namespace TestFixtureProject.DataAccess
{
    using TestFixtureProject.Common;

    public class TestFixtureDAQ
    {
        int counter = 0;

        #region constructor 

        internal MccDaq.MccBoard DaqBoard;
        internal MccDaq.ErrorInfo ULStat;
        private string _mexceptionMessage = null;


        int rgbvalue;
        public TestFixtureDAQ(ref MccDaq.MccBoard aDaqBoard)
        {
            DaqBoard = aDaqBoard;
        }

        //~TestFixtureDAQ()
        //{

        //    ResetPort();
        //}

        /*
         * this function is used configure digital port as input or output
         * Outbut will be read from this set digital port
         * This function is called from ReadStemVotlageFromDaq
         */
        public void TurnOnVoltageOn()
        {
        MccDaq.ErrorInfo ULStat;

        ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortA, MccDaq.DigitalPortDirection.DigitalIn);
        ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 1, MccDaq.DigitalLogicState.High);
        }

        public double ReadTempSensors()
        {
            float engunits;
            MccDaq.ErrorInfo ULStat;
            Int16 datavalue;
            int chanel = 4;
            MccDaq.Range range;
            double TemperatureDegC;
            double TemperatureDegF = 0;

            range = MccDaq.Range.Bip10Volts;

            ULStat = DaqBoard.AIn(chanel, range, out datavalue);

            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
               DaqBoard.ToEngUnits(range, datavalue, out engunits);
               TemperatureDegC = (((datavalue - 8192) / 819.2) - 1.375) / 0.0225;
               TemperatureDegF = (TemperatureDegC * 9 / 5) + 32;
               //stil coding to continue...no clarity further
            }
            else if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.BadRange))
            {
                MessageBox.Show("Change the Range argument to one supported by this board.", ULStat.Message.ToString());
            }
            else
            {
                MessageBox.Show("Some Problem has occured with the board", ULStat.Message.ToString());
            }
            return TemperatureDegF;
        }

        public int ReadGreenSensorValue()
        {
            rgbvalue = 0;
            MccDaq.ErrorInfo ULStat;
            int channel = 1;
            Int16 datavalue;
            float engunits;

            MccDaq.Range range;

            range = MccDaq.Range.Bip10Volts;

            ULStat = DaqBoard.AIn(channel, range, out datavalue);
            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                DaqBoard.ToEngUnits(range, datavalue, out engunits);
                if (datavalue > 11059)
                    rgbvalue = 9999;
                else
                    rgbvalue = (datavalue - 8192);// * RedSensorGain * RedTemperatureGain

            }
            else if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.BadRange))
            {
                MessageBox.Show("Change the Range argument to one supported by this board.", ULStat.Message.ToString());
            }
            else
            {
                MessageBox.Show("Some Problem has occured with the board", ULStat.Message.ToString());
            }

            return rgbvalue;
        }

        public int ReadBlueSensorValue()
        {
            rgbvalue = 0;
            MccDaq.ErrorInfo ULStat;
            int channel = 2;
            Int16 datavalue;
            float engunits;

            MccDaq.Range range;

            range = MccDaq.Range.Bip10Volts;

            ULStat = DaqBoard.AIn(channel, range, out datavalue);
            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                DaqBoard.ToEngUnits(range, datavalue, out engunits);
                if (datavalue > 11059)
                    rgbvalue = 9999;
                else
                    rgbvalue = (datavalue - 8192);// * RedSensorGain * RedTemperatureGain

            }
            else if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.BadRange))
            {
                MessageBox.Show("Change the Range argument to one supported by this board.", ULStat.Message.ToString());
            }
            else
            {
                MessageBox.Show("Some Problem has occured with the board", ULStat.Message.ToString());
            }

            return rgbvalue;
        }

        public double ReadAmbbientTemperature()
        {
            rgbvalue = 0;
            MccDaq.ErrorInfo ULStat;
            int channel = 6;
            Int16 datavalue;
            float engunits;
            double TemperatureDegC = 0;
            double TemperatureDegF = 0;
            //MccDaq.ChannelType.Analog = 4;

            MccDaq.Range range;
            range = MccDaq.Range.Bip10Volts;

            ULStat = DaqBoard.AIn(channel, range, out datavalue);
            ULStat = DaqBoard.AInputMode(MccDaq.AInputMode.SingleEnded);

            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                DaqBoard.ToEngUnits(range, datavalue, out engunits);
                TemperatureDegF = engunits * 100; // transfer function Vout = 10mV/degF
                //double value = 5000.0 / 1023.0;
                TemperatureDegC = (TemperatureDegF - 32.0) * 5.0 / 9.0;
            }
            else if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.BadRange))
            {
                MessageBox.Show("Change the Range argument to one supported by this board.", ULStat.Message.ToString());
            }
            else
            {
                MessageBox.Show("Some Problem has occured with the board", ULStat.Message.ToString());
            }


            return TemperatureDegC;
        }

        //public double ReadAmbbientTemperature()
        //{
        //    rgbvalue = 0;
        //    MccDaq.ErrorInfo ULStat;
        //    int channel = 4;
        //    Int16 datavalue;
        //    float engunits;
        //    double TemperatureDegC =0;
        //    //double TemperatureDegF=0;
        //    //MccDaq.ChannelType.Analog = 4;

        //    MccDaq.Range range;
        //    range = MccDaq.Range.Bip10Volts;

        //    ULStat = DaqBoard.AIn(channel, range, out datavalue);

        //    if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
        //    {
        //        DaqBoard.ToEngUnits(range, datavalue, out engunits);
        //        double mVoltage = engunits * 1000;
        //        double value = 5000.0 / 1023.0;
        //        TemperatureDegC = (mVoltage * value)/ 100;
        //    }
        //    else if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.BadRange))
        //    {
        //        MessageBox.Show("Change the Range argument to one supported by this board.", ULStat.Message.ToString());
        //    }
        //    else
        //    {
        //        MessageBox.Show("Some Problem has occured with the board", ULStat.Message.ToString());
        //    }


        //    return TemperatureDegC;
        //}
        /*
         * this function has to be called once PLCSolenoidPairing comes to low and wait for 8 seconds.
         * After 8 seconds, this function needs to called and make it high for 2seconds after that bring it low
         * */
        public string SendOnCommandToSolenoid(int aFlag)
        {
            MccDaq.ErrorInfo ULStat;
            string return_value = MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();

            ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortA, MccDaq.DigitalPortDirection.DigitalIn);
            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                if (aFlag == 1)
                {
                    ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 10, MccDaq.DigitalLogicState.High);
                    //ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 10, MccDaq.DigitalLogicState.High);
                    if (!ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                        return_value = ULStat.Value.ToString();
                }
                else
                {
                    ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 10, MccDaq.DigitalLogicState.Low);
                    //ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 10, MccDaq.DigitalLogicState.Low);
                    if (!ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                        return_value = ULStat.Value.ToString();
                }
            }
            else
            {
                return_value = ULStat.Value.ToString();
            }

            return return_value;
        }

        public string SendOffCommandToSolenoidTest()
        {
            MccDaq.ErrorInfo ULStat;
            string return_value = MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();

            ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortB, MccDaq.DigitalPortDirection.DigitalOut);
            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {

                    ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 10, MccDaq.DigitalLogicState.Low);
                       if (!ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                        return_value = ULStat.Value.ToString();
            }
            else
            {
                return_value = ULStat.Value.ToString();
            }

            return return_value;
        }

        public string SendOnCommandToSolenoidTest()
        {
            MccDaq.ErrorInfo ULStat;
            string return_value = MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();

            //ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortA, MccDaq.DigitalPortDirection.DigitalOut);
            ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortB, MccDaq.DigitalPortDirection.DigitalOut);
            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {

                ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 10, MccDaq.DigitalLogicState.High);
                if (!ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                    return_value = ULStat.Value.ToString();
            }
            else
            {
                return_value = ULStat.Value.ToString();
            }

            return return_value;
        }

        public string TurnOn120VRelayOn(int aFlag)
        {
            //TODO: Keith 
            MccDaq.ErrorInfo ULStat;
            string return_value = MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();
            ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortB, MccDaq.DigitalPortDirection.DigitalOut);
            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                if (aFlag == 1)
                {
                   ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 9, MccDaq.DigitalLogicState.High);

                    if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                    {
                        ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 8, MccDaq.DigitalLogicState.High);
                        if (!ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                            return_value = ULStat.Value.ToString();
                    }
                    else
                    {
                        return_value = ULStat.Value.ToString();
                    }

                    //if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                    //{
                    //    ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 8, MccDaq.DigitalLogicState.High);
                    //    if (!ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                    //        return_value = ULStat.Value.ToString();
                    //}
                    //else
                    //{
                    //    return_value = ULStat.Value.ToString();
                    //}
                }
                else
                {
                    ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 9, MccDaq.DigitalLogicState.Low);
                    if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                    {
                        ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 8, MccDaq.DigitalLogicState.Low);
                        if (!ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                            return_value = ULStat.Value.ToString();
                    }
                    else
                    {
                        return_value = ULStat.Value.ToString();
                    }
                }
            }
            else
            {
                return_value = ULStat.Value.ToString();
            }
            return return_value;
        }

        //public string TurnOn15VRelayOn(int aFlag)
        //{
        //    MccDaq.ErrorInfo ULStat;
        //    string return_value = MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();

        //    ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortB, MccDaq.DigitalPortDirection.DigitalOut);
        //    if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
        //    {
        //        if (aFlag == 1)
        //        {
        //            ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 8, MccDaq.DigitalLogicState.High);
        //            if (!ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
        //                return_value = ULStat.Value.ToString();
        //        }
        //        else
        //        {
        //            ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 8, MccDaq.DigitalLogicState.Low);
        //            if (!ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
        //                return_value = ULStat.Value.ToString();
        //        }
        //    }
        //    else
        //    {
        //        return_value = ULStat.Value.ToString();
        //    }
        //    return return_value;
        //}

        public string TurnOn15VRelayOn(int aFlag)
        {
            MccDaq.ErrorInfo ULStat;
            string return_value = MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();

            ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortB, MccDaq.DigitalPortDirection.DigitalOut);
            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                if (aFlag == 1)
                {
                    ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 8, MccDaq.DigitalLogicState.High);
                    if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                    {
                        ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 9, MccDaq.DigitalLogicState.Low);
                        if (!ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                            return_value = ULStat.Value.ToString();
                    }
                    else
                    {
                        return_value = ULStat.Value.ToString();
                    }
                }
                else
                {
                    ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 8, MccDaq.DigitalLogicState.Low);
                    if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                    {
                        ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 9, MccDaq.DigitalLogicState.Low);
                        if (!ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                            return_value = ULStat.Value.ToString();
                    }
                    else
                    {
                        return_value = ULStat.Value.ToString();
                    }
                }
            }
            else
            {
                return_value = ULStat.Value.ToString();
            }
            return return_value;
        }

        public string PressStartButonOnFixture()
        {
            MccDaq.ErrorInfo ULStat;
            string return_value = MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();
            short DataValue;
            ULStat = DaqBoard.DIn(MccDaq.DigitalPortType.FirstPortA, out DataValue);
            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {

                if (((DataValue & (1 << 0)) == 0))
                    return_value = "Start";
                else
                    return_value = "Not Started";
            }
            else
            {
                return_value = ULStat.Value.ToString();
            }
            return return_value;
        }

        public string PressStartButonOnFixtureSimulator()
        {
            MccDaq.ErrorInfo ULStat;
            string return_value = MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();

            ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 8, MccDaq.DigitalLogicState.Low); // select load sense resistor
            ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortA, MccDaq.DigitalPortDirection.DigitalOut);
            //ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 6, MccDaq.DigitalLogicState.Low); // set sense resistor voltage to 0

            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                return_value = "Press Start Button Pressed...";
                frmTestFixture.Instance.WriteToLog(return_value, ApplicationConstants.TraceLogType.Information);
            }
            else
            {
                return_value = ULStat.Value.ToString();
                frmTestFixture.Instance.WriteToLog(return_value, ApplicationConstants.TraceLogType.Error);
            }

            return return_value;
        }

        public void PressStopButonOnFixture()
        {
            PLCSolenoidPairing(0);
            SendOnCommandToSolenoid(0);
            TurnOn120VRelayOn(0);
            TurnOn15VRelayOn(0);       
        }

        public string PLCSolenoidPairing(int aFlag)
        {
            MccDaq.ErrorInfo ULStat;
            string return_value = MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();

                ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortB, MccDaq.DigitalPortDirection.DigitalOut);
                if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                {
                //TurnHighForPLCPairing(aFlag);
                return return_value;

                }
                else
                {
                    return_value = ULStat.Value.ToString();
                }
            return return_value;
      }

        public string TurnHighForPLCPairing(int aFlag)
        {
            ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortB, MccDaq.DigitalPortDirection.DigitalOut);
            //if (aFlag == 1)
            //{
               ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 11, MccDaq.DigitalLogicState.High);
               
                if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                {
                    return MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();
                }
                else
                {
                    return ULStat.Value.ToString();
                }
            //}
            //else
            //{
            //    ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 11, MccDaq.DigitalLogicState.Low);
               
            //    if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            //    {
            //        return MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();
            //    }
            //    else
            //    {
            //        return ULStat.Value.ToString();
            //    }
            //}
        }

        public string TurnLowForPLCPairing()
        {
            ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortB, MccDaq.DigitalPortDirection.DigitalOut);

            ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 11, MccDaq.DigitalLogicState.Low);

            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                return MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();
            }
            else
            {
                return ULStat.Value.ToString();
            }
        }

        public string TurnHighForLuminairePairing(int aFlag)
        {
            ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortA, MccDaq.DigitalPortDirection.DigitalIn);
            if (aFlag == 1)
            {
                ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 10, MccDaq.DigitalLogicState.High);

                if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                {
                    return MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();
                }
                else
                {
                    return ULStat.Value.ToString();
                }
            }
            else
            {
                ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 10, MccDaq.DigitalLogicState.Low);

                if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                {
                    return MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();
                }
                else
                {
                    return ULStat.Value.ToString();
                }
            }
        }

        public string DownPLCSolenoidPairing()
        {

 
        ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 11, MccDaq.DigitalLogicState.Low);
            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                return MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();
            }
            else
            {
                return ULStat.Value.ToString();
            }
        }

        //object sender, EventArgs e
        public string MakeItHigAgainForPLCPairing()
        {
 
        ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 11, MccDaq.DigitalLogicState.High);
            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                return MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();
            }
            else
            {
                return ULStat.Value.ToString();
            }
        }

        //object sender, EventArgs e
        public string MakeItLowCompletly()
        {
            ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 11, MccDaq.DigitalLogicState.Low);
            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                return MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();
            }
            else
            {
                return ULStat.Value.ToString();
            }
        }

        public float ReadOutVoltage()
        {
            MccDaq.ErrorInfo ULStat;
            MccDaq.Range range;
            ushort datavalue = 0;
            ushort datavalue1 = 0;
            ushort datavalue2 = 0;
            ushort datavalue3 = 0;
            ushort datavalue4 = 0;
            int chanel = 2;
            float engunits = 0;
            bool saveState = false;

            saveState = ViewModel.TestFixtureViewModel.isStartPressed;

            //Suspend the button read in the "ListenToStartEvent" thread unconditionally to keep start thread alive
            ViewModel.TestFixtureViewModel.isStartPressed = true;

            ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 8, MccDaq.DigitalLogicState.Low); // select load sense resistor
            ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortA, MccDaq.DigitalPortDirection.DigitalOut);
            ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 6, MccDaq.DigitalLogicState.Low); // set sense resistor voltage to 0

            System.Threading.Thread.Sleep(2000);

            ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 6, MccDaq.DigitalLogicState.High); // set sense resistor voltage to 5V
            //System.Threading.Thread.Sleep(10);// wait for capacitors to charge a little

            range = MccDaq.Range.Bip10Volts;
            ULStat = DaqBoard.AInputMode(MccDaq.AInputMode.Differential);

            ULStat = DaqBoard.AIn(chanel, range, out datavalue1);
            System.Threading.Thread.Sleep(5);

            ULStat = DaqBoard.AIn(chanel, range, out datavalue2);
            System.Threading.Thread.Sleep(5);

            ULStat = DaqBoard.AIn(chanel, range, out datavalue3);
            System.Threading.Thread.Sleep(5);

            ULStat = DaqBoard.AIn(chanel, range, out datavalue4);
            System.Threading.Thread.Sleep(5);

            datavalue += datavalue1;
            datavalue += datavalue2;
            datavalue += datavalue3;
            datavalue += datavalue4;
            datavalue /= 4;

            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                DaqBoard.ToEngUnits(range, datavalue, out engunits);
            }
            else if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.BadRange))
            {
                //MessageBox.Show("Change the Range argument to one supported by this board.", ULStat.Message.ToString());
                frmTestFixture.Instance.WriteToLog("Change the Range argument to one supported by this board." + Environment.NewLine + ULStat.Message.ToString(), ApplicationConstants.TraceLogType.Error);
            }
            else
            {
                //MessageBox.Show("Some Problem has occured with the board", ULStat.Message.ToString());
                frmTestFixture.Instance.WriteToLog("Some Problem has occured with the board." + Environment.NewLine + ULStat.Message.ToString(), ApplicationConstants.TraceLogType.Error);
            }
            ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortA, MccDaq.DigitalPortDirection.DigitalIn);
            ULStat = DaqBoard.AInputMode(MccDaq.AInputMode.SingleEnded);

            ViewModel.TestFixtureViewModel.isStartPressed = saveState;

            return engunits;
        }

        //public float ReadOutVoltageArchived()
        //{
        //    MccDaq.ErrorInfo ULStat;
        //    MccDaq.Range range;
        //    ushort datavalue = 0;
        //    ushort datavalue1 = 0;
        //    ushort datavalue2 = 0;
        //    ushort datavalue3 = 0;
        //    ushort datavalue4 = 0;
        //    int chanel = 2;
        //    float engunits = 0;
        //    bool saveState = false;

        //    saveState = ViewModel.TestFixtureViewModel.isStartPressed;

        //    //Suspend the button read in the "ListenToStartEvent" thread unconditionally to keep start thread alive
        //    ViewModel.TestFixtureViewModel.isStartPressed = true;

        //    ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 8, MccDaq.DigitalLogicState.Low); // select load sense resistor
        //    ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortA, MccDaq.DigitalPortDirection.DigitalOut);
        //    ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 6, MccDaq.DigitalLogicState.Low); // set sense resistor voltage to 0

        //    //System.Threading.Thread.Sleep(10);

        //    ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 6, MccDaq.DigitalLogicState.High); // set sense resistor voltage to 5V
        //    System.Threading.Thread.Sleep(100);// wait for capacitors to charge a little

        //    range = MccDaq.Range.Bip10Volts;
        //    ULStat = DaqBoard.AInputMode(MccDaq.AInputMode.Differential);

        //    ULStat = DaqBoard.AIn(chanel, range, out datavalue1);
        //    System.Threading.Thread.Sleep(10);

        //    ULStat = DaqBoard.AIn(chanel, range, out datavalue2);
        //    System.Threading.Thread.Sleep(10);

        //    ULStat = DaqBoard.AIn(chanel, range, out datavalue3);
        //    System.Threading.Thread.Sleep(10);

        //    ULStat = DaqBoard.AIn(chanel, range, out datavalue4);
        //    System.Threading.Thread.Sleep(10);

        //    datavalue += datavalue1;
        //    datavalue += datavalue2;
        //    datavalue += datavalue3;
        //    datavalue += datavalue4;
        //    datavalue /= 4;

        //    if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
        //    {
        //        DaqBoard.ToEngUnits(range, datavalue, out engunits);
        //    }
        //    else if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.BadRange))
        //    {
        //        MessageBox.Show("Change the Range argument to one supported by this board.", ULStat.Message.ToString());
        //    }
        //    else
        //    {
        //        MessageBox.Show("Some Problem has occured with the board", ULStat.Message.ToString());
        //    }
        //    ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortA, MccDaq.DigitalPortDirection.DigitalIn);
        //    ULStat = DaqBoard.AInputMode(MccDaq.AInputMode.SingleEnded);

        //    ViewModel.TestFixtureViewModel.isStartPressed = saveState;

        //    return engunits;
        //}


        //public float ReadOutVoltage()
        //{
        //    MccDaq.ErrorInfo ULStat;
        //    MccDaq.Range range;
        //    UInt16 datavalue = 0;
        //    int chanel = 5;
        //    float engunits = 0;

        //    range = MccDaq.Range.Bip10Volts;

        //    ULStat = DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 8, MccDaq.DigitalLogicState.Low);

        //    ULStat = DaqBoard.AIn(chanel, range, out datavalue);
        //    if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
        //    {
        //        DaqBoard.ToEngUnits(range, datavalue, out engunits);
        //    }
        //    else if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.BadRange))
        //    {
        //        MessageBox.Show("Change the Range argument to one supported by this board.", ULStat.Message.ToString());
        //    }
        //    else
        //    {
        //        MessageBox.Show("Some Problem has occured with the board", ULStat.Message.ToString());
        //    }
        //    return engunits;
        //}

        public float ReadTopRightPDiodeVoltage()
        {
            MccDaq.ErrorInfo ULStat;
            MccDaq.Range range;
            UInt16 datavalue = 0;
            int chanel = 0;
            float engunits = 0;

            range = MccDaq.Range.Bip10Volts;

            ULStat = DaqBoard.AIn(chanel, range, out datavalue);

            Thread.Sleep(10);

            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                DaqBoard.ToEngUnits(range, datavalue, out engunits);
            }
            else if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.BadRange))
            {
                MessageBox.Show("Change the Range argument to one supported by this board.", ULStat.Message.ToString());
            }
            else
            {     
                //MessageBox.Show("Some Problem has occured with the board", ULStat.Message.ToString());
            }
            return engunits;
        }

        public float ReadTopLeftPDiodeVoltage()
        {
            MccDaq.ErrorInfo ULStat;
            MccDaq.Range range;
            UInt16 datavalue = 0;
            int chanel = 1;
            float engunits = 0;

            range = MccDaq.Range.Bip10Volts;

            ULStat = DaqBoard.AIn(chanel, range, out datavalue);

            Thread.Sleep(10);

            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                DaqBoard.ToEngUnits(range, datavalue, out engunits);
            }
            else if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.BadRange))
            {
                MessageBox.Show("Change the Range argument to one supported by this board.", ULStat.Message.ToString());
            }
            else
            {
                MessageBox.Show("Some Problem has occured with the board", ULStat.Message.ToString());
            }
            return engunits;
        }

        public float ReadBottomRightPDiodeVoltage()
        {
            MccDaq.ErrorInfo ULStat;
            MccDaq.Range range;
            UInt16 datavalue = 0;
            int chanel = 2;
            float engunits = 0;

            range = MccDaq.Range.Bip10Volts;

            ULStat = DaqBoard.AIn(chanel, range, out datavalue);

            Thread.Sleep(10);

            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                DaqBoard.ToEngUnits(range, datavalue, out engunits);
            }
            else if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.BadRange))
            {
                MessageBox.Show("Change the Range argument to one supported by this board.", ULStat.Message.ToString());
            }
            else
            {
                MessageBox.Show("Some Problem has occured with the board", ULStat.Message.ToString());
            }
            return engunits;
        }


        public float ReadSensorChanel(ApplicationConstants.SensorChannel sensorChannel)
        {
            MccDaq.ErrorInfo ULStat;
            MccDaq.Range range;
            UInt16 datavalue = 0;
            int channel = -1;
            float engunits = 0;

            range = MccDaq.Range.Bip10Volts;

            if (sensorChannel == ApplicationConstants.SensorChannel.TopRightSensor)
                channel = 0;
            else if (sensorChannel == ApplicationConstants.SensorChannel.TopLeftSensor)
                channel = 1;
            else if (sensorChannel == ApplicationConstants.SensorChannel.BottomRightSensor)
                channel = 2;
            else if (sensorChannel == ApplicationConstants.SensorChannel.BottomLeftSensor)
                channel = 3;
            else
            {
                //Invalid sensor channel
                channel = -99;
            }

            ULStat = DaqBoard.AIn(channel, range, out datavalue);

            Thread.Sleep(10);

            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                DaqBoard.ToEngUnits(range, datavalue, out engunits);
            }
            else if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.BadRange))
            {
                MessageBox.Show("Change the Range argument to one supported by this board.", ULStat.Message.ToString());
            }
            else
            {
                MessageBox.Show("Some Problem has occured with the board", ULStat.Message.ToString());
            }
            return engunits;
        }

        public float ReadBottomLeftPDiodeVoltage()
        {
            MccDaq.ErrorInfo ULStat;
            MccDaq.Range range;
            UInt16 datavalue = 0;
            int chanel = 3;
            float engunits = 0;

            range = MccDaq.Range.Bip10Volts;

            ULStat = DaqBoard.AIn(chanel, range, out datavalue);

            Thread.Sleep(10);

            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
            {
                DaqBoard.ToEngUnits(range, datavalue, out engunits);
            }
            else if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.BadRange))
            {
                MessageBox.Show("Change the Range argument to one supported by this board.", ULStat.Message.ToString());
            }
            else
            {
                MessageBox.Show("Some Problem has occured with the board", ULStat.Message.ToString());
            }
            return engunits;
        }

        public string ConfigureDaqToSingleEnded()
        {
            MccDaq.ErrorInfo ULStat;
            string return_value = MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();
            ULStat =  DaqBoard.AInputMode(MccDaq.AInputMode.SingleEnded);
            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                return return_value;
            else
                return return_value;
        }

        public string ConfigureDaqToDifferential()
        {
            MccDaq.ErrorInfo ULStat;
            string return_value = MccDaq.ErrorInfo.ErrorCode.NoErrors.ToString();
            ULStat = DaqBoard.AInputMode(MccDaq.AInputMode.Differential);
            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                return return_value;
            else
                return return_value;
        }

        public bool ValidULStat(MccDaq.ErrorInfo ULStat)
        {
            if (ULStat.Value.Equals(MccDaq.ErrorInfo.ErrorCode.NoErrors))
                return true;
            else
                return false;
        }

        //public void ResetPort()
        //{
        //    if (!ValidULStat(DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortA, MccDaq.DigitalPortDirection.DigitalOut))) { return; }
        //    if (!ValidULStat(DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortB, MccDaq.DigitalPortDirection.DigitalOut))) { return; }
        //    if (!ValidULStat(DaqBoard.DOut(MccDaq.DigitalPortType.FirstPortA, 0))) { return; }
        //    if (!ValidULStat(DaqBoard.DOut(MccDaq.DigitalPortType.FirstPortB, 0))) { return; }


        //    //if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 0, MccDaq.DigitalLogicState.Low))) { return; }
        //    //if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 1, MccDaq.DigitalLogicState.Low))) { return; }
        //    //if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 2, MccDaq.DigitalLogicState.Low))) { return; }
        //    //if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 3, MccDaq.DigitalLogicState.Low))) { return; }
        //    //if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 4, MccDaq.DigitalLogicState.Low))) { return; }
        //    //if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 5, MccDaq.DigitalLogicState.Low))) { return; }
        //    //if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 6, MccDaq.DigitalLogicState.Low))) { return; }

        //    //Reset Light Magnet
        //    if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 7, MccDaq.DigitalLogicState.Low))) { return; }

            
        //    //if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 8, MccDaq.DigitalLogicState.Low))) { return; }
        //    //if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 9, MccDaq.DigitalLogicState.Low))) { return; }
        //    //if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 10, MccDaq.DigitalLogicState.Low))) { return; }

        //    //Reset PPM SWITCH
        //    if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 11, MccDaq.DigitalLogicState.Low))) { return; }
        ////    if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 12, MccDaq.DigitalLogicState.Low))) { return; }
        ////    if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 13, MccDaq.DigitalLogicState.Low))) { return; }
        ////    if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 14, MccDaq.DigitalLogicState.Low))) { return; }
        ////    if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 15, MccDaq.DigitalLogicState.Low))) { return; }
        //}

        public void ResetPort()
        {
            MccDaq.ErrorInfo ULStat;

            //if (!ValidULStat(DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortA, MccDaq.DigitalPortDirection.DigitalIn))) { return; }
            //if (!ValidULStat(DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortB, MccDaq.DigitalPortDirection.DigitalOut))) { return; }

            ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortA, MccDaq.DigitalPortDirection.DigitalIn);
            ULStat = DaqBoard.DConfigPort(MccDaq.DigitalPortType.FirstPortB, MccDaq.DigitalPortDirection.DigitalOut);

            //if (!ValidULStat(DaqBoard.DOut(MccDaq.DigitalPortType.FirstPortA, 0))) { return; }
            //if (!ValidULStat(DaqBoard.DOut(MccDaq.DigitalPortType.FirstPortB, 0))) { return; }

            ULStat = DaqBoard.DOut(MccDaq.DigitalPortType.FirstPortB, 0);
            ////Reset Light Magnet
            //if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 7, MccDaq.DigitalLogicState.Low)))
            //{
            //    return;
            //}

            ////Reset PPM SWITCH
            //if (!ValidULStat(DaqBoard.DBitOut(MccDaq.DigitalPortType.FirstPortA, 11, MccDaq.DigitalLogicState.Low))) 
            //{
            //    return;
            //}
        }
        #endregion
    }
}
