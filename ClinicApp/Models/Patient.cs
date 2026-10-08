using System;
using ClinicApp.Enums;
using ClinicApp.Utils;

namespace ClinicApp.Models;

public class Patient
{
    private static int _nextId = 1;

    private string _firstName = "";
    private string _lastName = "";
    private DateTime _dateOfBirth;
    private string _phone = "";

    public int Id { get; }
    public BloodType BloodType { get; set; }
    public string Email { get; set; }

    public string FirstName
    {
        get => _firstName;
        set => _firstName = value;
    }

    public string LastName
    {
        get => _lastName;
        set => _lastName = value;
    }

    public DateTime DateOfBirth
    {
        get => _dateOfBirth;
        set => _dateOfBirth = value;
    }

    public string Phone
    {
        get => _phone;
        set => _phone = value;
    }

    public string FullName => FirstName + " " + LastName;

    public int Age
    {
        get
        {
            int age = DateTime.Today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > DateTime.Today.AddYears(-age))
            {
                age--;
            }
            return age;
        }
    }

    public bool IsAdult => Age >= 18;

    public Patient()
        : this("Невідомий", "Пацієнт", DateTime.Today.AddYears(-26), BloodType.Unknown, "0000000000")
    {
    }

    public Patient(string firstName, string lastName)
        : this(firstName, lastName, DateTime.Today.AddYears(-18), BloodType.Unknown, "0000000000")
    {
    }

    public Patient(string firstName, string lastName, DateTime dateOfBirth, BloodType bloodType, string phone)
    {
        Id = _nextId++;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        BloodType = bloodType;
        Phone = phone;
        Email = "";
    }

    public string GetAgeCategory()
    {
        if (Age < 18) return "дитина";
        if (Age < 60) return "дорослий";
        return "літній";
    }

    public override string ToString()
    {
        string ageText = ClinicFormatter.FormatAge(Age);
        string bloodText = ClinicFormatter.FormatBloodType(BloodType);
        string phoneText = ClinicFormatter.FormatPhone(Phone);
        return $"[{Id}] {FullName} | Вік: {ageText} ({GetAgeCategory()}) | Кров: {bloodText} | Тел: {phoneText}";
    }
}