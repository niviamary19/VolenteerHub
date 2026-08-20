using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using VolenteerHub.Models;

namespace VolenteerHub.Data
{
    public static class DatabaseHelper
    {
        private static readonly string DatabasePath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "VolunteerHub.db");

        private static readonly string ConnectionString =
            "Data Source=" + DatabasePath + ";Version=3;";

        public static void InitializeDatabase()
        {
            if (!File.Exists(DatabasePath))
            {
                SQLiteConnection.CreateFile(DatabasePath);
            }

            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS Users
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FullName TEXT NOT NULL,
                        Email TEXT NOT NULL UNIQUE,
                        PasswordHash TEXT NOT NULL,
                        PasswordSalt TEXT NOT NULL,
                        Role TEXT NOT NULL,
                        CreatedAt TEXT NOT NULL
                    );";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        createUsersTable,
                        connection))
                {
                    command.ExecuteNonQuery();
                }

                string createEventsTable = @"
                    CREATE TABLE IF NOT EXISTS Events
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Description TEXT NOT NULL,
                        EventDate TEXT NOT NULL,
                        StartTime TEXT NOT NULL,
                        EndTime TEXT NOT NULL,
                        Category TEXT NOT NULL,
                        Location TEXT NOT NULL,
                        Latitude REAL NOT NULL,
                        Longitude REAL NOT NULL,
                        MaxVolunteers INTEGER NOT NULL,
                        OrganizerId INTEGER NOT NULL,

                        FOREIGN KEY (OrganizerId)
                            REFERENCES Users(Id)
                    );";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        createEventsTable,
                        connection))
                {
                    command.ExecuteNonQuery();
                }

                string createRegistrationsTable = @"
                    CREATE TABLE IF NOT EXISTS Registrations
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER NOT NULL,
                        EventId INTEGER NOT NULL,
                        Status TEXT NOT NULL,
                        HoursWorked REAL NOT NULL DEFAULT 0,
                        RegisteredAt TEXT NOT NULL,

                        FOREIGN KEY (UserId)
                            REFERENCES Users(Id),

                        FOREIGN KEY (EventId)
                            REFERENCES Events(Id),

                        UNIQUE(UserId, EventId)
                    );";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        createRegistrationsTable,
                        connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool EmailExists(
            string email)
        {
            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT COUNT(*)
                    FROM Users
                    WHERE LOWER(Email) = LOWER(@Email);";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@Email",
                        email.Trim());

                    long count =
                        (long)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        public static bool RegisterUser(
            string fullName,
            string email,
            string password,
            string role)
        {
            if (EmailExists(email))
            {
                return false;
            }

            string salt =
                GenerateSalt();

            string passwordHash =
                HashPassword(
                    password,
                    salt);

            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    INSERT INTO Users
                    (
                        FullName,
                        Email,
                        PasswordHash,
                        PasswordSalt,
                        Role,
                        CreatedAt
                    )
                    VALUES
                    (
                        @FullName,
                        @Email,
                        @PasswordHash,
                        @PasswordSalt,
                        @Role,
                        @CreatedAt
                    );";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@FullName",
                        fullName.Trim());

                    command.Parameters.AddWithValue(
                        "@Email",
                        email.Trim().ToLower());

                    command.Parameters.AddWithValue(
                        "@PasswordHash",
                        passwordHash);

                    command.Parameters.AddWithValue(
                        "@PasswordSalt",
                        salt);

                    command.Parameters.AddWithValue(
                        "@Role",
                        role);

                    command.Parameters.AddWithValue(
                        "@CreatedAt",
                        DateTime.Now.ToString(
                            "yyyy-MM-dd HH:mm:ss"));

                    command.ExecuteNonQuery();
                }
            }

            return true;
        }

        public static User LoginUser(
            string email,
            string password)
        {
            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT
                        Id,
                        FullName,
                        Email,
                        PasswordHash,
                        PasswordSalt,
                        Role,
                        CreatedAt
                    FROM Users
                    WHERE LOWER(Email) = LOWER(@Email)
                    LIMIT 1;";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@Email",
                        email.Trim());

                    using (SQLiteDataReader reader =
                        command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        string storedHash =
                            reader["PasswordHash"].ToString();

                        string storedSalt =
                            reader["PasswordSalt"].ToString();

                        string enteredPasswordHash =
                            HashPassword(
                                password,
                                storedSalt);

                        if (storedHash != enteredPasswordHash)
                        {
                            return null;
                        }

                        return new User
                        {
                            Id =
                                Convert.ToInt32(
                                    reader["Id"]),

                            FullName =
                                reader["FullName"].ToString(),

                            Email =
                                reader["Email"].ToString(),

                            Role =
                                reader["Role"].ToString(),

                            CreatedAt =
                                reader["CreatedAt"].ToString()
                        };
                    }
                }
            }
        }

        public static bool CreateEvent(
            VolunteerEvent volunteerEvent)
        {
            try
            {
                using (SQLiteConnection connection =
                    new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO Events
                        (
                            Title,
                            Description,
                            EventDate,
                            StartTime,
                            EndTime,
                            Category,
                            Location,
                            Latitude,
                            Longitude,
                            MaxVolunteers,
                            OrganizerId
                        )
                        VALUES
                        (
                            @Title,
                            @Description,
                            @EventDate,
                            @StartTime,
                            @EndTime,
                            @Category,
                            @Location,
                            @Latitude,
                            @Longitude,
                            @MaxVolunteers,
                            @OrganizerId
                        );";

                    using (SQLiteCommand command =
                        new SQLiteCommand(
                            query,
                            connection))
                    {
                        command.Parameters.AddWithValue(
                            "@Title",
                            volunteerEvent.Title);

                        command.Parameters.AddWithValue(
                            "@Description",
                            volunteerEvent.Description);

                        command.Parameters.AddWithValue(
                            "@EventDate",
                            volunteerEvent.EventDate);

                        command.Parameters.AddWithValue(
                            "@StartTime",
                            volunteerEvent.StartTime);

                        command.Parameters.AddWithValue(
                            "@EndTime",
                            volunteerEvent.EndTime);

                        command.Parameters.AddWithValue(
                            "@Category",
                            volunteerEvent.Category);

                        command.Parameters.AddWithValue(
                            "@Location",
                            volunteerEvent.Location);

                        command.Parameters.AddWithValue(
                            "@Latitude",
                            volunteerEvent.Latitude);

                        command.Parameters.AddWithValue(
                            "@Longitude",
                            volunteerEvent.Longitude);

                        command.Parameters.AddWithValue(
                            "@MaxVolunteers",
                            volunteerEvent.MaxVolunteers);

                        command.Parameters.AddWithValue(
                            "@OrganizerId",
                            volunteerEvent.OrganizerId);

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

        public static List<VolunteerEvent> GetAllEvents()
        {
            List<VolunteerEvent> events =
                new List<VolunteerEvent>();

            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT
                        Id,
                        Title,
                        Description,
                        EventDate,
                        StartTime,
                        EndTime,
                        Category,
                        Location,
                        Latitude,
                        Longitude,
                        MaxVolunteers,
                        OrganizerId
                    FROM Events
                    ORDER BY EventDate ASC,
                             StartTime ASC;";

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
                            events.Add(
                                ReadVolunteerEvent(reader));
                        }
                    }
                }
            }

            return events;
        }

        public static List<VolunteerEvent> GetOrganizerEvents(
            int organizerId)
        {
            List<VolunteerEvent> events =
                new List<VolunteerEvent>();

            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT
                        Id,
                        Title,
                        Description,
                        EventDate,
                        StartTime,
                        EndTime,
                        Category,
                        Location,
                        Latitude,
                        Longitude,
                        MaxVolunteers,
                        OrganizerId
                    FROM Events
                    WHERE OrganizerId = @OrganizerId
                    ORDER BY EventDate ASC,
                             StartTime ASC;";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@OrganizerId",
                        organizerId);

                    using (SQLiteDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            events.Add(
                                ReadVolunteerEvent(reader));
                        }
                    }
                }
            }

            return events;
        }

        private static VolunteerEvent ReadVolunteerEvent(
            SQLiteDataReader reader)
        {
            return new VolunteerEvent
            {
                Id =
                    Convert.ToInt32(
                        reader["Id"]),

                Title =
                    reader["Title"].ToString(),

                Description =
                    reader["Description"].ToString(),

                EventDate =
                    reader["EventDate"].ToString(),

                StartTime =
                    reader["StartTime"].ToString(),

                EndTime =
                    reader["EndTime"].ToString(),

                Category =
                    reader["Category"].ToString(),

                Location =
                    reader["Location"].ToString(),

                Latitude =
                    Convert.ToDouble(
                        reader["Latitude"]),

                Longitude =
                    Convert.ToDouble(
                        reader["Longitude"]),

                MaxVolunteers =
                    Convert.ToInt32(
                        reader["MaxVolunteers"]),

                OrganizerId =
                    Convert.ToInt32(
                        reader["OrganizerId"])
            };
        }

        public static bool IsUserRegistered(
            int userId,
            int eventId)
        {
            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT COUNT(*)
                    FROM Registrations
                    WHERE UserId = @UserId
                    AND EventId = @EventId;";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    command.Parameters.AddWithValue(
                        "@EventId",
                        eventId);

                    long count =
                        (long)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        public static int GetRegistrationCount(
            int eventId)
        {
            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT COUNT(*)
                    FROM Registrations
                    WHERE EventId = @EventId;";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@EventId",
                        eventId);

                    return Convert.ToInt32(
                        command.ExecuteScalar());
                }
            }
        }

        public static bool RegisterForEvent(
            int userId,
            int eventId)
        {
            try
            {
                using (SQLiteConnection connection =
                    new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO Registrations
                        (
                            UserId,
                            EventId,
                            Status,
                            HoursWorked,
                            RegisteredAt
                        )
                        VALUES
                        (
                            @UserId,
                            @EventId,
                            @Status,
                            @HoursWorked,
                            @RegisteredAt
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
                            "@EventId",
                            eventId);

                        command.Parameters.AddWithValue(
                            "@Status",
                            "Registered");

                        command.Parameters.AddWithValue(
                            "@HoursWorked",
                            0);

                        command.Parameters.AddWithValue(
                            "@RegisteredAt",
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

        public static bool CancelRegistration(
            int userId,
            int eventId)
        {
            try
            {
                using (SQLiteConnection connection =
                    new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"
                        DELETE FROM Registrations
                        WHERE UserId = @UserId
                        AND EventId = @EventId;";

                    using (SQLiteCommand command =
                        new SQLiteCommand(
                            query,
                            connection))
                    {
                        command.Parameters.AddWithValue(
                            "@UserId",
                            userId);

                        command.Parameters.AddWithValue(
                            "@EventId",
                            eventId);

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

        public static List<MyVolunteerEvent> GetUserVolunteerEvents(
            int userId)
        {
            List<MyVolunteerEvent> events =
                new List<MyVolunteerEvent>();

            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT
                        R.Id AS RegistrationId,
                        R.EventId,
                        R.Status,
                        R.HoursWorked,
                        E.Title,
                        E.EventDate,
                        E.StartTime,
                        E.EndTime,
                        E.Location,
                        E.Category
                    FROM Registrations R
                    INNER JOIN Events E
                        ON R.EventId = E.Id
                    WHERE R.UserId = @UserId
                    ORDER BY E.EventDate ASC,
                             E.StartTime ASC;";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    using (SQLiteDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            events.Add(
                                new MyVolunteerEvent
                                {
                                    RegistrationId =
                                        Convert.ToInt32(
                                            reader["RegistrationId"]),

                                    EventId =
                                        Convert.ToInt32(
                                            reader["EventId"]),

                                    Title =
                                        reader["Title"].ToString(),

                                    EventDate =
                                        reader["EventDate"].ToString(),

                                    StartTime =
                                        reader["StartTime"].ToString(),

                                    EndTime =
                                        reader["EndTime"].ToString(),

                                    Location =
                                        reader["Location"].ToString(),

                                    Category =
                                        reader["Category"].ToString(),

                                    RegistrationStatus =
                                        reader["Status"].ToString(),

                                    HoursWorked =
                                        Convert.ToDouble(
                                            reader["HoursWorked"])
                                });
                        }
                    }
                }
            }

            return events;
        }

        public static List<OrganizerParticipant>
            GetEventParticipants(
                int eventId)
        {
            List<OrganizerParticipant> participants =
                new List<OrganizerParticipant>();

            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT
                        R.Id AS RegistrationId,
                        R.UserId,
                        R.Status,
                        R.HoursWorked,
                        U.FullName,
                        U.Email
                    FROM Registrations R
                    INNER JOIN Users U
                        ON R.UserId = U.Id
                    WHERE R.EventId = @EventId
                    ORDER BY U.FullName ASC;";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@EventId",
                        eventId);

                    using (SQLiteDataReader reader =
                        command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            participants.Add(
                                new OrganizerParticipant
                                {
                                    RegistrationId =
                                        Convert.ToInt32(
                                            reader["RegistrationId"]),

                                    UserId =
                                        Convert.ToInt32(
                                            reader["UserId"]),

                                    FullName =
                                        reader["FullName"].ToString(),

                                    Email =
                                        reader["Email"].ToString(),

                                    Status =
                                        reader["Status"].ToString(),

                                    HoursWorked =
                                        Convert.ToDouble(
                                            reader["HoursWorked"])
                                });
                        }
                    }
                }
            }

            return participants;
        }

        public static bool UpdateVolunteerHours(
            int registrationId,
            double hoursWorked)
        {
            try
            {
                using (SQLiteConnection connection =
                    new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"
                        UPDATE Registrations
                        SET
                            HoursWorked = @HoursWorked,
                            Status = @Status
                        WHERE Id = @RegistrationId;";

                    using (SQLiteCommand command =
                        new SQLiteCommand(
                            query,
                            connection))
                    {
                        command.Parameters.AddWithValue(
                            "@HoursWorked",
                            hoursWorked);

                        command.Parameters.AddWithValue(
                            "@Status",
                            hoursWorked > 0
                                ? "Completed"
                                : "Registered");

                        command.Parameters.AddWithValue(
                            "@RegistrationId",
                            registrationId);

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

        public static double GetTotalVolunteerHours(
            int userId)
        {
            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT COALESCE(
                        SUM(HoursWorked),
                        0
                    )
                    FROM Registrations
                    WHERE UserId = @UserId;";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    return Convert.ToDouble(
                        command.ExecuteScalar());
                }
            }
        }

        public static int GetJoinedEventCount(
            int userId)
        {
            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT COUNT(*)
                    FROM Registrations
                    WHERE UserId = @UserId;";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    return Convert.ToInt32(
                        command.ExecuteScalar());
                }
            }
        }

        public static int GetUpcomingEventCount(
            int userId)
        {
            using (SQLiteConnection connection =
                new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string today =
                    DateTime.Today.ToString(
                        "yyyy-MM-dd");

                string query = @"
                    SELECT COUNT(*)
                    FROM Registrations R
                    INNER JOIN Events E
                        ON R.EventId = E.Id
                    WHERE R.UserId = @UserId
                    AND E.EventDate >= @Today;";

                using (SQLiteCommand command =
                    new SQLiteCommand(
                        query,
                        connection))
                {
                    command.Parameters.AddWithValue(
                        "@UserId",
                        userId);

                    command.Parameters.AddWithValue(
                        "@Today",
                        today);

                    return Convert.ToInt32(
                        command.ExecuteScalar());
                }
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
                Convert.FromBase64String(salt);

            using (Rfc2898DeriveBytes pbkdf2 =
                new Rfc2898DeriveBytes(
                    password,
                    saltBytes,
                    10000))
            {
                byte[] hash =
                    pbkdf2.GetBytes(32);

                return Convert.ToBase64String(hash);
            }
        }
    }
}