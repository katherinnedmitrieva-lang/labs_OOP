using System;
using System.Globalization;
using System.Threading;
using ClinicApp;
using ClinicApp.Comparators;
using ClinicApp.Enums;
using ClinicApp.Interfaces;
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
clinic.Appointments.BookUrgent(p2.Id, d2.Id, baseDate.AddHours(14), "сильний головний біль");
clinic.Appointments.BookSpecialist(p1.Id, d1.Id, baseDate.AddHours(12), "кардіологія", 60);

// ===== Медичні записи (Лаба 06) =====
DateTime today = DateTime.Today;

clinic.MedicalRecords.Add(new Diagnosis(p1.Id, d1.Id, today.AddDays(-30), "I10", "Гіпертонічна хвороба", true));
clinic.MedicalRecords.Add(new Diagnosis(p1.Id, d1.Id, today.AddDays(-5), "J06.9", "Гострий ринофарингіт"));
clinic.MedicalRecords.Add(new LabResult(p1.Id, d2.Id, today.AddDays(-10), "Гемоглобін", 145, "г/л", "120–160", true));
clinic.MedicalRecords.Add(new LabResult(p1.Id, d2.Id, today.AddDays(-10), "Холестерин", 6.2, "ммоль/л", "< 5.2", false));
clinic.MedicalRecords.Add(new Prescription(p1.Id, d1.Id, today.AddDays(-5), "Лізиноприл", "10 мг", 30, "1 раз на добу вранці"));

clinic.MedicalRecords.Add(new Prescription(p2.Id, d2.Id, today.AddDays(-40), "Ібупрофен", "200 мг", 10, "після їди"));
clinic.MedicalRecords.Add(new Diagnosis(p2.Id, d2.Id, today.AddDays(-3), "G43.9", "Мігрень"));
clinic.MedicalRecords.Add(new LabResult(p3.Id, d3.Id, today.AddDays(-2), "Лейкоцити", 7.5, "10^9/л", "4.0–9.0", true));

// ===== Демонстрація: override проти new (Лаба 08) =====
// Один об'єкт UrgentAppointment, на який посилаються ДВІ змінні різних типів.
UrgentAppointment urgentDemo = new UrgentAppointment(p1.Id, d1.Id, baseDate.AddHours(16), "тест");
Appointment viaBase = urgentDemo;   // та сама адреса в пам'яті, але змінна має тип Appointment

Console.WriteLine("\n== new vs override ==");
Console.WriteLine($"Через Appointment:       {viaBase.GetDescription()} | пріоритет {viaBase.GetPriority()}");
Console.WriteLine($"Через UrgentAppointment: {urgentDemo.GetDescription()} | пріоритет {urgentDemo.GetPriority()}");

// Відповіді на запитання:
//
// 1) Чому GetDescription() через змінну типу Appointment дає опис термінового прийому?
//    Бо GetDescription() у Appointment оголошено як virtual, а в UrgentAppointment він
//    перевизначений через override. Для virtual/override реалізацію обирає CLR під час
//    виконання програми за РЕАЛЬНИМ типом об'єкта (runtime dispatch / пізнє зв'язування).
//    Об'єкт у пам'яті — UrgentAppointment, тому виконується його версія, хоч змінна й типу Appointment.
//
// 2) Чому GetPriority() через ту саму змінну повертає 3, хоча об'єкт — UrgentAppointment?
//    Бо в Appointment метод GetPriority() НЕ virtual, а в UrgentAppointment він лише ПРИХОВУЄ
//    батьківський (модифікатор new) і нічого не перевизначає. Виклик невіртуального методу
//    (зокрема прихованого через new) компілятор прив'язує ще під час компіляції — за типом
//    ЗМІННОЇ. Змінна типу Appointment -> викликається Appointment.GetPriority() -> 3.
//    Змінна типу UrgentAppointment -> викликається UrgentAppointment.GetPriority() -> 1.
//
// 3) Що треба змінити в Appointment і UrgentAppointment, щоб GetPriority() теж працював поліморфно?
//    В Appointment: позначити метод модифікатором virtual -> public virtual int GetPriority().
//    В UrgentAppointment: замінити new на override -> public override int GetPriority().
//    Тоді виклик через змінну типу Appointment теж поверне 1 (за типом об'єкта).

