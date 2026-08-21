using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using VolenteerHub.Data;
using VolenteerHub.Models;

namespace VolenteerHub.Views
{
    public partial class CalendarWindow : Window
    {
        private User currentUser;

        private List<VolunteerEvent> allEvents;

        private HashSet<int> registeredEventIds;


        public CalendarWindow(
            User user)
        {
            InitializeComponent();

            currentUser = user;

            allEvents =
                DatabaseHelper.GetAllEvents();

            LoadRegisteredEventIds();

            EventCalendar.DisplayDate =
                DateTime.Today;

            EventCalendar.SelectedDate =
                DateTime.Today;

            ShowEventsForMonth(
                DateTime.Today);
        }


        // =====================================================
        // LOAD USER REGISTRATIONS
        // =====================================================

        private void LoadRegisteredEventIds()
        {
            registeredEventIds =
                new HashSet<int>();

            if (currentUser == null)
            {
                return;
            }

            try
            {
                List<MyVolunteerEvent> userEvents =
                    DatabaseHelper.GetUserVolunteerEvents(
                        currentUser.Id);

                foreach (MyVolunteerEvent volunteerEvent
                    in userEvents)
                {
                    registeredEventIds.Add(
                        volunteerEvent.EventId);
                }
            }
            catch
            {
                registeredEventIds =
                    new HashSet<int>();
            }
        }


        // =====================================================
        // SHOW MONTH
        // =====================================================

        private void ShowEventsForMonth(
            DateTime month)
        {
            RefreshCalendarData();

            List<VolunteerEvent> monthEvents =
                allEvents
                .Where(volunteerEvent =>
                {
                    DateTime eventDate;

                    if (!DateTime.TryParseExact(
                        volunteerEvent.EventDate,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out eventDate))
                    {
                        return false;
                    }

                    return eventDate.Year ==
                               month.Year &&
                           eventDate.Month ==
                               month.Month;
                })
                .ToList();

            CalendarEventsDataGrid.ItemsSource =
                monthEvents;

            MonthTitleText.Text =
                month.ToString(
                    "MMMM yyyy");

            FilterDescriptionText.Text =
                "Events in this month (" +
                monthEvents.Count +
                ")";
        }


        // =====================================================
        // SHOW ONE DATE
        // =====================================================

        private void ShowEventsForDate(
            DateTime selectedDate)
        {
            RefreshCalendarData();

            string dateText =
                selectedDate.ToString(
                    "yyyy-MM-dd");

            List<VolunteerEvent> dateEvents =
                allEvents
                .Where(volunteerEvent =>
                    volunteerEvent.EventDate ==
                    dateText)
                .ToList();

            CalendarEventsDataGrid.ItemsSource =
                dateEvents;

            MonthTitleText.Text =
                selectedDate.ToString(
                    "dd MMMM yyyy");

            FilterDescriptionText.Text =
                "Events on this date (" +
                dateEvents.Count +
                ")";
        }


        // =====================================================
        // REFRESH EVENTS + USER REGISTRATIONS
        // =====================================================

        private void RefreshCalendarData()
        {
            allEvents =
                DatabaseHelper.GetAllEvents();

            LoadRegisteredEventIds();

            RefreshCalendarDayButtons();
        }


        // =====================================================
        // CALENDAR DAY COLORS
        // =====================================================

        private void CalendarDayButton_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            CalendarDayButton button =
                sender as CalendarDayButton;

            ApplyCalendarDayStyle(
                button);
        }


        private void CalendarDayButton_DataContextChanged(
            object sender,
            DependencyPropertyChangedEventArgs e)
        {
            CalendarDayButton button =
                sender as CalendarDayButton;

            ApplyCalendarDayStyle(
                button);
        }


        private void ApplyCalendarDayStyle(
            CalendarDayButton button)
        {
            if (button == null)
            {
                return;
            }

            DateTime? buttonDate =
                GetCalendarButtonDate(
                    button);

            if (buttonDate == null)
            {
                return;
            }

            string dateText =
                buttonDate.Value.ToString(
                    "yyyy-MM-dd");

            List<VolunteerEvent> eventsOnDate =
                allEvents
                .Where(volunteerEvent =>
                    volunteerEvent.EventDate ==
                    dateText)
                .ToList();


            // ---------------------------------------------
            // DEFAULT - NO EVENT
            // ---------------------------------------------

            button.Background =
                Brushes.White;

            button.Foreground =
                new SolidColorBrush(
                    Color.FromRgb(
                        38,
                        58,
                        52));

            button.BorderBrush =
                Brushes.Transparent;

            button.FontWeight =
                FontWeights.Normal;

            button.ToolTip =
                null;


            if (eventsOnDate.Count == 0)
            {
                return;
            }


            bool userJoinedEvent =
                eventsOnDate
                .Any(volunteerEvent =>
                    registeredEventIds
                    .Contains(
                        volunteerEvent.Id));


            // ---------------------------------------------
            // GREEN - USER JOINED
            // ---------------------------------------------

            if (userJoinedEvent)
            {
                button.Background =
                    new SolidColorBrush(
                        Color.FromRgb(
                            191,
                            232,
                            214));

                button.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(
                            5,
                            98,
                            78));

                button.BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(
                            8,
                            122,
                            97));

