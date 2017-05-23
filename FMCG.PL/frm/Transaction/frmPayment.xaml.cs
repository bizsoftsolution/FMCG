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
    /// Interaction logic for frmPayment.xaml
    /// </summary>
    public partial class frmPayment : UserControl
    {

        BLL.Payment Data = new BLL.Payment();

        public frmPayment()
        {
            InitializeComponent();
            this.DataContext = Data;            
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(Data.RefNo==null)
            {
                MessageBox.Show(Message.PL.Transaction_RefNo_Validation);
            }
            else if(Data.PayTo==null)
            {
                MessageBox.Show(Message.PL.Payment_PayTo_Empty);
            }
            else if(Data.Amount==null)
            {
                MessageBox.Show(Message.PL.Payment_Amount_Empty);
            }
          
            else if (Data.FindRefNo() == false)
            {
                var rv = Data.Save();

                if (rv == true)
                {
                    MessageBox.Show(Message.PL.Saved_Alert);
                    Data.Clear();
                }
            }
            else
            {
                MessageBox.Show(Message.PL.Transaction_RefNo_ExistValidation, Data.RefNo);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(string.Format(Message.PL.TransactionDeleteConfirmation, Data.RefNo), "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var rv = Data.Delete();
                if (rv == true)
                {
                    MessageBox.Show(Message.PL.Delete_Alert);
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
            frm.Print.frmQuickPayment f = new Print.frmQuickPayment();
            string payto = "";

            if (cmbBank.Text != "")
            {
                payto = cmbBank.Text;
            }
            else if (cmbCustomer.Text != "")
            {
                payto = cmbCustomer.Text;
            }
            else if (cmbSupplier.Text != "")
            {
                payto = cmbSupplier.Text;
            }
            else if (cmbLedger.Text != "")
            {
                payto = cmbLedger.Text;
            }
            else if (cmbStaff.Text != "")
            {
                payto = cmbStaff.Text;
            }

            f.LoadReport(Data, payto.ToString(), txtChequeNo.Text, lblAmountInWords.Text);
            f.ShowDialog();
        }

        private void btnsearch_Click(object sender, RoutedEventArgs e)
        {
            var rv = Data.Find();
            if (rv == false) MessageBox.Show(String.Format(Message.PL.Data_Not_Found, Data.SearchText));
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
