using System;
using System.Globalization;
using System.IO;
using System.Text;
using ClinicApp.Models;

namespace ClinicApp.Utils;

// Зберігає пацієнтів у файл між запусками програми й відновлює їх. Формат файлу:
//   [PATIENTS]
//   Іван,Петренко,15.03.1985,APositive,0501234567
//   [END]
// Рядок пацієнта має той самий формат, що й у CSV-імпорті, тому розбирається тим самим методом.
public class SessionManager
{
    private const string PatientsSection = "[PATIENTS]";
    private const string EndMarker = "[END]";
    private const string DateFormat = "dd.MM.yyyy";

    private readonly string _sessionPath;

    public SessionManager(string sessionPath = "session.dat")
    {
        if (string.IsNullOrWhiteSpace(sessionPath))
            throw new ArgumentException("Шлях до файлу сесії не може бути порожнім.", nameof(sessionPath));
        _sessionPath = sessionPath;
    }

    public bool Exists()
    {
        return File.Exists(_sessionPath);
    }

    // Перезаписує файл сесії. Зберігаються ВСІ пацієнти клініки, включно з початковими.
    public void Save(Clinic clinic)
    {
        if (clinic == null) throw new ArgumentNullException(nameof(clinic));

        // false = перезаписати файл повністю; using гарантує, що файл буде закрито.
        using (StreamWriter writer = new StreamWriter(_sessionPath, false, Encoding.UTF8))
        {
            writer.WriteLine(PatientsSection);
            foreach (Patient patient in clinic.Patients.GetAll())
            {
                // Кома — роздільник полів, тому в імені її замінюємо пробілом, інакше рядок зламається при читанні.
                string firstName = patient.FirstName.Replace(',', ' ');
                string lastName = patient.LastName.Replace(',', ' ');
                string birth = patient.DateOfBirth.ToString(DateFormat, CultureInfo.InvariantCulture);
                writer.WriteLine($"{firstName},{lastName},{birth},{patient.BloodType},{patient.Phone}");
            }
            writer.WriteLine(EndMarker);
        }
    }

    // Додає пацієнтів зі збереженої сесії в клініку. Повертає, СКІЛЬКИ пацієнтів додано (0, якщо файлу немає).
    public int Load(Clinic clinic)
    {
        if (clinic == null) throw new ArgumentNullException(nameof(clinic));
        if (!File.Exists(_sessionPath)) return 0;

        string[] lines = File.ReadAllLines(_sessionPath, Encoding.UTF8);

        string section = "";   // поточна секція; до першого заголовка рядки не належать жодній секції
        int added = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (line.Length == 0) continue;

            // Рядок, що починається з «[», — заголовок секції. Запам'ятовуємо її і обробляємо наступні рядки
            // відповідно до неї: так у майбутньому легко додати нову секцію, наприклад [APPOINTMENTS].
            if (line.StartsWith("["))
            {
                if (line == EndMarker) break;
                section = line;
                continue;
            }

            // Рядки поза секцією [PATIENTS] (до першого заголовка або в невідомих секціях) ігноруємо.
            if (section != PatientsSection) continue;

            try
            {
                // Дубль перевіряємо ДО створення Patient: об'єкт, що створюється, одразу бере новий Id зі
                // статичного лічильника, і відкинутий дубль «з'їв» би номер.
                string[] parts = line.Split(',');
                if (parts.Length == 5 && IsAlreadyPresent(clinic, parts[0].Trim(), parts[1].Trim(), parts[2].Trim()))
                    continue;

                Patient patient = CsvImporter.ParsePatient(line);
                clinic.Patients.Add(patient);
                added++;
            }
            catch (Exception e)
            {
                // Пошкоджений рядок пропускаємо: краще завантажити 9 з 10 пацієнтів, ніж упасти. Причину пишемо в лог.
                clinic.Logger.LogWarning($"Сесія: пропущено пошкоджений рядок {i + 1}: {CsvImporter.CleanMessage(e)}");
            }
        }

        return added;
    }

    // Пацієнт «уже є», якщо збігаються ім'я, прізвище і дата народження.
    private static bool IsAlreadyPresent(Clinic clinic, string firstName, string lastName, string birthText)
    {
        foreach (Patient existing in clinic.Patients.GetAll())
        {
            if (existing.FirstName == firstName
                && existing.LastName == lastName
                && existing.DateOfBirth.ToString(DateFormat, CultureInfo.InvariantCulture) == birthText)
            {
                return true;
            }
        }
        return false;
    }
}
