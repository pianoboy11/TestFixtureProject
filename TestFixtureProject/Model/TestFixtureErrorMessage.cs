using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestFixtureProject.ViewModel;

namespace TestFixtureProject.Model
{
   public class TestFixtureErrorMessage : TestFixtureLoginBaseVM
    {
        private string testMessage;

        public string TestMessage
        {
            get { return testMessage; }

            set
            {
                testMessage = value;
                this.OnPropertyChanged("TestMessage");
            }
        }
        #region dummy implementation to satisfy compiler
        public override void ButtonOKClick()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
