namespace VolenteerHub.Models
{
    public class OrganizerParticipant
    {
        public int RegistrationId { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }

        public double HoursWorked { get; set; }

        public string HoursDisplay
        {
            get
            {
                return HoursWorked.ToString("0.##") + " h";
            }
        }
    }
}