using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HumanResources.AppLogic;
using HumanResources.Domain;
using Microsoft.EntityFrameworkCore;

namespace HumanResources.Infrastructure
{
    internal class EmployeeDbRepository : IEmployeeRepository
    {
        private readonly HumanResourcesContext _context;

        public EmployeeDbRepository(HumanResourcesContext context)
        {
            _context = context;
        }

        public async Task AddAsync(IEmployee newEmployee)
        {
            if (newEmployee == null)
            {
                throw new ArgumentNullException(nameof(newEmployee));
            }

            Employee employee = newEmployee as Employee;
            
            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();
        }

        public async Task CommitTrackedChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEmployee?> GetByNumberAsync(EmployeeNumber number)
        {
            if (string.IsNullOrEmpty(number))
            {
                throw new ArgumentNullException(nameof(number));
            }

            return await _context.Employees.SingleOrDefaultAsync(e => e.Number == number);
        }

        public async Task<int> GetNumberOfStartersOnAsync(DateTime startDate)
        {
            try
            {
                int numberOfStarters = await _context.Employees
                    .Where(e => e.StartDate >= startDate.Date)
                    .CountAsync();

                return numberOfStarters;
            }
            catch (Exception ex)
            {
                throw new Exception("ex");
            }
        }
    }
}
