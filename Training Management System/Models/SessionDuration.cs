using System;

namespace TrainingManagementSystem.Models;

// STEP 7/8 — immutable VALUE type. Assigning a struct copies its value;
// two durations with the same days are equal (value semantics).
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