using System;

namespace VolenteerHub.Models
{
    public class MyVolunteerEvent
    {
        public int RegistrationId { get; set; }

        public int EventId { get; set; }

        public string Title { get; set; }

        public string EventDate { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string Location { get; set; }

        public string Category { get; set; }

        public string RegistrationStatus { get; set; }

        public double HoursWorked { get; set; }

        public string EventStatus
        {
            get
            {
                DateTime eventDate;

                if (DateTime.TryParse(
                    EventDate,
                    out eventDate))
                {
                    if (eventDate.Date < DateTime.Today)
                    {
                        return "Past";
                    }

                    if (eventDate.Date == DateTime.Today)
                    {
                        return "Today";
                    }
                }

                return "Upcoming";
            }
        }

        public string HoursDisplay
        {
            get
            {
                return HoursWorked.ToString("0.##") + " h";
            }
        }
    }
}