RunMainMenu(clinic);

void RunMainMenu(Clinic activeClinic)
{
    bool running = true;
    while (running)
    {
        Console.WriteLine("\nГОЛОВНЕ МЕНЮ");
        Console.WriteLine("1 — Пацієнти");
        Console.WriteLine("2 — Лікарі");
        Console.WriteLine("3 — Записи");
        Console.WriteLine("4 — Медична картка");
        Console.WriteLine("5 — Рахунки");
        Console.WriteLine("6 — Черга — очікування, прийом");
        Console.WriteLine("7 — Звіт клініки");
        Console.WriteLine("8 — Аналітика");
        Console.WriteLine("9 — Плани лікування — рефлексія, атрибути");
        Console.WriteLine("10 — Файли");
        Console.WriteLine("11 — Тест GrowablePatientManager");
        Console.WriteLine("12 — Розклад на дату");
        Console.WriteLine("0 — Вихід");
        Console.Write("Ваш вибір: ");
        string choice = Console.ReadLine()!;

        switch (choice)
        {
            case "1": RunPatientsMenu(activeClinic.Patients); break;
            case "2": RunDoctorsMenu(activeClinic.Doctors); break;
            case "3": RunAppointmentsMenu(activeClinic.Appointments, activeClinic.Patients, activeClinic.Doctors); break;
            case "4": RunMedicalRecordsMenu(activeClinic); break;
            case "5": RunBillingMenu(activeClinic.Billing); break;
            case "6": WaitingRoomMenu(activeClinic); break;
            case "7": activeClinic.GenerateReport(); break;
            case "8": AnalyticsMenu(activeClinic); break;
            case "9": TreatmentPlanMenu(activeClinic); break;
            case "10": FilesMenu(activeClinic); break;
            case "11": RunGrowableDemo(); break;
            case "12":
                Console.Write("Дата (дд.мм.рррр): ");
                if (DateTime.TryParse(Console.ReadLine()!, out DateTime date))
                    activeClinic.DisplaySchedule(date);
                break;
            case "0": running = false; break;
        }
    }
}
// Меню черги очікування (Лаба 09): працює з clinic.WaitingRoom — це WaitingQueue<Patient>.
void WaitingRoomMenu(Clinic clinic)
{
    bool inMenu = true;
    while (inMenu)
    {
        Console.WriteLine("\nЧерга:");
        Console.WriteLine("1. Додати пацієнта до черги");
        Console.WriteLine("2. Прийняти першого");
        Console.WriteLine("3. Хто перший?");
        Console.WriteLine("4. Переглянути всю чергу");
        Console.WriteLine("0. Назад");
        Console.Write("Ваш вибір: ");
        string choice = Console.ReadLine() ?? "";
        switch (choice)
        {
            case "1":
                {
                    clinic.Patients.DisplayAll();
                    if (!TryReadInt("ID пацієнта: ", out int patientId)) break;
                    Patient? patient = clinic.Patients.FindById(patientId);
                    if (patient == null)
                    {
                        Console.WriteLine("Пацієнта не знайдено.");
                        break;
                    }
                    clinic.WaitingRoom.Enqueue(patient);
                    Console.WriteLine($"{patient.FullName} додано до черги. Позиція в черзі: {clinic.WaitingRoom.Count}.");
                    break;
                }
            case "2":
                {
                    // Dequeue на порожній черзі кидає InvalidOperationException —
                    // порожня черга для користувача нормальна ситуація, тому програма не падає, а пояснює.
                    try
                    {
                        Patient accepted = clinic.WaitingRoom.Dequeue();
                        Console.WriteLine($"Прийнято: {accepted.FullName}. У черзі лишилось: {clinic.WaitingRoom.Count}");
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine("Помилка: " + e.Message);
                        clinic.Logger.LogWarning("Помилка черги: " + ErrorText(e));
                    }
                    break;
                }
            case "3":
                {
                    // Peek лише дивиться на першого, не видаляючи його з черги.
                    try
                    {
                        Patient first = clinic.WaitingRoom.Peek();
                        Console.WriteLine($"Перший у черзі: {first.FullName}");
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine("Помилка: " + e.Message);
                        clinic.Logger.LogWarning("Помилка черги: " + ErrorText(e));
                    }
                    break;
                }
            case "4":
                {
                    Patient[] queued = clinic.WaitingRoom.ToArray();
                    if (queued.Length == 0)
                    {
                        Console.WriteLine("Черга порожня.");
                        break;
                    }
                    Console.WriteLine($"У черзі ({queued.Length}):");
                    for (int i = 0; i < queued.Length; i++)
                        Console.WriteLine($"{i + 1}. {queued[i].FullName}");
                    break;
                }
            case "0": inMenu = false; break;
        }
    }
}
// Меню «Файли» (Лаба 12): журнал, експорт звітів, імпорт CSV.
void FilesMenu(Clinic clinic)
{
    bool inMenu = true;
    while (inMenu)
    {
        Console.WriteLine("\n── Файли ──────────────────────────────");
        Console.WriteLine("  1. Останні рядки лога");
        Console.WriteLine("  2. Очистити лог");
        Console.WriteLine("  0. Назад");
        Console.Write("Оберіть: ");
        string? choice = Console.ReadLine();
        if (choice == null) break;   // кінець введення — виходимо, а не крутимо цикл вічно

        switch (choice.Trim())
        {
            case "1":
                {
                    if (!TryReadInt("Скільки останніх рядків показати? ", out int count)) break;
                    if (count <= 0)
                    {
                        Console.WriteLine("Кількість має бути більшою за 0.");
                        break;
                    }
                    string[] lines = clinic.Logger.GetLastLines(count);
                    if (lines.Length == 0)
                    {
                        Console.WriteLine("Лог порожній або файлу ще немає.");
                        break;
                    }
                    Console.WriteLine($"Останні рядки лога ({lines.Length}):");
                    foreach (string line in lines)
                    {
                        Console.WriteLine(line);
                    }
                    break;
                }
            case "2":
                {
                    if (!clinic.Logger.Exists())
                    {
                        Console.WriteLine("Лог уже порожній: файлу немає.");
                        break;
                    }
                    try
                    {
                        clinic.Logger.Clear();
                        Console.WriteLine("Лог очищено.");
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine("Не вдалося очистити лог: " + e.Message);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine("Немає доступу до файлу лога: " + e.Message);
                    }
                    break;
                }
            case "0": inMenu = false; break;
            default: Console.WriteLine("Невідомий пункт меню."); break;
        }
    }
}

