using System.Text;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Newtonsoft.Json;

namespace Mango.AzureBus
{
    public class AzureServiceBusMessageBus : IMessageBus
    {
        public async Task PublishMessage(BaseMessage message, string topicName)
        {
            ISenderClient senderClient = new TopicClient(SD.ServiceBusConnect, topicName);
            var jsonMessage = JsonConvert.SerializeObject(message);
            var finalMessage = new Message(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await senderClient.SendAsync(finalMessage);
            await senderClient.CloseAsync();

        }
    }
}