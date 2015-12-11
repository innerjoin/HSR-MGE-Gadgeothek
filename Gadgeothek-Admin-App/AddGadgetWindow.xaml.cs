using System;
using System.Windows;
using ch.hsr.wpf.gadgeothek.service;
using ch.hsr.wpf.gadgeothek.domain;
using MahApps.Metro.Controls;
using System.Configuration;
using Gadgeothek_Admin_App.ViewModels;

namespace Gadgeothek_Admin_App
{
    /// <summary>
    /// Interaction logic for AddGadgetWindow.xaml
    /// </summary>
    public partial class AddGadgetWindow : MetroWindow
    {
        readonly MainWindow _parentWindow;
        readonly LibraryAdminService _service;
        public AddGadgetWindow()
        {
            InitializeComponent();
            _service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);
            conditionComboBox.ItemsSource = Enum.GetValues(typeof(ch.hsr.wpf.gadgeothek.domain.Condition));
            conditionComboBox.SelectedIndex = 0;
            idTextBlock.Text = GetNewGadgetId();
        }

        public AddGadgetWindow(MainWindow parent) : this()
        {
            _parentWindow = parent;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            double price;
            Gadget gadget = new Gadget
            {
                Condition = (ch.hsr.wpf.gadgeothek.domain.Condition) conditionComboBox.SelectedValue,
                Manufacturer = manufacturerTextBox.Text,
                InventoryNumber = idTextBlock.Text,
                Name = nameTextBox.Text
            };
            double.TryParse(priceTextBox.Text, out price);
            gadget.Price = price;
            GadgetViewModel model = new GadgetViewModel(_service, gadget);
            _parentWindow.GadgetList.Add(model);
            Close();            
        }


        private string GetNewGadgetId()
        {
            int testId = 0;
            foreach (Gadget gadget in _service.GetAllGadgets())
            {
                var id = 0;
                int.TryParse(gadget.InventoryNumber, out id);
                if (id > testId)
                    testId = id;
            }

            return (testId + 1).ToString();
        }


    }
}
