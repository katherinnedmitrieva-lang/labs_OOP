using System;
using System.Globalization;
using System.Threading;
using ClinicApp;
using ClinicApp.Enums;
using ClinicApp.Managers;
using ClinicApp.Models;
using ClinicApp.Utils;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
Console.OutputEncoding = System.Text.Encoding.UTF8;

Clinic clinic = new Clinic("Медична Клініка \"Здоров'я\"");

Patient p1 = new Patient("Іван", "Петренко", new DateTime(1985, 3, 12), BloodType.APositive, "0501234567");
Patient p2 = new Patient("Олена", "Коваль", new DateTime(1993, 7, 4), BloodType.BNegative, "0672345678");
Patient p3 = new Patient("Максим", "Бойко", new DateTime(2010, 1, 20), BloodType.OPositive, "0933456789");
Patient p4 = new Patient();
Patient p5 = new Patient("Марія", "Ткач");
clinic.Patients.Add(p1);
clinic.Patients.Add(p2);
clinic.Patients.Add(p3);
clinic.Patients.Add(p4);
clinic.Patients.Add(p5);

Doctor d1 = new Doctor("Олег", "Сидоренко", Speciality.Cardiologist, "LIC-001", "0441234567");
d1.Schedule = new WorkSchedule(8, 16);
Doctor d2 = new Doctor("Nataliya", "Moroz", Speciality.Neurologist, "LIC-002", "0442345678");
d2.Schedule = new WorkSchedule(9, 18);
Doctor d3 = new Doctor("Андрій", "Власенко", Speciality.Pediatrician, "LIC-003", "0443456789");
clinic.Doctors.Add(d1);
clinic.Doctors.Add(d2);
clinic.Doctors.Add(d3);

DateTime baseDate = DateTime.Today.AddDays(1);
clinic.Appointments.Book(p1.Id, d1.Id, baseDate.AddHours(10), 30);
clinic.Appointments.Book(p2.Id, d2.Id, baseDate.AddHours(11), 45);
clinic.Appointments.Book(p3.Id, d3.Id, baseDate.AddDays(1).AddHours(9), 20);
// ===== Медичні записи (Лаба 06) =====
DateTime today = DateTime.Today;

// Пацієнт #1: два діагнози (хронічний + гострий), два аналізи, активний рецепт
clinic.MedicalRecords.Add(new Diagnosis(p1.Id, d1.Id, today.AddDays(-30), "I10", "Гіпертонічна хвороба", true));
clinic.MedicalRecords.Add(new Diagnosis(p1.Id, d1.Id, today.AddDays(-5), "J06.9", "Гострий ринофарингіт"));
clinic.MedicalRecords.Add(new LabResult(p1.Id, d2.Id, today.AddDays(-10), "Гемоглобін", 145, "г/л", "120–160", true));
clinic.MedicalRecords.Add(new LabResult(p1.Id, d2.Id, today.AddDays(-10), "Холестерин", 6.2, "ммоль/л", "< 5.2", false));
clinic.MedicalRecords.Add(new Prescription(p1.Id, d1.Id, today.AddDays(-5), "Лізиноприл", "10 мг", 30, "1 раз на добу вранці"));

// Пацієнт #2: завершений рецепт (40 днів тому, курс 10 днів, IsActive() == false) + діагноз
clinic.MedicalRecords.Add(new Prescription(p2.Id, d2.Id, today.AddDays(-40), "Ібупрофен", "200 мг", 10, "після їди"));
clinic.MedicalRecords.Add(new Diagnosis(p2.Id, d2.Id, today.AddDays(-3), "G43.9", "Мігрень"));

// Пацієнт #3: аналіз, інший лікар
clinic.MedicalRecords.Add(new LabResult(p3.Id, d3.Id, today.AddDays(-2), "Лейкоцити", 7.5, "10^9/л", "4.0–9.0", true));

WorkSchedule morning = new WorkSchedule(8, 12);
WorkSchedule copy = morning;
copy = new WorkSchedule(9, 14);

