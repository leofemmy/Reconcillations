//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Reconcillations.Reports {
    
    public partial class XtraRepSummary : DevExpress.XtraReports.UI.XtraReport {
        private void InitializeComponent() {
            DevExpress.XtraReports.ReportInitializer reportInitializer = new DevExpress.XtraReports.ReportInitializer(this, "Reconcillations.Reports.XtraRepSummary.repx");

            // Controls
            this.TopMargin = reportInitializer.GetControl<DevExpress.XtraReports.UI.TopMarginBand>("TopMargin");
            this.BottomMargin = reportInitializer.GetControl<DevExpress.XtraReports.UI.BottomMarginBand>("BottomMargin");
            this.Detail = reportInitializer.GetControl<DevExpress.XtraReports.UI.DetailBand>("Detail");
            this.table3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTable>("table3");
            this.xrPictureBox1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPictureBox>("xrPictureBox1");
            this.label1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRLabel>("label1");
            this.xrPictureBox2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPictureBox>("xrPictureBox2");
            this.tableRow3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("tableRow3");
            this.tableCell22 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell22");
            this.tableCell23 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell23");
            this.tableCell24 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell24");
            this.tableCell25 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell25");
            this.tableCell26 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell26");
            this.tableCell27 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell27");
            this.tableCell28 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell28");
            this.tableCell29 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell29");
            this.tableCell30 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell30");
            this.tableCell31 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell31");
            this.tableCell32 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell32");
            this.pageInfo1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRPageInfo>("pageInfo1");
            this.table1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTable>("table1");
            this.tableRow1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableRow>("tableRow1");
            this.tableCell1 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell1");
            this.tableCell2 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell2");
            this.tableCell3 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell3");
            this.tableCell4 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell4");
            this.tableCell5 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell5");
            this.tableCell6 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell6");
            this.tableCell7 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell7");
            this.tableCell15 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell15");
            this.tableCell16 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell16");
            this.tableCell19 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell19");
            this.tableCell21 = reportInitializer.GetControl<DevExpress.XtraReports.UI.XRTableCell>("tableCell21");

            // Data Sources
            this.sqlDataSource1 = reportInitializer.GetDataSource<DevExpress.DataAccess.Sql.SqlDataSource>("sqlDataSource1");

            // Calculated Fields
            this.calculatedField1 = reportInitializer.GetCalculatedField("calculatedField1");
            this.calopen = reportInitializer.GetCalculatedField("calopen");
            this.calReems = reportInitializer.GetCalculatedField("calReems");
            this.calunposted = reportInitializer.GetCalculatedField("calunposted");
            this.calInterest = reportInitializer.GetCalculatedField("calInterest");
            this.calTransferfrom = reportInitializer.GetCalculatedField("calTransferfrom");
            this.caloutofperioddrreveser = reportInitializer.GetCalculatedField("caloutofperioddrreveser");
            this.calcurrentperioddrreverse = reportInitializer.GetCalculatedField("calcurrentperioddrreverse");
            this.calothernoncollection = reportInitializer.GetCalculatedField("calothernoncollection");
            this.calBankchargesreverser = reportInitializer.GetCalculatedField("calBankchargesreverser");
            this.calunnotifiedcollections = reportInitializer.GetCalculatedField("calunnotifiedcollections");
            this.calothercredits = reportInitializer.GetCalculatedField("calothercredits");
            this.calBankchargeDr = reportInitializer.GetCalculatedField("calBankchargeDr");
            this.calTransafertootheraccountDr = reportInitializer.GetCalculatedField("calTransafertootheraccountDr");
            this.caloutofperiodcrreveserDR = reportInitializer.GetCalculatedField("caloutofperiodcrreveserDR");
            this.calcureentperiodcrreserverDR = reportInitializer.GetCalculatedField("calcureentperiodcrreserverDR");
            this.caloutofperiodchequesreturnDR = reportInitializer.GetCalculatedField("caloutofperiodchequesreturnDR");
            this.calotherunclassifieddebitDR = reportInitializer.GetCalculatedField("calotherunclassifieddebitDR");
            this.calOtherDebit = reportInitializer.GetCalculatedField("calOtherDebit");
            this.calclosebal = reportInitializer.GetCalculatedField("calclosebal");
            this.calGenerate = reportInitializer.GetCalculatedField("calGenerate");
            this.calculatedField2 = reportInitializer.GetCalculatedField("calculatedField2");
            this.calRevenue = reportInitializer.GetCalculatedField("calRevenue");
            this.calPrevCreditReversed = reportInitializer.GetCalculatedField("calPrevCreditReversed");
        }
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRTable table3;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBox1;
        private DevExpress.XtraReports.UI.XRLabel label1;
        private DevExpress.XtraReports.UI.XRPictureBox xrPictureBox2;
        private DevExpress.XtraReports.UI.XRTableRow tableRow3;
        private DevExpress.XtraReports.UI.XRTableCell tableCell22;
        private DevExpress.XtraReports.UI.XRTableCell tableCell23;
        private DevExpress.XtraReports.UI.XRTableCell tableCell24;
        private DevExpress.XtraReports.UI.XRTableCell tableCell25;
        private DevExpress.XtraReports.UI.XRTableCell tableCell26;
        private DevExpress.XtraReports.UI.XRTableCell tableCell27;
        private DevExpress.XtraReports.UI.XRTableCell tableCell28;
        private DevExpress.XtraReports.UI.XRTableCell tableCell29;
        private DevExpress.XtraReports.UI.XRTableCell tableCell30;
        private DevExpress.XtraReports.UI.XRTableCell tableCell31;
        private DevExpress.XtraReports.UI.XRTableCell tableCell32;
        private DevExpress.XtraReports.UI.XRPageInfo pageInfo1;
        private DevExpress.XtraReports.UI.XRTable table1;
        private DevExpress.XtraReports.UI.XRTableRow tableRow1;
        private DevExpress.XtraReports.UI.XRTableCell tableCell1;
        private DevExpress.XtraReports.UI.XRTableCell tableCell2;
        private DevExpress.XtraReports.UI.XRTableCell tableCell3;
        private DevExpress.XtraReports.UI.XRTableCell tableCell4;
        private DevExpress.XtraReports.UI.XRTableCell tableCell5;
        private DevExpress.XtraReports.UI.XRTableCell tableCell6;
        private DevExpress.XtraReports.UI.XRTableCell tableCell7;
        private DevExpress.XtraReports.UI.XRTableCell tableCell15;
        private DevExpress.XtraReports.UI.XRTableCell tableCell16;
        private DevExpress.XtraReports.UI.XRTableCell tableCell19;
        private DevExpress.XtraReports.UI.XRTableCell tableCell21;
        private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        private DevExpress.XtraReports.UI.CalculatedField calculatedField1;
        private DevExpress.XtraReports.UI.CalculatedField calopen;
        private DevExpress.XtraReports.UI.CalculatedField calReems;
        private DevExpress.XtraReports.UI.CalculatedField calunposted;
        private DevExpress.XtraReports.UI.CalculatedField calInterest;
        private DevExpress.XtraReports.UI.CalculatedField calTransferfrom;
        private DevExpress.XtraReports.UI.CalculatedField caloutofperioddrreveser;
        private DevExpress.XtraReports.UI.CalculatedField calcurrentperioddrreverse;
        private DevExpress.XtraReports.UI.CalculatedField calothernoncollection;
        private DevExpress.XtraReports.UI.CalculatedField calBankchargesreverser;
        private DevExpress.XtraReports.UI.CalculatedField calunnotifiedcollections;
        private DevExpress.XtraReports.UI.CalculatedField calothercredits;
        private DevExpress.XtraReports.UI.CalculatedField calBankchargeDr;
        private DevExpress.XtraReports.UI.CalculatedField calTransafertootheraccountDr;
        private DevExpress.XtraReports.UI.CalculatedField caloutofperiodcrreveserDR;
        private DevExpress.XtraReports.UI.CalculatedField calcureentperiodcrreserverDR;
        private DevExpress.XtraReports.UI.CalculatedField caloutofperiodchequesreturnDR;
        private DevExpress.XtraReports.UI.CalculatedField calotherunclassifieddebitDR;
        private DevExpress.XtraReports.UI.CalculatedField calOtherDebit;
        private DevExpress.XtraReports.UI.CalculatedField calclosebal;
        private DevExpress.XtraReports.UI.CalculatedField calGenerate;
        private DevExpress.XtraReports.UI.CalculatedField calculatedField2;
        private DevExpress.XtraReports.UI.CalculatedField calRevenue;
        private DevExpress.XtraReports.UI.CalculatedField calPrevCreditReversed;
    }
}
