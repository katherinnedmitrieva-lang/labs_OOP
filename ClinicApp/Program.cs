using System;
using ClinicApp;
using ClinicApp.Enums;
using ClinicApp.Managers;
using ClinicApp.Models;
using ClinicApp.Utils;

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
// === TEMP START (видалити в Задачі 4) ===
MedicalRecordManager tmpM = new MedicalRecordManager();
tmpM.Add(new Diagnosis(1, 1, DateTime.Today.AddDays(-30), "I10", "Гіпертонічна хвороба", true));
tmpM.Add(new LabResult(1, 1, DateTime.Today, "Холестерин", 6.2, "ммоль/л", "< 5.2", false));
tmpM.Add(new Prescription(1, 1, DateTime.Today.AddDays(-5), "Лізиноприл", "10 мг", 30, "вранці"));
tmpM.Add(new Prescription(2, 2, DateTime.Today.AddDays(-40), "Ібупрофен", "200 мг", 10));
tmpM.DisplayAll();
Console.WriteLine(tmpM[2]!.IsActive());   // True
Console.WriteLine(tmpM[3]!.IsActive());   // False
try { new Prescription(1, 1, DateTime.Today, "Аспірин", "500 мг", 0); }
catch (ArgumentOutOfRangeException e) { Console.WriteLine("OK: " + e.Message); }
tmpM.DisplayPatientSummary(1);
tmpM.DisplayPatientSummary(2);   // немає хронічних і активних: порожніх розділів бути не повинно
tmpM.DisplayPatientSummary(99);  // коротке повідомлення
try { Diagnosis bad = (Diagnosis)tmpM[2]!; }          // [2] це рецепт
catch (InvalidCastException e) { Console.WriteLine("InvalidCastException: " + e.Message); }
// === TEMP END ===
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
        Console.WriteLine("4. Розклад на дату");
        Console.WriteLine("5. Звіт клініки");
        Console.WriteLine("6. Тест GrowablePatientManager");
        Console.WriteLine("0. Вихід");
        Console.Write("Ваш вибір: ");
        string choice = Console.ReadLine()!;

        switch (choice)
        {
            case "1": RunPatientsMenu(activeClinic.Patients); break;
            case "2": RunDoctorsMenu(activeClinic.Doctors); break;
            case "3": RunAppointmentsMenu(activeClinic.Appointments, activeClinic.Patients, activeClinic.Doctors); break;
            case "4":
                Console.Write("Дата (дд.мм.рррр): ");
                if (DateTime.TryParse(Console.ReadLine()!, out DateTime date))
                    activeClinic.DisplaySchedule(date);
                break;
            case "5": activeClinic.GenerateReport(); break;
            case "6": RunGrowableDemo(); break;
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