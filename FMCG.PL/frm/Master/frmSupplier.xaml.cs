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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.AspNet.SignalR.Client;
namespace FMCG.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmUOM.xaml
    /// </summary>
    public partial class frmSupplier : UserControl
    {

        #region Field

        public static string FormName = "Supplier";
        BLL.Supplier data = new BLL.Supplier();

        #endregion

        #region Constructor

        public frmSupplier()
        {
            InitializeComponent();
            this.DataContext = data;

            RptSupplier.SetDisplayMode(DisplayMode.PrintLayout);
            onClientEvents();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvSupplier.ItemsSource = BLL.Supplier.toList;

            CollectionViewSource.GetDefaultView(dgvSupplier.ItemsSource).Filter = Supplier_Filter;
            CollectionViewSource.GetDefaultView(dgvSupplier.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.SupplierName), System.ComponentModel.ListSortDirection.Ascending));


            cmbCreditLimitTypeId.ItemsSource = BLL.CreditLimitType.toList;
            cmbCreditLimitTypeId.DisplayMemberPath = "LimitType";
            cmbCreditLimitTypeId.SelectedValuePath = "Id";

            cmbCityId.ItemsSource = BLL.City.toList;
            cmbCityId.SelectedValuePath = "Id";
            cmbCityId.DisplayMemberPath = "CityName";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(data.SupplierName==null)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Supplier"));
            }
            else if (data.Id == 0 && !BLL.UserAccount.AllowInsert(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyInsert, FormName));
            }
            else if (data.Id != 0 && !BLL.UserAccount.AllowUpdate(FormName))
            {
                MessageBox.Show(string.Format(Message.PL.DenyUpdate, FormName));
            }
            else
            {
                if (data.Save() == true)
                {
                    MessageBox.Show(Message.PL.Saved_Alert);
                    data.Clear();
                }
                else
                {
                    MessageBox.Show(string.Format(Message.PL.Existing_Data, data.SupplierName));
                }
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id != 0)
            {
                if (!BLL.UserAccount.AllowDelete(FormName))
                {
                    MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName));
                }
                else
                {
                    if (MessageBox.Show("Do you want to Delete this record?", "DELETE", MessageBoxButton.YesNo) != MessageBoxResult.No)
                    {
                        if (data.Delete() == true)
                        {
                            MessageBox.Show("Deleted");
                            data.Clear();
                        };
                    }
                }
            }
            else
            {
                MessageBox.Show("No Records to Delete");
            }


        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
        }

        private void dgvSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = dgvSupplier.SelectedItem as BLL.Supplier;
            if (d != null)
            {
                data.Find(d.Id);
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Grid_Refresh();
        }

        private void cbxCase_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptStartWith_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void cbxCase_Unchecked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptContain_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void rptEndWith_Checked(object sender, RoutedEventArgs e)
        {
            Grid_Refresh();
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tc = sender as TabControl;

            if (tc.SelectedIndex == 1)
            {
                LoadReport();
            }

        }

        #endregion

        #region Methods

        private bool Supplier_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.Supplier;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string strSearch = cbxCase.IsChecked == true ? txtSearch.Text : txtSearch.Text.ToLower();
                string strValue = "";

                foreach (var p in d.GetType().GetProperties())
                {
                    if (p.Name.ToLower().Contains("id") || p.GetValue(d) == null) continue;
                    strValue = p.GetValue(d).ToString();
                    if (cbxCase.IsChecked == false)
                    {
                        strValue = strValue.ToLower();
                    }
                    if (rptStartWith.IsChecked == true && strValue.StartsWith(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                    else if (rptContain.IsChecked == true && strValue.Contains(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                    else if (rptEndWith.IsChecked == true && strValue.EndsWith(strSearch))
                    {
                        RValue = true;
                        break;
                    }
                }
            }
            else
            {
                RValue = true;
            }
            return RValue;
        }

        private void Grid_Refresh()
        {
            try
            {
                CollectionViewSource.GetDefaultView(dgvSupplier.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }

        private void LoadReport()
        {
            try
            {
                RptSupplier.Reset();
                ReportDataSource data = new ReportDataSource("Supplier", BLL.Supplier.toList.Where(x => Supplier_Filter(x)).ToList());
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.Company.Id).ToList());
                RptSupplier.LocalReport.DataSources.Add(data);
                RptSupplier.LocalReport.DataSources.Add(data1);
                RptSupplier.LocalReport.ReportPath = @"rpt\master\RptSupplier.rdlc";

                RptSupplier.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.Product>("Product_Save", (prod) => {

                this.Dispatcher.Invoke(() =>
                {
                    prod.Save(true);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On("Product_Delete", (Action<int>)((pk) => {
                this.Dispatcher.Invoke((Action)(() => {
                    BLL.Product prod = new BLL.Product();
                    prod.Find((int)pk);
                    prod.Delete((bool)true);
                }));

            }));
        }

        #endregion
    }
}
