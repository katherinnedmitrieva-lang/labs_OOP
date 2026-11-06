using System;
using ClinicApp.Enums;
using ClinicApp.Utils;
using ClinicApp.Interfaces;

namespace ClinicApp.Models
{
    public class Appointment : IPayable, ICancellable, IIdentifiable
    {
        private static int _nextId = 1;

        private int _durationMinutes;
        private bool _isPaid;
        private string _cancellationReason = "";
        private decimal _discountMultiplier = 1m;

        public bool IsCancelled => Status == AppointmentStatus.Cancelled;
        public string CancellationReason => _cancellationReason;
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

        // Реалізація IPayable. Cost не має власної формули — він бере значення з віртуального
        // GetCost(), тому для будь-якого підкласу (терміновий, консультація...) повертає його ціну.
        public decimal Cost => GetCost();
        public bool IsPaid => _isPaid;

        // ВАРІАНТ А (VIP-знижка): обрано множник знижки в базовому класі Appointment, а не нові підкласи
        // (VipUrgentAppointment тощо). Причини:
        //  - GetDescription() у UrgentAppointment — sealed override, а SpecialistAppointment — sealed class,
        //    тож VIP-підкласи від них компілятор не дозволяє (помилки CS0239 і CS0509);
        //  - VIP-підкласи множать кількість класів: 3 типи -> 6 класів, з дитячим і пенсійним тарифами -> 12 і більше;
        //  - підкласи не змінюються: їхній base.GetCost() уже містить знижку (відкритий для розширення,
        //    закритий для змін — принцип Open/Closed).
        // 1 = без знижки; 0.8 = знижка 20%. Допустимі значення: більше 0 і не більше 1.
        public decimal DiscountMultiplier
        {
            get => _discountMultiplier;
            set
            {
                if (value <= 0m || value > 1m)
                    throw new ArgumentOutOfRangeException(nameof(DiscountMultiplier), "Множник знижки має бути більшим за 0 і не більшим за 1.");
                _discountMultiplier = value;
            }
        }

        // virtual — дозвіл підкласам змінити поведінку методу через override.
        // Базова ціна: 10 грн за хвилину тривалості з урахуванням знижкового множника.
        public virtual decimal GetCost()
        {
            return (decimal)DurationMinutes * 10m * DiscountMultiplier;
        }

        // virtual — підкласи повертають власний опис типу прийому.
        public virtual string GetDescription()
        {
            return "Прийом";
        }

        // Навмисно НЕ virtual: підклас може лише приховати його через new (див. Задачу 3).
        public int GetPriority()
        {
            return 3;
        }

        public void MarkPaid()
        {
            if (!IsCancelled)
            {
                _isPaid = true;
            }
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
            _cancellationReason = reason ?? "";
            if (_cancellationReason.Length > 0) Notes = _cancellationReason;
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
            // GetDescription() і GetCost() — віртуальні, тому тут спрацьовує реалізація реального типу об'єкта.
            string line = $"[{Id}] {GetDescription()} | Пацієнт #{PatientId} → Лікар #{DoctorId} | {ScheduledAt:dd.MM.yyyy HH:mm}–{EndsAt:HH:mm} | {Status}";
            if (Notes.Length > 0) line += $" | {Notes}";
            line += $" | {GetCost():F2} грн";
            return line;
        }
    }
}