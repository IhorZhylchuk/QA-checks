using QA_checks.Controllers;
using System.ComponentModel.DataAnnotations;

namespace QA_checks.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(2000000, 2999999, ErrorMessage = "The length must be 7 digits, example - '2214569'")]
        public long OrdersNumber { get; set; }

        [Required]
        [ForbiddenValue("string")]
        public string OrdersName { get; set; } = string.Empty;

        [Required]
        [MinValue(500)]
        [Range(500, 4000)]
        public int Count { get; set; }
    }
}
