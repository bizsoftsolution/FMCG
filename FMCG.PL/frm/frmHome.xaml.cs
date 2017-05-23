using FMCG.Common;
using MahApps.Metro.Controls;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FMCG.PL.frm
{
    /// <summary>
    /// Interaction logic for frmHome.xaml
    /// </summary>
    public partial class frmHome : MetroWindow
    {
        public frmHome()
        {
            InitializeComponent();
            ccContent.Content = new frmWelcome();
            onClientEvents();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-MY");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-MY");            
        }
        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.PurchaseOrder>("PurchaseOrder_POPendingSave", (PO) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    BLL.PurchaseOrder B_PO = BLL.PurchaseOrder.POPendingList.Where(x => x.Id == PO.Id).FirstOrDefault();
                    if(B_PO== null)
                    {
                        B_PO = new BLL.PurchaseOrder();
                        BLL.PurchaseOrder.POPendingList.Add(B_PO);
                    }

                    PO.toCopy<BLL.PurchaseOrder>(B_PO);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On<long>("PurchaseOrder_POPendingDelete", (PK) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    BLL.PurchaseOrder PO = BLL.PurchaseOrder.POPendingList.Where(x => x.Id == PK).FirstOrDefault();
                    BLL.PurchaseOrder.POPendingList.Remove(PO);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On<BLL.SalesOrder>("SalesOrder_SOPendingSave", (SO) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    BLL.SalesOrder B_SO = BLL.SalesOrder.SOPendingList.Where(x => x.Id == SO.Id).FirstOrDefault();
                    if (B_SO == null)
                    {
                        B_SO = new BLL.SalesOrder();
                        BLL.SalesOrder.SOPendingList.Add(B_SO);
                    }

                    SO.toCopy<BLL.SalesOrder>(B_SO);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On<long>("SalesOrder_SOPendingDelete", (PK) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    BLL.SalesOrder SO = BLL.SalesOrder.SOPendingList.Where(x => x.Id == PK).FirstOrDefault();
                    if(SO!= null) BLL.SalesOrder.SOPendingList.Remove(SO);
                });

            });

        }

        private void ListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var dependencyObject = Mouse.Captured as DependencyObject;
                while (dependencyObject != null)
                {
                    if (dependencyObject is ScrollBar) return;
                    dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
                }
                ListBox lb = sender as ListBox;
                Common.FMCGMenuItem mi = lb.SelectedItem as Common.FMCGMenuItem;
                if (!BLL.UserAccount.AllowFormShow(mi.FormName))
                {
                    
                    MessageBox.Show(string.Format(Message.PL.DenyFormShow, mi.MenuName));
                }
                else
                {
                    ccContent.Content = mi.Content;                    
                }                
            }
            catch (Exception ex) { }
            MenuToggleButton.IsChecked = false;
        }

    }
}
