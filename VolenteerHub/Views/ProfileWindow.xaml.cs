using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using VolenteerHub.Data;
using VolenteerHub.Models;

namespace VolenteerHub.Views
{
    public partial class ProfileWindow : Window
    {
        private User currentUser;

        private string profilePhotosFolder;


        public ProfileWindow(
            User user)
        {
            InitializeComponent();

            currentUser = user;

            profilePhotosFolder =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "ProfilePhotos");

            CreateProfilePhotosFolder();

            LoadProfile();

            LoadProfilePhoto();

            LoadVerificationStatus();
        }


        // =====================================================
        // PROFILE INFORMATION
        // =====================================================

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
                totalHours.ToString(
                    "0.##") +
                " h";


            JoinedEventsText.Text =
                joinedEvents.ToString();
        }


        // =====================================================
        // VERIFICATION STATUS
        // =====================================================

        private void LoadVerificationStatus()
        {
            try
            {
                string status =
                    VerificationHelper
                        .GetVerificationStatus(
                            currentUser.Id);


                if (status == "Verified")
                {
                    ShowVerifiedStatus();

                    return;
                }


                if (status == "Pending")
                {
                    ShowPendingStatus();

                    return;
                }


                ShowNotVerifiedStatus();
            }
            catch
            {
                ShowNotVerifiedStatus();
            }
        }


        // =====================================================
        // NOT VERIFIED
        // =====================================================

        private void ShowNotVerifiedStatus()
        {
            VerificationStatusBadge.Background =
                new SolidColorBrush(
                    Color.FromRgb(
                        236,
                        239,
                        237));


            VerificationStatusIcon.Text =
                "?";


            VerificationStatusIcon.Foreground =
                new SolidColorBrush(
                    Color.FromRgb(
                        102,
                        115,
                        110));


            VerificationStatusText.Text =
                "Not verified";


            VerificationStatusText.Foreground =
                new SolidColorBrush(
                    Color.FromRgb(
                        82,
                        97,
                        92));


            VerificationDescriptionText.Text =
                "Submit an identity document to start verification.";


            VerifiedAtText.Visibility =
                Visibility.Collapsed;


            ChooseIdDocumentButton.IsEnabled =
                true;


            ChooseIdDocumentButton.Content =
                "Choose ID document";


            GenerateCertificateButton.IsEnabled =
                false;


            CertificateLockText.Visibility =
                Visibility.Visible;


            CertificateLockText.Text =
                "Verify your identity first to unlock your certificate.";
        }


        // =====================================================
        // PENDING
        // =====================================================

        private void ShowPendingStatus()
        {
            VerificationStatusBadge.Background =
                new SolidColorBrush(
                    Color.FromRgb(
                        255,
                        240,
                        213));


            VerificationStatusIcon.Text =
                "…";


            VerificationStatusIcon.Foreground =
                new SolidColorBrush(
                    Color.FromRgb(
                        213,
                        138,
                        24));


            VerificationStatusText.Text =
                "Pending verification";


            VerificationStatusText.Foreground =
                new SolidColorBrush(
                    Color.FromRgb(
                        183,
                        122,
                        26));


            VerificationDescriptionText.Text =
                "Your verification request is waiting for admin approval.";


            VerifiedAtText.Visibility =
                Visibility.Collapsed;


            ChooseIdDocumentButton.IsEnabled =
                true;


            ChooseIdDocumentButton.Content =
                "Submit another ID";


            GenerateCertificateButton.IsEnabled =
                false;


            CertificateLockText.Visibility =
                Visibility.Visible;


            CertificateLockText.Text =
                "Certificate available after verification approval.";
        }


        // =====================================================
        // VERIFIED
        // =====================================================

        private void ShowVerifiedStatus()
        {
            VerificationStatusBadge.Background =
                new SolidColorBrush(
                    Color.FromRgb(
                        221,
                        242,
                        234));


            VerificationStatusIcon.Text =
                "✓";


            VerificationStatusIcon.Foreground =
                new SolidColorBrush(
                    Color.FromRgb(
                        8,
                        122,
                        97));


            VerificationStatusText.Text =
                "Verified Volunteer";


            VerificationStatusText.Foreground =
                new SolidColorBrush(
                    Color.FromRgb(
                        8,
                        122,
                        97));


            VerificationDescriptionText.Text =
                "Your identity has been successfully verified.";


            string verifiedAt =
                VerificationHelper.GetVerifiedAt(
                    currentUser.Id);


            if (!string.IsNullOrWhiteSpace(
                    verifiedAt))
            {
                VerifiedAtText.Text =
                    "Verified on " +
                    verifiedAt;


                VerifiedAtText.Visibility =
                    Visibility.Visible;
            }
            else
            {
                VerifiedAtText.Visibility =
                    Visibility.Collapsed;
            }


            ChooseIdDocumentButton.IsEnabled =
                false;


            ChooseIdDocumentButton.Content =
                "Identity verified";


            GenerateCertificateButton.IsEnabled =
                true;


            CertificateLockText.Visibility =
                Visibility.Collapsed;
        }


        // =====================================================
        // CHOOSE ID DOCUMENT
        // =====================================================

        private void ChooseIdDocumentButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            OpenFileDialog dialog =
                new OpenFileDialog();


            dialog.Title =
                "Choose an identity document";


            dialog.Filter =
                "Supported documents (*.jpg;*.jpeg;*.png;*.pdf)|*.jpg;*.jpeg;*.png;*.pdf";


            dialog.Multiselect =
                false;


            bool? result =
                dialog.ShowDialog();


            if (result != true)
            {
                return;
            }


            MessageBoxResult confirmation =
                MessageBox.Show(
                    "The selected document will be used only to start the verification request.\n\nVolunteerHub will not permanently copy or store the selected ID file in this project.\n\nDo you want to continue?",
                    "Identity verification",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);


            if (confirmation !=
                MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                // The ID file itself is deliberately not stored.
                // Only the verification status is stored.

                VerificationHelper.SetPending(
                    currentUser.Id);


                LoadVerificationStatus();


                MessageBox.Show(
                    "Your identity verification request was submitted successfully!\n\nStatus: Pending verification",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show(
                    "The verification request could not be submitted.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // GENERATE PDF CERTIFICATE
        // =====================================================

        private void GenerateCertificateButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                string status =
                    VerificationHelper
                        .GetVerificationStatus(
                            currentUser.Id);


                if (status !=
                    "Verified")
                {
                    MessageBox.Show(
                        "Your identity must be verified before you can generate a certificate.",
                        "VolunteerHub",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);


                    LoadVerificationStatus();

                    return;
                }


                // ---------------------------------------------
                // GENERATE ACTUAL PDF
                // ---------------------------------------------

                string certificatePath =
                    CertificateHelper
                        .GenerateCertificate(
                            currentUser);


                if (string.IsNullOrWhiteSpace(
                        certificatePath) ||
                    !File.Exists(
                        certificatePath))
                {
                    MessageBox.Show(
                        "The certificate could not be created.",
                        "VolunteerHub",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }


                MessageBoxResult openResult =
                    MessageBox.Show(
                        "Your VolunteerHub certificate was generated successfully!\n\n" +
                        "It was saved in:\n" +
                        certificatePath +
                        "\n\nWould you like to open it now?",
                        "Certificate created",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);


                if (openResult ==
                    MessageBoxResult.Yes)
                {
                    CertificateHelper
                        .OpenCertificate(
                            certificatePath);
                }
            }
            catch (InvalidOperationException exception)
            {
                MessageBox.Show(
                    exception.Message,
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    "The certificate could not be generated.\n\n" +
                    exception.Message,
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // PROFILE PHOTO FOLDER
        // =====================================================

        private void CreateProfilePhotosFolder()
        {
            try
            {
                if (!Directory.Exists(
                        profilePhotosFolder))
                {
                    Directory.CreateDirectory(
                        profilePhotosFolder);
                }
            }
            catch
            {
            }
        }


        // =====================================================
        // FIND SAVED PROFILE PHOTO
        // =====================================================

        private string GetSavedProfilePhotoPath()
        {
            if (!Directory.Exists(
                    profilePhotosFolder))
            {
                return null;
            }


            string prefix =
                "user_" +
                currentUser.Id +
                ".";


            string[] files =
                Directory.GetFiles(
                    profilePhotosFolder);


            foreach (string file
                in files)
            {
                string fileName =
                    Path.GetFileName(
                        file);


                if (fileName.StartsWith(
                    prefix,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return file;
                }
            }


            return null;
        }


        // =====================================================
        // LOAD PROFILE PHOTO
        // =====================================================

        private void LoadProfilePhoto()
        {
            string photoPath =
                GetSavedProfilePhotoPath();


            if (string.IsNullOrWhiteSpace(
                    photoPath) ||
                !File.Exists(
                    photoPath))
            {
                ShowDefaultProfileIcon();

                return;
            }


            try
            {
                BitmapImage bitmap =
                    new BitmapImage();


                bitmap.BeginInit();


                bitmap.CacheOption =
                    BitmapCacheOption.OnLoad;


                bitmap.UriSource =
                    new Uri(
                        photoPath,
                        UriKind.Absolute);


                bitmap.EndInit();


                bitmap.Freeze();


                ProfileImageBrush.ImageSource =
                    bitmap;


                ProfileIconText.Visibility =
                    Visibility.Collapsed;


                RemoveProfilePhotoButton.Visibility =
                    Visibility.Visible;
            }
            catch
            {
                ShowDefaultProfileIcon();
            }
        }


        // =====================================================
        // DEFAULT PROFILE ICON
        // =====================================================

        private void ShowDefaultProfileIcon()
        {
            ProfileImageBrush.ImageSource =
                null;


            ProfileIconText.Visibility =
                Visibility.Visible;


            RemoveProfilePhotoButton.Visibility =
                Visibility.Collapsed;
        }


        // =====================================================
        // CHOOSE PROFILE PHOTO
        // =====================================================

        private void ChooseProfilePhotoButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            OpenFileDialog dialog =
                new OpenFileDialog();


            dialog.Title =
                "Choose a profile photo";


            dialog.Filter =
                "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";


            dialog.Multiselect =
                false;


            bool? result =
                dialog.ShowDialog();


            if (result != true)
            {
                return;
            }


            try
            {
                CreateProfilePhotosFolder();


                DeleteExistingProfilePhoto();


                string extension =
                    Path.GetExtension(
                        dialog.FileName);


                string newFileName =
                    "user_" +
                    currentUser.Id +
                    extension.ToLower();


                string destinationPath =
                    Path.Combine(
                        profilePhotosFolder,
                        newFileName);


                File.Copy(
                    dialog.FileName,
                    destinationPath,
                    true);


                LoadProfilePhoto();


                MessageBox.Show(
                    "Your profile photo was updated successfully!",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show(
                    "The profile photo could not be saved.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // REMOVE PROFILE PHOTO
        // =====================================================

        private void RemoveProfilePhotoButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    "Do you want to remove your profile photo?",
                    "Remove profile photo",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (result !=
                MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                DeleteExistingProfilePhoto();


                ShowDefaultProfileIcon();


                MessageBox.Show(
                    "Your profile photo was removed.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show(
                    "The profile photo could not be removed.",
                    "VolunteerHub",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =====================================================
        // DELETE EXISTING PROFILE PHOTO
        // =====================================================

        private void DeleteExistingProfilePhoto()
        {
            if (!Directory.Exists(
                    profilePhotosFolder))
            {
                return;
            }


            string prefix =
                "user_" +
                currentUser.Id +
                ".";


            string[] files =
                Directory.GetFiles(
                    profilePhotosFolder);


            foreach (string file
                in files)
            {
                string fileName =
                    Path.GetFileName(
                        file);


                if (fileName.StartsWith(
                    prefix,
                    StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(
                        file);
                }
            }
        }


        // =====================================================
        // SAVE PROFILE
        // =====================================================

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


        // =====================================================
        // CHANGE PASSWORD
        // =====================================================

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


            CurrentPasswordInput.Password =
                "";


            NewPasswordInput.Password =
                "";


            ConfirmPasswordInput.Password =
                "";


            MessageBox.Show(
                "Your password was changed successfully!",
                "VolunteerHub",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }


        // =====================================================
        // DELETE ACCOUNT
        // =====================================================

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


            try
            {
                DeleteExistingProfilePhoto();
            }
            catch
            {
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


        // =====================================================
        // EVENTS
        // =====================================================

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


        // =====================================================
        // DASHBOARD
        // =====================================================

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