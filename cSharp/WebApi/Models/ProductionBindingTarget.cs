using Microsoft.AspNetCore.Mvc;

namespace WebApi.Models
{
    public class ProductionBindingTarget
    {
        public string MachineId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int HourOfDay { get; set; }  // Assicurati che il nome del campo corrisponda
        public int Make { get; set; }
    }
}
