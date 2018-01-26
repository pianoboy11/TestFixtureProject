using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFixtureProject.Common
{
    public class ApplicationConstants
    {
        public const string TESTFIXTUREUIPAGE = @"View\TestFixtureUI";

        public enum SpectrumColors
        {
            Dark, 
            Red,
            Green,
            Blue,
            White,
            Magenda,
            BlendedWhite,
        }

        public enum MirrorPosition
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            Home
        }

        public enum SensorChannel
        {
            TopLeftSensor,
            TopRightSensor,
            BottomLeftSensor,
            BottomRightSensor,
        }

        public enum TraceLogType
        {
            Information,
            Warning,
            Error
        }

        public enum AmbientLedState
        {
            On,
            Off,
        }

        public enum UserMode
        {
            Engineering,
            Operation,
        }
    }
}
