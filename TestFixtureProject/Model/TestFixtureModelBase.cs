using System.ComponentModel;

namespace TestFixtureProject.Model
{
    class TestFixtureModelBase : INotifyPropertyChanged
    {
        #region INotifyPropoertyChanged members
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyname)
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
