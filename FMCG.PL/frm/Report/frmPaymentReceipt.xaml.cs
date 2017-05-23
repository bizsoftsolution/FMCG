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

namespace FMCG.PL.frm.Report
{
    /// <summary>
    /// Interaction logic for frmPaymentReceipt.xaml
    /// </summary>
    public partial class frmPaymentReceipt : UserControl
    {
        public frmPaymentReceipt()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvPR.ItemsSource = BLL.TrialBalance.PRList;
        }
    }
}
