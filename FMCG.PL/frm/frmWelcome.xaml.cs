using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace FMCG.PL.frm
{
    /// <summary>
    /// Interaction logic for frmWelcome.xaml
    /// </summary>
    public partial class frmWelcome : UserControl
    {
        
        public frmWelcome()
        {
            InitializeComponent();            
        }
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            dgvStockReport.ItemsSource = BLL.Product.toList;
            dgvReOrderStockReport.ItemsSource = BLL.Product.toList.Where(x=> x.IsReOrderLevel).ToList();

            var l1 = BLL.Product.toList.OrderByDescending(x => x.SQty).Take(5).Select(x => new { x.ProductName,x.SQty, Amount = x.SellingRate * (decimal)(x.SQty ?? 0) }).ToList();            
            dgvTop5Item.ItemsSource = l1;

            var MyValue = l1.Select(x => new KeyValuePair<string, int>(x.ProductName, (int)(x.Amount))).ToList();
            BarChart.DataContext = MyValue;


            var l2 = BLL.Customer.toList.Where(x=> x.BillingAmount!=null).OrderByDescending(x => x.BillingAmount).Take(5).Select(x => new { x.CustomerName, x.BillingAmount }).ToList();
            dgvTop5Customer.ItemsSource = l2;

            var MyValue1 = l2.Select(x => new KeyValuePair<string, int>(x.CustomerName, (int)(x.BillingAmount))).ToList();
            BarChart1.DataContext = MyValue1;

        }
        
    }    
   
}
