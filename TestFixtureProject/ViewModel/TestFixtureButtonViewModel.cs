using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TestFixtureProject.Common;
using TestFixtureProject.NavigationServiceImp;
//using TestFixtureProject.View;

namespace TestFixtureProject.ViewModel
{
    public class TestFixtureButtonViewModel : TestFixtureObserverBase
    {
        #region Member fields
        protected bool _IsVisible;
        protected bool _IsEnabled;
        protected RelayCommand _ButtonCommand;
        protected String _Text;

        /// <summary>
        /// This delegate defines a bool function.
        /// </summary>
        /// <returns>bool</returns>
        public delegate bool IsEnabledDelegate();

        /// <summary>
        /// This is an object that can be assigned a function that is formatted as follows:
        /// 
        /// bool mymethod()
        /// 
        /// This allows for passing in a function that will enable or disable the button.
        /// </summary>
        public IsEnabledDelegate IsEnabledMethod = () => { return true; };
        #endregion

        #region Constructors
        /// <summary>
        /// The default constructor
        /// </summary>
        public TestFixtureButtonViewModel()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Set this to the function you want the button to execute when clicked.
        /// </summary>
        public virtual ICommand ButtonCommand
        {
            get { return _ButtonCommand; }
            set
            {
                _ButtonCommand = value as RelayCommand;
                OnPropertyChanged("ButtonCommand");
            }
        }

        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                _IsVisible = value;
                OnPropertyChanged("IsVisible");
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _IsEnabled && IsEnabledMethod();
            }
            set
            {
                _IsEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        /// <summary>
        /// The text of the button, for example, "OK" or "Cancel".
        /// </summary>
        public String Text
        {
            get { return _Text; }
            set
            {
                _Text = value;
                OnPropertyChanged("Text");
            }
        }
        #endregion

    }
}
