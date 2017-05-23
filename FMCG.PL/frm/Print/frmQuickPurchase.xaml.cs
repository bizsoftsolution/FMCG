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
    /// Interaction logic for frmQuickPurchase.xaml
    /// </summary>
    public partial class frmQuickPurchase : MetroWindow
    {
        public frmQuickPurchase()
        {
            InitializeComponent();
            rptQuickPurchase.SetDisplayMode(DisplayMode.PrintLayout);
        }
        public void LoadReport(BLL.Purchase data)
        {
            try
            {

                List<BLL.Purchase> POList = new List<BLL.Purchase>();
                List<BLL.PurchaseDetail> PODList = new List<BLL.PurchaseDetail>();
                List<BLL.CompanyDetail> CList = new List<BLL.CompanyDetail>();

                POList.Add(data);
                PODList.AddRange(data.PDetails);
                CList.Add(BLL.UserAccount.Company);


                rptQuickPurchase.Reset();
                ReportDataSource data1 = new ReportDataSource("Purchase", POList);
                ReportDataSource data2 = new ReportDataSource("PurchaseDetail", PODList);
                ReportDataSource data3 = new ReportDataSource("CompanyDetail", CList);

                rptQuickPurchase.LocalReport.DataSources.Add(data1);
                rptQuickPurchase.LocalReport.DataSources.Add(data2);
                rptQuickPurchase.LocalReport.DataSources.Add(data3);
                rptQuickPurchase.LocalReport.ReportPath = @"rpt\Transaction\rptPurchase.rdlc";

                rptQuickPurchase.RefreshReport();

            }
            catch (Exception ex)
            {

            }
        }

    }
}
