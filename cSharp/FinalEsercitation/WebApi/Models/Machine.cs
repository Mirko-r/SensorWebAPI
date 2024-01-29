using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class Machine
    {
        public string MachineId { get; set; }
        public string location { get; set; }

        public List<Production> Productions { get; set; }
    }
}
