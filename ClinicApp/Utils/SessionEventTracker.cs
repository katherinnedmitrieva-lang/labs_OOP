using System;
using System.Globalization;
using System.IO;
using System.Text;
using ClinicApp.Events;
using ClinicApp.Models;

namespace ClinicApp.Utils;

// Третій підписник: рахує події за сесію й реагує на скасування запису перевіркою черги очікування.
// Скасування — подія AppointmentManager, а черга живе в Clinic. Tracker зв'язує їх БЕЗ прямої залежності
// між менеджерами: AppointmentManager нічого не знає про чергу, а черга — про AppointmentManager.
public class SessionEventTracker
{
    // Ширина колонки з назвами лічильників — щоб числа в підсумку стояли рівно.
    private const int LabelWidth = 21;

    private readonly Clinic _clinic;

    // Лічильники: public get (читати можна всім), private set (змінювати — лише обробники цього класу).
    public int PatientsAdded { get; private set; }
    public int AppointmentsBooked { get; private set; }
    public int UrgentBooked { get; private set; }
    public int AppointmentsCancelled { get; private set; }
    public int AppointmentsCompleted { get; private set; }
    public int PaymentsReceived { get; private set; }
    public int PlansCompleted { get; private set; }

    public SessionEventTracker(Clinic clinic)
    {
        if (clinic == null) throw new ArgumentNullException(nameof(clinic));
        _clinic = clinic;
    }

    // ===== Обробники подій: кожен збільшує свій лічильник =====

    public void OnPatientAdded(object? sender, PatientEventArgs e)
    {
        PatientsAdded++;
    }

    public void OnAppointmentBooked(object? sender, AppointmentEventArgs e)
    {
        AppointmentsBooked++;
    }

    // Терміновий запис піднімає ДВІ події: AppointmentBooked (він входить у «Записів створено») і цю.
    public void OnUrgentAppointmentBooked(object? sender, AppointmentEventArgs e)
    {
        UrgentBooked++;
    }

    // Обробник скасування, окрім лічильника, перевіряє чергу очікування: слот звільнився — є кому його запропонувати?
    public void OnAppointmentCancelled(object? sender, AppointmentEventArgs e)
    {
        AppointmentsCancelled++;

        if (!_clinic.WaitingRoom.IsEmpty)
        {
            // Peek() лише ДИВИТЬСЯ на першого в черзі, а не видаляє його: кого записати на звільнений слот, вирішує людина.
            Patient next = _clinic.WaitingRoom.Peek();
            Console.WriteLine($"  [ЧЕРГА] Слот звільнився. Наступний: {next.FullName}");
        }
    }

    public void OnAppointmentCompleted(object? sender, AppointmentEventArgs e)
    {
        AppointmentsCompleted++;
    }

    public void OnPaymentReceived(object? sender, PaymentEventArgs e)
    {
        PaymentsReceived++;
    }

    public void OnPlanCompleted(object? sender, TreatmentPlanEventArgs e)
    {
        PlansCompleted++;
    }

    // ===== Підсумок сесії =====

    // Підсумок у консоль.
    public void PrintSummary()
    {
        Console.WriteLine("=== Підсумок сесії ===");
        foreach (string line in BuildSummaryLines())
        {
            Console.WriteLine(line);
        }
    }

    // Той самий підсумок у файл, з датою й часом формування. Файл перезаписується: це підсумок ОДНІЄЇ сесії.
    public void SaveSummary(string path = "session_summary.txt")
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Шлях до файлу підсумку не може бути порожнім.", nameof(path));

        using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
        {
            writer.WriteLine("=== Підсумок сесії ===");
            writer.WriteLine("Сформовано: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture));
            foreach (string line in BuildSummaryLines())
            {
                writer.WriteLine(line);
            }
        }
    }

    // Єдине місце, де формується текст підсумку: консоль і файл завжди показують однакові дані.
    private string[] BuildSummaryLines()
    {
        return new string[]
        {
            FormatLine("Пацієнтів додано:", PatientsAdded.ToString()),
            FormatLine("Записів створено:", $"{AppointmentsBooked} (термінових: {UrgentBooked})"),
            FormatLine("Скасовано:", AppointmentsCancelled.ToString()),
            FormatLine("Завершено:", AppointmentsCompleted.ToString()),
            FormatLine("Оплат:", PaymentsReceived.ToString()),
            FormatLine("Планів завершено:", PlansCompleted.ToString())
        };
    }

    // Назва, доповнена пробілами до фіксованої ширини, і значення: числа в колонці стоять рівно.
    private static string FormatLine(string label, string value)
    {
        return label.PadRight(LabelWidth) + value;
    }
}
