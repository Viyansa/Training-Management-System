namespace TrainingManagementSystem.Payments;
public interface IPaymentProcessor
{
    string ModeName { get; }
    (bool Success, string Message) ProcessPayment(int enrollmentId, decimal amount, bool simulateSuccess);
}