// Меню «Плани лікування» (Лаба 11): показує роботу атрибутів і рефлексії.
void TreatmentPlanMenu(Clinic clinic)
{
    bool inMenu = true;
    while (inMenu)
    {
        Console.WriteLine("\n── Плани лікування ────────────────────");
        Console.WriteLine("  1. Показати всі плани");
        Console.WriteLine("  2. Додати план лікування");
        Console.WriteLine("  3. Плани пацієнта");
        Console.WriteLine("  4. Активувати план");
        Console.WriteLine("  5. Завершити план");
        Console.WriteLine("  6. Скасувати план");
        Console.WriteLine("  7. Інформація про тип TreatmentPlan");
        Console.WriteLine("  0. Назад");
        Console.Write("Оберіть: ");
        string? choice = Console.ReadLine();
        if (choice == null) break;   // кінець введення — виходимо, а не крутимо цикл вічно

        switch (choice.Trim())
        {
            case "1":
                PrintTreatmentPlans(clinic.TreatmentPlans.GetAll(), "Планів лікування ще немає.");
                break;
            case "2":
                {
                    // Форма будується АВТОМАТИЧНО за атрибутами властивостей TreatmentPlan.
                    TreatmentPlan plan = FormBuilder.Build<TreatmentPlan>();
                    // Add сам перевіряє план валідатором: для невалідного виводить помилки й повертає false.
                    if (clinic.TreatmentPlans.Add(plan))
                        Console.WriteLine($"План [{plan.Id}] додано.");
                    else
                        Console.WriteLine("План не додано.");
                    break;
                }
            case "3":
                {
                    if (!TryReadInt("ID пацієнта: ", out int patientId)) break;
                    PrintTreatmentPlans(clinic.TreatmentPlans.GetByPatient(patientId), "Для цього пацієнта планів лікування немає.");
                    break;
                }
            case "4":
                {
                    TreatmentPlan? plan = ReadTreatmentPlan(clinic);
                    if (plan == null) break;
                    if (plan.Activate())
                        Console.WriteLine($"План [{plan.Id}] активовано. Статус: {plan.Status}.");
                    else
                        Console.WriteLine($"Перехід неможливий: активувати можна лише план зі статусом Planned (зараз — {plan.Status}).");
                    break;
                }
            case "5":
                {
                    TreatmentPlan? plan = ReadTreatmentPlan(clinic);
                    if (plan == null) break;
                    if (plan.Complete())
                        Console.WriteLine($"План [{plan.Id}] завершено. Статус: {plan.Status}.");
                    else
                        Console.WriteLine($"Перехід неможливий: завершити можна лише активний план (зараз — {plan.Status}).");
                    break;
                }
            case "6":
                {
                    TreatmentPlan? plan = ReadTreatmentPlan(clinic);
                    if (plan == null) break;
                    if (plan.Cancel())
                        Console.WriteLine($"План [{plan.Id}] скасовано. Статус: {plan.Status}.");
                    else
                        Console.WriteLine($"Перехід неможливий: завершений або вже скасований план скасувати не можна (зараз — {plan.Status}).");
                    break;
                }
            case "7":
                // typeof(...) дає Type без створення об'єкта — рефлексія працює і без екземпляра.
                ModelValidator.PrintInfo(typeof(TreatmentPlan));
                break;
            case "0": inMenu = false; break;
            default: Console.WriteLine("Невідомий пункт меню."); break;
        }
    }
}

