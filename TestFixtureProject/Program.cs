using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace TestFixtureProject
{
    using TestFixtureProject.Helpers;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
       {
            ParseArgs(args);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new frmTestFixtureMetroUI());
            Application.Run(new frmTestFixture());
        }

        /// <summary>
        /// This function accepts all arguments and puts them in the DataStore.
        /// Only valid arguments are used though.
        /// </summary>
        /// <param name="inArgs"></param>
        static void ParseArgs(String[] inArgs)
        {
            // Add exe details to DataStore, such as the Setup.exe path and directory
            TestFixtureDataStores.ExePath = Assembly.GetExecutingAssembly().Location;
            TestFixtureDataStores.ExeFile = Path.GetFileName(TestFixtureDataStores.ExePath);
            TestFixtureDataStores.ExeDir = Path.GetDirectoryName(TestFixtureDataStores.ExePath);

            // Parse passed in args
            foreach (string arg in inArgs)
            {
                // We don't care what args they pass in, we will store all the 
                // information in the DataStore.
                // 
                // Parameters should be in one of the following formats:
                //    /param1
                //    param1
                //    /Param1=Value
                //    Param1=Value
                // 

                int Parameter = 0;
                int Value = 1;

                string[] data = new String[2];
                string[] tmp = arg.Split("=".ToCharArray(), 2, StringSplitOptions.None);
                data[Parameter] = tmp[Parameter];

                if (tmp.Length > 1)
                    data[Value] = tmp[Value];

                // Remove the slash if there is one
                while (data[Parameter].StartsWith("/") || data[Parameter].StartsWith("-"))
                    data[Parameter] = data[Parameter].Substring(1);

                // If the KeyValue doesn't exist set it to true
                if (String.IsNullOrWhiteSpace(data[Value]))
                    data[Value] = "true";
                else
                    // If the KeyValue exists, trim quotes and then trim spaces
                    data[Value] = data[Value].Trim("\"".ToCharArray()).Trim();

                TestFixtureDataStores.Information.Add(data[Parameter], data[Value]);
            }
        }

        static void Application_Exit(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
            Environment.Exit(0);
        }

        static public bool DoHandle { get; set; }
        static void handleexception(Object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs args)
        {
            if (DoHandle)
                args.Handled = true;
            else
                args.Handled = false;
        }
    }


}


