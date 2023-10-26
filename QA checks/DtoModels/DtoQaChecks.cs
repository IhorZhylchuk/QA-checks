using QA_checks.Controllers;
using System.ComponentModel.DataAnnotations;

namespace QA_checks.DtoModels
{
    public class DtoQaChecks
    {
        [Required]
        [Range(2000000, 2999999, ErrorMessage = "The length must be 7 digits, example - '2214569'")]
        public long OrdersNumber { get; set; }

        [Required]
        public int Pasteryzacja { get; set; }

        public string PasteryzacjaKomentarz { get; set; } = string.Empty;

        [Required]
        public int CiałaObce { get; set; }

        public string CiałaObceKomentarz { get; set; } = string.Empty;

        [Required]
        public int DataOpakowania { get; set; }

        public string DataOpakowaniaKomentarz { get; set; } = string.Empty;

        [Required]
        public int Receptura { get; set; }

        public string RecepturaKomentarz { get; set; } = string.Empty;

        [Required]
        public int MetalDetektor { get; set; }

        public string MetalDetektorKomentarz { get; set; } = string.Empty;

        [Required]
        public int Opakowanie { get; set; }

        public string OpakowanieKomentarz { get; set; } = string.Empty;

        [Required]
        public int TestWodny { get; set; }

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
    }
}
