using System;
using System.Windows;
using ch.hsr.wpf.gadgeothek.service;
using ch.hsr.wpf.gadgeothek.domain;
using MahApps.Metro.Controls;
using System.Configuration;
using Gadgeothek_Admin_App.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;

namespace Gadgeothek_Admin_App
{
    /// <summary>
    /// Interaction logic for AddGadgetWindow.xaml
    /// </summary>
    public partial class AddGadgetWindow : MetroWindow
    {
        MainWindow parentWindow;
        LibraryAdminService _service;
        public AddGadgetWindow()
        {
            InitializeComponent();
            _service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);
            conditionComboBox.ItemsSource = Enum.GetValues(typeof(ch.hsr.wpf.gadgeothek.domain.Condition));
            conditionComboBox.SelectedIndex = 0;
            idTextBlock.Text = getNewGadgetId().ToString();
        }

        public AddGadgetWindow(MainWindow parent) : this()
        {
            this.parentWindow = parent;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            double price = 0.0;
            Gadget gadget = new Gadget();
            gadget.Condition = (ch.hsr.wpf.gadgeothek.domain.Condition)conditionComboBox.SelectedValue;
            gadget.Manufacturer = manufacturerTextBox.Text;
            gadget.InventoryNumber = idTextBlock.Text;
            gadget.Name = nameTextBox.Text;
            Double.TryParse(priceTextBox.Text, out price);
            gadget.Price = price;
            GadgetViewModel model = new GadgetViewModel(_service, gadget);
            parentWindow.GadgetList.Add(model);
            this.Close();            
        }


        private string getNewGadgetId()
        {
            int id = 0;
            int testId = 0;
            foreach (Gadget gadget in _service.GetAllGadgets())
            {
                Int32.TryParse(gadget.InventoryNumber, out id);
                if (id > testId)
                    testId = id;
            }

            return (testId + 1).ToString();
        }


    }
}
