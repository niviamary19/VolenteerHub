using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VolenteerHub.Data;
using VolenteerHub.Models;

namespace VolenteerHub.Views
{
    public partial class CalendarWindow : Window
    {
        private User currentUser;

        private List<VolunteerEvent> allEvents;

        public CalendarWindow(
            User user)
        {
            InitializeComponent();

            currentUser = user;

            allEvents =
                DatabaseHelper.GetAllEvents();

            EventCalendar.DisplayDate =
                DateTime.Today;

            EventCalendar.SelectedDate =
                DateTime.Today;

            ShowEventsForMonth(
                DateTime.Today);
        }

        private void ShowEventsForMonth(
            DateTime month)
        {
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

        private void ShowEventsForDate(
            DateTime selectedDate)
        {
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

        private void EventCalendar_SelectedDatesChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (EventCalendar.SelectedDate != null)
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
        }

        private void ShowMonthButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ShowEventsForMonth(
                EventCalendar.DisplayDate);
        }

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