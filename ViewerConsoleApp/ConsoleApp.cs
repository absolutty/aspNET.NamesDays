using System.Globalization;
using System.Resources;
using Library;
using ViewerConsoleApp.res;
using DateTime = System.DateTime;

namespace ViewerConsoleApp;

public class ConsoleApp
{
    private readonly NamedayCalendar _calendar;
    private readonly ResourceManager _resourceManager;
    
    public ConsoleApp()
    {
        _calendar = new NamedayCalendar();
        _resourceManager = new ResourceManager("ViewerConsoleApp.res.strings.Strings", typeof(Program).Assembly);
    }

    public void Run()
    {
        Console.WriteLine(_resourceManager.GetString("menu.title"));
        if (_calendar.NameCount > 0 && _calendar.DayCount > 0) //if some namedays are loaded
        {
            DateTime toBeUsedDatetime = DateTime.Today;
            DayMonth toBeUsedDaymonth = new DayMonth(toBeUsedDatetime.Day, toBeUsedDatetime.Month);

            string? nameDays = _calendar[toBeUsedDaymonth].ToString();
            Console.WriteLine($@"Dnes meniny {toBeUsedDatetime.ToString("dd.M.yyyy")} {(nameDays.Length > 0 ? string.Join(", ", _calendar[toBeUsedDaymonth]) : _resourceManager.GetString("menu.namedaynone"))}");
            
            toBeUsedDatetime = DateTime.Today.AddDays(1);
            toBeUsedDaymonth = new DayMonth(toBeUsedDatetime.Day, toBeUsedDatetime.Month);
            Console.WriteLine($@"Zajtra má meniny: {(nameDays.Length > 0 ? string.Join(", ", _calendar[toBeUsedDaymonth]) : _resourceManager.GetString("menu.namedaynone"))}");
            Console.WriteLine();
        }
        
        ShowMenu();
    }

    private const string OptionEnd = "6";
    private void ShowMenu()
    {
        string volba = string.Empty;
        while (!volba!.Equals(OptionEnd))
        {
            Console.Write(_resourceManager.GetString("menu.options"));
            volba = Console.ReadLine()!;

            Console.Clear();
            switch (volba)
            {
                case OptionLoadCalendar:
                    LoadCalendar();
                    break;
                case OptionShowStatistics:
                    ShowStatistics();
                    break;
                case OptionSearchNames:
                    SearchNames();
                    break;
                case OptionSearchNamesBasedOnDate:
                    SearchNamesBasedOnDate();
                    break;
                case OptionShowCalendarInMonth:
                    ShowCalendarInMonth();
                    break;
                case OptionEnd:
                    return;
            }
        }

    }

    private const string OptionLoadCalendar = "1";
    private void LoadCalendar()
    {
        Console.WriteLine(_resourceManager.GetString("load.title"));

        string csvFilePath = string.Empty;
        bool correctFileName = false;
        
        while (!correctFileName)
        {
            try
            {
                Console.Write(_resourceManager.GetString("load.text"));
                csvFilePath = Console.ReadLine()!;

                if (!csvFilePath.Substring(csvFilePath.Length - 4).ToLower().Equals(".csv"))
                {
                    throw new FormatException("You need to specify file in .csv format!");
                }

                LoadCalendar(csvFilePath);
                correctFileName = true;
            }
            catch (ArgumentException)
            {
                Console.WriteLine(_resourceManager.GetString("load.notFound")!, csvFilePath);
            }
            catch (FormatException)
            {
                Console.WriteLine(_resourceManager.GetString("load.wrongFormat")!, csvFilePath);
            }
        }

        WantToEndAccept();
    }
    
    public void LoadCalendar(string csvFilePath)
    {
        _calendar.Load(new FileInfo(csvFilePath));
    }

    private const string OptionShowStatistics = "2";
    public void ShowStatistics()
    {
        Console.WriteLine(_resourceManager.GetString("statistics.title"));
        Console.WriteLine(_resourceManager.GetString("statistics.nOfNames") + _calendar.NameCount);
        Console.WriteLine(_resourceManager.GetString("statistics.dayWithNames") + _calendar.DayCount);
        
        Console.WriteLine(_resourceManager.GetString("statistics.nOfNamesInMonths"));
        DateTimeFormatInfo dateFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
        for (int i = 1; i <= 12; i++)
        {
            Console.WriteLine($@"  {dateFormatInfo.GetMonthName(i)}: {_calendar.GetNamedays(i).Count()}");
        }

        Dictionary<char, int> nOfNamesBeginLetters = new Dictionary<char, int>();
        Dictionary<int, int> nOfNamesStringLengths = new Dictionary<int, int>();
        foreach (Nameday nameday in _calendar)
        {
            char beginLetter = nameday.Name[0];
            if (nOfNamesBeginLetters.ContainsKey(beginLetter))
            {
                nOfNamesBeginLetters[beginLetter]++;
            }
            else
            {
                nOfNamesBeginLetters.Add(beginLetter, 1);
            }

            int nameLength = nameday.Name.Length;
            if (nOfNamesStringLengths.ContainsKey(nameLength))
            {
                nOfNamesStringLengths[nameLength]++;
            }
            else
            {
                nOfNamesStringLengths.Add(nameLength, 1);
            }
        }
        
        Console.WriteLine(_resourceManager.GetString("statistics.nOfNamesBeginLetter"));
        foreach (var znakAbecedy in Shared.SlovakAlphabet)
        {
            if (nOfNamesBeginLetters.TryGetValue(znakAbecedy, out var value))
            {
                Console.WriteLine($@"  {znakAbecedy}: {value}");
            }
        }

        Console.WriteLine(_resourceManager.GetString("statistics.nOfNamesStringLength"));
        foreach (var key in nOfNamesStringLengths.Keys.OrderBy(key => key))
        {
            Console.WriteLine($@"  {key}: {nOfNamesStringLengths[key]}");
        }

        WantToEndAccept();
    }

