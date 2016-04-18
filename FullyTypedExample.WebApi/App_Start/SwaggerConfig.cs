// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwaggerConfig.cs" company="EastBanc Technologies">
//   Copyright © EastBanc Technologies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FullyTypedExample.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Web.Http;

    using FullyTypedExample.Models;
    using FullyTypedExample.WebApi.Controllers;

    using Swashbuckle.Application;

    /// <summary>
    /// The Swagger configuration.
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// Registers Swagger.
        /// </summary>
        public static void Register()
        {
            GlobalConfiguration.Configuration 
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "FullyTypedExample.WebApi");
                        c.IncludeXmlComments(GetXmlCommentsPathForControllers());
                        c.IncludeXmlComments(GetXmlCommentsPathForModels());
                        c.GroupActionsBy(apiDescription => apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName);
                        c.OrderActionGroupsBy(Comparer<string>.Default);
                        c.PrettyPrint();
                    })
                .EnableSwaggerUi(c =>
                    {
                    });
        }

        /// <summary>
        /// Gets xml comments path for controllers.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetXmlCommentsPathForControllers()
        {
            return GetXmlCommentsPath(typeof(EmployeesController));
        }

        /// <summary>
        /// Gets xml comments path for models.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetXmlCommentsPathForModels()
        {
            return GetXmlCommentsPath(typeof(Employee));
        }

        /// <summary>
        /// Gets xml comments path from the assembly for specified type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetXmlCommentsPath(Type type)
        {
            AssemblyName assemblyName = Assembly.GetAssembly(type).GetName();
            string directory = Path.GetDirectoryName(assemblyName.CodeBase);
            Debug.Assert(directory != null, "directory != null");

            string result = Path.Combine(directory, assemblyName.Name + ".XML");
            return result;
        }
    }
}
