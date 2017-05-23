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
    /// Interaction logic for frmLedgerOpening.xaml
    /// </summary>
    public partial class frmLedgerOpening : UserControl
    {
        BLL.LedgerOpening data = new BLL.LedgerOpening();
        public frmLedgerOpening()
        {
            InitializeComponent();
            this.DataContext = data;
            dgvLedgerOpening.ItemsSource = BLL.LedgerOpening.toList;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.Save() == true)
            {
                MessageBox.Show("Record Updated");
                data.Clear();
            }
        }

        private void dgvLedgerOpening_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var v = dgvLedgerOpening.SelectedItem as BLL.LedgerOpening;
            data.EntityId = v.EntityId;
            data.EntityType = v.EntityType;
            if (v != null)
            {
                if (data.Save() == true)
                {
                    data.Find(v.Id);
                }

            }
           
        }

           }
}
