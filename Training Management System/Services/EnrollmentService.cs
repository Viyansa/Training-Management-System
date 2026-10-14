using System;
using System.Collections.Generic;
using TrainingManagementSystem.Models;
using TrainingManagementSystem.Sessions;
using TrainingManagementSystem.Utilities;

namespace TrainingManagementSystem.Services;

public class EnrollmentService
{
    // STEP 3 collection: grows dynamically; cancelled records are KEPT.
    // (List.Remove is deliberately NOT used — history must be preserved.)
    private readonly List<Enrollment> _enrollments = new List<Enrollment>();

    private readonly TrainingService _trainingService;
    private readonly LearnerService _learnerService;
    private readonly FeeCalculator _feeCalculator;

    private int _nextEnrollmentId = 1000;      // first real ID will be 1001

    public EnrollmentService(TrainingService trainingService,
                             LearnerService learnerService,
                             FeeCalculator feeCalculator)
    {
        _trainingService = trainingService;
        _learnerService = learnerService;
        _feeCalculator = feeCalculator;
    }

    public int TotalRecords => _enrollments.Count;     // List.Count

    // ---------------- STEP 4: enrollment ----------------
    public (bool Success, Enrollment? Enrollment, string Message) Enroll(
        string learnerId, string courseCode, DateTime courseStartDate, char mode, int learnersEnrollingTogether)
    {
        // (12) learner must exist
        if (!_learnerService.TryGet(learnerId, out Learner? learner) || learner is null)
            return (false, null, $"Learner '{learnerId}' is not registered. Register first (menu option 4).");

        // (13) course must exist
        Course? course = _trainingService.FindByCode(courseCode);
        if (course is null)
            return (false, null, $"Course '{courseCode}' does not exist.");

        // (14) duplicate ACTIVE enrollment is a business-rule violation.
        // Cancelled enrollments do NOT count (we look only at Active).
        if (HasActiveEnrollment(learnerId, course.CourseCode))
            throw new DuplicateEnrollmentException(
                $"'{learner.Name}' already has an ACTIVE enrollment in {course.CourseCode} " +
                $"(E{FindActiveEnrollmentId(learnerId, course.CourseCode)}). Cancel it first or pick another course.");

        // (15) seats must be available — rejected WITHOUT touching seat counts
        if (course.IsFull)
            return (false, null, $"Course {course.CourseCode} is FULL " +
                                 $"({course.TotalSeats}/{course.TotalSeats} seats taken). Enrollment rejected.");

        // (16) fee — the session object supplies the delivery surcharge (Step 7 polymorphism)
        CourseSession session = CreateSession(mode, course, courseStartDate);
        decimal surcharge = session.CalculateSessionCharge();          // runtime polymorphism
        bool earlyBird = FeeCalculator.IsEarlyBird(courseStartDate);
        bool group = FeeCalculator.IsGroupEligible(learnersEnrollingTogether);
        var fee = _feeCalculator.CalculateFee(course.Fee, earlyBird, group, surcharge);

        // (17) create + store the record
        var enrollment = new Enrollment(
            GetNextId(ref _nextEnrollmentId),                          // 'ref' counter update (Step 6)
            learnerId, course.CourseCode, DateTime.Now,
            fee.BaseFee, fee.DiscountAmount, fee.FinalFee);
        _enrollments.Add(enrollment);

        // (18) increment ONLY after every check succeeded
        course.IncrementEnrolled();

        // (19) confirmation
        return (true, enrollment,
            $"E{enrollment.EnrollmentId}: {learner.Name} → {course.Title} ({session.Describe()}). " +
            $"Early-bird: {(earlyBird ? "YES (10%)" : "no")}, Group: {(group ? "YES (5%)" : "no")}.");
    }

    // ---------------- STEP 5: cancellation ----------------
    public (bool Success, string Message) Cancel(int enrollmentId)
    {
        // (20) find the record
        if (!TryGetEnrollment(enrollmentId, out Enrollment? enrollment) || enrollment is null)
            return (false, $"Enrollment E{enrollmentId} was not found.");

        // (21) reject missing OR already-cancelled records — count changes never repeat
        if (enrollment.Status == EnrollmentStatus.Cancelled)
            return (false, $"E{enrollmentId} is ALREADY cancelled — seat count unchanged (repeat-cancel protected).");

        Course? course = _trainingService.FindByCode(enrollment.CourseCode);

        enrollment.MarkCancelled();                // (22) status change; record kept for history
        course?.DecrementEnrolled();               // (23) releases exactly ONE seat

        return (true, $"E{enrollmentId} ({enrollment.CourseCode}) cancelled. One seat released. Record kept for history.");
    }

    // ---------------- lookups (loops only, no LINQ) ----------------
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
        // Generic search (Step 13 item 45) instead of a hand-written loop here.
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

    // Public so the menu can run 'what-if' fee breakdowns WITHOUT enrolling anyone.
    public CourseSession CreateSession(char mode, Course course, DateTime startDate)
    {
        var duration = new SessionDuration(course.DurationDays);
        return mode switch                                     // switch expression (Step 11)
        {
            'o' => new OnlineSession(course.CourseCode, startDate, duration),
            'c' => new ClassroomSession(course.CourseCode, startDate, duration),
            _ => throw new ArgumentException("Mode must be 'o' (online) or 'c' (classroom).", nameof(mode))
        };
    }

    // 'ref' — the counter is updated through a by-reference parameter (Step 6).
    private static int GetNextId(ref int counter) => ++counter;
}