using System;

namespace ClinicApp.Models;

// Незмінний (immutable) аналітичний «знімок» по одному лікарю: усі значення задаються один раз
// у конструкторі, а властивості мають лише get — після створення змінити їх неможливо.
// Реалізація IComparable<DoctorStats> дає об'єкту ПРИРОДНИЙ порядок: Array.Sort(DoctorStats[])
// працює без жодних додаткових параметрів і ставить найнавантаженішого лікаря першим.
public class DoctorStats : IComparable<DoctorStats>
{
    public int DoctorId { get; }
    public string DoctorName { get; }
    public int AppointmentCount { get; }
    public decimal TotalRevenue { get; }
    public DateTime LastAppointmentDate { get; }

    public DoctorStats(int doctorId, string doctorName, int appointmentCount, decimal totalRevenue, DateTime lastAppointmentDate)
    {
        if (doctorName == null)
            throw new ArgumentNullException(nameof(doctorName));
        if (appointmentCount < 0)
            throw new ArgumentOutOfRangeException(nameof(appointmentCount), "Кількість прийомів не може бути від'ємною.");
        if (totalRevenue < 0m)
            throw new ArgumentOutOfRangeException(nameof(totalRevenue), "Виручка не може бути від'ємною.");

        DoctorId = doctorId;
        DoctorName = doctorName;
        AppointmentCount = appointmentCount;
        TotalRevenue = totalRevenue;
        LastAppointmentDate = lastAppointmentDate;
    }

    // Контракт CompareTo: від'ємне число — this стоїть РАНІШЕ за other, 0 — рівні, додатне — ПІЗНІШЕ.
    // Лікар із БІЛЬШОЮ кількістю прийомів має стояти першим, тому порядок аргументів обернений:
    // other.AppointmentCount.CompareTo(AppointmentCount) (при звичайному this.CompareTo(other) вийшов би зворотний порядок).
    public int CompareTo(DoctorStats? other)
    {
        // За контрактом IComparable будь-який об'єкт «більший» за null (null іде першим у зростанні).
        if (other is null) return 1;
        return other.AppointmentCount.CompareTo(AppointmentCount);
    }

    // Один рядок: ID, ім'я, кількість прийомів, виручка, дата останнього прийому.
    // Немає прийомів (або дата = default(DateTime), тобто 01.01.0001) — замість дивної дати виводимо «—».
    public override string ToString()
    {
        string lastText = AppointmentCount == 0 || LastAppointmentDate == default(DateTime)
            ? "—"
            : LastAppointmentDate.ToString("dd.MM.yyyy");
        return $"[{DoctorId}] {DoctorName} | Прийомів: {AppointmentCount} | Виручка: {TotalRevenue:F2} грн | Останній прийом: {lastText}";
    }
}
