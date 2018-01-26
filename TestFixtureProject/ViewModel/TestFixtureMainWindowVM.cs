using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFixtureProject.Common;

namespace TestFixtureProject.ViewModel
{
    class TestFixtureMainWindowVM : TestFixtureObserverBase
    {
        String _mheading = "Test Fixture Software ";
        public TestFixtureMainWindowVM()
        {
        }

        public String TestFixtureHeading
        {
            get { return _mheading; }
            set
            {
                _mheading = value;
                OnPropertyChanged("Heading");
            }
        } 
    }
}
