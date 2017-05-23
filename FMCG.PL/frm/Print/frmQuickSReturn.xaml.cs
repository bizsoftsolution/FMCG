﻿using MahApps.Metro.Controls;
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
    /// Interaction logic for frmQuickSReturn.xaml
    /// </summary>
    public partial class frmQuickSReturn : MetroWindow
    {
        public frmQuickSReturn()
        {
            InitializeComponent();
            rptQuickSalesReturn.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.SalesReturn data)
        {
            try
            {

                List<BLL.SalesReturn> POList = new List<BLL.SalesReturn>();
                List<BLL.SalesReturnDetail> PODList = new List<BLL.SalesReturnDetail>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();

                POList.Add(data);
                PODList.AddRange(data.SRDetails);
                CList.Add(BLL.UserAccount.Company);


                rptQuickSalesReturn.Reset();
                ReportDataSource data1 = new ReportDataSource("SalesReturn", POList);
                ReportDataSource data2 = new ReportDataSource("SalesReturnDetail", PODList);
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);

                rptQuickSalesReturn.LocalReport.DataSources.Add(data1);
                rptQuickSalesReturn.LocalReport.DataSources.Add(data2);
                rptQuickSalesReturn.LocalReport.DataSources.Add(data3);
                rptQuickSalesReturn.LocalReport.ReportPath = @"rpt\Transaction\rptSalesReturn.rdlc";

                rptQuickSalesReturn.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }

    }
}
