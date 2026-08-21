using System.Collections.Generic;
using System.Windows;
using VolenteerHub.Data;
using VolenteerHub.Models;

namespace VolenteerHub.Views
{
    public partial class AdminVerificationWindow : Window
    {
        private User currentUser;


        public AdminVerificationWindow(
            User user)
        {
            InitializeComponent();

            currentUser = user;

            LoadRequests();
        }


        // =====================================================
        // LOAD REQUESTS
        // =====================================================

        private void LoadRequests()
        {
            List<VerificationRequest> requests =
                VerificationHelper
                    .GetPendingVerificationRequests();

            VerificationDataGrid.ItemsSource =
                requests;

            PendingCountText.Text =
                requests.Count.ToString();
        }


        // =====================================================
        // APPROVE
        // =====================================================

        private void ApproveButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            VerificationRequest selectedRequest =
                VerificationDataGrid.SelectedItem
                as VerificationRequest;


            if (selectedRequest == null)
            {
                MessageBox.Show(
                    "Please select a verification request first.",
                    "VolunteerHub Admin",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            MessageBoxResult confirmation =
                MessageBox.Show(
                    "Approve identity verification for:\n\n" +
                    selectedRequest.FullName +
                    "\n" +
                    selectedRequest.Email +
                    "?",
                    "Approve verification",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (confirmation !=
                MessageBoxResult.Yes)
            {
                return;
            }


            bool approved =
                VerificationHelper.SetVerified(
                    selectedRequest.UserId);


            if (!approved)
            {
                MessageBox.Show(
                    "The verification could not be approved.",
                    "VolunteerHub Admin",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }


            MessageBox.Show(
                selectedRequest.FullName +
                " is now a Verified Volunteer!",
                "VolunteerHub Admin",
                MessageBoxButton.OK,
                MessageBoxImage.Information);


            LoadRequests();
        }


        // =====================================================
        // REJECT
        // =====================================================

        private void RejectButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            VerificationRequest selectedRequest =
                VerificationDataGrid.SelectedItem
                as VerificationRequest;


            if (selectedRequest == null)
            {
                MessageBox.Show(
                    "Please select a verification request first.",
                    "VolunteerHub Admin",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            MessageBoxResult confirmation =
                MessageBox.Show(
                    "Reject identity verification for:\n\n" +
                    selectedRequest.FullName +
                    "\n" +
                    selectedRequest.Email +
                    "?",
                    "Reject verification",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);


            if (confirmation !=
                MessageBoxResult.Yes)
            {
                return;
            }


            bool rejected =
                VerificationHelper.SetRejected(
                    selectedRequest.UserId);


            if (!rejected)
            {
                MessageBox.Show(
                    "The verification could not be rejected.",
                    "VolunteerHub Admin",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }


            MessageBox.Show(
                "The verification request was rejected.",
                "VolunteerHub Admin",
                MessageBoxButton.OK,
                MessageBoxImage.Information);


            LoadRequests();
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