                button.FontWeight =
                    FontWeights.Bold;

                button.ToolTip =
                    BuildCalendarTooltip(
                        eventsOnDate,
                        true);

                return;
            }


            // ---------------------------------------------
            // ORANGE - EVENT AVAILABLE
            // ---------------------------------------------

            button.Background =
                new SolidColorBrush(
                    Color.FromRgb(
                        255,
                        228,
                        174));

            button.Foreground =
                new SolidColorBrush(
                    Color.FromRgb(
                        157,
                        97,
                        8));

            button.BorderBrush =
                new SolidColorBrush(
                    Color.FromRgb(
                        217,
                        149,
                        35));

            button.FontWeight =
                FontWeights.SemiBold;

            button.ToolTip =
                BuildCalendarTooltip(
                    eventsOnDate,
                    false);
        }


        private DateTime? GetCalendarButtonDate(
            CalendarDayButton button)
        {
            if (button.DataContext
                is DateTime)
            {
                return
                    (DateTime)
                    button.DataContext;
            }

            return null;
        }


        private string BuildCalendarTooltip(
            List<VolunteerEvent> events,
            bool hasJoinedEvent)
        {
            string title =
                hasJoinedEvent
                    ? "You joined an event on this date:"
                    : "Available event(s) on this date:";

            string eventNames =
                string.Join(
                    "\n",
                    events.Select(
                        volunteerEvent =>
                            "• " +
                            volunteerEvent.Title));

            return title +
                   "\n" +
                   eventNames;
        }


        // =====================================================
        // FORCE VISIBLE DATE BUTTONS TO REFRESH
        // =====================================================

        private void RefreshCalendarDayButtons()
        {
            EventCalendar.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    List<CalendarDayButton>
                        dayButtons =
                            FindVisualChildren
                                <CalendarDayButton>(
                                    EventCalendar)
                            .ToList();

                    foreach (
                        CalendarDayButton button
                        in dayButtons)
                    {
                        ApplyCalendarDayStyle(
                            button);
                    }
                }));
        }


        private static IEnumerable<T>
            FindVisualChildren<T>(
                DependencyObject dependencyObject)
            where T : DependencyObject
        {
            if (dependencyObject == null)
            {
                yield break;
            }

            int childrenCount =
                VisualTreeHelper
                .GetChildrenCount(
                    dependencyObject);

            for (int i = 0;
                 i < childrenCount;
                 i++)
            {
                DependencyObject child =
                    VisualTreeHelper
                        .GetChild(
                            dependencyObject,
                            i);

                T typedChild =
                    child as T;

                if (typedChild != null)
                {
                    yield return
                        typedChild;
                }

                foreach (
                    T childOfChild
                    in FindVisualChildren<T>(
                        child))
                {
                    yield return
                        childOfChild;
                }
            }
        }


        // =====================================================
        // CALENDAR EVENTS
        // =====================================================

        private void EventCalendar_SelectedDatesChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (EventCalendar.SelectedDate !=
                null)
            {
                ShowEventsForDate(
                    EventCalendar
                    .SelectedDate
                    .Value);
            }
        }


        private void EventCalendar_DisplayDateChanged(
            object sender,
            CalendarDateChangedEventArgs e)
        {
            ShowEventsForMonth(
                EventCalendar.DisplayDate);

            RefreshCalendarDayButtons();
        }


        private void ShowMonthButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowEventsForMonth(
                EventCalendar.DisplayDate);
        }


        // =====================================================
        // OPEN EVENT
        // =====================================================

        private void ViewEventButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            VolunteerEvent selectedEvent =
                CalendarEventsDataGrid
                .SelectedItem
                as VolunteerEvent;

            if (selectedEvent == null)
            {
                MessageBox.Show(
                    "Please select an event first.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            EventDetailsWindow detailsWindow =
                new EventDetailsWindow(
                    currentUser,
                    selectedEvent);

            detailsWindow.Show();

            this.Close();
        }


        // =====================================================
        // NAVIGATION
        // =====================================================

        private void EventsButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            EventsWindow eventsWindow =
                new EventsWindow(
                    currentUser);

            eventsWindow.Show();

            this.Close();
        }


        private void DashboardButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            DashboardWindow dashboard =
                new DashboardWindow(
                    currentUser);

            dashboard.Show();

            this.Close();
        }
    }
}