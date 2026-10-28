using System;

namespace TrainingManagementSystem.Utilities;


public static class InputValidator
{
    

    public static bool TryParseInt(string? input, out int value) =>
        int.TryParse(input?.Trim(), out value);

    
    public static bool TryParseLearnerId(string? input, out string learnerId)
    {
        learnerId = string.Empty;
        if (string.IsNullOrWhiteSpace(input)) return false;

        string candidate = input.Trim();
        if (candidate.Length < 2 || candidate.Length > 10) return false;

        foreach (char ch in candidate)
            if (!char.IsLetterOrDigit(ch)) return false;

        learnerId = candidate;
        return true;
    }

    
    public static bool IsValidContactNumber(string? contact)
    {
        if (string.IsNullOrEmpty(contact) || contact.Length != 10) return false;
        if (contact[0] < '6' || contact[0] > '9') return false;
        foreach (char ch in contact)
            if (ch < '0' || ch > '9') return false;
        return true;
    }

    
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        string value = email.Trim();

        int at = value.IndexOf('@');
        if (at < 1 || at == value.Length - 1) return false;
        if (value.IndexOf('@', at + 1) >= 0) return false;   

        string domain = value[(at + 1)..];
        int dot = domain.IndexOf('.');
        return dot > 0 && dot < domain.Length - 1;
    }

    

    public static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(input)) return input;
            Console.WriteLine("  ! Value cannot be empty — please try again.");
        }
    }

    public static string ReadLearnerId(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (TryParseLearnerId(Console.ReadLine(), out string learnerId)) return learnerId;
            Console.WriteLine("  ! ID must be 2-10 characters, letters and digits only.");
        }
    }

    public static string ReadContact(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (input is not null && IsValidContactNumber(input)) return input;
            Console.WriteLine("  ! Contact must be exactly 10 digits and start with 6, 7, 8 or 9.");
        }
    }

    public static string ReadEmail(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim();
            if (input is not null && IsValidEmail(input)) return input;
            Console.WriteLine("  ! Enter an email like name@example.com.");
        }
    }

    public static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            if (TryParseInt(Console.ReadLine(), out int value) && value >= min && value <= max)
                return value;
            Console.WriteLine($"  ! Enter a whole number between {min} and {max}.");
        }
    }

    public static DateTime ReadFutureDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParse(Console.ReadLine()?.Trim(), out DateTime value) && value.Date >= DateTime.Today)
                return value.Date;
            Console.WriteLine($"  ! Enter a valid date, today or later " +
                              $"(e.g. {DateTime.Today.AddDays(14):yyyy-MM-dd} qualifies for the early-bird discount).");
        }
    }

    public static char ReadOption(string prompt, params char[] options)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(input) && input.Length == 1)
            {
                char choice = input[0];
                foreach (char option in options)
                    if (option == choice) return choice;
            }
            Console.WriteLine($"  ! Please enter one of: {string.Join(" / ", options)}.");
        }
    }
}