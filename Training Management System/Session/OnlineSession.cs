using System;
using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Sessions;

// STEP 7 — online delivery: ₹200 surcharge (Step 6 fee table).
public sealed class OnlineSession : CourseSession
{
    public const decimal SessionCharge = 200m;    // single source of truth for this amount

    public OnlineSession(string courseCode, DateTime startDate, SessionDuration duration)
        : base(courseCode, startDate, duration)   // base(...) initializes the abstract class
    {
    }

    public override decimal CalculateSessionCharge() => SessionCharge;

    public override string Describe() => base.Describe() + " | mode: Online";
}