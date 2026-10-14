using System;
using System.Collections.Generic;
using TrainingManagementSystem.Models;
using TrainingManagementSystem.Payments;

namespace TrainingManagementSystem.Services;

// STEP 9 & 10 — coordinates simulated payments. Owns the ONE List<Payment>.
public class PaymentService
{
    // Every attempt (success OR failure) is preserved here.
    private readonly List<Payment> _payments = new List<Payment>();

    private readonly EnrollmentService _enrollmentService;
    private readonly IPaymentProcessor _onlineProcessor;       // interface references only
    private readonly IPaymentProcessor _classroomProcessor;    // (explicit impl REQUIRES this anyway)

    private int _nextPaymentId = 5000;                         // first real ID will be 5001

    public PaymentService(EnrollmentService enrollmentService,
                          IPaymentProcessor onlineProcessor,
                          IPaymentProcessor classroomProcessor)
    {
        _enrollmentService = enrollmentService;
        _onlineProcessor = onlineProcessor;
        _classroomProcessor = classroomProcessor;
    }

    public int TotalAttempts => _payments.Count;

    public IPaymentProcessor? GetProcessor(char mode) => mode switch   // switch expression
    {
        'o' => _onlineProcessor,
        'c' => _classroomProcessor,
        _ => null
    };

    public (bool Success, string Message) Pay(int enrollmentId, IPaymentProcessor processor, bool simulateSuccess)
    {
        // Reject missing enrollments before anything else.
        if (!_enrollmentService.TryGetEnrollment(enrollmentId, out Enrollment? enrollment) || enrollment is null)
            return (false, $"Enrollment E{enrollmentId} was not found.");

        // Reject payment for CANCELLED enrollments (spec, Step 9).
        if (enrollment.Status == EnrollmentStatus.Cancelled)
            return (false, "Payment rejected — this enrollment is CANCELLED.");

        if (enrollment.Status == EnrollmentStatus.Completed)
            return (false, "Payment rejected — this enrollment is already COMPLETED.");

        // No duplicate charging.
        if (enrollment.IsPaid)
            return (false, $"E{enrollmentId} is already paid (₹{enrollment.FinalFee:N2}).");

        // Record the ATTEMPT first — failures stay in history too.
        var payment = new Payment(GetNextId(ref _nextPaymentId), enrollmentId, enrollment.FinalFee, DateTime.Now);
        _payments.Add(payment);

        // Both implementations are invoked through the interface reference.
        var (success, message) = processor.ProcessPayment(enrollmentId, enrollment.FinalFee, simulateSuccess);

        if (success)
        {
            payment.MarkSuccessful(message);
            enrollment.MarkPaid();                 // payment status updated ONLY after success
        }
        else
        {
            payment.MarkFailed(message);           // enrollment NOT marked paid
        }

        return (success, $"[{processor.ModeName}] {message} (payment ref P{payment.PaymentId})");
    }

    public IReadOnlyList<Payment> GetAll() => _payments;

    // 'ref' counter update (Step 6 practise).
    private static int GetNextId(ref int counter) => ++counter;
}