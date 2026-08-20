using System.Collections.Generic;
using System.Windows;
using VolenteerHub.Data;
using VolenteerHub.Models;

namespace VolenteerHub.Views
{
    public partial class MyHoursWindow : Window
    {
        private User currentUser;

        public MyHoursWindow(
            User user)
        {
            InitializeComponent();

            currentUser = user;

            LoadInformation();
        }

        private void LoadInformation()
        {
            List<MyVolunteerEvent> myEvents =
                DatabaseHelper.GetUserVolunteerEvents(
                    currentUser.Id);

            MyEventsDataGrid.ItemsSource =
                myEvents;

            double totalHours =
                DatabaseHelper.GetTotalVolunteerHours(
                    currentUser.Id);

            int joinedEvents =
                DatabaseHelper.GetJoinedEventCount(
                    currentUser.Id);

            int upcomingEvents =
                DatabaseHelper.GetUpcomingEventCount(
                    currentUser.Id);

            TotalHoursText.Text =
                totalHours.ToString("0.##") +
                " h";

            JoinedEventsText.Text =
                joinedEvents.ToString();

            UpcomingEventsText.Text =
                upcomingEvents.ToString();

            EventsTitleText.Text =
                "My events (" +
                joinedEvents +
                ")";
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
    }
}