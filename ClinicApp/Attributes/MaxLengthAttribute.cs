using System;

namespace ClinicApp.Attributes;

// Правило «рядок не довший за Length символів».
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class MaxLengthAttribute : Attribute
{
    public int Length { get; }
    public string ErrorMessage { get; }

    public MaxLengthAttribute(int length, string errorMessage = "")
    {
        Length = length;
        ErrorMessage = errorMessage;
    }
}
