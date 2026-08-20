using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using VolenteerHub.Models;

namespace VolenteerHub.Data
{
    public static class ProfileHelper
    {
        private static readonly string DatabasePath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "VolunteerHub.db");

        private static readonly string ConnectionString =
            "Data Source=" + DatabasePath + ";Version=3;";

        public static bool UpdateProfile(
            User user,
            string fullName,
            string email)
        {
            try
            {
                using (SQLiteConnection connection =
                    new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string checkEmailQuery = @"
                        SELECT COUNT(*)
                        FROM Users
                        WHERE LOWER(Email) = LOWER(@Email)
                        AND Id != @UserId;";

                    using (SQLiteCommand checkCommand =
                        new SQLiteCommand(
                            checkEmailQuery,
                            connection))
                    {
                        checkCommand.Parameters.AddWithValue(
                            "@Email",
                            email.Trim());

                        checkCommand.Parameters.AddWithValue(
                            "@UserId",
                            user.Id);

                        long count =
                            (long)checkCommand.ExecuteScalar();

                        if (count > 0)
                        {
                            return false;
                        }
                    }

                    string updateQuery = @"
                        UPDATE Users
                        SET FullName = @FullName,
                            Email = @Email
                        WHERE Id = @UserId;";

                    using (SQLiteCommand updateCommand =
                        new SQLiteCommand(
                            updateQuery,
                            connection))
                    {
                        updateCommand.Parameters.AddWithValue(
                            "@FullName",
                            fullName.Trim());

                        updateCommand.Parameters.AddWithValue(
                            "@Email",
                            email.Trim().ToLower());

                        updateCommand.Parameters.AddWithValue(
                            "@UserId",
                            user.Id);

                        int rows =
                            updateCommand.ExecuteNonQuery();

                        if (rows == 0)
                        {
                            return false;
                        }
                    }
                }

                user.FullName =
                    fullName.Trim();

                user.Email =
                    email.Trim().ToLower();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ChangePassword(
            User user,
            string currentPassword,
            string newPassword)
        {
            User loginCheck =
                DatabaseHelper.LoginUser(
                    user.Email,
                    currentPassword);

            if (loginCheck == null)
            {
                return false;
            }

            try
            {
                string newSalt =
                    GenerateSalt();

                string newHash =
                    HashPassword(
                        newPassword,
                        newSalt);

                using (SQLiteConnection connection =
                    new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"
                        UPDATE Users
                        SET PasswordHash = @PasswordHash,
                            PasswordSalt = @PasswordSalt
                        WHERE Id = @UserId;";

                    using (SQLiteCommand command =
                        new SQLiteCommand(
                            query,
                            connection))
                    {
                        command.Parameters.AddWithValue(
                            "@PasswordHash",
                            newHash);

                        command.Parameters.AddWithValue(
                            "@PasswordSalt",
                            newSalt);

                        command.Parameters.AddWithValue(
                            "@UserId",
                            user.Id);

                        int rows =
                            command.ExecuteNonQuery();

                        return rows > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool DeleteAccount(
            User user)
        {
            SQLiteTransaction transaction = null;

            try
            {
                using (SQLiteConnection connection =
                    new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    transaction =
                        connection.BeginTransaction();

                    // Remove registrations for events
                    // owned by this organizer.
                    string deleteEventRegistrations = @"
                        DELETE FROM Registrations
                        WHERE EventId IN
                        (
                            SELECT Id
                            FROM Events
                            WHERE OrganizerId = @UserId
                        );";

                    using (SQLiteCommand command =
                        new SQLiteCommand(
                            deleteEventRegistrations,
                            connection,
                            transaction))
                    {
                        command.Parameters.AddWithValue(
                            "@UserId",
                            user.Id);

                        command.ExecuteNonQuery();
                    }

                    // Remove events created by this user.
                    string deleteEvents = @"
                        DELETE FROM Events
                        WHERE OrganizerId = @UserId;";

                    using (SQLiteCommand command =
                        new SQLiteCommand(
                            deleteEvents,
                            connection,
                            transaction))
                    {
                        command.Parameters.AddWithValue(
                            "@UserId",
                            user.Id);

                        command.ExecuteNonQuery();
                    }

                    // Remove registrations made by this user.
                    string deleteRegistrations = @"
                        DELETE FROM Registrations
                        WHERE UserId = @UserId;";

                    using (SQLiteCommand command =
                        new SQLiteCommand(
                            deleteRegistrations,
                            connection,
                            transaction))
                    {
                        command.Parameters.AddWithValue(
                            "@UserId",
                            user.Id);

                        command.ExecuteNonQuery();
                    }

                    // Finally remove the user.
                    string deleteUser = @"
                        DELETE FROM Users
                        WHERE Id = @UserId;";

                    using (SQLiteCommand command =
                        new SQLiteCommand(
                            deleteUser,
                            connection,
                            transaction))
                    {
                        command.Parameters.AddWithValue(
                            "@UserId",
                            user.Id);

                        int rows =
                            command.ExecuteNonQuery();

                        if (rows == 0)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    transaction.Commit();
                }

                return true;
            }
            catch
            {
                if (transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch
                    {
                    }
                }

                return false;
            }
        }

        private static string GenerateSalt()
        {
            byte[] saltBytes =
                new byte[16];

            using (RNGCryptoServiceProvider rng =
                new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }

            return Convert.ToBase64String(
                saltBytes);
        }

        private static string HashPassword(
            string password,
            string salt)
        {
            byte[] saltBytes =
                Convert.FromBase64String(
                    salt);

            using (Rfc2898DeriveBytes pbkdf2 =
                new Rfc2898DeriveBytes(
                    password,
                    saltBytes,
                    10000))
            {
                byte[] hash =
                    pbkdf2.GetBytes(32);

                return Convert.ToBase64String(
                    hash);
            }
        }
    }
}