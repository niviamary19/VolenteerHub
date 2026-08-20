using System.Windows;
using System.Windows.Controls;
using VolenteerHub.Data;

namespace VolenteerHub.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();

            DatabaseHelper.InitializeDatabase();
        }

        private void RegisterButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string name =
                NameTextBox.Text.Trim();

            string email =
                EmailTextBox.Text.Trim();

            string password =
                PasswordInput.Password;

            string confirmPassword =
                ConfirmPasswordInput.Password;

            ComboBoxItem selectedRole =
                RoleComboBox.SelectedItem as ComboBoxItem;

            string role =
                selectedRole == null
                    ? ""
                    : selectedRole.Content.ToString();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword) ||
                string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show(
                    "Please fill in all fields.",
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

            if (password.Length < 6)
            {
                MessageBox.Show(
                    "Your password must contain at least 6 characters.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show(
                    "The passwords do not match.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (DatabaseHelper.EmailExists(email))
            {
                MessageBox.Show(
                    "An account with this email address already exists.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            bool accountCreated =
                DatabaseHelper.RegisterUser(
                    name,
                    email,
                    password,
                    role);

            if (!accountCreated)
            {
                MessageBox.Show(
                    "Something went wrong while creating your account.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MessageBox.Show(
                "Your account was created successfully!\n\n" +
                "You can now log in.",
                "VolunteerHub",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            VolenteerHub.MainWindow loginWindow =
                new VolenteerHub.MainWindow();

            loginWindow.Show();

            this.Close();
        }

        private void BackToLogin_Click(
            object sender,
            RoutedEventArgs e)
        {
            VolenteerHub.MainWindow loginWindow =
                new VolenteerHub.MainWindow();

            loginWindow.Show();

            this.Close();
        }
    }
}