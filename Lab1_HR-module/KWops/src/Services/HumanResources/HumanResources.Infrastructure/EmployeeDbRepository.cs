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

        public async Task AddAsync(Employee newEmployee)
        {
            if (newEmployee == null)
            {
                throw new ArgumentNullException(nameof(newEmployee));
            }
            
            _context.Employees.Add(newEmployee);

            await _context.SaveChangesAsync();
        }

        public async Task<Employee?> GetByNumberAsync(string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                throw new ArgumentNullException(nameof(number));
            }

            return await _context.Employees.SingleOrDefaultAsync(e => e.Number == number);
        }
    }
}
