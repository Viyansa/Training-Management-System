using System;

namespace TrainingManagementSystem.Models;

// STEP 2 — a registered learner.
public class Learner
{
    public string LearnerId { get; }
    public string Name { get; }
    public string ContactNumber { get; }
    public string Email { get; }

    public Learner(string learnerId, string name, string contactNumber, string email)
    {
        if (string.IsNullOrWhiteSpace(learnerId))
            throw new ArgumentException("Learner ID is required.", nameof(learnerId));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        LearnerId = learnerId;
        Name = name;
        ContactNumber = contactNumber ?? string.Empty;
        Email = email ?? string.Empty;
    }

    // Column-aligned so the generic PrintAll<T> produces a tidy table.
    public override string ToString() =>
        $"{LearnerId,-8} | {Name,-26} | {ContactNumber,-10} | {Email}";
}