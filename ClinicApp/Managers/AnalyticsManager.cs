using System;
using System.Collections.Generic;
using ClinicApp.Models;

namespace ClinicApp.Managers;

// Збирає аналітику по лікарях і пацієнтах. Джерела даних (менеджери) отримує ЗЗОВНІ через конструктор
// (dependency injection), а не створює сам: клас не знає, звідки менеджери взялися, і його легко тестувати.
public class AnalyticsManager
{
    private readonly AppointmentManager _appointments;
    private readonly DoctorManager _doctors;
    private readonly PatientManager _patients;

    public AnalyticsManager(AppointmentManager appointments, DoctorManager doctors, PatientManager patients)
    {
        // Перевірка тут, у конструкторі: у ітераторах (yield) тіло методу виконується відкладено,
        // тому помилка в аргументах інакше спливла б лише під час перебору, далеко від справжньої причини.
        _appointments = appointments ?? throw new ArgumentNullException(nameof(appointments));
        _doctors = doctors ?? throw new ArgumentNullException(nameof(doctors));
        _patients = patients ?? throw new ArgumentNullException(nameof(patients));
    }

    // ЛІНИВЕ обчислення: метод-ітератор (yield return) нічого не рахує, доки хтось не почне перебір.
    // Кожен DoctorStats рахується в момент, коли foreach просить наступний елемент, і ОДРАЗУ віддається —
    // весь масив не накопичується. Якщо перервати перебір (break / Take(10)), решта лікарів
    // не обчислюватиметься взагалі.
    public IEnumerable<DoctorStats> ComputeDoctorStats()
    {
        // GetAll() повертає масив на 200 комірок, у якому незайняті — null, тому нижче є перевірка на null.
        Appointment[] allAppointments = _appointments.GetAll();

        foreach (Doctor doctor in _doctors.GetAll())
        {
            int count = 0;
            decimal revenue = 0m;
            DateTime last = default(DateTime);   // 01.01.0001 = «записів ще не було»

            foreach (Appointment? appointment in allAppointments)
            {
                if (appointment == null || appointment.DoctorId != doctor.Id) continue;

                count++;
                revenue += appointment.GetCost();   // віртуальний метод: кожен тип прийому дає свою ціну
                if (appointment.ScheduledAt > last) last = appointment.ScheduledAt;
            }

            // Лікар без записів теж потрапляє у звіт: count = 0, revenue = 0, дата = default.
            yield return new DoctorStats(doctor.Id, doctor.FullName, count, revenue, last);
        }
    }

    // Те саме для пацієнтів: візити, витрачена сума, дата останнього візиту.
    public IEnumerable<PatientStats> ComputePatientStats()
    {
        Appointment[] allAppointments = _appointments.GetAll();

        foreach (Patient patient in _patients.GetAll())
        {
            int visits = 0;
            decimal spent = 0m;
            DateTime last = default(DateTime);

            foreach (Appointment? appointment in allAppointments)
            {
                if (appointment == null || appointment.PatientId != patient.Id) continue;

                visits++;
                spent += appointment.GetCost();
                if (appointment.ScheduledAt > last) last = appointment.ScheduledAt;
            }

            yield return new PatientStats(patient.Id, patient.FullName, visits, spent, last);
        }
    }
}
