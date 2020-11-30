using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using System.Text;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace MessageSender
{
  class Program
  {
    private static string _connectionString;
    private static string _queuename;
    private static IQueueClient _client;
    public static IConfigurationRoot Configuration { get; set; }
    
    static void Main(string[] args)
    {
      var builder = ConfigureBuilder();
      _connectionString = builder["connectionstring"];
      _queuename = builder["queuename"];

      MainAsync().GetAwaiter().GetResult();
    }

    private static IConfigurationRoot ConfigureBuilder()
    {
      return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();
    }
    private static async Task MainAsync()
    {
      const int numberOfMessagesToSend = 10;
      _client = new QueueClient(_connectionString, _queuename);
      Console.WriteLine("Starting...");
      await SendMessagesAsync(numberOfMessagesToSend);
      Console.WriteLine("Ending...");
      Console.WriteLine("Press any key...");
      Console.ReadKey();
      await _client.CloseAsync();
    }

    private static async Task SendMessagesAsync(int numberOfMessagesToSend)
    {
      try
      {
        for (var index = 0; index < numberOfMessagesToSend; index++)
        {
          var customMessage = $"#{index}:A message from MessageSender.";
          var message = new Message(Encoding.UTF8.GetBytes(customMessage));
          Console.WriteLine($"Sending message: {customMessage}");
          await _client.SendAsync(message);
        }
      }
      catch (Exception exception)
      {
        Console.WriteLine($"Weird! It's exception with message: {exception.Message}");
      }
    }
  }
}
