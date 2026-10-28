using System;
using System.Collections.Generic;
using TrainingManagementSystem.Models;

namespace TrainingManagementSystem.Services;



public class TrainingService
{
    public const string CentreName = "Apex Skill Training Centre";   

    private readonly Course[] _courses;                              

    public TrainingService(Course[]? catalogue = null)
    {
        _courses = catalogue ?? CreateDefaultCatalogue();
    }

    private static Course[] CreateDefaultCatalogue() => new Course[]   
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

  
    public List<Course> SearchByTitle(string text)
    {
        var hits = new List<Course>();
        string needle = text?.Trim() ?? string.Empty;
        foreach (Course c in _courses)
            if (c.Title.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0)
                hits.Add(c);
        return hits;
    }

    
    public Course[] SortByFee(bool ascending)
    {
        Course[] copy = new Course[_courses.Length];
        Array.Copy(_courses, copy, _courses.Length);
        Array.Sort(copy, (a, b) => ascending ? a.Fee.CompareTo(b.Fee) : b.Fee.CompareTo(a.Fee));
        return copy;
    }
}