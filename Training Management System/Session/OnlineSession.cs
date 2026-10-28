using System;
using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Sessions;


public sealed class OnlineSession : CourseSession
{
    public const decimal SessionCharge = 200m;    

    public OnlineSession(string courseCode, DateTime startDate, SessionDuration duration)
        : base(courseCode, startDate, duration)   
    {
    }

    public override decimal CalculateSessionCharge() => SessionCharge;

    public override string Describe() => base.Describe() + " | mode: Online";
}