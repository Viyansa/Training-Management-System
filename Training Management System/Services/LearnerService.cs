using System;
using System.Collections.Generic;
using TrainingManagementSystem.Models;
using TrainingManagementSystem.Utilities;

namespace TrainingManagementSystem.Services;

// STEP 2 & 10 — the Dictionary<string, Learner> is the source of truth for learners.
public class LearnerService
{
    // Case-insensitive learner IDs.
    private readonly Dictionary<string, Learner> _learners =
        new Dictionary<string, Learner>(StringComparer.OrdinalIgnoreCase);

    public int Count => _learners.Count;

    public (bool Success, string Message) Register(string learnerId, string name, string contact, string email)
    {
        if (string.IsNullOrWhiteSpace(learnerId)) return (false, "Learner ID cannot be empty.");
        if (string.IsNullOrWhiteSpace(name)) return (false, "Name cannot be empty.");

        if (!InputValidator.IsValidContactNumber(contact))
            return (false, "Contact must be exactly 10 digits and start with 6, 7, 8 or 9.");
        if (!InputValidator.IsValidEmail(email))
            return (false, "Email must look like name@example.com.");

        if (_learners.ContainsKey(learnerId))                       // duplicate check
            return (false, $"Learner ID '{learnerId}' is already registered.");

        var learner = new Learner(learnerId.Trim(), name.Trim(), contact.Trim(), email.Trim());
        _learners.Add(learner.LearnerId, learner);                  // practising Add()
        return (true, $"Learner '{learner.Name}' registered with ID {learner.LearnerId}.");
    }

    public bool TryGet(string learnerId, out Learner? learner) =>
        _learners.TryGetValue(learnerId ?? string.Empty, out learner);   // TryGetValue

    public IEnumerable<Learner> GetAll() => _learners.Values;            // iterate Values

    // Practises Remove(). Deliberately not in the menu (centres rarely delete
    // learners) — kept so the Dictionary API is fully exercised.
    public bool Remove(string learnerId) => _learners.Remove(learnerId);
}