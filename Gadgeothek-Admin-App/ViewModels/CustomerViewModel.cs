using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;
using PropertyChanged;

namespace Gadgeothek_Admin_App.ViewModels
{
    [ImplementPropertyChanged]
    public class CustomerViewModel
    {
        private readonly Customer _customer;
        private readonly LibraryAdminService _service;

        public CustomerViewModel(LibraryAdminService service, Customer customer)
        {
            _service = service;
            _customer = customer;
        }

        public Customer getCustomer()
        {
            return _customer;
        }

        public string Password
        {
            get { return _customer.Password; }
            set
            {
                _customer.Password = value;
                _service.UpdateCustomer(_customer);
            }
        }

        public string Email
        {
            get { return _customer.Email; }
            set
            {
                _customer .Email= value;
                _service.UpdateCustomer(_customer);
            }
        }

        public string Studentnumber
        {
            get { return _customer.Studentnumber; }
            set
            {
                _customer.Studentnumber = value;
                _service.UpdateCustomer(_customer);
            }
        }

        public string Name
        {
            get { return _customer.Name; }
            set
            {
                _customer.Name = value;
                _service.UpdateCustomer(_customer);
            }
        }

        public bool Remove()
        {
            return _service.DeleteCustomer(_customer);
        }
    }
}
