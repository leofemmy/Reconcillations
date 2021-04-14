using System;
using System.Drawing;
using DevExpress.XtraReports.UI;

namespace Reconcillations.Reports
{
    public partial class XtraRepReversed
    {
        public XtraRepReversed()
        {
            InitializeComponent();
        }

        private void tableCell2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            tableCell2.ForeColor = Color.Blue;
        }
    }
}
