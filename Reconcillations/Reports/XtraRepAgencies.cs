using System;
using System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.XtraReports.UI;
using System.Windows.Forms;

namespace Reconcillations.Reports
{
    public partial class XtraRepAgencies
    {
        public XtraRepAgencies()
        {
            InitializeComponent();
        }

        private void tableCell2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            tableCell2.ForeColor = Color.Blue;
        }

        private void tableCell2_PreviewMouseMove(object sender, DevExpress.XtraReports.UI.PreviewMouseEventArgs e)
        {
            //Cursor.Current = Cursors.Hand;
        }
    }
}
