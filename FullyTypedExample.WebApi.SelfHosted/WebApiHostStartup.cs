// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiHostStartup.cs" company="EastBanc Technologies">
//   Copyright © EastBanc Technologies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FullyTypedExample.WebApi.SelfHosted
{
    using System.Web.Http;

    using Owin;

    /// <summary>
    /// The Web API host startup.
    /// Represents the code required to startup the OWIN host.
    /// </summary>
    public class WebApiHostStartup
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        /// <param name="appBuilder">
        /// The app builder.
        /// </param>
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host.
            var config = new HttpConfiguration();

            WebApiConfig.Register(config);
            SwaggerConfig.Register(config);

            appBuilder.UseWebApi(config);
        }
    }
}
