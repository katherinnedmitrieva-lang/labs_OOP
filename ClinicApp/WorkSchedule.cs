using System;

namespace ClinicApp;

public struct WorkSchedule
{
    public int Start { get; }
    public int End { get; }

    public WorkSchedule(int start, int end)
    {
        if (start >= end || start < 0 || end > 24)
        {
            Start = 8;
            End = 17;
        }
        else
        {
            Start = start;
            End = end;
        }
    }

    public int HoursPerDay => End - Start;
    public string Display => $"{Start:D2}:00–{End:D2}:00";
    public bool IsNow => Contains(DateTime.Now.Hour);

    public bool Contains(int hour)
    {
        return hour >= Start && hour < End;
    }

    public override string ToString() => Display;
}
