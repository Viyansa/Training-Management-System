using System;

namespace TrainingManagementSystem.Models;

// STEP 3 — one enrollment record. Cancelled records are NEVER deleted;
// they stay in the List<Enrollment> for history.
public class Enrollment
{
    public int EnrollmentId { get; }
    public string LearnerId { get; }
    public string CourseCode { get; }
    public DateTime EnrollmentDate { get; }
    public decimal BaseFee { get; }
    public decimal DiscountAmount { get; }
    public decimal FinalFee { get; }
    public EnrollmentStatus Status { get; private set; }
    public bool IsPaid { get; private set; }    // flipped ONLY by PaymentService after a SUCCESSFUL payment

    public Enrollment(int enrollmentId, string learnerId, string courseCode, DateTime enrollmentDate,
                      decimal baseFee, decimal discountAmount, decimal finalFee)
    {
        if (enrollmentId <= 0)
            throw new ArgumentOutOfRangeException(nameof(enrollmentId), "Enrollment ID must be positive.");
        if (string.IsNullOrWhiteSpace(learnerId))
            throw new ArgumentException("Learner ID is required.", nameof(learnerId));
        if (string.IsNullOrWhiteSpace(courseCode))
            throw new ArgumentException("Course code is required.", nameof(courseCode));
        if (baseFee < 0m) throw new ArgumentOutOfRangeException(nameof(baseFee), "Base fee cannot be negative.");
        if (discountAmount < 0m) throw new ArgumentOutOfRangeException(nameof(discountAmount), "Discount cannot be negative.");
        if (finalFee < 0m) throw new ArgumentOutOfRangeException(nameof(finalFee), "Final fee cannot be negative.");

        EnrollmentId = enrollmentId;
        LearnerId = learnerId;
        CourseCode = courseCode;
        EnrollmentDate = enrollmentDate;
        BaseFee = baseFee;
        DiscountAmount = discountAmount;
        FinalFee = finalFee;
        Status = EnrollmentStatus.Active;
    }

    public void MarkCancelled()
    {
        if (Status == EnrollmentStatus.Cancelled)
            throw new InvalidOperationException($"E{EnrollmentId} is already cancelled.");
        Status = EnrollmentStatus.Cancelled;
    }

    public void MarkPaid() => IsPaid = true;

    public override string ToString() =>
        $"E{EnrollmentId,-5} | {LearnerId,-8} | {CourseCode,-6} | {EnrollmentDate:dd-MMM-yyyy} | " +
        $"base {BaseFee,10:N2} | disc {DiscountAmount,9:N2} | final {FinalFee,10:N2} | " +
        $"{Status,-9} | {(IsPaid ? "PAID" : "UNPAID")}";
}

// STEP 13 (item 49) — custom exception for a meaningful BUSINESS-RULE violation:
// enrolling a learner who already has an ACTIVE enrollment in the same course.
// Ordinary input validation (empty text, bad numbers) stays with conditions/TryParse.
public class DuplicateEnrollmentException : Exception
{
    public DuplicateEnrollmentException(string message) : base(message) { }
}