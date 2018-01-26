using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFixtureProject.ViewModel;

namespace TestFixtureProject.Model
{
    public class TestFixtureMainModel : TestFixtureLoginBaseVM
    {
        private string _mstartest;

        //constructor
        public TestFixtureMainModel()
        { 
        
        }
        public string Testdatbind
        {
            get { return _mstartest; }
            set
            {
                _mstartest = value;
                OnPropertyChanged("Testdatbind");
            }
        }
        public override void ButtonOKClick()
        {
            throw new NotImplementedException();
        }

    }
}
