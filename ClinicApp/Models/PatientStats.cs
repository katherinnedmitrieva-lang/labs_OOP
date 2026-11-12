using System;

namespace ClinicApp.Models;

// Незмінний аналітичний «знімок» по одному пацієнту (за аналогією з DoctorStats).
// Природний порядок — за кількістю візитів: найактивніший пацієнт стоїть першим.
public class PatientStats : IComparable<PatientStats>
{
    public int PatientId { get; }
    public string PatientName { get; }
    public int VisitCount { get; }
    public decimal TotalSpent { get; }
    public DateTime LastVisitDate { get; }

    public PatientStats(int patientId, string patientName, int visitCount, decimal totalSpent, DateTime lastVisitDate)
    {
        if (patientName == null)
            throw new ArgumentNullException(nameof(patientName));
        if (visitCount < 0)
            throw new ArgumentOutOfRangeException(nameof(visitCount), "Кількість візитів не може бути від'ємною.");
        if (totalSpent < 0m)
            throw new ArgumentOutOfRangeException(nameof(totalSpent), "Витрачена сума не може бути від'ємною.");

        PatientId = patientId;
        PatientName = patientName;
        VisitCount = visitCount;
        TotalSpent = totalSpent;
        LastVisitDate = lastVisitDate;
    }

    // Більше візитів — вища позиція (раніше в масиві), тому порівнюємо у зворотному порядку.
    public int CompareTo(PatientStats? other)
    {
        if (other is null) return 1;
        return other.VisitCount.CompareTo(VisitCount);
    }

    // Один рядок з усіма даними. Якщо візитів не було, LastVisitDate лишається 01.01.0001 —
    // у такому разі виводимо «—», а не цю службову дату.
    public override string ToString()
    {
        string lastText = VisitCount == 0 || LastVisitDate == default(DateTime)
            ? "—"
            : LastVisitDate.ToString("dd.MM.yyyy");
        return $"[{PatientId}] {PatientName} | Візитів: {VisitCount} | Витрачено: {TotalSpent:F2} грн | Останній візит: {lastText}";
    }
}
