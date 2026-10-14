using System;
using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Sessions;

// STEP 7 — classroom delivery: ₹800 surcharge (Step 6 fee table).
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