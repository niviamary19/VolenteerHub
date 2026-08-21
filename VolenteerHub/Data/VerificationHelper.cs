using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using VolenteerHub.Models;

namespace VolenteerHub.Data
{
    public static class VerificationHelper
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
        // INITIALIZE TABLE
        // =====================================================

        public static void InitializeVerificationTable()
        {
            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    CREATE TABLE IF NOT EXISTS UserVerification
                    (
                        UserId INTEGER PRIMARY KEY,
                        VerificationStatus TEXT NOT NULL DEFAULT 'Not verified',
                        VerifiedAt TEXT NULL,

                        FOREIGN KEY (UserId)
                            REFERENCES Users(Id)
                    );";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }


        // =====================================================
        // STATUS
        // =====================================================

        public static string GetVerificationStatus(
            int userId)
        {
            InitializeVerificationTable();

            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT VerificationStatus
                    FROM UserVerification
                    WHERE UserId = @UserId
                    LIMIT 1;";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    object result =
                        command.ExecuteScalar();

                    if (result == null ||
                        result == DBNull.Value)
                    {
                        return "Not verified";
                    }

                    return result.ToString();
                }
            }
        }


        // =====================================================
        // VERIFIED DATE
        // =====================================================

        public static string GetVerifiedAt(
            int userId)
        {
            InitializeVerificationTable();

            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT VerifiedAt
                    FROM UserVerification
                    WHERE UserId = @UserId
                    LIMIT 1;";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    object result =
                        command.ExecuteScalar();

                    if (result == null ||
                        result == DBNull.Value)
                    {
                        return "";
                    }

                    return result.ToString();
                }
            }
        }


        // =====================================================
        // SET PENDING
        // =====================================================

        public static void SetPending(
            int userId)
        {
            InitializeVerificationTable();

            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    INSERT OR REPLACE INTO UserVerification
                    (
                        UserId,
                        VerificationStatus,
                        VerifiedAt
                    )
                    VALUES
                    (
                        @UserId,
                        'Pending',
                        NULL
                    );";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    command.ExecuteNonQuery();
                }
            }
        }


        // =====================================================
        // APPROVE
        // =====================================================

        public static bool SetVerified(
            int userId)
        {
            InitializeVerificationTable();

            try
            {
                using (SQLiteConnection connection =
                    new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT OR REPLACE INTO UserVerification
                        (
                            UserId,
                            VerificationStatus,
                            VerifiedAt
                        )
                        VALUES
                        (
                            @UserId,
                            'Verified',
                            @VerifiedAt
                        );";

                    using (SQLiteCommand command =
                        new SQLiteCommand(
                            query,
                            connection))
                    {
                        command.Parameters.AddWithValue(
                            "@UserId",
                            userId);

                        command.Parameters.AddWithValue(
                            "@VerifiedAt",
                            DateTime.Now.ToString(
                                "yyyy-MM-dd HH:mm:ss"));

                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        // =====================================================
        // REJECT
        // =====================================================

        public static bool SetRejected(
            int userId)
        {
            InitializeVerificationTable();

            try
            {
                using (SQLiteConnection connection =
                    new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT OR REPLACE INTO UserVerification
                        (
                            UserId,
                            VerificationStatus,
                            VerifiedAt
                        )
                        VALUES
                        (
                            @UserId,
                            'Rejected',
                            NULL
                        );";

                    using (SQLiteCommand command =
                        new SQLiteCommand(
                            query,
                            connection))
                    {
                        command.Parameters.AddWithValue(
                            "@UserId",
                            userId);

                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        // =====================================================
        // GET PENDING REQUESTS
        // =====================================================

        public static List<VerificationRequest>
            GetPendingVerificationRequests()
        {
            InitializeVerificationTable();

            List<VerificationRequest> requests =
                new List<VerificationRequest>();


            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT
                        U.Id,
                        U.FullName,
                        U.Email,
                        U.Role,
                        V.VerificationStatus
                    FROM UserVerification V

                    INNER JOIN Users U
                        ON V.UserId = U.Id

                    WHERE V.VerificationStatus = 'Pending'

                    ORDER BY U.FullName ASC;";


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
                            requests.Add(
                                new VerificationRequest
                                {
                                    UserId =
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

                                    VerificationStatus =
                                        reader["VerificationStatus"]
                                            .ToString()
                                });
                        }
                    }
                }
            }

            return requests;
        }
    }
}