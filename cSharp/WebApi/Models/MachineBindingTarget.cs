using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class MachineBindingTarget
    {
        [Required(ErrorMessage = "Il campo Machine ID è obbligatorio.")]
        [RegularExpression("[A-Za-z0-9]{10}", ErrorMessage = "Il campo Machine ID deve contenere 10 caratteri alfanumerici.")]
        public string MachineId { get; set; }

        [Required(ErrorMessage = "Il campo Location è obbligatorio.")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Il campo Make è obbligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Il campo Make deve essere un numero maggiore di zero.")]
        public int Make { get; set; }
    }
}
