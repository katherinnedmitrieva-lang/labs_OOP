using System;
using System.Collections.Generic;
using ClinicApp.Enums;
using ClinicApp.Events;
using ClinicApp.Models;
using ClinicApp.Utils;

namespace ClinicApp.Managers;

// Зберігає плани лікування. Невалідний план у список не потрапляє: перед додаванням
// кожен план проходить через ModelValidator (правила беруться з атрибутів на властивостях).
public class TreatmentPlanManager
{
    private readonly List<TreatmentPlan> _plans = new List<TreatmentPlan>();

    // Подія «план лікування завершено». Піднімається лише коли перехід у стан Completed справді відбувся.
    public event EventHandler<TreatmentPlanEventArgs>? PlanCompleted;

    // Повертає true, якщо план додано. Інакше виводить помилки й повертає false.
    public bool Add(TreatmentPlan plan)
    {
        if (plan == null) throw new ArgumentNullException(nameof(plan));

        ValidationResult validation = ModelValidator.Validate(plan);
        if (!validation.IsValid)
        {
            Console.WriteLine("План не пройшов валідацію:");
            validation.Print();
            return false;
        }

        // Той самий об'єкт двічі додавати не можна: ID має бути унікальним.
        if (GetById(plan.Id) != null)
        {
            Console.WriteLine($"План з ID {plan.Id} уже є в списку.");
            return false;
        }

        _plans.Add(plan);
        return true;
    }

    // Завершує план за ID. Повертає false, якщо плану немає або перехід неможливий (завершити можна лише активний план).
    public bool Complete(int planId)
    {
        TreatmentPlan? plan = GetById(planId);
        if (plan == null) return false;
        if (!plan.Complete()) return false;

        PlanCompleted?.Invoke(this, new TreatmentPlanEventArgs(plan.Id, plan.PatientId, plan.Diagnosis));
        return true;
    }

    public TreatmentPlan? GetById(int id)
    {
        foreach (TreatmentPlan plan in _plans)
        {
            if (plan.Id == id) return plan;
        }
        return null;
    }

    public TreatmentPlan[] GetByPatient(int patientId)
    {
        List<TreatmentPlan> found = new List<TreatmentPlan>();
        foreach (TreatmentPlan plan in _plans)
        {
            if (plan.PatientId == patientId) found.Add(plan);
        }
        return found.ToArray();
    }

    public TreatmentPlan[] GetByStatus(TreatmentStatus status)
    {
        List<TreatmentPlan> found = new List<TreatmentPlan>();
        foreach (TreatmentPlan plan in _plans)
        {
            if (plan.Status == status) found.Add(plan);
        }
        return found.ToArray();
    }

    // Повертаємо КОПІЮ у вигляді масиву: ззовні неможливо змінити внутрішній список, обминувши валідацію в Add.
    public TreatmentPlan[] GetAll()
    {
        return _plans.ToArray();
    }
}