string testAge = ClinicFormatter.FormatAge(21);
string testBlood = ClinicFormatter.FormatBloodType(BloodType.APositive);

Doctor? testIndexerDoc = clinic.Doctors[0];
Patient? testIndexerPat = clinic.Patients[0];

string? checkNullConditional = clinic.Patients.FindById(99)?.FullName;
string checkNullCoalescing = clinic.Patients.FindById(99)?.FullName ?? "";

clinic.Patients.TryFindById(1, out Patient? foundPatient);

Doctor[] searchByEnum = clinic.Doctors.FindBySpeciality(Speciality.Cardiologist);
Doctor[] searchByString = clinic.Doctors.FindBySpeciality("кардіо");
Appointment[] searchByTripleIntDate = clinic.Appointments.GetByDate(2026, 5, 10);

RunMainMenu(clinic);

void RunMainMenu(Clinic activeClinic)
{
    bool running = true;
    while (running)
    {
        Console.WriteLine("\nГОЛОВНЕ МЕНЮ");
        Console.WriteLine("1. Пацієнти");
        Console.WriteLine("2. Лікарі");
        Console.WriteLine("3. Записи");
        Console.WriteLine("4. Медична картка");
        Console.WriteLine("5. Звіт клініки");
        Console.WriteLine("6. Тест GrowablePatientManager");
        Console.WriteLine("7. Розклад на дату");
        Console.WriteLine("0. Вихід");
        Console.Write("Ваш вибір: ");
        string choice = Console.ReadLine()!;

        switch (choice)
        {
            case "1": RunPatientsMenu(activeClinic.Patients); break;
            case "2": RunDoctorsMenu(activeClinic.Doctors); break;
            case "3": RunAppointmentsMenu(activeClinic.Appointments, activeClinic.Patients, activeClinic.Doctors); break;
            case "4": RunMedicalRecordsMenu(activeClinic); break;
            case "5": activeClinic.GenerateReport(); break;
            case "6": RunGrowableDemo(); break;
            case "7":
                Console.Write("Дата (дд.мм.рррр): ");
                if (DateTime.TryParse(Console.ReadLine()!, out DateTime date))
                    activeClinic.DisplaySchedule(date);
                break;
            case "0": running = false; break;
        }
    }
}

void RunPatientsMenu(PatientManager manager)
{
    bool inMenu = true;
    while (inMenu)
    {
        Console.WriteLine("\nПацієнти:");
        Console.WriteLine("1. Показати всіх");
        Console.WriteLine("2. Додати пацієнта");
        Console.WriteLine("3. Знайти за ім'ям");
        Console.WriteLine("4. Видалити за ID");
        Console.WriteLine("5. Статистика");
        Console.WriteLine("0. Назад");
        Console.Write("Ваш вибір: ");
        string choice = Console.ReadLine()!;
        switch (choice)
        {
            case "1": manager.DisplayAll(); break;
            case "2":
                try
                {
                    Console.Write("Ім'я: "); string fn = Console.ReadLine()!;
                    Console.Write("Прізвище: "); string ln = Console.ReadLine()!;
                    Console.Write("Дата народження: "); DateTime.TryParse(Console.ReadLine()!, out DateTime dob);

                    Console.Write("Група крові (напр. APositive, BNegative, Unknown): ");
                    if (!Enum.TryParse(Console.ReadLine()!, true, out BloodType bType)
                        || !Enum.IsDefined(typeof(BloodType), bType))
                    {
                        bType = BloodType.Unknown;
                    }

                    Console.Write("Телефон: "); string ph = Console.ReadLine()!;
                    manager.Add(new Patient(fn, ln, dob, bType, ph));
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine("Помилка (значення поза межами): " + e.Message);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("Помилка: " + e.Message);
                }
                break;
            case "3":
                Console.Write("Частина імені: ");
                Patient[] found = manager.FindByName(Console.ReadLine()!);
                if (found.Length == 0) Console.WriteLine("Нікого не знайдено.");
                else foreach (Patient p in found) Console.WriteLine(p);
                break;
            case "4":
                Console.Write("ID: "); int.TryParse(Console.ReadLine()!, out int rid);
                Console.WriteLine(manager.Remove(rid) ? "Видалено." : "Не знайдено.");
                break;
            case "5": manager.DisplayStats(); break;
            case "0": inMenu = false; break;
        }
    }
}

