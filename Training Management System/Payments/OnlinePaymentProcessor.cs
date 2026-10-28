namespace TrainingManagementSystem.Payments;
public class OnlinePaymentProcessor : IPaymentProcessor
{
    public const decimal TransactionLimit = 100_000m;

    public string ModeName => "Online gateway";

    public (bool Success, string Message) ProcessPayment(int enrollmentId, decimal amount, bool simulateSuccess)
    {
        if (amount <= 0m)
            return (false, $"Invalid amount ₹{amount:N2}.");

        if (amount > TransactionLimit)
            return (false, $"Gateway limit exceeded (max ₹{TransactionLimit:N2}) — payment FAILED.");

        if (!simulateSuccess)
            return (false, "Gateway declined the transaction (simulated failure).");

        return (true, $"Gateway approved ₹{amount:N2} for E{enrollmentId} (simulated success).");
    }
}