    private const string OptionSearchNames = "3";
    public void SearchNames()
    {
        Console.WriteLine(_resourceManager.GetString("searchRegex.title"));
        Console.Write(_resourceManager.GetString("searchRegex.input"));

        string regexInput = Console.ReadLine()!;

        while (regexInput.Length > 0)
        {
            int index = 1;
            foreach (var nameday in _calendar.GetNamedays(regexInput))
            {
                Console.WriteLine($@"{index}. {nameday.Name} ({nameday.DayMonth})");
            
                index++;
            }
            
            Console.Write(_resourceManager.GetString("searchRegex.input"));
            regexInput = Console.ReadLine()!;
        }

    }

    private const string OptionSearchNamesBasedOnDate = "4";
    public void SearchNamesBasedOnDate()
    {
        Console.WriteLine(_resourceManager.GetString("searchDate.title"));
        Console.Write(_resourceManager.GetString("searchDate.input"));

        string regexInput = Console.ReadLine()!;

        while (regexInput.Length > 0)
        {
            string[] split = regexInput.Split('.');
            int day = int.Parse(split[0]);
            int month = int.Parse(split[1]);

            int index = 1;
            foreach (var nameday in _calendar[new DayMonth(day, month)])
            {
                Console.WriteLine($@"{index}. {nameday}");
            }
            
            Console.Write(_resourceManager.GetString("searchDate.input"));
            regexInput = Console.ReadLine()!;
        }
        
        Console.Clear();
    }

    private const string OptionShowCalendarInMonth = "5";
    public void ShowCalendarInMonth()
    {
        DateTime toBeUsedDate = DateTime.Now;
        bool endLoop = false;
        while (!endLoop)
        {
            Console.WriteLine(_resourceManager.GetString("calendar.title"));

            string formattedDate = toBeUsedDate.ToString("MMMM yyyy");
            Console.WriteLine($@"{formattedDate}: ");

            IEnumerable<DayMonth> datesInMonth = DatesInMonth(toBeUsedDate.Month, toBeUsedDate.Day);
            foreach (var daymonth in datesInMonth)
            {
                Shared.PrintColorfulBasedOnDate(
                    String.Format(
                        "{0}.{1} {2} {3}",
                        daymonth.Day,
                        daymonth.Month,
                        new DateTime(toBeUsedDate.Year, daymonth.Month, daymonth.Day).ToString("dddd").Substring(0, 2),
                        string.Join(", ", _calendar[daymonth])
                    ),
                    daymonth,
                    toBeUsedDate.Year
                );
            }
            
            
            Console.WriteLine(_resourceManager.GetString("calendar.caption"));
            Console.WriteLine(_resourceManager.GetString("menu.end"));
            switch (Console.ReadKey().Key) 
            {
                //odpocitaj mesiac
                case ConsoleKey.LeftArrow: 
                    toBeUsedDate = toBeUsedDate.AddMonths(-1);
                    break;
                //pripocitaj mesiac
                case ConsoleKey.RightArrow: 
                    toBeUsedDate = toBeUsedDate.AddMonths(1);
                    break;
                //odpocitaj rok
                case ConsoleKey.DownArrow:
                    toBeUsedDate = toBeUsedDate.AddYears(-1);
                    break;
                //pripocitaj rok
                case ConsoleKey.UpArrow: 
                    toBeUsedDate = toBeUsedDate.AddYears(1);
                    break;
                //aktualny mesiac
                case ConsoleKey.Home:
                case ConsoleKey.D:
                    toBeUsedDate = DateTime.Now;
                    break;
                //ukonci
                case ConsoleKey.Enter:
                    endLoop = true;
                    break;
            }
            Console.Clear();
        }
        
    }
    
    public static IEnumerable<DayMonth> DatesInMonth(int month, int year)
    {
        int days = DateTime.DaysInMonth(year, month);
        for (int day = 1; day <= days; day++)
        {
            yield return new DayMonth(day, month);
        }
    }
    
    private bool WantToEndAccept()
    {
        Console.WriteLine(_resourceManager.GetString("menu.end"));
        
        bool wantToEnd = Console.ReadKey().Key == ConsoleKey.Enter;
        Console.Clear();
        
        return wantToEnd;
    }
    
}