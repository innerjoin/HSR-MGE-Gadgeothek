using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ch.hsr.wpf.gadgeothek.service;
using System.Configuration;
using System.Windows;
using ch.hsr.wpf.gadgeothek.domain;
using Gadgeothek_Admin_App.ViewModels;
using MahApps.Metro.Controls;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Gadgeothek_Admin_App.Helpers;

namespace Gadgeothek_Admin_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public IEnumerable<CustomerViewModel> CustomerList { get; set; }
        public ObservableCollection<GadgetViewModel> GadgetList { get; set; }
        public ObservableCollection<ReservationViewModel> ReservationList { get; set; }
        public ObservableCollection<LoanViewModel> LoanList { get; set; }
        readonly LibraryAdminService _service;
        private string _customerId;

        public MainWindow()
        {
            InitializeComponent();
            _service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);

            EnableDisableButtons(false);

            GadgetList = GetGadgets();
            CustomerList = GetCustomers();
            ReservationList = GetReservations();
            LoanList = GetLoans();

            DataContext = this;

            GadgetsDataGridView.ItemsSource = GadgetList;
        }

        private ObservableCollection<LoanViewModel> GetLoans()
        {
            ObservableCollection<LoanViewModel> loans = ViewModelCollectionFactory.GetObservableCollection<LoanViewModel, Loan>(_service, _service.GetAllLoans());
            if(loans != null)
                loans.CollectionChanged += DataGrid_CollectionChanged;
            return loans;
        }

        private ObservableCollection<ReservationViewModel> GetReservations()
        {
            ObservableCollection<ReservationViewModel> res = ViewModelCollectionFactory.GetObservableCollection<ReservationViewModel, Reservation>(_service, _service.GetAllReservations());
            if(res != null)
                res.CollectionChanged += DataGrid_CollectionChanged;
            return res;
        }

        private IEnumerable<CustomerViewModel> GetCustomers()
        {
            IEnumerable<CustomerViewModel> customers = ViewModelCollectionFactory.GetObservableCollection<CustomerViewModel, Customer>(_service, _service.GetAllCustomers());
            return customers;
        }

        private ObservableCollection<GadgetViewModel> GetGadgets()
        {
            ObservableCollection<GadgetViewModel> gadgets = ViewModelCollectionFactory.GetObservableCollection<GadgetViewModel, Gadget>(_service, _service.GetAllGadgets());
            if(gadgets != null)
                gadgets.CollectionChanged += DataGrid_CollectionChanged;
            return gadgets;
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
                            if (_service.GetAllLoans().Where(items => { return items.GadgetId == gadgetToRemove.InventoryNumber && items.ReturnDate == null; }).ToList().Count == 0)
                            {
                                if (MessageBox.Show("Are you sure you want to delete '" + gadgetToRemove.Name + " '?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {

                                    var resList = ReservationList.Where(reservation => { return reservation.GadgetId == gadgetToRemove.InventoryNumber; });
                                    if (resList != null && MessageBox.Show("There are open Reservation for gadget '" + gadgetToRemove.Name + "'. Do you want to delete them too? ", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                    {
                                        foreach (ReservationViewModel model in resList)
                                        {
                                            model.Remove();
                                        }


                                        foreach (LoanViewModel model in LoanList.Where(loan => { return loan.GadgetId == gadgetToRemove.InventoryNumber && loan.ReturnDate != null; }))
                                        {
                                            model.Remove();
                                        }

                                        healthy = gadgetToRemove.Remove();

                                        ReservationList = GetReservations();
                                        FilterReservationList();

                                        LoanList = GetLoans();
                                        FilterLoanList();

                                        newReservationComboBox.ItemsSource = GetAvailableGadgetsForCustomer(_customerId);
                                        newLendingComboBox.ItemsSource = GetAvailableGadgetsForLoan(_customerId);
                                    }
                                }

                                if (!healthy) break;
                            }

                            else
                            {
                                MessageBox.Show("It isn't possible to delete Gadgets with open Loans!");
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Add:
                        foreach (GadgetViewModel newGadget in e.NewItems)
                        {
                            healthy = newGadget.Manufacturer != string.Empty 
                                && newGadget.Name != string.Empty
                                && _service.AddGadget(newGadget.GetGadget());
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
                if (!healthy)
                    GadgetsDataGridView.ItemsSource = GetGadgets();
                
            }

            else if (sender.GetType() == typeof(ObservableCollection<ReservationViewModel>))
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        foreach (ReservationViewModel reservationToRemove in e.OldItems)
                        {
                            if (MessageBox.Show("Are you sure you want to remove the reservation '" + reservationToRemove.Gadget.Name + "' assigned to '" + reservationToRemove.Customer.Name + "'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                int waitingId = reservationToRemove.WaitingPosition;
                                foreach (ReservationViewModel item in ReservationList)
                                {
                                    if (item.WaitingPosition > waitingId)
                                    {
                                        item.WaitingPosition -= 1;
                                    }
                                }

                                healthy = reservationToRemove.Remove();
                            }
                            if (!healthy) break;
                        }
                        break;
                    case NotifyCollectionChangedAction.Add:
                        foreach (ReservationViewModel newReservation in e.NewItems)
                        {
                            newReservation.Customer = _service.GetCustomer(_customerId);
                            healthy = ((newReservation.CustomerId != string.Empty || newReservation.Customer != null)
                                        && (newReservation.Gadget != null || newReservation.GadgetId != string.Empty)
                                        && newReservation.Id != string.Empty && newReservation.WaitingPosition != -1
                                        && _service.AddReservation(newReservation.GetReservation())
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

                if (!healthy)
                {
                    ReservationList = GetReservations();
                    FilterReservationList();
                }
                    
            }

            else if (sender.GetType() == typeof(ObservableCollection<LoanViewModel>))
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        foreach (LoanViewModel loanToRemove in e.OldItems)
                        {
                            if (MessageBox.Show("Are you sure you want to remove the lent '" + loanToRemove.Gadget.Name + "' by '" + loanToRemove.Customer.Name + "'?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                healthy = loanToRemove.Remove();
                            }
                            if (!healthy) break;
                        }
                        break;
                    case NotifyCollectionChangedAction.Add:
                        foreach (LoanViewModel newLoan in e.NewItems)
                        {
                            newLoan.Customer = _service.GetCustomer(_customerId);
                            healthy = ((newLoan.Customer != null || newLoan.CustomerId != string.Empty)
                                && (newLoan.Gadget != null || newLoan.GadgetId!= string.Empty)
                                && newLoan.Id != string.Empty && _service.AddLoan(newLoan.GetLoan()));
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
                if (!healthy)
                {
                    LoanList = GetLoans();
                    FilterLoanList();
                }
                    
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
                ICollectionView itemlist = itemSourceList.View;
                var costumFilter = new Predicate<object>(FilterGadgets);

                itemlist.Filter = costumFilter;

                GadgetsDataGridView.ItemsSource = itemlist;
            }           
        }

        private bool FilterGadgets(object item)
        {
            const StringComparison c = StringComparison.InvariantCultureIgnoreCase;
            return ((GadgetViewModel)item).Name.Contains(searchTextBox.Text, c) 
                || ((GadgetViewModel)item).Manufacturer.Contains(searchTextBox.Text, c) 
                || ((GadgetViewModel)item).Price.ToString(CultureInfo.InvariantCulture).Contains(searchTextBox.Text, c) 
                || ((GadgetViewModel)item).Condition.ToString().Contains(searchTextBox.Text, c)
                || ((GadgetViewModel)item).InventoryNumber.Contains(searchTextBox.Text, c);
        }

        private void addGadgetButton_Click(object sender, RoutedEventArgs e)
        {
            AddGadgetWindow addWindow = new AddGadgetWindow(this);
            addWindow.Show();
        }

        private void addReservationButton_Click(object sender, RoutedEventArgs e)
        {
            if (customerDataGrid.SelectedItem == null)
                return;
            GadgetViewModel gadget = (GadgetViewModel)newReservationComboBox.SelectedItem;
            if (gadget == null)
            {
                MessageBox.Show("You must select a gadget!");
                return;
            }
            Reservation res = new Reservation
            {
                Id = GetNewReservationId(),
                CustomerId = _customerId,
                Finished = false,
                GadgetId = gadget.InventoryNumber,
                Gadget = gadget.GetGadget(),
                ReservationDate = DateTime.Now
            };
            res.WaitingPosition = GetWaitingPosition(res.Gadget);
            ReservationViewModel model = new ReservationViewModel(_service, res);
            ReservationList.Add(model);
            newReservationComboBox.SelectedIndex = -1;
            newReservationComboBox.ItemsSource = GetAvailableGadgetsForCustomer(_customerId);
        }

        private void addLendingButton_Click(object sender, RoutedEventArgs e)
        {
            if (customerDataGrid.SelectedItem == null)
                return;
            GadgetViewModel gadget = (GadgetViewModel) newLendingComboBox.SelectedItem;
            if (gadget == null)
            {
                MessageBox.Show("You must select a gadget!");
                return;
            }
            Loan loan = new Loan
            {
                Id = GetNewLoanId(),
                CustomerId = _customerId,
                Gadget = gadget.GetGadget(),
                GadgetId = gadget.InventoryNumber,
                PickupDate = DateTime.Now
            };
            LoanViewModel model = new LoanViewModel(_service, loan);
            LoanList.Add(model);
            newLendingComboBox.SelectedIndex = -1;
            newLendingComboBox.ItemsSource = GetAvailableGadgetsForLoan(_customerId);
        }

        private bool FilterReservation(object item)
        {
            return ((ReservationViewModel)item).CustomerId == _customerId;
        }

        private bool FilterLoan(object item)
        {
            return ((LoanViewModel)item).CustomerId == _customerId;
        }

        private void customerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
            {
                _customerId = ((CustomerViewModel)item).Studentnumber;
            }

            FilterReservationList();
            FilterLoanList();


            newReservationComboBox.ItemsSource = GetAvailableGadgetsForCustomer(_customerId);
            newLendingComboBox.ItemsSource = GetAvailableGadgetsForLoan(_customerId);
            EnableDisableButtons(true);
        }

        private void FilterReservationList()
        {
            var resSourceList = new CollectionViewSource() { Source = ReservationList };
            ICollectionView resList = resSourceList.View;
            var costumFilter = new Predicate<object>(FilterReservation);
            resList.Filter = costumFilter;
            reservationDataGridView.ItemsSource = resList;
        }

        private void FilterLoanList()
        {
            var loanSourceList = new CollectionViewSource() { Source = LoanList };
            ICollectionView loanList = loanSourceList.View;
            var loanFilter = new Predicate<object>(FilterLoan);
            loanList.Filter = loanFilter;
            lendingDataGridView.ItemsSource = loanList;
        }

        private ObservableCollection<GadgetViewModel> GetGadgetsForCustomer(string customerId)
        {
            List<Gadget> liste = (from item in _service.GetAllReservations()
                                  where item.CustomerId == customerId
                                  select item.Gadget ?? _service.GetAllGadgets()
                                      .Find(gadget => gadget.InventoryNumber == item.GadgetId)).ToList();
            return new ObservableCollection<GadgetViewModel>(
                ViewModelCollectionFactory.GetObservableCollection<GadgetViewModel, Gadget>(_service, liste));
        }

        private ObservableCollection<GadgetViewModel> GetLentGadgets()
        {
            List<Gadget> liste = _service.GetAllLoans()
                .Select(item => item.Gadget ?? _service.GetAllGadgets()
                    .Find(gadget => gadget.InventoryNumber == item.GadgetId)).ToList();

            return new ObservableCollection<GadgetViewModel>(
                ViewModelCollectionFactory.GetObservableCollection<GadgetViewModel, Gadget>(_service, liste));
        }

        private ObservableCollection<GadgetViewModel> GetAvailableGadgetsForCustomer(string customerId)
        {
            ObservableCollection<GadgetViewModel> availableGadgets = new ObservableCollection<GadgetViewModel>(
                ViewModelCollectionFactory.GetObservableCollection<GadgetViewModel, Gadget>(_service, _service.GetAllGadgets()));
            foreach (GadgetViewModel item in GetGadgetsForCustomer(customerId))
            {
                availableGadgets.Remove(availableGadgets.FirstOrDefault(gadget => gadget != null && gadget.InventoryNumber == item.InventoryNumber));
            }

            return availableGadgets;
        }

        private ObservableCollection<GadgetViewModel> GetAvailableGadgetsForLoan(string customerId)
        {
            ObservableCollection<GadgetViewModel> availableGadgets = new ObservableCollection<GadgetViewModel>(
                ViewModelCollectionFactory.GetObservableCollection<GadgetViewModel, Gadget>(_service, _service.GetAllGadgets()));
            foreach (GadgetViewModel item in GetLentGadgets())
            {
                availableGadgets.Remove(availableGadgets.First(gadget => gadget.InventoryNumber == item.InventoryNumber));
            }

            return availableGadgets;
        }

        private string GetNewLoanId()
        {
            int testId = 0;
            foreach (Loan loan in _service.GetAllLoans())
            {
                int id;
                int.TryParse(loan.Id, out id);
                if (id > testId)
                    testId = id;
            }

            return (testId + 1).ToString();
        }

        private string GetNewReservationId()
        {
            int testId = 0;
            foreach (Reservation res in _service.GetAllReservations())
            {
                int id;
                int.TryParse(res.Id, out id);
                if (id > testId)
                    testId = id;
            }

            return (testId + 1).ToString();
        }

        private int GetWaitingPosition(Gadget item)
        {
            int waitingPosition = -1;
            foreach (Reservation res in _service.GetAllReservations())
            {
                if (((res.Gadget != null && res.Gadget.InventoryNumber == item.InventoryNumber) 
                        || (res.GadgetId == item.InventoryNumber)) 
                        && waitingPosition < res.WaitingPosition) {
                    waitingPosition = res.WaitingPosition;
                }
            }

            return waitingPosition + 1; 
        }

        private void EnableDisableButtons(bool input)
        {
            addLendingButton.IsEnabled = input;
            addReservationButton.IsEnabled = input;
            newReservationComboBox.IsEnabled = input;
            newLendingComboBox.IsEnabled = input;
        }
    }
}
