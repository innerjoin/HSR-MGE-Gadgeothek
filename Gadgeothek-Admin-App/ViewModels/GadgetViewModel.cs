using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;
using PropertyChanged;

namespace Gadgeothek_Admin_App.ViewModels
{
    [ImplementPropertyChanged]
    public class GadgetViewModel : IInstanceableViewModel
    {
        private readonly Gadget _gadget;
        private readonly LibraryAdminService _service;

        public GadgetViewModel(LibraryAdminService service, Gadget gadget)
        {
            _service = service;
            _gadget = gadget;
        }

        public Gadget GetGadget()
        {
            return _gadget;
        }

        public string InventoryNumber => _gadget.InventoryNumber;

        public Condition Condition
        {
            get { return _gadget.Condition; }
            set
            {
                _gadget.Condition = value;
                _service.UpdateGadget(_gadget);
            }
        }

        public double Price
        {
            get { return _gadget.Price; }
            set
            {
                _gadget.Price = value;
                _service.UpdateGadget(_gadget);
            }
        }

        public string Manufacturer
        {
            get { return _gadget.Manufacturer; }
            set
            {
                _gadget.Manufacturer = value;
                _service.UpdateGadget(_gadget);
            }
        }

        public string Name
        {
            get { return _gadget.Name; }
            set
            {
                _gadget.Name = value;
                _service.UpdateGadget(_gadget);
            }
        }

        public override string ToString()
        {
            return FullDescription();
        }

        public string ShortDescription()
        {
            return $"{_gadget.Name} [{_gadget.InventoryNumber}]";
        }

        public string FullDescription()
        {
            return $"{_gadget.Name} [{_gadget.InventoryNumber}] by {_gadget.Manufacturer} - Condition: {_gadget.Condition.ToString().ToUpper()}";
        }

        public bool Remove()
        {
            return _service.DeleteGadget(_gadget);
        }
    }
}
