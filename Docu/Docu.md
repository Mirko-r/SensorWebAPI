# Esercitazione finale

## 1. Introduzione

### 1.1 Scenario

![scenario]("./Scrh/case_study.png")

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

## 2\. SQL

```SQL
-- Creating the Machines table
CREATE TABLE Machines (
    machineid varchar(50) PRIMARY KEY,
    location VARCHAR(255) NOT NULL
);

-- Creating the Productions table
CREATE TABLE Productions (
    productionid INT PRIMARY KEY,
    machineid varchar(50) FOREIGN KEY REFERENCES Machines(machineid),
    year INT,
    month INT,
    day INT,
    hourofday INT CHECK (hourofday >= 0 AND hourofday <= 24),
    make int
);

create table Users(
    UserId bigint identity,
    Username varchar(50),
    Password varchar(500),
    Role varchar(500),  
    HashToVerify varchar(100),
    SaltToVerify varchar(100)
)

insert into Machines (machineid, location) 
values
('LSD23431OP', 'Milan'),
('LLOPRER13P', 'Rome')
```

## 3\. Implementazione di JWT

### Configurazione in Program.cs

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // JWT auth
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // chi ha rilasciato il token
            ValidateAudience = true, // portatore del token
            ValidateLifetime = true, // scadenza del token
            ValidateIssuerSigningKey = true, // validare chiave che ha firmato il token
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // chi è l'issuer?
            ValidAudience = builder.Configuration["Jwt:Audience"], // chi è l'audience
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"])
            ) //chiave per firmare il token
        };
    });
```

### Processo di autenticazione nel Controller

#### 1\. Post del login

```csharp
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
```

#### 2\. AuthenticatUser

```csharp
 private User AuthenticateUser(UserModel login)
{
    try
    {
        User user = _context.Users.FirstOrDefault(
            u => u.Username.Equals(login.Username)
            );
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
```

Questo metodo controlla nel se l'utente esiste nel DB e verifica la password utilizzando una funzione di hash.

#### 3\. GenerateJSONWebTOken

```csharp
 private string GenerateJSONWebToken(User login)
{
    var securitykey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_config["Jwt:Key"])
        );

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
        signingCredentials: new SigningCredentials(
            securitykey, SecurityAlgorithms.HmacSha256)
        );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

Genera il JWT per l'utente autenticato.

## 4\. Consumatore

### Struttura del codice

Il codice è diviso in due principali sezioni: la configurazione e l'utilizzo di RabbitMQ e l'utilizzo di ADO\.NET per l'accesso e la manipolazione dei dati nel database SQL Server. La gestione del multithreading è implementata attraverso l'uso di Thread.Sleep per simulare l'attesa di un'ora prima dell'elaborazione dei dati.
RabbitMq creerà un'altro Thread  in ascolto di messaggi da una coda specificata.

## 5\. ADO\.Net / Entity Framework

- **ADO\.Net:** fornisce accesso diretto al DB, utilizzato nel Consumatore per avere un controllo più diretto sulla manipolazione dei dati e logica personalizzata nelle query
- **Entity Framework:** è un ORM (Object-Relational Mapping) che semplifica la gestione dei dati mediante l'astrazione dei dati del database in oggetti nel codice, utilizzato nella web API per semplificare l'accesso ai dati.

## 6\. Esempi di funzionamento web API con Postman

### Successful Login

![loginyes]("./Scrh/Login_Example.png")

### Chiamata ad un Web API Method senza autorizzazione

![noauth]("./Scrh/Unhautorized_Example.png")

### HttpGet All Productions

![getall]("./Scrh/Postman_get_all.png")

### HttpGet by MAchineID

![getByMcId]("./Scrh/Postman_get_by_MachineID.png")

### HttpPost a Production

![post]("./Scrh/Postman_post_productions.png")

### HttpDelete all productions with a MachineId

![deleteAllWithMachineId]("./Scrh/Postman_delete_from_MachineId.png")

## 7\. React

