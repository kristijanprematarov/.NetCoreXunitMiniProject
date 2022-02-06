using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using Xunit;

namespace CreditCards.UITests
{
    public class CreditCardApplicationTests : IDisposable
    {
        private readonly IWebDriver _driver;

        public CreditCardApplicationTests()
        {
            _driver = new EdgeDriver();
        }

        [Fact]
        public void ShouldLoadApplicationPage_SmokeTest()
        {
            _driver.Navigate().GoToUrl("http://localhost:44108/Apply");

            Assert.Equal("Credit Card Application - CreditCards", _driver.Title);
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}
