using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using VolenteerHub.Data;
using VolenteerHub.Models;

namespace VolenteerHub.Views
{
    public partial class ManageEventWindow : Window
    {
        private User currentUser;

        private VolunteerEvent selectedEvent;

        private OrganizerParticipant selectedParticipant;

        public ManageEventWindow(
            User user)
        {
            InitializeComponent();

            currentUser = user;

            LoadOrganizerEvents();
        }

        private void LoadOrganizerEvents()
        {
            List<VolunteerEvent> organizerEvents =
                DatabaseHelper.GetOrganizerEvents(
                    currentUser.Id);

            EventComboBox.ItemsSource =
                organizerEvents;

            selectedEvent = null;
            selectedParticipant = null;

            ParticipantsDataGrid.ItemsSource =
                null;

            HoursTextBox.Text =
                "";

            SelectedVolunteerText.Text =
                "Select a volunteer to register hours.";

            if (organizerEvents.Count > 0)
            {
                EventComboBox.SelectedIndex =
                    0;
            }
            else
            {
                ParticipantsTitleText.Text =
                    "You have not created any events yet.";
            }
        }

        private void EventComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            selectedEvent =
                EventComboBox.SelectedItem
                as VolunteerEvent;

            LoadParticipants();
        }

        private void LoadParticipants()
        {
            selectedParticipant =
                null;

            SelectedVolunteerText.Text =
                "Select a volunteer to register hours.";

            HoursTextBox.Text =
                "";

            if (selectedEvent == null)
            {
                ParticipantsDataGrid.ItemsSource =
                    null;

                return;
            }

            List<OrganizerParticipant> participants =
                DatabaseHelper.GetEventParticipants(
                    selectedEvent.Id);

            ParticipantsDataGrid.ItemsSource =
                participants;

            ParticipantsTitleText.Text =
                "Registered volunteers - " +
                selectedEvent.Title +
                " (" +
                participants.Count +
                ")";
        }

        private void ParticipantsDataGrid_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            selectedParticipant =
                ParticipantsDataGrid.SelectedItem
                as OrganizerParticipant;

            if (selectedParticipant == null)
            {
                SelectedVolunteerText.Text =
                    "Select a volunteer to register hours.";

                HoursTextBox.Text =
                    "";

                return;
            }

            SelectedVolunteerText.Text =
                "Hours for " +
                selectedParticipant.FullName;

            HoursTextBox.Text =
                selectedParticipant
                .HoursWorked
                .ToString(
                    "0.##",
                    CultureInfo.InvariantCulture);
        }

        private void SaveHoursButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (selectedParticipant == null)
            {
                MessageBox.Show(
                    "Please select a volunteer first.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            double hours;

            bool validHours =
                double.TryParse(
                    HoursTextBox.Text.Trim(),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out hours);

            if (!validHours ||
                hours < 0 ||
                hours > 24)
            {
                MessageBox.Show(
                    "Please enter a valid number of hours between 0 and 24.\nUse a dot for decimals, for example 3.5.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            bool updated =
                DatabaseHelper.UpdateVolunteerHours(
                    selectedParticipant.RegistrationId,
                    hours);

            if (!updated)
            {
                MessageBox.Show(
                    "Something went wrong while saving the volunteer hours.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBox.Show(
                "Volunteer hours saved successfully!",
                "VolunteerHub",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            LoadParticipants();
        }

        private void EditEventButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (selectedEvent == null)
            {
                MessageBox.Show(
                    "Please select an event first.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            EditEventWindow editEventWindow =
                new EditEventWindow(
                    currentUser,
                    selectedEvent);

            editEventWindow.Show();

            this.Close();
        }

        private void DeleteEventButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (selectedEvent == null)
            {
                MessageBox.Show(
                    "Please select an event first.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MessageBoxResult result =
                MessageBox.Show(
                    "Are you sure you want to permanently delete \"" +
                    selectedEvent.Title +
                    "\"?\n\nAll volunteer registrations for this event will also be removed.",
                    "Delete event",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

            if (result !=
                MessageBoxResult.Yes)
            {
                return;
            }

            bool deleted =
                EventManagementHelper.DeleteEvent(
                    selectedEvent.Id,
                    currentUser.Id);

            if (!deleted)
            {
                MessageBox.Show(
                    "The event could not be deleted.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBox.Show(
                "Event deleted successfully.",
                "VolunteerHub",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            LoadOrganizerEvents();
        }

        private void CreateEventButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            CreateEventWindow createEventWindow =
                new CreateEventWindow(
                    currentUser);

            createEventWindow.Show();

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