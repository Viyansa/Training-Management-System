using System;
using System.Collections.Generic;
using TrainingManagementSystem.Models;
using TrainingManagementSystem.Sessions;
using TrainingManagementSystem.Utilities;

namespace TrainingManagementSystem.Services;

public class EnrollmentService
{
    private readonly List<Enrollment> _enrollments = new List<Enrollment>();

    private readonly TrainingService _trainingService;
    private readonly LearnerService _learnerService;
    private readonly FeeCalculator _feeCalculator;

    private int _nextEnrollmentId = 1000;   

    public EnrollmentService(TrainingService trainingService,
                             LearnerService learnerService,
                             FeeCalculator feeCalculator)
    {
        _trainingService = trainingService;
        _learnerService = learnerService;
        _feeCalculator = feeCalculator;
    }

    public int TotalRecords => _enrollments.Count;    
    public (bool Success, Enrollment? Enrollment, string Message) Enroll(
        string learnerId, string courseCode, DateTime courseStartDate, char mode, int learnersEnrollingTogether)
    {
        if (!_learnerService.TryGet(learnerId, out Learner? learner) || learner is null)
            return (false, null, $"Learner '{learnerId}' is not registered. Register first (menu option 4).");
        Course? course = _trainingService.FindByCode(courseCode);
        if (course is null)
            return (false, null, $"Course '{courseCode}' does not exist.");
        if (HasActiveEnrollment(learnerId, course.CourseCode))
            throw new DuplicateEnrollmentException(
                $"'{learner.Name}' already has an ACTIVE enrollment in {course.CourseCode} " +
                $"(E{FindActiveEnrollmentId(learnerId, course.CourseCode)}). Cancel it first or pick another course.");
        if (course.IsFull)
            return (false, null, $"Course {course.CourseCode} is FULL " +
                                 $"({course.TotalSeats}/{course.TotalSeats} seats taken). Enrollment rejected.");
        CourseSession session = CreateSession(mode, course, courseStartDate);
        decimal surcharge = session.CalculateSessionCharge();          
        bool earlyBird = FeeCalculator.IsEarlyBird(courseStartDate);
        bool group = FeeCalculator.IsGroupEligible(learnersEnrollingTogether);
        var fee = _feeCalculator.CalculateFee(course.Fee, earlyBird, group, surcharge);
        var enrollment = new Enrollment(
            GetNextId(ref _nextEnrollmentId),                         
            learnerId, course.CourseCode, DateTime.Now,
            fee.BaseFee, fee.DiscountAmount, fee.FinalFee);
        _enrollments.Add(enrollment);

       
        course.IncrementEnrolled();

       
        return (true, enrollment,
            $"E{enrollment.EnrollmentId}: {learner.Name} → {course.Title} ({session.Describe()}). " +
            $"Early-bird: {(earlyBird ? "YES (10%)" : "no")}, Group: {(group ? "YES (5%)" : "no")}.");
    }

   
    public (bool Success, string Message) Cancel(int enrollmentId)
    {
       
        if (!TryGetEnrollment(enrollmentId, out Enrollment? enrollment) || enrollment is null)
            return (false, $"Enrollment E{enrollmentId} was not found.");

        
        if (enrollment.Status == EnrollmentStatus.Cancelled)
            return (false, $"E{enrollmentId} is ALREADY cancelled — seat count unchanged (repeat-cancel protected).");

        Course? course = _trainingService.FindByCode(enrollment.CourseCode);

        enrollment.MarkCancelled();                
        course?.DecrementEnrolled();               
        return (true, $"E{enrollmentId} ({enrollment.CourseCode}) cancelled. One seat released. Record kept for history.");
    }

   
    public bool HasActiveEnrollment(string learnerId, string courseCode) =>
        FindActiveEnrollmentId(learnerId, courseCode) != -1;

    private int FindActiveEnrollmentId(string learnerId, string courseCode)
    {
        foreach (Enrollment e in _enrollments)
        {
            if (e.Status == EnrollmentStatus.Active &&
                string.Equals(e.LearnerId, learnerId, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(e.CourseCode, courseCode, StringComparison.OrdinalIgnoreCase))
                return e.EnrollmentId;
        }
        return -1;
    }

    public bool TryGetEnrollment(int enrollmentId, out Enrollment? enrollment)
    {
        
        enrollment = CollectionHelper.Find(_enrollments, e => e.EnrollmentId == enrollmentId);
        return enrollment is not null;
    }

    public IReadOnlyList<Enrollment> GetAll() => _enrollments;

    public List<Enrollment> GetActiveEnrollments()
    {
        var active = new List<Enrollment>();
        foreach (Enrollment e in _enrollments)
            if (e.Status == EnrollmentStatus.Active)
                active.Add(e);
        return active;
    }

    
    public CourseSession CreateSession(char mode, Course course, DateTime startDate)
    {
        var duration = new SessionDuration(course.DurationDays);
        return mode switch                                     
        {
            'o' => new OnlineSession(course.CourseCode, startDate, duration),
            'c' => new ClassroomSession(course.CourseCode, startDate, duration),
            _ => throw new ArgumentException("Mode must be 'o' (online) or 'c' (classroom).", nameof(mode))
        };
    }

    
    private static int GetNextId(ref int counter) => ++counter;
}