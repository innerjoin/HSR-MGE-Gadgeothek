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
        LibraryAdminService _service;
        string customerId;
        public MainWindow()
        {
            InitializeComponent();
            _service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);

            enableDisableButtons(false);
            deleteLendingButton.IsEnabled = false;
            deleteReservationButton.IsEnabled = false;
            List<Gadget> gadgetData = _service.GetAllGadgets();
            List<Customer> customerData = _service.GetAllCustomers();
            List<Reservation> reservationData = _service.GetAllReservations();
            List<Loan> loanData = _service.GetAllLoans();

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
            data.ForEach(x => obsGadgets.Add(new GadgetViewModel(_service, x)));
            return obsGadgets;
        }

        private ObservableCollection<CustomerViewModel> GetViewCustomers(List<Customer> data)
        {
            ObservableCollection<CustomerViewModel> obsCustomers = new ObservableCollection<CustomerViewModel>();
            if (data == null)
                return null;
            data.ForEach(x => obsCustomers.Add(new CustomerViewModel(_service, x)));
            return obsCustomers;
        }

        private ObservableCollection<ReservationViewModel> GetViewReservations(List<Reservation> data)
        {
            ObservableCollection<ReservationViewModel> obsReservations = new ObservableCollection<ReservationViewModel>();
            if (data == null)
                return null;
            data.ForEach(x => obsReservations.Add(new ReservationViewModel(_service, x)));
            return obsReservations;
        }

        private ObservableCollection<LoanViewModel> GetViewLoans(List<Loan> data)
        {
            ObservableCollection<LoanViewModel> obsGadgets = new ObservableCollection<LoanViewModel>();
            if (data == null)
                return null;
            data.ForEach(x => obsGadgets.Add(new LoanViewModel(_service, x)));
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
                        foreach (GadgetViewModel newGadget in e.NewItems)
                        {
                            healthy = newGadget.Manufacturer != string.Empty 
                                && newGadget.Name != string.Empty
                                && _service.AddGadget(newGadget.getGadget());
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
                        foreach (CustomerViewModel newCustomer in e.NewItems)
                        {
                            healthy = newCustomer.Name != string.Empty
                                && newCustomer.Email != string.Empty
                                && newCustomer.Password != string.Empty
                                && newCustomer.Studentnumber != string.Empty
                                && _service.AddCustomer(newCustomer.getCustomer());
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
                        foreach (ReservationViewModel newReservation in e.NewItems)
                        {
                            healthy = ((newReservation.CustomerId != string.Empty || newReservation.Customer != null)
                                        && (newReservation.Gadget != null || newReservation.GadgetId != string.Empty)
                                        && newReservation.Id != string.Empty && newReservation.WaitingPosition != -1
                                        && _service.AddReservation(newReservation.getReservation())
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
                        foreach (LoanViewModel newLoan in e.NewItems)
                        {
                            healthy = ((newLoan.Customer != null || newLoan.CustomerId != string.Empty)
                                && (newLoan.Gadget != null || newLoan.GadgetId!= string.Empty)
                                && newLoan.Id != string.Empty && _service.AddLoan(newLoan.getLoan()));
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
            if(_service != null)
            {
                var itemSourceList = new CollectionViewSource() { Source = GadgetList };
                ICollectionView Itemlist = itemSourceList.View;
                var costumFilter = new Predicate<object>(filterGadgets);

                Itemlist.Filter = costumFilter;

                GadgetsDataGridView.ItemsSource = Itemlist;
            }           
        }

        private bool filterGadgets(object item)
        {
            return ((GadgetViewModel)item).Name.Contains(searchTextBox.Text) || ((GadgetViewModel)item).Manufacturer.Contains(searchTextBox.Text) || ((GadgetViewModel)item).Price.ToString().Contains(searchTextBox.Text) || ((GadgetViewModel)item).Condition.ToString().Contains(searchTextBox.Text) || ((GadgetViewModel)item).InventoryNumber.ToString().Contains(searchTextBox.Text);
        }

        private void addGadgetButton_Click(object sender, RoutedEventArgs e)
        {
            AddGadgetWindow addWindow = new AddGadgetWindow(this);
            addWindow.Show();
        }

        private void deleteLendingButton_Click(object sender, RoutedEventArgs e)
        {
            if (lendingDataGridView.SelectedItem != null)
            {
                ((LoanViewModel)lendingDataGridView.SelectedItem).Remove();
                LoanList.Remove(((LoanViewModel)lendingDataGridView.SelectedItem));
            }
        }

        private void deleteReservationButton_Click(object sender, RoutedEventArgs e)
        {
            if(reservationDataGridView.SelectedItem != null)
            {
                ((ReservationViewModel)reservationDataGridView.SelectedItem).Remove();
                ReservationList.Remove(((ReservationViewModel)reservationDataGridView.SelectedItem));
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
                res.GadgetId = ((GadgetViewModel)newReservationComboBox.SelectedItem).InventoryNumber;
                res.Gadget = ((GadgetViewModel)newReservationComboBox.SelectedItem).getGadget();
                res.ReservationDate = DateTime.Now;
                res.WaitingPosition = getWaitingPosition(res.Gadget);
                ReservationViewModel model = new ReservationViewModel(_service, res);
                ReservationList.Add(model);
                newReservationComboBox.SelectedIndex = -1;   
            }
        }

        private void addLendingButton_Click(object sender, RoutedEventArgs e)
        {
            if (customerDataGrid.SelectedItem != null)
            {
                Loan loan = new Loan();
                loan.Id = getNewLoanId();
                loan.CustomerId = customerId;
                loan.Gadget = ((GadgetViewModel)newLendingComboBox.SelectedItem).getGadget();
                loan.GadgetId = ((GadgetViewModel)newLendingComboBox.SelectedItem).InventoryNumber;
                loan.PickupDate = DateTime.Now;
                LoanViewModel model = new LoanViewModel(_service, loan);
                LoanList.Add(model);
                newLendingComboBox.SelectedIndex = -1;
            }
        }

        private bool filterReservation(object item)
        {
            return ((ReservationViewModel)item).CustomerId == customerId;
        }

        private bool filterLoan(object item)
        {
            return ((LoanViewModel)item).CustomerId == customerId;
        }

        private void customerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
            {
                customerId = ((CustomerViewModel)item).Studentnumber;
            }

            var resSourceList = new CollectionViewSource() { Source = ReservationList };
            ICollectionView resList = resSourceList.View;
            var costumFilter = new Predicate<object>(filterReservation);
            resList.Filter = costumFilter;
            reservationDataGridView.ItemsSource = resList;

            var loanSourceList = new CollectionViewSource() { Source = LoanList };
            ICollectionView loanList = loanSourceList.View;
            var loanFilter = new Predicate<object>(filterLoan);
            loanList.Filter = loanFilter;
            lendingDataGridView.ItemsSource = loanList;

            newReservationComboBox.ItemsSource = getAvailableGadgetsForCustomer(customerId);
            newLendingComboBox.ItemsSource = getAvailableGadgetsForLoan(customerId);
            enableDisableButtons(true);
        }

        private ObservableCollection<GadgetViewModel> getGadgetsForCustomer(string customerId)
        {
            List<Gadget> liste = new List<Gadget>();
            foreach (Reservation item in _service.GetAllReservations())
            {
                if(item.CustomerId == customerId)
                {
                    liste.Add(item.Gadget);
                }
            }

            return new ObservableCollection<GadgetViewModel>(GetViewGadgets(liste));
        }

        private ObservableCollection<GadgetViewModel> getLentGadgets()
        {
            List<Gadget> liste = new List<Gadget>();
            foreach (Loan item in _service.GetAllLoans())
            {
                liste.Add(item.Gadget);
            }

            return new ObservableCollection<GadgetViewModel>(GetViewGadgets(liste));
        }

        private ObservableCollection<ReservationViewModel> getReservationForCustomer(string customerId)
        {
            List<Reservation> liste = new List<Reservation>();
            foreach (Reservation item in _service.GetAllReservations())
            {
                if (item.CustomerId == customerId)
                {
                    liste.Add(item);
                }
            }
            return new ObservableCollection<ReservationViewModel>(GetViewReservations(liste));
        }

        private ObservableCollection<LoanViewModel> getLoansForCustomer(string customerId)
        {
            List<Loan> liste = new List<Loan>();
            foreach (Loan item in _service.GetAllLoans())
            {
                if (item.CustomerId == customerId)
                {
                    liste.Add(item);
                }
            }

            return new ObservableCollection<LoanViewModel>(GetViewLoans(liste));
        }

        private ObservableCollection<GadgetViewModel> getAvailableGadgetsForCustomer(string customerId)
        {
            ObservableCollection<GadgetViewModel> availableGadgets = new ObservableCollection<GadgetViewModel>(GetViewGadgets(_service.GetAllGadgets()));
            foreach (GadgetViewModel item in getGadgetsForCustomer(customerId))
            {
                availableGadgets.Remove(item);
            }

            return availableGadgets;
        }

        private ObservableCollection<GadgetViewModel> getAvailableGadgetsForLoan(string customerId)
        {
            ObservableCollection<GadgetViewModel> availableGadgets = new ObservableCollection<GadgetViewModel>(GetViewGadgets(_service.GetAllGadgets()));
            foreach (GadgetViewModel item in getLentGadgets())
            {
                availableGadgets.Remove(item);
            }

            return availableGadgets;
        }

        private string getNewLoanId()
        {
            int id = 0;
            int testId = 0;
            foreach (Loan loan in _service.GetAllLoans())
            {
                Int32.TryParse(loan.Id, out id);
                if (id > testId)
                    testId = id;
            }

            return (testId + 1).ToString();
        }

        private string getNewReservationId()
        {
            int id = 0;
            int testId = 0;
            foreach (Reservation res in _service.GetAllReservations())
            {
                Int32.TryParse(res.Id, out id);
                if (id > testId)
                    testId = id;
            }

            return (testId + 1).ToString();
        }

        private int getWaitingPosition(Gadget item)
        {
            int waitingPosition = -1;
            foreach (Reservation res in _service.GetAllReservations())
            {
                if ((res.Gadget != null && res.Gadget.InventoryNumber == item.InventoryNumber) || (res.GadgetId == item.InventoryNumber))
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
