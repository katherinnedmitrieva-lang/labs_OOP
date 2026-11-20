using System;
using System.Globalization;
using System.Reflection;
using ClinicApp.Attributes;

namespace ClinicApp.Utils;

// Будує консольну форму введення для БУДЬ-ЯКОГО типу автоматично — за метаданими його властивостей:
// які поля питати, які підказки показати (з атрибутів), які пропустити.
public static class FormBuilder
{
    // where T : new() — T мусить мати публічний конструктор без параметрів; без цього рядок new T() не скомпілюється.
    public static T Build<T>() where T : new()
    {
        T obj = new T();

        // Лише публічні ЕКЗЕМПЛЯРНІ властивості: GetProperties() без прапорців повертає ще й статичні,
        // а форма має заповнювати поля об'єкта, а не стан класу.
        foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            // ПАСТКА: CanWrite == true і для ПРИВАТНОГО сеттера (як у властивості з private set).
            // GetSetMethod() без аргументів повертає лише ПУБЛІЧНИЙ сеттер, а для приватного/відсутнього — null.
            // Тому Id (немає сеттера) і Status (private set) форма не питає.
            if (property.GetSetMethod() == null) continue;
            if (property.GetIndexParameters().Length > 0) continue;

            // Тип, який неможливо ввести з консолі (колекція, власний клас), пропускаємо:
            // інакше цикл повторного запиту ніколи б не завершився.
            if (!IsSupported(property.PropertyType)) continue;

            string prompt = $"  {property.Name}{BuildHint(property)}: ";

            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                // Кінець введення (Ctrl+Z / закритий потік): далі питати нікого — повертаємо, що встигли заповнити.
                if (input == null)
                {
                    Console.WriteLine();
                    Console.WriteLine("  Введення перервано.");
                    return obj;
                }

                if (TryConvert(input, property.PropertyType, out object? converted))
                {
                    property.SetValue(obj, converted);
                    break;
                }

                Console.WriteLine("  Некоректне значення, спробуйте ще раз.");
            }
        }

        return obj;
    }

    // Підказка з атрибутів: «(обов'язкове, макс. 200)», «(мін. 1)». Для enum додаємо перелік значень.
    private static string BuildHint(PropertyInfo property)
    {
        string hint = "";

        if (property.GetCustomAttribute<RequiredAttribute>() != null)
            hint = Append(hint, "обов'язкове");

        MaxLengthAttribute? maxLength = property.GetCustomAttribute<MaxLengthAttribute>();
        if (maxLength != null)
            hint = Append(hint, $"макс. {maxLength.Length}");

        MinValueAttribute? minValue = property.GetCustomAttribute<MinValueAttribute>();
        if (minValue != null)
            hint = Append(hint, $"мін. {minValue.Min.ToString(CultureInfo.InvariantCulture)}");

        Type target = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        if (target.IsEnum)
            hint = Append(hint, "один з: " + string.Join(", ", Enum.GetNames(target)));

        return hint.Length == 0 ? "" : $" ({hint})";
    }

    private static string Append(string hint, string part)
    {
        return hint.Length == 0 ? part : hint + ", " + part;
    }

    // Чи вміємо ми перетворити рядок з консолі на цей тип: рядок, enum або будь-який IConvertible (числа, bool, DateTime...).
    private static bool IsSupported(Type propertyType)
    {
        Type target = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        return target == typeof(string) || target.IsEnum || typeof(IConvertible).IsAssignableFrom(target);
    }

    // Перетворює введений рядок на значення властивості. Повертає false (а не кидає виняток) при некоректному вводі,
    // щоб форма просто перепитала поле.
    private static bool TryConvert(string input, Type propertyType, out object? value)
    {
        value = null;

        Type? nullableUnderlying = Nullable.GetUnderlyingType(propertyType);
        Type target = nullableUnderlying ?? propertyType;

        // Рядок приймаємо як є, навіть порожній: чи він допустимий, вирішує валідатор (наприклад, [Required]).
        if (target == typeof(string))
        {
            value = input;
            return true;
        }

        string text = input.Trim();

        if (text.Length == 0)
        {
            // Порожнє введення для int? / DateTime? означає «значення відсутнє»; для звичайного int — некоректно.
            return nullableUnderlying != null;
        }

        // Convert.ChangeType не вміє перетворювати рядок на enum — для нього окрема гілка.
        if (target.IsEnum)
        {
            // Enum.TryParse приймає й числа («99»), тому додатково перевіряємо, що таке значення справді оголошене в enum.
            if (!Enum.TryParse(target, text, true, out object? parsed) || !Enum.IsDefined(target, parsed))
                return false;
            value = parsed;
            return true;
        }

        // Для дробових чисел дозволяємо і кому, і крапку як роздільник.
        if (target == typeof(double) || target == typeof(float) || target == typeof(decimal))
            text = text.Replace(',', '.');

        try
        {
            object converted = Convert.ChangeType(text, target, CultureInfo.InvariantCulture);

            // «NaN» і «Infinity» парсяться як double, але це не осмислені значення для форми.
            if (converted is double d && !double.IsFinite(d)) return false;
            if (converted is float f && !float.IsFinite(f)) return false;

            value = converted;
            return true;
        }
        catch (FormatException)
        {
            return false;   // «abc» замість числа
        }
        catch (OverflowException)
        {
            return false;   // число завелике для типу, наприклад 99999999999 для int
        }
        catch (InvalidCastException)
        {
            return false;
        }
    }
}
