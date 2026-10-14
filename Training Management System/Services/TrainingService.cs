using System;
using System.Collections.Generic;
using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Services;

// STEP 1 & 10 — owns the course catalogue (the SINGLE copy of course data)
// and coordinates all course operations.
public class TrainingService
{
    public const string CentreName = "Apex Skill Training Centre";   // constant

    private readonly Course[] _courses;                              // STEP 1: fixed array catalogue

    public TrainingService(Course[]? catalogue = null)
    {
        _courses = catalogue ?? CreateDefaultCatalogue();
    }

    private static Course[] CreateDefaultCatalogue() => new Course[]   // six predefined courses
    {
        new Course("C101", "C# Programming Fundamentals",  8000m, 10, 30),
        new Course("C102", "Advanced ASP.NET Core",       15000m, 15, 20),
        new Course("C103", "SQL Server for Developers",   10000m,  8, 25),
        new Course("C104", "Azure Cloud Basics",          12000m, 12, 18),
        new Course("C105", "Python for Data Analysis",     9500m, 10, 22),
        new Course("C106", "JavaScript and the DOM",       7000m,  6, 35),
    };

    public Course[] GetAllCourses() => _courses;

    public int CatalogueSize => _courses.Length;

    // Case-insensitive search by course code (loop, no LINQ).
    public Course? FindByCode(string courseCode)
    {
        foreach (Course c in _courses)
            if (string.Equals(c.CourseCode, courseCode?.Trim(), StringComparison.OrdinalIgnoreCase))
                return c;
        return null;
    }

    // Case-insensitive "contains" search by title.
    public List<Course> SearchByTitle(string text)
    {
        var hits = new List<Course>();
        string needle = text?.Trim() ?? string.Empty;
        foreach (Course c in _courses)
            if (c.Title.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0)
                hits.Add(c);
        return hits;
    }

    // Sorts a COPY so the master array keeps its original order.
    public Course[] SortByFee(bool ascending)
    {
        Course[] copy = new Course[_courses.Length];
        Array.Copy(_courses, copy, _courses.Length);
        Array.Sort(copy, (a, b) => ascending ? a.Fee.CompareTo(b.Fee) : b.Fee.CompareTo(a.Fee));
        return copy;
    }
}