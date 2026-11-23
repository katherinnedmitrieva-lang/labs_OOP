using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ClinicApp.Interfaces;
using ClinicApp.Models;

namespace ClinicApp.Utils;

// Експортує звіти клініки у текстові файли. Файли кладуться в теку з датою: <baseDir>/yyyy-MM-dd/.
public class ClinicExporter
{
    private const int SeparatorWidth = 40;

    private readonly Clinic _clinic;
    private readonly string _baseDir;

    public ClinicExporter(Clinic clinic, string baseDir = "reports")
    {
        if (clinic == null) throw new ArgumentNullException(nameof(clinic));
        if (string.IsNullOrWhiteSpace(baseDir))
            throw new ArgumentException("Базова тека не може бути порожньою.", nameof(baseDir));
        _clinic = clinic;
        _baseDir = baseDir;
    }

    // patients.txt — усі пацієнти
    public string ExportPatients()
    {
        Patient[] patients = _clinic.Patients.GetAll();
        string[] rows = new string[patients.Length];
        for (int i = 0; i < patients.Length; i++)
        {
            rows[i] = patients[i].ToString();
        }
        return WriteReport("patients.txt", "Пацієнти", rows, new string[0]);
    }

    // appointments.txt — усі записи
    public string ExportAppointments()
    {
        // GetAll() повертає масив із «порожніми» (null) комірками — пропускаємо їх.
        Appointment[] all = _clinic.Appointments.GetAll();
        List<string> rows = new List<string>();
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i] != null) rows.Add(all[i].ToString());
        }
        return WriteReport("appointments.txt", "Записи", rows.ToArray(), new string[0]);
    }

    // billing.txt — неоплачені записи і загальний борг
    public string ExportBilling()
    {
        IPayable[] unpaid = _clinic.Billing.GetUnpaidPayables();
        string[] rows = new string[unpaid.Length];
        for (int i = 0; i < unpaid.Length; i++)
        {
            rows[i] = unpaid[i].ToString() ?? "";
        }
        string debtLine = "Загальний борг: " + _clinic.Billing.GetTotalDebt().ToString("F2", CultureInfo.InvariantCulture) + " грн";
        return WriteReport("billing.txt", "Неоплачені рахунки", rows, new string[] { debtLine });
    }

    // treatment_plans.txt — усі плани лікування
    public string ExportTreatmentPlans()
    {
        TreatmentPlan[] plans = _clinic.TreatmentPlans.GetAll();
        string[] rows = new string[plans.Length];
        for (int i = 0; i < plans.Length; i++)
        {
            rows[i] = plans[i].ToString();
        }
        return WriteReport("treatment_plans.txt", "Плани лікування", rows, new string[0]);
    }

    // Викликає всі чотири експорти й повертає шляхи створених файлів.
    public string[] ExportAll()
    {
        return new string[]
        {
            ExportPatients(),
            ExportAppointments(),
            ExportBilling(),
            ExportTreatmentPlans()
        };
    }

    // Єдине місце, де формується файл звіту, — усі чотири експорти мають однаковий вигляд:
    // заголовок, дата генерації, роздільник, нумеровані рядки, роздільник, підсумок.
    private string WriteReport(string fileName, string title, string[] rows, string[] extraSummaryLines)
    {
        DateTime generatedAt = DateTime.Now;
        string dir = CreateDateDirectory(generatedAt);

        // Path.Combine сам підставляє правильний роздільник (\ або /) для поточної ОС.
        string path = Path.Combine(dir, fileName);
        string separator = new string('-', SeparatorWidth);

        // using закриває файл при виході з блоку навіть після винятку — без цього файл міг би лишитись відкритим.
        // false = перезаписати файл, якщо він уже існує (звіт завжди свіжий).
        using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
        {
            writer.WriteLine($"=== {title} — звіт ===");
            writer.WriteLine("Згенеровано: " + generatedAt.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture));
            writer.WriteLine(separator);
            for (int i = 0; i < rows.Length; i++)
            {
                // {i + 1,3} — номер, вирівняний по правому краю в 3 символи.
                writer.WriteLine($"{i + 1,3}. {rows[i]}");
            }
            writer.WriteLine(separator);
            writer.WriteLine($"Всього: {rows.Length}");
            for (int i = 0; i < extraSummaryLines.Length; i++)
            {
                writer.WriteLine(extraSummaryLines[i]);
            }
        }

        return path;
    }

    // Тека з датою генерації: reports/2026-10-15. Створюється, якщо її ще немає
    // (CreateDirectory не кидає виняток, якщо тека вже існує, і створює всі проміжні теки).
    private string CreateDateDirectory(DateTime date)
    {
        string dir = Path.Combine(_baseDir, date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        Directory.CreateDirectory(dir);
        return dir;
    }
}
