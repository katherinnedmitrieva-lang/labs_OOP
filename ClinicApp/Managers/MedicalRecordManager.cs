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

    // ===== Задача 3: фільтрація за реальним типом =====

    public Diagnosis[] GetDiagnoses(int patientId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId && _records[i] is Diagnosis) matchCount++;
        }

        Diagnosis[] result = new Diagnosis[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId && _records[i] is Diagnosis diagnosis)
            {
                result[index] = diagnosis;
                index++;
            }
        }
        return result;
    }

    // Тут для демонстрації використано as (повертає null, якщо тип не збігається)
    public LabResult[] GetLabResults(int patientId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            LabResult? lab = _records[i] as LabResult;
            if (lab != null && lab.PatientId == patientId) matchCount++;
        }

        LabResult[] result = new LabResult[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            LabResult? lab = _records[i] as LabResult;
            if (lab != null && lab.PatientId == patientId)
            {
                result[index] = lab;
                index++;
            }
        }
        return result;
    }

    public Prescription[] GetPrescriptions(int patientId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId && _records[i] is Prescription) matchCount++;
        }

        Prescription[] result = new Prescription[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId && _records[i] is Prescription prescription)
            {
                result[index] = prescription;
                index++;
            }
        }
        return result;
    }

    public Diagnosis[] GetChronicDiagnoses(int patientId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId
                && _records[i] is Diagnosis diagnosis && diagnosis.IsChronic) matchCount++;
        }

        Diagnosis[] result = new Diagnosis[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId
                && _records[i] is Diagnosis diagnosis && diagnosis.IsChronic)
            {
                result[index] = diagnosis;
                index++;
            }
        }
        return result;
    }

    public Prescription[] GetActivePrescriptions(int patientId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId
                && _records[i] is Prescription prescription && prescription.IsActive()) matchCount++;
        }

        Prescription[] result = new Prescription[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId
                && _records[i] is Prescription prescription && prescription.IsActive())
            {
                result[index] = prescription;
                index++;
            }
        }
        return result;
    }

    public void DisplayPatientSummary(int patientId)
    {
        MedicalRecord[] all = GetByPatient(patientId);
        if (all.Length == 0)
        {
            Console.WriteLine($"У пацієнта #{patientId} немає медичних записів.");
            return;
        }

        Diagnosis[] diagnoses = GetDiagnoses(patientId);
        LabResult[] labResults = GetLabResults(patientId);
        Prescription[] prescriptions = GetPrescriptions(patientId);

        Console.WriteLine($"=== Медична картка пацієнта #{patientId} ===");
        Console.WriteLine($"Всього записів: {all.Length} " +
                          $"(діагнозів: {diagnoses.Length}, аналізів: {labResults.Length}, рецептів: {prescriptions.Length})");

        Diagnosis[] chronic = GetChronicDiagnoses(patientId);
        if (chronic.Length > 0)
        {
            Console.WriteLine($"Хронічні діагнози ({chronic.Length}):");
            foreach (Diagnosis d in chronic) Console.WriteLine($"  {d}");
        }

        Prescription[] active = GetActivePrescriptions(patientId);
        if (active.Length > 0)
        {
            Console.WriteLine($"Активні рецепти ({active.Length}):");
            foreach (Prescription p in active)
                Console.WriteLine($"  {p} | до {p.ExpiresAt:dd.MM.yyyy}");
        }
    }
}