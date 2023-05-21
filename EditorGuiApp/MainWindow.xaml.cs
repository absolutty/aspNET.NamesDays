using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Library;

namespace EditorGuiApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private NamedayCalendar _calendar;
        private DateTime _selectedDateTime;
        private string _textBoxContent;
        public event PropertyChangedEventHandler? PropertyChanged;

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
            SelectedDateTime = today;
            TextBoxContent = JoinNamesFromDate(today);
        }

        private string JoinNamesFromDate(DateTime dateTime) => string.Join(Environment.NewLine, _calendar[dateTime]);
    }
}