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

namespace FMCG.PL.frm.Transaction
{
    /// <summary>
    /// Interaction logic for frmReceipt.xaml
    /// </summary>
    public partial class frmReceipt : UserControl
    {
        BLL.Receipt Data = new BLL.Receipt();

        public frmReceipt()
        {
            InitializeComponent();
            this.DataContext = Data;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var rv = Data.Save();
            if (rv == true)
            {
                MessageBox.Show("Saved");
                Data.Clear();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(Message.PL.TransactionDeleteConfirmation, Data.RefNo), "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var rv = Data.Delete();
                if (rv == true)
                {
                    MessageBox.Show("Deleted");
                    Data.Clear();
                }
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Data.Clear();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            var rv = Data.Find();
            if (rv == false) MessageBox.Show(String.Format("{0} is not found", Data.SearchText));
        }

        private void dgvCustomerDetails_CurrentCellChanged(object sender, EventArgs e)
        {
            Data.SetTotalAmount();
        }

        private void dgvSupplierDetails_CurrentCellChanged(object sender, EventArgs e)
        {
            Data.SetTotalAmount();
        }
    }
}
