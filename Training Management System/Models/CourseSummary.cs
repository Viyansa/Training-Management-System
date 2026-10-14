namespace TrainingManagementSystem.Models;

// STEP 8 — immutable positional record (a lightweight snapshot).
// Records get value-based Equals/==, GetHashCode and ToString for free.
// NOTE: this does NOT replace the mutable Course class used by the services.
public record CourseSummary(string CourseCode, string Title, decimal Fee, int AvailableSeats)
{
    public static CourseSummary FromCourse(Course course) =>
        new(course.CourseCode, course.Title, course.Fee, course.AvailableSeats);
}