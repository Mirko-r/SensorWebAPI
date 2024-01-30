using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Models;
using WebApplicationTraining.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [EnableCors("ReactEnd")]
    public class ProductionsController : Controller
    {
        private DataContext _context;
        private IConfiguration _config;

        public ProductionsController(IConfiguration config, DataContext context)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        [AllowAnonymous] // non c'è bisogno di presentare un token
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] UserModel login)
        {
            Console.WriteLine("ss");
            IActionResult response = Unauthorized(); // 401
            var user = AuthenticateUser(login);
            if (user != null)
            {
                Console.WriteLine("user not null");
                string tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }
            return response;
        }

        private User AuthenticateUser(UserModel login)
        {
            try
            {
                User user = _context.Users.FirstOrDefault(u => u.Username.Equals(login.Username));

                if (user.VerifyPassword(login.Password, user.HashToVerify, user.SaltToVerify))
                {
                    return user;
                }
                else
                {
                    throw new Exception("Password not verified");
                }
            }
            catch (Exception ex)
            {
                // Aggiungi la gestione dell'eccezione, ad esempio il logging
                Console.WriteLine($"Errore durante l'autenticazione: {ex.Message}");
                return null;
            }
        }

        private string GenerateJSONWebToken(User login)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, login.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // uid univoco del claim 
                new Claim("role", login.Role.ToString())
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<object> GetAll()
        {
            return _context.Productions.Include(p => p.Machine) // Include la navigazione alla macchina associata
            .Select(p => new
            {
                ProductionId = p.ProductionId,
                MachineId = p.MachineId,
                year = p.year,
                month = p.month,
                day = p.day,
                hourOfDay = p.hourOfDay,
                make = p.make,
                MachineLocation = p.Machine.location
            }).ToList();
        }

        [HttpGet("/filter/{location}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public IActionResult DeletebyId(string MachineId)
        {
            // Trova la macchina
            Machine machine = _context.Machines.Find(MachineId);

            if (machine == null)
            {
                return NotFound(); // La macchina non è stata trovata
            }
            var currentUser = HttpContext.User;

            foreach (var c in currentUser.Claims) // check for user claims
            {
                if (c.Type.Contains("role"))
                {
                    if (c.Value.Contains("ADMIN") || c.Value.Contains("DEV"))
                    {
                        // Trova tutte le produzioni associate alla macchina
                        List<Production> productionsToDelete = _context.Productions.Where(p => p.MachineId == MachineId).ToList();

                        // Rimuovi le produzioni
                        _context.Productions.RemoveRange(productionsToDelete);
                        _context.SaveChanges();

                        return Ok(productionsToDelete); // Restituisci le produzioni rimosse
                    }
                }
            }
            return Unauthorized(new string[] { "You are now allowed to invoke this method" });
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<IActionResult> AddProd([FromBody] ProductionBindingTarget prod)
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

                var currentUser = HttpContext.User;

                foreach (var c in currentUser.Claims) // check for user claims
                {
                    if (c.Type.Contains("role"))
                    {
                        if (c.Value.Contains("ADMIN") || c.Value.Contains("DEV"))
                        {
                            // Aggiungi la produzione al contesto e salva le modifiche
                            await _context.Productions.AddAsync(production);
                            await _context.SaveChangesAsync();

                            // Ritorna la produzione aggiunta con un codice di stato 201 (Created)
                            return Ok(production);
                        }
                    }
                }
                return Unauthorized(new string[] { "You are now allowed to invoke this method" });
            }
            else
            {
                return BadRequest("Dati della produzione non validi.");
            }
        }

    }
}
