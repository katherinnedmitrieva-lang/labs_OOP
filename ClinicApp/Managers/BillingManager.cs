using System.Linq;
using ClinicApp.Interfaces;

namespace ClinicApp.Managers
{
    public class BillingManager
    {
        private readonly AppointmentManager _appointmentManager;

        public BillingManager(AppointmentManager appointmentManager)
        {
            _appointmentManager = appointmentManager;
        }

        public IPayable[] GetUnpaidPayables()
        {
            var allAppointments = _appointmentManager.GetAll();
            if (allAppointments == null) return new IPayable[0];

            return allAppointments
                .Where(p => p != null) // Захист від null-елементів у масиві
                .Cast<IPayable>()
                .Where(p => !p.IsPaid && p is ICancellable c && !c.IsCancelled)
                .ToArray();
        }
        public decimal GetTotalDebt()
        {
            return GetUnpaidPayables().Sum(p => p.Cost);
        }
    }
}