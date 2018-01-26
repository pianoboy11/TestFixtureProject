using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestFixtureProject
{
    using TestFixtureProject.ViewModel;
    using TestFixtureProject.Helpers;

    public partial class frmTestFixtureLogin : MetroFramework.Forms.MetroForm
    {
        public frmTestFixtureLogin()
        {
            InitializeComponent();

            //if(frmTestFixtureMetroUI.Instance != null)
            //    frmTestFixtureMetroUI.Instance.MetroContainer.Controls["ucLogin"].BringToFront();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (this.DialogResult == DialogResult.Cancel)
            {
                this.Close();
                return;
            }

            TestFixtureLoginViewModel.Instance.ValidateToNavigate(null);

            if (frmTestFixture.Instance != null)
            {          
                if (frmTestFixture.Instance.pagelvm.AccessGranted)
                {
                    lblPasswordStatus.Text = string.Empty;
                    string tabname = frmTestFixture.Instance.tabControl2.SelectedTab.Name;

                    this.DialogResult = DialogResult.OK;

                    frmTestFixture.Instance.WriteToLog("", Common.ApplicationConstants.TraceLogType.Information);
                    this.Close();
                }
                else 
                {
                    lblPasswordStatus.Text = "User entered invalid password...";
                    lblPasswordStatus.ForeColor = Color.DarkRed;
                    frmTestFixture.Instance.WriteToLog(lblPasswordStatus.Text, Common.ApplicationConstants.TraceLogType.Information);

                    //this.DialogResult = DialogResult.No;
                    this.BringToFront();
                }
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            if (TestFixtureLoginViewModel.Instance != null)
                TestFixtureLoginViewModel.Instance.LoginPassword = txtPassword.Text.ToSecureString();
        }

        private void IsLoginValidated()
        {

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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            this.Close();
        }
    }
}
