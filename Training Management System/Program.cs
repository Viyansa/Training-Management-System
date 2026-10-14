using System;
using TrainingManagementSystem.Payments;
using TrainingManagementSystem.Utilities;
using TrainingManagementSystem.Payments;
using TrainingManagementSystem.Services;
using TrainingManagementSystem.Utilities;

namespace TrainingManagementSystem;

internal class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;   // so the ₹ symbol renders
        var trainingService = new TrainingService();            // owns Course[]
        var learnerService = new LearnerService();             // owns Dictionary<string, Learner>
        var feeCalculator = new FeeCalculator();              // stateless fee rules
        var enrollmentService = new EnrollmentService(trainingService, learnerService, feeCalculator); // owns List<Enrollment>
        IPaymentProcessor onlineProcessor = new OnlinePaymentProcessor();      // implicit interface impl
        IPaymentProcessor classroomProcessor = new ClassroomPaymentProcessor();  // explicit interface impl
        var paymentService = new PaymentService(enrollmentService, onlineProcessor, classroomProcessor); // owns List<Payment>
        var reportService = new ReportService(trainingService, enrollmentService, paymentService);

        var menu = new MenuHandler(trainingService, learnerService, enrollmentService,
                                   feeCalculator, paymentService, reportService);

        try
        {
            menu.Run();                                   // STEP 11 menu loop
        }
        catch (Exception ex)                              // STEP 13 (47): last-resort safety net
        {
            Console.WriteLine($"\n[UNEXPECTED ERROR] {ex.Message}");
            Console.WriteLine(ex.ToString());
        }
        finally                                           // STEP 13 (48): always runs
        {
            Console.WriteLine("\nTraining Management System closed (in-memory data is not persisted).");
        }
    }
}