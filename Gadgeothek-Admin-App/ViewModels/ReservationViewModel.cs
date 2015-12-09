using ch.hsr.wpf.gadgeothek.domain;
using ch.hsr.wpf.gadgeothek.service;
using PropertyChanged;
using System;

namespace Gadgeothek_Admin_App.ViewModels
{
    [ImplementPropertyChanged]
    public class ReservationViewModel
    {
        private readonly Reservation _reservation;
        private readonly LibraryAdminService _service;

        public ReservationViewModel(LibraryAdminService service, Reservation reservation)
        {
            _service = service;
            _reservation = reservation;
        }

        public Reservation getReservation()
        {
            return _reservation;
        }

        public string Id => _reservation.Id;
        public string CustomerId
        {
            get { return _reservation.CustomerId; }
            set
            {
                _reservation.CustomerId = value;
                _service.UpdateReservation(_reservation);
            }
        }

        public Customer Customer
        {
            get { return _reservation.Customer; }
            set
            {
                _reservation.Customer = value;
                _service.UpdateReservation(_reservation);
            }
        }
        public string GadgetId
        {
            get { return _reservation.GadgetId; }
            set
            {
                _reservation.GadgetId = value;
                _service.UpdateReservation(_reservation);
            }
        }
        public Gadget Gadget
        {
            get { return _reservation.Gadget; }
            set
            {
                _reservation.Gadget = value;
                _service.UpdateReservation(_reservation);
            }
        }
        public DateTime? ReservationDate
        {
            get { return _reservation.ReservationDate; }
            set
            {
                _reservation.ReservationDate = value;
                _service.UpdateReservation(_reservation);
            }
        }
        public bool Finished
        {
            get { return _reservation.Finished; }
            set
            {
                _reservation.Finished= value;
                _service.UpdateReservation(_reservation);
            }
        }
        public int WaitingPosition
        {
            get { return _reservation.WaitingPosition; }
            set
            {
                _reservation.WaitingPosition = value;
                _service.UpdateReservation(_reservation);
            }
        }

        public bool IsReady
        {
            get { return _reservation.IsReady; }
            set
            {
                _reservation.IsReady = value;
                _service.UpdateReservation(_reservation);
            }
        }

        public bool Remove()
        {
            return _service.DeleteReservation(_reservation);
        }
    }
}
