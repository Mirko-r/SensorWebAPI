using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Data.SqlClient;
using System.Text;
using Newtonsoft.Json;


namespace Consumer
{
    internal class Program
    {
        static List<Prod> Productions = new List<Prod>();

        static void Main(string[] args)
        {
            SqlConnectionStringBuilder SqcB = new SqlConnectionStringBuilder
            {
                DataSource = "localhost",
                InitialCatalog = "FinalEs",
                UserID = "sa",
                Password = "Uform@2023#",
                Encrypt = false
            };

            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
            };

            var connection = connectionFactory.CreateConnection();

            var channel = connection.CreateModel(); //Channel

            channel.QueueDeclare("make", true, false, false, null);

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = SqcB.ToString();
                conn.Open();


                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) => Consumer_Received(sender, e, conn); // Pass SqlConnection to event 

                channel.BasicConsume("make", true, consumer);

                Thread.Sleep(3600000); // 1hour

                SqlCommand cmd = new SqlCommand();
                // Raggruppa gli oggetti Prod per machineId
                var groupedProductions = Productions.GroupBy(p => p.machineid);
                cmd.Connection = conn;

                try
                {
                    foreach (var group in groupedProductions)
                    {
                        cmd.Parameters.Clear();
                        Console.WriteLine($"MachineId: {group.Key}, TotalMake: {group.Sum(p => p.make)}");

                        // Inserisci un solo record per gruppo
                        cmd.CommandText = "INSERT INTO [FinalEs].[dbo].[Productions] (MachineId, year, month, day, hourofday, make) VALUES (@Mid, @Year, @Month, @Day, @Hourofday, @Make)";
                        cmd.Parameters.AddWithValue("@Mid", group.Key);
                        cmd.Parameters.AddWithValue("@Year", DateTime.Now.Year);
                        cmd.Parameters.AddWithValue("@Month", DateTime.Now.Month);
                        cmd.Parameters.AddWithValue("@Day", DateTime.Now.Day);
                        cmd.Parameters.AddWithValue("@Hourofday", DateTime.Now.Hour);
                        cmd.Parameters.AddWithValue("@Make", group.Sum(p => p.make));
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.ExecuteNonQuery();
                        

                        Console.WriteLine(" ====== Record inserito con successo nel database.");

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e, SqlConnection conn)
        {
            try
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Prod prod = JsonConvert.DeserializeObject<Prod>(message);
                Productions.Add(prod);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class Prod
    {

        public string machineid { get; set; }
        public string location { get; set; }
        public int make { get; set; }

        // Costruttore senza parametri per la deserializzazione
        public Prod()
        {
        }

        // Costruttore con parametri per la creazione di nuovi oggetti Prod
        public Prod(string machineId, string location, int make)
        {
            this.machineid = machineId;
            this.location = location;
            this.make = make;
        }
    }
}
