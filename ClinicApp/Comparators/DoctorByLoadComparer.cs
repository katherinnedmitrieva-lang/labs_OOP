using System.Collections.Generic;
using ClinicApp.Models;

namespace ClinicApp.Comparators;

// Критерій «навантаження»: найМЕНШ завантажений лікар — першим (зростання кількості прийомів).
// Це свідомо ЗВОРОТНИЙ напрямок до природного порядку DoctorStats (там найнавантаженіший перший):
// так видно, кому з лікарів можна довірити нові записи.
public class DoctorByLoadComparer : IComparer<DoctorStats>
{
    public int Compare(DoctorStats? x, DoctorStats? y)
    {
        // Контракт IComparer: null вважається меншим за будь-який об'єкт, два null — рівні.
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        return x.AppointmentCount.CompareTo(y.AppointmentCount);
    }
}
