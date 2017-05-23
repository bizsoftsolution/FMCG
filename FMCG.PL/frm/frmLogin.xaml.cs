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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FMCG.PL.frm
{
    /// <summary>
    /// Interaction logic for frmLogin.xaml
    /// </summary>
    public partial class frmLogin : MetroWindow
    {
        public frmLogin()
        {
            InitializeComponent();
            //cmbYear.ItemsSource = BLL.YearList.toList;
            //cmbYear.SelectedValuePath = "AccountYear";
            //cmbYear.DisplayMemberPath = "AccountYear";

            var l1 = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Company").ToList();
            cmbCompany.ItemsSource = l1;
            cmbCompany.SelectedValuePath = "Id";
            cmbCompany.DisplayMemberPath = "CompanyName";

            cmbCompanyWarehousePrimay.ItemsSource = l1;
            cmbCompanyWarehousePrimay.SelectedValuePath = "Id";
            cmbCompanyWarehousePrimay.DisplayMemberPath = "CompanyName";

            cmbCompanyDealerPrimay.ItemsSource = l1;
            cmbCompanyDealerPrimay.SelectedValuePath = "Id";
            cmbCompanyDealerPrimay.DisplayMemberPath = "CompanyName";


            onClientEvents();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-MY");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-MY");

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
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (BLL.UserAccount.Login("", cmbCompany.Text, txtUserId.Text, txtPassword.Password) == true)
            {
                frmHome f = new frmHome();                
                f.Title = String.Format("{0} - {1}",  BLL.UserAccount.User.UserName, BLL.UserAccount.Company.CompanyName);
                this.Hide();
                cmbCompany.Text = "";
                txtUserId.Text = "";
                txtPassword.Password = "";
                f.ShowDialog();
                this.Show();
                cmbCompany.Focus();
            }
            else
            {
                MessageBox.Show("Invalid User");
            }
        }

        private void btnSignup_Click(object sender, RoutedEventArgs e)
        {
            frmCompanySignup f = new frmCompanySignup();
            f.ShowDialog();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            cmbCompany.Text = "";
            txtUserId.Text = "";
            txtPassword.Password = "";

        }

        private void btnLoginWarehouse_Click(object sender, RoutedEventArgs e)
        {
            if (BLL.UserAccount.Login("", cmbCompanyWarehouse.Text, txtUserIdWarehouse.Text, txtPasswordWarehouse.Password) == true)
            {
                frmHome f = new frmHome();
                f.Title = String.Format("{0} - {1}", BLL.UserAccount.User.UserName, BLL.UserAccount.Company.CompanyName);
                this.Hide();
                cmbCompanyWarehousePrimay.Text = "";
                cmbCompanyWarehouse.Text = "";
                txtUserIdWarehouse.Text = "";
                txtPasswordWarehouse.Password = "";
                f.ShowDialog();
                this.Show();
                cmbCompanyWarehousePrimay.Focus();
            }
            else
            {
                MessageBox.Show("Invalid User");
            }
        }

        private void btnClearWarehouse_Click(object sender, RoutedEventArgs e)
        {
            cmbCompanyWarehousePrimay.Text = "";
            cmbCompanyWarehouse.Text = "";
            txtUserIdWarehouse.Text = "";
            txtPasswordWarehouse.Password = "";
        }

        private void btnLoginDealer_Click(object sender, RoutedEventArgs e)
        {
            if (BLL.UserAccount.Login("", cmbCompanyDealer.Text, txtUserIdDealer.Text, txtPasswordDealer.Password) == true)
            {
                frmHome f = new frmHome();
                f.Title = String.Format("{0} - {1}", BLL.UserAccount.User.UserName, BLL.UserAccount.Company.CompanyName);
                this.Hide();
                cmbCompanyDealerPrimay.Text = "";
                cmbCompanyDealerWarehouse.Text = "";
                cmbCompanyDealer.Text = "";

                txtUserIdDealer.Text = "";
                txtPasswordDealer.Password = "";
                f.ShowDialog();
                this.Show();
                cmbCompanyDealerPrimay.Focus();
            }
            else
            {
                MessageBox.Show("Invalid User");
            }
        }

        private void btnClearDealer_Click(object sender, RoutedEventArgs e)
        {
            cmbCompanyDealerPrimay.Text = "";
            cmbCompanyDealerWarehouse.Text = "";
            cmbCompanyDealer.Text = "";
            txtUserIdDealer.Text = "";
            txtPasswordDealer.Password = "";
        }

        private void cmbCompanyWarehouse_GotFocus(object sender, RoutedEventArgs e)
        {
            var cm = cmbCompanyWarehousePrimay.SelectedItem as BLL.CompanyDetail;
            List<BLL.CompanyDetail> lstCom = new List<BLL.CompanyDetail>();
            if(cm!= null)
            {
                lstCom = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Warehouse" && x.UnderCompanyId==cm.Id).ToList();
            }
            cmbCompanyWarehouse.ItemsSource = lstCom;
            cmbCompanyWarehouse.SelectedValuePath = "Id";
            cmbCompanyWarehouse.DisplayMemberPath = "CompanyName";
        }

        private void cmbCompanyDealerWarehouse_GotFocus(object sender, RoutedEventArgs e)
        {
            var cm = cmbCompanyDealerPrimay.SelectedItem as BLL.CompanyDetail;
            List<BLL.CompanyDetail> lstCom = new List<BLL.CompanyDetail>();
            if (cm != null)
            {
                lstCom = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Warehouse" && x.UnderCompanyId == cm.Id).ToList();                
            }
            cmbCompanyDealerWarehouse.ItemsSource = lstCom;
            cmbCompanyDealerWarehouse.SelectedValuePath = "Id";
            cmbCompanyDealerWarehouse.DisplayMemberPath = "CompanyName";
        }

        private void cmbCompanyDealer_GotFocus(object sender, RoutedEventArgs e)
        {
            var cm = cmbCompanyDealerWarehouse.SelectedItem as BLL.CompanyDetail;
            List<BLL.CompanyDetail> lstCom = new List<BLL.CompanyDetail>();
            if (cm != null)
            {
                lstCom = BLL.CompanyDetail.toList.Where(x => x.CompanyType == "Dealer" && x.UnderCompanyId == cm.Id).ToList();
            }
            cmbCompanyDealer.ItemsSource = lstCom;
            cmbCompanyDealer.SelectedValuePath = "Id";
            cmbCompanyDealer.DisplayMemberPath = "CompanyName";
        }
    }
}