// Запитує ID плану й шукає його. Повертає null (з поясненням), якщо ID некоректний або плану немає.
TreatmentPlan? ReadTreatmentPlan(Clinic clinic)
{
    if (!TryReadInt("ID плану: ", out int planId)) return null;
    TreatmentPlan? plan = clinic.TreatmentPlans.GetById(planId);
    if (plan == null)
        Console.WriteLine("План не знайдено.");
    return plan;
}

void PrintTreatmentPlans(TreatmentPlan[] plans, string emptyMessage)
{
    if (plans.Length == 0)
    {
        Console.WriteLine(emptyMessage);
        return;
    }
    foreach (TreatmentPlan plan in plans)
    {
        Console.WriteLine(plan);
    }
}

// Меню «Аналітика» (Лаба 10): п'ять звітів, що показують і природні, і додаткові порядки сортування.
void AnalyticsMenu(Clinic clinic)
{
    bool inMenu = true;
    while (inMenu)
    {
        Console.WriteLine("\nАналітика:");
        Console.WriteLine("1. Лікарі — природний порядок (найбільше прийомів — першим)");
        Console.WriteLine("2. Лікарі — за навантаженням (найменше прийомів — першим)");
        Console.WriteLine("3. Лікарі — за виручкою (найбільша — першою)");
        Console.WriteLine("4. Лікарі — за алфавітом (А → Я)");
        Console.WriteLine("5. Пацієнти — природний порядок і за витратами");
        Console.WriteLine("6. Показати всі звіти");
        Console.WriteLine("0. Назад");
        Console.Write("Ваш вибір: ");
        string choice = Console.ReadLine() ?? "";

        switch (choice)
        {
            case "1": ShowDoctorsNaturalReport(clinic); break;
            case "2": ShowDoctorsByLoadReport(clinic); break;
            case "3": ShowDoctorsByRevenueReport(clinic); break;
            case "4": ShowDoctorsByNameReport(clinic); break;
            case "5": ShowPatientsReport(clinic); break;
            case "6":
                ShowDoctorsNaturalReport(clinic);
                ShowDoctorsByLoadReport(clinic);
                ShowDoctorsByRevenueReport(clinic);
                ShowDoctorsByNameReport(clinic);
                ShowPatientsReport(clinic);
                break;
            case "0": inMenu = false; break;
        }
    }
}

