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
    }
}
