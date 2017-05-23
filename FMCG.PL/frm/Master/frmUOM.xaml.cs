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
using MaterialDesignThemes.Wpf;

namespace FMCG.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmUOM.xaml
    /// </summary>
    public partial class frmUOM : UserControl
    {

        #region Field

        public static string FormName = "UOM";
        BLL.UOM data = new BLL.UOM();

        #endregion

        #region Constructor

        public frmUOM()
        {
            InitializeComponent();
            this.DataContext = data;
            RptUOM.SetDisplayMode(DisplayMode.PrintLayout);
            onClientEvents();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvUOM.ItemsSource = BLL.UOM.toList;

            CollectionViewSource.GetDefaultView(dgvUOM.ItemsSource).Filter = UOM_Filter;
            CollectionViewSource.GetDefaultView(dgvUOM.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.Symbol), System.ComponentModel.ListSortDirection.Ascending));

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Symbol == "")
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Symbol"));
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
                    MessageBox.Show(string.Format(Message.PL.Existing_Data, data.Symbol));
                }
            }



        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (data.Id == 0) MessageBox.Show("No Records to Delete");
            else
            {
                if (!BLL.UserAccount.AllowDelete(FormName)) MessageBox.Show(string.Format(Message.PL.DenyDelete, FormName));
                else if (MessageBox.Show("Do you want to Delete this record?", "DELETE", MessageBoxButton.YesNo) != MessageBoxResult.No)
                {
                    if (data.Delete() == true)
                    {
                        MessageBox.Show("Deleted");
                        data.Clear();
                    }
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
        }

        private void dgvUOM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = dgvUOM.SelectedItem as BLL.UOM;
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

        private bool UOM_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.UOM;

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
                CollectionViewSource.GetDefaultView(dgvUOM.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }

        private void LoadReport()
        {
            try
            {
                RptUOM.Reset();
                ReportDataSource data = new ReportDataSource("UOM", BLL.UOM.toList.Where(x => UOM_Filter(x)).ToList());
                ReportDataSource data1 = new ReportDataSource("CompanyDetails", BLL.CompanyDetail.toList.Where(x => x.Id == BLL.UserAccount.Company.Id));
                RptUOM.LocalReport.DataSources.Add(data);
                RptUOM.LocalReport.DataSources.Add(data1);
                RptUOM.LocalReport.ReportPath = @"rpt\master\rptUOM.rdlc";

                RptUOM.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.UOM>("UOM_Save", (uom) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    uom.Save(true);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On("UOM_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    BLL.UOM uom = new BLL.UOM();
                    uom.Find((int)pk);
                    uom.Delete((bool)true);
                }));

            }));
        }

        #endregion
    }
}
