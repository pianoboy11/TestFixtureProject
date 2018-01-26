using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFixtureProject.Common
{
    public class DataValidation
    {
        public static bool ValidateInputData(string aData)
        {
            double number = 0;
            bool ret_val = false;

            ret_val = IsInputDataEmpty(aData);
            if (ret_val == false)
            {
                ret_val = double.TryParse(aData, out number);
            }
           return ret_val;
        }
        private static bool IsInputDataEmpty(string aData)
        {
            bool ret_val = string.IsNullOrEmpty(aData);
            return ret_val;
        }
    }
 
}