// Звіт 1: ПРИРОДНИЙ порядок. Масив сортується одним викликом Array.Sort() без компаратора —
// він сам бере DoctorStats.CompareTo (IComparable<DoctorStats>).
void ShowDoctorsNaturalReport(Clinic clinic)
{
    DoctorStats[] stats = CollectDoctorStats(clinic).ToArray();
    Array.Sort(stats);
    PrintReport("Лікарі — природний порядок (за кількістю прийомів ↓)", stats);
}

// Звіти 2–4: ДОДАТКОВІ порядки. Окремий клас-компаратор передається в List<T>.Sort(comparer).
void ShowDoctorsByLoadReport(Clinic clinic)
{
    List<DoctorStats> stats = CollectDoctorStats(clinic);
    stats.Sort(new DoctorByLoadComparer());
    PrintReport("Лікарі — за навантаженням (найменше прийомів першим ↑)", stats);
}

void ShowDoctorsByRevenueReport(Clinic clinic)
{
    List<DoctorStats> stats = CollectDoctorStats(clinic);
    stats.Sort(new DoctorByRevenueComparer());
    PrintReport("Лікарі — за виручкою (↓)", stats);
}

void ShowDoctorsByNameReport(Clinic clinic)
{
    List<DoctorStats> stats = CollectDoctorStats(clinic);
    stats.Sort(new DoctorByNameComparer());
    PrintReport("Лікарі — за алфавітом (А → Я)", stats);
}

// Звіт 5: пацієнти — спочатку природний порядок (за візитами), потім додатковий (за витратами).
void ShowPatientsReport(Clinic clinic)
{
    PatientStats[] natural = CollectPatientStats(clinic).ToArray();
    Array.Sort(natural);
    PrintReport("Пацієнти — природний порядок (за кількістю візитів ↓)", natural);

    List<PatientStats> bySpent = CollectPatientStats(clinic);
    bySpent.Sort(new PatientBySpentComparer());
    PrintReport("Пацієнти — за витратами (↓)", bySpent);
}

// Загальний вивід звіту. Порожній набір — не помилка, а нормальна ситуація (ще немає лікарів чи пацієнтів).
void PrintReport<T>(string title, IEnumerable<T> items)
{
    Console.WriteLine($"\n{title}:");
    int number = 0;
    foreach (T item in items)
    {
        number++;
        Console.WriteLine($"{number}. {item}");
    }
    if (number == 0) Console.WriteLine("Даних немає.");
}

// Лінивий ітератор → колекція. ComputeDoctorStats() повертає IEnumerable<T> (yield return) — його НЕ можна
// відсортувати, бо елементи ще не обчислені. foreach «проганяє» ітератор і складає результати в List<DoctorStats>,
// який уже можна сортувати (list.Sort / Array.Sort).
List<DoctorStats> CollectDoctorStats(Clinic clinic)
{
    List<DoctorStats> result = new List<DoctorStats>();
    foreach (DoctorStats stats in clinic.Analytics.ComputeDoctorStats())
    {
        result.Add(stats);
    }
    return result;
}

List<PatientStats> CollectPatientStats(Clinic clinic)
{
    List<PatientStats> result = new List<PatientStats>();
    foreach (PatientStats stats in clinic.Analytics.ComputePatientStats())
    {
        result.Add(stats);
    }
    return result;
}

