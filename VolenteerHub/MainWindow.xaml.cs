using System.Windows;
using VolenteerHub.Data;
using VolenteerHub.Models;
using VolenteerHub.Views;

namespace VolenteerHub
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DatabaseHelper.InitializeDatabase();
        }

        private void LoginButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string email =
                EmailTextBox.Text.Trim();

            string password =
                PasswordInput.Password;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
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

            DashboardWindow dashboardWindow =
                new DashboardWindow(user);

            dashboardWindow.Show();

            this.Close();
        }

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