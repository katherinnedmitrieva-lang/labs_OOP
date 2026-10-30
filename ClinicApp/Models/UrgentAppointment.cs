using System;

namespace ClinicApp.Models;

/// <summary>
/// Терміновий прийом: на 50% дорожчий за базову ціну, має причину терміновості.
/// </summary>
public class UrgentAppointment : Appointment
{
    // Лише для читання: значення задається в конструкторі й далі не змінюється.
    public string UrgencyNote { get; }

    public UrgentAppointment(int patientId, int doctorId, DateTime scheduledAt, string urgencyNote = "", int durationMinutes = 30)
        : base(patientId, doctorId, scheduledAt, durationMinutes)
    {
        UrgencyNote = urgencyNote ?? "";
    }

    // base.GetCost() — ціна з Appointment. Формулу не дублюємо: якщо базова ставка зміниться,
    // ціна термінового прийому порахується правильно без жодних правок тут.
    public override decimal GetCost()
    {
        return base.GetCost() * 1.5m;
    }

    // sealed override: перевизначаємо тут, але подальші підкласи вже не зможуть змінити опис.
    public sealed override string GetDescription()
    {
        if (string.IsNullOrWhiteSpace(UrgencyNote)) return "Терміновий";
        return $"Терміновий ({UrgencyNote})";
    }

    // new (а не override): цей метод ПРИХОВУЄ GetPriority() батька, а не перевизначає його.
    // Appointment.GetPriority() не virtual, тому override тут неможливий. Який з двох методів
    // викличеться — залежить від типу ЗМІННОЇ, а не об'єкта (див. демонстрацію в Program.cs).
    public new int GetPriority()
    {
        return 1;
    }
}
