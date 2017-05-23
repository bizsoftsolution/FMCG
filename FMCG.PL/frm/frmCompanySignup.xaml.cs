using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace FMCG.PL.frm
{
    /// <summary>
    /// Interaction logic for frmCompanySignup.xaml
    /// </summary>
    public partial class frmCompanySignup : MetroWindow
    {
        public BLL.CompanyDetail data = new BLL.CompanyDetail();
       
        public frmCompanySignup()
        {
            InitializeComponent();
            this.DataContext = data;
            cmbCityId.ItemsSource = BLL.City.toList;
            cmbCityId.DisplayMemberPath = "CityName";
            cmbCityId.SelectedValuePath = "Id";
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            data.Clear();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Save() == true)
            {
                MessageBox.Show("Saved");
                data.Clear();
            }
        }
    }
}
