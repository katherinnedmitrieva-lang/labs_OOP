using System;
using System.Collections.Generic;

namespace ClinicApp.Utils;

// Підсумок імпорту: скільки рядків імпортовано, скільки пропущено і чому саме.
public class ImportResult
{
    private readonly List<string> _errors = new List<string>();
    private readonly IReadOnlyList<string> _readOnlyErrors;

    // private set: лічильники змінюються лише через AddSuccess / AddError, тому вони завжди узгоджені зі списком помилок.
    public int Imported { get; private set; }
    public int Skipped { get; private set; }

    public ImportResult()
    {
        // Обгортка лише для читання, створена один раз: ззовні список помилок змінити неможливо.
        _readOnlyErrors = _errors.AsReadOnly();
    }

    public IReadOnlyList<string> Errors
    {
        get { return _readOnlyErrors; }
    }

    public void AddSuccess()
    {
        Imported++;
    }

    // Рядок помилки має вигляд «Рядок N: причина». Номер рядка — той, який бачить людина у файлі (з 1).
    public void AddError(int lineNumber, string reason)
    {
        Skipped++;
        _errors.Add($"Рядок {lineNumber}: {reason}");
    }

    // Підсумок і всі помилки (кожна з нового рядка).
    public void Print()
    {
        Console.WriteLine($"Імпортовано: {Imported} | Пропущено: {Skipped}");
        foreach (string error in _errors)
        {
            Console.WriteLine($"  {error}");
        }
    }
}
