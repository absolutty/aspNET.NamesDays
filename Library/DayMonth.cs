namespace Library
{
    public record struct DayMonth
    {
        /// <summary>
        /// Index of Day from interval (1; 31)
        /// </summary>
        public readonly int Day { get; init; }
        
        /// <summary>
        /// Index of Month from interval (1; 12)
        /// </summary>
        public readonly int Month { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Nameday"/> class with the default parameters.
        /// </summary>
        public DayMonth() : this(1, 1) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DayMonth"/> class with the specified parameters.
        /// </summary>
        /// <param name="day">Index of Day to be used in instance of object.</param>
        /// <param name="month">Index of Month to be used in instance of object.</param>
        public DayMonth(int day, int month)
        {
            _tryCreateDate(day, month);
            
            Day = day;
            Month = month;
        }

        private static readonly int LeapYear = 2020;
        /// <summary>
        /// Tries to create a DateTime object based on the provided day and month values.
        /// </summary>
        /// <param name="day">The day value.</param>
        /// <param name="month">The month value.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one of the provided parameters is out of range.</exception>
        private static void _tryCreateDate(int day, int month)
        {
            try
            {
                var tryDateTime = new DateTime(LeapYear, month, day);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException(
                    String.Format("[Day: {0}, Month: {1}]", day, month), 
                    "One of provided parameters is out of range!"
                );
            }
        }

        /// <summary>
        /// Converts the DayMonth object to a DateTime object with the current year and the specified month and day.
        /// </summary>
        /// <returns>A DateTime object representing the same month and day with the current year.</returns>
        public DateTime ToDateTime()
        {
            _tryCreateDate(Day, Month);
            return new DateTime(LeapYear, Month, Day);
        }

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return $"[Day: {Day}, Month: {Month}, DateTime: {ToDateTime()} ]";
        }
    }
}