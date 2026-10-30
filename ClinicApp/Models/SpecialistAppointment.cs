using System;

namespace ClinicApp.Models;

/// <summary>
/// Консультація спеціаліста: на 30% дорожча за базову ціну, має тему консультації.
/// sealed — клас є "листком" ієрархії, від нього не можна успадкуватись.
/// </summary>
public sealed class SpecialistAppointment : Appointment
{
    // Лише для читання: значення задається в конструкторі й далі не змінюється.
    public string ConsultationTopic { get; }

    public SpecialistAppointment(int patientId, int doctorId, DateTime scheduledAt, string topic = "", int durationMinutes = 45)
        : base(patientId, doctorId, scheduledAt, durationMinutes)
    {
        ConsultationTopic = topic ?? "";
    }

    public override decimal GetCost()
    {
        return base.GetCost() * 1.3m;
    }

    public override string GetDescription()
    {
        if (string.IsNullOrWhiteSpace(ConsultationTopic)) return "Консультація спеціаліста";
        return $"Консультація спеціаліста: {ConsultationTopic}";
    }
}
