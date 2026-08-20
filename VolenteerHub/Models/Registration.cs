namespace VolenteerHub.Models
{
    public class Registration
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int EventId { get; set; }

        public string Status { get; set; }

        public double HoursWorked { get; set; }

        public string RegisteredAt { get; set; }
    }
}