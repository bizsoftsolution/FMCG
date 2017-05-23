using MahApps.Metro.Controls;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FMCG.PL.frm.Print
{
    /// <summary>
    /// Interaction logic for frmQuickSO.xaml
    /// </summary>
    public partial class frmQuickSO : MetroWindow
    {
        public frmQuickSO()
        {
            InitializeComponent();
            rptQuickSalesOrder.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.SalesOrder data)
        {
            try
            {

                List<BLL.SalesOrder> POList = new List<BLL.SalesOrder>();
                List<BLL.SalesOrderDetail> PODList = new List<BLL.SalesOrderDetail>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();

                POList.Add(data);
                PODList.AddRange(data.SODetails);
                CList.Add(BLL.UserAccount.Company);


                rptQuickSalesOrder.Reset();
                ReportDataSource data1 = new ReportDataSource("SalesOrder", POList);
                ReportDataSource data2 = new ReportDataSource("SalesOrderDetail", PODList);
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);

                rptQuickSalesOrder.LocalReport.DataSources.Add(data1);
                rptQuickSalesOrder.LocalReport.DataSources.Add(data2);
                rptQuickSalesOrder.LocalReport.DataSources.Add(data3);
                rptQuickSalesOrder.LocalReport.ReportPath = @"rpt\Transaction\rptQuickSalesOrder.rdlc";

                rptQuickSalesOrder.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }

    }
}
