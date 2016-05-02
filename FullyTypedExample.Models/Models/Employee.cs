// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Employee.cs" company="EastBanc Technologies">
//   Copyright © EastBanc Technologies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FullyTypedExample.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents the employee.
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the employee name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the employee first name.
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Required]
        // ReSharper disable once InconsistentNaming
        public string LAST_NAME { get; set; }
    }
}
