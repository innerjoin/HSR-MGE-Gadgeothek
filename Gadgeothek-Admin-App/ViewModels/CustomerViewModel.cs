using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;
using PropertyChanged;

namespace Gadgeothek_Admin_App.ViewModels
{
    [ImplementPropertyChanged]
    public class CustomerViewModel : IInstanceableViewModel
    {
        private readonly Customer _loan;
        private readonly LibraryAdminService _service;

        public CustomerViewModel(LibraryAdminService service, Customer customer)
        {
            _service = service;
            _loan = customer;
        }

        public string Password
        {
            get { return _loan.Password; }
            set
            {
                _loan.Password = value;
                _service.UpdateCustomer(_loan);
            }
        }

        public string Email
        {
            get { return _loan.Email; }
            set
            {
                _loan .Email= value;
                _service.UpdateCustomer(_loan);
            }
        }

        public string Studentnumber
        {
            get { return _loan.Studentnumber; }
            set
            {
                _loan.Studentnumber = value;
                _service.UpdateCustomer(_loan);
            }
        }

        public string Name
        {
            get { return _loan.Name; }
            set
            {
                _loan.Name = value;
                _service.UpdateCustomer(_loan);
            }
        }

        public bool Remove()
        {
            return _service.DeleteCustomer(_loan);
        }
    }
}
