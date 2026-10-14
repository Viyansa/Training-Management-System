using System;

namespace TrainingManagementSystem.Models;

// STEP 1 — one training course. EnrolledCount changes ONLY through
// IncrementEnrolled()/DecrementEnrolled() so it can never drift out of sync.
public class Course
{
    public string CourseCode { get; }
    public string Title { get; }
    public decimal Fee { get; }
    public int DurationDays { get; }
    public int TotalSeats { get; }
    public int EnrolledCount { get; private set; }

    public Course(string courseCode, string title, decimal fee, int durationDays, int totalSeats)
    {
        if (string.IsNullOrWhiteSpace(courseCode))
            throw new ArgumentException("Course code is required.", nameof(courseCode));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Course title is required.", nameof(title));
        if (fee < 0m) throw new ArgumentOutOfRangeException(nameof(fee), "Fee cannot be negative.");
        if (durationDays <= 0) throw new ArgumentOutOfRangeException(nameof(durationDays), "Duration must be positive.");
        if (totalSeats <= 0) throw new ArgumentOutOfRangeException(nameof(totalSeats), "Total seats must be positive.");

        CourseCode = courseCode;
        Title = title;
        Fee = fee;
        DurationDays = durationDays;
        TotalSeats = totalSeats;
    }

    public int AvailableSeats => TotalSeats - EnrolledCount;
    public bool IsFull => AvailableSeats <= 0;

    public void IncrementEnrolled()
    {
        if (IsFull)
            throw new InvalidOperationException($"No seats left in {CourseCode}.");
        EnrolledCount++;
    }

    public void DecrementEnrolled()
    {
        if (EnrolledCount <= 0)
            throw new InvalidOperationException($"EnrolledCount for {CourseCode} is already zero.");
        EnrolledCount--;
    }

    public override string ToString() => $"{CourseCode} | {Title}";
}