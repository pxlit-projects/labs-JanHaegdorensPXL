using HumanResources.Domain;
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
        private IEmployeeFactory _employeeFactory;
        private IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeFactory employeeFactory, IEmployeeRepository employeeRepository) { 
            _employeeFactory = employeeFactory;
            _employeeRepository = employeeRepository;
        }

        public Task DismissAsync(EmployeeNumber employeeNumber, bool withNotice)
        {
            IEmployee employee = _employeeRepository.GetByNumberAsync(employeeNumber).Result;
            if (employee == null) { throw new ArgumentNullException(nameof(employee)); }
            employee.Dismiss();
            _employeeRepository.CommitTrackedChangesAsync();
            return Task.CompletedTask;
        }

        public async Task<IEmployee> HireNewAsync(string lastName, string firstName, DateTime startDate)
        {
            int sequence = _employeeRepository.GetNumberOfStartersOnAsync(startDate).Result + 1;
            IEmployee employee = _employeeFactory.CreateNew(lastName, firstName, startDate, sequence);
            await _employeeRepository.AddAsync(employee);
            return employee;
        }
    }
}
