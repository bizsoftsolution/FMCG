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
    public partial class frmBank : UserControl
    {

        #region Field

        public static string FormName = "Bank";
        BLL.Bank data = new BLL.Bank();

        #endregion

        #region Constructor

        public frmBank()
        {
            InitializeComponent();
            this.DataContext = data;

            RptBank.SetDisplayMode(DisplayMode.PrintLayout);
            onClientEvents();


        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvBank.ItemsSource = BLL.Bank.toList;

            CollectionViewSource.GetDefaultView(dgvBank.ItemsSource).Filter = Bank_Filter;
            CollectionViewSource.GetDefaultView(dgvBank.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.BankName), System.ComponentModel.ListSortDirection.Ascending));

            cmbCityId.ItemsSource = BLL.City.toList;
            cmbCityId.SelectedValuePath = "Id";
            cmbCityId.DisplayMemberPath = "CityName";

            
            cmbAccountTypeId.ItemsSource = BLL.BankAccountType.toList;
            cmbAccountTypeId.SelectedValuePath = "Id";
            cmbAccountTypeId.DisplayMemberPath = "AccountType";

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(data.BankName==null)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Bank Name"));
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
                    MessageBox.Show(string.Format(Message.PL.Existing_Data, data.BankName));
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

        private void dgvBank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = dgvBank.SelectedItem as BLL.Bank;
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

        private bool Bank_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.Bank;

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
                CollectionViewSource.GetDefaultView(dgvBank.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }

        private void LoadReport()
        {
            try
            {
                RptBank.Reset();
                ReportDataSource data = new ReportDataSource("Bank", BLL.Bank.toList.Where(x => Bank_Filter(x)).ToList());
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.Company.Id).ToList());
                RptBank.LocalReport.DataSources.Add(data);
                RptBank.LocalReport.DataSources.Add(data1);
                RptBank.LocalReport.ReportPath = @"rpt\master\rptBank.rdlc";

                RptBank.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.Bank>("Bank_Save", (bank) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    bank.Save(true);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On("Bank_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    BLL.Bank bank = new BLL.Bank();
                    bank.Find((int)pk);
                    bank.Delete((bool)true);
                }));

            }));
        }


        #endregion

        private void txtBankName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
