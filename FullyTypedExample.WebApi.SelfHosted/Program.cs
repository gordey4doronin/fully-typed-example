// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="EastBanc Technologies">
//   Copyright © EastBanc Technologies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FullyTypedExample.WebApi.SelfHosted
{
    using System;
    using System.IO;
    using System.Net.Http;

    using Microsoft.Owin.Testing;

    /// <summary>
    /// The main program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main program entry point.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            string arg = args.Length == 0 ? "" : args[0];

            switch (arg)
            {
                case "--swagger":
                    string filePath = args.Length > 1 ? args[1] : "swagger.json";
                    GenerateSwaggerJson(filePath);
                    return;

                default:
                    throw new InvalidOperationException("No parameters. Nothing to do.");
            }
        }

        /// <summary>
        /// Generate Swagger JSON document.
        /// </summary>
        /// <param name="filePath">
        /// The file path where to write the generated document.
        /// </param>
        private static void GenerateSwaggerJson(string filePath)
        {
            // Start OWIN host
            using (TestServer server = TestServer.Create<WebApiHostStartup>())
            {
                HttpResponseMessage response = server.CreateRequest("/swagger/docs/v1").GetAsync().Result;

                string result = response.Content.ReadAsStringAsync().Result;
                string path = Path.GetFullPath(filePath);

                File.WriteAllText(path, result);
            }
        }
    }
}
