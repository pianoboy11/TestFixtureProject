using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TestFixtureProject.Common
{
    class BooleanToVisibilityConverter : IValueConverter 
    {
        private Visibility _whenvaluetrue = Visibility.Visible;
        private Visibility _whenvaluefalse = Visibility.Collapsed;
        private Visibility _whenvaluenull = Visibility.Hidden;

        public Visibility WhenValueTrue
        {
            get { return _whenvaluetrue; }
            set
            {
                _whenvaluetrue = value;
            }
        }

        public Visibility WhenValueFalse
        {
            get { return _whenvaluefalse; }

            set
            {
                _whenvaluefalse = value;
            }
        }
        public Visibility WhenValueNull
        {
            get { return _whenvaluenull; }

            set
            {
                _whenvaluenull = value;
            }
        }

        public Object Convert(Object value, Type targettype, Object parameter, CultureInfo culture)
        {
            return GetVisibility(value);
        }
        private Object GetVisibility(Object aValue)
        {
            if ((aValue == null) || (!(aValue is bool)))
                return WhenValueNull;

            if ((aValue is bool))
                return WhenValueTrue;

            return WhenValueFalse;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return true;
            return WhenValueTrue;
        }
    }
}
