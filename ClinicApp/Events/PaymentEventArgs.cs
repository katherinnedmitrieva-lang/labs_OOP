using System;

namespace ClinicApp.Events;

// Дані про оплату: за який запис і яка сума. Для грошей — decimal, а не double (без помилок округлення).
public class PaymentEventArgs : EventArgs
{
    public int AppointmentId { get; }
    public decimal Amount { get; }

    public PaymentEventArgs(int appointmentId, decimal amount)
    {
        AppointmentId = appointmentId;
        Amount = amount;
    }
}
