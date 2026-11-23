using System;
using System.Globalization;
using System.IO;
using System.Text;
using ClinicApp.Enums;
using ClinicApp.Models;

namespace ClinicApp.Utils;

// Імпорт пацієнтів із CSV. Формат (перший рядок — заголовок):
//   FirstName,LastName,DateOfBirth,BloodType,Phone
//   Іван,Петренко,15.03.1985,APositive,0501234567
// Помилковий рядок не зупиняє імпорт: він потрапляє в ImportResult з номером і причиною.
public static class CsvImporter
{
    private const int FieldCount = 5;
    private const string DateFormat = "dd.MM.yyyy";

    public static ImportResult ImportPatients(Clinic clinic, string filePath)
    {
        if (clinic == null) throw new ArgumentNullException(nameof(clinic));

        ImportResult result = new ImportResult();

        // Немає шляху або файлу — одна помилка з номером рядка 0 (це помилка файлу, а не рядка).
        if (string.IsNullOrWhiteSpace(filePath))
        {
            result.AddError(0, "Шлях до файлу не вказано.");
            return result;
        }
        if (!File.Exists(filePath))
        {
            result.AddError(0, $"Файл не знайдено: {filePath}");
            return result;
        }

        string[] lines;
        try
        {
            // Encoding.UTF8 — кирилиця; BOM на початку файлу (його додає Excel) відкидається автоматично.
            lines = File.ReadAllLines(filePath, Encoding.UTF8);
        }
        catch (IOException e)
        {
            result.AddError(0, "Не вдалося прочитати файл: " + e.Message);
            return result;
        }
        catch (UnauthorizedAccessException e)
        {
            result.AddError(0, "Немає доступу до файлу: " + e.Message);
            return result;
        }

        // i = 0 — заголовок, тому починаємо з 1.
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;   // порожні рядки мовчки пропускаємо

            int lineNumber = i + 1;   // для людини рядки нумеруються з 1

            // try/catch — навколо ОДНОГО рядка, а не всього циклу: помилка в рядку 3 не зупиняє рядки 4, 5, 6...
            try
            {
                Patient patient = ParsePatient(line);
                clinic.Patients.Add(patient);
                result.AddSuccess();
            }
            catch (Exception e)
            {
                result.AddError(lineNumber, CleanMessage(e));
            }
        }

        return result;
    }

    // Розбирає один рядок CSV у пацієнта. Кидає виняток із зрозумілим поясненням, якщо рядок некоректний.
    // Метод відкритий, бо його ж використовує SessionManager: формат рядка пацієнта в обох місцях однаковий.
    public static Patient ParsePatient(string line)
    {
        string[] parts = line.Split(',');
        if (parts.Length != FieldCount)
            throw new FormatException($"Рядок має {parts.Length} {FieldWord(parts.Length)} замість {FieldCount}.");

        string firstName = parts[0].Trim();
        string lastName = parts[1].Trim();
        string dateText = parts[2].Trim();
        string bloodText = parts[3].Trim();
        string phone = parts[4].Trim();

        // Імена перевіряємо ПЕРШИМИ, щоб про порожнє ім'я повідомити раніше, ніж про дату чи групу крові.
        if (firstName.Length == 0)
            throw new FormatException("Ім'я не може бути порожнім.");
        if (lastName.Length == 0)
            throw new FormatException("Прізвище не може бути порожнім.");

        DateTime dateOfBirth;
        try
        {
            // ParseExact — суворий парсинг: приймає лише формат дд.мм.рррр.
            dateOfBirth = DateTime.ParseExact(dateText, DateFormat, CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            throw new FormatException($"Некоректна дата народження «{dateText}» (очікується дд.мм.рррр).");
        }

        BloodType bloodType = BloodType.Unknown;   // порожнє поле = група крові невідома
        if (bloodText.Length > 0)
        {
            try
            {
                // true = ігнорувати регістр: «apositive» і «APositive» — те саме значення.
                bloodType = (BloodType)Enum.Parse(typeof(BloodType), bloodText, true);
            }
            catch (ArgumentException)
            {
                throw new FormatException($"Невідома група крові «{bloodText}».");
            }
            // Enum.Parse приймає й числа («99»), хоча такого значення в enum немає — відсікаємо їх.
            if (!Enum.IsDefined(typeof(BloodType), bloodType))
                throw new FormatException($"Невідома група крові «{bloodText}».");
        }

        // Решту (задовге ім'я, телефон, дату в майбутньому) перевіряє сам конструктор Patient через ClinicValidator (Лаба 05).
        return new Patient(firstName, lastName, dateOfBirth, bloodType, phone);
    }

    // «1 поле», «2 поля», «5 полів», «11 полів», «21 поле» — правильна форма слова для української мови.
    private static string FieldWord(int count)
    {
        int lastTwo = count % 100;
        int last = count % 10;
        if (lastTwo >= 11 && lastTwo <= 14) return "полів";
        if (last == 1) return "поле";
        if (last >= 2 && last <= 4) return "поля";
        return "полів";
    }

    // .NET дописує до ArgumentException технічний суфікс « (Parameter 'phone')» — для користувача він зайвий.
    // Відкритий, бо його використовує й SessionManager для запису причин у лог.
    public static string CleanMessage(Exception e)
    {
        string message = e.Message;
        int index = message.IndexOf(" (Parameter '");
        if (index >= 0) message = message.Substring(0, index);
        return message;
    }
}
