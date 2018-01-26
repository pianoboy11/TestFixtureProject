using System.ComponentModel;

namespace TestFixtureProject.Common
{
   public  class TestFixtureObserverBase : INotifyPropertyChanged
    {
        #region INotifyPropoertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged(string propertyname)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyname));
            }
        }
        #endregion
    }

}
