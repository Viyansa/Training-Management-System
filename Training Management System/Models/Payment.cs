using System;

namespace TrainingManagementSystem.Models;

// STEP 9 — one payment ATTEMPT. Failures are kept in history too.
public class Payment
{
    public int PaymentId { get; }
    public int EnrollmentId { get; }
    public decimal Amount { get; }
    public DateTime PaymentDate { get; }
    public PaymentStatus Status { get; private set; }
    public string Detail { get; private set; }

    public Payment(int paymentId, int enrollmentId, decimal amount, DateTime paymentDate)
    {
        if (paymentId <= 0)
            throw new ArgumentOutOfRangeException(nameof(paymentId), "Payment ID must be positive.");
        if (enrollmentId <= 0)
            throw new ArgumentOutOfRangeException(nameof(enrollmentId), "Enrollment ID must be positive.");
        if (amount < 0m)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");

        PaymentId = paymentId;
        EnrollmentId = enrollmentId;
        Amount = amount;
        PaymentDate = paymentDate;
        Status = PaymentStatus.Pending;
        Detail = "Attempt recorded, awaiting gateway result.";
    }

    public void MarkSuccessful(string detail)
    {
        Status = PaymentStatus.Successful;
        Detail = detail;
    }

    public void MarkFailed(string reason)
    {
        Status = PaymentStatus.Failed;
        Detail = reason;
    }

    public override string ToString() =>
        $"P{PaymentId,-5} | E{EnrollmentId,-6} | ₹{Amount,10:N2} | {PaymentDate:dd-MMM-yyyy HH:mm} | {Status,-9} | {Detail}";
}