### 1\. Librerie utilizzate

- **Axios:**  per le chiamate HTTP al server, permette di fare richieste API.
-  **React Router:** per la navigazione tra le pagine dell'app
- **Bootstrap:** per l'utilizzo del layout e delle componenti grafiche
- **sweetalert2:**  per mostrare messaggi di conferma o errore alle azioni eseguite dall'utente

### 2\. Componenti principali

#### App.js

La principale componente della nostra applicazione è `App`, che contiene il routing e le diverse pagine dell'applicazione. 

#### Login.jsx

Componente  che gestisce il login dell'utente. Invia una richiesta POST all'API per autenticare l'utente. Se l'autenticazione è andata a buon fine fornisce all'utente un token per andare avanti.

#### Visual.jsx

Componetne  che gestisce il rendering della home page

La visual è composta da due sezioni principali:

- La sezione inferiore che mostra i prodotti disponibili per la macchina selezionata.

- Il caricamento dei dati iniziali attraverso una chiamata API.


## 3\. Codice 

####  Recupero dati`Visual.jsx`
```js
const fetchVisual = () => {
  // Effettua una richiesta GET all'endpoint specificato dell'API
  axios
    .get("https://192.168.4.182:7014/api/Productions")
    .then(function (response) {
      // Se la richiesta ha successo, imposta lo stato originalVisual con i dati ricevuti
      setOriginalVisual(response.data);

      // Chiama la funzione filterData per filtrare inizialmente i dati in base a locationFilter
      filterData(locationFilter);
    })
    .catch(function (error) {
      // Registra eventuali errori che si verificano durante la richiesta API
      console.log(error);
    });
};
```



####  Filtering data`Visual.jsx`
```js

const filterData = (filter) => {
  // Filtra i dati in base al filtro specificato (locationFilter)
  const filteredData = originalVisual.filter((item) =>
    // Confronta il valore di machineLocation (trasformato in minuscolo) con il filtro
    (item.machineLocation ? item.machineLocation.toLowerCase() : "").includes(
      filter.toLowerCase()
    )
  );
  // Imposta lo stato filteredVisual con i dati filtrati
  setFilteredVisual(filteredData);
};
```

#### Visualizzazione dati `Visual.jsx`
```html
 <table className="table table-hover item-center lead">
          <thead>
            <tr>
              <th className="col">Productions</th>
              <th className="col">Sensor</th>
              <th className="col">year</th>
              <th className="col">location</th>
              <th className="col">month</th>
              <th className="col">day</th>
              <th className="col">hourofday</th>
              <th className="col">summake</th>
            </tr>
          </thead>
          <tbody>
            {slicedData.map((currentVisual, key) => (
              <tr key={key}>
                <td>{currentVisual.productionId}</td>
                <td>{currentVisual.machineId}</td>
                <td>{currentVisual.year}</td>
                <td>{currentVisual.machineLocation}</td>
                <td>{currentVisual.month}</td>
                <td>{currentVisual.day}</td>
                <td>{currentVisual.hourOfDay}</td>
                <td>{currentVisual.make}</td>
              </tr>
            ))}
          </tbody>
        </table>
```


#### Login `Login.jsx`

```js


const handleSubmit = async (e) => {
  // Previeni il comportamento predefinito del form (evita il ricaricamento della pagina)
  e.preventDefault();

  try {
    // Effettua una richiesta POST al backend per il login
    const response = await axios.post(
      'https://192.168.4.182:7014/api/Productions/login',
      {
        Username: user,
        Password: password
      }
    );

    // Estrai il token dalla risposta del backend
    const { token } = response.data;

    // Salva il token nel localStorage per mantenerlo tra le sessioni
    localStorage.setItem('token', token);
    console.log("Token salvato:", token);

    // Reindirizza manualmente il componente a /visual
    navigate("/visual");
  } catch (error) {
    // Gestisci gli errori durante il login, ad esempio mostrando un messaggio di errore
    console.error('Errore durante il login:', error);
  }
};
```
