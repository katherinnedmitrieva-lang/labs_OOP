using System;
using System.Globalization;
using System.Reflection;
using ClinicApp.Attributes;

namespace ClinicApp.Utils;

// Універсальний валідатор: нічого не знає про конкретні класи моделі.
// Правила він дізнається під час виконання — читає атрибути з властивостей через рефлексію.
// Тому працює з будь-яким об'єктом, на властивостях якого стоять наші атрибути.
public static class ModelValidator
{
    // Публічні екземплярні властивості (разом з успадкованими).
    private const BindingFlags InstanceProperties = BindingFlags.Public | BindingFlags.Instance;

    public static ValidationResult Validate(object obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        ValidationResult result = new ValidationResult();

        // GetType() — РЕАЛЬНИЙ тип об'єкта під час виконання (на відміну від typeof(T), відомого на етапі компіляції).
        // Прапорці вказуємо явно: GetProperties() без аргументів повертає й СТАТИЧНІ властивості,
        // а це стан класу, а не перевіреного об'єкта.
        PropertyInfo[] properties = obj.GetType().GetProperties(InstanceProperties);

        foreach (PropertyInfo property in properties)
        {
            // Індексатори (this[int i]) вимагають аргументів, а властивості без get прочитати неможливо — пропускаємо.
            if (!property.CanRead || property.GetIndexParameters().Length > 0) continue;

            object? value = property.GetValue(obj);

            // GetCustomAttribute<T>() повертає null, якщо такого атрибута на властивості немає.
            RequiredAttribute? required = property.GetCustomAttribute<RequiredAttribute>();
            if (required != null && IsMissing(value))
            {
                result.AddError($"{property.Name}: {required.ErrorMessage}");
            }

            MaxLengthAttribute? maxLength = property.GetCustomAttribute<MaxLengthAttribute>();
            if (maxLength != null && value is string text && text.Length > maxLength.Length)
            {
                string message = MessageOrDefault(maxLength.ErrorMessage, $"Length must not exceed {maxLength.Length}.");
                result.AddError($"{property.Name}: {message}");
            }

            MinValueAttribute? minValue = property.GetCustomAttribute<MinValueAttribute>();
            if (minValue != null && value != null && TryGetNumber(value, out double number) && !(number >= minValue.Min))
            {
                // Запис !(number >= Min), а не number < Min — щоб NaN теж вважався помилкою.
                string message = MessageOrDefault(minValue.ErrorMessage,
                    $"Value must be at least {minValue.Min.ToString(CultureInfo.InvariantCulture)}.");
                result.AddError($"{property.Name}: {message}");
            }
        }

        return result;
    }

    // Виводить назву типу та список його властивостей з атрибутами. Приймає Type, а не об'єкт:
    // рефлексія працює без створення екземпляра.
    public static void PrintInfo(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        Console.WriteLine(type.Name);

        PropertyInfo[] properties = type.GetProperties(InstanceProperties);

        // Ширина колонки з назвами — за найдовшою назвою, щоб атрибути стояли рівно.
        int nameWidth = 0;
        foreach (PropertyInfo property in properties)
        {
            if (property.Name.Length > nameWidth) nameWidth = property.Name.Length;
        }

        foreach (PropertyInfo property in properties)
        {
            string attributes = FormatAttributes(property);
            if (attributes.Length == 0)
                Console.WriteLine($"  {property.Name}");
            else
                Console.WriteLine($"  {property.Name.PadRight(nameWidth + 2)}{attributes}");
        }
    }

    // «Відсутнє» значення: null або рядок, що складається лише з пробілів.
    // Для int/decimal/enum значення ніколи не null, тому [Required] на них нічого не перевіряє.
    private static bool IsMissing(object? value)
    {
        if (value == null) return true;
        if (value is string text) return string.IsNullOrWhiteSpace(text);
        return false;
    }

    // Якщо в атрибута порожній ErrorMessage — підставляємо запасний текст, щоб помилка не була порожньою.
    private static string MessageOrDefault(string message, string fallback)
    {
        return string.IsNullOrWhiteSpace(message) ? fallback : message;
    }

    // Перетворює на double лише числові типи. Рядок, bool чи об'єкт мовчки пропускаються:
    // Convert.ToDouble("abc") кинув би виняток, а MinValue має сенс тільки для чисел.
    private static bool TryGetNumber(object value, out double number)
    {
        switch (Type.GetTypeCode(value.GetType()))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
                number = Convert.ToDouble(value, CultureInfo.InvariantCulture);
                return true;
            default:
                number = 0;
                return false;
        }
    }

    // «[MinValue] [Required]» — назви атрибутів без суфікса Attribute.
    private static string FormatAttributes(PropertyInfo property)
    {
        string result = "";
        foreach (Attribute attribute in property.GetCustomAttributes())
        {
            Type attributeType = attribute.GetType();

            // Службові атрибути, які додає сам компілятор (наприклад, Nullable), користувачу показувати не потрібно.
            if (attributeType.Namespace == "System.Runtime.CompilerServices") continue;

            string name = attributeType.Name;
            if (name.EndsWith("Attribute") && name.Length > "Attribute".Length)
                name = name.Substring(0, name.Length - "Attribute".Length);

            if (result.Length > 0) result += " ";
            result += "[" + name + "]";
        }
        return result;
    }
}
