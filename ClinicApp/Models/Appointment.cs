using System;
using ClinicApp.Enums;
using ClinicApp.Utils;
using ClinicApp.Interfaces;

namespace ClinicApp.Models
{
    public class Appointment : IPayable
    {
        private static int _nextId = 1;

        private int _durationMinutes;
        private bool _isPaid;

        public int Id { get; }
        public int PatientId { get; }
        public int DoctorId { get; }
        public DateTime ScheduledAt { get; set; }

        public int DurationMinutes
        {
            get => _durationMinutes;
            set
            {
                ClinicValidator.ValidatePositive(value, nameof(DurationMinutes));
                _durationMinutes = value;
            }
        }

        public AppointmentStatus Status { get; private set; }
        public string Notes { get; private set; }

        public DateTime EndsAt => ScheduledAt.AddMinutes(DurationMinutes);
        public bool IsUpcoming => ScheduledAt > DateTime.Now && Status == AppointmentStatus.Scheduled;

        // Реалізація IPayable (10 грн за хвилину тривалості)
        public decimal Cost => (decimal)DurationMinutes * 10m;
        public bool IsPaid => _isPaid;

        public void MarkPaid()
        {
            _isPaid = true;
        }

        public Appointment(int patientId, int doctorId, DateTime scheduledAt, int durationMinutes = 30)
        {
            PatientId = patientId;
            DoctorId = doctorId;
            ScheduledAt = scheduledAt;
            DurationMinutes = durationMinutes;
            Status = AppointmentStatus.Scheduled;
            Notes = "";
            Id = _nextId++;   // ОСТАННІМ рядком
        }

        public bool Cancel(string reason = "")
        {
            if (Status != AppointmentStatus.Scheduled) return false;
            Status = AppointmentStatus.Cancelled;
            if (reason.Length > 0) Notes = reason;
            return true;
        }

        public bool Complete()
        {
            if (Status != AppointmentStatus.Scheduled) return false;
            Status = AppointmentStatus.Completed;
            return true;
        }

        public override string ToString()
        {
            string line = $"[{Id}] Пацієнт #{PatientId} → Лікар #{DoctorId} | {ScheduledAt:dd.MM.yyyy HH:mm}–{EndsAt:HH:mm} | {Status}";
            if (Notes.Length > 0) line += $" | {Notes}";
            return line;
        }
    }
}