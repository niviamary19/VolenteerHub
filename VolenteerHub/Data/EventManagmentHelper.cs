using System;
using System.Data.SQLite;
using System.IO;
using VolenteerHub.Models;

namespace VolenteerHub.Data
{
    public static class EventManagementHelper
    {
        private static readonly string DatabasePath =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "VolunteerHub.db");

        private static readonly string ConnectionString =
            "Data Source=" + DatabasePath + ";Version=3;";

        public static bool UpdateEvent(
            VolunteerEvent volunteerEvent,
            int organizerId)
        {
            try
            {
                using (SQLiteConnection connection =
                    new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    string query = @"
                        UPDATE Events
                        SET
                            Title = @Title,
                            Description = @Description,
                            EventDate = @EventDate,
                            StartTime = @StartTime,
                            EndTime = @EndTime,
                            Category = @Category,
                            Location = @Location,
                            Latitude = @Latitude,
                            Longitude = @Longitude,
                            MaxVolunteers = @MaxVolunteers
                        WHERE Id = @EventId
                        AND OrganizerId = @OrganizerId;";

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
                            "@EventId",
                            volunteerEvent.Id);

                        command.Parameters.AddWithValue(
                            "@OrganizerId",
                            organizerId);

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

        public static bool DeleteEvent(
            int eventId,
            int organizerId)
        {
            SQLiteTransaction transaction = null;

            try
            {
                using (SQLiteConnection connection =
                    new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    // First make sure the event belongs
                    // to the logged-in organizer.
                    string ownerQuery = @"
                        SELECT COUNT(*)
                        FROM Events
                        WHERE Id = @EventId
                        AND OrganizerId = @OrganizerId;";

                    using (SQLiteCommand ownerCommand =
                        new SQLiteCommand(
                            ownerQuery,
                            connection))
                    {
                        ownerCommand.Parameters.AddWithValue(
                            "@EventId",
                            eventId);

                        ownerCommand.Parameters.AddWithValue(
                            "@OrganizerId",
                            organizerId);

                        long count =
                            (long)ownerCommand.ExecuteScalar();

                        if (count == 0)
                        {
                            return false;
                        }
                    }

                    transaction =
                        connection.BeginTransaction();

                    // Remove registrations first.
                    string deleteRegistrationsQuery = @"
                        DELETE FROM Registrations
                        WHERE EventId = @EventId;";

                    using (SQLiteCommand command =
                        new SQLiteCommand(
                            deleteRegistrationsQuery,
                            connection,
                            transaction))
                    {
                        command.Parameters.AddWithValue(
                            "@EventId",
                            eventId);

                        command.ExecuteNonQuery();
                    }

                    // Remove the event.
                    string deleteEventQuery = @"
                        DELETE FROM Events
                        WHERE Id = @EventId
                        AND OrganizerId = @OrganizerId;";

                    using (SQLiteCommand command =
                        new SQLiteCommand(
                            deleteEventQuery,
                            connection,
                            transaction))
                    {
                        command.Parameters.AddWithValue(
                            "@EventId",
                            eventId);

                        command.Parameters.AddWithValue(
                            "@OrganizerId",
                            organizerId);

                        int rows =
                            command.ExecuteNonQuery();

                        if (rows == 0)
                        {
                            transaction.Rollback();

                            return false;
                        }
                    }

                    transaction.Commit();

                    return true;
                }
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
    }
}