using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using VolenteerHub.Models;

namespace VolenteerHub.Data
{
    public static class AdminHelper
    {
        private static readonly string DatabasePath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "VolunteerHub.db");

        private static readonly string ConnectionString =
            "Data Source=" +
            DatabasePath +
            ";Version=3;";


        // =====================================================
        // TOTAL USERS
        // =====================================================

        public static int GetTotalUserCount()
        {
            return GetCount(
                "SELECT COUNT(*) FROM Users;");
        }


        // =====================================================
        // VOLUNTEERS
        // =====================================================

        public static int GetVolunteerCount()
        {
            return GetCount(
                "SELECT COUNT(*) FROM Users WHERE LOWER(Role) = 'volunteer';");
        }


        // =====================================================
        // ORGANIZERS
        // =====================================================

        public static int GetOrganizerCount()
        {
            return GetCount(
                "SELECT COUNT(*) FROM Users WHERE LOWER(Role) = 'organizer';");
        }


        // =====================================================
        // ADMINS
        // =====================================================

        public static int GetAdminCount()
        {
            return GetCount(
                "SELECT COUNT(*) FROM Users WHERE LOWER(Role) = 'admin';");
        }


        // =====================================================
        // PENDING VERIFICATIONS
        // =====================================================

        public static int GetPendingVerificationCount()
        {
            VerificationHelper
                .InitializeVerificationTable();

            return GetCount(
                @"SELECT COUNT(*)
                  FROM UserVerification
                  WHERE VerificationStatus = 'Pending';");
        }


        // =====================================================
        // VERIFIED USERS
        // =====================================================

        public static int GetVerifiedUserCount()
        {
            VerificationHelper
                .InitializeVerificationTable();

            return GetCount(
                @"SELECT COUNT(*)
                  FROM UserVerification
                  WHERE VerificationStatus = 'Verified';");
        }


        // =====================================================
        // GET ALL USERS
        // =====================================================

        public static List<User> GetAllUsers()
        {
            List<User> users =
                new List<User>();


            try
            {
                using (SQLiteConnection connection =
                    new SQLiteConnection(
                        ConnectionString))
                {
                    connection.Open();


                    string query = @"
                        SELECT
                            Id,
                            FullName,
                            Email,
                            Role,
                            CreatedAt
                        FROM Users
                        ORDER BY FullName ASC;";


                    using (SQLiteCommand command =
                        new SQLiteCommand(
                            query,
                            connection))
                    {
                        using (SQLiteDataReader reader =
                            command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                User user =
                                    new User
                                    {
                                        Id =
                                            Convert.ToInt32(
                                                reader["Id"]),

                                        FullName =
                                            reader["FullName"]
                                                .ToString(),

                                        Email =
                                            reader["Email"]
                                                .ToString(),

                                        Role =
                                            reader["Role"]
                                                .ToString(),

                                        CreatedAt =
                                            reader["CreatedAt"]
                                                .ToString()
                                    };


                                users.Add(
                                    user);
                            }
                        }
                    }
                }
            }
            catch
            {
            }


            return users;
        }


        // =====================================================
        // DELETE USER
        // =====================================================

        public static bool DeleteUser(
            User user)
        {
            if (user == null)
            {
                return false;
            }


            try
            {
                // Reuse the profile deletion logic that already
                // works elsewhere in VolunteerHub.
                return ProfileHelper
                    .DeleteAccount(
                        user);
            }
            catch
            {
                return false;
            }
        }


        // =====================================================
        // GENERIC COUNT
        // =====================================================

        private static int GetCount(
            string query)
        {
            try
            {
                using (SQLiteConnection connection =
                    new SQLiteConnection(
                        ConnectionString))
                {
                    connection.Open();


                    using (SQLiteCommand command =
                        new SQLiteCommand(
                            query,
                            connection))
                    {
                        object result =
                            command.ExecuteScalar();


                        if (result == null ||
                            result == DBNull.Value)
                        {
                            return 0;
                        }


                        return Convert.ToInt32(
                            result);
                    }
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}