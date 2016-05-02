// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DepartmentsController.cs" company="EastBanc Technologies">
//   Copyright © EastBanc Technologies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FullyTypedExample.WebApi.Controllers
{
    using System.Data;
    using System.Web.Http;
    using System.Web.Http.Description;

    using FullyTypedExample.WebApi.Responses;

    /// <summary>
    /// The departments API endpoint.
    /// </summary>
    public class DepartmentsController : ApiController
    {
        /// <summary>
        /// Gets all departments.
        /// </summary>
        /// <remarks>
        /// Gets the list of all departments.
        /// </remarks>
        /// <returns>
        /// The list of departments.
        /// </returns>
        [Route("api/departments")]
        [HttpGet]
        [ResponseType(typeof(DepartmentsResponse))]
        public DataSet GetDepartments()
        {
            var dataTable = new DataTable("Departments");

            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));

            dataTable.Rows.Add(1, "IT");
            dataTable.Rows.Add(2, "Sales");

            var dataSet = new DataSet();
            dataSet.Tables.Add(dataTable);

            return dataSet;
        }
    }
}