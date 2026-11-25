using System;

namespace ClinicApp.Events;

// Дані про подію з пацієнтом (реєстрація). Лише для читання, заповнюються в конструкторі.
public class PatientEventArgs : EventArgs
{
    public int PatientId { get; }
    public string FullName { get; }

    public PatientEventArgs(int patientId, string fullName)
    {
        PatientId = patientId;
        FullName = fullName ?? "";
    }
}
