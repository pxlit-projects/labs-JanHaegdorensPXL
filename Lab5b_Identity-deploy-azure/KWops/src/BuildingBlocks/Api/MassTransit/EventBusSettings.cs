namespace Api.MassTransit;

public class EventBusSettings
{
    public bool UseAzureServiceBus { get; set; }
    public string QueueName { get; set; } = string.Empty;
    public RabbitMqSettings RabbitMQ { get; set; } = new RabbitMqSettings();
    public AzureSettings Azure { get; set; } = new AzureSettings();

}