void RunDoctorsMenu(DoctorManager manager)
{
    bool inMenu = true;
    while (inMenu)
    {
        Console.WriteLine("\nЛікарі:");
        Console.WriteLine("1. Показати всіх");
        Console.WriteLine("2. Додати лікаря");
        Console.WriteLine("3. Знайти за спеціальністю");
        Console.WriteLine("4. Видалити за ID");
        Console.WriteLine("5. Статистика");
        Console.WriteLine("0. Назад");
        Console.Write("Ваш вибір: ");
        string choice = Console.ReadLine()!;
        switch (choice)
        {
            case "1": manager.DisplayAll(); break;
            case "2":
                try
                {
                    Console.Write("Ім'я: "); string fn = Console.ReadLine()!;
                    Console.Write("Прізвище: "); string ln = Console.ReadLine()!;

                    Console.Write("Спеціальність (напр. Therapist, Cardiologist, Pediatrician): ");
                    if (!Enum.TryParse(Console.ReadLine()!, true, out Speciality spec)
                        || !Enum.IsDefined(typeof(Speciality), spec))
                    {
                        spec = Speciality.Therapist;
                    }

                    Console.Write("Ліцензія: "); string lic = Console.ReadLine()!;
                    Console.Write("Телефон: "); string ph = Console.ReadLine()!;

                    Console.Write("Година початку роботи: "); int.TryParse(Console.ReadLine()!, out int startH);
                    Console.Write("Година кінця роботи: "); int.TryParse(Console.ReadLine()!, out int endH);

                    // розклад створюємо ДО лікаря: якщо години некоректні — Id лікаря не витрачається
                    WorkSchedule schedule = new WorkSchedule(startH, endH);
                    Doctor newDoc = new Doctor(fn, ln, spec, lic, ph);
                    newDoc.Schedule = schedule;
                    manager.Add(newDoc);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine("Помилка (значення поза межами): " + e.Message);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("Помилка: " + e.Message);
                }
                break;
            case "3":
                Console.Write("Спеціальність (напр. Therapist, Cardiologist): ");
                string searchInput = Console.ReadLine()!;

                if (Enum.TryParse(searchInput, true, out Speciality searchSpec))
                {
                    Doctor[] foundDocs = manager.FindBySpeciality(searchSpec);
                    if (foundDocs.Length == 0) Console.WriteLine("Нікого не знайдено.");
                    else foreach (Doctor d in foundDocs) Console.WriteLine(d);
                }
                else
                {
                    Doctor[] foundDocs = manager.FindBySpeciality(searchInput);
                    if (foundDocs.Length == 0) Console.WriteLine("Нікого не знайдено.");
                    else foreach (Doctor d in foundDocs) Console.WriteLine(d);
                }
                break;
            case "4":
                Console.Write("ID: "); int.TryParse(Console.ReadLine()!, out int rid);
                Console.WriteLine(manager.Remove(rid) ? "Видалено." : "Не знайдено.");
                break;
            case "5": manager.DisplayStats(); break;
            case "0": inMenu = false; break;
        }
    }
}

