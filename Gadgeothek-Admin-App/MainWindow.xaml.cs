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
using System.Globalization;
using System.Linq;

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
        readonly LibraryAdminService _service;
        private string _customerId;

        public MainWindow()
        {
            InitializeComponent();
            _service = new LibraryAdminService(ConfigurationManager.AppSettings["server"]);

            //GadgetsDataGridView.ItemsSource = _service.GetAllGadgets();
            EnableDisableButtons(false);
            deleteLendingButton.IsEnabled = false;
            deleteReservationButton.IsEnabled = false;

            GadgetList = ViewModelCollectionFactory.GetObservableCollection<GadgetViewModel, Gadget>(_service, _service.GetAllGadgets());
            CustomerList = ViewModelCollectionFactory.GetObservableCollection<CustomerViewModel, Customer>(_service, _service.GetAllCustomers());
            ReservationList =  ViewModelCollectionFactory.GetObservableCollection<ReservationViewModel, Reservation>(_service, _service.GetAllReservations());
            LoanList = ViewModelCollectionFactory.GetObservableCollection<LoanViewModel, Loan>(_service, _service.GetAllLoans());

            DataContext = this;

            if (GadgetList != null)
                GadgetList.CollectionChanged += DataGrid_CollectionChanged;
            if (CustomerList != null)
                CustomerList.CollectionChanged += DataGrid_CollectionChanged;
            if (ReservationList != null)
                ReservationList.CollectionChanged += DataGrid_CollectionChanged;
            if (LoanList != null)
                LoanList.CollectionChanged += DataGrid_CollectionChanged;

            GadgetsDataGridView.ItemsSource = GadgetList;
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
                                && _service.AddCustomer(newCustomer.GetCustomer());
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
                            int waitingId = reservationToRemove.WaitingPosition;
                            List<Reservation> reservationList = (from item in _service.GetAllReservations() where item.GadgetId == reservationToRemove.GadgetId select item).ToList();
                            foreach(ReservationViewModel item in ReservationList)
                            {
                                if(item.WaitingPosition > waitingId)
                                {
                                    item.WaitingPosition -= 1;
                                }
                            }

                            healthy = reservationToRemove.Remove();
                            //FilterReservationList();
                        }
                        break;
                    case NotifyCollectionChangedAction.Add:
                        foreach (ReservationViewModel newReservation in e.NewItems)
                        {
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
                ICollectionView itemlist = itemSourceList.View;
                var costumFilter = new Predicate<object>(FilterGadgets);

                itemlist.Filter = costumFilter;

                GadgetsDataGridView.ItemsSource = itemlist;
            }           
        }

        private bool FilterGadgets(object item)
        {
            return ((GadgetViewModel)item).Name.Contains(searchTextBox.Text) 
                || ((GadgetViewModel)item).Manufacturer.Contains(searchTextBox.Text) 
                || ((GadgetViewModel)item).Price.ToString(CultureInfo.InvariantCulture).Contains(searchTextBox.Text) 
                || ((GadgetViewModel)item).Condition.ToString().Contains(searchTextBox.Text) 
                || ((GadgetViewModel)item).InventoryNumber.Contains(searchTextBox.Text);
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
                Reservation res = new Reservation
                {
                    Id = GetNewReservationId(),
                    CustomerId = _customerId,
                    Finished = false,
                    GadgetId = ((GadgetViewModel) newReservationComboBox.SelectedItem).InventoryNumber,
                    Gadget = ((GadgetViewModel) newReservationComboBox.SelectedItem).GetGadget(),
                    ReservationDate = DateTime.Now
                };
                res.WaitingPosition = GetWaitingPosition(res.Gadget);
                ReservationViewModel model = new ReservationViewModel(_service, res);
                ReservationList.Add(model);
                newReservationComboBox.SelectedIndex = -1;
                newReservationComboBox.ItemsSource = GetAvailableGadgetsForCustomer(_customerId);
            }
        }

        private void addLendingButton_Click(object sender, RoutedEventArgs e)
        {
            if (customerDataGrid.SelectedItem != null)
            {
                Loan loan = new Loan
                {
                    Id = GetNewLoanId(),
                    CustomerId = _customerId,
                    Gadget = ((GadgetViewModel) newLendingComboBox.SelectedItem).GetGadget(),
                    GadgetId = ((GadgetViewModel) newLendingComboBox.SelectedItem).InventoryNumber,
                    PickupDate = DateTime.Now
                };
                LoanViewModel model = new LoanViewModel(_service, loan);
                LoanList.Add(model);
                newLendingComboBox.SelectedIndex = -1;
                newLendingComboBox.ItemsSource = GetAvailableGadgetsForLoan(_customerId);
            }
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
            List<Gadget> liste = (from item in _service.GetAllReservations() where item.CustomerId == customerId select item.Gadget == null ? _service.GetAllGadgets().Find(gadget => { return gadget.InventoryNumber == item.GadgetId; }) : item.Gadget).ToList();
            return new ObservableCollection<GadgetViewModel>(ViewModelCollectionFactory.GetObservableCollection<GadgetViewModel, Gadget>(_service, liste));
        }

        private ObservableCollection<GadgetViewModel> getLentGadgets()
        {
            List<Gadget> liste = _service.GetAllLoans().Select(item => item.Gadget == null ? _service.GetAllGadgets().Find(gadget => { return gadget.InventoryNumber == item.GadgetId; }) : item.Gadget).ToList();

            return new ObservableCollection<GadgetViewModel>(ViewModelCollectionFactory.GetObservableCollection<GadgetViewModel, Gadget>(_service, liste));
        }

        private ObservableCollection<ReservationViewModel> GetReservationForCustomer(string customerId)
        {
            List<Reservation> liste = _service.GetAllReservations()
                .Where(item => item.CustomerId == customerId).ToList();
            return new ObservableCollection<ReservationViewModel>(ViewModelCollectionFactory.GetObservableCollection<ReservationViewModel, Reservation>(_service, liste));
        }

        private ObservableCollection<LoanViewModel> GetLoansForCustomer(string customerId)
        {
            List<Loan> liste = _service.GetAllLoans()
                .Where(item => item.CustomerId == customerId).ToList();

            return new ObservableCollection<LoanViewModel>(ViewModelCollectionFactory.GetObservableCollection<LoanViewModel, Loan>(_service, liste));
        }

        private ObservableCollection<GadgetViewModel> GetAvailableGadgetsForCustomer(string customerId)
        {
            ObservableCollection<GadgetViewModel> availableGadgets = new ObservableCollection<GadgetViewModel>(ViewModelCollectionFactory.GetObservableCollection<GadgetViewModel, Gadget>(_service, _service.GetAllGadgets()));
            foreach (GadgetViewModel item in GetGadgetsForCustomer(customerId))
            {
                availableGadgets.Remove(availableGadgets.First<GadgetViewModel>(gadget => gadget.InventoryNumber == item.InventoryNumber));
            }

            return availableGadgets;
        }

        private ObservableCollection<GadgetViewModel> GetAvailableGadgetsForLoan(string customerId)
        {
            ObservableCollection<GadgetViewModel> availableGadgets = new ObservableCollection<GadgetViewModel>(ViewModelCollectionFactory.GetObservableCollection<GadgetViewModel, Gadget>(_service, _service.GetAllGadgets()));
            foreach (GadgetViewModel item in getLentGadgets())
            {
                availableGadgets.Remove(availableGadgets.First<GadgetViewModel>(gadget => gadget.InventoryNumber == item.InventoryNumber));
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
            int id = 0;
            int testId = 0;
            foreach (Reservation res in _service.GetAllReservations())
            {
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
