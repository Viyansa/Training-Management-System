
using System;
using System.Text;
using Microsoft.Data.SqlClient;
using TrainingManagementSystem.Payments;
using TrainingManagementSystem.Utilities;
using TrainingManagementSystem.Services;
using TrainingManagementSystem.FileHandling;

namespace TrainingManagementSystem;

internal class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        try
        {
            string connectionString =
                @"Server=.\SQLEXPRESS;Database=TrainingManagementDB;Integrated Security=True;TrustServerCertificate=True;";

            using (SqlConnection connection =
                   new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Database connected successfully!");
            }

            var fileHandler = new LearnerFileHandler();

            Console.WriteLine("=== SAVED LEARNER DETAILS ===");
            fileHandler.DisplayLearners();

            var trainingService = new TrainingService();
            var learnerService = new LearnerService();
            var feeCalculator = new FeeCalculator();

            var enrollmentService = new EnrollmentService(
                trainingService,
                learnerService,
                feeCalculator
            );

            IPaymentProcessor onlineProcessor =
                new OnlinePaymentProcessor();

            IPaymentProcessor classroomProcessor =
                new ClassroomPaymentProcessor();

            var paymentService = new PaymentService(
                enrollmentService,
                onlineProcessor,
                classroomProcessor
            );

            var reportService = new ReportService(
                trainingService,
                enrollmentService,
                paymentService
            );

            var menu = new MenuHandler(
                trainingService,
                learnerService,
                enrollmentService,
                feeCalculator,
                paymentService,
                reportService
            );

            menu.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"\n[ERROR] {ex.Message}"
            );
        }
        finally
        {
            Console.WriteLine(
                "\nTraining Management System closed."
            );
        }
    }
}
