using System;
using System.Text.RegularExpressions;

namespace ClinicApp.Utils;

public static class ClinicValidator
{
    // Створюються один раз, а не при кожній перевірці.
    // [0-9] — лише ASCII-цифри (на відміну від \d), \z — суворий кінець рядка (на відміну від $)
    private static readonly Regex PhoneRegex = new Regex(@"^[0-9]{10}\z");
    private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+\z");

    public static void ValidateName(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"Поле «{fieldName}» не може бути порожнім.", fieldName);
        if (value.Length > 50)
            throw new ArgumentException($"Поле «{fieldName}» не може бути довшим за 50 символів.", fieldName);
    }

    public static void ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Телефон не може бути порожнім.", nameof(phone));
        if (!PhoneRegex.IsMatch(phone))
            throw new ArgumentException("Телефон має містити рівно 10 цифр.", nameof(phone));
    }

    public static void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
            throw new ArgumentException("Email має мати вигляд імʼя@домен.зона без пробілів.", nameof(email));
    }

    public static void ValidateDate(DateTime value, string fieldName)
    {
        if (value.Date > DateTime.Today)
            throw new ArgumentOutOfRangeException(fieldName, $"Поле «{fieldName}» не може бути в майбутньому.");
        if (value.Year < 1900)
            throw new ArgumentOutOfRangeException(fieldName, $"Поле «{fieldName}» не може бути раніше 1900 року.");
    }

    public static void ValidatePositive(int value, string fieldName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(fieldName, $"Поле «{fieldName}» має бути більшим за 0.");
    }
}