using System;
using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Services;

public readonly struct FeeSummary
{
    public decimal BaseFee { get; }
    public decimal DiscountAmount { get; }
    public decimal SessionSurcharge { get; }
    public decimal FinalFee { get; }

    public FeeSummary(decimal baseFee, decimal discountAmount, decimal sessionSurcharge, decimal finalFee)
    {
        if (baseFee < 0m) throw new ArgumentOutOfRangeException(nameof(baseFee), "Base fee cannot be negative.");
        if (discountAmount < 0m) throw new ArgumentOutOfRangeException(nameof(discountAmount), "Discount cannot be negative.");
        if (sessionSurcharge < 0m) throw new ArgumentOutOfRangeException(nameof(sessionSurcharge), "Surcharge cannot be negative.");
        if (finalFee < 0m) throw new ArgumentOutOfRangeException(nameof(finalFee), "Final fee cannot be negative.");

        BaseFee = baseFee;
        DiscountAmount = discountAmount;
        SessionSurcharge = sessionSurcharge;
        FinalFee = finalFee;
    }

    
    public static FeeSummary FromEnrollment(Enrollment enrollment) =>
        new(enrollment.BaseFee,
            enrollment.DiscountAmount,
            enrollment.FinalFee - enrollment.BaseFee + enrollment.DiscountAmount,
            enrollment.FinalFee);
}
public class FeeCalculator
{
    public const decimal EarlyBirdDiscountPercent = 10m;
    public const decimal GroupDiscountPercent = 5m;
    public const int EarlyBirdDaysAhead = 14;
    public const int GroupSizeMinimum = 3;

    
    public (decimal BaseFee, decimal DiscountAmount, decimal FinalFee) CalculateFee(decimal baseFee)
    {
        EnsureNonNegative(baseFee, nameof(baseFee));
        return (baseFee, 0m, baseFee);
    }

   
    public (decimal BaseFee, decimal DiscountAmount, decimal FinalFee) CalculateFee(decimal baseFee, decimal sessionSurcharge)
    {
        var (b, d, f) = CalculateFee(baseFee);
        EnsureNonNegative(sessionSurcharge, nameof(sessionSurcharge));
        return (b, d, f + sessionSurcharge);
    }

    public (decimal BaseFee, decimal DiscountAmount, decimal FinalFee) CalculateFee(
        decimal baseFee, bool earlyBirdEligible, bool groupEligible, decimal sessionSurcharge)
    {
        EnsureNonNegative(baseFee, nameof(baseFee));
        EnsureNonNegative(sessionSurcharge, nameof(sessionSurcharge));

        decimal discount = 0m;
        if (earlyBirdEligible) discount += baseFee * EarlyBirdDiscountPercent / 100m;
        if (groupEligible) discount += baseFee * GroupDiscountPercent / 100m;

        if (discount < 0m) throw new InvalidOperationException("Total discount cannot be negative.");

        decimal finalFee = baseFee - discount + sessionSurcharge;
        return (baseFee, discount, finalFee);              
    }

    public static bool IsEarlyBird(DateTime courseStartDate) =>
        (courseStartDate.Date - DateTime.Today).TotalDays >= EarlyBirdDaysAhead;

    public static bool IsGroupEligible(int learnersEnrollingTogether) =>
        learnersEnrollingTogether >= GroupSizeMinimum;

   
    public string FormatBreakdown(in FeeSummary summary)
    {
        return string.Join(Environment.NewLine, new[]
        {
            "  ------------------- FEE BREAKDOWN -------------------",
            $"  Base fee           : ₹{summary.BaseFee,12:N2}",
            $"  Total discount     : -₹{summary.DiscountAmount,12:N2}",
            $"  Session surcharge  : +₹{summary.SessionSurcharge,12:N2}",
            "  -----------------------------------------------------",
            $"  FINAL PAYABLE      : ₹{summary.FinalFee,12:N2}",
        });
    }

    private static void EnsureNonNegative(decimal amount, string paramName)
    {
        if (amount < 0m)
            throw new ArgumentOutOfRangeException(paramName, "Money values cannot be negative.");
    }
}