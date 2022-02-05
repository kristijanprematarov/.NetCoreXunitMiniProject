using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Xunit;
using System.Collections.Generic;

namespace CreditCards.IntegrationTests
{
    public class CreditCardApplicationsShould
    {
        [Fact]
        public async Task RenderApplicationForm()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(@"C:\Users\Kristijan\source\repos\.NET Core MVC Testing Fundamentals\src\CreditCards")
                .UseEnvironment("DEVELOPMENT")
                .UseStartup<CreditCards.Startup>()
                .UseApplicationInsights();

            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/Apply");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Contains("New Credit Card Application", responseString);
        }

        [Fact]
        public async Task NotAcceptPostedApplicationDetailsWithMissingFrequentFlyerNumber()
        {
            var builder = new WebHostBuilder()
                .UseContentRoot(@"C:\Users\Kristijan\source\repos\.NET Core MVC Testing Fundamentals\src\CreditCards")
                .UseEnvironment("DEVELOPMENT")
                .UseStartup<CreditCards.Startup>()
                .UseApplicationInsights();

            var server = new TestServer(builder);

            var client = server.CreateClient();

            HttpRequestMessage postRequest =
                new HttpRequestMessage(HttpMethod.Post, "/Apply");

            var formData = new Dictionary<string, string>()
            {
                {"FirstName","Sarah" },
                {"LastName","Smith" },
                {"Age","18" },
                {"GrossAnnualIncome","100000" },
                //FREQUENT FLYER NUMBER NOT ADDED a.k.a. MISSING
            };

            postRequest.Content = new FormUrlEncodedContent(formData);

            HttpResponseMessage responseFromPost = await client.SendAsync(postRequest);

            responseFromPost.EnsureSuccessStatusCode();

            var responseString = await responseFromPost.Content.ReadAsStringAsync();

            Assert.Contains("Please provide a frequent flyer number", responseString);
        }
    }
}
