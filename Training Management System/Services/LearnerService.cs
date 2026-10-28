using System;
using System.Collections.Generic;
using TrainingManagementSystem.Models;
using TrainingManagementSystem.Utilities;

namespace TrainingManagementSystem.Services;

using TrainingManagementSystem.FileHandling;



public class LearnerService
{
    
    private readonly Dictionary<string, Learner> _learners =
        new Dictionary<string, Learner>(StringComparer.OrdinalIgnoreCase);

    public int Count => _learners.Count;


    public (bool Success, string Message) Register(
        string learnerId, string name, string contact, string email)
    {
        if (string.IsNullOrWhiteSpace(learnerId))
            return (false, "Learner ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(name))
            return (false, "Name cannot be empty.");

        if (!InputValidator.IsValidContactNumber(contact))
            return (false, "Contact must be exactly 10 digits and start with 6, 7, 8 or 9.");

        if (!InputValidator.IsValidEmail(email))
            return (false, "Email must look like name@example.com.");

        if (_learners.ContainsKey(learnerId.Trim()))
            return (false, $"Learner ID '{learnerId}' is already registered.");

        var learner = new Learner(
            learnerId.Trim(),
            name.Trim(),
            contact.Trim(),
            email.Trim()
        );

        // 1. Add learner to the existing collection
        _learners.Add(learner.LearnerId, learner);

        // 2. Save learner details to the file
        LearnerFileHandler fileHandler = new LearnerFileHandler();
        fileHandler.SaveLearner(learner);

        // 3. Return registration success
        return (true,
            $"Learner '{learner.Name}' registered with ID {learner.LearnerId}.");
    }


    public bool TryGet(string learnerId, out Learner? learner) =>
        _learners.TryGetValue(learnerId ?? string.Empty, out learner);   

    public IEnumerable<Learner> GetAll() => _learners.Values;            

    public bool Remove(string learnerId) => _learners.Remove(learnerId);
}