using System;
using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Sessions;


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

    
    public abstract decimal CalculateSessionCharge();

    
    public virtual string Describe() =>
        $"{CourseCode} | starts {StartDate:dd-MMM-yyyy} | {Duration}";

    public override string ToString() => Describe();
}