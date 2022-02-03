using Xunit;
using Moq;
using System;
using CreditCards.Controllers;
using CreditCards.Core.Model;
using CreditCards.Core.Interfaces;
using CreditCards.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CreditCards.Tests.Controller
{
    public class ApplyControllerShould
    {
        private readonly Mock<ICreditCardApplicationRepository> _mockRepository;
        private readonly ApplyController _sut;
        public ApplyControllerShould()
        {
            _mockRepository = new Mock<ICreditCardApplicationRepository>();
            _sut = new ApplyController(_mockRepository.Object);
        }

        [Fact]
        public void ReturnViewForIndex()
        {
            IActionResult result = _sut.Index();

            Assert.IsType<ViewResult>(result);
        }
    }
}
