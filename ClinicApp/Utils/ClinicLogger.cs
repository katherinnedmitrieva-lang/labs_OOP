using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace ClinicApp.Utils;

// Журнал дій клініки. Кожен запис — один рядок у файлі, що ДОПИСУЄТЬСЯ в кінець (стара історія не стирається).
// Формат рядка: [yyyy-MM-dd HH:mm:ss] [LEVEL] message
public class ClinicLogger
{
    private readonly string _logPath;

    public ClinicLogger(string logPath = "clinic.log")
    {
        if (string.IsNullOrWhiteSpace(logPath))
            throw new ArgumentException("Шлях до файлу лога не може бути порожнім.", nameof(logPath));
        _logPath = logPath;
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
