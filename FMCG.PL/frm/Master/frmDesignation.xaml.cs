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

namespace FMCG.PL.frm.Master
{
    /// <summary>
    /// Interaction logic for frmDesignation.xaml
    /// </summary>
    public partial class frmDesignation : MetroWindow
    {

        #region Field

        public static string FormName = "Designation";
        BLL.Designation data = new BLL.Designation();

        #endregion

        #region Constructor

        public frmDesignation()
        {
            InitializeComponent();
            this.DataContext = data;

            onClientEvents();
        }

        #endregion

        #region Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dgvDetail.ItemsSource = BLL.Designation.toList;

            cmbUserType.ItemsSource = BLL.UserType.toList;
            cmbUserType.DisplayMemberPath = "TypeOfUser";
            cmbUserType.SelectedValuePath = "Id";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (data.DesignationName == null)
            {
                MessageBox.Show(string.Format(Message.PL.Empty_Record, "Designation"));
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
                    MessageBox.Show(string.Format(Message.PL.Existing_Data, data.DesignationName));
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

        private void dgvDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var d = dgvDetail.SelectedItem as BLL.Designation;
            if (d != null)
            {
                data.Find(d.Id);
            }
        }



        #endregion

        #region Methods


        private void onClientEvents()
        {
            BLL.FMCGHubClient.FMCGHub.On<BLL.Designation>("designation_Save", (uom) =>
            {

                this.Dispatcher.Invoke(() =>
                {
                    uom.Save(true);
                });

            });

            BLL.FMCGHubClient.FMCGHub.On("designation_Delete", (Action<int>)((pk) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    BLL.Designation uom = new BLL.Designation();
                    uom.Find((int)pk);
                    uom.Delete((bool)true);
                }));

            }));
        }

        #endregion



        private void btnUserTypeSetting_Click(object sender, RoutedEventArgs e)
        {
            frmUserType f = new frmUserType();
            f.ShowDialog();
        }


    }
}
