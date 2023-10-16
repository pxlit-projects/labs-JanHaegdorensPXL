using Domain;
using HumanResources.Domain;
using IntegrationEvents.Employee;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("HumanResources.Api")]
[assembly: InternalsVisibleTo("HumanResources.Applogic.Tests")]

namespace HumanResources.AppLogic
{
    internal class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeFactory _employeeFactory;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPublishEndpoint _eventBus;

        public EmployeeService(IEmployeeFactory employeeFactory, IEmployeeRepository employeeRepository, IPublishEndpoint eventBus)
        {
            _employeeFactory = employeeFactory;
            _employeeRepository = employeeRepository;
            _eventBus = eventBus;
        }

        public async Task<IEmployee> HireNewAsync(string lastName, string firstName, DateTime startDate)
        {
            int sequence = await _employeeRepository.GetNumberOfStartersOnAsync(startDate) + 1;
            IEmployee newEmployee = _employeeFactory.CreateNew(lastName, firstName, startDate, sequence);
            await _employeeRepository.AddAsync(newEmployee);

            var @event = new EmployeeHiredIntegrationEvent
            {
                Number = newEmployee.Number,
                LastName = newEmployee.LastName,
                FirstName = newEmployee.FirstName
            };
            await _eventBus.Publish(@event);

            return newEmployee;
        }

        public async Task DismissAsync(EmployeeNumber employeeNumber, bool withNotice)
        {
            IEmployee? employeeToDismiss = await _employeeRepository.GetByNumberAsync(employeeNumber);
            Contracts.Require(employeeToDismiss != null, $"Cannot find an employee with number '{employeeNumber}'");
            employeeToDismiss.Dismiss(withNotice);
            await _employeeRepository.CommitTrackedChangesAsync();
        }
    }
}