void RunAppointmentsMenu(AppointmentManager manager, PatientManager patients, DoctorManager doctors)
{
    bool inMenu = true;
    while (inMenu)
    {
        Console.WriteLine("\nЗаписи:");
        Console.WriteLine("1. Майбутні записи");
        Console.WriteLine("2. Новий запис");
        Console.WriteLine("3. Скасувати запис");
        Console.WriteLine("4. Завершити запис");
        Console.WriteLine("0. Назад");
        Console.Write("Ваш вибір: ");
        string choice = Console.ReadLine()!;
        switch (choice)
        {
            case "1": manager.DisplayList(manager.GetUpcoming()); break;
            case "2":
                patients.DisplayAll(); doctors.DisplayAll();
                try
                {
                    Console.Write("ID пацієнта: "); int.TryParse(Console.ReadLine()!, out int pid);
                    Console.Write("ID лікаря: "); int.TryParse(Console.ReadLine()!, out int did);
                    Console.Write("Дата та час: "); DateTime.TryParse(Console.ReadLine()!, out DateTime sched);
                    Console.Write("Тривалість (хв): "); int.TryParse(Console.ReadLine()!, out int dur);
                    manager.Book(pid, did, sched, dur);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine("Помилка (значення поза межами): " + e.Message);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("Помилка: " + e.Message);
                }
                break;
            case "3":
                Console.Write("ID запису: "); int.TryParse(Console.ReadLine()!, out int cid);
                manager.Cancel(cid);
                break;
            case "4":
                Console.Write("ID запису: "); int.TryParse(Console.ReadLine()!, out int fid);
                manager.Complete(fid);
                break;
            case "0": inMenu = false; break;
        }
    }
}

void RunGrowableDemo()
{
    Console.WriteLine("\nТест GrowablePatientManager:");
    GrowablePatientManager growable = new GrowablePatientManager();
    for (int i = 1; i <= 20; i++)
    {
        Patient testPatient = new Patient("Тест", $"Пацієнт{i}");
        growable.Add(testPatient);
        Console.WriteLine($"  Додано [{testPatient.Id}]. Розмір: {growable.Count} / {growable.Capacity}");
    }
}
void RunMedicalRecordsMenu(Clinic activeClinic)
{
    MedicalRecordManager manager = activeClinic.MedicalRecords;
    bool inMenu = true;
    while (inMenu)
    {
        Console.WriteLine("\nМедична картка:");
        Console.WriteLine("1. Картка пацієнта");
        Console.WriteLine("2. Усі записи пацієнта");
        Console.WriteLine("3. Додати діагноз");
        Console.WriteLine("4. Додати аналіз");
        Console.WriteLine("5. Додати рецепт");
        Console.WriteLine("6. Записи лікаря");
        Console.WriteLine("0. Назад");
        Console.Write("Ваш вибір: ");
        string choice = Console.ReadLine() ?? "";

        switch (choice)
        {
            case "1":
                {
                    if (!TryReadExistingPatientId(activeClinic, out int patientId)) break;
                    manager.DisplayPatientSummary(patientId);
                    break;
                }
            case "2":
                {
                    if (!TryReadExistingPatientId(activeClinic, out int patientId)) break;
                    // Поліморфізм: масив MedicalRecord[] містить різні типи,
                    // а виклики GetRecordType()/GetSummary() дають правильний рядок для кожного
                    MedicalRecord[] records = manager.GetByPatient(patientId);
                    if (records.Length == 0)
                    {
                        Console.WriteLine("Записів не знайдено.");
                        break;
                    }
                    Console.WriteLine($"Записи пацієнта #{patientId}:");
                    foreach (MedicalRecord r in records)
                        Console.WriteLine($"[{r.Id}] {r.GetRecordType()}: {r.GetSummary()}");
                    break;
                }
            case "3":
                {
                    activeClinic.Patients.DisplayAll();
                    activeClinic.Doctors.DisplayAll();
                    if (!TryReadExistingPatientId(activeClinic, out int patientId)) break;
                    if (!TryReadExistingDoctorId(activeClinic, out int doctorId)) break;
                    Console.Write("Код діагнозу (напр. J06.9): "); string code = Console.ReadLine() ?? "";
                    Console.Write("Опис: "); string description = Console.ReadLine() ?? "";
                    if (!TryReadFlag("Хронічне? (1=так, 0=ні): ", out bool isChronic)) break;
                    try
                    {
                        manager.Add(new Diagnosis(patientId, doctorId, DateTime.Today, code, description, isChronic));
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine("Помилка (значення поза межами): " + e.Message);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Помилка: " + e.Message);
                    }
                    break;
                }
            case "4":
                {
                    activeClinic.Patients.DisplayAll();
                    activeClinic.Doctors.DisplayAll();
                    if (!TryReadExistingPatientId(activeClinic, out int patientId)) break;
                    if (!TryReadExistingDoctorId(activeClinic, out int doctorId)) break;
                    Console.Write("Назва аналізу: "); string testName = Console.ReadLine() ?? "";
                    if (!TryReadDouble("Значення (число): ", out double value)) break;
                    Console.Write("Одиниці виміру: "); string unit = Console.ReadLine() ?? "";
                    Console.Write("Норма (напр. 4.0–9.0): "); string range = Console.ReadLine() ?? "";
                    if (!TryReadFlag("В нормі? (1=так, 0=ні): ", out bool isNormal)) break;
                    try
                    {
                        manager.Add(new LabResult(patientId, doctorId, DateTime.Today, testName, value, unit, range, isNormal));
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine("Помилка (значення поза межами): " + e.Message);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Помилка: " + e.Message);
                    }
                    break;
                }
            case "5":
                {
                    activeClinic.Patients.DisplayAll();
                    activeClinic.Doctors.DisplayAll();
                    if (!TryReadExistingPatientId(activeClinic, out int patientId)) break;
                    if (!TryReadExistingDoctorId(activeClinic, out int doctorId)) break;
                    Console.Write("Препарат: "); string medication = Console.ReadLine() ?? "";
                    Console.Write("Дозування (напр. 10 мг): "); string dosage = Console.ReadLine() ?? "";
                    if (!TryReadInt("Кількість днів: ", out int days)) break;
                    Console.Write("Інструкція (Enter — пропустити): "); string instructions = Console.ReadLine() ?? "";
                    try
                    {
                        manager.Add(new Prescription(patientId, doctorId, DateTime.Today, medication, dosage, days, instructions));
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine("Помилка (значення поза межами): " + e.Message);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Помилка: " + e.Message);
                    }
                    break;
                }
            case "6":
                {
                    if (!TryReadExistingDoctorId(activeClinic, out int doctorId)) break;
                    manager.DisplayList(manager.GetByDoctor(doctorId));
                    break;
                }
            case "0": inMenu = false; break;
        }
    }
}

