using System.Collections.Generic;
using System.Linq;
using System.Windows;
using VolenteerHub.Data;
using VolenteerHub.Models;

namespace VolenteerHub.Views
{
    public partial class EventsWindow : Window
    {
        private User currentUser;

        private List<VolunteerEvent> allEvents;

        public EventsWindow(User user)
        {
            InitializeComponent();

            currentUser = user;

            LoadEvents();
        }

        private void LoadEvents()
        {
            allEvents =
                DatabaseHelper.GetAllEvents();

            EventsDataGrid.ItemsSource =
                allEvents;

            ResultText.Text =
                "Available events (" +
                allEvents.Count +
                ")";
        }

        private void SearchButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string searchText =
                SearchTextBox.Text
                .Trim()
                .ToLower();

            if (string.IsNullOrWhiteSpace(
                searchText))
            {
                EventsDataGrid.ItemsSource =
                    allEvents;

                ResultText.Text =
                    "Available events (" +
                    allEvents.Count +
                    ")";

                return;
            }

            List<VolunteerEvent> filteredEvents =
                allEvents
                .Where(volunteerEvent =>
                    volunteerEvent.Title
                        .ToLower()
                        .Contains(searchText) ||

                    volunteerEvent.Category
                        .ToLower()
                        .Contains(searchText) ||

                    volunteerEvent.Location
                        .ToLower()
                        .Contains(searchText) ||

                    volunteerEvent.Description
                        .ToLower()
                        .Contains(searchText))
                .ToList();

            EventsDataGrid.ItemsSource =
                filteredEvents;

            ResultText.Text =
                "Search results (" +
                filteredEvents.Count +
                ")";
        }

        private void ViewEventButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            VolunteerEvent selectedEvent =
                EventsDataGrid.SelectedItem
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

        private void DashboardButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            DashboardWindow dashboard =
                new DashboardWindow(currentUser);

            dashboard.Show();

            this.Close();
        }
    }
}