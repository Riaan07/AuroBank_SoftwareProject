namespace AuroBank_SoftwareProject.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime NotificationDate { get; set; }
        public bool IsRead { get; set; }
        public bool IsAdvice { get; set; }
        public string UserEmail { get; set; }
    }
}
