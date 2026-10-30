using System;
using ClinicApp.Enums;
using ClinicApp.Utils;
using ClinicApp.Interfaces;

namespace ClinicApp.Models
{
    public class Doctor : ISchedulable
    {
        private static int _nextId = 1;

        private string _firstName = "";
        private string _lastName = "";
        private string _licenseNumber = "";
        private string _phone = "";

        public int Id { get; }
        public Speciality Speciality { get; set; }
        public WorkSchedule Schedule { get; set; }

        public string FirstName
        {
            get => _firstName;
            set
            {
                ClinicValidator.ValidateName(value, nameof(FirstName));
                _firstName = value;
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                ClinicValidator.ValidateName(value, nameof(LastName));
                _lastName = value;
            }
        }

        public string LicenseNumber
        {
            get => _licenseNumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Номер ліцензії не може бути порожнім.", nameof(LicenseNumber));
                _licenseNumber = value;
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                ClinicValidator.ValidatePhone(value);
                _phone = value;
            }
        }

        public string FullName => FirstName + " " + LastName;

        public int WorkingHoursPerDay => Schedule.HoursPerDay;
        public string WorkSchedule => Schedule.Display;
        public bool IsAvailableNow => Schedule.IsNow;

        public Doctor()
            : this("Невідомий", "Лікар", Speciality.Therapist)
        {
        }

        public Doctor(string firstName, string lastName, Speciality speciality)
            : this(firstName, lastName, speciality, "LIC-000", "0000000000")
        {
        }

        public Doctor(string firstName, string lastName, Speciality speciality, string licenseNumber, string phone)
        {
            FirstName = firstName;
            LastName = lastName;
            Speciality = speciality;
            LicenseNumber = licenseNumber;
            Phone = phone;
            Schedule = new WorkSchedule(8, 17);
            Id = _nextId++;   // ОСТАННІМ рядком
        }

        public bool CanAcceptAt(int hour)
        {
            return Schedule.Contains(hour);
        }

        public override string ToString()
        {
            string status = IsAvailableNow ? "доступний зараз" : "не в робочий час";
            string specText = ClinicFormatter.FormatSpeciality(Speciality);
            string phoneText = ClinicFormatter.FormatPhone(Phone);
            return $"[{Id}] {FullName} | {specText} | {LicenseNumber} | Тел: {phoneText} | {WorkSchedule} ({WorkingHoursPerDay} god) | {status}";
        }

        // Реалізація ISchedulable
        public bool CanSchedule(DateTime at)
        {
            return at.Hour >= 9 && at.Hour < 17;
        }

        public DateTime[] GetAvailableSlots(DateTime date, int slotCount)
        {
            // Передаємо назву параметра, як вимагає твій валідатор
            ClinicValidator.ValidatePositive(slotCount, nameof(slotCount));

            var slots = new System.Collections.Generic.List<DateTime>();
            for (int i = 9; i < 9 + slotCount && i < 17; i++)
            {
                slots.Add(new DateTime(date.Year, date.Month, date.Day, i, 0, 0));
            }
            return slots.ToArray();
        }
    }
}