using System;

namespace ClinicApp;

public static class ClinicFormatter
{
    public static string FormatBloodType(BloodType bt)
    {
        switch (bt)
        {
            case BloodType.APositive: return "A+";
            case BloodType.ANegative: return "A-";
            case BloodType.BPositive: return "B+";
            case BloodType.BNegative: return "B-";
            case BloodType.ABPositive: return "AB+";
            case BloodType.ABNegative: return "AB-";
            case BloodType.OPositive: return "O+";
            case BloodType.ONegative: return "O-";
            default: return "Невідомо";
        }
    }

    public static string FormatSpeciality(Speciality s)
    {
        switch (s)
        {
            case Speciality.Therapist: return "Терапевт";
            case Speciality.Cardiologist: return "Кардіолог";
            case Speciality.Pediatrician: return "Педіатр";
            case Speciality.Surgeon: return "Хірург";
            case Speciality.Neurologist: return "Невролог";
            case Speciality.Dermatologist: return "Дерматолог";
            case Speciality.Ophthalmologist: return "Офтальмолог";
            case Speciality.Dentist: return "Стоматолог";
            default: return "Загальна практика";
        }
    }

    public static string FormatAge(int age)
    {
        int mod100 = age % 100;
        if (mod100 >= 11 && mod100 <= 19) return $"{age} років";

        int mod10 = age % 10;
        if (mod10 == 1) return $"{age} рік";
        if (mod10 >= 2 && mod10 <= 4) return $"{age} роки";
        return $"{age} років";
    }

    public static string FormatPhone(string phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 10) return phone;
        string clean = phone.Substring(phone.Length - 10);
        return $"({clean.Substring(0, 3)}) {clean.Substring(3, 3)}-{clean.Substring(6)}";
    }
}
