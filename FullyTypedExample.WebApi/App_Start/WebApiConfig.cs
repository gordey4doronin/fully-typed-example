// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiConfig.cs" company="EastBanc Technologies">
//   Copyright © EastBanc Technologies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FullyTypedExample.WebApi
{
    using System.Web.Http;

    /// <summary>
    /// The Web API configuration.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers Web API.
        /// </summary>
        /// <param name="config">
        /// The http configuration.
        /// </param>
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });
        }
    }
}
