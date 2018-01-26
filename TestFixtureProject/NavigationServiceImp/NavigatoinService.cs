using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace TestFixtureProject.NavigationServiceImp
{
    public sealed class Navigator
    {
        private static readonly Navigator instance = new Navigator();
        private Navigator() { }
        public static NavigationService NavigationService { get; set; }
        public static void Cancel()
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to cancel?", "Cancel", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                //App.Current.Shutdown(1); //wpf
                Application.Current.Shutdown(1); //winforms
        }
    }
}
