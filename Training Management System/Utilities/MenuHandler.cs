
using System;
using System.Collections.Generic;
using TrainingManagementSystem.Models;
using TrainingManagementSystem.Payments;
using TrainingManagementSystem.Services;
using TrainingManagementSystem.Sessions;

namespace TrainingManagementSystem.Utilities;

public class MenuHandler
{
    private readonly TrainingService _trainingService;
    private readonly LearnerService _learnerService;
    private readonly EnrollmentService _enrollmentService;
    private readonly FeeCalculator _feeCalculator;
    private readonly PaymentService _paymentService;
    private readonly ReportService _reportService;

    public MenuHandler(
        TrainingService trainingService,
        LearnerService learnerService,
        EnrollmentService enrollmentService,
        FeeCalculator feeCalculator,
        PaymentService paymentService,
        ReportService reportService)
    {
        _trainingService = trainingService;
        _learnerService = learnerService;
        _enrollmentService = enrollmentService;
        _feeCalculator = feeCalculator;
        _paymentService = paymentService;
        _reportService = reportService;
    }

    public void Run()
    {
        Console.WriteLine(
            "Welcome to the Training Course Enrollment & Fee Management System.");
        Console.WriteLine(
            "(Learner data is saved to a file; other data may remain in memory.)");

        bool running = true;

        while (running)
        {
            PrintMenu();
            Console.Write("Enter choice (1-14): ");

            string? rawInput = Console.ReadLine();

            if (!InputValidator.TryParseInt(rawInput, out int choice))
            {
                Console.WriteLine(
                    "  ! Invalid input — please type a whole number (1-14).");
                continue;
            }

            Console.WriteLine(
                $"\n=== {DescribeChoice(choice).ToUpperInvariant()} ===");

            switch (choice)
            {
                case 1:
                    ListCourses();
                    break;

                case 2:
                    SearchCourses();
                    break;

                case 3:
                    SortCoursesByFee();
                    break;

                case 4:
                    RegisterLearner();
                    break;

                case 5:
                    ListLearners();
                    break;

                case 6:
                    SearchLearner();
                    break;

                case 7:
                    EnrollInCourse();
                    break;

                case 8:
                    ViewEnrollments();
                    break;

                case 9:
                    CancelEnrollment();
                    break;

                case 10:
                    ShowFeeBreakdown();
                    break;

                case 11:
                    ProcessPayment();
                    break;

                case 12:
                    ViewPayments();
                    break;

                case 13:
                    ShowReports();
                    break;

                case 14:
                    running = false;
                    break;

                case 0:
                    RunConceptDemos();
                    break;

                default:
                    Console.WriteLine(
                        $"  ! '{choice}' is not a valid option. Please choose 1-14 (or 0 for demos).");
                    break;
            }
        }

        Console.WriteLine("Goodbye!");
    }

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine(
            "=====================================================================");
        Console.WriteLine($"   {TrainingService.CentreName}");
        Console.WriteLine(
            "=====================================================================");
        Console.WriteLine("  1. List all courses              8. View enrollment records");
        Console.WriteLine("  2. Search courses                9. Cancel an enrollment");
        Console.WriteLine("  3. Sort courses by fee          10. Display a fee breakdown");
        Console.WriteLine("  4. Register a learner           11. Process a simulated payment");
        Console.WriteLine("  5. List learners                12. View payment history");
        Console.WriteLine("  6. Search for a learner         13. Display reports");
        Console.WriteLine("  7. Enroll in a course           14. Exit");
        Console.WriteLine("  0. Concept demos (Steps 6 / 7 / 8 / 13)");
        Console.WriteLine(
            "---------------------------------------------------------------------");
    }

    private static string DescribeChoice(int choice) => choice switch
    {
        1 => "Course catalogue",
        2 => "Course search",
        3 => "Courses sorted by fee",
        4 => "Learner registration",
        5 => "Learner list",
        6 => "Learner search",
        7 => "Course enrollment",
        8 => "Enrollment records",
        9 => "Enrollment cancellation",
        10 => "Fee breakdown",
        11 => "Simulated payment",
        12 => "Payment history",
        13 => "Reports",
        14 => "Exit",
        0 => "Concept demos",
        _ => "Unknown option"
    };

    private void ListCourses()
    {
        PrintCourseTable(_trainingService.GetAllCourses());
    }

    private void SearchCourses()
    {
        char by = InputValidator.ReadOption(
            "Search by (c)ode or (t)itle: ", 'c', 't');

        string term = InputValidator.ReadRequired("Search text: ");

        if (by == 'c')
        {
            Course? course = _trainingService.FindByCode(term);

            if (course is null)
                Console.WriteLine($"  ! No course with code '{term}'.");
            else
                PrintCourseTable(new Course[] { course });
        }
        else
        {
            List<Course> hits = _trainingService.SearchByTitle(term);

            if (hits.Count == 0)
                Console.WriteLine(
                    $"  ! No course title contains '{term}'.");
            else
                PrintCourseTable(hits);
        }
    }

    private void SortCoursesByFee()
    {
        char order = InputValidator.ReadOption(
            "(a)scending or (d)escending fee: ", 'a', 'd');

        Course[] sorted =
            _trainingService.SortByFee(ascending: order == 'a');

        PrintCourseTable(sorted);
    }

    private void PrintCourseTable(IReadOnlyList<Course> courses)
    {
        Console.WriteLine();
        Console.WriteLine(
            $"{"Code",-7} {"Title",-32} {"Fee",12} {"Days",5} {"Seats",6} {"Enrolled",9} {"Free",6}");
        Console.WriteLine(new string('-', 80));

        for (int i = 0; i < courses.Count; i++)
        {
            Course c = courses[i];

            Console.WriteLine(
                $"{c.CourseCode,-7} {c.Title,-32} {c.Fee,12:N2} " +
                $"{c.DurationDays,5} {c.TotalSeats,6} " +
                $"{c.EnrolledCount,9} {c.AvailableSeats,6}");
        }

        Console.WriteLine(new string('-', 80));
        Console.WriteLine($"Courses listed: {courses.Count}");
    }

    private void RegisterLearner()
    {
        while (true)
        {
            string id = InputValidator.ReadLearnerId(
                "Learner ID (2-10 letters/digits, e.g. L001): ");

            string name = InputValidator.ReadRequired("Full name: ");

            string contact = InputValidator.ReadContact(
                "Contact number (10 digits, starts 6-9): ");

            string email = InputValidator.ReadEmail(
                "Email (name@example.com): ");

            (bool ok, string message) =
                _learnerService.Register(id, name, contact, email);

            Console.WriteLine(
                ok ? $"  OK — {message}" : $"  REJECTED — {message}");

            if (ok)
                return;
        }
    }

    private void ListLearners()
    {
        if (_learnerService.Count == 0)
        {
            Console.WriteLine(
                "  No learners registered yet — use menu option 4 first.");
            return;
        }

        Console.WriteLine(
            $"\n{"ID",-8} | {"Name",-26} | {"Contact",-10} | Email");

        Console.WriteLine(new string('-', 76));

        CollectionHelper.PrintAll(_learnerService.GetAll());

        Console.WriteLine(
            $"Learners registered: {_learnerService.Count}");
    }

    private void SearchLearner()
    {
        string id = InputValidator.ReadRequired(
            "Learner ID to search: ");

        if (_learnerService.TryGet(id, out Learner? learner)
            && learner is not null)
        {
            Console.WriteLine("  Found: " + learner);
        }
        else
        {
            Console.WriteLine($"  ! No learner with ID '{id}'.");
        }
    }

    private void EnrollInCourse()
    {
        int groupSize = InputValidator.ReadInt(
            "How many learners are enrolling together? (1 = single; 3+ earns the 5% group discount): ",
            1, 60);

        Course? course = PromptForCourse(
            "Course code (or 'list', blank to cancel): ");

        if (course is null)
            return;

        DateTime start = InputValidator.ReadFutureDate(
            "Course start date (yyyy-mm-dd): ");

        if (FeeCalculator.IsEarlyBird(start))
        {
            Console.WriteLine(
                "  i Start date is 14+ days ahead — the 10% early-bird discount will apply.");
        }

        char mode = InputValidator.ReadOption(
            "Delivery mode — (o)nline (+₹200) / (c)lassroom (+₹800): ",
            'o', 'c');

        for (int i = 1; i <= groupSize; i++)
        {
            Console.WriteLine(
                $"\n--- Learner {i} of {groupSize} ---");

            string learnerId = InputValidator.ReadLearnerId(
                "Learner ID: ");

            try
            {
                var (success, enrollment, message) =
                    _enrollmentService.Enroll(
                        learnerId,
                        course.CourseCode,
                        start,
                        mode,
                        groupSize);

                if (success && enrollment is not null)
                {
                    Console.WriteLine($"  OK — {message}");

                    FeeSummary summary =
                        FeeSummary.FromEnrollment(enrollment);

                    Console.WriteLine(
                        _feeCalculator.FormatBreakdown(in summary));
                }
                else
                {
                    Console.WriteLine($"  REJECTED — {message}");
                }
            }
            catch (DuplicateEnrollmentException ex)
            {
                Console.WriteLine($"  REJECTED — {ex.Message}");
            }
        }
    }

    private void ViewEnrollments()
    {
        IReadOnlyList<Enrollment> all =
            _enrollmentService.GetAll();

        if (all.Count == 0)
        {
            Console.WriteLine("  No enrollment records yet.");
            return;
        }

        Console.WriteLine(
            $"\nAll enrollment records (cancelled ones kept for history): {all.Count}");

        CollectionHelper.PrintAll(all);
    }

    private void CancelEnrollment()
    {
        int id = InputValidator.ReadInt(
            "Enrollment ID to cancel (e.g. 1001; 0 = back): ",
            0, int.MaxValue);

        if (id == 0)
            return;

        (bool ok, string message) =
            _enrollmentService.Cancel(id);

        Console.WriteLine(
            ok ? $"  OK — {message}" : $"  REJECTED — {message}");
    }

    private void ShowFeeBreakdown()
    {
        Course? course = PromptForCourse(
            "Course code (or 'list', blank to cancel): ");

        if (course is null)
            return;

        DateTime start = InputValidator.ReadFutureDate(
            "Course start date (yyyy-mm-dd): ");

        char mode = InputValidator.ReadOption(
            "Delivery mode — (o)nline / (c)lassroom: ", 'o', 'c');

        int group = InputValidator.ReadInt(
            "Learners enrolling together (1 = individual): ",
            1, 60);

        CourseSession session =
            _enrollmentService.CreateSession(mode, course, start);

        decimal surcharge = session.CalculateSessionCharge();

        var fee = _feeCalculator.CalculateFee(
            course.Fee,
            FeeCalculator.IsEarlyBird(start),
            FeeCalculator.IsGroupEligible(group),
            surcharge);

        var summary = new FeeSummary(
            fee.BaseFee,
            fee.DiscountAmount,
            surcharge,
            fee.FinalFee);

        Console.WriteLine();
        Console.WriteLine(
            _feeCalculator.FormatBreakdown(in summary));

        Console.WriteLine(
            FeeCalculator.IsEarlyBird(start)
                ? "  i Early-bird: YES — start date is 14+ days ahead."
                : "  i Early-bird: no — start date is less than 14 days ahead.");

        Console.WriteLine(
            FeeCalculator.IsGroupEligible(group)
                ? "  i Group: YES — 3 or more learners enrolling together."
                : "  i Group: no — needs 3 or more learners.");
    }

    private void ProcessPayment()
    {
        int enrollmentId = InputValidator.ReadInt(
            "Enrollment ID to pay (0 = back): ",
            0, int.MaxValue);

        if (enrollmentId == 0)
            return;

        char mode = InputValidator.ReadOption(
            "Pay via (o)nline gateway or (c)ash at counter: ",
            'o', 'c');

        char outcome = InputValidator.ReadOption(
            "Simulated result — (s)uccess or (f)ailure: ",
            's', 'f');

        IPaymentProcessor? processor =
            _paymentService.GetProcessor(mode);

        if (processor is null)
        {
            Console.WriteLine("  ! Unknown payment mode.");
            return;
        }

        Console.WriteLine($"  Using processor: {processor.ModeName}");

        (bool success, string message) =
            _paymentService.Pay(
                enrollmentId, processor, outcome == 's');

        Console.WriteLine(
            success ? $"  OK — {message}" : $"  FAILED — {message}");
    }

    private void ViewPayments()
    {
        IReadOnlyList<Payment> payments =
            _paymentService.GetAll();

        if (payments.Count == 0)
        {
            Console.WriteLine("  No payment attempts recorded yet.");
            return;
        }

        Console.WriteLine(
            $"\nPayment attempts (successes AND failures kept): {payments.Count}");

        CollectionHelper.PrintAll(payments);
    }

    private void ShowReports()
    {
        while (true)
        {
            Console.WriteLine("\n--- REPORTS (no LINQ — loops only) ---");
            Console.WriteLine(" a. Course occupancy");
            Console.WriteLine(" b. Expected fees (active enrollments)");
            Console.WriteLine(" c. Payment summary");
            Console.WriteLine(" d. Course enrollment summary");
            Console.WriteLine(" x. Back to main menu");

            char choice = InputValidator.ReadOption(
                "Select report: ", 'a', 'b', 'c', 'd', 'x');

            switch (choice)
            {
                case 'a':
                    _reportService.PrintCourseOccupancy();
                    break;

                case 'b':
                    _reportService.PrintExpectedFees();
                    break;

                case 'c':
                    _reportService.PrintPaymentSummary();
                    break;

                case 'd':
                    _reportService.PrintCourseEnrollmentSummary();
                    break;

                case 'x':
                    return;
            }
        }
    }

    private Course? PromptForCourse(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? code = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(code))
            {
                Console.WriteLine("  Cancelled.");
                return null;
            }

            if (code.Equals(
                "list", StringComparison.OrdinalIgnoreCase))
            {
                PrintCourseTable(_trainingService.GetAllCourses());
                continue;
            }

            Course? course = _trainingService.FindByCode(code);

            if (course is not null)
                return course;

            Console.WriteLine(
                $"  ! No course with code '{code}'. Type 'list' to see all codes.");
        }
    }

    private void RunConceptDemos()
    {
        Console.WriteLine(
            "\n--- Step 6: method overloading + named tuples ---");

        var feeCalc = new FeeCalculator();

        var noDiscount = feeCalc.CalculateFee(8000m);
        var withSurcharge = feeCalc.CalculateFee(8000m, 200m);
        var full = feeCalc.CalculateFee(8000m, true, true, 200m);

        Console.WriteLine(
            $"  base only               → final ₹{noDiscount.FinalFee}");

        Console.WriteLine(
            $"  base + ₹200 online      → final ₹{withSurcharge.FinalFee}");

        Console.WriteLine(
            $"  base + 10% + 5% + ₹200  → discount ₹{full.DiscountAmount}, final ₹{full.FinalFee}");

        Console.WriteLine(
            "\n--- Step 7: runtime polymorphism through a base-class reference ---");

        var duration = new SessionDuration(5);

        CourseSession session = new OnlineSession(
            "C101", DateTime.Today.AddDays(20), duration);

        Console.WriteLine(
            $"  {session} → charge ₹{session.CalculateSessionCharge()}");

        session = new ClassroomSession(
            "C101", DateTime.Today.AddDays(20), duration);

        Console.WriteLine(
            $"  {session} → charge ₹{session.CalculateSessionCharge()}");

        Console.WriteLine(
            "\n--- Step 8: record value equality and the 'with' expression ---");

        var s1 = new CourseSummary(
            "C101", "C# Programming Fundamentals", 8000m, 30);

        var s2 = new CourseSummary(
            "C101", "C# Programming Fundamentals", 8000m, 30);

        Console.WriteLine(
            $"  s1 == s2 → {s1 == s2} (records compare by VALUE)");

        CourseSummary snapshot =
            CourseSummary.FromCourse(_trainingService.GetAllCourses()[0]);

        CourseSummary afterOneSeat = snapshot with
        {
            AvailableSeats = snapshot.AvailableSeats - 1
        };

        Console.WriteLine($"  original : {snapshot}");
        Console.WriteLine($"  with-copy: {afterOneSeat}");

        Console.WriteLine(
            "\n--- Step 8: SessionDuration value-type (struct) behaviour ---");

        SessionDuration a = new SessionDuration(3);
        SessionDuration b = a;

        a = a.Extend(2);

        Console.WriteLine(
            $"  a = {a}, b = {b} (b kept its copied value)");

        Console.WriteLine(
            $"  a.Equals(new SessionDuration(5)) → {a.Equals(new SessionDuration(5))} (equal by value)");

        Console.WriteLine(
            "\n--- Step 13 (46): T vs var vs dynamic ---");

        var number = 42;
        dynamic anything = number;

        anything = "now I am text";

        Console.WriteLine(
            $"  var number       → int, value {number} (compile-time type: {number.GetType().Name})");

        Console.WriteLine(
            $"  dynamic anything → runtime type now: {((object)anything).GetType().Name}");

        Console.WriteLine(
            "  T (generics)     → chosen by the CALLER at compile time; one method, many types, fully checked.");
    }
}

// Generic collection helper methods
public static class CollectionHelper
{
    public static void PrintAll<T>(IEnumerable<T> items)
    {
        foreach (T item in items)
        {
            Console.WriteLine("  " + item);
        }
    }

    public static T? Find<T>(
        IList<T> items,
        Predicate<T> match)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (match(items[i]))
            {
                return items[i];
            }
        }

        return default;
    }
}
