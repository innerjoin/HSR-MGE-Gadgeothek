using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;
using PropertyChanged;
using System;

namespace Gadgeothek_Admin_App.ViewModels
{
    [ImplementPropertyChanged]
    public class LoanViewModel
    {
        private readonly Loan _loan;
        private readonly LibraryAdminService _service;

        public LoanViewModel(LibraryAdminService service, Loan loan)
        {
            _service = service;
            _loan = loan;
        }

        public string Id => _loan.Id;
        public string CustomerId
        {
            get { return _loan.CustomerId; }
            set
            {
                _loan.CustomerId = value;
                _service.UpdateLoan(_loan);
            }
        }

        public Customer Customer
        {
            get { return _loan.Customer; }
            set
            {
                _loan.Customer = value;
                _service.UpdateLoan(_loan);
            }
        }
        public string GadgetId
        {
            get { return _loan.GadgetId; }
            set
            {
                _loan.GadgetId = value;
                _service.UpdateLoan(_loan);
            }
        }
        public Gadget Gadget
        {
            get { return _loan.Gadget; }
            set
            {
                _loan.Gadget = value;
                _service.UpdateLoan(_loan);
            }
        }
        public DateTime? PickupDate
        {
            get { return _loan.PickupDate; }
            set
            {
                _loan.PickupDate = value;
                _service.UpdateLoan(_loan);
            }
        }
        public DateTime? ReturnDate
        {
            get { return _loan.ReturnDate; }
            set
            {
                _loan.ReturnDate = value;
                _service.UpdateLoan(_loan);
            }
        }

        public bool Remove()
        {
            return _service.DeleteLoan(_loan);
        }
    }
}
