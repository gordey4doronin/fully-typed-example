// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmployeesController.cs" company="EastBanc Technologies">
//   Copyright © EastBanc Technologies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FullyTypedExample.WebApi.Controllers
{
    using System.Linq;
    using System.Web.Http;

    using FullyTypedExample.Models;

    /// <summary>
    /// The employees API endpoint.
    /// </summary>
    public class EmployeesController : ApiController
    {
        /// <summary>
        /// Gets all employees.
        /// </summary>
        /// <remarks>
        /// Gets the list of all employees.
        /// </remarks>
        /// <returns>
        /// The list of employees.
        /// </returns>
        [Route("api/employees")]
        [HttpGet]
        public Employee[] GetEmployees()
        {
            return new[]
                {
                    new Employee { Id = 1, Name = "John Doe" },
                    new Employee { Id = 2, Name = "Jane Doe" }
                };
        }

        /// <summary>
        /// Gets employee by id.
        /// </summary>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <remarks>
        /// Gets the employee by specified id.
        /// </remarks>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [Route("api/employees/{employeeId:int}")]
        public Employee GetEmployeeById(int employeeId)
        {
            return this.GetEmployees().SingleOrDefault(x => x.Id == employeeId);
        }
    }
}
