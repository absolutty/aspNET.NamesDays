using Library;

namespace ViewerConsoleApp.res;

public class Shared
{
    public const string SlovakAlphabet = "AÁBCČDĎEÉFGHIÍJKLMNŇOÓÔPQRŔSŠTŤUÚVWXYÝZŽ";

    public static readonly ConsoleColor ColorOfToday = ConsoleColor.Green;
    public static readonly ConsoleColor ColorOfWorkday = ConsoleColor.White;
    public static readonly ConsoleColor ColorOfWeekend = ConsoleColor.Red;
    public static void PrintColorfulBasedOnDate(string toBePrinted, DayMonth dayMonth, int year)
    {
        DateTime date = new DateTime(year, dayMonth.Month, dayMonth.Day);

        if (date == DateTime.Today)
        {
            Console.ForegroundColor = ColorOfToday;
        }
        else if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            Console.ForegroundColor = ColorOfWeekend;
        }

        Console.WriteLine(toBePrinted);
        Console.ForegroundColor = ColorOfWorkday;
    }
} 