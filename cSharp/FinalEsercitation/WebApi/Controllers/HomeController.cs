using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class HomeController : Controller
    {
        private DataContext _context;

        public HomeController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProductionInfo(MachineBindingTarget model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index",model);
            }
            else
            {

                Machine m = new Machine
                {
                    MachineId = model.MachineId,
                    location = model.Location
                };

                Production p = new Production
                {
                    MachineId = model.MachineId,
                    year = DateTime.Now.Year,
                    month = DateTime.Now.Month,
                    day = DateTime.Now.Day,
                    hourOfDay = DateTime.Now.Hour,
                    make = model.Make
                };

                _context.Machines.Add(m);
                _context.Productions.Add(p);
                _context.SaveChanges();

                // Redirect a una pagina successiva o visualizza un messaggio di successo
                return RedirectToAction("Success");
            }
        }

        public IActionResult Success()
        {
            return View("Index");
        }
    }
}
