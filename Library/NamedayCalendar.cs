using System.Collections;
using System.Text.RegularExpressions;

namespace Library;

public record NamedayCalendar : IEnumerable<Nameday>
{
    private Dictionary<DayMonth, List<Nameday>> _namedays = null!;
    public int NameCount => _namedays?.Values.Sum(listOfNamedays => listOfNamedays.Count) ?? 0;
    public int DayCount => _namedays?.Values.Count(listOfNamedays => listOfNamedays.Any()) ?? 0;
    
    /// <summary>
    /// Gets the <see cref="DayMonth"/> associated with the specified name.
    /// </summary>
    /// <param name="name">The name to search for.</param>
    /// <returns>
    /// The <see cref="DayMonth"/> associated with the specified name, or <c>null</c> if the name is not found.
    /// </returns>
    public DayMonth? this[string name]
    {
        get
        {
            foreach (var key in _namedays.Keys)
            {
                var nameDays = _namedays[key];
                try
                {
                    var nameday = nameDays.First(nameday => nameday.Name == name);
                    return nameday.DayMonth;
                }
                catch (InvalidOperationException)
                {
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Gets the names associated with the specified <see cref="DayMonth"/>.
    /// </summary>
    /// <param name="dayMonth">The <see cref="DayMonth"/> to retrieve names for.</param>
    /// <returns>An array of names associated with the specified <see cref="DayMonth"/>.</returns>
    public string[] this[DayMonth dayMonth] => _namedays[dayMonth].Select(nameday => nameday.Name).ToArray();

    /// <summary>
    /// Gets the names associated with the specified <see cref="DateOnly"/>.
    /// </summary>
    /// <param name="date">The <see cref="DateOnly"/> to retrieve names for.</param>
    /// <returns>An array of names associated with the specified <see cref="DateOnly"/>.</returns>
    public string[] this[DateOnly date] => this[date.Day, date.Month];

    /// <summary>
    /// Gets the names associated with the specified <see cref="DateTime"/>.
    /// </summary>
    /// <param name="date">The <see cref="DateTime"/> to retrieve names for.</param>
    /// <returns>An array of names associated with the specified <see cref="DateTime"/>.</returns>
    public string[] this[DateTime date] => this[date.Day, date.Month];

    /// <summary>
    /// Gets the names associated with the specified day and month.
    /// </summary>
    /// <param name="day">The day of the month.</param>
    /// <param name="month">The month of the year.</param>
    /// <returns>An array of names associated with the specified day and month.</returns>
    public string[] this[int day, int month] => this[new DayMonth(day, month)];
    
    /// <summary>
    /// Returns an enumerator that iterates through the collection of Nameday objects.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection of Nameday objects.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<Nameday> GetEnumerator()
    {
        return _namedays.Values.SelectMany(namedays => namedays).GetEnumerator();
    }
    
    /// <summary>
    /// Returns the namedays for the specified month.
    /// </summary>
    /// <param name="month">The month for which to retrieve the namedays.</param>
    /// <returns>An enumerable collection of Nameday objects for the specified month.</returns>
    public IEnumerable<Nameday> GetNamedays(int month)
    {
        foreach (var namedaysList in _namedays.Values)
        {
            foreach (var nameday in namedaysList)
            {
                if (nameday.DayMonth.Month == month)
                {
                    yield return nameday;
                }
            }
        }
    }
    
    /// <summary>
    /// Returns the namedays matching the specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match against the names.</param>
    /// <returns>An enumerable collection of Nameday objects matching the specified pattern.</returns>
    public IEnumerable<Nameday> GetNamedays(string pattern)
    {
        foreach (var namedaysList in _namedays.Values)
        {
            foreach (var nameday in namedaysList)
            {
                if (Regex.IsMatch(nameday.Name.ToLower(), pattern))
                {
                    yield return nameday;
                }
            }
        }
    }

    /// <summary>
    /// Adds the specified nameday to the nameday dictionary.
    /// </summary>
    /// <param name="nameday">The nameday to add.</param>
    public void Add(Nameday nameday) => Add(nameday.DayMonth, nameday.Name);
    
    /// <summary>
    /// Adds the specified names to the nameday dictionary for the given day and month.
    /// </summary>
    /// <param name="day">The day for the namedays.</param>
    /// <param name="month">The month for the namedays.</param>
    /// <param name="names">The names to add.</param>
    public void Add(int day, int month, params string[] names) => Add(new DayMonth(day, month), names);

    /// <summary>
    /// Adds the specified names to the nameday dictionary for the given day and month.
    /// </summary>
    /// <param name="dayMonth">The day and month for the namedays.</param>
    /// <param name="names">The names to add.</param>
    public void Add(DayMonth dayMonth, params string[] names)
    {
        var namedayToAdd =
            _namedays.Values.FirstOrDefault(namedays => namedays.Any(nameday => nameday.DayMonth.Equals(dayMonth)));

        if (namedayToAdd != null)
        {
            foreach (var name in names)
            {
                namedayToAdd.Add(new Nameday(name, dayMonth));
            }
        }
    }
    
    /// <summary>
    /// Removes the first occurrence of a person with the specified name from the namedays dictionary.
    /// </summary>
    /// <param name="name">The name of the person to remove.</param>
    /// <returns><c>true</c> if a person with the specified name was found and removed; otherwise, <c>false</c>.</returns>
    public bool Remove(string name)
    {
        var namedayToRemove = _namedays.Values.FirstOrDefault(namedays => namedays.Any(nameday => nameday.Name == name));

        if (namedayToRemove == null) return false;
        {
            namedayToRemove.RemoveAll(nameday => nameday.Name == name);
            return true;
        }
    }

    /// <summary>
    /// Determines whether the namedays dictionary contains a person with the specified name.
    /// </summary>
    /// <param name="name">The name to search for.</param>
    /// <returns><c>true</c> if a person with the specified name is found; otherwise, <c>false</c>.</returns>
    public bool Contains(string name) => this[name] != null;

    /// <summary>
    /// Removes all namedays from the dictionary, clearing it.
    /// </summary>
    public void Clear() => _namedays.Clear();
    
    /// <summary>
    /// Loads nameday data from a CSV file and populates the internal dictionary.
    /// </summary>
    /// <param name="csvFile">The CSV file containing the nameday data.</param>
    /// <exception cref="ArgumentException">Thrown if the provided <paramref name="csvFile"/> doesn't exist.</exception>
    public void Load(FileInfo csvFile)
    {
        const char csvSeparator = ';';
        const char dateSeparator = '.';
        
        if (!csvFile.Exists)
        {
            throw new ArgumentException("File info that was provided doesn't exists!", $"[csvFile: {csvFile}]");
        }
        
        using var reader = csvFile.OpenText();
        _namedays = new Dictionary<DayMonth, List<Nameday>>();
        while (reader.ReadLine() is { } line)
        {
            var lineValues = line.Split(csvSeparator);
            var dateValues = lineValues[0].Split(dateSeparator);
            var dayMonth = new DayMonth(int.Parse(dateValues[0]), int.Parse(dateValues[1]));

            if (!_namedays.TryGetValue(dayMonth, out var namedays))
            {
                namedays = new List<Nameday>();
                _namedays.Add(dayMonth, namedays);
            }

            foreach (var name in lineValues[1..])
            {
                if (string.IsNullOrWhiteSpace(name) || name == " -")
                {
                    continue;
                }
                namedays.Add(new Nameday(name, dayMonth));
            }
        }
    }

    public void Save(FileInfo csvFile)
    {
        const char csvSeparator = ';';
        const char dateSeparator = '.';

        using (var sw = new StreamWriter("D:\\Knižnice\\Dokumenty\\Škola\\C#\\ulohy-doma\\NamesDays\\NamesDays\\LibraryTests\\toBeSaved.csv"))
        {
            foreach (var dayMonth in _namedays.Keys)
            {
                string date = $"{dayMonth.Day}{dateSeparator}{dayMonth.Month}";
                string names = string.Join(csvSeparator, _namedays[dayMonth].Select(nameday => nameday.Name).ToArray());
                
                sw.WriteLine(string.Concat(date, csvSeparator, names));
            }
            
            sw.Close();
        }
    }

    //TODO: delete function
    public void Vypis()
    {
        foreach (var key in _namedays.Keys)
        {
            Console.WriteLine($"{key.Day}.{ key.Month}: {string.Join(", ", _namedays[key].Select(nameday => nameday.Name).ToArray())}");
        }
    }
}