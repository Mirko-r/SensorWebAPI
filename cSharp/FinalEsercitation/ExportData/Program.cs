using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        // Richiesta di input per il nome del file
        Console.Write("Location: ");
        string location = Console.ReadLine();

        Console.Write("Machineid: ");
        string machineid = Console.ReadLine();

        Console.Write("Hour: ");
        string hour = Console.ReadLine();

        Console.Write("Day: ");
        string day = Console.ReadLine();

        Console.Write("Month: ");
        string month = Console.ReadLine();

        Console.Write("Year: ");
        string year = Console.ReadLine();

        Console.Write("Correct [Y/N]: ");
        string correctInput = Console.ReadLine().ToUpper();

        // Verifica se le informazioni inserite sono corrette
        if (correctInput != "Y")
        {
            Console.WriteLine("Operazione annullata.");
            return;
        }

        SqlConnectionStringBuilder SqcB = new SqlConnectionStringBuilder
        {
            DataSource = "localhost",
            InitialCatalog = "FinalEs",
            UserID = "sa",
            Password = "Uform@2023#",
            Encrypt = false
        };

        string connectionString = SqcB.ToString();

        string sqlQuery = $"SELECT P.machineid, M.location, SUM(P.make) as totalmake " +
                          $"FROM Productions P " +
                          $"JOIN Machines M ON P.machineid = M.machineid " +
                          $"WHERE P.machineid = '{machineid}' " +
                          $"AND P.year = {year} " +
                          $"AND P.month = {month} " +
                          $"AND P.day = {day} " +
                          $"AND P.hourofday = {hour} " +
                          $"GROUP BY P.machineid, M.location";


        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Esecuzione della query
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Costruzione del nome del file
                    string filename = $"C:\\Users\\04mir\\Download\\{location}_{machineid}_{hour}_{day}_{month}_{year}.csv";

                    // Creazione e scrittura del file CSV
                    using (StreamWriter sw = new StreamWriter(filename, false, Encoding.UTF8))
                    {
                        // Intestazione del file CSV
                        sw.WriteLine("machineid,location,summake");

                        // Scrittura dei dati nel file CSV
                        while (reader.Read())
                        {
                            sw.WriteLine($"{reader["machineid"]},{reader["location"]},{reader["totalmake"]}");
                        }

                        Console.WriteLine($"File CSV creato con successo: {filename}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore durante l'accesso al database o la creazione del file CSV: {ex.Message}");
        }
    }
}
