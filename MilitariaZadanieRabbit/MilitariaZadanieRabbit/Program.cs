using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Mail;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Rabbit.Consumer.Model;
using System.Runtime.Serialization.Formatters.Binary;

const string QUEUE_NAME = "MailSenderReceiver";

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: QUEUE_NAME, durable: false,
        exclusive: false, autoDelete: false, arguments: null);

    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, ea) =>
    {
        var emailMessage = Utility<EmailMessage>.GetEmailMessageFromBytes(ea.Body.ToArray());
        if (emailMessage != null)
        {

            switch (emailMessage.Type)
            {
                case EmailType.Smtp:
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    using (var smtpClient = new SmtpClient("smtp.office365.com"))
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Port = 587;
                        smtpClient.Credentials = new NetworkCredential("smtp.office365.com", "smtp.office365.com");
                        smtpClient.EnableSsl = true;

                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress(emailMessage.From),
                            Subject = emailMessage.Subject,
                            Body = emailMessage.Body
                        };
                        mailMessage.To.Add(emailMessage.To);

                        try
                        {
                            smtpClient.Send(mailMessage);
                            Console.WriteLine("Email sent successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Failed to send email. Error: " + ex.Message);
                        }
                    }
                    break;
            }
        }
    };

    channel.BasicConsume(queue: QUEUE_NAME,
        autoAck: true, consumer: consumer);


    // operacja utrzymujaca program przy życiu 
    char key = 'z';

    while (key != 'q' && key != 'Q')
    {
        Console.WriteLine(" Press [q] or [Q] to exit.");
        key = Console.ReadKey().KeyChar;
    }
}


public static class Utility<T> where T : class
{
    public static T GetEmailMessageFromBytes(byte[] bytes)
    {
        try
        {
            var content = Encoding.UTF8.GetString(bytes);
            return JsonConvert.DeserializeObject<T>(content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Getting email message from bytes with error: {ex.Message}");
            return null;
        }
    }
}