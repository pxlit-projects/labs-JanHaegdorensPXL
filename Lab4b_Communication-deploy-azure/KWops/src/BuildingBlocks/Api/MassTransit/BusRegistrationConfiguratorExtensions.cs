using MassTransit;

namespace Api.MassTransit
{
    public static class BusRegistrationConfiguratorExtensions
    {
        public static void UseEventBus(this IBusRegistrationConfigurator x, EventBusSettings settings)
        {
            if (settings.UseAzureServiceBus)
            {
                x.UsingAzureServiceBus((context, config) =>
                {
                    config.Host(settings.Azure.Connection);
                    ConfigureEventBus(config, context, settings.QueueName);
                });
            }
            else
            {
                x.UsingRabbitMq((context, config) =>
                {
                    config.Host(settings.RabbitMQ.Host, "/", host =>
                    {
                        host.Username(settings.RabbitMQ.UserName);
                        host.Password(settings.RabbitMQ.Password);
                    });

                    ConfigureEventBus(config, context, settings.QueueName);
                });
            }
        }

        private static void ConfigureEventBus(IBusFactoryConfigurator config,
            IBusRegistrationContext context, string queueName)
        {
            //One receive endpoint (queue) for all consumers
            config.ReceiveEndpoint(queueName, e =>
            {
                e.ConfigureConsumers(context);
            });
        }
    }
}