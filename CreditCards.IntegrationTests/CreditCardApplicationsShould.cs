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
        private const string AntiForgeryFieldName = "_AFTField";
        private const string AntiForgeryCookieName = "AFTCookie";

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
                .ConfigureServices(x =>
                {
                    //this makes sure that the AFT will be with the same name everytime
                    x.AddAntiforgery(token =>
                    {
                        token.CookieName = AntiForgeryCookieName;
                        token.FormFieldName = AntiForgeryFieldName;
                    });
                })
                .UseApplicationInsights();

            var server = new TestServer(builder);

            var client = server.CreateClient();

            HttpResponseMessage initialResponse = await client.GetAsync("/Apply");

            string antiForgeryCookieValue =
                ExtractAntiForgeryCookieValueFrom(initialResponse);

            string antiForgeryToken =
                ExtractAntiForgeryToken(await initialResponse.Content.ReadAsStringAsync());

            HttpRequestMessage postRequest =
                new HttpRequestMessage(HttpMethod.Post, "/Apply");

            postRequest.Headers.Add("Cookie",
                new CookieHeaderValue(AntiForgeryCookieName,antiForgeryCookieValue).ToString());

            var formData = new Dictionary<string, string>()
            {
                {AntiForgeryFieldName, antiForgeryToken },
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

        private string ExtractAntiForgeryToken(string htmlBody)
        {
            var requestVerificationTokenMatch =
                Regex.Match(htmlBody, $@"\<input name=""{AntiForgeryFieldName}"" type=""hidden"" value=""([^""]+)"" \/\>");

            if (requestVerificationTokenMatch.Success)
            {
                return requestVerificationTokenMatch.Groups[1].Captures[0].Value;
            }

            throw new ArgumentException($"Anti forgery token '{AntiForgeryFieldName}' not found in htmlBody", nameof(htmlBody));
        }

        private string ExtractAntiForgeryCookieValueFrom(HttpResponseMessage response)
        {
            string antiForgeryCookie = response.Headers.GetValues("Set-Cookie")
                .FirstOrDefault(x => x.Contains(AntiForgeryCookieName));

            if (antiForgeryCookie is null)
            {
                throw new ArgumentException(
                    $"Cookie '{AntiForgeryCookieName}' not found in HTTP response",
                    nameof(response));
            }

            string antiForgeryCookieValue =
                SetCookieHeaderValue.Parse(antiForgeryCookie).Value;

            return antiForgeryCookieValue;
        }
    }
}
