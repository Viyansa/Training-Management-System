namespace TrainingManagementSystem.Models;
public record CourseSummary(string CourseCode, string Title, decimal Fee, int AvailableSeats)
{
    public static CourseSummary FromCourse(Course course) =>
        new(course.CourseCode, course.Title, course.Fee, course.AvailableSeats);
}