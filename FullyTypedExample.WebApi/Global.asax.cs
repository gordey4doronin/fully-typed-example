// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="EastBanc Technologies">
//   Copyright © EastBanc Technologies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FullyTypedExample.WebApi
{
    using System.Web.Http;

    /// <summary>
    /// The Web API application.
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// The application start handler.
        /// </summary>
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
