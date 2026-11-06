using System;
using System.Collections.Generic;

namespace ClinicApp.Models;

// Узагальнений (generic) клас черги очікування — обгортка над стандартною Queue<T>.
// <T> — параметр типу: конкретний тип (Patient, string, int...) вказує той, хто створює об'єкт,
// напр. new WaitingQueue<Patient>(). Обмежень на T немає, тому підходить будь-який тип.
// Черга працює за принципом FIFO: перший прийшов — перший приймається.
public class WaitingQueue<T>
{
    // Внутрішня колекція прихована (private): зовні черга дає лише безпечні операції.
    private readonly Queue<T> _queue = new Queue<T>();

    // Кількість елементів у черзі (лише читання).
    public int Count => _queue.Count;

    // Чи порожня черга.
    public bool IsEmpty => _queue.Count == 0;

    // Додати в кінець черги.
    public void Enqueue(T item)
    {
        _queue.Enqueue(item);
    }

    // Прийняти першого: повертає його і ВИДАЛЯЄ з черги.
    // На порожній черзі кидає InvalidOperationException зі зрозумілим повідомленням
    // (явна перевірка IsEmpty дає краще повідомлення, ніж стандартне від Queue<T>).
    public T Dequeue()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Черга порожня: нікого приймати.");
        return _queue.Dequeue();
    }

    // Подивитись, хто перший, НЕ видаляючи його з черги.
    public T Peek()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Черга порожня: немає першого в черзі.");
        return _queue.Peek();
    }

    // Поточний стан черги у вигляді масиву (для виводу). Порядок елементів — від першого до останнього.
    // Це копія, тому зміни масиву не впливають на чергу.
    public T[] ToArray()
    {
        return _queue.ToArray();
    }
}