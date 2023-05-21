using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Library;
using Calendar = System.Windows.Controls.Calendar;

namespace EditorGuiApp
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private NamedayCalendar _calendar;
        public ObservableCollection<Nameday> Namedays { get; set; } = null!;
        private DateTime _selectedDateTime;
        private string _textBoxContent = null!;
        private ObservableCollection<string> _comboBoxItems = null!;
        public event PropertyChangedEventHandler? PropertyChanged;

        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                string? selectedMonth = comboBox.SelectedItem as string;
                if (selectedMonth != null)
                {
                    int monthIndex = GetMonthIndex(selectedMonth);
                    SetNameDays(monthIndex);
                }
            }
        }

        private int GetMonthIndex(string? monthName)
        {
            var dateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
            return Array.IndexOf(dateTimeFormatInfo.MonthNames, monthName) + 1;
        }
        
        public DateTime SelectedDateTime
        {
            get => _selectedDateTime; 
            set
            {
                if (_selectedDateTime != value)
                {
                    _selectedDateTime = value;
                    TextBoxContent = JoinNamesFromDate(value);
                    
                    OnPropertyChanged(nameof(SelectedDateTime));
                }
            }
        }
        
        public string TextBoxContent
        {
            get => _textBoxContent; 
            set
            {
                if (_textBoxContent != value)
                {
                    _textBoxContent = value;
                    OnPropertyChanged(nameof(TextBoxContent));
                }
            }
        }
        
        public ObservableCollection<string> ComboBoxItems
        {
            get => _comboBoxItems;
            set
            {
                _comboBoxItems = value;
                OnPropertyChanged(nameof(ComboBoxItems));
            }
        }

        private void CalendarSelectedListener(object sender, SelectionChangedEventArgs e)
        {
            if (sender is Calendar { SelectedDate: not null } calendar)
            {
                SelectedDateTime = calendar.SelectedDate.Value;
            }
        }
        
        private void TodayButtonListener(object sender, RoutedEventArgs routedEventArgs)
        {
            SelectedDateTime = DateTime.Now;
        } 

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            _calendar = new NamedayCalendar();
            _calendar.Load(new FileInfo("res\\csv\\toBeLoaded.csv"));
            InitializeComponent();
            DataContext = this;

            InitialViewValues();
        }

        private void InitialViewValues()
        {
            DateTime today = DateTime.Now;

            SetNameDays(12);
            
            SelectedDateTime = today;
            TextBoxContent = JoinNamesFromDate(today);
            ComboBoxItems = GetMonthNames();
        }

        private string JoinNamesFromDate(DateTime dateTime) => string.Join(Environment.NewLine, _calendar[dateTime]);
        
        private ObservableCollection<string> GetMonthNames()
        {
            var monthNames = new ObservableCollection<string>();
            
            for (int monthIndex = 1; monthIndex <= 12; monthIndex++)
            {
                var dateTimeFormatInfo = CultureInfo.CurrentCulture.DateTimeFormat;
                monthNames.Add(dateTimeFormatInfo.GetMonthName(monthIndex));
            }

            return monthNames;
        }

        private void SetNameDays(int monthIndex)
        {
            Namedays = new ObservableCollection<Nameday>();
            foreach (var nameday in _calendar.GetNamedays(monthIndex))
            {
                Namedays.Add(nameday);
            }
        }
    }
}