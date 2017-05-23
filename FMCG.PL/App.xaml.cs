using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FMCG.PL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {            
            BLL.FMCGHubClient.HubConnect();
            frm.frmLogin f = new frm.frmLogin();           
            f.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            BLL.FMCGHubClient.HubDisconnect();
        }

    }
}
