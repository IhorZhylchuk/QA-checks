using QA_checks.Controllers;
using System.ComponentModel.DataAnnotations;

namespace QA_checks.DtoModels
{
    public class DtoUpdateOrderModel
    {
 
        [Required]
        [ForbiddenValue("string")]
        public string OrdersName { get; set; } = string.Empty;

        [Required]
        [MinValue(500)]
        [Range(500, 4000)]
        public int Count { get; set; }
    }
}
