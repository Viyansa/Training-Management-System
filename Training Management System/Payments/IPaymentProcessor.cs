namespace TrainingManagementSystem.Payments;

// STEP 9 — abstraction for simulated payment channels. No real money, ever.
public interface IPaymentProcessor
{
    string ModeName { get; }

    // simulateSuccess lets the operator rehearse BOTH outcomes deterministically.
    (bool Success, string Message) ProcessPayment(int enrollmentId, decimal amount, bool simulateSuccess);
}