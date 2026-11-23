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