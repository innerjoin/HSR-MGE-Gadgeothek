using ch.hsr.wpf.gadgeothek.service;
using System.Configuration;
using System.Windows;
using MahApps.Metro.Controls;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;
using System;
using ch.hsr.wpf.gadgeothek.domain;
using System.Collections.Generic;

namespace Gadgeothek_Admin_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        LibraryAdminService service;
        string customerId;
        public MainWindow()
        {
            InitializeComponent();
            service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);

            gadgetsDataGridView.ItemsSource = service.GetAllGadgets();
            enableDisableButtons(false);
            deleteLendingButton.IsEnabled = false;
            deleteReservationButton.IsEnabled = false;
            customerDataGrid.ItemsSource = service.GetAllCustomers();
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
                var yourCostumFilter = new Predicate<object>(filterGadgets);

                Itemlist.Filter = yourCostumFilter;

                gadgetsDataGridView.ItemsSource = Itemlist;
            }           
        }

        private bool filterGadgets(object item)
        {
            return ((Gadget)item).Name.Contains(searchTextBox.Text) || ((Gadget)item).Manufacturer.Contains(searchTextBox.Text) || ((Gadget)item).Price.ToString().Contains(searchTextBox.Text) || ((Gadget)item).Condition.ToString().Contains(searchTextBox.Text) || ((Gadget)item).InventoryNumber.ToString().Contains(searchTextBox.Text);
        }

        private void eEditGadgetButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addGadgetButton_Click(object sender, RoutedEventArgs e)
        {
            AddGadgetWindow addWindow = new AddGadgetWindow();
            addWindow.Show();
            gadgetsDataGridView.ItemsSource = service.GetAllGadgets();
        }

        private void deleteLendingButton_Click(object sender, RoutedEventArgs e)
        {
            if (lendingDataGridView.SelectedItem != null)
            {
                service.DeleteLoan((Loan)lendingDataGridView.SelectedItem);
            }
        }

        private void deleteReservationButton_Click(object sender, RoutedEventArgs e)
        {
            if(reservationDataGridView.SelectedItem != null)
            {
                service.DeleteReservation((Reservation)reservationDataGridView.SelectedItem);
            }
        }

        private void addReservationButton_Click(object sender, RoutedEventArgs e)
        {
            if(customerDataGrid.SelectedItem != null)
            {
                Reservation res = new Reservation();
                res.CustomerId = customerId;
                res.Finished = false;
                res.Gadget = (Gadget)newReservationComboBox.SelectedItem;
                res.ReservationDate = DateTime.Now;
                res.WaitingPosition = getWaitingPosition(res.Gadget);
                service.AddReservation(res);
            }
        }

        private void addLendingButton_Click(object sender, RoutedEventArgs e)
        {
            if (customerDataGrid.SelectedItem != null)
            {
                Reservation res = new Reservation();
                res.CustomerId = customerId;
                res.Finished = false;
                res.Gadget = (Gadget)newReservationComboBox.SelectedItem;
                res.ReservationDate = DateTime.Now;
                res.WaitingPosition = getWaitingPosition(res.Gadget);
                service.AddReservation(res);
            }
        }

        private void customerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
            {
                customerId = ((Customer)item).Studentnumber;
            }
            reservationDataGridView.ItemsSource = getReservationForCustomer(customerId);
            newReservationComboBox.ItemsSource = getAvailableGadgetsForCustomer(customerId);
            lendingDataGridView.ItemsSource = getLoansForCustomer(customerId);

            enableDisableButtons(true);
        }

        private List<Gadget> getGadgetsForCustomer(string customerId)
        {
            List<Gadget> liste = new List<Gadget>();
            foreach (Reservation item in service.GetAllReservations())
            {
                if(item.CustomerId == customerId)
                {
                    liste.Add(item.Gadget);
                }
            }

            return liste;
        }

        private List<Reservation> getReservationForCustomer(string customerId)
        {
            List<Reservation> liste = new List<Reservation>();
            foreach (Reservation item in service.GetAllReservations())
            {
                if (item.CustomerId == customerId)
                {
                    liste.Add(item);
                }
            }

            return liste;
        }

        private List<Loan> getLoansForCustomer(string customerId)
        {
            List<Loan> liste = new List<Loan>();
            foreach (Loan item in service.GetAllLoans())
            {
                if (item.CustomerId == customerId)
                {
                    liste.Add(item);
                }
            }

            return liste;
        }

        private List<Gadget> getAvailableGadgetsForCustomer(string customerId)
        {
            List<Gadget> availableGadgets = service.GetAllGadgets();
            foreach (Gadget item in getGadgetsForCustomer(customerId))
            {
                availableGadgets.Remove(item);
            }

            return availableGadgets;
        }

        private int getWaitingPosition(Gadget item)
        {
            int waitingPosition = -1;
            foreach (Reservation res in service.GetAllReservations())
            {
                if (res.Gadget == item)
                {
                    if (waitingPosition < res.WaitingPosition)
                    {
                        waitingPosition = res.WaitingPosition;
                    }
                }
            }

            return waitingPosition + 1; 
        }

        private void enableDisableButtons(bool input)
        {
            addLendingButton.IsEnabled = input;
            addReservationButton.IsEnabled = input;
            newReservationComboBox.IsEnabled = input;
            newLendingComboBox.IsEnabled = input;
        }

        private void reservationDataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            deleteReservationButton.IsEnabled = true;
        }

        private void lendingDataGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            deleteLendingButton.IsEnabled = true;
        }
    }
}
