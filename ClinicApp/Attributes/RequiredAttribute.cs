using System;

namespace ClinicApp.Attributes;

// Правило «поле обов'язкове». Сам атрибут нічого не перевіряє — це лише МЕТАДАНІ,
// які під час виконання зчитує ModelValidator (через рефлексію) і форма FormBuilder.
// AttributeTargets.Property — ставити можна лише на властивості; AllowMultiple = false — не більше одного разу.
// sealed — від атрибута не потрібно успадковуватись.
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class RequiredAttribute : Attribute
{
    // Лише get: параметри правила задаються при застосуванні атрибута і далі не змінюються.
    public string ErrorMessage { get; }

    public RequiredAttribute(string errorMessage = "Field is required.")
    {
        ErrorMessage = errorMessage;
    }
}
