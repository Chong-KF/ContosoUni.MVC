using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUni.Services
{
    public interface IAzureQueue
    {
        public void QueueMessage(string message);
    }

    public class AzureQueue : IAzureQueue
    {
        //private readonly string connStr = "DefaultEndpointsProtocol=https;AccountName=myxzystorage;AccountKey=KhitklC4F86OrPXbEagl9W4+t8B++WyUbagSFFixmKExI/59vdY1bA8DduO8wYi93ceH/gyYAXhEnZfiTIjy5g==;EndpointSuffix=core.windows.net";
        //private readonly string queueName = "contoso";

        private readonly IConfiguration _configuration;

        public AzureQueue(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //-------------------------------------------------
        // Create a message queue
        //-------------------------------------------------
        public void QueueMessage(string message)
        {
            try
            {
                // Instantiate a QueueClient which will be used to create and manipulate the queue
                //QueueClient queueClient = new(connStr, queueName);

                QueueClient queueClient = new(
                     _configuration.GetValue<string>("AzureStorage:StorageConnection"),
                     _configuration.GetValue<string>("AzureStorage:QueueName"));

                // Create the queue
                queueClient.CreateIfNotExistsAsync().Wait();

                if (queueClient.Exists())
                {
                    // Send a message to the queue
                    var base64 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message));
                    queueClient.SendMessage(base64);
                    Console.WriteLine($"Inserted: {message}");
                }
                //else
                //{
                //    Console.WriteLine($"Make sure the Azurite storage emulator running and try again.");
                //    return false;
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}\n\n");
                Console.WriteLine($"Make sure the Azurite storage emulator running and try again.");
            }
        }
    }
}
