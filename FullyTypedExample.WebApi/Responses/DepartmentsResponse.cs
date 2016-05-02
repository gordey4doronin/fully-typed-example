// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DepartmentsResponse.cs" company="EastBanc Technologies">
//   Copyright © EastBanc Technologies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FullyTypedExample.WebApi.Responses
{
    using System.ComponentModel.DataAnnotations;

    using FullyTypedExample.Models;

    /// <summary>
    /// Represents the response containing the list of departments.
    /// </summary>
    public class DepartmentsResponse
    {
        /// <summary>
        /// Gets or sets the list of departments.
        /// </summary>
        [Required]
        public Department[] Departments { get; set; }
    }
}