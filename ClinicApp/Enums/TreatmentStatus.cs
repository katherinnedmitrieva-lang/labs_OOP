namespace ClinicApp.Enums;

// Стан плану лікування. Допустимі переходи (див. TreatmentPlan):
// Planned → Active → Completed; скасувати можна з Planned або Active.
public enum TreatmentStatus
{
    Planned,
    Active,
    Completed,
    Cancelled
}
