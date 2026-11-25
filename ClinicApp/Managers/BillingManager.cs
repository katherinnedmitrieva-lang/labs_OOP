using System;
using System.Linq;
using ClinicApp.Events;
using ClinicApp.Interfaces;
using ClinicApp.Models;

namespace ClinicApp.Managers
{
    public class BillingManager
    {
        private readonly AppointmentManager _appointmentManager;

        // Подія «оплату отримано». Піднімається після кожної успішної оплати запису.
        public event EventHandler<PaymentEventArgs>? PaymentReceived;

        public BillingManager(AppointmentManager appointmentManager)
        {
            _appointmentManager = appointmentManager;
        }

        // Оплачує запис. Повертає true, лише якщо оплату виконано; запис не знайдено, скасований або вже оплачений — false.
        // Сума в події — GetCost(): віртуальний метод, тому для термінового запису й консультації вона своя.
        public bool PayAppointment(int appointmentId)
        {
            Appointment? appointment = _appointmentManager.FindById(appointmentId);
            if (appointment == null || appointment.IsCancelled || appointment.IsPaid) return false;

            appointment.MarkPaid();
            PaymentReceived?.Invoke(this, new PaymentEventArgs(appointment.Id, appointment.GetCost()));
            return true;
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