namespace AuroBank_SoftwareProject.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string UserEmail { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
    }
}
