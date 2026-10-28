using System;
using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Services;

public class ReportService
{
    private readonly TrainingService _trainingService;
    private readonly EnrollmentService _enrollmentService;
    private readonly PaymentService _paymentService;

    public ReportService(TrainingService trainingService,
                         EnrollmentService enrollmentService,
                         PaymentService paymentService)
    {
        _trainingService = trainingService;
        _enrollmentService = enrollmentService;
        _paymentService = paymentService;
    }

    
    public void PrintCourseOccupancy()
    {
        Console.WriteLine("\n=== REPORT A: COURSE OCCUPANCY ===");
        Console.WriteLine($"{"Code",-8} {"Title",-30} {"Total",6} {"Occupied",9} {"Free",6} {"Occupied %",11}");
        Console.WriteLine(new string('-', 74));

        foreach (Course c in _trainingService.GetAllCourses())
        {
            decimal pct = c.TotalSeats == 0
                ? 0m
                : (decimal)c.EnrolledCount * 100m / c.TotalSeats;   
            Console.WriteLine($"{c.CourseCode,-8} {c.Title,-30} {c.TotalSeats,6} " +
                              $"{c.EnrolledCount,9} {c.AvailableSeats,6} {pct,10:F1}%");
        }
    }

    
    public void PrintExpectedFees()
    {
        Console.WriteLine("\n=== REPORT B: EXPECTED FEES (ACTIVE ENROLLMENTS) ===");
        decimal total = 0m, alreadyPaid = 0m;
        int activeCount = 0, paidCount = 0, pendingCount = 0;

        foreach (Enrollment e in _enrollmentService.GetAll())
        {
            if (e.Status != EnrollmentStatus.Active)
                continue;                                          
            activeCount++;
            total += e.FinalFee;                                   
            if (e.IsPaid)
            {
                paidCount++;
                alreadyPaid += e.FinalFee;
            }
            else
            {
                pendingCount++;
            }
        }

        Console.WriteLine($"Active enrollments  : {activeCount}");
        Console.WriteLine($"Expected total fees : ₹{total:N2} (paid + pending)");
        Console.WriteLine($"  Already collected : ₹{alreadyPaid:N2} ({paidCount} paid)");
        Console.WriteLine($"  Still pending     : ₹{total - alreadyPaid:N2} ({pendingCount} awaiting payment)");
    }

    // ---- Report C: payment summary ----
    public void PrintPaymentSummary()
    {
        Console.WriteLine("\n=== REPORT C: PAYMENT SUMMARY ===");
        int successful = 0, failed = 0, pending = 0;
        decimal collected = 0m;

        foreach (Payment p in _paymentService.GetAll())
        {
            switch (p.Status)
            {
                case PaymentStatus.Successful:
                    successful++;
                    collected += p.Amount;
                    break;
                case PaymentStatus.Failed:
                    failed++;
                    break;
                default:
                    pending++;
                    break;
            }
        }

        Console.WriteLine($"Successful attempts : {successful}");
        Console.WriteLine($"Failed attempts     : {failed}");
        Console.WriteLine($"Pending attempts    : {pending}");
        Console.WriteLine($"Total collected     : ₹{collected:N2} (from successful payments only)");
    }

    // ---- Report D: course enrollment summary ----
    public void PrintCourseEnrollmentSummary()
    {
        Console.WriteLine("\n=== REPORT D: COURSE ENROLLMENT SUMMARY ===");
        int fullCourses = 0;

        foreach (Course c in _trainingService.GetAllCourses())
        {
            int active = 0;
            foreach (Enrollment e in _enrollmentService.GetAll())       
            {
                if (e.Status == EnrollmentStatus.Active &&
                    string.Equals(e.CourseCode, c.CourseCode, StringComparison.OrdinalIgnoreCase))
                    active++;
            }

            bool full = c.AvailableSeats == 0;
            if (full) fullCourses++;

            Console.WriteLine($"{c.CourseCode,-8} {c.Title,-30} active: {active,3} / {c.TotalSeats,3} seats" +
                              $"{(full ? "   << FULL — no remaining seats" : "")}");
        }

        Console.WriteLine($"Courses with no remaining seats: {fullCourses}");
    }
}