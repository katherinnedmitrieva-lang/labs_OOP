using System;
using ClinicApp.Managers;
using ClinicApp.Models;
using ClinicApp.Utils;
namespace ClinicApp;

public class Clinic
{
    public string Name { get; }
    public PatientManager Patients { get; }
    public DoctorManager Doctors { get; }
    public AppointmentManager Appointments { get; }
    public MedicalRecordManager MedicalRecords { get; }
    public BillingManager Billing { get; }

    // Черга очікування пацієнтів (Лаба 09): generic WaitingQueue<T>, інстанційований типом Patient.
    public WaitingQueue<Patient> WaitingRoom { get; }

    // Аналітика (Лаба 10): статистика по лікарях і пацієнтах, джерела даних — менеджери цієї ж клініки.
    public AnalyticsManager Analytics { get; }

    // Плани лікування (Лаба 11): зберігаються лише після перевірки ModelValidator.
    public TreatmentPlanManager TreatmentPlans { get; }

    // Журнал дій (Лаба 12): рядки з часовою міткою дописуються у файл clinic.log.
    public ClinicLogger Logger { get; }

    // Експорт звітів у текстові файли reports/yyyy-MM-dd/ (Лаба 12).
    public ClinicExporter Exporter { get; }

    // Збереження пацієнтів між запусками у файл session.dat (Лаба 12).
    public SessionManager Session { get; }

    // Генерація паспорта пацієнта (Лаба 13): другий незалежний підписник подій.
    public PatientPassportWriter Passport { get; }

    public Clinic(string name)
    {
        Name = name;
        Patients = new PatientManager();
        Doctors = new DoctorManager();
        Appointments = new AppointmentManager(Patients, Doctors);
        MedicalRecords = new MedicalRecordManager();
        Billing = new BillingManager(Appointments);
        WaitingRoom = new WaitingQueue<Patient>();
        // Створюється ПІСЛЯ Patients, Doctors і Appointments: конструктор отримує вже готові менеджери.
        Analytics = new AnalyticsManager(Appointments, Doctors, Patients);
        TreatmentPlans = new TreatmentPlanManager();
        Logger = new ClinicLogger();
        Exporter = new ClinicExporter(this);
        Session = new SessionManager();
        Passport = new PatientPassportWriter(this);

        // В КІНЦІ конструктора: підписувати можна лише тоді, коли вже створено і менеджери (джерела подій),
        // і Logger (підписник).
        SubscribeEvents();
    }

    // Clinic — «оркестратор»: він знає всі менеджери й вирішує, хто на що підписаний. Менеджери нічого не знають
    // про підписників, а Program.cs не мусить пам'ятати викликати Logger після кожної дії.
    // Підписка — це += з посиланням на метод (без дужок).
    private void SubscribeEvents()
    {
        Patients.PatientAdded += Logger.OnPatientAdded;
        Appointments.AppointmentBooked += Logger.OnAppointmentBooked;
        Appointments.UrgentAppointmentBooked += Logger.OnUrgentAppointmentBooked;
        Appointments.AppointmentCancelled += Logger.OnAppointmentCancelled;
        Appointments.AppointmentCompleted += Logger.OnAppointmentCompleted;
        Billing.PaymentReceived += Logger.OnPaymentReceived;
        TreatmentPlans.PlanCompleted += Logger.OnPlanCompleted;

        // Паспорт пацієнта оновлюється при реєстрації, після завершеного прийому і після завершеного плану.
        Patients.PatientAdded += Passport.OnPatientAdded;
        Appointments.AppointmentCompleted += Passport.OnAppointmentCompleted;
        TreatmentPlans.PlanCompleted += Passport.OnPlanCompleted;
    }

    public void DisplaySchedule(DateTime date)
    {
        Console.WriteLine($"Розклад на {date:dd.MM.yyyy} :");
        Appointments.DisplayList(Appointments.GetByDate(date));
    }

    public void GenerateReport()
    {
        Appointment[] upcoming = Appointments.GetUpcoming();
        Doctor[] allDoctors = Doctors.GetAll();

        Console.WriteLine($"Звіт — {Name}");
        Console.WriteLine($"Пацієнтів:          {Patients.Count}");
        Console.WriteLine($"Лікарів:            {Doctors.Count}");
        Console.WriteLine($"Майбутніх записів:  {upcoming.Length}");
        Console.WriteLine("Навантаження лікарів (майбутні записи):");

        for (int i = 0; i < allDoctors.Length; i++)
        {
            int loadCount = 0;
            for (int j = 0; j < upcoming.Length; j++)
            {
                if (upcoming[j].DoctorId == allDoctors[i].Id) loadCount++;
            }
            string specName = ClinicFormatter.FormatSpeciality(allDoctors[i].Speciality);
            Console.WriteLine($"    {allDoctors[i].FullName} ({specName}): {loadCount} записів");
        }
    }
}