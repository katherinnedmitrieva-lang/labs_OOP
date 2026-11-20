using System;
using System.Collections.Generic;

namespace ClinicApp.Utils;

// Контейнер помилок валідації. Окремий клас (а не просто List<string>), щоб ззовні помилки можна було
// лише читати: додавати їх можна тільки через AddError.
public class ValidationResult
{
    private readonly List<string> _errors = new List<string>();
    private readonly IReadOnlyList<string> _readOnlyErrors;

    public ValidationResult()
    {
        // Обгортка лише для читання, створена один раз: навіть явне приведення типу не дасть змінити внутрішній список.
        _readOnlyErrors = _errors.AsReadOnly();
    }

    // true, якщо помилок немає
    public bool IsValid
    {
        get { return _errors.Count == 0; }
    }

    public IReadOnlyList<string> Errors
    {
        get { return _readOnlyErrors; }
    }

    public void AddError(string error)
    {
        if (error == null) throw new ArgumentNullException(nameof(error));
        _errors.Add(error);
    }

    // Кожна помилка — окремий рядок з позначкою [!]
    public void Print()
    {
        foreach (string error in _errors)
        {
            Console.WriteLine($"  [!] {error}");
        }
    }
}
