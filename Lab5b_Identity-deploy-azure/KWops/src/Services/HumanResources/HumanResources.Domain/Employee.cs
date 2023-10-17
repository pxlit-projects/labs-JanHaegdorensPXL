using Domain;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HumanResources.Infrastructure")]
[assembly: InternalsVisibleTo("HumanResources.Api")]
[assembly: InternalsVisibleTo("HumanResources.Domain.Tests")]

namespace HumanResources.Domain
{
    internal class Employee : Entity, IEmployee
    {
        public EmployeeNumber Number { get; private set; }
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        protected override IEnumerable<object> GetIdComponents()
        {
            throw new NotImplementedException();
        }

        public void Dismiss(bool withNotice = true)
        {
            if (EndDate != null && withNotice == true)
            {
                
                throw new ContractException("error");
            }
            if(EndDate != null && withNotice == false)
            {
                EndDate = DateTime.Now;
            }


            if (withNotice)
            {

                DateTime today = DateTime.Now;

                if ((today - StartDate).TotalDays < 90) // Less than 3 months
                {
                    EndDate = today.AddDays(7); // 1 week notice
                }
                else if ((today - StartDate).TotalDays < 365) // Less than a year
                {
                    EndDate = today.AddDays(14); // 2 weeks notice
                }
                else
                {
                    EndDate = today.AddDays(28); // 4 weeks notice
                }
            }
            else
            {
                EndDate = DateTime.Now;
            }

        }

        private Employee()
        {
            Number = new EmployeeNumber(DateTime.MinValue, 1);
            LastName = string.Empty;
            FirstName = string.Empty;
        }

        internal class Factory : IEmployeeFactory
        {
            public IEmployee CreateNew(string lastName, string firstName, DateTime startDate, int sequence)
            {
                Contracts.Require(startDate >= DateTime.Now.AddYears(-1), "The start date of an employee cannot be more than 1 year in the past");
                Contracts.Require(!string.IsNullOrEmpty(lastName), "The last name of an employee cannot be empty");
                Contracts.Require(lastName.Length >= 2, "The last name of an employee must at least have 2 characters");
                Contracts.Require(!string.IsNullOrEmpty(firstName), "The first name of an employee cannot be empty");
                Contracts.Require(firstName.Length >= 2, "The first name of an employee must at least have 2 characters");

                var employee = new Employee
                {
                    Number = new EmployeeNumber(startDate, sequence),
                    FirstName = firstName,
                    LastName = lastName,
                    StartDate = startDate,
                    EndDate = null
                };
                return employee;
            }
        }
    }
}