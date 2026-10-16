using System;
using System.Globalization;
using ClinicApp.Utils;

namespace ClinicApp.Models;

public class LabResult : MedicalRecord
{
    private string _testName = "";
    private string _unit = "";
    private string _referenceRange = "";

    public string TestName
    {
        get => _testName;
        set
        {
            ClinicValidator.ValidateName(value, nameof(TestName));
            _testName = value;
        }
    }

    public string Unit
    {
        get => _unit;
        set
        {
            ClinicValidator.ValidateName(value, nameof(Unit));
            _unit = value;
        }
    }

    public string ReferenceRange
    {
        get => _referenceRange;
        set
        {
            ClinicValidator.ValidateName(value, nameof(ReferenceRange));
            _referenceRange = value;
        }
    }

    public double Value { get; set; }
    public bool IsNormal { get; set; }

    public LabResult(int patientId, int doctorId, DateTime date,
                     string testName, double value, string unit,
                     string referenceRange, bool isNormal)
        : base(patientId, doctorId, date)
    {
        TestName = testName;
        Value = value;
        Unit = unit;
        ReferenceRange = referenceRange;
        IsNormal = isNormal;
    }

    public override string GetSummary()
    {
        string valueText = Value.ToString(CultureInfo.InvariantCulture);
        string summary = $"{TestName}: {valueText} {Unit} (норма: {ReferenceRange})";
        if (!IsNormal) summary += " ⚠ поза нормою";
        return summary;
    }

    public override string GetRecordType()
    {
        return "Аналіз";
    }
}