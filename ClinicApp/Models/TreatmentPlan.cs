using ClinicApp.Attributes;
using ClinicApp.Enums;

namespace ClinicApp.Models;

// План лікування. Правила полів описані АТРИБУТАМИ над властивостями — сам клас їх не перевіряє:
// перевірку виконує ModelValidator (рефлексія), а підказки для введення будує FormBuilder.
public class TreatmentPlan
{
    private static int _nextId = 1;   // спільний лічильник: кожен новий план отримує наступний номер

    public int Id { get; }

    // У int не буває null, тому [Required] тут не мав би сенсу — «обов'язковість» числового ID
    // виражаємо мінімальним значенням 1.
    [MinValue(1, "Patient ID is required.")]
    public int PatientId { get; set; }

    // Кілька РІЗНИХ атрибутів на одній властивості — допустимо (AllowMultiple = false забороняє лише дублі одного типу).
    [Required("Diagnosis cannot be empty.")]
    [MaxLength(200, "Diagnosis must not exceed 200 characters.")]
    public string Diagnosis { get; set; } = "";

    [MinValue(1, "Duration must be at least 1 day.")]
    public int DurationDays { get; set; }

    // private set: стан змінюється ЛИШЕ методами-переходами нижче, тож правила переходів неможливо обійти.
    public TreatmentStatus Status { get; private set; } = TreatmentStatus.Planned;

    // Конструктор без параметрів потрібен для ініціалізатора new TreatmentPlan { ... } і для FormBuilder (where T : new()).
    public TreatmentPlan()
    {
        Id = _nextId++;
    }

    // Planned → Active. З будь-якого іншого стану перехід неможливий.
    public bool Activate()
    {
        if (Status != TreatmentStatus.Planned) return false;
        Status = TreatmentStatus.Active;
        return true;
    }

    // Active → Completed. Завершити можна лише план, який уже розпочато.
    public bool Complete()
    {
        if (Status != TreatmentStatus.Active) return false;
        Status = TreatmentStatus.Completed;
        return true;
    }

    // Будь-який стан, крім Completed і Cancelled, → Cancelled. Завершений чи вже скасований план скасувати не можна.
    public bool Cancel()
    {
        if (Status == TreatmentStatus.Completed || Status == TreatmentStatus.Cancelled) return false;
        Status = TreatmentStatus.Cancelled;
        return true;
    }

    public override string ToString()
    {
        return $"[{Id}] Пацієнт #{PatientId} | {Diagnosis} | {DurationDays} дн. | {Status}";
    }
}
