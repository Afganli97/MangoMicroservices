using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mango.Services.OrderAPI.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Mango.Services.OrderAPI.Extensions
{
    public static class AppBuilderExtensions
    {
        public static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostAppLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            hostAppLife.ApplicationStarted.Register(OnStart);
            hostAppLife.ApplicationStopped.Register(OnStop);
            return app; 
        }

        public static void OnStart()
        {
            ServiceBusConsumer.Start();
        }

        public static void OnStop()
        {
            ServiceBusConsumer.Stop();
        }

    }
}