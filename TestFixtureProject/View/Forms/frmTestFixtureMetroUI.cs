using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace TestFixtureProject
{
    using TestFixtureProject.Helpers;
    using TestFixtureProject.ViewModel;

    public partial class frmTestFixtureMetroUI : MetroFramework.Forms.MetroForm
    {
        #region private
        TestFixtureMainWindowVM vm;
        TestFixtureViewModel pagevm;
        

        //singleton design pattern
        private static frmTestFixtureMetroUI _instance; 

        public static frmTestFixtureMetroUI Instance
        {
            get
            {
                //if (Instance == null)
                //    _instance = new frmTestFixtureMetroUI();
                return _instance;
            }
        }

        public MetroFramework.Controls.MetroPanel MetroContainer
        {
            get { return this.mPanel; }
            set { this.mPanel = value;  }
        }
        #endregion

        #region Constructor
        public frmTestFixtureMetroUI()
        {
            InitializeComponent();

            _instance = this;
        }
        #endregion

        private void frmTestFixtureMetroUI_Load(object sender, EventArgs e)
        { 
            //frmTestFixtureLogin frmLogin = new frmTestFixtureLogin();
            //frmLogin.ShowDialog();
            ucLogin uc = new ucLogin();
            uc.Dock = DockStyle.Fill;
            mPanel.Controls.Add(uc);         
        }

        private void frmTestFixtureMetroUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (TestFixtureViewModel.Instance != null)
                {
                    TestFixtureViewModel.Instance.ResetDaqBoardPort();
                    TestFixtureViewModel.Instance.SaveImgDetailsToFile(e);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Environment.Exit(1);
        }
    }
}
