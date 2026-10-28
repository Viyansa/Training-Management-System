
using System;
using Microsoft.Data.SqlClient;

namespace TrainingManagementSystem.Data
{
    public class LearnerRepository
    {
        private readonly DbConnectionFactory factory =
            new DbConnectionFactory();

        public void AddLearner(
            string learnerId,
            string name,
            string contactNumber,
            string email)
        {
            string query = @"INSERT INTO Learners
                (LearnerId, Name, ContactNumber, Email)
                VALUES
                (@LearnerId, @Name, @ContactNumber, @Email)";

            using SqlConnection connection =
                factory.CreateConnection();

            using SqlCommand command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@LearnerId", learnerId);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@ContactNumber", contactNumber);
            command.Parameters.AddWithValue("@Email", email);

            connection.Open();
            int rows = command.ExecuteNonQuery();

            if (rows > 0)
            {
                Console.WriteLine("Learner added successfully!");
            }
        }

        public void DisplayLearners()
        {
            string query =
                "SELECT LearnerId, Name, ContactNumber, Email FROM Learners";

            using SqlConnection connection =
                factory.CreateConnection();

            using SqlCommand command =
                new SqlCommand(query, connection);

            connection.Open();

            using SqlDataReader reader =
                command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine(
                    $"ID: {reader["LearnerId"]}, " +
                    $"Name: {reader["Name"]}, " +
                    $"Contact: {reader["ContactNumber"]}, " +
                    $"Email: {reader["Email"]}"
                );
            }
        }
    }
}
