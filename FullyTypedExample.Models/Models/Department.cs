// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Department.cs" company="EastBanc Technologies">
//   Copyright © EastBanc Technologies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FullyTypedExample.Models
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents the department.
    /// </summary>
    public class Department
    {
        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the department name.
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
