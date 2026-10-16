using System;
using ClinicApp.Models;

namespace ClinicApp.Managers;

public class MedicalRecordManager
{
    private const int MaxRecords = 1000;
    private MedicalRecord[] _records = new MedicalRecord[MaxRecords];
    private int _count = 0;

    public int Count => _count;

    public MedicalRecord? this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) return null;
            return _records[index];
        }
    }

    public void Add(MedicalRecord record)
    {
        if (_count >= MaxRecords)
        {
            Console.WriteLine($"Не вдалося додати запис: досягнуто ліміту ({MaxRecords}).");
            return;
        }
        _records[_count] = record;
        _count++;
        Console.WriteLine($"Запис [{record.Id}] {record.GetRecordType()} додано.");
    }

    public MedicalRecord? FindById(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].Id == id) return _records[i];
        }
        return null;
    }

    public MedicalRecord[] GetByPatient(int patientId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId) matchCount++;
        }

        MedicalRecord[] result = new MedicalRecord[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId)
            {
                result[index] = _records[i];
                index++;
            }
        }
        return result;
    }

    public MedicalRecord[] GetByDoctor(int doctorId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].DoctorId == doctorId) matchCount++;
        }

        MedicalRecord[] result = new MedicalRecord[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].DoctorId == doctorId)
            {
                result[index] = _records[i];
                index++;
            }
        }
        return result;
    }

    public void DisplayAll()
    {
        Console.WriteLine($"Медичні записи ({_count} / {MaxRecords}):");
        if (_count == 0)
        {
            Console.WriteLine("Записів не знайдено.");
            return;
        }
        for (int i = 0; i < _count; i++) Console.WriteLine(_records[i]);
        Console.WriteLine(new string('─', 62));
    }

    // Жодних is/as: кожен об'єкт сам виводить себе через ToString() (поліморфізм)
    public void DisplayList(MedicalRecord[] list)
    {
        if (list.Length == 0)
        {
            Console.WriteLine("Записів не знайдено.");
            return;
        }
        foreach (MedicalRecord record in list)
        {
            Console.WriteLine(record);
        }
    }
}