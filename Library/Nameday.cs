using System;

namespace Library
{
    public record struct Nameday
    {
        /// <summary>
        /// Name from calendar in string form.
        /// </summary>
        public readonly string Name { get; init; }
        
        /// <summary>
        /// Daymonth class that contains int Day and int Month.
        /// </summary>
        public readonly DayMonth DayMonth { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Nameday"/> class with the default parameters.
        /// </summary>
        public Nameday() : this("DefaultName", new DayMonth()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Nameday"/> class with the specified parameters.
        /// </summary>
        /// <param name="name">Name to be used in instance of object.</param>
        /// <param name="dayMonth">Name to be used in instance of object.</param>
        public Nameday(string name, DayMonth dayMonth)
        {
            Name = name;
            DayMonth = dayMonth;
        }

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>A string representation of the object.</returns>
        public override string ToString()
        {
            return $"[Name: {Name}, DayMonth: {DayMonth.ToString()} ]";
        }
    }
}
