using ch.hsr.wpf.gadgeothek.service;
using System.Configuration;
using System.Windows;
using MahApps.Metro.Controls;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;
using System;
using ch.hsr.wpf.gadgeothek.domain;

namespace Gadgeothek_Admin_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        LibraryAdminService service;
        public MainWindow()
        {
            InitializeComponent();
            service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);
            gadgetsDataGridView.ItemsSource = service.GetAllGadgets();
        }

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "";
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(service != null)
            {
                var _itemSourceList = new CollectionViewSource() { Source = service.GetAllGadgets() };
                ICollectionView Itemlist = _itemSourceList.View;
                var yourCostumFilter = new Predicate<object>(item => ((Gadget)item).Name.Contains(((TextBox)sender).Text));

                Itemlist.Filter = yourCostumFilter;

                gadgetsDataGridView.ItemsSource = Itemlist;
            }
            /*string InventoryNumber;
            Condition Condition;
            double Price;
            string Manufacturer;
            string Name;*/            
        }
    }
}
