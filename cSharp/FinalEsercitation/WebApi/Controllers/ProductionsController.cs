using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [EnableCors("ReactEnd")]
    public class ProductionsController : Controller
    {
        private DataContext _context;

        public ProductionsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<object> GetAll()
        {
            return _context.Productions
        .Include(p => p.Machine) // Include la navigazione alla macchina associata
        .Select(p => new
        {
            ProductionId = p.ProductionId,
            MachineId = p.MachineId,
            year = p.year,
            month = p.month,
            day = p.day,
            hourOfDay = p.hourOfDay,
            make = p.make,
            // Altre proprietà della produzione, se necessario
            MachineLocation = p.Machine.location
        })
        .ToList();
        }

        [HttpGet("/filter/{location}")]
        public IActionResult GetByMachineLocation(string location)
        {
            Machine? machine = _context.Machines.Where(m => m.location.Contains(location)).Include(m => m.Productions).FirstOrDefault();
            var result = new
            {
                Machine = new
                {
                    Location = machine.location,
                },
                Productions = machine.Productions
            };

            return Ok(result);
        }

        [HttpGet("{machineId}")]
        public ActionResult<IEnumerable<object>> GetByMachineId(string machineId)
        {
            // Trova la macchina nel contesto del database in base all'ID fornito
            var machineData = _context.Machines
                .Where(m => m.MachineId == machineId)
                // Per ogni macchina trovata, seleziona le produzioni associate trasformate in un nuovo oggetto anonimo
                .Select(m => m.Productions.Select(p => new
                {
                    p.ProductionId,
                    p.MachineId,
                    p.year,
                    p.month,
                    p.day,
                    p.hourOfDay,
                    p.make,
                })
                )
                // Trasforma il risultato in una lista
                .ToList();

            // Verifica se la macchina è stata trovata nel database
            if (machineData == null)
            {
                // Se la macchina non è stata trovata, restituisci un risultato 404
                return NotFound();
            }

            // Se la macchina è stata trovata, restituisci le produzioni associate come risultato OK
            return Ok(machineData);
        }




        [HttpDelete("{MachineId}")]
        public IActionResult DeletebyId(string MachineId)
        {
            // Trova la macchina
            Machine machine = _context.Machines.Find(MachineId);

            if (machine == null)
            {
                return NotFound(); // La macchina non è stata trovata
            }

            // Trova tutte le produzioni associate alla macchina
            List<Production> productionsToDelete = _context.Productions.Where(p => p.MachineId == MachineId).ToList();

            // Rimuovi le produzioni
            _context.Productions.RemoveRange(productionsToDelete);
            _context.SaveChanges();

            return Ok(productionsToDelete); // Restituisci le produzioni rimosse
        }


        [HttpPost] // post a production fromBody
        public IActionResult AddProd([FromBody] ProductionBindingTarget prod)
        {
                if (prod != null)
                {

                    // Crea un oggetto Production dalla ProductionBindingTarget
                    var production = new Production
                    {
                        MachineId = prod.MachineId,
                        year = prod.Year,
                        month = prod.Month,
                        day = prod.Day,
                        hourOfDay = prod.HourOfDay,
                        make = prod.Make
                    };

                    // Aggiungi la produzione al contesto e salva le modifiche
                    _context.Productions.Add(production);
                    _context.SaveChanges();

                    // Ritorna la produzione aggiunta con un codice di stato 201 (Created)
                    return Ok(production);
                }
                else
                {
                    return BadRequest("Dati della produzione non validi.");
                }
        }

    }
}
