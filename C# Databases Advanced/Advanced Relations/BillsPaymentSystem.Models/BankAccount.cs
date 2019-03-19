using System.ComponentModel.DataAnnotations;

namespace BillsPaymentSystem.Models
{
    public class BankAccount
    {
        [Key]
        public int BankAccountId { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Balance { get; set; }

        public string BankName { get; set; }

        public string SWIFT { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
    }
}
