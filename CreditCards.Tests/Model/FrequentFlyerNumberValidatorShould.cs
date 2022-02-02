using CreditCards.Core.Model;
using System;
using Xunit;

namespace CreditCards.Tests.Model
{
    public class FrequentFlyerNumberValidatorShould
    {
        [Theory]
        [InlineData("012345-A")]
        [InlineData("012345-Q")]
        [InlineData("012345-Y")]
        public void AcceptValidSchemes(string number)
        {
            var sut = new FrequentFlyerNumberValidator();

            Assert.True(sut.IsValid(number));
        }

        [Theory]
        [InlineData("012345-Z")]
        [InlineData("012345-X")]
        [InlineData("012345-C")]
        [InlineData("012345-1")]
        [InlineData("012345- ")]
        [InlineData("012345-a")]
        [InlineData("012345-q")]
        [InlineData("012345-y")]
        public void RejectInvalidSchemes(string number)
        {
            var sut = new FrequentFlyerNumberValidator();

            Assert.False(sut.IsValid(number));
        }

        [Theory]
        [InlineData("001X345-A")]
        [InlineData("01234-A")]
        [InlineData("X12345-A")]
        [InlineData("01234X-A")]
        public void RejectInvalidMembers(string number)
        {
            var sut = new FrequentFlyerNumberValidator();

            Assert.False(sut.IsValid(number));
        }

        [Theory]
        [InlineData("      -A")]
        [InlineData("0  1  -A")]
        [InlineData("X1  45-A")]
        [InlineData("     X-A")]
        public void RejectEmptyMemberNumberDigits(string number)
        {
            var sut = new FrequentFlyerNumberValidator();

            Assert.False(sut.IsValid(number));
        }

        [Theory]
        [InlineData("       ")]
        [InlineData("")]
        public void RejectEmptyFrequentFlyerNumber(string number)
        {
            var sut = new FrequentFlyerNumberValidator();

            Assert.False(sut.IsValid(number));
        }

        [Fact]
        public void ThrowExceptionWhenFrequentFlyerNumberIsNull()
        {
            var sut = new FrequentFlyerNumberValidator();

            Assert.Throws<ArgumentNullException>(() => 
            {
                sut.IsValid(null);
            });
        }
    }
}
