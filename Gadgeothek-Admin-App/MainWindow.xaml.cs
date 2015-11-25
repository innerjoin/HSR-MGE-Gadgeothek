using ch.hsr.wpf.gadgeothek.service;
using System.Configuration;
using MahApps.Metro.Controls;

namespace Gadgeothek_Admin_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            LibraryAdminService service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);
            gadgetsDataGridView.ItemsSource = service.GetAllGadgets();
        }
    }
}
