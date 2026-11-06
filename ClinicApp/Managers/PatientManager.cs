using System;
using System.Collections.Generic;
using ClinicApp.Enums;
using ClinicApp.Models;

namespace ClinicApp.Managers;


public class PatientManager
{
    // Раніше тут був масив фіксованого розміру з ручним лічильником.
    // Тепер List<Patient> — динамічний масив, який сам знає свій розмір (Count)
    // і сам збільшується при додаванні. Тому штучний ліміт на кількість пацієнтів більше не потрібен.
    private List<Patient> _patients = new List<Patient>();

    // Кількість елементів тепер рахує сам список.
    public int Count => _patients.Count;

    public Patient? this[int index]
    {
        get
        {
            if (index < 0 || index >= _patients.Count) return null;
            return _patients[index];
        }
    }

    public void Add(Patient patient)
    {
        // Без перевірки ліміту: List<T> сам розширюється.
        _patients.Add(patient);
        Console.WriteLine($"Пацієнта [{patient.Id}] {patient.FullName} додано.");
    }

    public Patient? FindById(int id)
    {
        for (int i = 0; i < _patients.Count; i++)
        {
            if (_patients[i].Id == id) return _patients[i];
        }
        return null;
    }

    public bool TryFindById(int id, out Patient? patient)
    {
        patient = FindById(id);
        return patient != null;
    }

    public Patient[] FindByName(string name)
    {
        string search = name.ToLower();

        // Двопрохідний патерн більше не потрібен: наповнюємо проміжний список
        // (не треба наперед знати, скільки буде збігів) і в кінці перетворюємо його на масив.
        List<Patient> result = new List<Patient>();
        for (int i = 0; i < _patients.Count; i++)
        {
            if (_patients[i].FullName.ToLower().Contains(search)) result.Add(_patients[i]);
        }
        return result.ToArray();
    }

    public Patient[] FindByBloodType(BloodType bloodType)
    {
        List<Patient> result = new List<Patient>();
        for (int i = 0; i < _patients.Count; i++)
        {
            if (_patients[i].BloodType == bloodType) result.Add(_patients[i]);
        }
        return result.ToArray();
    }

    public Patient[] GetAll()
    {
        // ToArray() створює НОВИЙ масив-копію однією операцією (замість Array.Copy у циклі),
        // тому зовнішній код не може зіпсувати внутрішній список.
        return _patients.ToArray();
    }

    public bool Remove(int id)
    {
        for (int i = 0; i < _patients.Count; i++)
        {
            if (_patients[i].Id == id)
            {
                // RemoveAt сам зсуває всі наступні елементи — ручний зсув не потрібен.
                _patients.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public void DisplayAll()
    {
        Console.WriteLine($"Пацієнти ({_patients.Count}):");
        if (_patients.Count == 0)
        {
            Console.WriteLine("Список порожній.");
            return;
        }
        for (int i = 0; i < _patients.Count; i++) Console.WriteLine(_patients[i]);
        Console.WriteLine(new string('─', 62));
    }

    public void DisplayStats()
    {
        Console.WriteLine("Статистика пацієнтів:");
        if (_patients.Count == 0)
        {
            Console.WriteLine("Список порожній.");
            Console.WriteLine(new string('=', 26));
            return;
        }

        int adults = 0;
        int children = 0;
        for (int i = 0; i < _patients.Count; i++)
        {
            if (_patients[i].IsAdult) adults++;
            else children++;
        }

        Console.WriteLine($"Всього:    {_patients.Count}");
        Console.WriteLine($"Дорослих:  {adults}");
        Console.WriteLine($"Дітей:     {children}");
        Console.WriteLine(new string('=', 26));
    }
}