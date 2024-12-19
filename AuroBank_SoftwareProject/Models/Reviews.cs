namespace AuroBank_SoftwareProject.Models
{
    public class Reviews
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime DateReviewed { get; set; }
        public int Rate { get; set; }
        public string UserEmail { get; set; }
    }
}
