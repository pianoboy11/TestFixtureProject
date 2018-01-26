using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Controls;

namespace TestFixtureProject
{
    using TestFixtureProject.ViewModel;
    using TestFixtureProject.Helpers;

    public partial class ucLogin : MetroUserControl
    {
        frmTestFixture uc { set; get; }

        public ucLogin()
        {
            InitializeComponent();

            uc = new frmTestFixture();


            txtUserName.Text = "admin"; //Temporary....remove when debugging is completed
            txtPassword.Text = "Pentair"; //Temporary....remove when debugging is completed

            uc.Dock = DockStyle.Fill;
            //frmTestFixtureMetroUI.Instance.MetroContainer.Controls.Add(uc);
            //frmTestFixtureMetroUI.Instance.MetroContainer.Controls["ucHostContainer"].BringToFront();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            TestFixtureLoginViewModel.Instance.ValidateToNavigate(null);

            if (frmTestFixture.Instance != null)
            {
                if (frmTestFixture.Instance.pagelvm.SelectedIndex != 0)
                {
                    if (!frmTestFixture.Instance._accessGranted && !frmTestFixture.Instance._partialAccess)
                    {
                        if (frmTestFixture.Instance.pagelvm.AccessGranted)
                        {
                            frmTestFixture.Instance._accessGranted = true;
                            frmTestFixture.Instance._partialAccess = false;
                            //frmTestFixture.Instance.MetroContainer.Controls["ucHostContainer"].BringToFront();
                        }
                        else if (frmTestFixture.Instance.pagelvm.PartialAccessGranted)
                        {
                            frmTestFixture.Instance._partialAccess = true;
                            frmTestFixture.Instance._accessGranted = false;
                            //frmTestFixtureMetroUI.Instance.MetroContainer.Controls["ucHostContainer"].BringToFront();
                        }
                        else
                        {
                            frmTestFixture.Instance.tabControl2.SelectedIndexChanged -= frmTestFixture.Instance.tcHostTabControl_SelectedIndexChanged;
                            frmTestFixture.Instance.tabControl2.SelectedIndex = 0;
                            frmTestFixture.Instance.tabControl2.SelectedIndexChanged += frmTestFixture.Instance.tcHostTabControl_SelectedIndexChanged;
                        }
                    }
                }
                //else
                    //frmTestFixtureMetroUI.Instance.MetroContainer.Controls["ucLogin"].BringToFront();


                if (frmTestFixture.Instance._partialAccess)
                {
                    string tabname = frmTestFixture.Instance.tabControl2.SelectedTab.Name;

                    //if (tabname.Equals("Diagnostics"))
                    //    item.IsSelected = true;
                    //if (tabname.Equals("Projector Diagnostics"))
                    //    item.IsSelected = true;
                    if (tabname.Equals("Settings"))
                    {
                        if (frmTestFixture.Instance.pagelvm.AccessGranted)
                        {
                            frmTestFixture.Instance._accessGranted = true;
                            frmTestFixture.Instance._partialAccess = false;
                            frmTestFixture.Instance.tabControl2.SelectedIndex = frmTestFixture.Instance.pagelvm.SelectedIndex;
                            //frmTestFixtureMetroUI.Instance.MetroContainer.Controls["ucHostContainer"].BringToFront();
                        }
                        else
                        {
                            frmTestFixture.Instance.tabControl2.SelectedIndexChanged -= frmTestFixture.Instance.tcHostTabControl_SelectedIndexChanged;
                            frmTestFixture.Instance.tabControl2.SelectedIndex = 0;
                            frmTestFixture.Instance.tabControl2.SelectedIndexChanged += frmTestFixture.Instance.tcHostTabControl_SelectedIndexChanged;
                        }
                    }

                    if (tabname.Equals("Calibration"))
                    {
                        if (frmTestFixture.Instance.pagelvm.AccessGranted)
                        {
                            frmTestFixture.Instance._accessGranted = true;
                            frmTestFixture.Instance._partialAccess = false;
                            frmTestFixture.Instance.tabControl2.SelectedIndex = frmTestFixture.Instance.pagelvm.SelectedIndex;
                            //frmTestFixtureMetroUI.Instance.MetroContainer.Controls["ucHostContainer"].BringToFront();
                        }
                        else
                        {
                            frmTestFixture.Instance.tabControl2.SelectedIndexChanged -= frmTestFixture.Instance.tcHostTabControl_SelectedIndexChanged;
                            frmTestFixture.Instance.tabControl2.SelectedIndex = 0;
                            frmTestFixture.Instance.tabControl2.SelectedIndexChanged += frmTestFixture.Instance.tcHostTabControl_SelectedIndexChanged;
                        }
                    }
                }
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            if (TestFixtureLoginViewModel.Instance != null)
                TestFixtureLoginViewModel.Instance.LoginPassword =  txtPassword.Text.ToSecureString();
        }
    }
}
