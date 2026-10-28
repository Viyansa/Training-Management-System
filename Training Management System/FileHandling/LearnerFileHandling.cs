
using System;
using System.IO;
using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.FileHandling
{
    public class LearnerFileHandler
    {
        private readonly string filePath =
            Path.Combine(
                AppContext.BaseDirectory,
                "Learners.txt"
            );

        public void SaveLearner(Learner learner)
        {
            try
            {
                string record =
                    $"{learner.LearnerId}," +
                    $"{learner.Name}," +
                    $"{learner.ContactNumber}," +
                    $"{learner.Email}";

                File.AppendAllText(
                    filePath,
                    record + Environment.NewLine
                );

                Console.WriteLine(
                    "Learner saved successfully!"
                );
            }
            catch (IOException ex)
            {
                Console.WriteLine(
                    $"File error: {ex.Message}"
                );
            }
        }

        public void DisplayLearners()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine(
                        "No saved learner records found."
                    );
                    return;
                }

                string[] records =
                    File.ReadAllLines(filePath);

                foreach (string record in records)
                {
                    Console.WriteLine(record);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(
                    $"File error: {ex.Message}"
                );
            }
        }
    }
}
