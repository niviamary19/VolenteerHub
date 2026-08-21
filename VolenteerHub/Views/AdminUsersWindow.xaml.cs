using System;
using System.Collections.Generic;
using System.Windows;
using VolenteerHub.Data;
using VolenteerHub.Models;

namespace VolenteerHub.Views
{
    public partial class AdminUsersWindow : Window
    {
        private User currentUser;


        public AdminUsersWindow(
            User user)
        {
            InitializeComponent();

            currentUser = user;

            LoadUsers();
        }


        // =====================================================
        // LOAD USERS
        // =====================================================

        private void LoadUsers()
        {
            List<User> users =
                AdminHelper
                    .GetAllUsers();


            UsersDataGrid.ItemsSource =
                users;


            UserCountText.Text =
                users.Count.ToString();
        }


        // =====================================================
        // SHOW REFRESH STATUS
        // =====================================================

        private void ShowRefreshStatus(
            string message)
        {
            RefreshStatusText.Text =
                message +
                " at " +
                DateTime.Now.ToString(
                    "HH:mm:ss");


            RefreshStatusPanel.Visibility =
                Visibility.Visible;
        }


        // =====================================================
        // REFRESH
        // =====================================================

        private void RefreshButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            LoadUsers();


            ShowRefreshStatus(
                "User list refreshed");
        }


        // =====================================================
        // DELETE USER
        // =====================================================

        private void DeleteUserButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            User selectedUser =
                UsersDataGrid
                    .SelectedItem
                as User;


            if (selectedUser == null)
            {
                MessageBox.Show(
                    "Please select a user first.",
                    "VolunteerHub Admin",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (selectedUser.Id ==
                currentUser.Id)
            {
                MessageBox.Show(
                    "You cannot delete the admin account you are currently logged in with.",
                    "VolunteerHub Admin",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            MessageBoxResult result =
                MessageBox.Show(
                    "Are you sure you want to permanently delete this account?\n\n" +
                    "Name: " +
                    selectedUser.FullName +
                    "\n" +
                    "Email: " +
                    selectedUser.Email +
                    "\n" +
                    "Role: " +
                    selectedUser.Role +
                    "\n\n" +
                    "This action cannot be undone.",
                    "Delete user",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);


            if (result !=
                MessageBoxResult.Yes)
            {
                return;
            }


            bool deleted =
                AdminHelper
                    .DeleteUser(
                        selectedUser);


            if (!deleted)
            {
                MessageBox.Show(
                    "The user could not be deleted.",
                    "VolunteerHub Admin",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }


            MessageBox.Show(
                "The user account was deleted successfully.",
                "VolunteerHub Admin",
                MessageBoxButton.OK,
                MessageBoxImage.Information);


            LoadUsers();


            ShowRefreshStatus(
                "User list updated");
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
        // DASHBOARD
        // =====================================================

        private void DashboardButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            AdminDashboardWindow dashboard =
                new AdminDashboardWindow(
                    currentUser);


            dashboard.Show();


            this.Close();
        }
    }
}