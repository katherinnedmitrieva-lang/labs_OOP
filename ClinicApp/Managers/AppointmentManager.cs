using ClinicApp.Interfaces;
using ClinicApp.Models;
using System;

namespace ClinicApp.Managers;


public class AppointmentManager
{
    private const int MaxAppointments = 200;
    private Appointment[] _appointments = new Appointment[MaxAppointments];
    private int _count = 0;

    private PatientManager _patientManager;
    private DoctorManager _doctorManager;

    public int Count => _count;

    public Appointment? this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) return null;
            return _appointments[index];
        }
    }

    public AppointmentManager(PatientManager patientManager, DoctorManager doctorManager)
    {
        _patientManager = patientManager;
        _doctorManager = doctorManager;
    }

    // Звичайний прийом. Три методи Book* відрізняються лише тим, ЯКИЙ об'єкт створюється,
    // а спільна частина (перевірка ліміту й додавання в масив) винесена в AddAppointment.
    public bool Book(int patientId, int doctorId, DateTime scheduledAt, int durationMinutes = 30)
    {
        return AddAppointment(new RegularAppointment(patientId, doctorId, scheduledAt, durationMinutes));
    }

    // Терміновий прийом
    public bool BookUrgent(int patientId, int doctorId, DateTime scheduledAt, string urgencyNote = "", int durationMinutes = 30)
    {
        return AddAppointment(new UrgentAppointment(patientId, doctorId, scheduledAt, urgencyNote, durationMinutes));
    }

    // Консультація спеціаліста
    public bool BookSpecialist(int patientId, int doctorId, DateTime scheduledAt, string topic = "", int durationMinutes = 45)
    {
        return AddAppointment(new SpecialistAppointment(patientId, doctorId, scheduledAt, topic, durationMinutes));
    }

    // Приймає вже створений запис будь-якого типу (параметр має базовий тип Appointment).
    private bool AddAppointment(Appointment app)
    {
        if (_count >= MaxAppointments)
        {
            Console.WriteLine("Не вдалося створити запис: досягнуто ліміту клініки.");
            return false;
        }

        _appointments[_count] = app;
        _count++;
        Console.WriteLine($"Запис [{app.Id}] успішно створено на {app.ScheduledAt:dd.MM.yyyy HH:mm}.");
        return true;
    }

    public Appointment? FindById(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].Id == id) return _appointments[i];
        }
        return null;
    }

    public bool TryFindById(int id, out Appointment? appointment)
    {
        appointment = FindById(id);
        return appointment != null;
    }

    public Appointment[] GetUpcoming()
    {
        int upcomingCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].IsUpcoming) upcomingCount++;
        }

        Appointment[] result = new Appointment[upcomingCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].IsUpcoming)
            {
                result[index] = _appointments[i];
                index++;
            }
        }
        return result;
    }

    public Appointment[] GetByDate(DateTime date)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].ScheduledAt.Date == date.Date) matchCount++;
        }

        Appointment[] result = new Appointment[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].ScheduledAt.Date == date.Date)
            {
                result[index] = _appointments[i];
                index++;
            }
        }
        return result;
    }

    public Appointment[] GetByDate(int year, int month, int day)
    {
        return GetByDate(new DateTime(year, month, day));
    }

    public bool Cancel(int id, string reason = "")
    {
        Appointment? app = FindById(id);
        if (app == null) return false;
        return app.Cancel(reason);
    }

    public bool Complete(int id)
    {
        Appointment? app = FindById(id);
        if (app == null) return false;
        return app.Complete();
    }

    public void DisplayList(Appointment[] list)
    {
        if (list.Length == 0)
        {
            Console.WriteLine("Записів не знайдено.");
            return;
        }
        foreach (Appointment app in list)
        {
            Console.WriteLine(app);
        }
    }
    public Appointment[] GetAll()
    {
        return (Appointment[])_appointments.Clone();
    }
    public static int CancelAll(ICancellable[] items, string reason = "")
    {
        int count = 0;
        foreach (var item in items)
        {
            if (!item.IsCancelled && item.Cancel(reason))
            {
                count++;
            }
        }
        return count;
    }
}
