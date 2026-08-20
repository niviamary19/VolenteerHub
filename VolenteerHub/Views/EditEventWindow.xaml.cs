using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using VolenteerHub.Data;
using VolenteerHub.Models;

namespace VolenteerHub.Views
{
    public partial class EditEventWindow : Window
    {
        private User currentUser;

        private VolunteerEvent currentEvent;

        public EditEventWindow(
            User user,
            VolunteerEvent volunteerEvent)
        {
            InitializeComponent();

            currentUser = user;
            currentEvent = volunteerEvent;

            LoadEventInformation();
        }

        private void LoadEventInformation()
        {
            TitleTextBox.Text =
                currentEvent.Title;

            DescriptionTextBox.Text =
                currentEvent.Description;

            StartTimeTextBox.Text =
                currentEvent.StartTime;

            EndTimeTextBox.Text =
                currentEvent.EndTime;

            LocationTextBox.Text =
                currentEvent.Location;

            LatitudeTextBox.Text =
                currentEvent.Latitude.ToString(
                    CultureInfo.InvariantCulture);

            LongitudeTextBox.Text =
                currentEvent.Longitude.ToString(
                    CultureInfo.InvariantCulture);

            MaxVolunteersTextBox.Text =
                currentEvent.MaxVolunteers.ToString();

            DateTime eventDate;

            if (DateTime.TryParseExact(
                currentEvent.EventDate,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out eventDate))
            {
                EventDatePicker.SelectedDate =
                    eventDate;
            }

            for (int i = 0;
                 i < CategoryComboBox.Items.Count;
                 i++)
            {
                ComboBoxItem item =
                    CategoryComboBox.Items[i]
                    as ComboBoxItem;

                if (item != null &&
                    item.Content.ToString() ==
                    currentEvent.Category)
                {
                    CategoryComboBox.SelectedIndex =
                        i;

                    break;
                }
            }

            if (CategoryComboBox.SelectedIndex < 0)
            {
                CategoryComboBox.SelectedIndex = 0;
            }
        }

        private void SaveButton_Click(
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

            if (endTimeValue <=
                startTimeValue)
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

            int currentRegistrations =
                DatabaseHelper.GetRegistrationCount(
                    currentEvent.Id);

            if (maxVolunteers <
                currentRegistrations)
            {
                MessageBox.Show(
                    "Maximum volunteers cannot be lower than the number of people already registered.\n\nCurrent registrations: " +
                    currentRegistrations,
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            DateTime eventDate =
                EventDatePicker.SelectedDate.Value;

            currentEvent.Title =
                title;

            currentEvent.Description =
                description;

            currentEvent.EventDate =
                eventDate.ToString(
                    "yyyy-MM-dd");

            currentEvent.StartTime =
                startTime;

            currentEvent.EndTime =
                endTime;

            currentEvent.Category =
                category;

            currentEvent.Location =
                location;

            currentEvent.Latitude =
                latitude;

            currentEvent.Longitude =
                longitude;

            currentEvent.MaxVolunteers =
                maxVolunteers;

            bool updated =
                EventManagementHelper.UpdateEvent(
                    currentEvent,
                    currentUser.Id);

            if (!updated)
            {
                MessageBox.Show(
                    "The event could not be updated.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBox.Show(
                "Event updated successfully!",
                "VolunteerHub",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            ManageEventWindow manageWindow =
                new ManageEventWindow(
                    currentUser);

            manageWindow.Show();

            this.Close();
        }

        private void CancelButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ManageEventWindow manageWindow =
                new ManageEventWindow(
                    currentUser);

            manageWindow.Show();

            this.Close();
        }
    }
}