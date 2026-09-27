using ClinicApp;
Console.OutputEncoding = System.Text.Encoding.UTF8;
PatientManager patientManager = new PatientManager();
Patient p1 = new Patient("Іван", "Петренко", new DateTime(1985, 3, 12), "A+", "0501234567");
Patient p2 = new Patient("Олена", "Коваль", new DateTime(1993, 7, 4), "B-", "0672345678");
Patient p3 = new Patient("Максим", "Бойко", new DateTime(2010, 1, 20), "O+", "0933456789");
Patient p4 = new Patient();
Patient p5 = new Patient("Марія", "Ткач");

patientManager.Add(p1);
patientManager.Add(p2);
patientManager.Add(p3);
patientManager.Add(p4);
patientManager.Add(p5);

DoctorManager doctorManager = new DoctorManager();

Doctor d1 = new Doctor("Олег", "Сидоренко", "Кардіологія", "LIC-001", "0441234567");
d1.WorkStartHour = 8;
d1.WorkEndHour = 16;
Doctor d2 = new Doctor("Наталія", "Мороз", "Неврологія", "LIC-002", "0442345678");
d2.WorkStartHour = 9;
d2.WorkEndHour = 18;
Doctor d3 = new Doctor("Андрій", "Власенко", "Педіатрія", "LIC-003", "0443456789");

doctorManager.Add(d1);
doctorManager.Add(d2);
doctorManager.Add(d3);

RunPatientsMenu(patientManager);

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
            case "1":
                manager.DisplayAll();
                break;
            case "2":
                Console.Write("Ім'я: ");
                string firstName = Console.ReadLine()!;
                Console.Write("Прізвище: ");
                string lastName = Console.ReadLine()!;
                Console.Write("Дата народження (дд.мм.рррр): ");
                DateTime.TryParse(Console.ReadLine()!, out DateTime dob);
                Console.Write("Група крові: ");
                string bloodType = Console.ReadLine()!;
                Console.Write("Телефон: ");
                string phone = Console.ReadLine()!;
                manager.Add(new Patient(firstName, lastName, dob, bloodType, phone));
                break;
            case "3":
                Console.Write("Частина імені: ");
                Patient[] found = manager.FindByName(Console.ReadLine()!);
                if (found.Length == 0) Console.WriteLine("Нікого не знайдено.");
                else foreach (Patient p in found) Console.WriteLine(p);
                break;
            case "4":
                Console.Write("ID: ");
                int.TryParse(Console.ReadLine()!, out int removeId);
                Console.WriteLine(manager.Remove(removeId) ? "Видалено." : "Не знайдено.");
                break;
            case "5":
                manager.DisplayStats();
                break;
            case "0":
                inMenu = false;
                break;
        }
    }
}
RunDoctorsMenu(doctorManager);

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
            case "1":
                manager.DisplayAll();
                break;
            case "2":
                Console.Write("Ім'я: ");
                string firstName = Console.ReadLine()!;
                Console.Write("Прізвище: ");
                string lastName = Console.ReadLine()!;
                Console.Write("Спеціальність: ");
                string speciality = Console.ReadLine()!;
                Console.Write("Ліцензія: ");
                string license = Console.ReadLine()!;
                Console.Write("Телефон: ");
                string phone = Console.ReadLine()!;
                manager.Add(new Doctor(firstName, lastName, speciality, license, phone));
                break;
            case "3":
                Console.Write("Спеціальність: ");
                Doctor[] found = manager.FindBySpeciality(Console.ReadLine()!);
                if (found.Length == 0) Console.WriteLine("Нікого не знайдено.");
                else foreach (Doctor d in found) Console.WriteLine(d);
                break;
            case "4":
                Console.Write("ID: ");
                int.TryParse(Console.ReadLine()!, out int removeId);
                Console.WriteLine(manager.Remove(removeId) ? "Видалено." : "Не знайдено.");
                break;
            case "5":
                manager.DisplayStats();
                break;
            case "0":
                inMenu = false;
                break;
        }
        Console.WriteLine("\nТест Appointment (кінцевий автомат):");
        Appointment testAppt = new Appointment(p1.Id, d1.Id, DateTime.Today.AddDays(1).AddHours(10));
        Console.WriteLine(testAppt);
        Console.WriteLine("Cancel(): " + testAppt.Cancel("Пацієнт не зміг прийти"));
        Console.WriteLine(testAppt);
        Console.WriteLine("Complete() після Cancel (має бути false): " + testAppt.Complete());
    }
}