using Microsoft.EntityFrameworkCore;

namespace WebApi.Models
{
    public class Production
    {
        public int ProductionId { get; set; }
        public string MachineId { get; set; }
        public Machine Machine { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public int hourOfDay { get; set; }
        public int make { get; set; }
    }
}
