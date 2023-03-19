using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;
using Newtonsoft.Json;

namespace Mango.Services.OrderAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string subscriptionCheckOut;
        private readonly string checkOutMessageTopic;
        private readonly OrderRepository _orderRepository;
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor checkOutProcessor;

        public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("OrderBusConnectionString");
            subscriptionCheckOut = _configuration.GetValue<string>("SubscriptionCheckOut");
            checkOutMessageTopic = _configuration.GetValue<string>("CheckOutMessageTopic");
            var client = new ServiceBusClient(serviceBusConnectionString);
            checkOutProcessor = client.CreateProcessor(checkOutMessageTopic, subscriptionCheckOut);
        }

        public async Task Start()
        {
            checkOutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
            checkOutProcessor.ProcessErrorAsync += ErrorHandler;
            await checkOutProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await checkOutProcessor.StopProcessingAsync();
            await checkOutProcessor.DisposeAsync(); 
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CheckOutHeaderDto checkOutHeaderDto = JsonConvert.DeserializeObject<CheckOutHeaderDto>(body);

            OrderHeader orderHeader = new()
            {
                UserId = checkOutHeaderDto.UserId,
                FirstName = checkOutHeaderDto.FirstName,
                LastName = checkOutHeaderDto.LastName,
                OrderDetails = new(),
                CardNumber = checkOutHeaderDto.CardNumber,
                CouponCode = checkOutHeaderDto.CouponCode,
                CVV = checkOutHeaderDto.CVV,
                DiscountTotal = checkOutHeaderDto.DiscountTotal,
                Email = checkOutHeaderDto.Email,
                ExpiryMonthYear = checkOutHeaderDto.ExpiryMonthYear,
                OrderTime = DateTime.Now,
                OrderTotal = checkOutHeaderDto.OrderTotal,
                PaymentStatus = false,
                PhoneNumber = checkOutHeaderDto.PhoneNumber,
                PickUpDateTime = checkOutHeaderDto.PickUpDateTime
            };
            foreach (var detail in checkOutHeaderDto.CartDetails)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = detail.ProductId,
                    ProductName = detail.ProductDto.Name,
                    Price = detail.ProductDto.Price,
                    Count = detail.Count
                };
                orderHeader.CartTotalItems += detail.Count;
                orderHeader.OrderDetails.Add(orderDetail);
            }
            await _orderRepository.AddOrder(orderHeader);
        }
    }
}