using System;
using System.Collections.Generic;
using System.Text;

namespace ClinicApp.Interfaces
{
    // Контракт: будь-який тип, що реалізує цей інтерфейс, має цілочисельний ідентифікатор.
    // Саме на цю гарантію спирається Repository<T> (обмеження where T : IIdentifiable).
    public interface IIdentifiable
    {
        int Id { get; }
    }
}