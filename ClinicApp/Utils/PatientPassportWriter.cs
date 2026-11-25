using System;
using System.Globalization;
using System.IO;
using System.Text;
using ClinicApp.Events;
using ClinicApp.Models;

namespace ClinicApp.Utils;

// Другий незалежний підписник: генерує файл «паспорта пацієнта» при реєстрації пацієнта, після кожного завершеного
// прийому і після кожного завершеного плану лікування. Менеджери про цей клас нічого не знають — вони лише піднімають події.
public class PatientPassportWriter
{
    private readonly Clinic _clinic;
    private readonly string _passportDir;

    public PatientPassportWriter(Clinic clinic, string passportDir = "patients")
    {
        if (clinic == null) throw new ArgumentNullException(nameof(clinic));
        if (string.IsNullOrWhiteSpace(passportDir))
            throw new ArgumentException("Тека для паспортів не може бути порожньою.", nameof(passportDir));

        _clinic = clinic;
        _passportDir = passportDir;

        // Теку створюємо в конструкторі: до першого запису вона вже існує. Якщо тека вже є — виняток не виникає.
        Directory.CreateDirectory(_passportDir);
    }

    // Три обробники — на три різні події, але робота однакова, тому всі викликають ОДИН приватний метод:
    // логіка генерації не дублюється, а обробник лише дістає з «посилки» ID пацієнта.
    public void OnPatientAdded(object? sender, PatientEventArgs e)
    {
        WritePassport(e.PatientId);
    }

    public void OnAppointmentCompleted(object? sender, AppointmentEventArgs e)
    {
        WritePassport(e.PatientId);
    }

    public void OnPlanCompleted(object? sender, TreatmentPlanEventArgs e)
    {
        WritePassport(e.PatientId);
    }

    // Записує patients/passport_{id}.txt. Файл ПЕРЕЗАПИСУЄТЬСЯ повністю: паспорт — це знімок поточного стану,
    // а не журнал, тому старі версії не накопичуються.
    private void WritePassport(int patientId)
    {
        Patient? patient = _clinic.Patients.FindById(patientId);
        if (patient == null) return;   // пацієнта немає — нічого не пишемо, щоб не створити порожній файл

        string path = Path.Combine(_passportDir, $"passport_{patientId}.txt");

        try
        {
            // append: false — перезапис. using гарантує, що файл буде закрито навіть після винятку.
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.WriteLine($"=== Паспорт пацієнта #{patient.Id} ===");
                writer.WriteLine("Згенеровано: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture));
                writer.WriteLine();

                WritePersonalData(writer, patient);
                writer.WriteLine();
                WriteMedicalRecords(writer, patientId);
                writer.WriteLine();
                decimal debt = WriteAppointments(writer, patientId);
                writer.WriteLine();
                WriteTreatmentPlans(writer, patientId);
                writer.WriteLine();
                writer.WriteLine("Заборгованість: " + debt.ToString("F2", CultureInfo.InvariantCulture) + " грн");
            }
        }
        catch (IOException ex)
        {
            // Збій запису паспорта не повинен ламати основну дію (додавання пацієнта, завершення прийому): пишемо в лог.
            _clinic.Logger.LogError($"Не вдалося записати паспорт пацієнта #{patientId}: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            _clinic.Logger.LogError($"Немає доступу до паспорта пацієнта #{patientId}: {ex.Message}");
        }
    }

    // Секція 1: особисті дані (джерело — Patients).
    private static void WritePersonalData(StreamWriter writer, Patient patient)
    {
        string birth = patient.DateOfBirth.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
        writer.WriteLine($"Ім'я: {patient.FullName}");
        writer.WriteLine($"Дата народження: {birth} ({ClinicFormatter.FormatAge(patient.Age)})");
        writer.WriteLine($"Група крові: {ClinicFormatter.FormatBloodType(patient.BloodType)}   Телефон: {ClinicFormatter.FormatPhone(patient.Phone)}");
    }

    // Секція 2: медичні записи, окремо діагнози, аналізи, рецепти (джерело — MedicalRecords).
    // Три окремі проходи простіші за один прохід із кількома if: кожна секція самодостатня.
    private void WriteMedicalRecords(StreamWriter writer, int patientId)
    {
        writer.WriteLine("-- Діагнози --");
        Diagnosis[] diagnoses = _clinic.MedicalRecords.GetDiagnoses(patientId);
        if (diagnoses.Length == 0) writer.WriteLine("(немає)");
        foreach (Diagnosis diagnosis in diagnoses)
        {
            writer.WriteLine(diagnosis.GetSummary());
        }

        writer.WriteLine("-- Аналізи --");
        LabResult[] labResults = _clinic.MedicalRecords.GetLabResults(patientId);
        if (labResults.Length == 0) writer.WriteLine("(немає)");
        foreach (LabResult labResult in labResults)
        {
            writer.WriteLine(labResult.GetSummary());
        }

        writer.WriteLine("-- Рецепти --");
        Prescription[] prescriptions = _clinic.MedicalRecords.GetPrescriptions(patientId);
        if (prescriptions.Length == 0) writer.WriteLine("(немає)");
        foreach (Prescription prescription in prescriptions)
        {
            writer.WriteLine(prescription.GetSummary());
        }
    }

    // Секція 3: записи на прийом (джерело — Appointments). Повертає борг пацієнта: суму неоплачених
    // записів, які не скасовані. Рахуємо в тому самому проході, щоб не ходити по записах двічі.
    private decimal WriteAppointments(StreamWriter writer, int patientId)
    {
        writer.WriteLine("-- Записи --");
        decimal debt = 0m;
        bool any = false;

        // GetAll() повертає масив із «порожніми» (null) комірками — їх пропускаємо.
        foreach (Appointment? appointment in _clinic.Appointments.GetAll())
        {
            if (appointment == null || appointment.PatientId != patientId) continue;

            any = true;
            string when = appointment.ScheduledAt.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
            string cost = appointment.GetCost().ToString("F2", CultureInfo.InvariantCulture);
            writer.WriteLine($"[{appointment.Id}] {appointment.GetDescription()} | {when} | {appointment.Status} | {cost} грн");

            if (!appointment.IsPaid && !appointment.IsCancelled)
                debt += appointment.GetCost();
        }

        if (!any) writer.WriteLine("(немає)");
        return debt;
    }

    // Секція 4: плани лікування (джерело — TreatmentPlans).
    private void WriteTreatmentPlans(StreamWriter writer, int patientId)
    {
        writer.WriteLine("-- Плани лікування --");
        TreatmentPlan[] plans = _clinic.TreatmentPlans.GetByPatient(patientId);
        if (plans.Length == 0) writer.WriteLine("(немає)");
        foreach (TreatmentPlan plan in plans)
        {
            writer.WriteLine($"[{plan.Id}] {plan.Diagnosis} | {plan.DurationDays} днів | {plan.Status}");
        }
    }
}
