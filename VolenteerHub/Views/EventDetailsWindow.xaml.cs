using System;
using System.Diagnostics;
using System.Windows;
using VolenteerHub.Data;
using VolenteerHub.Models;

namespace VolenteerHub.Views
{
    public partial class EventDetailsWindow : Window
    {
        private User currentUser;

        private VolunteerEvent currentEvent;


        public EventDetailsWindow(
            User user,
            VolunteerEvent volunteerEvent)
        {
            InitializeComponent();

            currentUser = user;

            currentEvent = volunteerEvent;

            LoadEventInformation();
        }


        // =====================================================
        // LOAD EVENT
        // =====================================================

        private void LoadEventInformation()
        {
            TitleText.Text =
                currentEvent.Title;


            CategoryText.Text =
                currentEvent.Category;


            DescriptionText.Text =
                currentEvent.Description;


            DateText.Text =
                currentEvent.EventDate;


            TimeText.Text =
                currentEvent.StartTime +
                " - " +
                currentEvent.EndTime;


            LocationText.Text =
                currentEvent.Location;


            RefreshRegistrationState();
        }


        // =====================================================
        // REGISTRATION STATE
        // =====================================================

        private void RefreshRegistrationState()
        {
            int registrations =
                DatabaseHelper
                    .GetRegistrationCount(
                        currentEvent.Id);


            PlacesText.Text =
                registrations +
                " / " +
                currentEvent.MaxVolunteers;


            SignUpButton.Visibility =
                Visibility.Visible;


            CancelRegistrationButton.Visibility =
                Visibility.Collapsed;


            SignUpButton.IsEnabled =
                true;


            SignUpButton.Content =
                "Sign up for this event";


            StatusText.Text =
                "";


            if (currentUser.Role !=
                "Volunteer")
            {
                SignUpButton.Visibility =
                    Visibility.Collapsed;


                CancelRegistrationButton.Visibility =
                    Visibility.Collapsed;


                StatusText.Text =
                    "Only volunteer accounts can sign up for events.";


                return;
            }


            bool alreadyRegistered =
                DatabaseHelper
                    .IsUserRegistered(
                        currentUser.Id,
                        currentEvent.Id);


            if (alreadyRegistered)
            {
                SignUpButton.Visibility =
                    Visibility.Collapsed;


                CancelRegistrationButton.Visibility =
                    Visibility.Visible;


                StatusText.Text =
                    "You are registered for this event.";


                return;
            }


            if (registrations >=
                currentEvent.MaxVolunteers)
            {
                SignUpButton.IsEnabled =
                    false;


                SignUpButton.Content =
                    "Event is full";


                StatusText.Text =
                    "There are no available places left.";
            }
        }


        // =====================================================
        // GOOGLE MAPS
        // =====================================================

        private void ViewMapButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(
                    currentEvent.Location))
            {
                MessageBox.Show(
                    "No location is available for this event.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            string googleMapsUrl =
                "https://www.google.com/maps/search/?api=1&query=" +
                Uri.EscapeDataString(
                    currentEvent.Location);


            try
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName =
                            googleMapsUrl,

                        UseShellExecute =
                            true
                    });
            }
            catch
            {
                MessageBox.Show(
                    "Google Maps could not be opened.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // SIGN UP
        // =====================================================

        private void SignUpButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (currentUser.Role !=
                "Volunteer")
            {
                return;
            }


            if (DatabaseHelper
                    .IsUserRegistered(
                        currentUser.Id,
                        currentEvent.Id))
            {
                MessageBox.Show(
                    "You are already registered for this event.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                RefreshRegistrationState();


                return;
            }


            int registrations =
                DatabaseHelper
                    .GetRegistrationCount(
                        currentEvent.Id);


            if (registrations >=
                currentEvent.MaxVolunteers)
            {
                MessageBox.Show(
                    "This event is already full.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);


                RefreshRegistrationState();


                return;
            }


            bool registered =
                DatabaseHelper
                    .RegisterForEvent(
                        currentUser.Id,
                        currentEvent.Id);


            if (!registered)
            {
                MessageBox.Show(
                    "Something went wrong while registering for the event.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }


            MessageBox.Show(
                "You successfully signed up for " +
                currentEvent.Title +
                "!",
                "VolunteerHub",
                MessageBoxButton.OK,
                MessageBoxImage.Information);


            RefreshRegistrationState();
        }


        // =====================================================
        // CANCEL REGISTRATION
        // =====================================================

        private void CancelRegistrationButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    "Are you sure you want to cancel your registration for " +
                    currentEvent.Title +
                    "?",
                    "Cancel registration",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (result !=
                MessageBoxResult.Yes)
            {
                return;
            }


            bool cancelled =
                DatabaseHelper
                    .CancelRegistration(
                        currentUser.Id,
                        currentEvent.Id);


            if (!cancelled)
            {
                MessageBox.Show(
                    "Your registration could not be cancelled.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }


            MessageBox.Show(
                "Your registration has been cancelled.",
                "VolunteerHub",
                MessageBoxButton.OK,
                MessageBoxImage.Information);


            RefreshRegistrationState();
        }


        // =====================================================
        // BACK
        // =====================================================

        private void BackButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            EventsWindow eventsWindow =
                new EventsWindow(
                    currentUser);


            eventsWindow.Show();


            this.Close();
        }
    }
}