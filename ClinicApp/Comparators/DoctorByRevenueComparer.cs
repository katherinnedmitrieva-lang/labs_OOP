using System.Collections.Generic;
using ClinicApp.Models;

namespace ClinicApp.Comparators;

// Критерій «виручка»: найбільша виручка — першою (спадання).
// Для спадання аргументи CompareTo міняємо місцями: y.TotalRevenue.CompareTo(x.TotalRevenue).
public class DoctorByRevenueComparer : IComparer<DoctorStats>
{
    public int Compare(DoctorStats? x, DoctorStats? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        return y.TotalRevenue.CompareTo(x.TotalRevenue);
    }
}
