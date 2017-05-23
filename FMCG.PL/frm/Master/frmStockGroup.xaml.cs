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
    public partial class frmStockGroup : UserControl
    {

        #region Field

        public static string FormName = "StockGroup";
        BLL.StockGroup data = new BLL.StockGroup();

        #endregion

        #region Constructor

        public frmStockGroup()
        {
            InitializeComponent();
            this.DataContext = data;

            RptStock.SetDisplayMode(DisplayMode.PrintLayout);
            onClientEvents();

            cmbUnder.ItemsSource = BLL.StockGroup.toList;
            cmbUnder.DisplayMemberPath = "StockGroupName";
            cmbUnder.SelectedValuePath = "Id";
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvStock.ItemsSource = BLL.StockGroup.toList;

            CollectionViewSource.GetDefaultView(dgvStock.ItemsSource).Filter = Stock_Filter;
            CollectionViewSource.GetDefaultView(dgvStock.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.StockGroupName), System.ComponentModel.ListSortDirection.Ascending));

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(data.StockGroupName==null)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Stock group"));
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
                    MessageBox.Show(string.Format(Message.PL.Existing_Data, data.StockGroupName));
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

        private void dgvStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = dgvStock.SelectedItem as BLL.StockGroup;
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

        private bool Stock_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.StockGroup;

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
                CollectionViewSource.GetDefaultView(dgvStock.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }

        private void LoadReport()
        {
            try
            {
                RptStock.Reset();
                ReportDataSource data = new ReportDataSource("StockGroup", BLL.StockGroup.toList.Where(x => Stock_Filter(x)).ToList());
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.Company.Id).ToList());
                RptStock.LocalReport.DataSources.Add(data);
                RptStock.LocalReport.DataSources.Add(data1);
                RptStock.LocalReport.ReportPath = @"rpt\master\rptStock.rdlc";

                RptStock.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.StockGroup>("StockGroup_Save", (stk) => {

                this.Dispatcher.Invoke(() =>
                {
                    stk.Save(true);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On("StockGroup_Delete", (Action<int>)((pk) => {
                this.Dispatcher.Invoke((Action)(() => {
                    BLL.StockGroup stk = new BLL.StockGroup();
                    stk.Find((int)pk);
                    stk.Delete((bool)true);
                }));

            }));
        }

        #endregion
    }
}
