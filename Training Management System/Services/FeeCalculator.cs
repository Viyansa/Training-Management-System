using System;
using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Services;

// STEP 6 — read-only VALUE snapshot of one fee calculation.
// Passed to display methods with the 'in' modifier (by reference, read-only).
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

    // Rebuilds a summary from a stored enrollment (surcharge = final − base + discount).
    public static FeeSummary FromEnrollment(Enrollment enrollment) =>
        new(enrollment.BaseFee,
            enrollment.DiscountAmount,
            enrollment.FinalFee - enrollment.BaseFee + enrollment.DiscountAmount,
            enrollment.FinalFee);
}

// STEP 6 & 10 — all fee rules in one place.
// NOTE: the ₹200/₹800 surcharge AMOUNTS live in OnlineSession/ClassroomSession
// (single source of truth) and reach this class through polymorphism.
public class FeeCalculator
{
    public const decimal EarlyBirdDiscountPercent = 10m;
    public const decimal GroupDiscountPercent = 5m;
    public const int EarlyBirdDaysAhead = 14;
    public const int GroupSizeMinimum = 3;

    // ---- Overload 1 (item 25): base fee only, no discounts ----
    public (decimal BaseFee, decimal DiscountAmount, decimal FinalFee) CalculateFee(decimal baseFee)
    {
        EnsureNonNegative(baseFee, nameof(baseFee));
        return (baseFee, 0m, baseFee);
    }

    // ---- Overload 2: base fee + session surcharge ----
    public (decimal BaseFee, decimal DiscountAmount, decimal FinalFee) CalculateFee(decimal baseFee, decimal sessionSurcharge)
    {
        var (b, d, f) = CalculateFee(baseFee);
        EnsureNonNegative(sessionSurcharge, nameof(sessionSurcharge));
        return (b, d, f + sessionSurcharge);
    }

    // ---- Overload 3 (item 26): full rules ----
    // Both discounts are computed against the BASE fee and simply added together
    // (not compounded), then the session surcharge is added (item 28 + fee table).
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
        return (baseFee, discount, finalFee);              // named tuple (item 27)
    }

    public static bool IsEarlyBird(DateTime courseStartDate) =>
        (courseStartDate.Date - DateTime.Today).TotalDays >= EarlyBirdDaysAhead;

    public static bool IsGroupEligible(int learnersEnrollingTogether) =>
        learnersEnrollingTogether >= GroupSizeMinimum;

    // 'in' — passes the read-only struct BY REFERENCE (no copy, cannot be mutated).
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