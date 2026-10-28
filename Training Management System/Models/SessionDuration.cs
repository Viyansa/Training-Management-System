using System;

namespace TrainingManagementSystem.Models;
public readonly struct SessionDuration
{
    public int Days { get; }

    public SessionDuration(int days)
    {
        if (days <= 0)
            throw new ArgumentOutOfRangeException(nameof(days), "Duration must be at least 1 day.");
        Days = days;
    }

    public SessionDuration Extend(int extraDays) => new SessionDuration(Days + extraDays);

    public override string ToString() => $"{Days} day(s)";
}