namespace VolenteerHub.Models
{
    public class VerificationRequest
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public string VerificationStatus { get; set; }
    }
}