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
    /// Interaction logic for frm.xaml
    /// </summary>
    public partial class frmStaff : UserControl
    {

        #region Field

        public static string FormName = "Staff";
        BLL.Staff data = new BLL.Staff();

        #endregion

        #region Constructor

        public frmStaff()
        {
            InitializeComponent();
            this.DataContext = data;

            RptStaff.SetDisplayMode(DisplayMode.PrintLayout);
            onClientEvents();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvStaff.ItemsSource = BLL.Staff.toList;

            CollectionViewSource.GetDefaultView(dgvStaff.ItemsSource).Filter = Staff_Filter;
            CollectionViewSource.GetDefaultView(dgvStaff.ItemsSource).SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(data.StaffName), System.ComponentModel.ListSortDirection.Ascending));

            cmbCityId.ItemsSource = BLL.City.toList;
            cmbCityId.DisplayMemberPath = "CityName";
            cmbCityId.SelectedValuePath = "Id";

            cmbDesignationId.ItemsSource = BLL.Designation.toList;
            cmbDesignationId.DisplayMemberPath = "DesignationName";
            cmbDesignationId.SelectedValuePath = "Id";

            cmbGenderId.ItemsSource = BLL.Gender.toList;
            cmbGenderId.DisplayMemberPath = "GenderName";
            cmbGenderId.SelectedValuePath = "Id";


        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if(data.StaffName==null)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Staff Name"));
                txtStaffName.Focus();
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
                    MessageBox.Show(string.Format(Message.PL.Existing_Data, data.StaffName));
                    txtStaffName.Focus();
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

        private void dgvStaff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = dgvStaff.SelectedItem as BLL.Staff;
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

        private bool Staff_Filter(object obj)
        {
            bool RValue = false;
            var d = obj as BLL.Staff;

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
                CollectionViewSource.GetDefaultView(dgvStaff.ItemsSource).Refresh();
            }
            catch (Exception ex) { };

        }

        private void LoadReport()
        {
            try
            {
                RptStaff.Reset();
                ReportDataSource data = new ReportDataSource("Staff", BLL.Staff.toList.Where(x => Staff_Filter(x)).ToList());
                ReportDataSource data1 = new ReportDataSource("CompanyDetail", BLL.CompanyDetail.toList.Where(x =>x.Id==BLL.UserAccount.Company.Id).ToList());
                RptStaff.LocalReport.DataSources.Add(data);
                RptStaff.LocalReport.DataSources.Add(data1);
                RptStaff.LocalReport.ReportPath = @"rpt\master\rptStaff.rdlc";

                RptStaff.RefreshReport();

            }
            catch (Exception ex)
            {

            }


        }

        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.Staff>("Staff_Save", (prod) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    prod.Save(true);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On("Staff_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    BLL.Staff prod = new BLL.Staff();
                    prod.Find((int)pk);
                    prod.Delete((bool)true);
                }));

            }));
        }


        #endregion

        private void btnDesignation_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("hi");
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            frmDesignation f = new frmDesignation();
            f.ShowDialog();
        }
    }
}
