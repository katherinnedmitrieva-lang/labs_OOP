using ClinicApp;
Console.OutputEncoding = System.Text.Encoding.UTF8;

Patient p1 = new Patient("Іван", "Петренко", new DateTime(1985, 3, 12), "A+", "0501234567");
Patient p2 = new Patient("Олена", "Коваль", new DateTime(1993, 7, 4), "B-", "0672345678");
Patient p3 = new Patient("Максим", "Бойко", new DateTime(2010, 1, 20), "O+", "0933456789");
Patient p4 = new Patient();
Patient p5 = new Patient("Марія", "Ткач");

Console.WriteLine(p1);
Console.WriteLine(p2);
Console.WriteLine(p3);
Console.WriteLine(p4);
Console.WriteLine(p5);

Doctor d1 = new Doctor("Олег", "Сидоренко", "Кардіологія", "LIC-001", "0441234567");
d1.WorkStartHour = 8;
d1.WorkEndHour = 16;
Doctor d2 = new Doctor("Наталія", "Мороз", "Неврологія", "LIC-002", "0442345678");
d2.WorkStartHour = 9;
d2.WorkEndHour = 18;
Doctor d3 = new Doctor("Андрій", "Власенко", "Педіатрія", "LIC-003", "0443456789");

Console.WriteLine();
Console.WriteLine(d1);
Console.WriteLine(d2);
Console.WriteLine(d3);