using System;

namespace VolenteerHub.Models
{
    public class VolunteerEvent
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string EventDate { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string Category { get; set; }

        public string Location { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public int MaxVolunteers { get; set; }

        public int OrganizerId { get; set; }

        public string EventStatus
        {
            get
            {
                DateTime date;

                if (DateTime.TryParse(
                    EventDate,
                    out date))
                {
                    if (date.Date < DateTime.Today)
                    {
                        return "Past";
                    }

                    if (date.Date == DateTime.Today)
                    {
                        return "Today";
                    }
                }

                return "Upcoming";
            }
        }

        public string TimeDisplay
        {
            get
            {
                return StartTime + " - " + EndTime;
            }
        }
    }
}