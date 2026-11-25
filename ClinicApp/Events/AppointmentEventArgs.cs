using System;

namespace ClinicApp.Events;

// «Посилка» з даними про запис на прийом. Її менеджер передає підписникам разом із подією.
// Усі властивості лише для читання (тільки get): значення задаються в конструкторі й далі не змінюються,
// тому жоден підписник не зможе випадково «зіпсувати» дані, які отримають наступні підписники.
public class AppointmentEventArgs : EventArgs
{
    public int AppointmentId { get; }
    public int PatientId { get; }
    public int DoctorId { get; }
    public DateTime ScheduledAt { get; }
    public string Notes { get; }

    public AppointmentEventArgs(int appointmentId, int patientId, int doctorId, DateTime scheduledAt, string notes = "")
    {
        AppointmentId = appointmentId;
        PatientId = patientId;
        DoctorId = doctorId;
        ScheduledAt = scheduledAt;
        Notes = notes ?? "";   // null перетворюємо на порожній рядок, щоб підписникам не треба було його перевіряти
    }
}
