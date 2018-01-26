using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFixtureProject.Common;

namespace TestFixtureProject.ViewModel
{
    public abstract class TestFixtureLoginBaseVM : TestFixtureObserverBase
    {
        #region private variables
        protected TestFixtureButtonViewModel _mbuttonok;

        #endregion

        public TestFixtureLoginBaseVM()
        { 
        
        }

        public virtual TestFixtureButtonViewModel ButtonOK
        { 
            get
            {
             if(_mbuttonok == null)
                {
                    _mbuttonok = new TestFixtureButtonViewModel()
                        {
                            Text = "OK",
                            IsVisible=true,
                            IsEnabled = true,
                            ButtonCommand = new RelayCommand(param => ButtonOKClick())
                        };
                 }
             return _mbuttonok;
            }
            set
            {
                _mbuttonok = value;
                OnPropertyChanged("ButtonOKClick");
            }
        }
        /// <summary>
        /// This function is called when the Continue Button is clicked.
        /// </summary>
        public abstract void ButtonOKClick();
    }
}
