using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HumanResources.Api.Models;
using Test;

namespace HumanResources.Api.Tests.Builders
{
    internal class EmployeeCreateModelBuilder : BuilderBase<EmployeeCreateModel>
    {
        public EmployeeCreateModelBuilder()
        {
            Item = new EmployeeCreateModel
            {
                FirstName = Random.NextString(),
                LastName = Random.NextString(),
                StartDate = DateTime.Now
            };
        }
    }
}
