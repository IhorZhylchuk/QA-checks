using QA_checks.Controllers;
using System.ComponentModel.DataAnnotations;

namespace QA_checks.Models
{
    public class QAchecks
    {
         public int Id { get; set; }
         public int OrderId { get; set; }

        [Required]
        [Range(2000000, 2999999, ErrorMessage = "The length must be 7 digits, example - '2214569'")]
        public long OrdersNumber { get; set; }

        [Required]
        public int Pasteryzacja { get; set; }

        [Required]
        [ForbiddenValue("string")]
        public string PasteryzacjaKomentarz { get; set; } = string.Empty;

        [Required]
        public int CiałaObce { get; set; }

        [Required]
        [ForbiddenValue("string")]
        public string CiałaObceKomentarz { get; set; } = string.Empty;

        [Required]
        public int DataOpakowania { get; set; }

        [Required]
        [ForbiddenValue("string")]
        public string DataOpakowaniaKomentarz { get; set; } = string.Empty;

        [Required]
        public int Receptura { get; set; }

        [Required]
        [ForbiddenValue("string")]
        public string RecepturaKomentarz { get; set; } = string.Empty;

        [Required]
        public int MetalDetektor { get; set; }

        [Required]
        [ForbiddenValue("string")]
        public string MetalDetektorKomentarz { get; set; } = string.Empty;

        [Required]
        public int Opakowanie { get; set; }

        [Required]
        [ForbiddenValue("string")]
        public string OpakowanieKomentarz { get; set; } = string.Empty;

        [Required]
        public int TestWodny { get; set; }

        [Required]
        [ForbiddenValue("string")]
        public string TestKomentarz { get; set; } = string.Empty;

        [Required]
        [MinValue(0)]
        public float Lepkość { get; set; }

        [Required]
        [MinValue(0)]
        public float Ekstrakt { get; set; }

        [Required]
        [MinValue(0)]
        public float Ph { get; set; }

        [Required]
        [MinValue(0)]
        public float Temperatura { get; set; }

        [Required]
        public string Date { get; set; } = string.Empty;

    }
}
