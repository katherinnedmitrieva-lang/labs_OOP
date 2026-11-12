using System.Collections.Generic;
using ClinicApp.Interfaces;

namespace ClinicApp.Managers;

// Узагальнений репозиторій (CRUD) для будь-якого типу з ідентифікатором.
// Обмеження where T : IIdentifiable ГАРАНТУЄ компілятору, що в кожного T є властивість Id,
// тому всередині класу можна писати item.Id. Без обмеження це не скомпілювалось би
// (T міг би бути чим завгодно, наприклад string), а Repository<string> тепер заборонений.
// На відміну від WaitingQueue<T>, якій від типу нічого не потрібно (вона лише зберігає елементи),
// Repository мусить уміти шукати й видаляти за Id — тому йому потрібна гарантія.
public class Repository<T> where T : IIdentifiable
{
    private readonly List<T> _items = new List<T>();

    // Кількість елементів у репозиторії.
    public int Count => _items.Count;

    // Додати елемент.
    public void Add(T item)
    {
        _items.Add(item);
    }

    // Знайти за Id. Якщо елемента немає — default (для класів це null),
    // тому тип результату T? і виклик зазвичай дописують як repo.GetById(id)?.Властивість.
    public T? GetById(int id)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Id == id) return _items[i];
        }
        return default;
    }

    // Усі елементи як масив (копія — внутрішній список зовні недоступний).
    public T[] GetAll()
    {
        return _items.ToArray();
    }

    // Видалити за Id. true — елемент був і видалений; false — такого Id не було.
    public bool Remove(int id)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i].Id == id)
            {
                _items.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
}