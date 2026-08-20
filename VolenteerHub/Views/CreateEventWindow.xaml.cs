using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using VolenteerHub.Data;
using VolenteerHub.Models;

namespace VolenteerHub.Views
{
    public partial class CreateEventWindow : Window
    {
        private User currentUser;

        public CreateEventWindow(User user)
        {
            InitializeComponent();

            currentUser = user;

            EventDatePicker.SelectedDate =
                DateTime.Today.AddDays(1);
        }

        private void CreateEventButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string title =
                TitleTextBox.Text.Trim();

            string description =
                DescriptionTextBox.Text.Trim();

            string startTime =
                StartTimeTextBox.Text.Trim();

            string endTime =
                EndTimeTextBox.Text.Trim();

            string location =
                LocationTextBox.Text.Trim();

            ComboBoxItem selectedCategory =
                CategoryComboBox.SelectedItem
                as ComboBoxItem;

            string category =
                selectedCategory == null
                    ? ""
                    : selectedCategory.Content.ToString();

            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(description) ||
                EventDatePicker.SelectedDate == null ||
                string.IsNullOrWhiteSpace(startTime) ||
                string.IsNullOrWhiteSpace(endTime) ||
                string.IsNullOrWhiteSpace(category) ||
                string.IsNullOrWhiteSpace(location) ||
                string.IsNullOrWhiteSpace(
                    LatitudeTextBox.Text) ||
                string.IsNullOrWhiteSpace(
                    LongitudeTextBox.Text) ||
                string.IsNullOrWhiteSpace(
                    MaxVolunteersTextBox.Text))
            {
                MessageBox.Show(
                    "Please fill in all fields.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            DateTime startTimeValue;
            DateTime endTimeValue;

            bool validStartTime =
                DateTime.TryParseExact(
                    startTime,
                    "HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out startTimeValue);

            bool validEndTime =
                DateTime.TryParseExact(
                    endTime,
                    "HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out endTimeValue);

            if (!validStartTime ||
                !validEndTime)
            {
                MessageBox.Show(
                    "Please enter the time as HH:mm.\nExample: 09:30",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (endTimeValue <= startTimeValue)
            {
                MessageBox.Show(
                    "The end time must be later than the start time.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            double latitude;
            double longitude;

            bool validLatitude =
                double.TryParse(
                    LatitudeTextBox.Text.Trim(),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out latitude);

            bool validLongitude =
                double.TryParse(
                    LongitudeTextBox.Text.Trim(),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out longitude);

            if (!validLatitude ||
                !validLongitude)
            {
                MessageBox.Show(
                    "Latitude and longitude must be valid numbers.\nUse a dot for decimals.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (latitude < -90 ||
                latitude > 90 ||
                longitude < -180 ||
                longitude > 180)
            {
                MessageBox.Show(
                    "The coordinates are outside the valid range.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            int maxVolunteers;

            if (!int.TryParse(
                    MaxVolunteersTextBox.Text.Trim(),
                    out maxVolunteers) ||
                maxVolunteers <= 0)
            {
                MessageBox.Show(
                    "Maximum volunteers must be a number greater than 0.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            DateTime eventDate =
                EventDatePicker.SelectedDate.Value;

            if (eventDate.Date <
                DateTime.Today)
            {
                MessageBox.Show(
                    "You cannot create an event in the past.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            VolunteerEvent volunteerEvent =
                new VolunteerEvent
                {
                    Title =
                        title,

                    Description =
                        description,

                    EventDate =
                        eventDate.ToString(
                            "yyyy-MM-dd"),

                    StartTime =
                        startTime,

                    EndTime =
                        endTime,

                    Category =
                        category,

                    Location =
                        location,

                    Latitude =
                        latitude,

                    Longitude =
                        longitude,

                    MaxVolunteers =
                        maxVolunteers,

                    OrganizerId =
                        currentUser.Id
                };

            bool created =
                DatabaseHelper.CreateEvent(
                    volunteerEvent);

            if (!created)
            {
                MessageBox.Show(
                    "Something went wrong while creating the event.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBox.Show(
                "Event created successfully!",
                "VolunteerHub",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DashboardWindow dashboard =
                new DashboardWindow(
                    currentUser);

            dashboard.Show();

            this.Close();
        }

        private void CancelButton_Click(
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