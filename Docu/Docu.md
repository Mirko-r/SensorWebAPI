# Esercitazione finale

## 1. Introduzione

### 1.1 Scenario

Consideriamo un'azienda fittizia distribuita su territorio nazionale che produce componenti elettronici. La produzione è gestita da macchine dotate di sensori, che inviano informazioni a un broker di messaggi. Le informazioni vengono poi consumate da un'applicazione .NET che le memorizza in un database. I dati di produzione sono esposti attraverso un servizio web ASP.NET Core, con autenticazione JWT. Infine, una dashboard SPA React consente di visualizzare i dati in tempo reale e interagire con essi.

### 1.2 Tech Stack

- **.NET:** Utilizzato per lo sviluppo delle applicazioni console e del servizio web.
- **RabbitMQ:** Broker di messaggi per la gestione delle code.
- **Entity Framework Core:** ORM per l'accesso al database.
- **ASP.NET Core:** Framework per lo sviluppo del servizio web API.
- **React:** Libreria JavaScript per lo sviluppo della dashboard SPA.
- **JWT (JSON Web Token):** Utilizzato per l'autenticazione nelle API.
- **Swagger:** Utilizzato per documentare il servizio web ASP.NET Core.
- **Postman:** Utilizzato per testare gli endpoint del servizio web.

### 1.3 Ruolo di RabbitMq

Il broker di messaggi RabbitMQ è utilizzato per garantire una comunicazione asincrona tra le macchine produttive e il sistema di raccolta dati. Ogni macchina invia i dati di produzione ad una coda nel broker, e poi i messaggi verranno letti in modo asincrono.

### 1.4 MVC Desing Pattern

Il design pattern Model-View-Controller (MVC) è utilizzato nella progettazione e implementazione del servizio web ASP.NET Core. Questo pattern divide l'applicazione in tre componenti principali:

- **Modello:** rappresenta i dati e la logica di business.
- **Vista:** gestisce l'interfaccia utente.
- **Controller:** gestisce le interazioni tra la Vista e il Modello.

### 1.5 Autenticazione/Autorizzazione JWT

- **Autenticazione:** è il processo di verifica dell'identità dell'utente  per proteggere risorse sensibili e garantire che solo gli utenti autorizzati possano accedervi.
- **Autorizzazione:** Determina quali azioni può eseguire l'utente autenticato, il sistema deve determinare quali azioni o risorse l'utente è autorizzato a utilizzare. Ad esempio, un utente può avere il permesso di leggere dati ma non di modificarli, o può avere l'accesso solo a determinate parti di un'applicazione.

### 1.5.1 Jwt

Un JSON Web Token (JWT) è composto da tre parti: Header, Payload e Signature (Firma). Ognuna di queste parti è codificata in Base64URL e concatenate con un punto ("."). Il risultato finale è una stringa che rappresenta il token completo.

- **Header:** contiene informazioni sull'alogoritmo utilizzato per la firma del token
- **Payload:** contiene le informazioni utili del token. Le informazioni sono chiamate Claims e possono includere dati sull'utente, autorizzazioni, scadenza del token, roles, audience e issuer.
- **Signature:** è il risultato della firma digitale dell'Header e del Payload combinati con una chiave segreta.

## 2. Implementazione di JWT

### Configurazione

```csharp
[HttpGet]
public IEnumerable<object> GetAll()
{
    return _context.Productions.Include(p => p.Machine) 
    // Include la navigazione alla macchina associata
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
```

#### HttpGet All

```csharp
[HttpGet]
public IEnumerable<object> GetAll()
{
    return _context.Productions.Include(p => p.Machine) 
    // Include la navigazione alla macchina associata
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
```

Questo END-Point viene usato da React  per caricare tutte le produzioni presenti nel database.
Il metodo utilizza Entity Framework Core per accedere al database e recuperare le informazioni sulla produzione. Include anche la navigazione alla macchina associata per ottenere il dettaglio sulla sua posizione. I dati vengono quindi proiettati in un nuovo oggetto anonimo che include solo le informazioni rilevanti.

##### Esempio di funzionamente con PostMan

![img]("./Scrh/Postman_get_all.png")

#### HttpGet location

```csharp
[HttpGet("/filter/{location}")]
public IActionResult GetByMachineLocation(string location)
{
    Machine? machine = _context.Machines.Where(m => m.location.Contains(location))
        .Include(m => m.Productions).FirstOrDefault();
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
```

#### HttpGet MachineID

```csharp
[HttpGet("{machineId}")]
public ActionResult<IEnumerable<object>> GetByMachineId(string machineId)
{
    // Trova la macchina nel contesto del database in base all'ID fornito
    var machineData = _context.Machines
        .Where(m => m.MachineId == machineId)
        // Per ogni macchina trovata, seleziona le produzioni associate 
        //trasformate in un nuovo oggetto anonimo
        .Select(m => m.Productions.Select(p => new
        {
            p.ProductionId,
            p.MachineId,
            p.year,
            p.month,
            p.day,
            p.hourOfDay,
            p.make,
        })).ToList();

    if (machineData == null)
    {
        return NotFound();
    }

    return Ok(machineData);
}
```

##### Esempio di funzionamente con PostMan

![img]("./Scrh/Postman_get_by_MachineID.png")

#### HttpDelete MachineID

```csharp
[HttpDelete("{MachineId}")]
public IActionResult DeletebyId(string MachineId)
{
    Machine machine = _context.Machines.Find(MachineId);

    if (machine == null)
    {
        return NotFound();
    }

    List<Production> productionsToDelete = _context.Productions
        .Where(p => p.MachineId == MachineId).ToList();

    _context.Productions.RemoveRange(productionsToDelete);
    _context.SaveChanges();

    return Ok(productionsToDelete);
}
```

##### Esempio di funzionamente con PostMan

![img]("./Scrh/Postman_delete_from_MachineId.png")

#### HttpPost

```csharp
 [HttpPost] // post a production fromBody
 public async Task<IActionResult> AddProd([FromBody] ProductionBindingTarget prod)
 {
         if (prod != null)
         {
             var production = new Production
             {
                 MachineId = prod.MachineId,
                 year = prod.Year,
                 month = prod.Month,
                 day = prod.Day,
                 hourOfDay = prod.HourOfDay,
                 make = prod.Make
             };

             await _context.Productions.AddAsync(production);
             await _context.SaveChangesAsync();

             return Ok(production);
         }
         else
         {
             return BadRequest("Dati della produzione non validi.");
         }
 }
```

##### Esempio di funzionamente con PostMan

![img]("./Scrh/Postman_post_productions.png")