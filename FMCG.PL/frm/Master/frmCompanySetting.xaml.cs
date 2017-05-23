using FMCG.Common;
using Microsoft.AspNet.SignalR.Client;
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

namespace FMCG.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmCompanySetting.xaml
    /// </summary>
    public partial class frmCompanySetting : UserControl
    {
        BLL.CompanyDetail data = new BLL.CompanyDetail();
        public frmCompanySetting()
        {
            InitializeComponent();
            this.DataContext = data;
            
            onClientEvents();
        }



        private void btnSave_Click(object sender, RoutedEventArgs e)
        {


            if (data.Save() == true)
            {
                MessageBox.Show("Saved");

            }

        }

        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.CompanyDetail>("CompanyDetail_Save", (cs) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    cs.Save(true);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On<BLL.UserAccount>("UserAccount_Save", (ua) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    BLL.UserAccount u = new BLL.UserAccount();
                    ua.toCopy<BLL.UserAccount>(u);
                    BLL.UserAccount.toList.Add(u);
                });

            });
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            data.Find(BLL.UserAccount.Company.Id);
            cmbCityId.ItemsSource = BLL.City.toList;
            cmbCityId.DisplayMemberPath = "CityName";
            cmbCityId.SelectedValuePath = "Id";

            var lstCompany = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Warehouse" && x.UnderCompanyId == BLL.UserAccount.Company.Id);
            dgvWarehouse.ItemsSource = lstCompany;
        }

        private void btnNewWareHouse_Click(object sender, RoutedEventArgs e)
        {
            frmCompanySignup f = new frmCompanySignup();
            f.data.UnderCompanyId = BLL.UserAccount.Company.Id;
            f.data.CompanyType = "Warehouse";
            f.ShowDialog();
            var lstCompany = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Warehouse" && x.UnderCompanyId == BLL.UserAccount.Company.Id);
            dgvWarehouse.ItemsSource = lstCompany;
        }

        private void btnNewDealer_Click(object sender, RoutedEventArgs e)
        {
            var cm = dgvWarehouse.SelectedItem as BLL.CompanyDetail;
            if(cm!= null)
            {
                frmCompanySignup f = new frmCompanySignup();
                f.data.UnderCompanyId = cm.Id;
                f.data.CompanyType = "Dealer";
                f.ShowDialog();
                List<BLL.CompanyDetail> lstCompany = new List<BLL.CompanyDetail>();
                if (cm != null)
                {
                    lstCompany = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Dealer" && x.UnderCompanyId == cm.Id).ToList();
                }
                dgvDealer.ItemsSource = lstCompany;
            }            
        }

        private void btnEditWarehouse_Click(object sender, RoutedEventArgs e)
        {
            var cm = dgvWarehouse.SelectedItem as BLL.CompanyDetail;

            frmCompanySignup f = new frmCompanySignup();
            cm.toCopy<BLL.CompanyDetail>(f.data);
            f.data.UnderCompanyId = BLL.UserAccount.Company.Id;
            f.data.CompanyType = "Warehouse";
            f.ShowDialog();
            var lstCompany = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Warehouse" && x.UnderCompanyId == BLL.UserAccount.Company.Id);
            dgvWarehouse.ItemsSource = lstCompany;
        }

        private void btnDeleteWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Do you Delete this?","Delete", MessageBoxButton.YesNo)== MessageBoxResult.Yes)
            {

            }
        }

        private void btnEditDealer_Click(object sender, RoutedEventArgs e)
        {
            var cm = dgvWarehouse.SelectedItem as BLL.CompanyDetail;
            var cmd = dgvDealer.SelectedItem as BLL.CompanyDetail;
            if (cm != null)
            {
                frmCompanySignup f = new frmCompanySignup();
                cmd.toCopy<BLL.CompanyDetail>(f.data);
                f.data.UnderCompanyId = cm.Id;
                f.data.CompanyType = "Dealer";
                f.ShowDialog();
                List<BLL.CompanyDetail> lstCompany = new List<BLL.CompanyDetail>();
                if (cm != null)
                {
                    lstCompany = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Dealer" && x.UnderCompanyId == cm.Id).ToList();
                }
                dgvDealer.ItemsSource = lstCompany;
            }
        }

        private void btnDeleteDealer_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you Delete this?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

            }
        }

        private void dgvWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cm = dgvWarehouse.SelectedItem as BLL.CompanyDetail;
            List<BLL.CompanyDetail> lstCompany = new List<BLL.CompanyDetail>();
            if(cm!= null)
            {
                lstCompany = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Dealer" && x.UnderCompanyId == cm.Id).ToList();
            }
            dgvDealer.ItemsSource = lstCompany;
        }
    }
}