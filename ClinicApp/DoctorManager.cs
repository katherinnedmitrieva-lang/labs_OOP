using System;

namespace ClinicApp;

public class DoctorManager
{
    private const int MaxDoctors = 50;
    private Doctor[] _doctors = new Doctor[MaxDoctors];
    private int _count = 0;

    public int Count => _count;

    public Doctor this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
            {
                throw new IndexOutOfRangeException("Індекс знаходиться поза межами списку лікарів.");
            }
            return _doctors[index];
        }
    }

    public void Add(Doctor doctor)
    {
        if (_count >= MaxDoctors)
        {
            Console.WriteLine($"Не вдалося додати лікаря: досягнуто ліміту ({MaxDoctors}).");
            return;
        }
        _doctors[_count] = doctor;
        _count++;
        Console.WriteLine($"Лікаря [{doctor.Id}] {doctor.FullName} додано.");
    }

    public Doctor? FindById(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].Id == id) return _doctors[i];
        }
        return null;
    }

    public bool TryFindById(int id, out Doctor? doctor)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].Id == id)
            {
                doctor = _doctors[i];
                return true;
            }
        }
        doctor = null;
        return false;
    }

    public Doctor[] FindBySpeciality(string speciality)
    {
        string search = speciality.ToLower();
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            string ukrSpec = ClinicFormatter.FormatSpeciality(_doctors[i].Speciality).ToLower();
            if (ukrSpec.Contains(search)) matchCount++;
        }

        Doctor[] result = new Doctor[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            string ukrSpec = ClinicFormatter.FormatSpeciality(_doctors[i].Speciality).ToLower();
            if (ukrSpec.Contains(search))
            {
                result[index] = _doctors[i];
                index++;
            }
        }
        return result;
    }

    public Doctor[] FindBySpeciality(Speciality speciality)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].Speciality == speciality) matchCount++;
        }

        Doctor[] result = new Doctor[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].Speciality == speciality)
            {
                result[index] = _doctors[i];
                index++;
            }
        }
        return result;
    }

    public Doctor[] GetAll()
    {
        Doctor[] copy = new Doctor[_count];
        Array.Copy(_doctors, copy, _count);
        return copy;
    }

    public bool Remove(int id)
    {
        int indexToRemove = -1;
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].Id == id) { indexToRemove = i; break; }
        }
        if (indexToRemove == -1) return false;

        for (int i = indexToRemove; i < _count - 1; i++)
        {
            _doctors[i] = _doctors[i + 1];
        }
        _doctors[_count - 1] = null!;
        _count--;
        return true;
    }

    public void DisplayAll()
    {
        Console.WriteLine($"Лікарі ({_count} / {MaxDoctors}):");
        if (_count == 0)
        {
            Console.WriteLine("Список порожній.");
            return;
        }
        for (int i = 0; i < _count; i++) Console.WriteLine(_doctors[i]);
        Console.WriteLine(new string('─', 62));
    }

    public void DisplayStats()
    {
        Console.WriteLine("Статистика лікарів:");
        if (_count == 0)
        {
            Console.WriteLine("Список порожній.");
            Console.WriteLine(new string('=', 26));
            return;
        }

        int availableCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].IsAvailableNow) availableCount++;
        }

        Console.WriteLine($"Всього:         {_count}");
        Console.WriteLine($"Доступні зараз: {availableCount}");
        Console.WriteLine("По спеціальностях:");

        for (int i = 0; i < _count; i++)
        {
            bool seenBefore = false;
            for (int j = 0; j < i; j++)
            {
                if (_doctors[j].Speciality == _doctors[i].Speciality)
                {
                    seenBefore = true;
                    break;
                }
            }
            if (!seenBefore)
            {
                int specialityCount = 0;
                for (int k = 0; k < _count; k++)
                {
                    if (_doctors[k].Speciality == _doctors[i].Speciality) specialityCount++;
                }
                string specName = ClinicFormatter.FormatSpeciality(_doctors[i].Speciality);
                Console.WriteLine($"  {specName}: {specialityCount}");
            }
        }
        Console.WriteLine(new string('=', 26));
    }
}