void RunBillingMenu(BillingManager billingManager)
{
    bool inMenu = true;
    while (inMenu)
    {
        Console.WriteLine("\nРахунки:");
        Console.WriteLine("1. Показати неоплачені рахунки");
        Console.WriteLine("2. Показати загальний борг");
        Console.WriteLine("3. Оплатити запис");
        Console.WriteLine("0. Назад");
        Console.Write("Ваш вибір: ");
        string choice = Console.ReadLine()!;
        switch (choice)
        {
            case "1":
                var unpaid = billingManager.GetUnpaidPayables();
                if (unpaid.Length == 0) Console.WriteLine("Неоплачених рахунків немає.");
                else foreach (var p in unpaid) Console.WriteLine(p);
                break;
            case "2":
                Console.WriteLine($"Загальний борг: {billingManager.GetTotalDebt()} грн");
                break;
            case "3":
                {
                    if (!TryReadInt("ID запису: ", out int payId)) break;
                    Appointment? toPay = clinic.Appointments.FindById(payId);
                    if (toPay == null)
                    {
                        Console.WriteLine("Запис не знайдено.");
                        break;
                    }
                    // MarkPaid() на скасованому записі мовчки нічого не робить, тому перевіряємо стан заздалегідь.
                    if (toPay.IsCancelled)
                    {
                        Console.WriteLine("Скасований запис оплатити не можна.");
                        break;
                    }
                    if (toPay.IsPaid)
                    {
                        Console.WriteLine("Цей запис уже оплачено.");
                        break;
                    }
                    toPay.MarkPaid();
                    Console.WriteLine($"Запис [{toPay.Id}] оплачено: {toPay.Cost:F2} грн.");
                    clinic.Logger.LogInfo($"Оплачено запис #{toPay.Id}: {toPay.Cost:F2} грн");
                    break;
                }
            case "0": inMenu = false; break;
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
                    Patient newPatient = new Patient(fn, ln, dob, bType, ph);
                    manager.Add(newPatient);
                    clinic.Logger.LogInfo($"Додано пацієнта #{newPatient.Id}: {newPatient.FullName}");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine("Помилка (значення поза межами): " + e.Message);
                    clinic.Logger.LogWarning("Помилка введення: " + ErrorText(e));
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("Помилка: " + e.Message);
                    clinic.Logger.LogWarning("Помилка введення: " + ErrorText(e));
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
        Console.WriteLine("5. Вільні години лікаря");
        Console.WriteLine("6. Статистика");
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

                    WorkSchedule schedule = new WorkSchedule(startH, endH);
                    Doctor newDoc = new Doctor(fn, ln, spec, lic, ph);
                    newDoc.Schedule = schedule;
                    manager.Add(newDoc);
                    clinic.Logger.LogInfo($"Додано лікаря #{newDoc.Id}: {newDoc.FullName}");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine("Помилка (значення поза межами): " + e.Message);
                    clinic.Logger.LogWarning("Помилка введення: " + ErrorText(e));
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("Помилка: " + e.Message);
                    clinic.Logger.LogWarning("Помилка введення: " + ErrorText(e));
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
            case "5": // Вимоги завд. 4: використання інтерфейсу ISchedulable
                manager.DisplayAll();
                Console.Write("Введіть ID лікаря: ");
                if (int.TryParse(Console.ReadLine()!, out int docId))
                {
                    Doctor? doc = manager.FindById(docId);
                    if (doc != null)
                    {
                        ISchedulable schedulableDoc = doc; // Використання змінної інтерфейсного типу
                        Console.Write("Дата (дд.мм.рррр): ");
                        if (DateTime.TryParse(Console.ReadLine()!, out DateTime slotDate))
                        {
                            Console.Write("Кількість слотів: ");
                            if (int.TryParse(Console.ReadLine()!, out int count))
                            {
                                try
                                {
                                    DateTime[] slots = schedulableDoc.GetAvailableSlots(slotDate, count);
                                    Console.WriteLine("Доступні слоти:");
                                    foreach (var slot in slots)
                                    {
                                        Console.WriteLine(slot.ToString("dd.MM.yyyy HH:mm"));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Помилка: " + ex.Message);
                                    clinic.Logger.LogWarning("Помилка введення: " + ErrorText(ex));
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Лікаря не знайдено.");
                    }
                }
                break;
            case "6": manager.DisplayStats(); break;
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
        Console.WriteLine("8. Скасувати всі записи пацієнта");
        Console.WriteLine("9. За типом прийому");
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
                    if (manager.Book(pid, did, sched, dur))
                    {
                        // Щойно створений запис — останній у менеджері; з нього беремо ID для журналу.
                        Appointment? created = manager[manager.Count - 1];
                        if (created != null)
                            clinic.Logger.LogInfo($"Запис #{created.Id} створено: пацієнт #{pid} → лікар #{did}");
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.WriteLine("Помилка (значення поза межами): " + e.Message);
                    clinic.Logger.LogWarning("Помилка введення: " + ErrorText(e));
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine("Помилка: " + e.Message);
                    clinic.Logger.LogWarning("Помилка введення: " + ErrorText(e));
                }
                break;
            case "3":
                Console.Write("ID запису: "); int.TryParse(Console.ReadLine()!, out int cid);
                if (manager.Cancel(cid))
                {
                    Console.WriteLine($"Запис [{cid}] скасовано.");
                    clinic.Logger.LogInfo($"Запис #{cid} скасовано");
                }
                else
                {
                    Console.WriteLine("Не вдалося скасувати: запис не знайдено або він уже не активний.");
                }
                break;
            case "4":
                Console.Write("ID запису: "); int.TryParse(Console.ReadLine()!, out int fid);
                if (manager.Complete(fid))
                {
                    Console.WriteLine($"Запис [{fid}] завершено.");
                    clinic.Logger.LogInfo($"Запис #{fid} завершено");
                }
                else
                {
                    Console.WriteLine("Не вдалося завершити: запис не знайдено або він уже не активний.");
                }
                break;
            case "8": // Вимоги завд. 4: масове скасування через AppointmentManager.CancelAll
                patients.DisplayAll();
                Console.Write("Введіть ID пацієнта, записи якого треба скасувати: ");
                if (int.TryParse(Console.ReadLine()!, out int patientId))
                {
                    Console.Write("Причина скасування: ");
                    string reason = Console.ReadLine() ?? "";

                    var allAppointments = manager.GetAll();
                    // Захист від null-елементів у масиві за допомогою Where(a => a != null)
                    var patientItems = allAppointments
                        .Where(a => a != null && a.PatientId == patientId)
                        .Cast<ICancellable>()
                        .ToArray();

                    int cancelledCount = AppointmentManager.CancelAll(patientItems, reason);
                    Console.WriteLine($"Успішно скасовано записів: {cancelledCount}");
                    if (cancelledCount > 0)
                        clinic.Logger.LogInfo($"Скасовано записів пацієнта #{patientId}: {cancelledCount}");
                }
                break;
            case "9": // Перегляд записів за типом (GetUrgent / GetSpecialist / GetRegular)
                Console.WriteLine("1. Термінові");
                Console.WriteLine("2. Консультації спеціаліста");
                Console.WriteLine("3. Звичайні");
                Console.Write("Оберіть: ");
                string typeChoice = Console.ReadLine() ?? "";
                switch (typeChoice)
                {
                    case "1": manager.DisplayList(manager.GetUrgent()); break;
                    case "2": manager.DisplayList(manager.GetSpecialist()); break;
                    case "3": manager.DisplayList(manager.GetRegular()); break;
                    default: Console.WriteLine("Невідомий варіант."); break;
                }
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
                        clinic.Logger.LogWarning("Помилка введення: " + ErrorText(e));
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Помилка: " + e.Message);
                        clinic.Logger.LogWarning("Помилка введення: " + ErrorText(e));
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
                        clinic.Logger.LogWarning("Помилка введення: " + ErrorText(e));
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Помилка: " + e.Message);
                        clinic.Logger.LogWarning("Помилка введення: " + ErrorText(e));
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
                        clinic.Logger.LogWarning("Помилка введення: " + ErrorText(e));
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine("Помилка: " + e.Message);
                        clinic.Logger.LogWarning("Помилка введення: " + ErrorText(e));
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

// Текст помилки для журналу: .NET дописує до ArgumentException технічний суфікс « (Parameter 'phone')» —
// у журналі він зайвий, тому відрізаємо його.
string ErrorText(Exception e)
{
    string message = e.Message;
    int index = message.IndexOf(" (Parameter '");
    if (index >= 0) message = message.Substring(0, index);
    return message;
}

bool TryReadInt(string prompt, out int value)
{
    Console.Write(prompt);
    if (int.TryParse(Console.ReadLine(), out value)) return true;
    Console.WriteLine("Помилка: потрібно ввести ціле число.");
    return false;
}

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