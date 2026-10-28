using System;
using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Sessions;


public sealed class ClassroomSession : CourseSession
{
    public const decimal SessionCharge = 800m;

    public ClassroomSession(string courseCode, DateTime startDate, SessionDuration duration)
        : base(courseCode, startDate, duration)
    {
    }

    public override decimal CalculateSessionCharge() => SessionCharge;

    public override string Describe() => base.Describe() + " | mode: Classroom";
}