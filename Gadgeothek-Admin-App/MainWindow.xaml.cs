using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ch.hsr.wpf.gadgeothek.service;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using ch.hsr.wpf.gadgeothek.domain;
using Gadgeothek_Admin_App.ViewModels;
using MahApps.Metro.Controls;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;

namespace Gadgeothek_Admin_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [SuppressMessage("ReSharper", "SuggestVarOrType_Elsewhere")]
    public partial class MainWindow : MetroWindow
    {
        public ObservableCollection<CustomerViewModel> CustomerList { get; set; }
        public ObservableCollection<GadgetViewModel> GadgetList { get; set; }
        public ObservableCollection<ReservationViewModel> ReservationList { get; set; }
        public ObservableCollection<LoanViewModel> LoanList { get; set; }
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
            List<Gadget> gadgetData = service.GetAllGadgets();
            List<Customer> customerData = service.GetAllCustomers();
            List<Reservation> reservationData = service.GetAllReservations();
            List<Loan> loanData = service.GetAllLoans();

            GadgetList = gadgetData != null ? new ObservableCollection<GadgetViewModel>(GetViewGadgets(gadgetData)): null;
            CustomerList = customerData != null ? new ObservableCollection<CustomerViewModel>(GetViewCustomers(customerData)) : null;
            ReservationList = reservationData != null ? new ObservableCollection<ReservationViewModel>(GetViewReservations(reservationData)) : null;
            LoanList = loanData != null ? new ObservableCollection<LoanViewModel>(GetViewLoans(loanData)) : null;

            DataContext = this;

            if (GadgetList != null)
            {
                GadgetList.CollectionChanged +=
                    new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);
            }

            if (CustomerList != null)
            {
                CustomerList.CollectionChanged +=
                    new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);
            }

            if (ReservationList != null)
            {
                ReservationList.CollectionChanged +=
                    new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);
            }

            if (LoanList != null)
            {
                LoanList.CollectionChanged +=
                    new NotifyCollectionChangedEventHandler(DataGrid_CollectionChanged);
            }

            else
            {
                MessageBox.Show("server seems to be unavailable");
            }

        }

        private ObservableCollection<GadgetViewModel> GetViewGadgets(List<Gadget> data)
        {
            ObservableCollection<GadgetViewModel> obsGadgets = new ObservableCollection<GadgetViewModel>();
            if (data == null)
                return null;
            data.ForEach(x => obsGadgets.Add(new GadgetViewModel(service, x)));
            return obsGadgets;
        }

        
        private ObservableCollection<CustomerViewModel> GetViewCustomers(List<Customer> data)
        {
            ObservableCollection<CustomerViewModel> obsCustomers = new ObservableCollection<CustomerViewModel>();
            if (data == null)
                return null;
            data.ForEach(x => obsCustomers.Add(new CustomerViewModel(service, x)));
            return obsCustomers;
        }

        private ObservableCollection<ReservationViewModel> GetViewReservations(List<Reservation> data)
        {
            ObservableCollection<ReservationViewModel> obsGadgets = new ObservableCollection<ReservationViewModel>();
            if (data == null)
                return null;
            data.ForEach(x => obsGadgets.Add(new ReservationViewModel(service, x)));
            return obsGadgets;
        }

        private ObservableCollection<LoanViewModel> GetViewLoans(List<Loan> data)
        {
            ObservableCollection<LoanViewModel> obsGadgets = new ObservableCollection<LoanViewModel>();
            if (data == null)
                return null;
            data.ForEach(x => obsGadgets.Add(new LoanViewModel(service, x)));
            return obsGadgets;
        }

        void DataGrid_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool healthy = false;
            if (sender.GetType() == typeof(ObservableCollection<GadgetViewModel>))
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        foreach (GadgetViewModel gadgetToRemove in e.OldItems)
                        {
                            healthy = gadgetToRemove.Remove();
                        }
                        break;
                    case NotifyCollectionChangedAction.Add:
                        foreach (Gadget newGadget in e.NewItems)
                        {
                            healthy = newGadget.Manufacturer != null
                                && newGadget.Name != null
                                && service.AddGadget(newGadget);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            else if (sender.GetType() == typeof(ObservableCollection<CustomerViewModel>))
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        foreach (CustomerViewModel customerToRemove in e.OldItems)
                        {
                            healthy = customerToRemove.Remove();
                        }
                        break;
                    case NotifyCollectionChangedAction.Add:
                        foreach (Customer newCustomer in e.NewItems)
                        {
                            healthy = newCustomer.Name != null
                                && newCustomer.Email != null
                                && newCustomer.Password != null
                                && newCustomer.Studentnumber != null
                                && service.AddCustomer(newCustomer);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            else if (sender.GetType() == typeof(ObservableCollection<ReservationViewModel>))
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        foreach (ReservationViewModel reservationToRemove in e.OldItems)
                        {
                            healthy = reservationToRemove.Remove();
                        }
                        break;
                    case NotifyCollectionChangedAction.Add:
                        foreach (Reservation newReservation in e.NewItems)
                        {
                            healthy = ((newReservation.CustomerId != null || newReservation.Customer != null)
                                        && (newReservation.Gadget != null || newReservation.GadgetId != null)
                                        && newReservation.Id != null && newReservation.WaitingPosition != -1
                                        && service.AddReservation(newReservation)
                                );
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            else if (sender.GetType() == typeof(ObservableCollection<LoanViewModel>))
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        foreach (LoanViewModel loanToRemove in e.OldItems)
                        {
                            healthy = loanToRemove.Remove();
                        }
                        break;
                    case NotifyCollectionChangedAction.Add:
                        foreach (Loan newLoan in e.NewItems)
                        {
                            healthy = ((newLoan.Customer != null || newLoan.CustomerId != null)
                                && (newLoan.Gadget != null || newLoan.GadgetId!= null)
                                && newLoan.Id != null && newLoan.ReturnDate != null && service.AddLoan(newLoan));
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (!healthy)
            {
                // TODO prevent grid change
            }
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
                ((LoanViewModel)lendingDataGridView.SelectedItem).Remove();
            }
        }

        private void deleteReservationButton_Click(object sender, RoutedEventArgs e)
        {
            if(reservationDataGridView.SelectedItem != null)
            {
                ((ReservationViewModel)reservationDataGridView.SelectedItem).Remove();
            }
        }

        private void addReservationButton_Click(object sender, RoutedEventArgs e)
        {
            if(customerDataGrid.SelectedItem != null)
            {
                Reservation res = new Reservation();
                res.Id = getNewReservationId();
                res.CustomerId = customerId;
                res.Finished = false;
                res.GadgetId = ((Gadget)newReservationComboBox.SelectedItem).InventoryNumber;
                res.ReservationDate = DateTime.Now;
                res.WaitingPosition = getWaitingPosition(res.Gadget);
                service.AddReservation(res);
            }
        }

        private void addLendingButton_Click(object sender, RoutedEventArgs e)
        {
            if (customerDataGrid.SelectedItem != null)
            {
                Loan loan = new Loan();
                loan.Id = getNewLoanId();
                loan.CustomerId = customerId;
                loan.Gadget = (Gadget)newReservationComboBox.SelectedItem;
                loan.GadgetId = ((Gadget)newReservationComboBox.SelectedItem).InventoryNumber;
                loan.PickupDate = DateTime.Now;
                service.AddLoan(loan);
            }
        }

        private void customerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
            {
                customerId = ((CustomerViewModel)item).Studentnumber;
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
        private string getNewReservationId()
        {
            int id = 0;
            int testId = 0;
            foreach (Reservation res in service.GetAllReservations())
            {
                Int32.TryParse(res.Id, out id);
                if (id > testId)
                    testId = id;
            }

            return (testId + 1).ToString();
        }

        private string getNewLoanId()
        {
            int id = 0;
            int testId = 0;
            foreach (Loan loan in service.GetAllLoans())
            {
                Int32.TryParse(loan.Id, out id);
                if (id > testId)
                    testId = id;
            }

            return (testId + 1).ToString();
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
