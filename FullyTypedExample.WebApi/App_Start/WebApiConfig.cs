// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiConfig.cs" company="EastBanc Technologies">
//   Copyright © EastBanc Technologies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FullyTypedExample.WebApi
{
    using System.Web.Http;
    using System.Web.Http.Cors;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

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
            // Enable CORS
            config.EnableCors(new EnableCorsAttribute("*", "*", "*"));

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            // Configure JSON serializer
            JsonSerializerSettings settings = config.Formatters.JsonFormatter.SerializerSettings;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Formatting = Formatting.Indented;
        }
    }
}
