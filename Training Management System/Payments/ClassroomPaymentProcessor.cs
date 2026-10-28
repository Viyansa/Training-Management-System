namespace TrainingManagementSystem.Payments;
public class ClassroomPaymentProcessor : IPaymentProcessor
{
    public string ModeName => "Classroom counter (cash)";

    (bool Success, string Message) IPaymentProcessor.ProcessPayment(int enrollmentId, decimal amount, bool simulateSuccess)
    {
        if (amount <= 0m)
            return (false, $"Invalid amount ₹{amount:N2}.");

        if (amount % 1m != 0m)
            return (false, "Counter accepts whole rupees only (no paise) — payment FAILED.");

        if (!simulateSuccess)
            return (false, "Cash payment aborted at the counter (simulated failure).");

        return (true, $"Counter received ₹{amount:N2} in cash for E{enrollmentId} (simulated success).");
    }
}