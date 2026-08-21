using System.Windows;
using VolenteerHub.Data;
using VolenteerHub.Models;

namespace VolenteerHub.Views
{
    public partial class AdminDashboardWindow : Window
    {
        private User currentUser;


        public AdminDashboardWindow(
            User user)
        {
            InitializeComponent();

            currentUser = user;

            LoadDashboard();
        }


        // =====================================================
        // LOAD DASHBOARD
        // =====================================================

        private void LoadDashboard()
        {
            if (currentUser != null)
            {
                AdminWelcomeText.Text =
                    "Welcome, " +
                    currentUser.FullName +
                    ". Manage VolunteerHub users, verification and platform activity.";
            }


            TotalUsersText.Text =
                AdminHelper
                    .GetTotalUserCount()
                    .ToString();


            VolunteerCountText.Text =
                AdminHelper
                    .GetVolunteerCount()
                    .ToString();


            OrganizerCountText.Text =
                AdminHelper
                    .GetOrganizerCount()
                    .ToString();


            PendingVerificationText.Text =
                AdminHelper
                    .GetPendingVerificationCount()
                    .ToString();
        }


        // =====================================================
        // MANAGE USERS
        // =====================================================

        private void ManageUsersButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            AdminUsersWindow usersWindow =
                new AdminUsersWindow(
                    currentUser);


            usersWindow.Show();


            this.Close();
        }


        // =====================================================
        // VERIFICATION
        // =====================================================

        private void VerificationButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            AdminVerificationWindow verificationWindow =
                new AdminVerificationWindow(
                    currentUser);


            verificationWindow.Show();


            this.Close();
        }


        // =====================================================
        // VIEW EVENTS
        // =====================================================

        private void ViewEventsButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            EventsWindow eventsWindow =
                new EventsWindow(
                    currentUser);


            eventsWindow.Show();


            this.Close();
        }


        // =====================================================
        // LOG OUT
        // =====================================================

        private void LogoutButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    "Do you want to log out?",
                    "VolunteerHub",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (result !=
                MessageBoxResult.Yes)
            {
                return;
            }


            MainWindow login =
                new MainWindow();


            login.Show();


            this.Close();
        }
    }
}