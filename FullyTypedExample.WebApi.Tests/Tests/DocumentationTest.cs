// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentationTest.cs" company="EastBanc Technologies">
//   Copyright © EastBanc Technologies. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FullyTypedExample.WebApi.Tests
{
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    using FluentAssertions;

    using FullyTypedExample.WebApi.SelfHosted;

    using Microsoft.Owin.Testing;
    using Microsoft.Rest.Generator;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DocumentationTest
    {
        /// <summary>
        /// The test server to be disposed in <see cref="TestCleanup"/>.
        /// </summary>
        private TestServer testServer;

        /// <summary>
        /// Initializes required things such as <see cref="testServer"/> before the test run.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            this.testServer = TestServer.Create<WebApiHostStartup>();
        }

        /// <summary>
        /// Cleans required things such as <see cref="testServer"/> after the test run.
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            if (this.testServer != null)
            {
                this.testServer.Dispose();
            }
        }

        // https://github.com/domaindrivendev/Swashbuckle/issues/559
        [TestMethod]
        public async Task Swagger_Should_Have_Valid_Swagger_Docs_With_Api_Endpoints()
        {
            // Arrange
            const string RequestUri = "swagger/docs/v1";

            // Act
            HttpResponseMessage response = await this.testServer.CreateRequest(RequestUri).GetAsync();
            string result = await response.Content.ReadAsStringAsync();

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
            result.Should().Contain("swagger", "because should be valid swagger schema");
            result.Should().Contain("\"title\": \"FullyTypedExample.WebApi\"", "because should have valid API name");
            result.Should().Contain("api/employees", "because there should be employees API endpoint");
        }

        [TestMethod]
        public async Task Swagger_Should_Have_Generated_Docs_Be_Equal_To_Build_Docs()
        {
            // Arrange
            const string RequestUri = "swagger/docs/v1";
            string swaggerJson = NormalizeLineEndings(File.ReadAllText("swagger.json"));

            // Act
            HttpResponseMessage response = await this.testServer.CreateRequest(RequestUri).GetAsync();
            string result = await response.Content.ReadAsStringAsync();

            // Assert
            result.Should().NotBeNullOrWhiteSpace("because swagger should generate something");
            result.ShouldBeEquivalentTo(swaggerJson, "because generated swagger docs should be equivalent to build-side swagger docs");
        }

        [TestMethod]
        public void AutoRest_Should_Have_Generated_Definitions_Be_Equal_To_Client_Definitions()
        {
            // Arrange
            string modelsDefinitions = NormalizeLineEndings(File.ReadAllText("models.d.ts"));
            var settings = new Settings { CodeGenerator = "NodeJS", Input = "swagger.json" };

            // Act
            AutoRest.Generate(settings);
            string result = NormalizeLineEndings(File.ReadAllText("Generated\\models\\index.d.ts"));

            // Assert
            result.Should().NotBeNullOrWhiteSpace("because autorest should generate something");
            result.ShouldBeEquivalentTo(modelsDefinitions, "because generated models definitions should be equivalent to client-side models definitions");
        }

        /// <summary>
        /// Normalizes line endings.
        /// </summary>
        /// <remarks>
        /// There is a problem on the build machine.
        /// It reads the file from git with LF instead of CRLF.
        /// So replacing hack is required.
        /// </remarks>
        /// <param name="source">
        /// The source to normalize.
        /// </param>
        /// <returns>
        /// The normalized string.
        /// </returns>
        private static string NormalizeLineEndings(string source)
        {
            return source.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
        }
    }
}
