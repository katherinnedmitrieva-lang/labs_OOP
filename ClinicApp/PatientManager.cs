using System;

namespace ClinicApp;

public class PatientManager
{
    private const int MaxPatients = 100;
    private Patient[] _patients = new Patient[MaxPatients];
    private int _count = 0;

    public int Count => _count;

    public Patient this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException("Індекс поза межами масиву пацієнтів.");
            }
            return _patients[index];
        }
    }

    public void Add(Patient patient)
    {
        if (_count >= MaxPatients)
        {
            Console.WriteLine($"Не вдалося додати пацієнта: досягнуто ліміту ({MaxPatients}).");
            return;
        }
        _patients[_count] = patient;
        _count++;
        Console.WriteLine($"Пацієнта [{patient.Id}] {patient.FullName} додано.");
    }

    public Patient? FindById(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].Id == id) return _patients[i];
        }
        return null;
    }

    public bool TryFindById(int id, out Patient? patient)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].Id == id)
            {
                patient = _patients[i];
                return true;
            }
        }
        patient = null;
        return false;
    }

    public Patient[] FindByName(string name)
    {
        string search = name.ToLower();
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].FullName.ToLower().Contains(search)) matchCount++;
        }

        Patient[] result = new Patient[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].FullName.ToLower().Contains(search))
            {
                result[index] = _patients[i];
                index++;
            }
        }
        return result;
    }

    public Patient[] FindByBloodType(BloodType bloodType)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].BloodType == bloodType) matchCount++;
        }

        Patient[] result = new Patient[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].BloodType == bloodType)
            {
                result[index] = _patients[i];
                index++;
            }
        }
        return result;
    }

    public Patient[] GetAll()
    {
        Patient[] copy = new Patient[_count];
        Array.Copy(_patients, copy, _count);
        return copy;
    }

    public bool Remove(int id)
    {
        int indexToRemove = -1;
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].Id == id) { indexToRemove = i; break; }
        }
        if (indexToRemove == -1) return false;

        for (int i = indexToRemove; i < _count - 1; i++)
        {
            _patients[i] = _patients[i + 1];
        }
        _patients[_count - 1] = null!;
        _count--;
        return true;
    }

    public void DisplayAll()
    {
        Console.WriteLine($"Пацієнти ({_count} / {MaxPatients}):");
        if (_count == 0)
        {
            Console.WriteLine("Список порожній.");
            return;
        }
        for (int i = 0; i < _count; i++) Console.WriteLine(_patients[i]);
        Console.WriteLine(new string('─', 62));
    }

    public void DisplayStats()
    {
        Console.WriteLine("Статистика пацієнтів:");
        if (_count == 0)
        {
            Console.WriteLine("Список порожній.");
            Console.WriteLine(new string('=', 26));
            return;
        }

        int adults = 0;
        int children = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].IsAdult) adults++;
            else children++;
        }

        Console.WriteLine($"Всього:    {_count}");
        Console.WriteLine($"Дорослих:  {adults}");
        Console.WriteLine($"Дітей:     {children}");
        Console.WriteLine(new string('=', 26));
    }
}
