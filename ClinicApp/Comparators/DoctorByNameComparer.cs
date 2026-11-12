using System;
using System.Collections.Generic;
using System.Globalization;
using ClinicApp.Models;

namespace ClinicApp.Comparators;

// Критерій «алфавіт»: імена від А до Я (зростання).
// Імена порівнюємо з урахуванням МОВИ (культура uk-UA), а не за кодами символів:
// ordinal-порівняння ставило б «Я» перед «а» (малі літери мають більші коди) і неправильно
// розташовувало б «і», «ї», «є», «ґ». Регістр ігноруємо, щоб «олег» і «Олег» були рівними.
public class DoctorByNameComparer : IComparer<DoctorStats>
{
    // Створюється один раз на весь клас (static readonly), а не при кожному порівнянні.
    private static readonly StringComparer NameComparer =
        StringComparer.Create(new CultureInfo("uk-UA"), ignoreCase: true);

    public int Compare(DoctorStats? x, DoctorStats? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        return NameComparer.Compare(x.DoctorName, y.DoctorName);
    }
}
