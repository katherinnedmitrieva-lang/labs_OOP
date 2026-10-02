using System;

namespace ClinicApp;

public class AppointmentManager
{
    private const int MaxAppointments = 200;
    private Appointment[] _appointments = new Appointment[MaxAppointments];
    private int _count = 0;

    private PatientManager _patientManager;
    private DoctorManager _doctorManager;

    public int Count => _count;

    public Appointment this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException("Індекс поза межами масиву записів.");
            }
            return _appointments[index];
        }
    }

    public AppointmentManager(PatientManager patientManager, DoctorManager doctorManager)
    {
        _patientManager = patientManager;
        _doctorManager = doctorManager;
    }

    public bool Book(int patientId, int doctorId, DateTime scheduledAt, int durationMinutes = 30)
    {
        if (_count >= MaxAppointments)
        {
            Console.WriteLine("Не вдалося створити запис: досягнуто ліміту клініки.");
            return false;
        }

        Appointment app = new Appointment(patientId, doctorId, scheduledAt, durationMinutes);
        _appointments[_count] = app;
        _count++;
        Console.WriteLine($"Запис [{app.Id}] успішно створено на {scheduledAt:dd.MM.yyyy HH:mm}.");
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
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].Id == id)
            {
                appointment = _appointments[i];
                return true;
            }
        }
        appointment = null;
        return false;
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
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].ScheduledAt.Year == year &&
                _appointments[i].ScheduledAt.Month == month &&
                _appointments[i].ScheduledAt.Day == day) matchCount++;
        }

        Appointment[] result = new Appointment[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].ScheduledAt.Year == year &&
                _appointments[i].ScheduledAt.Month == month &&
                _appointments[i].ScheduledAt.Day == day)
            {
                result[index] = _appointments[i];
                index++;
            }
        }
        return result;
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
}
