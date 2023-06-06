using Newtonsoft.Json;
using Rabbit.Producer.Model;
using RabbitMQ.Client;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

const string QUEUE_NAME = "MailSenderReceiver";

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: QUEUE_NAME,
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);

    while (true)
    {
        Console.WriteLine("Type a email message: ");
        Console.WriteLine("For expit program type empty value.");
        string messageFromInputConsole = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(messageFromInputConsole))
        {
            break;
        }

        var emailMessage = new EmailMessage()
        {
            To = "smtp.office365.com",
            From = "smtp.office365.com",
            Subject = "Test Email",
            Body = messageFromInputConsole,
            Type = EmailType.Smtp
        };

        channel.BasicPublish(exchange: "",
              routingKey: QUEUE_NAME,
              basicProperties: null, emailMessage.ToBytes());

        Console.WriteLine("");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\tSent {0}", messageFromInputConsole);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("");
    }
}


public static class Utility
{
    public static byte[] ToBytes(this EmailMessage emailMessage)
    {
        var content = JsonConvert.SerializeObject(emailMessage);
        return Encoding.UTF8.GetBytes(content);
    }
}