bool TryReadInt(string prompt, out int value)
{
    Console.Write(prompt);
    if (int.TryParse(Console.ReadLine(), out value)) return true;
    Console.WriteLine("Помилка: потрібно ввести ціле число.");
    return false;
}

// NumberStyles.Float не дозволяє роздільник розрядів, тож "6,2" буде відхилено
bool TryReadDouble(string prompt, out double value)
{
    Console.Write(prompt);
    if (double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out value)
        && double.IsFinite(value))
        return true;
    Console.WriteLine("Помилка: потрібно ввести число з крапкою (напр. 6.2).");
    return false;
}

bool TryReadFlag(string prompt, out bool flag)
{
    Console.Write(prompt);
    string input = (Console.ReadLine() ?? "").Trim();
    flag = false;
    if (input == "1") { flag = true; return true; }
    if (input == "0") return true;
    Console.WriteLine("Помилка: введіть 1 або 0.");
    return false;
}

bool TryReadExistingPatientId(Clinic c, out int id)
{
    if (!TryReadInt("ID пацієнта: ", out id)) return false;
    if (c.Patients.FindById(id) == null)
    {
        Console.WriteLine("Пацієнта з таким ID не знайдено.");
        return false;
    }
    return true;
}

bool TryReadExistingDoctorId(Clinic c, out int id)
{
    if (!TryReadInt("ID лікаря: ", out id)) return false;
    if (c.Doctors.FindById(id) == null)
    {
        Console.WriteLine("Лікаря з таким ID не знайдено.");
        return false;
    }
    return true;
}