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
using Microsoft.Build.Framework.XamlTypes;
using ch.hsr.wpf.gadgeothek.service;
using ch.hsr.wpf.gadgeothek.domain;
using System.Configuration;
namespace Gadgeothek_Admin_App
{
    /// <summary>
    /// Interaction logic for AddGadgetWindow.xaml
    /// </summary>
    public partial class AddGadgetWindow : Window
    {
        public AddGadgetWindow()
        {
            InitializeComponent();
            conditionComboBox.ItemsSource = Enum.GetValues(typeof(ch.hsr.wpf.gadgeothek.domain.Condition));
            conditionComboBox.SelectedIndex = 0;
            idTextBlock.Text = Guid.NewGuid().ToString();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            LibraryAdminService service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);
            double price = 0.0;
            Gadget gadget = new Gadget();
            gadget.Condition = (ch.hsr.wpf.gadgeothek.domain.Condition)conditionComboBox.SelectedValue;
            gadget.Manufacturer = manufacturerTextBox.Text;
            gadget.InventoryNumber = idTextBlock.Text;
            gadget.Name = nameTextBox.Text;
            Double.TryParse(priceTextBox.Text, out price);
            gadget.Price = price;
            service.AddGadget(gadget);
            this.Close();            
        }
    }
}
