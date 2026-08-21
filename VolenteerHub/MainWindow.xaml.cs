using System;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using VolenteerHub.Data;
using VolenteerHub.Models;
using VolenteerHub.Views;

namespace VolenteerHub
{
    public partial class MainWindow : Window
    {
        private static readonly string DatabasePath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "VolunteerHub.db");

        private static readonly string ConnectionString =
            "Data Source=" +
            DatabasePath +
            ";Version=3;";


        public MainWindow()
        {
            InitializeComponent();

            DatabaseHelper.InitializeDatabase();

            VerificationHelper.InitializeVerificationTable();

            CreateDefaultAdminAccount();
        }


        // =====================================================
        // CREATE DEFAULT ADMIN
        // =====================================================

        private void CreateDefaultAdminAccount()
        {
            string adminEmail =
                "admin@volunteerhub.com";

            // If the admin already exists, do nothing.
            if (DatabaseHelper.EmailExists(
                    adminEmail))
            {
                return;
            }

            // We use the normal registration logic once,
            // but the role is forced to Admin here in code.
            DatabaseHelper.RegisterUser(
                "VolunteerHub Admin",
                adminEmail,
                "Admin123!",
                "Admin");
        }


        // =====================================================
        // LOGIN
        // =====================================================

        private void LoginButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string email =
                EmailTextBox.Text.Trim();

            string password =
                PasswordInput.Password;


            if (string.IsNullOrWhiteSpace(
                    email) ||
                string.IsNullOrWhiteSpace(
                    password))
            {
                MessageBox.Show(
                    "Please enter your email and password.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            User user =
                DatabaseHelper.LoginUser(
                    email,
                    password);


            if (user == null)
            {
                MessageBox.Show(
                    "Incorrect email or password.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            // =================================================
            // ADMIN
            // =================================================

            if (user.Role.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
            {
                AdminDashboardWindow adminDashboard =
                    new AdminDashboardWindow(
                        user);

                adminDashboard.Show();

                this.Close();

                return;
            }


            // =================================================
            // VOLUNTEER / ORGANIZER
            // =================================================
            // Your existing DashboardWindow already handles
            // volunteer and organizer functionality.

            DashboardWindow dashboardWindow =
                new DashboardWindow(
                    user);

            dashboardWindow.Show();

            this.Close();
        }


        // =====================================================
        // REGISTER
        // =====================================================

        private void RegisterButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            RegisterWindow registerWindow =
                new RegisterWindow();

            registerWindow.Show();

            this.Close();
        }
    }
}