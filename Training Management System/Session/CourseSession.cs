using System;
using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Sessions;

// STEP 7 — abstract base for a delivered session. Subclasses decide how the
// session charge is computed (runtime polymorphism).
public abstract class CourseSession
{
    public string CourseCode { get; }
    public DateTime StartDate { get; }
    public SessionDuration Duration { get; }

    protected CourseSession(string courseCode, DateTime startDate, SessionDuration duration)
    {
        if (string.IsNullOrWhiteSpace(courseCode))
            throw new ArgumentException("Course code is required.", nameof(courseCode));

        CourseCode = courseCode;
        StartDate = startDate.Date;
        Duration = duration;
    }

    // Every mode charges differently — subclasses MUST override this.
    public abstract decimal CalculateSessionCharge();

    // Virtual so subclasses extend the description while reusing base behaviour.
    public virtual string Describe() =>
        $"{CourseCode} | starts {StartDate:dd-MMM-yyyy} | {Duration}";

    public override string ToString() => Describe();
}