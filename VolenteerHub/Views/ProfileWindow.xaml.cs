using System.Windows;
using VolenteerHub.Data;
using VolenteerHub.Models;

namespace VolenteerHub.Views
{
    public partial class ProfileWindow : Window
    {
        private User currentUser;

        public ProfileWindow(
            User user)
        {
            InitializeComponent();

            currentUser = user;

            LoadProfile();
        }

        private void LoadProfile()
        {
            FullNameTextBox.Text =
                currentUser.FullName;

            EmailTextBox.Text =
                currentUser.Email;

            RoleText.Text =
                currentUser.Role;

            CreatedAtText.Text =
                currentUser.CreatedAt;

            double totalHours =
                DatabaseHelper.GetTotalVolunteerHours(
                    currentUser.Id);

            int joinedEvents =
                DatabaseHelper.GetJoinedEventCount(
                    currentUser.Id);

            TotalHoursText.Text =
                totalHours.ToString("0.##") +
                " h";

            JoinedEventsText.Text =
                joinedEvents.ToString();
        }

        private void SaveProfileButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string fullName =
                FullNameTextBox.Text.Trim();

            string email =
                EmailTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                    fullName) ||
                string.IsNullOrWhiteSpace(
                    email))
            {
                MessageBox.Show(
                    "Please enter your name and email address.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!email.Contains("@") ||
                !email.Contains("."))
            {
                MessageBox.Show(
                    "Please enter a valid email address.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            bool updated =
                ProfileHelper.UpdateProfile(
                    currentUser,
                    fullName,
                    email);

            if (!updated)
            {
                MessageBox.Show(
                    "The profile could not be updated.\nThe email address may already be in use.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MessageBox.Show(
                "Your profile was updated successfully!",
                "VolunteerHub",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            LoadProfile();
        }

        private void ChangePasswordButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string currentPassword =
                CurrentPasswordInput.Password;

            string newPassword =
                NewPasswordInput.Password;

            string confirmPassword =
                ConfirmPasswordInput.Password;

            if (string.IsNullOrWhiteSpace(
                    currentPassword) ||
                string.IsNullOrWhiteSpace(
                    newPassword) ||
                string.IsNullOrWhiteSpace(
                    confirmPassword))
            {
                MessageBox.Show(
                    "Please fill in all password fields.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (newPassword.Length < 6)
            {
                MessageBox.Show(
                    "Your new password must contain at least 6 characters.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (newPassword !=
                confirmPassword)
            {
                MessageBox.Show(
                    "The new passwords do not match.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            bool changed =
                ProfileHelper.ChangePassword(
                    currentUser,
                    currentPassword,
                    newPassword);

            if (!changed)
            {
                MessageBox.Show(
                    "Your current password is incorrect.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            CurrentPasswordInput.Password = "";
            NewPasswordInput.Password = "";
            ConfirmPasswordInput.Password = "";

            MessageBox.Show(
                "Your password was changed successfully!",
                "VolunteerHub",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void DeleteAccountButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    "Are you sure you want to permanently delete your account?\n\nThis action cannot be undone.",
                    "Delete account",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

            if (result !=
                MessageBoxResult.Yes)
            {
                return;
            }

            bool deleted =
                ProfileHelper.DeleteAccount(
                    currentUser);

            if (!deleted)
            {
                MessageBox.Show(
                    "The account could not be deleted.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBox.Show(
                "Your account has been deleted.",
                "VolunteerHub",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            MainWindow loginWindow =
                new MainWindow();

            loginWindow.Show();

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