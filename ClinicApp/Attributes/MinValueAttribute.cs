using System;

namespace ClinicApp.Attributes;

// Правило «число не менше за Min». Min має тип double, щоб один атрибут підходив і для int, і для decimal, і для double
// (в атрибутах дозволені лише прості типи, тому decimal тут використати не можна).
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class MinValueAttribute : Attribute
{
    public double Min { get; }
    public string ErrorMessage { get; }

    public MinValueAttribute(double min, string errorMessage = "")
    {
        Min = min;
        ErrorMessage = errorMessage;
    }
}
