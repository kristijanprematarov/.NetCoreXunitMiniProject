﻿using CreditCards.Core.Model;
using System;
using Xunit;
using Moq;
using CreditCards.Core.Interfaces;

namespace CreditCards.Tests.Model
{
    public class CreditCardApplicationEvaluatorShould
    {
        private const int ExpectedLowIncomeThreshhold = 20_000;
        private const int ExpectedHighIncomeThreshhold = 100_000;
        private const string ValidFrequentFlyerNumber = "012345-A";

        [Theory]
        [InlineData(ExpectedHighIncomeThreshhold)]
        [InlineData(ExpectedHighIncomeThreshhold + 1)]
        [InlineData(int.MaxValue)]
        public void AcceptAllHighIncomeApplicants(int income)
        {
            var sut = new CreditCardApplicationEvaluator(new FrequentFlyerNumberValidator());

            var application = new CreditCardApplication()
            {
                GrossAnnualIncome = income,
                FrequentFlyerNumber = ValidFrequentFlyerNumber
            };

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted,sut.Evaluate(application));
        }

        [Theory]
        [InlineData(20)]
        [InlineData(19)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        public void ReferYoungApplicantsWhoAreNotHighIncome(int age)
        {
            var sut = new CreditCardApplicationEvaluator(new FrequentFlyerNumberValidator());

            var application = new CreditCardApplication()
            {
                GrossAnnualIncome = ExpectedHighIncomeThreshhold - 1,
                Age = age,
                FrequentFlyerNumber = ValidFrequentFlyerNumber
            };

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, sut.Evaluate(application));
        }

        [Theory]
        [InlineData(ExpectedLowIncomeThreshhold)]
        [InlineData(ExpectedLowIncomeThreshhold + 1)]
        [InlineData(ExpectedHighIncomeThreshhold - 1)]
        public void ReferYoungApplicantsWhoAreMiddleIncome(int income)
        {
            var sut = new CreditCardApplicationEvaluator(new FrequentFlyerNumberValidator());

            var application = new CreditCardApplication()
            {
                GrossAnnualIncome = income,
                Age = 21,
                FrequentFlyerNumber = ValidFrequentFlyerNumber
            };

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, sut.Evaluate(application));
        }

        [Theory]
        [InlineData(ExpectedLowIncomeThreshhold-1)]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        public void DeclineAllAplicantsWhoAreLowIncome(int income)
        {
            var sut = new CreditCardApplicationEvaluator(new FrequentFlyerNumberValidator());

            var application = new CreditCardApplication()
            {
                GrossAnnualIncome = income,
                Age = 21,
                FrequentFlyerNumber = ValidFrequentFlyerNumber
            };

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, sut.Evaluate(application));
        }

        [Fact]
        public void ReferInvalidFrequentFlyerNumbers_RealValidator()
        {
            var sut = new CreditCardApplicationEvaluator(new FrequentFlyerNumberValidator());

            var application = new CreditCardApplication()
            {
                FrequentFlyerNumber = "0dm389dn29"
            };

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, sut.Evaluate(application));
        }

        [Fact]
        public void ReferInvalidFrequentFlyerNumbers_MockValidator()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication();

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, sut.Evaluate(application));
        }
    }
}
