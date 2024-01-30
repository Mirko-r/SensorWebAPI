using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FinalEsercitation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Verifica se sono stati forniti argomenti da console
            if (args.Length > 0)
            {
                Console.WriteLine("Argomenti forniti:");

                Console.WriteLine("MachineId" + args[0]);
                Console.WriteLine("location" + args[1]);

                string machineId = args[0];
                string location = args[1];

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
                var p = JsonSerializer.Serialize(new
                {
                    machineid = machineId,
                    location = location,
                    make = new Random().Next(1, 100)
                }) ;
                var body = Encoding.UTF8.GetBytes(p);


                while (true)
                {
                    channel.BasicPublish("", "make", null, body);
                    Console.WriteLine("Sent to make ->" + Encoding.UTF8.GetString(body));
                    Thread.Sleep(60000);
                }

            }
            else
            {
                Console.WriteLine("Nessun argomento fornito.");
            }
            Console.ReadLine();
        }
    }
}
