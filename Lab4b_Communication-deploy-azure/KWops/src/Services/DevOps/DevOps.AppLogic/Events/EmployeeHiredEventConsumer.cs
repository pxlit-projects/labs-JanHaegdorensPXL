using DevOps.Domain;
using IntegrationEvents.Employee;
using MassTransit;
using MassTransit.Middleware;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7")]

namespace DevOps.AppLogic.Events;
internal class EmployeeHiredEventConsumer : IConsumer<EmployeeHiredIntegrationEvent>
{
    private readonly IDeveloperRepository _developerRepository;
    private readonly ILogger<EmployeeHiredEventConsumer> _logger;

    public EmployeeHiredEventConsumer(IDeveloperRepository developerRepository, ILogger<EmployeeHiredEventConsumer> logger)
    {
        _developerRepository = developerRepository;
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<EmployeeHiredIntegrationEvent> context)
    {
        EmployeeHiredIntegrationEvent @event = context.Message;
        _logger.LogDebug($"DevOps - Handling employee hire. Id: {@event.EventId}");
        Developer? developer = await _developerRepository.GetByIdAsync(@event.Number);
        if (developer is not null)
        {
            _logger.LogDebug($"DevOps - No developer added. A developer with id '{@event.Number}' already exists. Id: {@event.EventId}");
            return;
        }
        developer = Developer.CreateNew(@event.Number, @event.FirstName, @event.LastName, 0.0);
        await _developerRepository.AddAsync(developer);
        _logger.LogDebug($"DevOps - Developer with id '{@event.Number}' added. Id: {@event.EventId}");
    }
}