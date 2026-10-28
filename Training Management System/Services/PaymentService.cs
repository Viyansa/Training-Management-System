using System;
using System.Collections.Generic;
using TrainingManagementSystem.Models;
using TrainingManagementSystem.Payments;

namespace TrainingManagementSystem.Services;


public class PaymentService
{
    
    private readonly List<Payment> _payments = new List<Payment>();

    private readonly EnrollmentService _enrollmentService;
    private readonly IPaymentProcessor _onlineProcessor;       
    private readonly IPaymentProcessor _classroomProcessor;    

    private int _nextPaymentId = 5000;                         

    public PaymentService(EnrollmentService enrollmentService,
                          IPaymentProcessor onlineProcessor,
                          IPaymentProcessor classroomProcessor)
    {
        _enrollmentService = enrollmentService;
        _onlineProcessor = onlineProcessor;
        _classroomProcessor = classroomProcessor;
    }

    public int TotalAttempts => _payments.Count;

    public IPaymentProcessor? GetProcessor(char mode) => mode switch   
    {
        'o' => _onlineProcessor,
        'c' => _classroomProcessor,
        _ => null
    };

    public (bool Success, string Message) Pay(int enrollmentId, IPaymentProcessor processor, bool simulateSuccess)
    {
        
        if (!_enrollmentService.TryGetEnrollment(enrollmentId, out Enrollment? enrollment) || enrollment is null)
            return (false, $"Enrollment E{enrollmentId} was not found.");

        
        if (enrollment.Status == EnrollmentStatus.Cancelled)
            return (false, "Payment rejected — this enrollment is CANCELLED.");

        if (enrollment.Status == EnrollmentStatus.Completed)
            return (false, "Payment rejected — this enrollment is already COMPLETED.");

        
        if (enrollment.IsPaid)
            return (false, $"E{enrollmentId} is already paid (₹{enrollment.FinalFee:N2}).");

        
        var payment = new Payment(GetNextId(ref _nextPaymentId), enrollmentId, enrollment.FinalFee, DateTime.Now);
        _payments.Add(payment);

        
        var (success, message) = processor.ProcessPayment(enrollmentId, enrollment.FinalFee, simulateSuccess);

        if (success)
        {
            payment.MarkSuccessful(message);
            enrollment.MarkPaid();                 
        }
        else
        {
            payment.MarkFailed(message);           
        }

        return (success, $"[{processor.ModeName}] {message} (payment ref P{payment.PaymentId})");
    }

    public IReadOnlyList<Payment> GetAll() => _payments;

   
    private static int GetNextId(ref int counter) => ++counter;
}