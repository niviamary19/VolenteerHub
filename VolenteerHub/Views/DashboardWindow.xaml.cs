using System.Windows;
using VolenteerHub.Data;
using VolenteerHub.Models;

namespace VolenteerHub.Views
{
    public partial class DashboardWindow : Window
    {
        private User currentUser;

        public DashboardWindow(
            User user)
        {
            InitializeComponent();

            currentUser = user;

            LoadUserInformation();

            LoadDashboardStatistics();
        }

        private void LoadUserInformation()
        {
            WelcomeText.Text =
                "Welcome back, " +
                currentUser.FullName +
                "!";

            RoleText.Text =
                "Logged in as " +
                currentUser.Role;

            UserNameText.Text =
                currentUser.FullName;

            UserEmailText.Text =
                currentUser.Email;

            if (currentUser.Role == "Organizer")
            {
                CreateEventButton.Visibility =
                    Visibility.Visible;

                ManageEventsButton.Visibility =
                    Visibility.Visible;
            }
            else
            {
                CreateEventButton.Visibility =
                    Visibility.Collapsed;

                ManageEventsButton.Visibility =
                    Visibility.Collapsed;
            }
        }

        private void LoadDashboardStatistics()
        {
            int upcomingEvents =
                DatabaseHelper.GetUpcomingEventCount(
                    currentUser.Id);

            int joinedEvents =
                DatabaseHelper.GetJoinedEventCount(
                    currentUser.Id);

            double totalHours =
                DatabaseHelper.GetTotalVolunteerHours(
                    currentUser.Id);

            UpcomingEventsCountText.Text =
                upcomingEvents.ToString();

            JoinedEventsCountText.Text =
                joinedEvents.ToString();

            VolunteerHoursText.Text =
                totalHours.ToString("0.##") +
                " h";
        }

        private void CalendarButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            CalendarWindow calendarWindow =
                new CalendarWindow(
                    currentUser);

            calendarWindow.Show();

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

        private void MyHoursButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MyHoursWindow myHoursWindow =
                new MyHoursWindow(
                    currentUser);

            myHoursWindow.Show();

            this.Close();
        }

        private void ProfileButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ProfileWindow profileWindow =
                new ProfileWindow(
                    currentUser);

            profileWindow.Show();

            this.Close();
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

        private void ManageEventsButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            ManageEventWindow manageEventWindow =
                new ManageEventWindow(
                    currentUser);

            manageEventWindow.Show();

            this.Close();
        }

        private void LogoutButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MainWindow loginWindow =
                new MainWindow();

            loginWindow.Show();

            this.Close();
        }
    }
}