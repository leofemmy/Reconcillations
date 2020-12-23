using System;
using System.Drawing;
using DevExpress.XtraReports.UI;

namespace Reconcillations.Reports
{
    public partial class XtraRepBanks
    {
        public XtraRepBanks()
        {
            InitializeComponent();
        }

        private void tableCell3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            tableCell3.ForeColor = Color.Blue;
        }
    }
}
