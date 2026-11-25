using System;
using System.Globalization;
using System.IO;
using System.Text;
using ClinicApp.Events;

namespace ClinicApp.Utils;

// Журнал дій клініки. Кожен запис — один рядок у файлі, що ДОПИСУЄТЬСЯ в кінець (стара історія не стирається).
// Формат рядка: [yyyy-MM-dd HH:mm:ss] [LEVEL] message
public class ClinicLogger
{
    private readonly string _logPath;
    private readonly string _alertsDir;

    public ClinicLogger(string logPath = "clinic.log", string alertsDir = "alerts")
    {
        if (string.IsNullOrWhiteSpace(logPath))
            throw new ArgumentException("Шлях до файлу лога не може бути порожнім.", nameof(logPath));
        if (string.IsNullOrWhiteSpace(alertsDir))
            throw new ArgumentException("Тека для термінових сповіщень не може бути порожньою.", nameof(alertsDir));
        _logPath = logPath;
        _alertsDir = alertsDir;
    }

    public void LogInfo(string message)
    {
        Write("INFO", message);
    }

    public void LogWarning(string message)
    {
        Write("WARN", message);
    }

    public void LogError(string message)
    {
        Write("ERROR", message);
    }

    // Останні n рядків лога (від старішого до новішого). Порожній масив, якщо файлу немає або n <= 0.
    public string[] GetLastLines(int n)
    {
        if (n <= 0 || !File.Exists(_logPath)) return new string[0];

        // ReadAllLines читає весь файл у пам'ять — для журналу це прийнятно.
        string[] allLines = File.ReadAllLines(_logPath, Encoding.UTF8);

        // Скільки рядків пропустити з початку. Math.Max гарантує, що результат не від'ємний:
        // якщо просять більше рядків, ніж є у файлі, віддаємо всі наявні.
        int skip = Math.Max(0, allLines.Length - n);
        string[] result = new string[allLines.Length - skip];
        Array.Copy(allLines, skip, result, 0, result.Length);
        return result;
    }

    // Очищення лога = видалення файлу. Наступний запис створить його заново.
    public void Clear()
    {
        if (File.Exists(_logPath))
            File.Delete(_logPath);
    }

    public bool Exists()
    {
        return File.Exists(_logPath);
    }

    // ===== Обробники подій =====
    // Logger — ПІДПИСНИК: менеджери піднімають події, а він реагує сам, автоматично й завжди.
    // Сигнатура (object? sender, XxxEventArgs e) задається делегатом EventHandler<T>.
    // Підписує ці методи на події клас Clinic (метод SubscribeEvents).

    public void OnPatientAdded(object? sender, PatientEventArgs e)
    {
        LogInfo($"Новий пацієнт #{e.PatientId}: {e.FullName}");
    }

    public void OnAppointmentBooked(object? sender, AppointmentEventArgs e)
    {
        LogInfo($"Запис #{e.AppointmentId} створено: пацієнт #{e.PatientId} → лікар #{e.DoctorId}");
    }

    // Терміновий запис: попередження в лог + окремий рядок у файлі alerts/urgent_{yyyy-MM-dd}.txt.
    // Подію UrgentAppointmentBooked менеджер піднімає ПОРУЧ з AppointmentBooked, тож у лозі буде і звичайний рядок про створення запису, і цей.
    public void OnUrgentAppointmentBooked(object? sender, AppointmentEventArgs e)
    {
        string reason = e.Notes.Length > 0 ? ": " + e.Notes : "";
        LogWarning($"ТЕРМІНОВИЙ запис #{e.AppointmentId}{reason}");
        WriteUrgentAlert(e, reason);
    }

    public void OnAppointmentCancelled(object? sender, AppointmentEventArgs e)
    {
        // У Notes скасованого запису лежить причина скасування (якщо її вказали).
        string reason = e.Notes.Length > 0 ? " (причина: " + e.Notes + ")" : "";
        LogInfo($"Запис #{e.AppointmentId} скасовано{reason}");
    }

    public void OnAppointmentCompleted(object? sender, AppointmentEventArgs e)
    {
        LogInfo($"Запис #{e.AppointmentId} завершено");
    }

    public void OnPaymentReceived(object? sender, PaymentEventArgs e)
    {
        LogInfo($"Оплата запису #{e.AppointmentId}: {e.Amount.ToString("F2", CultureInfo.InvariantCulture)} грн");
    }

    public void OnPlanCompleted(object? sender, TreatmentPlanEventArgs e)
    {
        LogInfo($"План лікування #{e.PlanId} завершено: пацієнт #{e.PatientId}, {e.Diagnosis}");
    }

    // Додатковий рядок про терміновий запис у файл дня. Теку alerts створюємо перед записом
    // (CreateDirectory не кидає виняток, якщо тека вже є). Збій запису не повинен валити програму, як і в Write.
    private void WriteUrgentAlert(AppointmentEventArgs e, string reason)
    {
        DateTime now = DateTime.Now;
        string fileName = "urgent_" + now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".txt";
        string time = now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        string scheduled = e.ScheduledAt.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
        string line = $"[{time}] ТЕРМІНОВИЙ запис #{e.AppointmentId}{reason} (пацієнт #{e.PatientId} → лікар #{e.DoctorId}, {scheduled}){Environment.NewLine}";

        try
        {
            Directory.CreateDirectory(_alertsDir);
            File.AppendAllText(Path.Combine(_alertsDir, fileName), line, Encoding.UTF8);
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine("Не вдалося записати термінове сповіщення: " + ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.Error.WriteLine("Немає доступу до теки сповіщень: " + ex.Message);
        }
    }

    // Єдине місце, де формується рядок лога: формат однаковий для всіх рівнів.
    private void Write(string level, string message)
    {
        // Запис — це ОДИН рядок, тому переноси всередині повідомлення замінюємо пробілами.
        string text = (message ?? "").Replace("\r\n", " ").Replace('\r', ' ').Replace('\n', ' ');
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        // {level,-5} вирівнює рівень по лівому краю до 5 символів: [INFO ], [WARN ], [ERROR].
        string line = $"[{timestamp}] [{level,-5}] {text}{Environment.NewLine}";

        try
        {
            // Encoding.UTF8 — щоб кирилиця записувалась коректно. Якщо файлу немає, його буде створено.
            File.AppendAllText(_logPath, line, Encoding.UTF8);
        }
        catch (IOException e)
        {
            // Збій журналювання (файл зайнятий, диск заповнений) не повинен валити програму.
            Console.Error.WriteLine("Не вдалося записати в лог: " + e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            Console.Error.WriteLine("Немає доступу до файлу лога: " + e.Message);
        }
    }
}
