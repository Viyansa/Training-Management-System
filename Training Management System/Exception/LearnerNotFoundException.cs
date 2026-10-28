namespace Course_Catalogue.Exceptions
{
    using System;

    public class LearnerNotFoundException : Exception
    {
        public string LearnerId { get; }

        public LearnerNotFoundException(string learnerId)
            : base($"Learner with ID '{learnerId}' was not found.")
        {
            LearnerId = learnerId;
        }
    }
    public static class LearnerExceptionHandler
    {
        public static void SafeExecute(Action action)
        {
            try
            {
                action();
            }
            catch (LearnerNotFoundException ex)
            {
                Console.WriteLine($"\n[Error]: {ex.Message}");
            }
        }
    }
}