using System;

namespace ClinicApp.Models;

/// <summary>
/// Звичайний прийом: ціна — базова (з Appointment), змінюється лише опис.
/// </summary>
public class RegularAppointment : Appointment
{
    // Конструктор нічого не рахує сам: лише передає параметри батьківському конструктору через : base(...)
    public RegularAppointment(int patientId, int doctorId, DateTime scheduledAt, int durationMinutes = 30)
        : base(patientId, doctorId, scheduledAt, durationMinutes)
    {
    }

    public override string GetDescription()
    {
        return "Звичайний прийом";
    }
}
