using System;
using ClinicApp;

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
Doctor d2 = new Doctor("Наталія", "Мороз", Speciality.Neurologist, "LIC-002", "0442345678");
d2.Schedule = new WorkSchedule(9, 18);
Doctor d3 = new Doctor("Андрій", "Власенко", Speciality.Pediatrician, "LIC-003", "0443456789");
clinic.Doctors.Add(d1);
clinic.Doctors.Add(d2);
clinic.Doctors.Add(d3);

DateTime baseDate = DateTime.Today.AddDays(1);
clinic.Appointments.Book(p1.Id, d1.Id, baseDate.AddHours(10), 30);
clinic.Appointments.Book(p2.Id, d2.Id, baseDate.AddHours(11), 45);
clinic.Appointments.Book(p3.Id, d3.Id, baseDate.AddDays(1).AddHours(9), 20);
Doctor testIndexer = clinic.Doctors[0];
Console.WriteLine("\nТЕСТ ЗАВДАННЯ 4 (Оператори ?. та ??)");

Doctor? sampleDoctor;
bool isFound = clinic.Doctors.TryFindById(999, out sampleDoctor);

string? docName = sampleDoctor?.FullName;
string outputString = docName ?? "Повідомлення: Лікаря з ID 999 не знайдено в базі даних.";

Console.WriteLine($"Статус пошуку лікаря: {isFound}");
Console.WriteLine($"Виведення на екран: {outputString}");
Console.WriteLine(new string('═', 45));

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
                Console.Write("Ім'я: "); string fn = Console.ReadLine()!;
                Console.Write("Прізвище: "); string ln = Console.ReadLine()!;
                Console.Write("Дата народження: "); DateTime.TryParse(Console.ReadLine()!, out DateTime dob);

                Console.Write("Група крові (напр. APositive, BNegative, Unknown): ");
                if (!Enum.TryParse(Console.ReadLine()!, true, out BloodType bType))
                {
                    bType = BloodType.Unknown;
                }

                Console.Write("Телефон: "); string ph = Console.ReadLine()!;
                manager.Add(new Patient(fn, ln, dob, bType, ph));
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
                Console.Write("Ім'я: "); string fn = Console.ReadLine()!;
                Console.Write("Прізвище: "); string ln = Console.ReadLine()!;

                Console.Write("Спеціальність (напр. Therapist, Cardiologist, Pediatrician): ");
                if (!Enum.TryParse(Console.ReadLine()!, true, out Speciality spec))
                {
                    spec = Speciality.Therapist;
                }

                Console.Write("Ліцензія: "); string lic = Console.ReadLine()!;
                Console.Write("Телефон: "); string ph = Console.ReadLine()!;

                Console.Write("Година початку роботи: "); int.TryParse(Console.ReadLine()!, out int startH);
                Console.Write("Година кінця роботи: "); int.TryParse(Console.ReadLine()!, out int endH);

                Doctor newDoc = new Doctor(fn, ln, spec, lic, ph);
                newDoc.Schedule = new WorkSchedule(startH, endH);
                manager.Add(newDoc);
                break;
            case "3":
                Console.Write("Спеціальність (напр. Therapist, Cardiologist): ");
                string searchInput = Console.ReadLine()!;

                if (Enum.TryParse(searchInput, true, out Speciality searchSpec))
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
        Console.Write("Ваш viбір: ");
        string choice = Console.ReadLine()!;
        switch (choice)
        {
            case "1": manager.DisplayList(manager.GetUpcoming()); break;
            case "2":
                patients.DisplayAll(); doctors.DisplayAll();
                Console.Write("ID пацієнта: "); int.TryParse(Console.ReadLine()!, out int pid);
                Console.Write("ID лікаря: "); int.TryParse(Console.ReadLine()!, out int did);
                Console.Write("Дата та час: "); DateTime.TryParse(Console.ReadLine()!, out DateTime sched);
                manager.Book(pid, did, sched);
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
