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
    public static class SwaggerConfig
    {
        /// <summary>
        /// Registers Swagger using <see cref="GlobalConfiguration.Configuration"/>.
        /// </summary>
        public static void RegisterGlobal()
        {
            Register(GlobalConfiguration.Configuration);
        }

        /// <summary>
        /// Registers Swagger using provided <see cref="HttpConfiguration"/>.
        /// </summary>
        /// <param name="httpConfiguration">
        /// The http configuration.
        /// </param>
        public static void Register(HttpConfiguration httpConfiguration)
        {
            httpConfiguration
                .EnableSwagger(ConfigureSwagger)
                .EnableSwaggerUi(ConfigureSwaggerUi);
        }

        /// <summary>
        /// Configures Swagger.
        /// </summary>
        /// <param name="config">
        /// The Swagger configuration.
        /// </param>
        public static void ConfigureSwagger(SwaggerDocsConfig config)
        {
            config.SingleApiVersion("v1", "FullyTypedExample.WebApi");
            config.IncludeXmlComments(GetXmlCommentsPathForControllers());
            config.IncludeXmlComments(GetXmlCommentsPathForModels());
            config.GroupActionsBy(apiDescription => apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName);
            config.OrderActionGroupsBy(Comparer<string>.Default);
            config.PrettyPrint();
        }

        /// <summary>
        /// Configures Swagger UI.
        /// </summary>
        /// <param name="config">
        /// The Swagger UI configuration.
        /// </param>
        public static void ConfigureSwaggerUi(SwaggerUiConfig config)
        {
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
